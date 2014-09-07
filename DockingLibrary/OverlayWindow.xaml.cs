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
using System.Windows.Shapes;
using System.IO;
using System.Windows.Markup;

namespace DockingLibrary
{
    class OverlayWindowDockingButton : IDropSurface
    {
        OverlayWindow _owner;
        public readonly Button _btnDock;
        public OverlayWindowDockingButton(Button btnDock, OverlayWindow owner) : this(btnDock, owner, true)
        {

        }
        public OverlayWindowDockingButton(Button btnDock, OverlayWindow owner, bool enabled)
        {
            _btnDock = btnDock;
            _owner = owner;
            Enabled = enabled;
        } 
        
        public bool Enabled = true;



        #region IDropSurface Membri di

        public Rect SurfaceRectangle
        {
            get 
            {
                if (!_owner.IsLoaded)
                    return new Rect();

                return new Rect(_btnDock.PointToScreen(new Point(0,0)), new Size(_btnDock.ActualWidth, _btnDock.ActualHeight)); 
            }
        }

        public void OnDragEnter(Point point)
        {
            
        }

        public void OnDragOver(Point point)
        {
            
        }

        public void OnDragLeave(Point point)
        {
            
        }

        public bool OnDrop(Point point)
        {
            if (!Enabled)
                return false;

            return _owner.OnDrop(_btnDock, point);
        }

        #endregion
    }

    /// <summary>
    /// Interaction logic for OverlayWindow.xaml
    /// </summary>
    public partial class OverlayWindow : System.Windows.Window
    {
        OverlayWindowDockingButton owdBottom;
        OverlayWindowDockingButton owdTop;
        OverlayWindowDockingButton owdLeft;
        OverlayWindowDockingButton owdRight;
        OverlayWindowDockingButton owdInto;

        DockManager _owner;

        public DockManager DockManager
        {
            get { return _owner; }
        }


        public OverlayWindow(DockManager owner)
        {
            //if (!Application.Current.Resources.Contains("DockDownButtonSyle"))
            //{ 
            //    using (FileStream fs = new FileStream(@"generic.xaml", FileMode.Open, FileAccess.Read))
            //    {
            //        ResourceDictionary resources = (ResourceDictionary)XamlReader.Load(fs);
            //        Application.Current.Resources.Add("DockDownButtonSyle", resources["DockDownButtonSyle"]);
            //    }
            //}

            InitializeComponent();

            _owner = owner;

            DockManager.DragPaneServices.Register(new OverlayWindowDockingButton(btnDockBottom, this));
            DockManager.DragPaneServices.Register(new OverlayWindowDockingButton(btnDockTop, this));
            DockManager.DragPaneServices.Register(new OverlayWindowDockingButton(btnDockLeft, this));
            DockManager.DragPaneServices.Register(new OverlayWindowDockingButton(btnDockRight, this));

            owdBottom   = new OverlayWindowDockingButton(btnDockPaneBottom, this, false);
            owdTop      = new OverlayWindowDockingButton(btnDockPaneTop,    this, false);
            owdLeft     = new OverlayWindowDockingButton(btnDockPaneLeft,   this, false);
            owdRight    = new OverlayWindowDockingButton(btnDockPaneRight,  this, false);
            owdInto     = new OverlayWindowDockingButton(btnDockPaneInto, this, false);



            DockManager.DragPaneServices.Register(owdBottom);
            DockManager.DragPaneServices.Register(owdTop);
            DockManager.DragPaneServices.Register(owdLeft);
            DockManager.DragPaneServices.Register(owdRight);
            DockManager.DragPaneServices.Register(owdInto);

            //gridPaneRelativeDockingOptions.Width = 88;
            //gridPaneRelativeDockingOptions.Height = 88;
        }

        void OnLoaded(object sender, EventArgs e)
        { 
            
        }

        void OnClosed(object sender, EventArgs e)
        { 
            
        }

        internal bool OnDrop(Button btnDock, Point point)
        {
            if (btnDock == btnDockBottom)
                DockManager.DragPaneServices.FloatingWindow.HostedPane.ReferencedPane.ChangeDock(Dock.Bottom);
            else if (btnDock == btnDockLeft)
                DockManager.DragPaneServices.FloatingWindow.HostedPane.ReferencedPane.ChangeDock(Dock.Left);
            else if (btnDock == btnDockRight)
                DockManager.DragPaneServices.FloatingWindow.HostedPane.ReferencedPane.ChangeDock(Dock.Right);
            else if (btnDock == btnDockTop)
                DockManager.DragPaneServices.FloatingWindow.HostedPane.ReferencedPane.ChangeDock(Dock.Top);
            else if (btnDock == btnDockPaneTop)
                DockManager.DragPaneServices.FloatingWindow.HostedPane.ReferencedPane.MoveTo(CurrentDropPane, Dock.Top);
            else if (btnDock == btnDockPaneBottom)
                DockManager.DragPaneServices.FloatingWindow.HostedPane.ReferencedPane.MoveTo(CurrentDropPane, Dock.Bottom);
            else if (btnDock == btnDockPaneLeft)
                DockManager.DragPaneServices.FloatingWindow.HostedPane.ReferencedPane.MoveTo(CurrentDropPane, Dock.Left);
            else if (btnDock == btnDockPaneRight)
                DockManager.DragPaneServices.FloatingWindow.HostedPane.ReferencedPane.MoveTo(CurrentDropPane, Dock.Right);
            else if (btnDock == btnDockPaneInto)
            {
                DockManager.DragPaneServices.FloatingWindow.HostedPane.ReferencedPane.MoveInto(CurrentDropPane);
                return true;
            }

            DockManager.DragPaneServices.FloatingWindow.HostedPane.ReferencedPane.Show();

            return true;
        }

        Pane CurrentDropPane = null;

        public void ShowOverlayPaneDockingOptions(Pane pane)
        {
            Rect rectPane = pane.SurfaceRectangle;

            Point myScreenTopLeft = PointToScreen(new Point(0, 0));
            rectPane.Offset(-myScreenTopLeft.X, -myScreenTopLeft.Y);//relative to me
            gridPaneRelativeDockingOptions.SetValue(Canvas.LeftProperty, rectPane.Left+rectPane.Width/2-gridPaneRelativeDockingOptions.Width/2);
            gridPaneRelativeDockingOptions.SetValue(Canvas.TopProperty, rectPane.Top + rectPane.Height/2-gridPaneRelativeDockingOptions.Height/2);
            gridPaneRelativeDockingOptions.Visibility = Visibility.Visible;

            owdBottom.Enabled = true;
            owdTop   .Enabled = true;
            owdLeft  .Enabled = true;
            owdRight .Enabled = true;
            owdInto  .Enabled = true;
            CurrentDropPane = pane;
        }

        public void HideOverlayPaneDockingOptions(Pane surfaceElement)
        {
            owdBottom.Enabled = false;
            owdTop.Enabled = false;
            owdLeft.Enabled = false;
            owdRight.Enabled = false;
            owdInto.Enabled = false; 
            
            gridPaneRelativeDockingOptions.Visibility = Visibility.Collapsed;
            CurrentDropPane = null;
        }
    
    }
}