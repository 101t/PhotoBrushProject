////////////////////////////////////////////////////////////////
//                Created By Richard Blythe 2008
//   There are no licenses or warranty attached to this object.
//   If you distribute the code as part of your application, please
//   be courteous enough to mention the assistance provided you.
//   Modified By Tarek MOH Omer Kala'ajy 2013
////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using Painting.Painter;

namespace Painting.Shapes
{
    [Serializable]
    class ShapeClipRectangle : ShapeRectangle
    {
        public ShapeClipRectangle()
        {
            painter.State.IsSelected = true;
            painter.PaintFill = false;
            painter.PaintBorder = true;
            painter.State.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
        }

        public override string ShapeType
        {
            get { return "Clipping Rectangle"; }
        }

        public override bool MouseMove(System.Windows.Forms.MouseEventArgs e)
        {
            if (isSelected && !editingOn && !mouseIsPressed)
            {
                ResizeHandles.HitTest(e.Location);
            }
            doOperation = false;
            if (mouseIsPressed)
            {
                eventData.NeedsPainted = true;
                if (isResizing)
                {
                    Rectangle oldBounds = ResizeHandles.Bounds;//InvalidationArea;
                    Resize(e.Location);
                    blnSuppressInflate = true;
                    InvalidationArea = Rectangle.Union(oldBounds, GetShapeBounds(true));
                    blnSuppressInflate = false;
                    doOperation = true;
                }
                else
                {
                    if (bmpMove == null)
                    {
                        bmpMove = new Bitmap((int)Size.Width + 2, (int)Size.Height + 2);
                        Graphics g = Graphics.FromImage(bmpMove);
                        Pen pen = new Pen(Color.Black, 1f);
                        pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
                        g.DrawRectangle(pen, new Rectangle(new Point(0,0), Size));
                        pen.Color = Color.White;
                        g.DrawRectangle(pen, new Rectangle(1, 1, Size.Width - 2, Size.Height - 2));
                        pen.Dispose();
                        pen = null;
                        g.Dispose(); g = null;
                        _Path = null;
                    }
                    Rectangle oldBounds = new Rectangle(pntBitmapPos, bmpMove.Size);
                    pntBitmapPos = new Point((int)(e.X + mouseOffset.X), (int)(e.Y + mouseOffset.Y));
                    InvalidationArea = Rectangle.Union(oldBounds,
                         new Rectangle(pntBitmapPos, bmpMove.Size));
                    doOperation = true;
                }
            }
            return doOperation;
        }

        public override void Render(Graphics g)
        {
            if (bmpMove != null)
            {
                g.DrawImageUnscaled(bmpMove, pntBitmapPos);
            }
            else
            {
                Pen pen = new Pen(Color.Black, 1f);
                pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;

                g.DrawRectangle(pen, new Rectangle(Location, Size));

                pen.Color = Color.White;
                g.DrawRectangle(pen, new Rectangle(Location.X+1,Location.Y+1, Size.Width - 2, Size.Height-2));
                pen.Dispose();
                pen = null;
            }
        }
    }
}
