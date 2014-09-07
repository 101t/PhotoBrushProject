using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace DockingLibrary
{
    class DocumentsPane : Pane
    {
        public DocumentsPane()
        {
            panelHeader.Visibility = Visibility.Collapsed;
            _tabs.TabStripPlacement = Dock.Top;
            ShowTabs();
        }

        #region IDockPane Membri di
        protected override void DragContent(ManagedContent content)
        {
            if (content is DocumentContent)
                return;
            base.DragContent(content);
        }
        
        public override System.Windows.GridLength GridWidth
        {
            get
            {
                return new GridLength(1, GridUnitType.Star);
            }
            set
            {
                throw new InvalidOperationException();
            }
        }

        public override System.Windows.GridLength GridHeight
        {
            get
            {
                return new GridLength(1, GridUnitType.Star);
            }
            set
            {
                throw new InvalidOperationException();
            }
        }

        public override void AdjustSize()
        {
            //nothing to do
        }

        public override void Add(ManagedContent content)
        {
            base.Add(content);

            AddItem(content);
        }

        public override void Remove(ManagedContent content)
        {
            base.Remove(content);

            RemoveItem(content);
        }
        private bool _hidden = false;

        public override bool Hidden
        {
            get
            {
                return _hidden;
            }
        }


        public override void Show()
        {
            if (!Hidden)
                return;

            foreach (ManagedContent content in Contents)
                AddItem(content);

            _hidden = false;

            base.Show();
        }


        public override void Hide()
        {
            if (Hidden)
                return;

            foreach (ManagedContent content in Contents)
                RemoveItem(content);

            _hidden = true;

            base.Hide();
        }
        #endregion


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
                e.Data.GetDataPresent(typeof(DocumentContent)) ||
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
