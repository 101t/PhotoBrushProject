using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Controls.Primitives;

namespace DockingLibrary
{
    /// <summary>
    /// Interaction logic for FloatingWindow.xaml
    /// </summary>

    public partial class FloatingWindow : System.Windows.Window
    {
        private const int WM_MOVE = 0x0003;
        private const int WM_SIZE = 0x0005;
        private const int WM_NCMOUSEMOVE = 0xa0;
        private const int WM_NCLBUTTONDOWN = 0xA1;
        private const int WM_NCLBUTTONUP = 0xA2;
        private const int WM_NCLBUTTONDBLCLK = 0xA3;
        private const int WM_NCRBUTTONDOWN = 0xA4;
        private const int WM_NCRBUTTONUP = 0xA5;
        private const int HTCAPTION = 2;
        private const int SC_MOVE = 0xF010;
        private const int WM_SYSCOMMAND = 0x0112;

        internal readonly FloatingWindowHostedPane HostedPane;
        //public readonly DockablePane ReferencedPane;

        public FloatingWindow(DockablePane pane)
        {
            InitializeComponent();

            #region Hosted Pane
            HostedPane = new FloatingWindowHostedPane(this, pane);
            HostedPane.ReferencedPane.OnTitleChanged += new EventHandler(HostedPane_OnTitleChanged);
            Content = HostedPane;
            Title = HostedPane.Title;
            #endregion
        }

        void HostedPane_OnTitleChanged(object sender, EventArgs e)
        {
            Title = HostedPane.Title;
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            HostedPane.ReferencedPane.SaveFloatingWindowSizeAndPosition(this);
            HostedPane.ReferencedPane.OnTitleChanged -= new EventHandler(HostedPane_OnTitleChanged);
            HostedPane.Close();
            
            if (_hwndSource != null)
                _hwndSource.RemoveHook(_wndProcHandler);

            base.OnClosing(e);
        }

        HwndSource _hwndSource;
        HwndSourceHook _wndProcHandler;

        protected void OnLoaded(object sender, EventArgs e)
        {
            WindowInteropHelper helper = new WindowInteropHelper(this);
            _hwndSource = HwndSource.FromHwnd(helper.Handle);
            _wndProcHandler = new HwndSourceHook(HookHandler);
            _hwndSource.AddHook(_wndProcHandler);
        }
        

        private IntPtr HookHandler(
            IntPtr hwnd,
            int msg,
            IntPtr wParam,
            IntPtr lParam,
            ref bool handled
        )
        {
            handled = false;

            switch (msg)
            {
                case WM_SIZE:
                case WM_MOVE:
                    HostedPane.ReferencedPane.SaveFloatingWindowSizeAndPosition(this);
                    break;
                case WM_NCLBUTTONDOWN:
                    if (HostedPane.ReferencedPane.State == PaneState.DockableWindow && wParam.ToInt32() == HTCAPTION)
                    {
                        short x = (short)((lParam.ToInt32() & 0xFFFF));
                        short y = (short)((lParam.ToInt32() >> 16));


                        HostedPane.ReferencedPane.DockManager.Drag(this, new Point(x, y), new Point(x - Left, y - Top));

                        handled = true;
                    }
                    break;
                case WM_NCLBUTTONDBLCLK:
                    if (HostedPane.ReferencedPane.State == PaneState.DockableWindow && wParam.ToInt32() == HTCAPTION)
                    {
                        //
                        HostedPane.ReferencedPane.ChangeState(PaneState.Docked);
                        HostedPane.ReferencedPane.Show();
                        this.Close();

                        handled = true;
                    }
                    break;
                case WM_NCRBUTTONDOWN:
                    if (wParam.ToInt32() == HTCAPTION)
                    {
                        short x = (short)((lParam.ToInt32() & 0xFFFF));
                        short y = (short)((lParam.ToInt32() >> 16));

                        ContextMenu cxMenu = HostedPane.OptionsMenu;
                        cxMenu.Placement = PlacementMode.AbsolutePoint;
                        cxMenu.PlacementRectangle = new Rect(new Point(x, y), new Size(0, 0));
                        cxMenu.PlacementTarget = this;
                        cxMenu.IsOpen = true;

                        handled = true;
                    }
                    break;
                case WM_NCRBUTTONUP:
                    if (wParam.ToInt32() == HTCAPTION)
                    {

                        handled = true;
                    }
                    break;
                
            }
            

            return IntPtr.Zero;
        }
    }
}