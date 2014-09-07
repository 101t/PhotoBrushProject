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
    class FloatingWindowHostedPane : DockablePane
    {
        public readonly DockablePane ReferencedPane;
        FloatingWindow _floatingWindow;

        public FloatingWindowHostedPane(FloatingWindow floatingWindow, DockablePane referencedPane) : base(referencedPane.DockManager)
        {
            ReferencedPane = referencedPane;
            _floatingWindow = floatingWindow;

            DockableContent lastSelectedContent = ReferencedPane.ActiveContent;

            ChangeState(ReferencedPane.State);
            //ReferencedPane.Hide();

            //DockManager = ReferencedPane.DockManager;
            foreach (DockableContent content in ReferencedPane.Contents)
            {
                ReferencedPane.Hide(content);
                Add(content);
                Show(content);
                content.SetContainerPane(ReferencedPane);
            }

            ActiveContent = lastSelectedContent;
            ShowHeader = false;

        }

        

        public override void Remove(DockableContent content)
        {
            ReferencedPane.Remove(content);
            base.Remove(content);
        }

        protected override void OnDockingMenu(object sender, EventArgs e)
        {
            if (sender == menuFloatingWindow)
            {
                ReferencedPane.ChangeState(PaneState.FloatingWindow);
                ChangeState(ReferencedPane.State);
            }

            if (sender == menuDockedWindow)
            {
                ReferencedPane.ChangeState(PaneState.DockableWindow);
                ChangeState(ReferencedPane.State);
            }

            if (sender == menuTabbedDocument || sender == menuClose || sender == menuAutoHide)
            {
                foreach (DockableContent content in Contents)
                    content.SetContainerPane(ReferencedPane);

                Close();

                _floatingWindow.Close();
            }

            if (sender == menuTabbedDocument)
                ReferencedPane.TabbedDocument();
            if (sender == menuClose)
                ReferencedPane.Close();
            if (sender == menuAutoHide)
            {
                ReferencedPane.Show();
                ReferencedPane.AutoHide();
            }
        }


        //protected override void DragContent(DockableContent contentToDrag, Point startDragPoint, Point offset)
        //{
        //    Remove(contentToDrag);
        //    DockablePane pane = new DockablePane();
        //    pane = new DockablePane();
        //    pane.DockManager = DockManager;
        //    pane.Add(contentToDrag);
        //    pane.Show();
        //    DockManager.Add(pane);
        //    //DockManager.Add(contentToDrag);
        //    //pane.ChangeState(PaneState.DockableWindow);
        //    FloatingWindow wnd = new FloatingWindow(pane);
        //    pane.ChangeState(PaneState.DockableWindow);
        //    DockManager.Drag(wnd, startDragPoint, offset);            
        //}
    }
}
