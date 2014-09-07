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
    class DockPanelSplitter : Border
    {
        SplitOrientation _split;
        FrameworkElement _prevControl;
        FrameworkElement _nextControl;

        public DockPanelSplitter(FrameworkElement prevControl, FrameworkElement nextControl, SplitOrientation split)
        {
            _prevControl = prevControl;
            _nextControl = nextControl;
            _split = split;
            Background = new SolidColorBrush(Colors.LightGray);

            if (_split == SplitOrientation.Vertical)
            {
                Cursor = Cursors.SizeWE;
                Width = 5;
            }
            else
            {
                Cursor = Cursors.SizeNS;
                Height = 5;
            }
        }

        Point ptStartDrag;
        Point AdjustControls(Point delta)
        {
            //Console.WriteLine(delta);
            if (_split == SplitOrientation.Vertical)
            {
                if (delta.X > 0 && _nextControl != null)
                {
                    if (_nextControl.ActualWidth - delta.X < _nextControl.MinWidth)
                        delta.X = _nextControl.ActualWidth - _nextControl.MinWidth;
                }
                else if (delta.X < 0 && _prevControl != null)
                {
                    if (_prevControl.ActualWidth + delta.X < _prevControl.MinWidth)
                        delta.X = _prevControl.MinWidth - _prevControl.ActualWidth;
                }

                if (_prevControl!=null)
                    _prevControl.Width += delta.X;
                if (_nextControl!=null)
                    _nextControl.Width -= delta.X;
            }
            else
            {
                if (delta.Y > 0 && _nextControl!=null)
                {
                    if (_nextControl.ActualHeight - delta.Y < _nextControl.MinHeight)
                        delta.Y = _nextControl.ActualHeight - _nextControl.MinHeight;
                }
                else if (delta.Y < 0 &&_prevControl != null)
                {
                    if (_prevControl.ActualHeight + delta.Y < _prevControl.MinHeight)
                        delta.Y = _prevControl.MinHeight - _prevControl.ActualHeight;

                }
                if (_prevControl != null)
                    _prevControl.Height += delta.Y;
                if (_nextControl != null)
                    _nextControl.Height -= delta.Y;
            }

            return delta;
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            if (!IsMouseCaptured)
            {
                //DockPanel parent = ((DockPanel)Parent);
                //int index = parent.Children.IndexOf(this);
                //if (index == 0 || index == parent.Children.Count - 1)
                //    return;
                //_prevControl = parent.Children[index - 1] as FrameworkElement;
                //_nextControl = parent.Children[index + 1] as FrameworkElement;


                ptStartDrag = e.GetPosition(Parent as IInputElement);
                CaptureMouse();
            }

            base.OnMouseDown(e);
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (IsMouseCaptured)
            {
                Point ptCurrontRelative = e.GetPosition(this);
                //if (ptCurrontRelative.X >= 0 && ptCurrontRelative.X <= Width)
                {
                    Point ptCurrent = e.GetPosition(Parent as IInputElement);
                    Point delta = new Point(ptCurrent.X - ptStartDrag.X, ptCurrent.Y - ptStartDrag.Y);

                    delta = AdjustControls(delta);
                        
                    ptStartDrag = new Point(ptStartDrag.X+delta.X, ptStartDrag.Y+delta.Y);
                }
                
            }
            base.OnMouseMove(e);
        }
        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            if (IsMouseCaptured)
            {
                Point ptCurrent = e.GetPosition(Parent as IInputElement);
                Point delta = new Point(ptCurrent.X - ptStartDrag.X , ptCurrent.Y - ptStartDrag.Y);
                AdjustControls(delta);
                ReleaseMouseCapture();
            }

            base.OnMouseUp(e);
        }
    }
}
