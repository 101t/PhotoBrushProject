
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Xml;

namespace DockingLibrary
{
    public abstract class Pane : UserControl , IDropSurface, ILayoutSerializable
    {
        public double PaneWidth = 150;
        public double PaneHeight = 150;

        public readonly List<DockableContent> Contents = new List<DockableContent>();


        DockManager _dockManager;
        public virtual DockManager DockManager
        {
            get { return _dockManager; }
            //set { _dockManager = value; }
        }


        public Pane(DockManager dockManager):this(dockManager, null) { }

        public Pane(DockManager dockManager, DockableContent content)
        {
            _dockManager = dockManager;

            if (content != null)
                Add(content);
        }

        public virtual void Add(DockableContent content)
        {
            if (DockManager != null)
                DockManager.Add(content);
            content.SetContainerPane(this);
            Contents.Add(content);
        }

        public virtual void Remove(DockableContent content)
        {
            if (DockManager != null)
                DockManager.Remove(content);
            content.SetContainerPane(null);
            Contents.Remove(content);
        }

        public virtual void Show(DockableContent content)
        {
            System.Diagnostics.Debug.Assert(Contents.Contains(content));
        }

        public virtual void Hide(DockableContent content)
        { 
        
        }

        public virtual void Show()
        {
            DockManager.DragPaneServices.Register(this);
        }

        public virtual void Hide()
        {
            DockManager.DragPaneServices.Unregister(this);
        }

        public virtual void Close()
        {
            DockManager.DragPaneServices.Unregister(this);
        }

        public virtual void Close(DockableContent content)
        {

        }
        
        
        public virtual bool IsHidden
        {
            get { return false; }
        }

        public virtual void SaveSize()
        {

        }


        public virtual DockableContent ActiveContent
        {
            get { return null; }
            set { }
        }

        protected virtual void DragContent(DockableContent contentToDrag, Point startDragPoint, Point offset)
        {
            Remove(contentToDrag);
            DockablePane pane = new DockablePane(DockManager);
            //pane = new DockablePane();
            //pane.DockManager = DockManager;
            pane.Add(contentToDrag);
            pane.Show();
            DockManager.Add(pane);
            //DockManager.Add(contentToDrag);
            FloatingWindow wnd = new FloatingWindow(pane);
            pane.ChangeState(PaneState.DockableWindow);
            DockManager.Drag(wnd, startDragPoint, offset);
        }

        public virtual void RefreshTitle()
        { 
        
        }

        #region Membri di IDropSurface 

        public Rect SurfaceRectangle
        {
            get 
            {
                if (IsHidden)
                    return new Rect();
                
                return new Rect(PointToScreen(new Point(0,0)), new Size(ActualWidth, ActualHeight)); 
            }
        }

        public virtual void OnDragEnter(Point point)
        {
            DockManager.OverlayWindow.ShowOverlayPaneDockingOptions(this);
        }

        public virtual void OnDragOver(Point point)
        {
            
        }

        public virtual void OnDragLeave(Point point)
        {
            DockManager.OverlayWindow.HideOverlayPaneDockingOptions(this);
        }

        public virtual bool OnDrop(Point point)
        {
            return false;
        }

        #endregion

        #region ILayoutSerializable Membri di

        public virtual void Serialize(XmlDocument doc, XmlNode parentNode)
        {
            parentNode.Attributes.Append(doc.CreateAttribute("Width"));
            parentNode.Attributes["Width"].Value = PaneWidth.ToString();
            parentNode.Attributes.Append(doc.CreateAttribute("Height"));
            parentNode.Attributes["Height"].Value = PaneHeight.ToString();

            foreach (ManagedContent content in Contents)
            {
                DockableContent dockableContent = content as DockableContent;
                if (dockableContent != null)
                {
                    XmlNode nodeDockableContent = doc.CreateElement(dockableContent.GetType().ToString());
                    parentNode.AppendChild(nodeDockableContent);
                }
            }
        }

        public virtual void Deserialize(DockManager managerToAttach, XmlNode node, GetContentFromTypeString getObjectHandler)
        {
            _dockManager = managerToAttach;

            PaneWidth = double.Parse(node.Attributes["Width"].Value);
            PaneHeight = double.Parse(node.Attributes["Height"].Value);

            foreach (XmlNode nodeDockableContent in node.ChildNodes)
            {
                DockableContent content = getObjectHandler(nodeDockableContent.Name);
                Add(content);
                Show(content);
            }

            DockManager.DragPaneServices.Register(this);
        }

        #endregion

    }
}
