using System.Windows;
using System.Windows.Forms;

namespace DockingLibrary
{
    public abstract class ManagedContent : Window
    {
        protected Pane _containerPane = null;

        public DockManager DockManager;

        public ManagedContent(DockManager manager)
        {
            DockManager = manager;
        }

        public ManagedContent() { }

        public virtual new void Show()
        {
            
        }

        public virtual new void Close()
        {
            base.Close();
        }

        public Pane ContainerPane
        {
            get { return _containerPane; }
        }

        internal void SetContainerPane(Pane pane)
        {
            _containerPane = pane;
        }
    }
}
