////////////////////////////////////////////////////////////////
//                Created By Richard Blythe 2008
//   There are no licenses or warranty attached to this object.
//   If you distribute the code as part of your application, please
//   be courteous enough to mention the assistance provided you.
////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace Painting.Shapes
{
    public class Handle : IDisposable
    {
        public enum eHandle
        {
            UpperLeft,  UpperMiddle, UpperRight, MiddleRight,
            LowerRight, LowerMiddle, Lowerleft,  MiddleLeft, Standard
        }


        Bitmap bmpHandle;
        short handleWidth, handleHeight;
        bool isStandardCursor;

        Size demension;
        public eHandle selectedHandle;
        Point[] handlePos = {new Point(0,0), new Point(0,0)
                            ,new Point(0,0), new Point(0,0)
                            ,new Point(0,0), new Point(0,0)
                            ,new Point(0,0), new Point(0,0)};


        public Handle()
        {
            bmpHandle = new Bitmap(7,7);
            Graphics g = Graphics.FromImage(bmpHandle);
            g.FillEllipse(Brushes.White, 0,0,6,6);
            g.DrawEllipse(Pens.Black, 0, 0, 6, 6);
            g.Dispose();

            handleWidth = (short)(bmpHandle.Width);
            handleHeight = (short)bmpHandle.Height;
            demension = new Size(handleWidth, handleHeight);
        }

        private short mouseOffsetX=0;
        public short MouseOffsetX
        {
            get { return mouseOffsetX; }
        }
        private short mouseOffsetY = 0;
        public short MouseOffsetY
        {
            get { return mouseOffsetY; }
        }


        public Size Demension
        {
            get { return demension; }
        }


        private short _ShapeIndexNum;
        public short ShapeIndexNum
        {
            get { return _ShapeIndexNum; }
            set {_ShapeIndexNum = value; }
        }

        public void SetHandlePositions(RectangleF bounds)
        {
            SetHandlePositions(Rectangle.Round(bounds));
        }

        Rectangle rectBounds;
        public Rectangle Bounds
        {
            get { return rectBounds; }
        }

        public void SetHandlePositions(Rectangle bounds)
        {
            if (rectBounds == bounds) return;
            //upper left handle
            handlePos[0].X = (bounds.Left - handleWidth);
            handlePos[0].Y = (bounds.Top - handleHeight);
            //upper middle handle
            handlePos[1].X = ((bounds.Left + (bounds.Width/2)) - (handleWidth/2));
            handlePos[1].Y = handlePos[0].Y;
            //upper right handle
            handlePos[2].X = (bounds.Right);
            handlePos[2].Y = handlePos[0].Y;
            //middle right handle
            handlePos[3].X = (bounds.Right);
            handlePos[3].Y = ((bounds.Top + (bounds.Height / 2)) - (handleHeight / 2));
            //lower right handle
            handlePos[4].X = (bounds.Right);
            handlePos[4].Y = (bounds.Bottom);
            //lower middle handle
            handlePos[5].X = handlePos[1].X;//time saver
            handlePos[5].Y = (bounds.Bottom);
            //lower left handle
            handlePos[6].X = handlePos[0].X;
            handlePos[6].Y = (bounds.Bottom);
            //middle left handle
            handlePos[7].X = handlePos[0].X;
            handlePos[7].Y = handlePos[3].Y;
            
            //set the bounding area
            rectBounds = new Rectangle(bounds.X - 10,bounds.Y - 10,
                                        bounds.Width + 28, bounds.Height + 20);
            //rectBounds.Inflate(handleWidth * 2), handleHeight + (handleWidth / 2));
        }


        public bool HitTest(Point mousePos)
        {
            Rectangle curBounds = new Rectangle(0,0,bmpHandle.Width,bmpHandle.Height);
            for (short i = 0; i < 8; i++)
            {
                curBounds.X = handlePos[i].X;
                curBounds.Y = handlePos[i].Y;
                if (curBounds.Contains(mousePos))
                {
                    selectedHandle = (eHandle)i;
                    mouseOffsetX = (short)(handlePos[i].X - mousePos.X);
                    mouseOffsetY = (short)(handlePos[i].Y - mousePos.Y);
                    SetCursor();
                    return true;
                }
                   
            }
            if (!isStandardCursor)
            {
                Cursor.Current = Cursors.Default;
                isStandardCursor = true;
            }

            return false;
        }


        private void SetCursor()
        {
            switch (selectedHandle)
            {
                case Handle.eHandle.LowerMiddle:
                case Handle.eHandle.UpperMiddle:
                    if (Cursor.Current != Cursors.SizeNS)
                        Cursor.Current = Cursors.SizeNS;    
                    break;

                case Handle.eHandle.MiddleLeft:
                case Handle.eHandle.MiddleRight:
                    if (Cursor.Current != Cursors.SizeWE)
                        Cursor.Current = Cursors.SizeWE;
                    break;

                case Handle.eHandle.UpperLeft:
                case Handle.eHandle.LowerRight:
                    if (Cursor.Current != Cursors.SizeNWSE)
                        Cursor.Current = Cursors.SizeNWSE;
                    break;

                case Handle.eHandle.UpperRight:
                case Handle.eHandle.Lowerleft:
                    if (Cursor.Current != Cursors.SizeNESW)
                        Cursor.Current = Cursors.SizeNESW;
                    break;
            }
            isStandardCursor = false;
        }


        public void Render(Graphics g)
        {
            for (short i = 0; i < 8; i++)
            {
                g.DrawImageUnscaled(bmpHandle, handlePos[i]);
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (bmpHandle != null)
                bmpHandle.Dispose();
        }

        #endregion
    }
}
