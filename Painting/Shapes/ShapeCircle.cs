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
    public class ShapeCircle : Shape
    {
        public ShapeCircle() : base(0)
        {
            painter = new Painters(new Font("Times New Roman", 10), 2);
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
            _Path = new System.Drawing.Drawing2D.GraphicsPath();
            _Path.AddEllipse(new Rectangle(this.Location, this.Size));
        }

        protected override void GeneratePath(Rectangle bounds)
        {
            this.Size = bounds.Size;
            Location = bounds.Location;
            GeneratePath();
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

        public override string EmitGDICode(string graphicsName, GDIGenerationTool helper)
        {
            string str = "//Circle: " + this.Name + "\r\n" +
                helper.GeneratePaintToolInit(this) +

            "//Paint the shape\r\n";
            if (painter.PaintFill)
                str += graphicsName + ".FillEllipse("+this.Name+"Brush, shapeBoundsNew);\r\n";
            if (painter.PaintBorder)
                str += graphicsName + ".DrawEllipse("+this.Name+"Pen, Rectangle.Round(shapeBoundsNew));\r\n";
            
            str += helper.GeneratePaintToolCleanup(this);
            return str;
        }

        public override Shape Clone()
        {
            ShapeCircle newShape = new ShapeCircle();
            newShape.Location = new Point(this.Location.X + 15, this.Location.Y + 15) ;
            newShape.Size = this.Size;
            newShape.painter.CopyProperties(ref this.painter);
            return newShape;
        }

        protected override void Restore_NonSerialized()
        {
            //No variables to restore from a null state.
        }

        public override string ShapeType
        {
            get { return "Circle"; }
        }

    }
}
