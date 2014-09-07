using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace DockingLibrary
{
    class DockablePane : Pane
    {
        public Dock Dock;
        
        public DockablePane(ManagedContent content, Dock dock)
        {
            Dock = dock;
            _tabs.TabStripPlacement = Dock.Bottom;
            

            Add(content);
        }
        private bool _hidden = false;

        public override bool Hidden
        {
            get
            {
                return _hidden;
            }
        }

        public override void Add(ManagedContent content)
        {
            base.Add(content);

            if (Contents.Count == 1)
            { 
                tbTitle.Text = content.Title;
                windowContent.Content = content.WindowContent;
            }
            else if (Contents.Count == 2)
            {
                ShowTabs();
                AddItem(Contents[0]);
                AddItem(content);
            }
            else
                AddItem(content);
        }

        public override void Remove(ManagedContent content)
        {
            base.Remove(content);

            RemoveItem(content);

            if (Contents.Count == 1)
            {
                RemoveItem(Contents[0]); 
                HideTabs();
                windowContent.Content = Contents[0].WindowContent;
                tbTitle.Text = Contents[0].Title;
            }
            else if (Contents.Count==0)
                windowContent.Content = null;
        }


        public override void Show()
        {
            if (!Hidden)
                return;

            if (Contents.Count == 1)
                windowContent.Content = Contents[0].WindowContent;
            else
            {
                foreach (ManagedContent content in Contents)
                    AddItem(content);

                windowContent.Content = _tabs;
            }

            _hidden = false;

            base.Show();
        }


        public override void Hide()
        {
            if (Hidden)
                return;

            //if (_tabs != null)
            //{
            foreach (TabItem item in _tabs.Items)
            {
                
                item.Content = null;
            }

            _tabs.Items.Clear();

            //}
            //else
            windowContent.Content = null;

            _hidden = true;

            base.Hide();
        }

        public override void AdjustSize()
        {
            if (Parent == null)
                return;

            Grid gridParent = (this.Parent as Grid).Parent as Grid;

            foreach (ColumnDefinition colDef in gridParent.ColumnDefinitions)
                if (!colDef.Width.IsStar)
                {
                    GridWidth = colDef.Width;
                    break;
                }
            foreach (RowDefinition rowDef in gridParent.RowDefinitions)
                if (!rowDef.Height.IsStar)
                {
                    GridHeight = rowDef.Height;
                    break;
                }

            base.AdjustSize();
        }


        #region Drag&Drop

        protected override void OnDragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(DockablePane)) ||
                e.Data.GetDataPresent(typeof(ManagedContent)))
            {
                panelDrag.Visibility = Visibility.Visible;
                e.Handled = true;
            }
            else
                e.Effects = DragDropEffects.None;
        }

        protected override void OnDragLeave(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(DockablePane)) ||
                e.Data.GetDataPresent(typeof(ManagedContent)))
            {
                panelDrag.Visibility = Visibility.Collapsed;
                e.Handled = true;
            }
            else
                e.Effects = DragDropEffects.None;
        }

        protected override void OnDrop(object sender, DragEventArgs e)
        {
            panelDrag.Visibility = Visibility.Collapsed;
            e.Effects = DragDropEffects.None;
        }

        protected override void OnDragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(DockablePane)) ||
                e.Data.GetDataPresent(typeof(ManagedContent)))
            {
                panelDrag.Visibility = Visibility.Visible;
                e.Handled = true;
            }
        }



        protected override void OnDropInto(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(DockablePane)) ||
                e.Data.GetDataPresent(typeof(ManagedContent)))
            {
                FireOnDragEndEvent(e);

                panelDrag.Visibility = Visibility.Collapsed;
                e.Handled = true;
            }
        }

        protected override void OnDropDockLeft(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(DockablePane)) ||
                e.Data.GetDataPresent(typeof(ManagedContent)))
            {
                FireOnDragEndEvent(e, Dock.Left);

                panelDrag.Visibility = Visibility.Collapsed;
                e.Handled = true;
            }
        }
        protected override void OnDropDockRight(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(DockablePane)) ||
                e.Data.GetDataPresent(typeof(ManagedContent)))
            {
                FireOnDragEndEvent(e, Dock.Right);

                panelDrag.Visibility = Visibility.Collapsed;
                e.Handled = true;
            }
        }

        protected override void OnDropDockBottom(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(DockablePane)) ||
                e.Data.GetDataPresent(typeof(ManagedContent)))
            {
                FireOnDragEndEvent(e, Dock.Bottom);

                panelDrag.Visibility = Visibility.Collapsed;
                e.Handled = true;
            }
        }

        protected override void OnDropDockTop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(DockablePane)) ||
                e.Data.GetDataPresent(typeof(ManagedContent)))
            {
                FireOnDragEndEvent(e, Dock.Top);

                panelDrag.Visibility = Visibility.Collapsed;
                e.Handled = true;
            }
        } 
        #endregion
    }
}
