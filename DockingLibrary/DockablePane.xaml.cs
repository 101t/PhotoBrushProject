using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml;

namespace DockingLibrary
{
    /// <summary>
    /// States that a dockable pane can assume
    /// </summary>
    public enum PaneState
    { 
        Hidden,

        AutoHide,

        Docked,

        TabbedDocument,

        FloatingWindow,

        DockableWindow
    }


    /// <summary>
    /// A dockable pane is a resizable and movable window region which can host one or more dockable content
    /// </summary>
    /// <remarks>A dockable pane occupies a window region. It can be in two different states: docked to a border or hosted in a floating window.
    /// When is docked it can be resizes only in a direction. User can switch between pane states using mouse or context menus.
    /// Contents whitin a dockable pane are arranged through a tabcontrol.</remarks>
    partial class DockablePane : Pane
    {
 
        /// <summary>
        /// When created pane is hidden
        /// </summary>
        protected PaneState _state = PaneState.Hidden;

        /// <summary>
        /// Get pane state
        /// </summary>
        public PaneState State
        {
            get
            {
                return _state;
            }
        }

        /// <summary>
        /// Show/hide pane header (title, buttons etc...)
        /// </summary>
        public bool ShowHeader
        {
            get 
            {
                return PaneHeader.Visibility == Visibility.Visible;
            }
            set 
            {
                if (value)
                    PaneHeader.Visibility = Visibility.Visible;
                else
                    PaneHeader.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Current docking border
        /// </summary>
        Dock _dock = Dock.Right;

        /// <summary>
        /// Current docking border
        /// </summary>
        public Dock Dock
        {
            get
            {
                return _dock;
            }
        }


        public DockablePane(DockManager dockManager) : this(dockManager, Dock.Right) { }
        public DockablePane(DockManager dockManager, Dock initialDock) : base(dockManager)
        {
            _dock = initialDock;
            InitializeComponent();

            //this.GotFocus += new RoutedEventHandler(item_GotFocus);

        }

        /// <summary>
        /// Active visible content
        /// </summary>
        public override DockableContent ActiveContent
        {
            get 
            {
                if (VisibleContents.Count == 1)
                    return VisibleContents[0];
                else if (VisibleContents.Count > 1)
                    return VisibleContents[tbcContents.SelectedIndex];

                return null;
            }
            set
            {
                if (VisibleContents.Count > 1)
                {
                    tbcContents.SelectedIndex = VisibleContents.IndexOf(value);
                }
            }
        }
        /// <summary>
        /// Event raised when title is changed
        /// </summary>
        public event EventHandler OnTitleChanged;

        /// <summary>
        /// Change pane's title and fires OnTitleChanged event
        /// </summary>
        public override void RefreshTitle()
        {
            if (ActiveContent != null)
            {
                tbTitle.Text = Title;

                if (tbcContents.Items.Count > 0)
                {
                    SetTabItemHeader(
                        tbcContents.Items[VisibleContents.IndexOf(ActiveContent)] as TabItem,
                        ActiveContent);
                }
                if (OnTitleChanged != null)
                    OnTitleChanged(this, new EventArgs());
            }
        }


        /// <summary>
        /// Get pane title
        /// </summary>
        public virtual string Title
        {
            get 
            {
                if (ActiveContent != null)
                    return ActiveContent.Title;

                return null;
            }
        }
        /// <summary>
        /// Get visible contents
        /// </summary>
        public readonly List<DockableContent> VisibleContents = new List<DockableContent>();

        /// <summary>
        /// Add a dockable content to Contents list
        /// </summary>
        /// <param name="content">Content to add</param>
        /// <remarks>Content is automatically shown.</remarks>
        public override void Add(DockableContent content)
        {
            if (Contents.Count == 0)
            {
                SaveFloatingWindowSizeAndPosition(content);
            }


            base.Add(content);
        }

        /// <summary>
        /// Remove a content from pane Contents list
        /// </summary>
        /// <param name="content">Content to remove</param>
        /// <remarks>Notice that when no more contents are present in a pane, it is automatically removed</remarks>
        public override void Remove(DockableContent content)
        {
            Hide(content);

            base.Remove(content);

            if (Contents.Count == 0)
                DockManager.Remove(this);
        }

        /// <summary>
        /// Show a content previuosly added
        /// </summary>
        /// <param name="content">DockableContent object to show</param>
        public override void Show(DockableContent content)
        {
            AddVisibleContent(content);

            if (VisibleContents.Count == 1 && State == PaneState.Hidden)
            {
                ChangeState(PaneState.Docked);
                DockManager.DragPaneServices.Register(this);
            }


            base.Show(content);
        }

        /// <summary>
        /// Hide a contained dockable content
        /// </summary>
        /// <param name="content">DockableContent object to hide</param>
        /// <remarks>Pane is automatically hidden if no more visible contents are shown</remarks>
        public override void Hide(DockableContent content)
        {
            RemoveVisibleContent(content);

            if (VisibleContents.Count == 0 && State==PaneState.Docked)
                Hide();

            base.Hide(content);
        }

        /// <summary>
        /// Add a visible content
        /// </summary>
        /// <param name="content">DockableContent object to add</param>
        /// <remarks>If more then one contents are visible, this method dinamically creates a tab control and
        /// adds new content to it.</remarks>
        void AddVisibleContent(DockableContent content)
        {
            if (VisibleContents.Contains(content))
                return;

            if (VisibleContents.Count == 0)
            {
                VisibleContents.Add(content);
                ShowSingleContent(content);
            }
            else if (VisibleContents.Count == 1)
            {
                HideSingleContent(VisibleContents[0]);
                AddItem(VisibleContents[0]);
                VisibleContents.Add(content);
                AddItem(content);
                ShowTabs();
            }
            else
            {
                VisibleContents.Add(content);
                AddItem(content);
            }
        }

        /// <summary>
        /// Remove a visible content from pane
        /// </summary>
        /// <param name="content">DockableContent object to remove</param>
        /// <remarks>Remove related tab item from contents tab control. if only one content is visible than hide tab control.</remarks>
        void RemoveVisibleContent(DockableContent content)
        {
            if (!VisibleContents.Contains(content))
                return;

            if (VisibleContents.Count == 1)
            {
                VisibleContents.Remove(content);
                HideSingleContent(content);
                HideTabs();
            }
            else if (VisibleContents.Count == 2)
            {
                RemoveItem(VisibleContents[0]);
                RemoveItem(VisibleContents[1]);
                VisibleContents.Remove(content);
                ShowSingleContent(VisibleContents[0]);
                HideTabs();
            }
            else
            {
                VisibleContents.Remove(content);
                RemoveItem(content);
            }
        }

        /// <summary>
        /// Close a dockable content
        /// </summary>
        /// <param name="content">DockableContent object to close</param>
        /// <remarks>In this library version this method simply hide the content</remarks>
        public override void Close(DockableContent content)
        {
            Hide(content);
        }

        #region Contents management
        private bool IsSingleContentVisible
        {
            get { return cpClientWindowContent.Visibility == Visibility.Visible; }
        }

        void ShowSingleContent(DockableContent content)
        {
            cpClientWindowContent.Content = content.Content;
            cpClientWindowContent.Visibility = Visibility.Visible;
            RefreshTitle();
        }

        void HideSingleContent(DockableContent content)
        {
            cpClientWindowContent.Content = null; 
            cpClientWindowContent.Visibility = Visibility.Collapsed;
        }


        private bool IsContentsTbcVisible
        {
            get { return tbcContents.Visibility == Visibility.Visible; }
        }

        protected void ShowTabs()
        {
            tbcContents.SelectionChanged += new SelectionChangedEventHandler(_tabs_SelectionChanged);
            tbcContents.Visibility = Visibility.Visible;
        }

        protected void HideTabs()
        {
            tbcContents.SelectionChanged -= new SelectionChangedEventHandler(_tabs_SelectionChanged);
            tbcContents.Visibility = Visibility.Collapsed;
        }

        void SetTabItemHeader(TabItem item, DockableContent content)
        {
            StackPanel spHeader = new StackPanel();
            spHeader.Orientation = Orientation.Horizontal;
            Image iconContent = new Image();
            iconContent.Source = content.Icon;
            spHeader.Children.Add(iconContent);
            TextBlock titleContent = new TextBlock();
            titleContent.Text = content.Title;
            titleContent.Margin = new Thickness(2, 0, 0, 0);
            spHeader.Children.Add(titleContent);
            item.Header = spHeader;
        }

        protected virtual void AddItem(DockableContent content)
        {
            TabItem item = new TabItem();
            DockPanel tabPanel = new DockPanel();

            //SetTabItemHeader(item, content);

            item.Style = FindResource("DockablePaneTabItemStyle") as Style;
            item.Content = new ContentPresenter();
            (item.Content as ContentPresenter).Content = content.Content;

            //item.PreviewMouseDown += new MouseButtonEventHandler(OnTabItemMouseDown);
            //item.MouseMove += new MouseEventHandler(OnTabItemMouseMove);
            //item.MouseUp += new MouseButtonEventHandler(OnTabItemMouseUp);
            //item.Focusable = true;
            //item.GotFocus += new RoutedEventHandler(item_GotFocus);
            tbcContents.Items.Add(item);
            tbcContents.SelectedItem = item;

            RefreshTitle();
        }

        void item_GotFocus(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("GotFocus");
        }

        protected virtual void RemoveItem(DockableContent content)
        {
            foreach (TabItem item in tbcContents.Items)
                if ((item.Content as ContentPresenter).Content == content.Content)
                {
                    //item.PreviewMouseDown -= new MouseButtonEventHandler(OnTabItemMouseDown);
                    //item.MouseMove -= new MouseEventHandler(OnTabItemMouseMove);
                    //item.MouseUp -= new MouseButtonEventHandler(OnTabItemMouseUp);

                    item.Content = null;
                    tbcContents.Items.Remove(item);
                    //ChangeTitle();
                    break;
                }
        }


        void _tabs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (tbcContents.SelectedIndex >= 0)
                RefreshTitle();
        }

        #endregion

        //List<EventHandler> _list = new List<EventHandler>();
        
        ///// <summary>
        ///// Event fired when pane internal state is changed
        ///// </summary>
        //public event EventHandler OnStateChanged
        //{
        //    add { _list.Add(value); }
        //    remove { _list.Remove(value); }
        //}

        public EventHandler OnStateChanged;

        /// <summary>
        /// Fires OnStateChanged event
        /// </summary>
        private void FireOnOnStateChanged()
        {
            if (OnStateChanged != null)
                OnStateChanged(this, EventArgs.Empty);
            //foreach (EventHandler eh in _list)
            //    eh(this, EventArgs.Empty);
            
        }
        
        /// <summary>
        /// Change pane internal state
        /// </summary>
        /// <param name="newState">New pane state</param>
        /// <remarks>OnStateChanged event is raised only if newState is different from State.</remarks>
        internal void ChangeState(PaneState newState)
        {
            if (State != newState)
            {
                SaveSize();

                _lastState = _state;
                _state = newState;

                FireOnOnStateChanged();
            }
        }


        /// <summary>
        /// Return true if pane is hidden, ie State is different from PaneState.Docked
        /// </summary>
        public override bool IsHidden
        {
            get
            {
                return State != PaneState.Docked;
            }
        }

        /// <summary>
        /// Internal last pane state
        /// </summary>
        PaneState _lastState = PaneState.Docked;

        /// <summary>
        /// Show this pane and all contained contents
        /// </summary>
        public override void Show()
        {
            foreach (DockableContent content in Contents)
                Show(content);

            if (State == PaneState.AutoHide || State == PaneState.Hidden)
                ChangeState(PaneState.Docked);

            base.Show();
        }

        /// <summary>
        /// Hide this pane and all contained contents
        /// </summary>
        public override void Hide()
        {
            foreach (DockableContent content in Contents)
                RemoveVisibleContent(content);

            ChangeState(PaneState.Hidden);
            base.Hide();
        }

        /// <summary>
        /// Close this pane
        /// </summary>
        /// <remarks>Consider that in this version library this method simply hides the pane.</remarks>
        public override  void Close()
        {
            Hide();

            base.Close();
        }

        /// <summary>
        /// Create and show a floating window hosting this pane
        /// </summary>
        public virtual void FloatingWindow()
        {
            ChangeState(PaneState.FloatingWindow); 
            

            FloatingWindow wnd = new FloatingWindow(this);
            SetFloatingWindowSizeAndPosition(wnd);

            wnd.Owner = DockManager.ParentWindow;
            wnd.Show();
        }

        /// <summary>
        /// Create and show a dockable window hosting this pane
        /// </summary>
        public virtual void DockableWindow()
        {
            FloatingWindow wnd = new FloatingWindow(this);
            SetFloatingWindowSizeAndPosition(wnd);

            ChangeState(PaneState.DockableWindow);

            wnd.Owner = DockManager.ParentWindow;
            wnd.Show();
        }

        /// <summary>
        /// Show contained contents as documents and close this pane
        /// </summary>
        public virtual void TabbedDocument()
        {
            while (Contents.Count > 0)
            {
                DockableContent contentToRemove = Contents[0];
                Remove(contentToRemove);
                DockManager.AddDocument(contentToRemove);
            }

            ChangeState(PaneState.TabbedDocument);
        }

        /// <summary>
        /// Dock this pane to a destination pane border
        /// </summary>
        /// <param name="destinationPane"></param>
        /// <param name="relativeDock"></param>
        internal void MoveTo(Pane destinationPane, Dock relativeDock)
        {
            DockablePane dockableDestPane = destinationPane as DockablePane;
            if (dockableDestPane != null)
                ChangeDock(dockableDestPane.Dock);
            else
                ChangeDock(relativeDock);
            
            
            DockManager.MoveTo(this, destinationPane, relativeDock);
            ChangeState(PaneState.Docked);
            //Show();
            //ChangeState(PaneState.Docked);
        }

        /// <summary>
        /// Move contained contents into a destination pane and close this one
        /// </summary>
        /// <param name="destinationPane"></param>
        internal void MoveInto(Pane destinationPane)
        {
            //DockablePane dockableDestPane = destinationPane as DockablePane;
            //if (dockableDestPane != null)
            //    ChangeDock(dockableDestPane.Dock);

            DockManager.MoveInto(this, destinationPane);

            //if (destinationPane is DocumentsPane)
            //    ChangeState(PaneState.TabbedDocument);
            //else
            //    ChangeState(PaneState.Docked);
        }

        /// <summary>
        /// Event raised when Dock property is changed
        /// </summary>
        public event EventHandler OnDockChanged;

        /// <summary>
        /// Fires OnDockChanged
        /// </summary>
        private void FireOnOnDockChanged()
        {
            if (OnDockChanged != null)
                OnDockChanged(this, EventArgs.Empty);
        }
        
        /// <summary>
        /// Change dock border
        /// </summary>
        /// <param name="dock">New dock border</param>
        public void ChangeDock(Dock dock)
        {
            //if (dock != _dock)
            {
                //SaveSize();

                _dock = dock;

                FireOnOnDockChanged();

                ChangeState(PaneState.Docked);
                //Show();
            }
            
        }

        /// <summary>
        /// Auto-hide this pane 
        /// </summary>
        public void AutoHide()
        {
            foreach (DockableContent content in Contents)
                RemoveVisibleContent(content);

            ChangeState(PaneState.AutoHide);

            DockManager.DragPaneServices.Unregister(this);
        }

        /// <summary>
        /// Handles effective pane resizing 
        /// </summary>
        /// <param name="sizeInfo"></param>
        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            //SaveSize();
            base.OnRenderSizeChanged(sizeInfo);
        }


        /// <summary>
        /// Save current pane size
        /// </summary>
        public override void SaveSize()
        {
            if (IsHidden)
                return;

            if (Dock == Dock.Left || Dock == Dock.Right)
                PaneWidth = ActualWidth > 150 ? ActualWidth : 150;
            else
                PaneHeight = ActualHeight > 150 ? ActualHeight : 150;

            base.SaveSize();
        }


        Point ptFloatingWindow = new Point(0,0);
        Size sizeFloatingWindow = new Size(300, 300);

        internal void SaveFloatingWindowSizeAndPosition(Window fw)
        {
            if (!double.IsNaN(fw.Left) && !double.IsNaN(fw.Top))
                ptFloatingWindow = new Point(fw.Left, fw.Top);
            
            if (!double.IsNaN(fw.Width) && !double.IsNaN(fw.Height))
                sizeFloatingWindow = new Size(fw.Width, fw.Height);
        }

        internal void SetFloatingWindowSizeAndPosition(FloatingWindow fw)
        {
            fw.Left = ptFloatingWindow.X;
            fw.Top = ptFloatingWindow.Y;
            fw.Width = sizeFloatingWindow.Width;
            fw.Height = sizeFloatingWindow.Height;
        }

        /// <summary>
        /// Get swith options context menu
        /// </summary>
        internal ContextMenu OptionsMenu
        {
            get 
            {
                return btnMenu.ContextMenu;
            }
        }

        /// <summary>
        /// Handles user click on OptionsMenu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        protected virtual void OnDockingMenu(object sender, EventArgs e)
        {
            if (sender == menuTabbedDocument)
                TabbedDocument();
            if (sender == menuFloatingWindow)
                FloatingWindow();
            if (sender == menuDockedWindow)
                DockableWindow();


            if (sender == menuAutoHide)
            {
                if (menuAutoHide.IsChecked)
                    ChangeState(PaneState.Docked);
                else
                    AutoHide();
            }
            if (sender == menuClose)
            {
                Close(ActiveContent);
            }
        }
        /// <summary>
        /// Show switch options menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnBtnMenuMouseDown(object sender, RoutedEventArgs e)
        {
            btnMenu.ContextMenu.IsOpen = true;
            e.Handled = true;
        }

        /// <summary>
        /// Handles user click event on auto-hide button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnBtnAutoHideMouseDown(object sender, RoutedEventArgs e)
        {
            if (State == PaneState.AutoHide)
                ChangeState(PaneState.Docked);
            else
                AutoHide();

            e.Handled = true;
        }

        /// <summary>
        /// Handles user click event on close button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnBtnCloseMouseDown(object sender, RoutedEventArgs e)
        {
            Close(ActiveContent);
            e.Handled = true;
        }
 
        /// <summary>
        /// Enable/disable switch options menu items
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnBtnMenuPopup(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (btnMenu.ContextMenu.IsOpen)
            {
                menuFloatingWindow.IsEnabled = _state != PaneState.AutoHide && _state != PaneState.Hidden;
                menuFloatingWindow.IsChecked = _state == PaneState.FloatingWindow;
                menuDockedWindow.IsEnabled = _state != PaneState.AutoHide && _state != PaneState.Hidden;
                menuDockedWindow.IsChecked = _state == PaneState.Docked || _state == PaneState.DockableWindow;
                menuTabbedDocument.IsEnabled = _state != PaneState.AutoHide && _state != PaneState.Hidden;
                menuAutoHide.IsChecked = _state == PaneState.AutoHide;
            }
        }

        /// <summary>
        /// Drag starting point
        /// </summary>
        Point ptStartDrag;

        /// <summary>
        /// Handles mouse douwn event on pane header
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks>Save current mouse position in ptStartDrag and capture mouse event on PaneHeader object.</remarks>
        void OnHeaderMouseDown(object sender, MouseEventArgs e)
        {
            if (DockManager == null)
                return;

            if (!PaneHeader.IsMouseCaptured)
            {
                ptStartDrag = e.GetPosition(this);
                PaneHeader.CaptureMouse();
            }
        }
        /// <summary>
        /// Handles mouse up event on pane header
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks>Release any mouse capture</remarks>
        void OnHeaderMouseUp(object sender, MouseEventArgs e)
        {
            PaneHeader.ReleaseMouseCapture();
        }

        /// <summary>
        /// Handles mouse move event and eventually starts draging this pane
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnHeaderMouseMove(object sender, MouseEventArgs e)
        {
            if (PaneHeader.IsMouseCaptured && Math.Abs(ptStartDrag.X - e.GetPosition(this).X) > 4)
            {
                PaneHeader.ReleaseMouseCapture();
                DragPane(DockManager.PointToScreen(e.GetPosition(DockManager)), e.GetPosition(PaneHeader));
            }
        }

        /// <summary>
        /// Initiate a dragging operation of this pane, relative DockManager is also involved
        /// </summary>
        /// <param name="startDragPoint"></param>
        /// <param name="offset"></param>
        protected virtual void DragPane(Point startDragPoint, Point offset)
        {
            FloatingWindow wnd = new FloatingWindow(this);
            SetFloatingWindowSizeAndPosition(wnd);

            ChangeState(PaneState.DockableWindow);
            
            DockManager.Drag(wnd, startDragPoint, offset);        
        }


        /// <summary>
        /// Mouse down event on a content tab item
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnTabItemMouseDown(object sender, MouseButtonEventArgs e)
        {
            //TabItem item = sender as TabItem;
            FrameworkElement senderElement = sender as FrameworkElement;
            TabItem item = senderElement.TemplatedParent as TabItem;

            if (!senderElement.IsMouseCaptured)
            {
                ptStartDrag = e.GetPosition(this);
                senderElement.CaptureMouse();
            }

        }

        /// <summary>
        /// Mouse move event on a content tab item
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks>If mouse is moved when left button is pressed than this method starts a dragging content operations. Also in this case relative DockManager is involved.</remarks>
        void OnTabItemMouseMove(object sender, MouseEventArgs e)
        {
            //TabItem item = sender as TabItem;
            FrameworkElement senderElement = sender as FrameworkElement;
            TabItem item = senderElement.TemplatedParent as TabItem;

            if (senderElement.IsMouseCaptured && Math.Abs(ptStartDrag.X - e.GetPosition(this).X) > 4)
            {
                senderElement.ReleaseMouseCapture();
                DockableContent contentToDrag = Contents[tbcContents.Items.IndexOf(item)] as DockableContent;
                if (contentToDrag != null)
                    DragContent(contentToDrag, e.GetPosition(DockManager), e.GetPosition(item));
            }
        }

        /// <summary>
        /// Handles MouseUp event fired from a content tab item and eventually release mouse event capture 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnTabItemMouseUp(object sender, MouseButtonEventArgs e)
        {
            //TabItem item = sender as TabItem;
            FrameworkElement senderElement = sender as FrameworkElement;
            TabItem item = senderElement.TemplatedParent as TabItem;

            senderElement.ReleaseMouseCapture();
        }


        #region persistence

        public override void Serialize(XmlDocument doc, XmlNode parentNode)
        {
            SaveSize();

            parentNode.Attributes.Append(doc.CreateAttribute("Dock"));
            parentNode.Attributes["Dock"].Value = _dock.ToString();
            parentNode.Attributes.Append(doc.CreateAttribute("State"));
            parentNode.Attributes["State"].Value = _state.ToString();
            parentNode.Attributes.Append(doc.CreateAttribute("LastState"));
            parentNode.Attributes["LastState"].Value = _lastState.ToString();

            
            parentNode.Attributes.Append(doc.CreateAttribute("ptFloatingWindow"));
            parentNode.Attributes["ptFloatingWindow"].Value = System.ComponentModel.TypeDescriptor.GetConverter(typeof(Point)).ConvertToInvariantString(ptFloatingWindow);
            parentNode.Attributes.Append(doc.CreateAttribute("sizeFloatingWindow"));
            parentNode.Attributes["sizeFloatingWindow"].Value = System.ComponentModel.TypeDescriptor.GetConverter(typeof(Size)).ConvertToInvariantString(sizeFloatingWindow);

            base.Serialize(doc, parentNode);
        }

        public override void Deserialize(DockManager managerToAttach, XmlNode node, GetContentFromTypeString getObjectHandler)
        {
            base.Deserialize(managerToAttach, node, getObjectHandler);

            _dock = (Dock)Enum.Parse(typeof(Dock), node.Attributes["Dock"].Value);
            _state = (PaneState)Enum.Parse(typeof(PaneState), node.Attributes["State"].Value);
            _lastState = (PaneState)Enum.Parse(typeof(PaneState), node.Attributes["LastState"].Value);

            ptFloatingWindow = (Point)System.ComponentModel.TypeDescriptor.GetConverter(typeof(Point)).ConvertFromInvariantString(node.Attributes["ptFloatingWindow"].Value);
            sizeFloatingWindow = (Size)System.ComponentModel.TypeDescriptor.GetConverter(typeof(Size)).ConvertFromInvariantString(node.Attributes["sizeFloatingWindow"].Value);

            

            if (State == PaneState.FloatingWindow)
                FloatingWindow();
            else if (State == PaneState.DockableWindow)
                DockableWindow();

            DockManager.AttachPaneEvents(this);
        }

        #endregion
    }
}