using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Painting.Shapes;

namespace PhotoBrushProject
{
    public partial class ShapeControlPanel : UserControl
    {
        public ShapeControlPanel()
        {
            InitializeComponent();
        }

        private List<Button> ButtonsList = null;
        DrawingCanvas DC = null;
        private ShapeMenuItem[] SMI = null;
        public ShapeControlPanel(DrawingCanvas DC)
        {
            InitializeComponent();
            this.DC = DC;
            SMI = this.DC.shapeManager.GetShapeList();
            ButtonsList = new List<Button>();
            ButtonsList.AddRange(new Button[]{ Button01ShapeText, Button02ShapeLine, Button03ShapeRectangle, Button04ShapeTriangle,
                Button05ShapePolygon, Button06ShapeCircle, Button07Bitmap });
            for (int i = 0; i < SMI.Length; i++)
            {
                switch (SMI[i].ShapeName)
                {
                    case "Text":
                        ButtonsList[0].Tag = SMI[i].ShapeType;
                        ButtonsList[0].Click +=new EventHandler(ShapeControlPanel_Click);
                        break;
                    case "Line":
                        ButtonsList[1].Tag = SMI[i].ShapeType;
                        ButtonsList[1].Click += new EventHandler(ShapeControlPanel_Click);
                        break;
                    case "Rectangle":
                        ButtonsList[2].Tag = SMI[i].ShapeType;
                        ButtonsList[2].Click += new EventHandler(ShapeControlPanel_Click);
                        break;
                    case "Triangle":
                        ButtonsList[3].Tag = SMI[i].ShapeType;
                        ButtonsList[3].Click += new EventHandler(ShapeControlPanel_Click);
                        break;
                    case "Polygon":
                        ButtonsList[4].Tag = SMI[i].ShapeType;
                        ButtonsList[4].Click += new EventHandler(ShapeControlPanel_Click);
                        break;
                    case "Circle":
                        ButtonsList[5].Tag = SMI[i].ShapeType;
                        ButtonsList[5].Click += new EventHandler(ShapeControlPanel_Click);
                        break;
                    default:
                        break;
                }
            }
            ButtonsList[6].Click +=new EventHandler(ShapeControlPanelBitmap_Click);
        }

        void ShapeControlPanel_Click(object sender, EventArgs e)
        {
            Button B = (Button)sender;
            DC.shapeManager.AddShape((byte)B.Tag);
        }

        void ShapeControlPanelBitmap_Click(object sender, EventArgs e)
        {
            FormSetImageProperties FSIP = new FormSetImageProperties(DC);
            FSIP.ShowDialog();
            FSIP.Dispose();
            DC.Invalidate();
        }
    }
}
