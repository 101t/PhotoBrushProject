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
using System.Xml;

namespace DockingLibrary
{
    /// <summary>
    /// Interaction logic for DockingGrid.xaml
    /// </summary>

    public partial class DockingGrid : System.Windows.Controls.UserControl, ILayoutSerializable
    {
        public DockingGrid()
        {
            InitializeComponent();

        }

        internal void AttachDockManager(DockManager dockManager)
        {
            _docsPane = new DocumentsPane(dockManager);
            _rootGroup = new DockablePaneGroup(DocumentsPane);
            ArrangeLayout();
        }
        
        //Creates a root group with a DocumentsPane
        DockablePaneGroup _rootGroup;

        DocumentsPane _docsPane;

        public DocumentsPane DocumentsPane
        {
            get { return _docsPane; }
        }

        internal void AttachPaneEvents(DockablePane pane)
        {
            pane.OnStateChanged += new EventHandler(pane_OnStateChanged);
            pane.OnDockChanged += new EventHandler(pane_OnDockChanged);
        }

        internal void DetachPaneEvents(DockablePane pane)
        {
            pane.OnStateChanged -= new EventHandler(pane_OnStateChanged);
            pane.OnDockChanged -= new EventHandler(pane_OnDockChanged);
        }

        void pane_OnDockChanged(object sender, EventArgs e)
        {
            DockablePane pane = sender as DockablePane;
            Remove(pane);
            Add(pane);
        }

        void pane_OnStateChanged(object sender, EventArgs e)
        {
            DockablePane pane = sender as DockablePane;

            //if (pane.State == PaneState.FloatingWindow)
            //    Remove(pane);
            //else
                ArrangeLayout();
        }


        public void Add(DockablePane pane)
        {
            _rootGroup = _rootGroup.AddPane(pane);
            ArrangeLayout();
        }

        void Dump(DockablePaneGroup group, int indent)
        {
            if (indent == 0)
                Console.WriteLine("Dump()");
            for (int i = 0; i < indent; i++)
                Console.Write("-");
            Console.Write(">");
            if (group.AttachedPane == null)
            {
                Console.WriteLine(group.Dock);
                Dump(group.FirstChildGroup, indent + 4);
                Console.WriteLine();
                Dump(group.SecondChildGroup, indent + 4);
            }
            else if (group.AttachedPane.ActiveContent!=null)
                Console.WriteLine(group.AttachedPane.ActiveContent.Title);
            else
                Console.WriteLine(group.AttachedPane.ToString() + " {null}");
        }

        public void Add(DockablePane pane, Pane relativePane, Dock relativeDock)
        {
            Console.WriteLine("Add(...)");
            AttachPaneEvents(pane);
            DockablePaneGroup group = GetPaneGroup(relativePane);
            //group.ParentGroup.ReplaceChildGroup(group, new DockablePaneGroup(group, new DockablePaneGroup(relativePane), relativeDock));

            switch (relativeDock)
            {
                case Dock.Right:
                case Dock.Bottom:
                    {
                        if (group == _rootGroup)
                        {
                            _rootGroup = new DockablePaneGroup(group, new DockablePaneGroup(pane), relativeDock);
                        }
                        else
                        {
                            DockablePaneGroup parentGroup = group.ParentGroup;
                            DockablePaneGroup newChildGroup = new DockablePaneGroup(group, new DockablePaneGroup(pane), relativeDock);
                            parentGroup.ReplaceChildGroup(group, newChildGroup);
                        }
                    }
                    break;
                case Dock.Left:
                case Dock.Top:
                    {
                        if (group == _rootGroup)
                        {
                            _rootGroup = new DockablePaneGroup(new DockablePaneGroup(pane), group, relativeDock);
                        }
                        else
                        {
                            DockablePaneGroup parentGroup = group.ParentGroup;
                            DockablePaneGroup newChildGroup = new DockablePaneGroup(new DockablePaneGroup(pane), group, relativeDock);
                            parentGroup.ReplaceChildGroup(group, newChildGroup);
                        }
                    }
                    break;
                    //return new DockablePaneGroup(new DockablePaneGroup(pane), this, pane.Dock);
            }

            
            //group.ChildGroup = new DockablePaneGroup(group.ChildGroup, pane, relativeDock);
            ArrangeLayout();
            
        }

        public void Remove(DockablePane pane)
        {
            DockablePaneGroup groupToAttach = _rootGroup.RemovePane(pane);
            if (groupToAttach != null)
            {
                _rootGroup = groupToAttach;
                _rootGroup.ParentGroup = null;
            }

            ArrangeLayout();
        }

        public void MoveTo(DockablePane sourcePane, Pane destinationPane, Dock relativeDock)
        {
            Remove(sourcePane);
            Add(sourcePane, destinationPane, relativeDock);
        }

        public void MoveInto(DockablePane sourcePane, Pane destinationPane)
        {
            Remove(sourcePane);
            while (sourcePane.Contents.Count > 0)
            {
                DockableContent content = sourcePane.Contents[0];
                sourcePane.Remove(content);
                destinationPane.Add(content);
                destinationPane.Show(content);
            }
            sourcePane.Close();
        }

        public Pane GetPaneFromContent(DockableContent content)
        {
            return _rootGroup.GetPaneFromContent(content);
        }

        DockablePaneGroup GetPaneGroup(Pane pane)
        {
            return _rootGroup.GetPaneGroup(pane);
        }

        internal void ArrangeLayout()
        {
            //_rootGroup.SaveChildPanesSize();
            Clear(_panel);
            _rootGroup.Arrange(_panel);
            Dump(_rootGroup, 0);
        }

      
        private void Clear(Grid grid)
        {
            foreach (UIElement child in grid.Children)
            {
                if (child is Grid)
                    Clear(child as Grid);
            }

            grid.Children.Clear();
            grid.ColumnDefinitions.Clear();
            grid.RowDefinitions.Clear();
        }


        #region ILayoutSerializable Membri di

        public void Serialize(XmlDocument doc, XmlNode parentNode)
        {
            XmlNode node_rootGroup = doc.CreateElement("_rootGroup");

            _rootGroup.Serialize(doc, parentNode);

            parentNode.AppendChild(node_rootGroup);
        }

        public void Deserialize(DockManager managerToAttach, XmlNode node, GetContentFromTypeString getObjectHandler)
        {
            _rootGroup = new DockablePaneGroup();
            _rootGroup.Deserialize(managerToAttach, node, getObjectHandler);

            //_docsPane = FindDocumentsPane(_rootGroup);
            
            ArrangeLayout();
        }

        DocumentsPane FindDocumentsPane(DockablePaneGroup group)
        {
            if (group == null)
                return null;

            if (group.AttachedPane is DocumentsPane)
                return group.AttachedPane as DocumentsPane;
            else
            {
                DocumentsPane docsPane = FindDocumentsPane(group.FirstChildGroup);
                if (docsPane != null)
                    return docsPane;

                docsPane = FindDocumentsPane(group.SecondChildGroup);
                if (docsPane != null)
                    return docsPane;
            }

            return null;
        }

        #endregion
    }
}