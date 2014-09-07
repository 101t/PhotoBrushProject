using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace Painting.Painter
{
    class PainterSolid : Painters
    {
        private Color _LineColor;
        public Color LineColor
        {
            get { return _LineColor; }
            set {_LineColor = value; }
        }
        private Color _BackColor;
        public Color BackColor
        {
            get { return _BackColor; }
            set { _BackColor = value; }
        }

        private bool _PaintLine = true;
        public bool PaintLine
        {
            get { return _PaintLine; }
            set 
            {           
                _PaintLine = value; 
                
            }
        }
        private bool _PaintBack = true;
        public bool PaintBack
        {
            get { return _PaintBack; }
            set { _PaintBack = value; }
        }
        //------ End Properties -------//

        // --- Constructors
        public PainterSolid()
        {
            _BackColor = Color.White;
            _LineColor = Color.Black;
        }
        public PainterSolid(Color lineColor, Color backColor)
        {
            _BackColor = backColor;
            _LineColor = lineColor;
        }
        //------ End Constructors -----//
        

        public override void Paint(Graphics g, System.Drawing.Drawing2D.GraphicsPath gp, bool isSelected)
        {

            if (isSelected)
            {
                SolidBrush br = new SolidBrush(selectedBack);
                Pen pen = new Pen(selectedLine);
                g.FillPath(br, gp);
                g.DrawPath(pen, gp);
                br.Dispose();
                pen.Dispose();
            }
            else  //is not selected
            {
                if (_PaintBack)
                {
                    SolidBrush br = new SolidBrush(_BackColor);
                    g.FillPath(br,gp);
                    br.Dispose();
                }

                if (_PaintLine)
                {
                    Pen pen = new Pen(_LineColor);
                    g.DrawPath(pen, gp);
                    pen.Dispose();
                }
            }




        }

        public override void Cleanup()
        {
            
        }
    }
}
