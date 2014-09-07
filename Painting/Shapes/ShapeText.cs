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
    public class ShapeText : Shape , IDisposable
    {
        /// <summary>
        /// This graphicsPath is used to optimize text drawing 
        /// </summary>
        [NonSerialized]
        GraphicsPath gpText;
        short intBmpOffsetX = 0;
        bool blnBorderThicknessApplied;
        private Color FlavorColor = Color.Blue;

        //to prevent typos
        public const string EDIT_TEXT = "Edit Text";
        public const string SCALE_TEXT = "Scale Text";

        public override bool HasNodes
        {
            get { return false; }
        }

        protected string text = "Text";
        public string Text
        {
            get {return text;}
            set 
            {
                if (text != value)
                {
                    text = value;
                    gpText = null;
                    _Path = null;
                    bmpBuffer = null;
                    this.ResizeHandles.SetHandlePositions(GetShapeBounds(true));
                }
            }
        }

        protected Font font = new Font("Segoe UI", 12.0f);
        public Font Font
        {
            get {return font;}
            set 
            {
                bool blnChange = false;
                if (font.Name != value.Name)
                    blnChange = true;
                if (font.Size != value.Size)
                    blnChange = true;
                if (font.Style != value.Style)
                    blnChange = true;
                if (font.Strikeout != value.Strikeout)
                    blnChange = true;
                if (font.Underline != value.Underline)
                    blnChange = true;
                if (blnChange)
                {
                    font = value;
                    gpText = null;
                    _Path = null;
                    bmpBuffer = null;
                }
            }
        }

        /// <summary>
        /// The proccess of drawing text is an intensive operation.
        /// Further more, we are giving the user the option of drawing
        /// the text in gradient colors.  Instead of the conventional
        /// method of drawing the shapes contents at render time, we
        /// draw the string to this buffer and only redraw when a property
        /// change ocurrs.
        /// </summary>
        [NonSerialized]
        protected Bitmap bmpBuffer;
        protected Bitmap Buffer
        {
            get
            {
                if (bmpBuffer == null)
                    GeneratePath();
                return bmpBuffer;
            }
        }
        [NonSerialized]
        protected SizeF BufferSize;

        [NonSerialized]
        protected StringFormat format = new StringFormat();
        public StringAlignment Alignment
        {
            get { return format.Alignment; }
            set
            {
                format.Alignment = value;
                alignment = (int)format.Alignment;
                _Path = null;
                gpText = null;
            }
        }

        /// <summary>
        /// The StringFormat cannot be serialized so we store it's enum
        /// properties in integers to serialize.  Note: This variable is
        /// not to be used in any drawing operations. Use the Format property instead.
        /// </summary>
        protected int alignment;
        [NonSerialized]
        bool isMoving;
        [NonSerialized]
        bool isInitializing = true;

        public ShapeText() : base(1)
        {
            _Size = new Size(100, 100);
            painter = new Painters(new Font("Times New Roman", 10), 2);
            painter.State.AcceptsEditing = true;
            painter.ChangeColor(0, Color.Black);
            painter.PaintBorder = false;
            editingOn = false;
            isSelected = true;
        }

        protected override void SetProperties()
        {
            GdiProperties.AddProperty(SCALE_TEXT, true,
                "Set the value to true if you want the text to shrink "+
                "and grow with resizes. "+
                "If the value is set to false, the text will align " +
                "itself within the area of the scaled bounds. (If the " +
                "bounds it set to scale of course).");
            GdiProperties.AddProperty(EDIT_TEXT, false,
                "Set the value to true if you want to alter the text " +
                "after code generation. If the value is false, the text " +
                "will not be allowed to change");
        }

        protected override void GeneratePath()
        {
            if (isResizing && _Path == null)
            {
                GeneratePath(new Rectangle(Location,Size));
                return;
            }
            /////////////////////////////////////////////////////////////////
            //Since the proccess of drawing text is time and cpu intensive,
            //I am drawing the the text to a back buffer that will be painted
            //in the Render() method.  The problem is, we only want the buffer
            //to be the same size as the text size.  For example, we don't
            //want the shape size to be 400x200 if the text's dimensions only
            //require 50x28 for the word: "Hello".
            //I have attempted to solve that problem by writing some algorithms
            //that adjusts values to produce a tighter buffer size.  Most of
            //calculations have came by trial and error.  I have tested a variety
            //of fonts using these values and they all work fine.
            /////////////////////////////////////////////////////////////////

            float percentange = 0.0f;

            if (painter.ColorCount > 1 || painter.PaintBorder)
            {
                setGraphicsPath();
            }
            else
            {
                if (gpText != null)
                {
                    gpText.Dispose();
                    gpText = null;
                }
                Bitmap bmpTemp = new Bitmap(1, 1);
                Graphics gr = Graphics.FromImage(bmpTemp);
                int intChars = 0;
                int intLines = 0;
                //the .70 percent that is in this calulation allows us to get
                //closer to the right edge of the shape.  Under normal 
                //circumstances, the format object will wrap text when there
                //is sometimes physical space left to draw the character.
                BufferSize = gr.MeasureString(text, font, new SizeF(Size.Width + (font.Size * .70f), Size.Height + 10), format, out intChars, out intLines);
                gr.Dispose();
                bmpTemp.Dispose();
                bmpTemp = null;
                gr = null;
                _Path = new GraphicsPath(); //this must be set for invalidation to occur.
               
                //If you have had any experience drawing text, you will know
                //that the bigger a font becomes, the more it scales from the
                //top left corner of the bounding area.  This code will 
                //determine a the alignment setting and set the percentage
                //var to "scoot" the shape back against the top-left of the
                //buffer. Again, these values came by trial and error.
                if (format.Alignment == StringAlignment.Near)
                    percentange = .20f;
                else if (format.Alignment == StringAlignment.Center)
                    percentange = .35f;
                else if (format.Alignment == StringAlignment.Far)
                {
                    percentange = .60f;
                    if (font.Italic || font.Bold)
                        percentange += .15f;
                }
            }

            if (isInitializing)
            {
                _Size.Width = (int)BufferSize.Width;
                _Size.Height = (int)BufferSize.Height;
                isInitializing = false;
            }
            else
            {
                //only inflate the BufferSize once.
                if (!blnBorderThicknessApplied)
                {

                    BufferSize.Width += painter.IntBorderThickness;
                    BufferSize.Height += painter.IntBorderThickness;
                    blnBorderThicknessApplied = true;
                }
                //make sure the BufferSize is not greater than the shape size.
                if (BufferSize.Width > _Size.Width)
                    BufferSize.Width = _Size.Width;
                if (BufferSize.Height > _Size.Height)
                    BufferSize.Height = _Size.Height;

                if (BufferSize.Width < 1)
                {
                    BufferSize.Width = 1;
                    BufferSize.Height = 1;
                }
            }
            //create a new back buffer with the current BufferSize values
            if (BufferSize.Width > 1 || (BufferSize.Width == 1 && bmpBuffer.Width != 1))
                bmpBuffer = new Bitmap((int)BufferSize.Width, (int)BufferSize.Height);

            if (BufferSize.Width > 1)
            {
                //This code will allow the buffer to be offset by the appropriate
                //amount to simulate the current alignment.            
                if (format.Alignment == StringAlignment.Near)
                    intBmpOffsetX = 0;
                else if (format.Alignment == StringAlignment.Center)
                    intBmpOffsetX = ((short)((Size.Width - BufferSize.Width) / 2));
                else if (format.Alignment == StringAlignment.Far)
                    intBmpOffsetX = (short)((Size.Width - BufferSize.Width));

                Graphics g = Graphics.FromImage(bmpBuffer);
                g.SmoothingMode = SmoothingMode.AntiAlias;
                //g.DrawRectangle(Pens.Red, 0, 0, bmpBuffer.Width - 1, bmpBuffer.Height - 2);

                //////////////////////////////////
                //        Draw The Text
                if (gpText != null)
                {
                    //We need to translate the coords to paint onto the buffer.
                    if (format.Alignment == StringAlignment.Far &&
                        (font.Bold || font.Italic))
                        g.TranslateTransform(-(intBmpOffsetX + (font.Size * .35f)), 0);
                    else
                        g.TranslateTransform(-intBmpOffsetX, 0);
                    painter.Paint(g, gpText);
                }
                else
                {
                    SolidBrush br = new SolidBrush(painter.GetColor(0));
                    g.DrawString(text, font, br, new RectangleF(
                                -(font.Size * percentange), -(font.Size * .2f),
                                bmpBuffer.Width + (font.Size * .70f), bmpBuffer.Height + 10), format);
                    br.Dispose();
                }
                g.Dispose();

                this.ResizeHandles.SetHandlePositions(GetShapeBounds(true));
                invalidationArea = this.ResizeHandles.Bounds;
            }
        }

        protected override void GeneratePath(Rectangle bounds)
        {
            //Generates a rubber band for resizing
            Size = bounds.Size;
            Location = bounds.Location;
            _Path = new GraphicsPath();
            _Path.AddRectangle(bounds);
        }

        #region GraphicsPath Text

        private void setGraphicsPath()
        {
            if (gpText == null && (text != "" || text != null))
            {
                //this path is not used but it cannot be null 
                //for invalidation to function correctly
                _Path = new GraphicsPath();

                gpText = new GraphicsPath();
                if (!isInitializing)
                    gpText.AddString(text, font.FontFamily, (int)font.Style, font.Size * 1.3f, new RectangleF(
                        -(font.Size * .2f), -(font.Size * .2f),
                        Size.Width + (font.Size * .4f), Size.Height), format);
                else
                {
                    gpText.AddString(text, font.FontFamily, (int)font.Style, font.Size, new RectangleF(0, 0, 1000, 1000), format);
                }

                //The graphicsPath.GetBounds() method does not return the
                //true dimensions.  This algorithm will.


                //get a copy of the array for faster access;
                PointF[] tmpPoints = gpText.PathPoints;
                int index = 0;
                int maxIndex = 0;
                int lessXCount = 0;
                float maxX = 0;
                float maxY = 0;
                float minX = float.MaxValue;

                //What we are about to do is make three passes into the 
                //point array of the graphics path.  On each run, we will
                //find the maximum X position in that area.  We will then 
                //select the area with the right-most value and comb through
                //it in greater detail to find the exact right-most point.
                //NOTE: We won't make the three passes if the point length
                //      is less than 300.
                if (tmpPoints.Length < 300)
                {
                    maxIndex = tmpPoints.Length;
                    for (index = 0; index < maxIndex; index += 4)
                    {
                        if (tmpPoints[index].X > maxX) maxX = tmpPoints[index].X;
                        if (tmpPoints[index].X < minX) minX = tmpPoints[index].X;
                    }
                }
                else
                {
                    int i = 0;
                    index = 0;
                    maxIndex = (int)(tmpPoints.Length * .40f);
                    //note that we are "jumping" through every 16th point
                    //to speed up the algorithm.
                    for (; i < maxIndex; i += 16)
                    {
                        if (tmpPoints[i].X > tmpPoints[index].X) index = i;
                        if (tmpPoints[i].X < minX) minX = tmpPoints[i].X;
                    }

                    int index2 = 0;
                    i = (int)(tmpPoints.Length * .40f);
                    maxIndex = (int)(tmpPoints.Length * .80f);
                    for (; i < maxIndex; i += 16)
                        if (tmpPoints[i].X > tmpPoints[index2].X) index2 = i;

                    int index3 = 0;
                    i = (int)(tmpPoints.Length * .80f);
                    maxIndex = tmpPoints.Length;
                    for (; i < maxIndex; i += 16)
                        if (tmpPoints[i].X > tmpPoints[index3].X) index3 = i;

                    //now that we've triangulated the approximate position
                    //of the maximum X. We will refine the measurement
                    if (tmpPoints[index].X >= tmpPoints[index2].X &&
                        tmpPoints[index].X > tmpPoints[index3].X)
                    {
                        i = index;
                        maxIndex = tmpPoints.Length;
                        for (; i < maxIndex; i += 8)
                        {
                            if (tmpPoints[i].X > tmpPoints[index].X)
                            { index = i; lessXCount = 0; }
                            else
                            {
                                lessXCount++;
                                if (lessXCount > 40) i = maxIndex;
                            }
                        }

                        //set the right-most position
                        maxX = tmpPoints[index].X;

                    }
                    // If the second pass produced a greater value
                    //than the first and third pass...
                    else if (tmpPoints[index2].X >= tmpPoints[index].X &&
                        tmpPoints[index2].X > tmpPoints[index3].X)
                    {
                        i = index2;
                        maxIndex = tmpPoints.Length;
                        for (; i < maxIndex; i += 8)
                        {
                            if (tmpPoints[i].X > tmpPoints[index2].X)
                            { index2 = i; lessXCount = 0; }
                            else
                            {
                                lessXCount++;
                                if (lessXCount > 40) i = maxIndex;
                            }
                        }
                        //set the right-most position
                        maxX = tmpPoints[index2].X;
                    }
                    else
                    {   //The third pass must have produced a greater value
                        //than the previous two passes.
                        i = index3;
                        maxIndex = tmpPoints.Length;
                        for (; i < maxIndex; i += 8)
                        {
                            if (tmpPoints[i].X > tmpPoints[index3].X)
                            { index3 = i; lessXCount = 0; }
                            else
                            {
                                lessXCount++;
                                if (lessXCount > 40) i = maxIndex;
                            }
                        }
                        //set the right-most position
                        maxX = tmpPoints[index3].X;
                    }

                }

                //Now we'll scan a portion of the last section to ensure 
                //the height is set correctly
                index = tmpPoints.Length - 100 < 0 ? 0 : tmpPoints.Length - 100;
                maxIndex = tmpPoints.Length;
                for (; index < maxIndex; index += 4)
                {
                    if (tmpPoints[index].Y > maxY) maxY = tmpPoints[index].Y;
                }
                //This var is used in the GeneratePath method
                blnBorderThicknessApplied = false;

                //Trim the buffer width to a tighter area. (save memory)
                if (format.Alignment == StringAlignment.Near)
                    BufferSize.Width = (int)maxX - (minX - 10);
                else
                    BufferSize.Width = (int)maxX;
                //Applying this calulation will ensure enough height.
                BufferSize.Height = (int)(maxY + (font.Size * .4));
            }
            else
            {
                if (text == "" || text == null)
                   BufferSize.Width = 0;
            }
        }

        #endregion

        public override bool HitTest(MouseEventArgs e)
        {
            //we only need the Location of the mouse but by passing
            //the MouseEventArgs, no value copying has to occur.
            //Only the memory reference will be passed
            if (!isSelected && BufferSize.Width > 1)
                return new Rectangle(Location.X + intBmpOffsetX, Location.Y,
                        (int)Buffer.Size.Width, (int)BufferSize.Height).Contains(e.Location); ;

            return InvalidationArea.Contains(e.Location);
        }

        public override EventData MouseDown(System.Windows.Forms.MouseEventArgs e)
        {
            ResetEventData();

            eventData.WasHit = HitTest(e);
            if (eventData.WasHit)
            {
                if (!isSelected)
                {
                    isSelected = true;
                    GeneratePath();//draw the text: selected
                }
                if (e.Clicks > 1)
                {
                    OnEditorRequested();
                }
                else
                {
                    mouseIsPressed = true;
                    if (!editingOn)
                    {
                        mouseOffset = new PointF(GetShapeBounds(true).X - e.X, GetShapeBounds(true).Y - e.Y);
                        pntMoveStartPos = Location;
                        rectOldBounds = GetShapeBounds(true);
                        isResizing = (ResizeHandles.HitTest(e.Location));
                    }
                }            
                eventData.NeedsPainted = true;
            }
            else
            {
                if (isSelected)
                {
                    isSelected = false;
                    GeneratePath(); //redraw the text: unselected
                    eventData.NeedsPainted = true;
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

            if (!editingOn)
            {
                if (isResizing)
                {
                    Rectangle oldBounds = ResizeHandles.Bounds;//InvalidationArea;
                    Resize(e.Location);
                    blnSuppressInflate = true;
                    InvalidationArea = Rectangle.Union(oldBounds,
                        GetShapeBounds(true));
                    blnSuppressInflate = false;
                    doOperation = true;
                }
                else
                {
                    Rectangle oldBounds = new Rectangle(Location, Size);
                    oldBounds.Inflate(12, 12);
                    Location = new Point((int)(e.X + mouseOffset.X), (int)(e.Y + mouseOffset.Y));
                    InvalidationArea = Rectangle.Union(oldBounds,
                         new Rectangle(Location, Size));
                    //invalidationArea.Inflate(4, 4);
                    isMoving = true;
                    doOperation = true;
                    _Path = new GraphicsPath();
                }
            }
            return doOperation;
        }

        public override EventData MouseUp(System.Windows.Forms.MouseEventArgs e)
        {
            ResetEventData();
            mouseIsPressed = false;
            eventData.WasHit = HitTest(e);
            eventData.HoldPosition = eventData.WasHit;

            if (isResizing)
            {
                eventData.WasHit = true;
                Rectangle bounds = GetShapeBounds(true);
                isResizing = false;
                _Path = null;
                gpText = null;
                bmpBuffer = null;

                this.ResizeHandles.SetHandlePositions(GetShapeBounds(true));
                InvalidationArea = Rectangle.Union(bounds, GetShapeBounds(true));
                eventData.NeedsPainted = true;
            }
            if (isMoving)
            {
                isMoving = false;
                this.ResizeHandles.SetHandlePositions(GetShapeBounds(true));
                eventData.NeedsPainted = true;
            }
            if (!eventData.WasHit && isSelected)
            {
                isSelected = false;
                bmpBuffer = null;
            }
            return eventData;
        }

        public override void Render(Graphics g)
        {
            if (isResizing)
            {
                bool blnTmp = painter.PaintFill;
                painter.PaintFill = false;
                painter.Paint(g, Path);
                painter.PaintFill = blnTmp;
            }
            else
            {
                g.DrawImageUnscaled(Buffer, Location.X + intBmpOffsetX, Location.Y);
                if (isSelected)
                {
                    Pen pen = new Pen(Color.FromArgb(8, 42, 105), 1);
                    pen.DashStyle = DashStyle.Dash;
                    g.DrawRectangle(pen, _Location.X, _Location.Y, Size.Width - 1, Size.Height - 1);
                    pen.Color = Color.White;
                    g.DrawRectangle(pen, _Location.X+1, _Location.Y + 1, Size.Width - 3, Size.Height - 3);
                    pen.Dispose();
                }
            }
        }

        public override void RenderHandles(Graphics g)
        {
            if (_isSelected && !editingOn && !isMoving)
            {
                //if the shape is selected, paint the resize handles
                ResizeHandles.Render(g);
            }
        }

        public override string EmitGDICode(string graphicsName, GDIGenerationTool helper)
        {
            bool blnScaleText = (bool)GdiProperties.GetValue(SCALE_TEXT);
            bool blnEditText = (bool)GdiProperties.GetValue(EDIT_TEXT);
            

            string strText = blnEditText ?  
                "dText[eShapeText." + this.Name + "]" 
                : helper.FormatText(text);
            string strBounds = blnScaleText ?
                    "shapeBoundsOld" : "shapeBoundsNew";

            string str = "//Text: " + this.Name + "\r\n";

            if (painter.PaintWithPath)
               str += helper.SetShapeBounds(this);
            else
               str += helper.GeneratePaintToolInit(this);

             str += "Font " + this.Name + "Font = new Font(\"" + font.FontFamily.Name + "\", " +
                 font.Size.ToString() + "f * (" + Size.Width + "f / (float)shapeBoundsOld.Width), (FontStyle)" + (int)font.Style + ");\r\n" +
             "StringFormat " + this.Name + "Format = new StringFormat();\r\n" +
             this.Name + "Format.Alignment = (StringAlignment)" + (int)format.Alignment + ";\r\n";

            if (blnScaleText)
                str += "//Will apply the scaling factors to the graphics object\r\n"+
                 graphicsName + ".ScaleTransform(scale.Width, scale.Height);\r\n";

            if (painter.PaintWithPath)
            {
                str += "path = new GraphicsPath();\r\n" +
                "path.AddString(" +strText + ", " + this.Name + 
                            "Font.FontFamily ,(int)" + this.Name + "Font.Style," +
                           this.Name + "Font.Size,\r\n" + strBounds+ "," +
                           this.Name + "Format);\r\n" +

                 helper.GeneratePaintToolInit(this, "path.GetBounds()",false) +

                 "//Paint the text\r\n";
                if (painter.PaintFill)
                    str += graphicsName + ".FillPath(" + this.Name + "Brush, path);\r\n";
                if (painter.PaintBorder)
                    str += graphicsName + ".DrawPath(" + this.Name + "Pen, path);\r\n";
            }
            else
            {
                str += graphicsName + ".DrawString("+ strText +", " +
                    this.Name + "Font, " + this.Name + "Brush, "+ strBounds +", " 
                    + this.Name + "Format);\r\n";
            }
            if (blnScaleText)
                str += graphicsName + ".ResetTransform();\r\n";

            str += "//Cleanup Text objects\r\n"+
                   this.Name + "Font.Dispose();\r\n"+
                   this.Name +"Format.Dispose();\r\n"+
                helper.GeneratePaintToolCleanup(this);
            return str;
        }

        public override Shape Clone()
        {
            ShapeText newShape = new ShapeText();
            newShape.Text = this.text;
            newShape.Location = new Point(this.Location.X + 15, this.Location.Y + 15);
            newShape.Size = this.Size;
            newShape.Font = (Font)this.font.Clone();
            newShape.painter.CopyProperties(ref this.painter);
            newShape.isInitializing = false;
            if (painter.ColorCount > 1 || painter.PaintBorder)
                newShape.GeneratePath();
            this.isSelected = false;
            return newShape;
        }

        protected override void Restore_NonSerialized()
        {
            format = new StringFormat();
            format.Alignment = (StringAlignment)alignment;
        }

        public override Rectangle GetShapeBounds(bool inflated)
        {
            Rectangle rect = new Rectangle(Location, Size);
            if (inflated)
                rect.Inflate(painter.IntBorderThickness, painter.IntBorderThickness);
            return rect;
        }

        public override string ShapeType
        {
            get { return "Text"; }
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (bmpBuffer != null) bmpBuffer.Dispose();
            if (gpText != null) gpText.Dispose();
            if (_Path != null) _Path.Dispose();
            if (font != null) font.Dispose();
        }

        #endregion
    }
}
