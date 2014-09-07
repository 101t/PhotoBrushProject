using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DockingLibrary
{
    /// <summary>
    /// Interaction logic for DocumentsPane.xaml
    /// </summary>

    partial class DocumentsPane : Pane
    {
        public readonly List<ManagedContent> Documents = new List<ManagedContent>();

        public DocumentsPane(DockManager dockManager) : base(dockManager)
        {
            InitializeComponent();
        }

        public DocumentContent ActiveDocument
        {
            get
            {
                if (tbcDocuments.SelectedContent == null)
                    return null;

                return Documents[tbcDocuments.SelectedIndex] as DocumentContent;
            }
        }

        //public override DockManager DockManager
        //{
        //    get
        //    {
        //        return base.DockManager;
        //    }
        //    set
        //    {
        //        base.DockManager = value;

        //        Show();
        //    }
        //}

        public new ManagedContent ActiveContent
        {
            get
            {
                if (tbcDocuments.SelectedContent == null)
                    return null;
                return Documents[tbcDocuments.SelectedIndex];
            }
            set
            {
                if (Documents.Count > 1)
                {
                    tbcDocuments.SelectedIndex = Documents.IndexOf(value);
                }
            }
        }

        //void OnUnloaded(object sender, EventArgs e)
        //{
        //    foreach (ManagedContent content in Documents)
        //        content.Close();

            
        //    Documents.Clear();
        //}

        

        public override void Add(DockableContent content)
        {
            System.Diagnostics.Debug.Assert(content != null);
            if (content == null)
                return;


            Documents.Add(content);
            AddItem(content);

            base.Add(content);
        }

        public void Add(DocumentContent content)
        {
            System.Diagnostics.Debug.Assert(content != null);
            if (content == null)
                return;

            Documents.Add(content);
            AddItem(content);
        }

        public override void Remove(DockableContent content)
        {
            System.Diagnostics.Debug.Assert(content != null);
            if (content == null)
                return;

            RemoveItem(content);
            Documents.Remove(content);
            DockManager.Remove(content);

            base.Remove(content);
        }

        public void Remove(DocumentContent content)
        {
            System.Diagnostics.Debug.Assert(content != null);
            if (content == null)
                return;

            RemoveItem(content);
            Documents.Remove(content);
            content.Close();
        }

        public override void Show(DockableContent content)
        {
            if (!Contents.Contains(content))
                Add(content);

            base.Show(content);
        }


        public override void RefreshTitle()
        {
            TabItem tabItem = tbcDocuments.Items[Documents.IndexOf(ActiveContent)] as TabItem;
            tabItem.Header = ActiveContent.Title;

            base.RefreshTitle();
        }

        protected virtual void AddItem(ManagedContent content)
        {
            TabItem item = new TabItem();
            DockPanel tabPanel = new DockPanel();
            item.Header = content.Title;
            item.Content = new ContentPresenter();
            (item.Content as ContentPresenter).Content = content.Content;
            item.Style = FindResource("DocTabItemStyle") as Style;

            tbcDocuments.Items.Add(item);
            tbcDocuments.SelectedItem = item;

            if (tbcDocuments.Items.Count == 1)
                tbcDocuments.Visibility = Visibility.Visible;

        }

        protected virtual void RemoveItem(ManagedContent content)
        {
            TabItem tabItem = tbcDocuments.Items[Documents.IndexOf(content)] as TabItem;

            tabItem.Content = null;

            tbcDocuments.Items.Remove(tabItem);

            if (tbcDocuments.Items.Count == 0)
                tbcDocuments.Visibility = Visibility.Collapsed;

        }

        #region Drag inner contents
        Point ptStartDrag;
        
        void OnTabItemMouseDown(object sender, MouseButtonEventArgs e)
        {
            FrameworkElement senderElement = sender as FrameworkElement;
            TabItem item = senderElement.TemplatedParent as TabItem;
            if (!senderElement.IsMouseCaptured)
            {
                ptStartDrag = e.GetPosition(this);
                senderElement.CaptureMouse();
            }
            
        }
        void OnTabItemMouseMove(object sender, MouseEventArgs e)
        {
            FrameworkElement senderElement = sender as FrameworkElement;
            TabItem item = senderElement.TemplatedParent as TabItem;
            if (senderElement.IsMouseCaptured && Math.Abs(ptStartDrag.X - e.GetPosition(this).X) > 4)
            {
                senderElement.ReleaseMouseCapture();
                int index = tbcDocuments.Items.IndexOf(item);
                DockableContent contentToDrag = null;
                //foreach (DockableContent content in Contents)
                //    if (content.Content == (item.Content as ContentPresenter).Content)
                //        contentToDrag = content;

                contentToDrag = Documents[index] as DockableContent;
                
                if (contentToDrag != null)
                    DragContent(contentToDrag, e.GetPosition(DockManager), e.GetPosition(item));
                
                e.Handled = true;
            }
        }
        void OnTabItemMouseUp(object sender, MouseButtonEventArgs e)
        {
            FrameworkElement senderElement = sender as FrameworkElement;
            TabItem item = senderElement.TemplatedParent as TabItem;
            senderElement.ReleaseMouseCapture();

        }
        #endregion

        #region TabControl commands (switch menu / close current content)
        void OnDocumentSwitch(object sender, EventArgs e)
        {
            int index = cxDocumentSwitchMenu.Items.IndexOf((sender as MenuItem));

            TabItem tabItem = tbcDocuments.Items[index] as TabItem;
            tbcDocuments.Items.RemoveAt(index);
            tbcDocuments.Items.Insert(0, tabItem);
            tbcDocuments.SelectedIndex = 0;

            ManagedContent contentToSwap = Documents[index];
            Documents.RemoveAt(index);
            Documents.Insert(0, contentToSwap);

            foreach(MenuItem item in cxDocumentSwitchMenu.Items)
                item.Click -= new RoutedEventHandler(OnDocumentSwitch);
        }

        ContextMenu cxDocumentSwitchMenu;

        void OnBtnDocumentsMenu(object sender, MouseButtonEventArgs e)
        {
            if (Documents.Count <= 1)
                return;

            cxDocumentSwitchMenu = new ContextMenu();

            foreach (ManagedContent content in Documents)
            {
                MenuItem item = new MenuItem();
                Image imgIncon = new Image();
                imgIncon.Source = content.Icon;
                item.Icon = imgIncon;
                item.Header = content.Title;
                item.Click += new RoutedEventHandler(OnDocumentSwitch);
                cxDocumentSwitchMenu.Items.Add(item);
            }

            cxDocumentSwitchMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.AbsolutePoint;
            cxDocumentSwitchMenu.PlacementRectangle = new Rect(PointToScreen(e.GetPosition(this)), new Size(0, 0));
            cxDocumentSwitchMenu.PlacementTarget = this;
            cxDocumentSwitchMenu.IsOpen = true;
        }

        void OnBtnDocumentClose(object sender, MouseButtonEventArgs e)
        {
            if (ActiveContent == null)
                return;

            if (ActiveContent is DockableContent)
                Remove(ActiveContent as DockableContent);
            else 
                Remove(ActiveDocument);
        }

        //void OnShowDocumentsMenu(object sender, DependencyPropertyChangedEventArgs e)
        //{ 
            
        //}
        #endregion
    }
}