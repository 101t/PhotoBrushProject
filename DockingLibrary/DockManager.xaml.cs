using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;
using System.Xml;

namespace DockingLibrary
{
    /// <summary>
    /// Manages and controls panes layout
    /// </summary>
    /// <remarks>This is the main user control which is usually embedded in a window. DockManager can control
    /// other windows arraging them in panes like VS. Each pane can be docked to a DockManager border, can be shown/hidden or auto-hidden.</remarks>
    public partial class DockManager : System.Windows.Controls.UserControl, IDropSurface
    {
        public DockManager()
        {
            InitializeComponent();
            
            DragPaneServices.Register(this);

            _overlayWindow = new OverlayWindow(this);

            gridDocking.AttachDockManager(this);
            gridDocking.DocumentsPane.Show();// .DockManager = this;
        }

        /// <summary>
        /// List of managed contents (hiddens too)
        /// </summary>
        List<DockableContent> Contents = new List<DockableContent>();

        /// <summary>
        /// Returns a documents list
        /// </summary>
        public DocumentContent[] Documents
        {
            get
            {
                DocumentContent[] docs = new DocumentContent[gridDocking.DocumentsPane.Documents.Count-gridDocking.DocumentsPane.Contents.Count];
                int i = 0;
                foreach (ManagedContent content in gridDocking.DocumentsPane.Documents)
                {
                    if (content is DocumentContent)
                        docs[i++] = content as DocumentContent;
                }

                //gridDocking.DocumentsPane.Documents.CopyTo(docs);
                return docs;
            }
        }

        /// <summary>
        /// Return active document
        /// </summary>
        /// <remarks>If no document is present or a dockable content is active in the Documents pane return null</remarks>
        public DocumentContent ActiveDocument
        {
            get
            {
                return gridDocking.DocumentsPane.ActiveDocument;
            }
        }

        /// <summary>
        /// Add dockable content to layout management
        /// </summary>
        /// <param name="content">Content to add</param>
        /// <returns></returns>
        internal void Add(DockableContent content)
        {
            if (!Contents.Contains(content))
                Contents.Add(content);
        }

        /// <summary>
        /// Add a dockapble to layout management
        /// </summary>
        /// <param name="pane">Pane to manage</param>
        internal void Add(DockablePane pane)
        {
            gridDocking.Add(pane);
            AttachPaneEvents(pane);
        }

        /// <summary>
        /// Remove a dockable pane from layout management
        /// </summary>
        /// <param name="dockablePane">Dockable pane to remove</param>
        /// <remarks>Also pane event handlers are detached</remarks>
        internal void Remove(DockablePane dockablePane)
        { 
             gridDocking.Remove(dockablePane);
             DetachPaneEvents(dockablePane);
        }

        /// <summary>
        /// Remove a dockable content from internal contents list
        /// </summary>
        /// <param name="content"></param>
        internal void Remove(DockableContent content)
        {
            Contents.Remove(content);
        }

        /// <summary>
        /// Add a document content
        /// </summary>
        /// <param name="content">Document content to adde</param>
        /// <returns>Returns DocumentsPane where document is added</returns>
        internal DocumentsPane AddDocument(DocumentContent content)
        {
            System.Diagnostics.Debug.Assert(!gridDocking.DocumentsPane.Documents.Contains(content));
            //gridDocking.DocumentsPane.DockManager = this;
            if (!gridDocking.DocumentsPane.Documents.Contains(content))
            {
                gridDocking.DocumentsPane.Add(content);
                //gridDocking.ArrangeLayout();
            }

            return gridDocking.DocumentsPane;
        }

        /// <summary>
        /// Add a docable content to default documents pane
        /// </summary>
        /// <param name="content">Dockable content to add</param>
        /// <returns>Documents pane where dockable content is added</returns>
        internal DocumentsPane AddDocument(DockableContent content)
        {
            System.Diagnostics.Debug.Assert(!Contents.Contains(content));
            System.Diagnostics.Debug.Assert(!gridDocking.DocumentsPane.Contents.Contains(content));
            
            if (!gridDocking.DocumentsPane.Contents.Contains(content))
            {
                gridDocking.DocumentsPane.Add(content);
                gridDocking.ArrangeLayout();
            }

            return gridDocking.DocumentsPane;
        }

        /// <summary>
        /// Returns currently active documents pane (at the moment this is only one per DockManager control)
        /// </summary>
        /// <returns>The DocumentsPane</returns>
        internal DocumentsPane GetDocumentsPane()
        {
            return gridDocking.DocumentsPane;
        }

        /// <summary>
        /// During unolad process close active contents windows
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnUnloaded(object sender, EventArgs e)
        {
            foreach (DockableContent content in Contents)
                content.Close();
            foreach (DocumentContent docContent in Documents)
                docContent.Close();
            
            _overlayWindow.Close();
        }

        /// <summary>
        /// Attach pane events handler
        /// </summary>
        /// <param name="pane"></param>
        internal void AttachPaneEvents(DockablePane pane)
        {
            pane.OnStateChanged += new EventHandler(pane_OnStateChanged);

            gridDocking.AttachPaneEvents(pane);
        }

        /// <summary>
        /// Detach pane events handler
        /// </summary>
        /// <param name="pane"></param>
        internal void DetachPaneEvents(DockablePane pane)
        {
            pane.OnStateChanged -= new EventHandler(pane_OnStateChanged);

            gridDocking.DetachPaneEvents(pane);
        }

        /// <summary>
        /// List of managed docking button groups currently shown in border stack panels
        /// </summary>
        List<DockingButtonGroup> _dockingBtnGroups = new List<DockingButtonGroup>();

        /// <summary>
        /// Handles pane state changed event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void pane_OnStateChanged(object sender, EventArgs e)
        {
            DockablePane pane = sender as DockablePane;
            if (pane.State == PaneState.AutoHide)
            {
                AddPaneDockingButtons(pane);
                ShowTempPane(false);
                HideTempPane(true);
            }
        }

        /// <summary>
        /// Add a group of docking buttons for a pane docked to a dockingmanager border
        /// </summary>
        /// <param name="pane"></param>
        private void AddPaneDockingButtons(DockablePane pane)
        {
            DockingButtonGroup buttonGroup = new DockingButtonGroup();
            buttonGroup.Dock = pane.Dock;

            foreach (DockableContent content in pane.Contents)
            {
                DockingButton btn = new DockingButton();
                btn.DockableContent = content;
                btn.DockingButtonGroup = buttonGroup;

                if (_currentButton == null)
                    _currentButton = btn;

                buttonGroup.Buttons.Add(btn);
            }

            _dockingBtnGroups.Add(buttonGroup);

            AddDockingButtons(buttonGroup);
        }

        /// <summary>
        /// Add a group of docking buttons to the relative border stack panel
        /// </summary>
        /// <param name="group">Group to add</param>
        private void AddDockingButtons(DockingButtonGroup group)
        {
            foreach (DockingButton btn in group.Buttons)
                btn.MouseEnter += new MouseEventHandler(OnShowAutoHidePane); 
            
            Border br = new Border();
            br.Width = br.Height = 10;
            switch (group.Dock)
            {
                case Dock.Left:
                    foreach (DockingButton btn in group.Buttons)
                    {
                        btn.LayoutTransform = new RotateTransform(90);
                        btnPanelLeft.Children.Add(btn);
                    }
                    btnPanelLeft.Children.Add(br);
                    break;
                case Dock.Right:
                    foreach (DockingButton btn in group.Buttons)
                    {
                        btn.LayoutTransform = new RotateTransform(90);
                        btnPanelRight.Children.Add(btn);
                    }
                    btnPanelRight.Children.Add(br);
                    break;
                case Dock.Top:
                    foreach (DockingButton btn in group.Buttons)
                        btnPanelTop.Children.Add(btn);
                    btnPanelTop.Children.Add(br);
                    break;
                case Dock.Bottom:
                    foreach (DockingButton btn in group.Buttons)
                        btnPanelBottom.Children.Add(btn);
                    btnPanelBottom.Children.Add(br);
                    break;
            }


        }

        /// <summary>
        /// Remove a group of docking buttons from the relative border stack panel
        /// </summary>
        /// <param name="group">Group to remove</param>
        private void RemoveDockingButtons(DockingButtonGroup group)
        {
            foreach (DockingButton btn in group.Buttons)
                btn.MouseEnter -= new MouseEventHandler(OnShowAutoHidePane);

            switch (group.Dock)
            {
                case Dock.Left:
                    btnPanelLeft.Children.RemoveAt(btnPanelLeft.Children.IndexOf(group.Buttons[group.Buttons.Count - 1]) + 1);
                    foreach (DockingButton btn in group.Buttons)
                        btnPanelLeft.Children.Remove(btn);
                    break;
                case Dock.Right:
                    btnPanelRight.Children.RemoveAt(btnPanelRight.Children.IndexOf(group.Buttons[group.Buttons.Count - 1]) + 1);
                    foreach (DockingButton btn in group.Buttons)
                        btnPanelRight.Children.Remove(btn);
                    break;
                case Dock.Top:
                    btnPanelTop.Children.RemoveAt(btnPanelTop.Children.IndexOf(group.Buttons[group.Buttons.Count - 1]) + 1);
                    foreach (DockingButton btn in group.Buttons)
                        btnPanelTop.Children.Remove(btn);
                    break;
                case Dock.Bottom:
                    btnPanelBottom.Children.RemoveAt(btnPanelBottom.Children.IndexOf(group.Buttons[group.Buttons.Count - 1]) + 1);
                    foreach (DockingButton btn in group.Buttons)
                        btnPanelBottom.Children.Remove(btn);
                    break;
            }
        }

        #region Overlay Panel
        /// <summary>
        /// Temporary pane used to host orginal content which is autohidden
        /// </summary>
        OverlayDockablePane _tempPane = null;

        /// <summary>
        /// Current docking button attached to current temporary pane
        /// </summary>
        DockingButton _currentButton;

        /// <summary>
        /// Event handler which show a temporary pane with a single content attached to a docking button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnShowAutoHidePane(object sender, MouseEventArgs e)
        {
            if (_currentButton == sender)
                return;

            HideTempPane(true);

            _currentButton = sender as DockingButton;

            ShowTempPane(true);
        }
       
        /// <summary>
        /// Event handler which hide temporary pane
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnHideAutoHidePane(object sender, MouseEventArgs e)
        {
            HideTempPane(true);
        }

        /// <summary>
        /// Hide temporay pane and reset current docking button
        /// </summary>
        /// <param name="smooth">True if resize animation is enabled</param>
        private void HideTempPane(bool smooth)
        {
            if (_tempPane != null)
            {
                DockablePane pane = gridDocking.GetPaneFromContent(_tempPane.Contents[0]) as DockablePane;
                bool right_left = false;
                double length = 0.0;

                switch (_currentButton.DockingButtonGroup.Dock)
                {
                    case Dock.Left:
                    case Dock.Right:
                        if (_tempPaneAnimation != null)
                            pane.PaneWidth = _lengthAnimation;
                        else 
                            pane.PaneWidth = _tempPane.Width;
                        length = _tempPane.Width;
                        right_left = true;
                        break;
                    case Dock.Top:
                    case Dock.Bottom:
                        if (_tempPaneAnimation != null)
                            pane.PaneHeight = _lengthAnimation;
                        else
                            pane.PaneHeight = _tempPane.Height;
                        length = _tempPane.Height;
                        right_left = false;
                        break;
                }

                _tempPane.OnStateChanged-=new EventHandler(_tempPane_OnStateChanged);

                if (smooth)
                {
                    HideOverlayPanel(length, right_left);
                }
                else
                {
                    ForceHideOverlayPanel();
                    panelFront.BeginAnimation(DockPanel.OpacityProperty, null);
                    panelFront.Children.Clear();
                    panelFront.Opacity = 0.0;
                    _tempPane.Close();
                }

                _currentButton = null;
                _tempPane = null;
            }

        }

        /// <summary>
        /// Show tampoary pane attached to current docking buttno
        /// </summary>
        /// <param name="smooth">True if resize animation is enabled</param>
        private void ShowTempPane(bool smooth)
        {

            _tempPane = new OverlayDockablePane(this, _currentButton.DockableContent, _currentButton.DockingButtonGroup.Dock);
            _tempPane.OnStateChanged += new EventHandler(_tempPane_OnStateChanged);

            DockablePane pane = gridDocking.GetPaneFromContent(_currentButton.DockableContent) as DockablePane;
            panelFront.Children.Clear();
            _tempPane.SetValue(DockPanel.DockProperty, _currentButton.DockingButtonGroup.Dock);
            panelFront.Children.Add(_tempPane);
            DockPanelSplitter splitter = null;
            bool right_left = false;
            double length = 0.0;

            switch (_currentButton.DockingButtonGroup.Dock)
            {
                case Dock.Left:
                    splitter = new DockPanelSplitter(_tempPane, null, SplitOrientation.Vertical);
                    length = pane.PaneWidth;
                    right_left = true;
                    break;
                case Dock.Right:
                    splitter = new DockPanelSplitter(null, _tempPane, SplitOrientation.Vertical);
                    length = pane.PaneWidth;
                    right_left = true;
                    break;
                case Dock.Top:
                    splitter = new DockPanelSplitter(_tempPane, null, SplitOrientation.Horizontal);
                    length = pane.PaneHeight;
                    right_left = false;
                    break;
                case Dock.Bottom:
                    splitter = new DockPanelSplitter(null, _tempPane, SplitOrientation.Horizontal);
                    length = pane.PaneHeight;
                    right_left = false;
                    break;
            }

            splitter.SetValue(DockPanel.DockProperty, _currentButton.DockingButtonGroup.Dock);
            panelFront.Children.Add(splitter);

            if (smooth)
                ShowOverlayPanel(length, right_left);
            else
            {
                if (right_left)
                    _tempPane.Width = length;
                else
                    _tempPane.Height = length;
                panelFront.Opacity = 1.0;
            }
                
        }

        /// <summary>
        /// Handle AutoHide/Hide commande issued by user on temporary pane
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void _tempPane_OnStateChanged(object sender, EventArgs e)
        {
            Pane pane = gridDocking.GetPaneFromContent(_currentButton.DockableContent);

            if (_currentButton != null)
            {
                switch (_currentButton.DockingButtonGroup.Dock)
                {
                    case Dock.Left:
                    case Dock.Right:
                        pane.PaneWidth = _tempPane.PaneWidth;
                        break;
                    case Dock.Top:
                    case Dock.Bottom:
                        pane.PaneHeight = _tempPane.PaneHeight;
                        break;
                }

                RemoveDockingButtons(_currentButton.DockingButtonGroup);
                _dockingBtnGroups.Remove(_currentButton.DockingButtonGroup);
            }



            bool showOriginalPane = (_tempPane.State == PaneState.Docked);

            HideTempPane(false);

            if (showOriginalPane)
                pane.Show();
            else
                pane.Hide();

        }


        #region Temporary pane animation methods
        /// <summary>
        /// Current resize orientation
        /// </summary>
        bool _leftRightAnimation = false;
        /// <summary>
        /// Target size of animation
        /// </summary>
        double _lengthAnimation = 0;

        /// <summary>
        /// Temporary overaly pane used for animation
        /// </summary>
        OverlayDockablePane _tempPaneAnimation;

        /// <summary>
        /// Current animation object itself
        /// </summary>
        DoubleAnimation _animation;

        /// <summary>
        /// Show current overlay pane which hosts current auto-hiding content
        /// </summary>
        /// <param name="length">Target length</param>
        /// <param name="left_right">Resize orientaion</param>
        void ShowOverlayPanel(double length, bool left_right)
        {
            ForceHideOverlayPanel();

            _leftRightAnimation = left_right;
            _tempPaneAnimation = _tempPane;
            _lengthAnimation = length;

            _animation = new DoubleAnimation();
            _animation.From = 0.0;
            _animation.To = length;
            _animation.Duration = new Duration(TimeSpan.FromMilliseconds(200));
            _animation.Completed += new EventHandler(ShowOverlayPanel_Completed);
            if (_leftRightAnimation)
                _tempPaneAnimation.BeginAnimation(FrameworkElement.WidthProperty, _animation);
            else
                _tempPaneAnimation.BeginAnimation(FrameworkElement.HeightProperty, _animation);

            DoubleAnimation anOpacity = new DoubleAnimation();
            anOpacity.From = 0.0;
            anOpacity.To = 1.0;
            anOpacity.Duration = new Duration(TimeSpan.FromMilliseconds(200));
            panelFront.BeginAnimation(DockPanel.OpacityProperty, anOpacity);
        }

        /// <summary>
        /// Showing animation completed event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks>Set final lenght and reset animation object on temp overlay panel</remarks>
        void ShowOverlayPanel_Completed(object sender, EventArgs e)
        {
            _animation.Completed -= new EventHandler(ShowOverlayPanel_Completed);

            if (_tempPaneAnimation != null)
            {
                if (_leftRightAnimation)
                {
                    _tempPaneAnimation.BeginAnimation(FrameworkElement.WidthProperty, null);
                    _tempPaneAnimation.Width = _lengthAnimation;
                }
                else
                {
                    _tempPaneAnimation.BeginAnimation(FrameworkElement.HeightProperty, null);
                    _tempPaneAnimation.Height = _lengthAnimation;
                }
            }

            _tempPaneAnimation = null;
        }
        
        /// <summary>
        /// Hide current overlay panel
        /// </summary>
        /// <param name="length"></param>
        /// <param name="left_right"></param>
        void HideOverlayPanel(double length, bool left_right)
        {
            _leftRightAnimation = left_right;
            _tempPaneAnimation = _tempPane;
            _lengthAnimation = length;

            _animation = new DoubleAnimation();
            _animation.From = length;
            _animation.To = 0.0;
            _animation.Duration = new Duration(TimeSpan.FromMilliseconds(200));
            _animation.Completed += new EventHandler(HideOverlayPanel_Completed);

            if (left_right)
                _tempPaneAnimation.BeginAnimation(FrameworkElement.WidthProperty, _animation);
            else
                _tempPaneAnimation.BeginAnimation(FrameworkElement.HeightProperty, _animation);

            DoubleAnimation anOpacity = new DoubleAnimation();
            anOpacity.From = 1.0;
            anOpacity.To = 0.0;
            anOpacity.Duration = new Duration(TimeSpan.FromMilliseconds(200));
            panelFront.BeginAnimation(DockPanel.OpacityProperty, anOpacity);        
        }

        /// <summary>
        /// Hiding animation completed event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks>Set final lenght to 0 and reset animation object on temp overlay panel</remarks>
        void HideOverlayPanel_Completed(object sender, EventArgs e)
        {
            ForceHideOverlayPanel();
            panelFront.Children.Clear();
        }

        /// <summary>
        /// Forces to hide current overlay panel
        /// </summary>
        /// <remarks>Usually used when a second animation is about to start from a different button</remarks>
        void ForceHideOverlayPanel()
        {
            if (_tempPaneAnimation != null)
            {
                _animation.Completed -= new EventHandler(HideOverlayPanel_Completed);
                _animation.Completed -= new EventHandler(ShowOverlayPanel_Completed);
                if (_leftRightAnimation)
                {
                    _tempPaneAnimation.BeginAnimation(FrameworkElement.WidthProperty, null);
                    //_tempPaneAnimation.Width = 0;
                }
                else
                {
                    _tempPaneAnimation.BeginAnimation(FrameworkElement.HeightProperty, null);
                    //_tempPaneAnimation.Height = 0;
                }

                _tempPaneAnimation.Close();
                _tempPaneAnimation = null;
            }
        }
        #endregion

        #endregion

        /// <summary>
        /// Handle dockable pane layout changing
        /// </summary>
        /// <param name="sourcePane">Source pane to move</param>
        /// <param name="destinationPane">Relative pane</param>
        /// <param name="relativeDock"></param>
        internal void MoveTo(DockablePane sourcePane, Pane destinationPane, Dock relativeDock)
        {
            gridDocking.MoveTo(sourcePane, destinationPane, relativeDock);
        }

        /// <summary>
        /// Called from a pane when it's dropped into an other pane
        /// </summary>
        /// <param name="sourcePane">Source pane which is going to be closed</param>
        /// <param name="destinationPane">Destination pane which is about to host contents from SourcePane</param>
        internal void MoveInto(DockablePane sourcePane, Pane destinationPane)
        {
            gridDocking.MoveInto(sourcePane, destinationPane);
        }

        #region DragDrop Operations
        /// <summary>
        /// Parent window hosting DockManager user control
        /// </summary>
        public Window ParentWindow = null;

        /// <summary>
        /// Begins dragging operations
        /// </summary>
        /// <param name="floatingWindow">Floating window containing pane which is dragged by user</param>
        /// <param name="point">Current mouse position</param>
        /// <param name="offset">Offset to be use to set floating window screen position</param>
        /// <returns>Retruns True is drag is completed, false otherwise</returns>
        public bool Drag(FloatingWindow floatingWindow, Point point, Point offset)
        {
            if (!IsMouseCaptured)
            {
                if (CaptureMouse())
                {
                    floatingWindow.Owner = ParentWindow;
                    DragPaneServices.StartDrag(floatingWindow, point, offset);
                    
                    return true;
                }
            }

            return false;
        }
        /// <summary>
        /// Handles OnMouseDown event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnMouseDown(object sender, MouseEventArgs e)
        {

        }

        /// <summary>
        /// Handles mousemove event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (IsMouseCaptured)
                DragPaneServices.MoveDrag(PointToScreen(e.GetPosition(this)));
        }

        /// <summary>
        /// Handles mouseUp event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks>Releases eventually camptured mouse events</remarks>
        void OnMouseUp(object sender, MouseEventArgs e)
        {
            if (IsMouseCaptured)
            {
                DragPaneServices.EndDrag(PointToScreen(e.GetPosition(this)));
                ReleaseMouseCapture();
            }
        }

        DragPaneServices _dragPaneServices;

        internal DragPaneServices DragPaneServices
        {
            get 
            {
                if (_dragPaneServices == null)
                    _dragPaneServices = new DragPaneServices(this);

                return _dragPaneServices;
            }
        }
        #endregion


        #region IDropSurface

        /// <summary>
        /// Returns a rectangle where this surface is active
        /// </summary>
        public Rect SurfaceRectangle
        {
            get 
            { return new Rect(PointToScreen(new Point(0,0)), new Size(ActualWidth, ActualHeight)); }
        }

        /// <summary>
        /// Overlay window which shows docking placeholders
        /// </summary>
        OverlayWindow _overlayWindow;

        /// <summary>
        /// Returns current overlay window
        /// </summary>
        internal OverlayWindow OverlayWindow
        {
            get
            {
                return _overlayWindow; 
            }
        }

        /// <summary>
        /// Handles this sourface mouse entering (show current overlay window)
        /// </summary>
        /// <param name="point">Current mouse position</param>
        public void OnDragEnter(Point point)
        {
            OverlayWindow.Owner = DragPaneServices.FloatingWindow;
            OverlayWindow.Left = PointToScreen(new Point(0, 0)).X;
            OverlayWindow.Top = PointToScreen(new Point(0, 0)).Y;
            OverlayWindow.Width = ActualWidth;
            OverlayWindow.Height = ActualHeight;
            OverlayWindow.Show();       
        }

        /// <summary>
        /// Handles mouse overing this surface
        /// </summary>
        /// <param name="point"></param>
        public void OnDragOver(Point point)
        {

        }

        /// <summary>
        /// Handles mouse leave event during drag (hide overlay window)
        /// </summary>
        /// <param name="point"></param>
        public void OnDragLeave(Point point)
        {
            _overlayWindow.Owner = null;
            _overlayWindow.Hide();
            ParentWindow.Activate();
            
        }

        /// <summary>
        /// Handler drop events
        /// </summary>
        /// <param name="point">Current mouse position</param>
        /// <returns>Returns alwasy false because this surface doesn't support direct drop</returns>
        public bool OnDrop(Point point)
        {
            return false;
        }

        #endregion


        #region Persistence

        /// <summary>
        /// Serialize layout state of panes and contents into a xml string
        /// </summary>
        /// <returns>Xml containing layout state</returns>
        public string GetLayoutAsXml()
        {
            XmlDocument doc = new XmlDocument();
            doc.AppendChild(doc.CreateElement("DockingLibrary_Layout"));
            gridDocking.Serialize(doc, doc.DocumentElement);
            return doc.OuterXml;
        }

        /// <summary>
        /// Restore docking layout reading a xml string which is previously generated by a call to GetLayoutState
        /// </summary>
        /// <param name="xml">Xml containing layout state</param>
        /// <param name="getContentHandler">Delegate used by serializer to get user defined dockable contents</param>
        public void RestoreLayoutFromXml(string xml, GetContentFromTypeString getContentHandler)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);

            gridDocking.Deserialize(this, doc.ChildNodes[0], getContentHandler);

            List<Pane> addedPanes = new List<Pane>();
            foreach (DockableContent content in Contents)
            {
                DockablePane pane = content.ContainerPane as DockablePane;
                if (pane != null && !addedPanes.Contains(pane))
                {
                    if (pane.State == PaneState.AutoHide)
                    {
                        addedPanes.Add(pane);
                        AddPaneDockingButtons(pane);
                    }
                }
            }

            _currentButton = null;
        }
        #endregion
    }
}