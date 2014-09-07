using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace DockingLibrary
{
    public interface IPane
    {
        bool Hidden { get;}

        void AdjustSize();
      
        GridLength GridWidth { get; set;}

        GridLength GridHeight { get; set;}

        IPane FindPaneFromContent(ManagedContent content);
    }

    public interface IContentContainer : IPane
    { 
        List<ManagedContent> Contents {get;}

        void Add(ManagedContent content);
        void Remove(ManagedContent content);
    
    }
}
