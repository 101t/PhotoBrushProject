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
using Painting.Painter;

namespace Painting.Shapes
{
    [Serializable]
    public class ShapeTriangle : Shape
    {
        public ShapeTriangle() : base(0)
        {
            painter = new Painters(new Font("Times New Roman",10),2);
            Size = new Size(100, 100);
        }

        public override bool HasNodes
        {
            get { return false; }
        }

        protected override void SetProperties()
        { /*No properties to set*/}

        protected override void GeneratePath()
        {
            //Creates a triangle shape
            _Path = new System.Drawing.Drawing2D.GraphicsPath();
            _Path.AddLine(Location.X + (Size.Width/2), Location.Y,
                          Location.X + Size.Width, Location.Y+Size.Height);
            _Path.AddLine(Location.X + Size.Width, Location.Y+Size.Height,
                          Location.X, Location.Y+Size.Height);
            _Path.AddLine(Location.X, Location.Y+Size.Height,
              Location.X + (Size.Width/2), Location.Y);
        }

        protected override void GeneratePath(Rectangle bounds)
        {
            Size = bounds.Size;
            Location = bounds.Location;
            GeneratePath();
        }

        public override string EmitGDICode(string graphicsName, GDIGenerationTool helper)
        {
            Rectangle rect = new Rectangle(Location.X - helper.AreaBounds.X,
                        Location.Y - helper.AreaBounds.Y,
                        Size.Width, Size.Height);
            string str =

            helper.GeneratePaintToolInit(this) +

            generatePointArray(rect) +
            generateTypeArray() +


            "ScalePoints(ref " + this.Name + "Points, (float)shapeBoundsNew.Width / shapeBoundsOld.Width,\r\n" +
                    "(float)shapeBoundsNew.Height / shapeBoundsOld.Height);\r\n//\r\n" +
            "//Create the path\r\npath = new GraphicsPath(" + this.Name + "Points," + this.Name + "Types);\r\n" +
            "path.CloseAllFigures();\r\n";

             if (painter.PaintFill)
                 str += graphicsName + ".FillPath(" + this.Name + "Brush, path);\r\n";
             if (painter.PaintBorder)
                 str += graphicsName + ".DrawPath(" + this.Name + "Pen, path);\r\n";

             str += helper.GeneratePaintToolCleanup(this);
             helper.GenerateScalePoints();
             return str;
            
        }

        public string generatePointArray(Rectangle rect)
        {

            return "PointF[] " + this.Name + "Points = { " +
               "new PointF(" + (rect.X + (rect.Width / 2)) + "," + rect.Y + ")," +
               "new PointF(" + rect.Right + "," + rect.Bottom + ")," +
               "new PointF(" + rect.Left + "," + rect.Bottom + ")," +
               "new PointF(" + (rect.X + (rect.Width / 2)) + "," + rect.Y + ")\r\n};\r\n";

        }

        public string generateTypeArray()
        {

            return "byte[] " + this.Name + "Types = {0,1,1,1};\r\n";
        }

        public override bool MouseMove(System.Windows.Forms.MouseEventArgs e)
        {
            doOperation = false;
            if (isSelected && !editingOn && !mouseIsPressed)
            {
                ResizeHandles.HitTest(e.Location);
            }
            if (mouseIsPressed)
            {
                if (isResizing)
                {
                    Rectangle oldBounds = InvalidationArea;
                    Resize(e.Location);
                    InvalidationArea = Rectangle.Union(oldBounds,
                        GetShapeBounds(true));
                    doOperation = true;
                }
                else
                {
                    if (bmpMove == null)
                    {
                        RectangleF rectBounds = Path.GetBounds();
                        Point tmpLocation = Point.Round(rectBounds.Location);
                        Size tmpSize = Size.Round(rectBounds.Size);
                        GeneratePath(new Rectangle(0, 0, (int)rectBounds.Width, (int)rectBounds.Height));

                        bmpMove = new Bitmap((int)rectBounds.Width, (int)rectBounds.Height);
                        Graphics g = Graphics.FromImage(bmpMove);
                        bool blnTemp = painter.PaintFill;
                        painter.PaintFill = false;
                        painter.Paint(g, Path);
                        painter.PaintFill = blnTemp;
                        g.Dispose(); g = null;

                        Location = tmpLocation;
                        Size = tmpSize;

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
                return;
            }
            if (isResizing)
            {
                bool blnTmp = painter.PaintFill;
                painter.PaintFill = false;
                painter.Paint(g, Path);
                painter.PaintFill = blnTmp;
            }
            else
            {
                painter.Paint(g, Path);
            }
        }

        public override Shape Clone()
        {
            ShapeTriangle newShape = new ShapeTriangle();
            newShape.Location = new Point(this.Location.X + 15, this.Location.Y + 15);
            newShape.Size = this.Size;
            newShape.painter.CopyProperties(ref this.painter);
            return newShape;
        }

        public override string ShapeType
        {
            get { return "Triangle"; }
        }

        protected override void Restore_NonSerialized()
        {
            //No variables to restore from a null state.
        }
    }
}
