using System;
using System.Collections.Generic;
using System.Text;

namespace DockingLibrary
{
    interface IDockPane
    {
        bool Docked { get;}
        System.Windows.GridLength GridWidth { get; set;}
        System.Windows.GridLength GridHeight { get; set;}
    }
}
