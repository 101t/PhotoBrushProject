using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace DockingLibrary
{
    public enum SplitOrientation
    { 
        Horizontal,
        Vertical
    }
        
    class PaneGroup : IPane
    {
        public IPane First;
        public IPane Second;
        public readonly SplitOrientation Split;

        public PaneGroup(IPane first, IPane second, SplitOrientation split)
        {
            First = first;
            Second = second;
            Split = split;
        }

        #region IPane Membri di

        public bool Hidden
        {
            get { return First.Hidden && Second.Hidden; }
        }


        public void AdjustSize()
        {
            First.AdjustSize();
            Second.AdjustSize();
        }

        private System.Windows.GridLength _width = new GridLength(1, GridUnitType.Star);
        private System.Windows.GridLength _height = new GridLength(1, GridUnitType.Star);

        public System.Windows.GridLength GridWidth
        {
            get
            {
                return _width;
            }
            set
            {
                throw new InvalidOperationException();
            }
        }

        public System.Windows.GridLength GridHeight
        {
            get
            {
                return _height;
            }
            set
            {
                throw new InvalidOperationException();
            }
        }

        public IPane FindPaneFromContent(ManagedContent content)
        {
            IPane pane = First.FindPaneFromContent(content);
            if (pane != null)
                return pane;
            
            pane = Second.FindPaneFromContent(content);
            if (pane != null)
                return pane;

            return null;
        }

        #endregion
    }
}
