using System;
using System.Collections.Generic;
using System.Text;

namespace DockingLibrary
{
    public enum SplitOrientation
    { 
        Horizontal,
        Vertical
    }
        
    class DockPaneGroup : IDockPane
    {
        public readonly IDockPane First;
        public readonly IDockPane Second;
        public readonly SplitOrientation Split;

        public DockPaneGroup(IDockPane first, IDockPane second, SplitOrientation split)
        {
            First = first;
            Second = second;
            Split = split;
        }


        #region IDockPane Membri di

        public bool Docked
        {
            get { return First.Docked && Second.Docked; }
        }

        private System.Windows.GridLength _width = System.Windows.GridLength.Auto;
        private System.Windows.GridLength _height = System.Windows.GridLength.Auto;

        public System.Windows.GridLength GridWidth
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

        public System.Windows.GridLength GridHeight
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

        #endregion
    }
}
