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
    /// <summary>
    /// Interaction logic for DockPane.xaml
    /// </summary>

    public partial class DockPane : System.Windows.Controls.UserControl, IDockPane
    {
        List<DockContent> Contents = new List<DockContent>();

        public DockPane(DockContent content)
        {
            InitializeComponent();

            Contents.Add(content);
            windowContent.Content = content.Content;
        }



        #region IDockPane Membri di
        private bool _docked = false;
        public bool Docked
        {
            get { return _docked; }
        }

        private System.Windows.GridLength _width = new GridLength(100);
        private System.Windows.GridLength _height = new GridLength(100);

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