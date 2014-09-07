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
using System.Drawing.Drawing2D;

namespace Painting.Painter
{
    /// <summary>
    /// Provides GDI+ Painting for custom controls
    /// </summary>
    [Serializable]
    public class Painters : IDisposable
    {
        List<Color> colors;
        private Color FlavorColor = Color.Blue;
        public short selectedBorderWidth = 2;
        [NonSerialized]
        Brush paintBrush = null;
        [NonSerialized]
        Blend blend = null;
        public StateSettings State = new StateSettings();
        [NonSerialized]
        public short ID;
        /// <summary>
        /// Initializes a new painter object
        /// </summary>
        /// <param name="capacity">Please estimate how many colors the background will have</param>
        public Painters(Font fnt, short capacity)
        {
            colors = new List<Color>(capacity);
            colors.Add(Color.White);
            //rectBounds = bounds;
            Font = fnt;
        }

        public Painters(ref Painters painter)
        {
            rectBounds = painter.rectBounds;
            CopyProperties(ref painter);
        }

        public void AddColor(Color color)
        {
            colors.Add(color);
            if (colors.Count > 1 && blend == null) 
                SetBlendFactors();
        }

        public void ChangeColor(int index, Color color)
        {
            colors[index] = color;
        }

        private float borderThickness = 1.0f;
        public float BorderThickness
        {
            get { return borderThickness; }
            set { borderThickness = value; }
        }

        /// <summary>
        /// returns the border thickness rounded to the nearest whole
        /// number and converted to an integer
        /// </summary>
        public int IntBorderThickness
        {
            get { return (int)(borderThickness + .5f); }
        }


        public void ClearColors(int startIndex, int amount)
        {
            colors.RemoveRange(startIndex, amount);
        }

        public Color GetColor(int index)
        {
            return colors[index];
        }

        public int ColorCount
        {
            get { return colors.Count; }
        }

        private Font Font;


        [NonSerialized]
        Rectangle rectBounds = Rectangle.Empty;
        public bool PaintBorder = true;
        public bool PaintFill = true;
        public Color BorderColor = Color.Black;
        public LinearGradientMode LinearMode;
        public bool IsSolidColor = true;


        public Color TextColor = Color.Black;
        private Rectangle _TextBounds;
        private string _Text = string.Empty;


        public string Text
        {
            get { return _Text; }
            set
            {
                
                _Text = value;
                BoundsChange(rectBounds);
            }
        }

        public void BoundsChange(Rectangle rect)
        {
            if (rectBounds == rect) return;
            rectBounds = rect;
            if (_Text != null || _Text != "")
            {
                Bitmap bmpTemp = new Bitmap(1, 1);
                Graphics g = Graphics.FromImage(bmpTemp);
                Size size = MeasureDisplayString(g, _Text, Font, (short)rectBounds.Width);
                g.Dispose();
                bmpTemp.Dispose();

                //now we can set the position since we have the size
                //------  HORIZONTAL POSITIONING  ----------- 
                int intPositionX = (int)(((float)_TextPositionX / 100)
                    * rectBounds.Width) - (size.Width / 2);

                //ensure that the text won't go out of bounds
                if (intPositionX < 0) intPositionX = 0;
                if ((intPositionX + size.Width) > rectBounds.Width)
                    intPositionX = (rectBounds.Width - size.Width);

                //------  VERTICAL POSITIONING  ----------- 
                int intPositionY = (int)(((float)_TextPositionY / 100)
                     * rectBounds.Height) - (size.Height / 2);

                //ensure that the text won't go out of bounds
                if (intPositionY < 0) intPositionY = 0;
                if ((intPositionY + size.Height) > rectBounds.Height)
                    intPositionY = (rectBounds.Height - size.Height);

                _TextBounds = new Rectangle(intPositionX, intPositionY, size.Width+15, size.Height);

            }
        }



        private byte _TextPositionX = 50;
        /// <summary>
        /// gets or sets the text position 'percentage' relative to the
        /// bounds in which it resides.  0 will generate a
        /// full left position and 100 will generate a full
        /// right position.  Note that the text will go no 
        /// further if the right side is equal to bounding right side
        /// (a value greater than 100 will throw an error)
        /// </summary>
        public byte TextPositionX
        {
            get { return _TextPositionX; }
            set
            {
                if (value > 100)
                    throw new ArgumentException("The TextPositionX should be less than 100 percent!", "TextPositionX");
                _TextPositionX = value;
            }
        }

        private byte _TextPositionY = 50;
        /// <summary>
        /// gets or sets the text position 'percentage' relative to the
        /// bounds in which it resides.  0 will generate a
        /// full Top oriented position and 100 will generate a full
        /// bottom oriented position.  Note that the text's bounds
        /// will not go outside the bounding area
        /// (a value greater than 100 will throw an error)
        /// </summary>
        public byte TextPositionY
        {
            get { return _TextPositionY; }
            set
            {
                if (value > 100)
                    throw new ArgumentException("The TextPositionY should be less than 100 percent!", "TextPositionY");
                _TextPositionY = value;
            }
        }

        private byte _BlendSmoothness = 50;
        /// <summary>
        /// gets or sets the amount of smoothing that will occur
        /// when two different colors meet each other.  A lower
        /// value will produce a more distinguishable edge.
        /// (a value greater than 100 will throw an error)
        /// </summary>
        public byte BlendSmoothness
        {
            get { return _BlendSmoothness; }
            set 
            {
                if (value > 100)
                    throw new ArgumentException("The blend smoothness should be less than 100 percent!", "BlendSmoothness");
                _BlendSmoothness = value;
                if (colors.Count > 1) SetBlendFactors();
            }
        }

        private byte _Coverage = 50;
        /// <summary>
        /// gets or sets the amount of coverage the gradient will
        /// cover.  (a value greater than 100 will throw an error)
        /// </summary>
        public byte Coverage
        {
            get { return _Coverage; }
            set
            {
                if (value > 100)
                    throw new ArgumentException("The gradient coverage should be less than 100 percent!", "GradientCoverage");
                _Coverage = value;
                if (colors.Count > 1) SetBlendFactors();
            }
        }

        private void SetBlendFactors()
        {
            // Image blend is selected so add blend
            blend = null; //flush
            blend = new Blend(9);
            short i = 0;

            blend.Positions[0] = 0.0f;
            for (i = 1; i < 9; i++)
                blend.Positions[i] = (float)(blend.Positions[i-1] + 0.125f);

            byte intHalf = Convert.ToByte(((float)(_Coverage+1)/100)*8);
            float increment = (((float)(_BlendSmoothness)/100) / (8 - intHalf));
            blend.Factors[intHalf] = ((float)(100 - _BlendSmoothness) / 100);

            if (_BlendSmoothness > 0 && intHalf != 0)
            {
                short startingFactor;
                if (_BlendSmoothness == 100)
                    startingFactor = 0;
                else
                    startingFactor = Convert.ToInt16(((1-_BlendSmoothness/100.0f))*intHalf);
                float smoothIncrement = (blend.Factors[intHalf] / (intHalf - startingFactor+1));

                blend.Factors[startingFactor] = smoothIncrement;
                for (i = (short)(startingFactor+1); i < intHalf; i++)
                    blend.Factors[i] = (float)(blend.Factors[i - 1] + smoothIncrement);
            }

            for (i = (short)(intHalf + 1); i < 9; i++)
                blend.Factors[i] = (float)(blend.Factors[i - 1] + increment);
        }

        /// <summary>
        /// Paints a rectangular area with the colors previously set
        /// </summary>
        /// <param name="g">a reference to the graphics object</param>
        /// <param name="bounds">the paint area</param>
        public void Paint(Graphics g, Rectangle bounds)
        {
            if (!State.IsResizing)
                paintRectangle(g, bounds);
            else
            {
                if (State.PaintFill)
                {
                    initFillBrush(bounds);
                    g.FillRectangle(paintBrush, bounds);
                }

                if (State.PaintOutline)
                {
                    Pen outlinePen = new Pen(Color.Black,borderThickness);
                    outlinePen.DashStyle = State.DashStyle;
                    g.DrawRectangle(outlinePen, bounds);
                    outlinePen.Dispose();
                    outlinePen = null;
                }
            }
        }

        /// <summary>
        /// Paints a path generated area with the colors previously set
        /// </summary>
        /// <param name="g">a reference to the graphics object</param>
        /// <param name="path">the paint area</param>
        /// <param name="strText">the text to paint. (pass a null value if no text is desired)</param>
        public void Paint(Graphics g, GraphicsPath path)
        {
            if (!State.IsResizing)
                paintPath(g, path);
            else
            {
                if (State.PaintFill)
                {
                    initFillBrush(path.GetBounds());
                    g.FillPath(paintBrush, path);
                }

                if (State.PaintOutline)
                {
                    Pen outlinePen = new Pen(Color.Black,borderThickness);
                    outlinePen.DashStyle = State.DashStyle;
                    g.DrawPath(outlinePen, path);
                    outlinePen.Dispose();
                    outlinePen = null;
                }
            }
        }


        /// <summary>
        /// Paints a curve with the colors previously set
        /// </summary>
        /// <param name="g">a reference to the graphics object</param>
        /// <param name="path">the curve</param>
        /// <param name="strText">the text to paint. (pass a null value if no text is desired)</param>
        public void PaintCurve(Graphics g, PointF[] points)
        {
            Pen borderPen;
            if (State.IsSelected && !State.IsEditing)
                borderPen = new Pen(FlavorColor, selectedBorderWidth);
            else
                borderPen = new Pen(BorderColor,borderThickness);

            g.DrawCurve(borderPen, points);
            borderPen.Dispose();
            borderPen = null;
        }

        private void paintRectangle(Graphics g, Rectangle bounds)
        {
            //paint the area
            if (PaintFill)
            {
                initFillBrush(bounds);
                g.FillRectangle(paintBrush, bounds);
            }


            if (PaintBorder)
            {
                Pen borderPen = null;
                if (State.IsSelected && !State.IsEditing)
                    borderPen = new Pen(FlavorColor, selectedBorderWidth);
                else
                    borderPen = new Pen(BorderColor,borderThickness);
                g.DrawRectangle(borderPen, bounds);
                borderPen.Dispose();
            }
            if (paintBrush != null)
               paintBrush.Dispose(); //cleanup
            if(_Text != null || _Text != "")
                paintText(g);

            if (State.IsEditing && !State.AcceptsEditing)
            {
                //If the shape is in editing mode but the the shape
                //does not perform editing, we will draw two dashed rectangles
                //inside the shape to symbolize the non acceptance.
                Pen dashedPen = new Pen(Color.White, 1.0f);
                dashedPen.DashStyle = DashStyle.Dot;

                Rectangle rect = new Rectangle(bounds.X + 2, bounds.Y + 2, bounds.Width - 4, bounds.Height - 4);
                g.DrawRectangle(dashedPen, rect);
                rect.Inflate(-2, -2);
                dashedPen.Color = Color.Black;
                g.DrawRectangle(dashedPen, rect);
                dashedPen.Dispose();
                dashedPen = null;
            }
        }

        private void paintPath(Graphics g, GraphicsPath path)
        {

            //paint the area
            if (PaintFill)
            {
                initFillBrush(path.GetBounds());
                g.FillPath(paintBrush, path);
            }


            if (State.IsSelected || PaintBorder || (State.IsEditing && !State.AcceptsEditing))
            {

                Pen borderPen = null;
                if (State.IsSelected && (State.IsEditing && !State.AcceptsEditing))
                    borderPen = new Pen(FlavorColor, 1);
                else if (State.IsSelected && !State.IsEditing)
                    borderPen = new Pen(FlavorColor, selectedBorderWidth);
                else
                    borderPen = new Pen(BorderColor,borderThickness);
                borderPen.DashStyle = State.DashStyle;
                g.DrawPath(borderPen, path);
                borderPen.Dispose();
            }

            if (paintBrush != null)
            {
                paintBrush.Dispose(); //cleanup
                paintBrush = null;
            }


            if (_Text != null && _Text != "")
                paintText(g);



            if (State.IsSelected && (State.IsEditing && !State.AcceptsEditing))
            {
                //If the shape is in editing mode but the the shape
                //does not perform editing, we will draw two dashed rectangles
                //inside the shape to symbolize the non acceptance.
                Pen dashedPen = new Pen(Color.Black, 1.0f);
                dashedPen.DashStyle = DashStyle.Dot;

                Rectangle rect = Rectangle.Round(path.GetBounds());
                rect.Inflate(2, 2);
                g.DrawRectangle(dashedPen, rect);

                dashedPen.Color = Color.White;
                rect.Inflate(-2, -2);
                g.DrawRectangle(dashedPen, rect);

                dashedPen.Dispose();
                dashedPen = null;

            }
        }


        private void initFillBrush(RectangleF bounds)
        {
            if (colors.Count < 2)
            {
                if (colors.Count == 0)
                    paintBrush = new SolidBrush(Color.Black);
                else
                    paintBrush = new SolidBrush(colors[0]);
            }
            else //paint in linear mode
            {
                LinearGradientBrush tmp = new LinearGradientBrush(bounds, colors[0], 
                                                            colors[1], LinearMode);
                if (blend == null) SetBlendFactors();
                tmp.Blend = blend;
                paintBrush = tmp;
            }
        }


        private void paintText(Graphics g)
        {
            //paint the text
            if (_Text != null || _Text != "")
            {
                if (_TextBounds != null)
                {
                    if (TextColor == Color.Black)
                        g.DrawString(_Text, Font, Brushes.Black, _TextBounds, StringFormat.GenericDefault);
                    else
                    {
                        SolidBrush br = new SolidBrush(TextColor);
                        g.DrawString(_Text, Font, br, _TextBounds, StringFormat.GenericDefault);
                        br.Dispose();
                    }
                }
            }
        }

        public bool PaintWithPath
        {
            get { return (this.colors.Count > 1 || this.PaintBorder); }
        }

        public string LinearModeString
        {
            get
            {
                switch (LinearMode)
                {
                    case LinearGradientMode.BackwardDiagonal:
                        return "LinearGradientMode.BackwardDiagonal";
                    case LinearGradientMode.ForwardDiagonal:
                        return "LinearGradientMode.ForwardDiagonal";
                    case LinearGradientMode.Horizontal:
                        return "LinearGradientMode.Horizontal";
                    case LinearGradientMode.Vertical:
                        return "LinearGradientMode.Vertical";
                }
                throw new Exception("Error proccessing the linear gradient string!");
            }
        }

        public void CopyProperties(ref Painters painter)
        {
            _BlendSmoothness = painter.BlendSmoothness ;
            _Coverage = painter.Coverage;
            _Text = painter.Text;
            _TextPositionX = painter.TextPositionX;
            _TextPositionY = painter.TextPositionY;
            LinearMode = painter.LinearMode;
            blend = painter.blend;
            PaintBorder = painter.PaintBorder;
            PaintFill = painter.PaintFill;
            borderThickness = painter.borderThickness;
            if (colors != null)
                colors.Clear();
            else
                colors = new List<Color>();
            colors.Capacity = painter.colors.Capacity;
            colors.Add(painter.colors[0]);
            if (painter.colors.Count > 1)
                colors.Add(painter.colors[1]);
            BorderColor = painter.BorderColor;
            TextColor = painter.TextColor;
            Font = painter.Font;

        }


        public void Dispose()
        {
          
        }

        public static Size MeasureDisplayString(Graphics graphics, string text, Font font, int width, int height)
        {
            if (text == string.Empty || text == null) return new Size(0, 0);

            System.Drawing.StringFormat format = new System.Drawing.StringFormat();
            System.Drawing.RectangleF rect = new System.Drawing.RectangleF(0, 0,
                                                                          width, height);
            System.Drawing.CharacterRange[] ranges = 
                                       { new System.Drawing.CharacterRange(0, 
                                                               text.Length) };
            System.Drawing.Region[] regions = new System.Drawing.Region[1];

            format.SetMeasurableCharacterRanges(ranges);

            regions = graphics.MeasureCharacterRanges(text, font, rect, format);
            rect = regions[0].GetBounds(graphics);

            //cleanup
            format.Dispose();
            ranges = null;
            regions[0].Dispose();
            regions = null;

            return new Size((int)(rect.Right + 1.0f), (int)(rect.Height + 1.0f));
            //return (short)(rect.Right + 1.0f);
        }

        public static RectangleF MeasureDisplayStringF(Graphics graphics, string text, Font font, int width, int height)
        {
            if (text == string.Empty || text == null) return new RectangleF(0,0,0,0);

            System.Drawing.StringFormat format = new System.Drawing.StringFormat();
            System.Drawing.RectangleF rect = new System.Drawing.RectangleF(0, 0,
                                                                          width, height);
            System.Drawing.CharacterRange[] ranges = 
                                       { new System.Drawing.CharacterRange(0, 
                                                               text.Length) };
            System.Drawing.Region[] regions = new System.Drawing.Region[1];

            format.SetMeasurableCharacterRanges(ranges);

            regions = graphics.MeasureCharacterRanges(text, font, rect, format);
            rect = regions[0].GetBounds(graphics);

            //cleanup
            format.Dispose();
            ranges = null;
            regions[0].Dispose();
            regions = null;

            return rect;
        }


        public static Size MeasureDisplayString(Graphics graphics, string text, Font font, int width)
        {
           return MeasureDisplayString(graphics, text, font, width, int.MaxValue);
        }
    }
}
