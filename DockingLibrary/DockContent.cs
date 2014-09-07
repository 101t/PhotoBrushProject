using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace DockingLibrary
{
    public class DockContent
    {
        public readonly string Title;

        public readonly object Content;

        public DockContent(Window window)
        {
            Title = window.Title;
            Content = window.Content;

            window.Content = null;
        }
    }
}
