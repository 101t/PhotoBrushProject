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
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Painting.Shapes
{
    [Serializable]
    public class ShapeLine : Shape 
    {
        PointF[] points = { new Point(int.MaxValue, int.MaxValue),
                           new Point(int.MaxValue, int.MaxValue),
                           new Point(int.MaxValue, int.MaxValue),
                           new Point(int.MaxValue, int.MaxValue)};
        /// <summary>
        /// This pen is only used to hit test the outline.
        /// It is not used in any drawing operations.
        /// </summary>
        [NonSerialized]
        Pen hitTestPen;
        [NonSerialized]
        short selectedNode;
        [NonSerialized]
        bool blnStraightened;

        public ShapeLine() :base(0)
        {
            painter = new Painters(new Font("Times New Roman", 10), 2);
            painter.State.AcceptsEditing = true;
            hitTestPen = new Pen(Color.Black, 4);
            editingOn = true;
            painter.State.IsEditing = true;
            isInitializing = true;
        }

        protected override void SetProperties()
        { /*No properties to set*/}

        private bool isInitializing;
        public bool IsInitializing
        {
            get { return isInitializing; }
        }

        public override bool HasNodes
        {
            get { return true; }
        }

        protected override void GeneratePath()
        {
            _Path = new GraphicsPath();
            if (points[0].X == int.MaxValue)
            {
                _Path.AddLine(int.MaxValue, int.MaxValue, int.MaxValue, int.MaxValue);
                invalidationArea = new Rectangle(0, 0, 0, 0);
            }
            else if (points[3].X == int.MaxValue)
            {
                _Path.AddLine(points[0].X, points[0].Y, points[0].X + 1, points[0].Y);
                InvalidationArea = new Rectangle(Point.Ceiling(points[0]), new Size(2, 2));
            }
            else
            {
                _Path.AddCurve(points);
                if (!blnRefreshInvalidate)
                  InvalidationArea = Rectangle.Round(_Path.GetBounds());
            }
                
        }
        protected override void GeneratePath(Rectangle bounds)
        {
            Size = bounds.Size;
            Location = bounds.Location;
            _Path = new GraphicsPath();
            _Path.AddRectangle(bounds);
        }

        public override bool HitTest(MouseEventArgs e)
        {
            //we only need the Location of the mouse but by passing
            //the MouseEventArgs, no value copying has to occur.
            //Only the memory reference will be passed

            if (isSelected && InvalidationArea.Contains(e.Location) || isInitializing)
                return true;
            else if (Path.IsVisible(e.Location))
                return true;
            else if (Path.IsOutlineVisible(e.Location,hitTestPen))
                return true;

            return false;
        }

        public override EventData MouseDown(System.Windows.Forms.MouseEventArgs e)
        {
            ResetEventData();
            if (isInitializing)
            {
                if (points[0].X == int.MaxValue && points[3].X == int.MaxValue)
                {
                    points[0] = e.Location;
                    InvalidationArea = new Rectangle(Point.Ceiling(points[0]), new Size(2, 2));
                }
                else if (points[3].X == int.MaxValue)
                {
                    points[3] = e.Location;
                    for (int i = 1; i < 3; ++i)
                    {
                        float frac = (float)i / (float)(3);
                        Point mid = new Point((int)(points[0].X + frac * (points[3].X - points[0].X)),
                                              (int)(points[0].Y + frac * (points[3].Y - points[0].Y)));
                        points[i] = mid;
                    }
                    _Path = null;
                    //InvalidationArea = GetShapeBounds(true);
                    eventData.FinalizeShape = true;
                    isInitializing = false;
                }

                eventData.WasHit = true;
                eventData.NeedsPainted = true;
            }
            else
            {
                selectedNode = -1;
                eventData.WasHit = HitTest(e);
                if (eventData.WasHit)
                {
                    isSelected = true;
                    mouseIsPressed = true;
                    if (!editingOn)
                    {
                        mouseOffset = new PointF(Path.GetBounds().X - e.X, Path.GetBounds().Y - e.Y);
                        pntMoveStartPos = Point.Round(Path.GetBounds().Location);
                        rectOldBounds = GetShapeBounds(true);
                        isResizing = (ResizeHandles.HitTest(e.Location));

                    }
                    else
                    {
                        RectangleF rectF = new RectangleF(0, 0, 6, 6);
                        for (short i =0;i<4;i++)
                        {
                            rectF.Location = new PointF(points[i].X - 3, points[i].Y - 3);
                            if (rectF.Contains(e.Location))
                            {
                                selectedNode = i;
                                i = 4; //terminate the loop
                            }
                        }
                        if (e.Clicks > 1)
                        {
                            for (int i = 1; i < 3; ++i)
                            {
                                float frac = (float)i / (float)(3);
                                Point mid = new Point((int)(points[0].X + frac * (points[3].X - points[0].X)),
                                                      (int)(points[0].Y + frac * (points[3].Y - points[0].Y)));
                                points[i] = mid;
                            }
                            _Path = null;
                            blnRefreshInvalidate = true;
                            blnStraightened = true;
                        }
                    }
                    eventData.NeedsPainted = true;
                }
                else
                {
                    if (isSelected)
                    {
                        isSelected = false;
                        eventData.NeedsPainted = true;
                    }
                }
            }

            return eventData;
        }

        public override bool MouseMove(System.Windows.Forms.MouseEventArgs e)
        {
            doOperation = false;
            if (isSelected && !editingOn && !mouseIsPressed)
            {
                ResizeHandles.HitTest(e.Location);
            }

            if (!mouseIsPressed) return false;



            if (!isInitializing && editingOn && selectedNode != -1)
            {
                RectangleF oldBounds = Path.GetBounds();
                points[selectedNode] = e.Location;

                _Path = null;
                if (Path.GetBounds().Contains(oldBounds))
                    InvalidationArea = GetShapeBounds(true);
                else
                    InvalidationArea = Rectangle.Round(oldBounds);
                doOperation = true;
            }
            else if (!editingOn)
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
                        Rectangle rectBounds = GetShapeBounds(true);

                        bmpMove = new Bitmap((int)rectBounds.Width+2, (int)rectBounds.Height+2);
                        Graphics g = Graphics.FromImage(bmpMove);

                        PointF[] arrTmp = new PointF[4];
                        points.CopyTo(arrTmp, 0);

                        Point tmp = new Point(0 - rectBounds.X,
                                                0 - rectBounds.Y);
                        for (short i = 0; i < 4; i++)
                        {
                            arrTmp[i] = new PointF((points[i].X + tmp.X),
                                                   (points[i].Y + tmp.Y));
                        }
                        g.DrawCurve(Pens.MediumBlue, arrTmp);

                        arrTmp = null;
                        g.Dispose(); g = null;
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

        public override EventData MouseUp(System.Windows.Forms.MouseEventArgs e)
        {
            ResetEventData();
            mouseIsPressed = false;
            eventData.WasHit = (HitTest(e) || blnStraightened);

            if (isResizing)
            {
                isResizing = false;
                Rectangle bounds = GetShapeBounds(true);
                float percentX, percentY;
                bool isLeftAnchored = bounds.X == rectOldBounds.X;
                bool isTopAnchored = bounds.Y == rectOldBounds.Y;
                int compensateX = isLeftAnchored ? rectOldBounds.X : rectOldBounds.Right;
                int compensateY = isTopAnchored ? rectOldBounds.Y : rectOldBounds.Bottom;
                int widthChange = bounds.Width - rectOldBounds.Width;
                int heightChange = bounds.Height - rectOldBounds.Height;

                for (short i = 0; i < 4; i++)
                {
                    if (isLeftAnchored)
                    {
                        if (points[i].X - compensateX != 0)
                            percentX = (points[i].X - compensateX) / rectOldBounds.Width;
                        else
                            percentX = 0.0f;
                    }
                    else
                    {
                        if (compensateX - points[i].X != 0)
                            percentX = (points[i].X - compensateX) / rectOldBounds.Width;
                        else
                            percentX = 0.0f;
                    }

                    if (isTopAnchored)
                    {
                        if (points[i].Y - compensateY != 0)
                            percentY = (points[i].Y - compensateY) / rectOldBounds.Height;
                        else
                            percentY = 0.0f;
                    }
                    else
                    {
                        if (compensateY - points[i].Y != 0)
                            percentY = (points[i].Y - compensateY) / rectOldBounds.Height;
                        else
                            percentY = 0.0f;
                    }


                    points[i] = new PointF((points[i].X + widthChange * percentX),
                                           (points[i].Y + heightChange * percentY));
                }
                _Path = null;
                this.ResizeHandles.SetHandlePositions(Path.GetBounds());
                eventData.NeedsPainted = true;

            }
            if (bmpMove != null)
            {
                bmpMove = null;
                mouseOffset.X = pntBitmapPos.X - pntMoveStartPos.X;
                mouseOffset.Y = pntBitmapPos.Y - pntMoveStartPos.Y;
                for (short i = 0; i < 4; i++)
                {
                    points[i] = new PointF((points[i].X + mouseOffset.X),
                                           (points[i].Y + mouseOffset.Y));
                }
                _Path = null;
                this.ResizeHandles.SetHandlePositions(Path.GetBounds());
                eventData.NeedsPainted = true;
            }
            blnStraightened = false;
            return eventData;
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
                if (points[1].X != int.MaxValue)
                    painter.PaintCurve(g, points);
            }
        }

        public override void RenderHandles(Graphics g)
        {
            if (bmpMove != null) return;
            if (editingOn && (isSelected || isInitializing))
            {
                SolidBrush brFill = new SolidBrush(Color.White);
                Pen penBorder = new Pen(Color.Black);
                for (short i = 0; i < 4; i++)
                {
                    if (points[i].X != int.MaxValue)
                    {
                        g.FillRectangle(brFill, points[i].X - 3, points[i].Y - 3, 6, 6);
                        g.DrawRectangle(penBorder, points[i].X - 3, points[i].Y - 3, 6, 6);
                    }
                }
                penBorder.Dispose();
                brFill.Dispose();
            }
            else if (_isSelected && !editingOn)
            {
                //if the shape is selected, paint the resize handles
                ResizeHandles.Render(g);
            }
        }

        public override string EmitGDICode(string graphicsName, GDIGenerationTool helper)
        {
            string str = "//Line: " + this.Name + "\r\n" +
            helper.GeneratePaintToolInit(this) +

            generatePointArray(helper) +

            "//Paint the shape\r\n";
                str += graphicsName + ".DrawCurve(" + this.Name + "Pen," + this.Name + "Points);\r\n";

            str += helper.GeneratePaintToolCleanup(this);
            return str;
        }

        public string generatePointArray(GDIGenerationTool helper)
        {
            return "PointF[] " + this.Name + "Points = { " +
               "new PointF(" + (points[0].X - helper.AreaBounds.X) + "f," + 
                   (points[0].Y - helper.AreaBounds.Y) + "f)," +
               "new PointF(" + (points[1].X - helper.AreaBounds.X) + "f," +
                   (points[1].Y - helper.AreaBounds.Y) + "f)," +
               "new PointF(" + (points[2].X - helper.AreaBounds.X) + "f," +
                   (points[2].Y - helper.AreaBounds.Y) + "f)," +
               "new PointF(" + (points[3].X - helper.AreaBounds.X) + "f," + 
                   (points[3].Y - helper.AreaBounds.Y) + "f)\r\n};\r\n" +

               "ScalePoints(ref " + this.Name + "Points, shapeBoundsOld, shapeBoundsNew);\r\n";          
                
        }

        public override Shape Clone()
        {
            ShapeRectangle newShape = new ShapeRectangle();
            newShape.Location = new Point(this.Location.X + 15, this.Location.Y + 15);
            newShape.Size = this.Size;
            newShape.painter.CopyProperties(ref this.painter);
            return newShape;
        }

        protected override void Restore_NonSerialized()
        {
            hitTestPen = new Pen(Color.Black, 4);
        }

        public override string ShapeType
        {
            get { return "Line"; }
        }
    }
}
