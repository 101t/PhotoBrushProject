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
    public enum DockingType
    { 
        Dock,
        Floating,
        Document
    }

    public enum Dockable
    { 
        Dock,
        Document,
    }

    public class PaneDragEventArgs : EventArgs
    {
        public readonly Dock Dock;
        public readonly bool MoveInto = true;
        public readonly DragEventArgs DragEventArgs;
        public PaneDragEventArgs(DragEventArgs dragEventArgs)
        {
            DragEventArgs = dragEventArgs;
        }

        public PaneDragEventArgs(DragEventArgs dragEventArgs, Dock dock)
        {
            DragEventArgs = dragEventArgs;
            MoveInto = false;
            Dock = dock;
        }
    }
    public delegate void PaneDragEventHandler(object sender, PaneDragEventArgs e);

    public class ContentEventArgs : EventArgs
    {
        public readonly ManagedContent Content;
        public ContentEventArgs(ManagedContent content)
        {
            Content = content;
        }
    }
    public delegate void ContentEventHandler(object sender, ContentEventArgs e);


    /// <summary>
    /// Interaction logic for DockPane.xaml
    /// </summary>
    public partial class Pane : System.Windows.Controls.UserControl, IContentContainer
    {
        public List<ManagedContent> Contents
        {
            get { return _contents; }
        }
        
        protected List<ManagedContent> _contents = new List<ManagedContent>();

        protected TabControl _tabs = new TabControl();

 
        public Pane()
        {
            InitializeComponent();
        }

        public virtual void Add(ManagedContent content)
        {
            Contents.Add(content);
        }
      
        public virtual void Remove(ManagedContent content)
        {
            Contents.Remove(content);
        }


        #region TabControl management

        protected void ShowTabs()
        {
            _tabs.SelectionChanged += new SelectionChangedEventHandler(_tabs_SelectionChanged);
            windowContent.Content = _tabs;
        }

        protected void HideTabs()
        {
            _tabs.SelectionChanged -= new SelectionChangedEventHandler(_tabs_SelectionChanged);
            windowContent.Content = null;
        }

        protected void AddItem(ManagedContent content)
        {
            TabItem item = new TabItem();
            item.Header = content.Title;
            item.Content = content.WindowContent;

            _tabs.Items.Add(item);
            _tabs.SelectedItem = item;
        }

        protected void RemoveItem(ManagedContent content)
        {
            foreach (TabItem item in _tabs.Items)
                if (item.Content == content.WindowContent)
                {
                    item.Content = null;
                    _tabs.Items.Remove(item);
                    break;
                }
        }
        
        bool mouseItemIsDown = false;
        Point ptStart;
        
        void item_MouseDown(object sender, MouseButtonEventArgs e)
        {
            mouseItemIsDown = true;
            ptStart = e.GetPosition(this);
        }

        void item_MouseUp(object sender, MouseButtonEventArgs e)
        {
            mouseItemIsDown = false;
        }

        void item_MouseMove(object sender, MouseEventArgs e)
        {
            if (mouseItemIsDown)
            {
                if (Point.Subtract(e.GetPosition(this), ptStart).Length < 5)
                    return;

                TabItem item = (sender as Border).TemplatedParent as TabItem;
                ManagedContent content = Contents[_tabs.Items.IndexOf(item)];

                DragContent(content);
            }
        }

        void item_Close(object sender, RoutedEventArgs e)
        {
            TabItem item = (sender as Button).TemplatedParent as TabItem;
            ManagedContent content = Contents[_tabs.Items.IndexOf(item)];

            Remove(content);

            if (OnCloseContent != null)
                OnCloseContent(this, new ContentEventArgs(content));

        }


        void _tabs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_tabs.SelectedIndex>=0)
                tbTitle.Text = Contents[_tabs.SelectedIndex].Title;
        }

        protected virtual void DragContent(ManagedContent content)
        {
            Remove(content);

            mouseItemIsDown = false;
            panelDrag.Visibility = Visibility.Visible;

            if (DragDrop.DoDragDrop(this, content, DragDropEffects.Move) == DragDropEffects.None)
            {
                FireOnDragAbortEvent();
                Add(content);
            }


            panelDrag.Visibility = Visibility.Collapsed;
        }
        #endregion

        #region Gestione del layout
        public event EventHandler OnClose;
        public event ContentEventHandler OnCloseContent;
        public event EventHandler OnAutoHide;

        private void btnOnClose(object sender, EventArgs e)
        {
            if (OnClose != null)
                OnClose(this, EventArgs.Empty);
        }

        private void btnOnAutoHide(object sender, EventArgs e)
        {
            if (OnAutoHide != null)
                OnAutoHide(this, EventArgs.Empty);
        }

        public virtual void Show()
        {
        }

        public virtual void Hide()
        {
        }
        
        #endregion

        #region Drag&Drop
        public event EventHandler OnDrag;
        public event EventHandler OnDragAbort;
        public event PaneDragEventHandler OnDragEnd;

        protected void FireOnDragEvent()
        {
            if (OnDrag != null)
                OnDrag(this, EventArgs.Empty);
        }
        protected void FireOnDragAbortEvent()
        {
            if (OnDragAbort != null)
                OnDragAbort(this, EventArgs.Empty);
        }
        protected void FireOnDragEndEvent(DragEventArgs e)
        {
            if (OnDragEnd != null)
                OnDragEnd(this, new PaneDragEventArgs(e));
        }

        protected void FireOnDragEndEvent(DragEventArgs e, Dock dock)
        {
            if (OnDragEnd != null)
                OnDragEnd(this, new PaneDragEventArgs(e, dock));
        }


        bool mouseIsDown = false;
        private void HeaderMouseDown(object sender, MouseEventArgs e)
        {
            mouseIsDown = true;
        }
        private void HeaderMouseUp(object sender, MouseEventArgs e)
        {
            mouseIsDown = false;
        }

        private void HeaderMouseMove(object sender, MouseEventArgs e)
        {
            if (mouseIsDown)
            {
                FireOnDragEvent();

                mouseIsDown = false;

                if (DragDrop.DoDragDrop(this, this, DragDropEffects.Move) == DragDropEffects.None)
                {
                    FireOnDragAbortEvent();
                }
            }

        }


        protected virtual void OnDragEnter(object sender, DragEventArgs e)
        {
        }

        protected virtual void OnDragLeave(object sender, DragEventArgs e)
        {
        }

        protected virtual void OnDrop(object sender, DragEventArgs e)
        {
        }

        protected virtual void OnDragOver(object sender, DragEventArgs e)
        {
        }

        protected virtual void OnDropInto(object sender, DragEventArgs e)
        {
        } 

        protected virtual void OnDropDockLeft(object sender, DragEventArgs e)
        {
        }
        protected virtual void OnDropDockRight(object sender, DragEventArgs e)
        {
        }

        protected virtual void OnDropDockBottom(object sender, DragEventArgs e)
        {
        }

        protected virtual void OnDropDockTop(object sender, DragEventArgs e)
        {
        } 

        #endregion

        #region IPane Membri di
        public virtual bool Hidden
        {
            get { return false; }
        }

        protected System.Windows.GridLength _width = new GridLength(100);
        protected System.Windows.GridLength _height = new GridLength(100);

        public virtual System.Windows.GridLength GridWidth
        {
            get
            {
                return _width;
            }
            set
            {
                _width = value;
            }
        }

        public virtual System.Windows.GridLength GridHeight
        {
            get
            {
                return _height;
            }
            set
            {
                _height = value;
            }
        }

        public virtual void AdjustSize()
        {
        }


        public virtual IPane FindPaneFromContent(ManagedContent content)
        {
            if (Contents.Contains(content))
                return this;

            return null;
        }

        #endregion
    }
}