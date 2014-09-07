using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using PsdFramework;

namespace PhotoBrushProject
{
    public partial class PsdHandlerForm : Form, IFormBuilder
    {
        Rectangle IFormBuilder.Bounds { get { return this.Bounds; } set { this.Bounds = value; } }
        FormBorderStyle IFormBuilder.FormBorderStyled { get { return this.FormBorderStyle; } set { this.FormBorderStyle = value; } }
        Color IFormBuilder.BackColor { get { return this.BackColor; } set { this.BackColor = value; } }
        string IFormBuilder.Text { get { return this.Text; } set { this.Text = value; } }
        bool IFormBuilder.TopLevel { get { return this.TopLevel; } set { this.TopLevel = value; } }

        CPSD psd = null;

        public PsdHandlerForm()
        {
            InitializeComponent();
            psd = new CPSD();
        }

        public void OpenPsdFile(string FileName)
        {
            int nResult = psd.Load(FileName);
            if (nResult == 0)
            {
                int nCompression = psd.GetCompression();
                string strCompression = "Unknown";
                switch (nCompression)
                {
                    case 0:
                        strCompression = "Raw data";
                        break;
                    case 1:
                        strCompression = "RLE";
                        break;
                    case 2:
                        strCompression = "ZIP without prediction";
                        break;
                    case 3:
                        strCompression = "ZIP with prediction";
                        break;
                }
                label1.Text = string.Format("Image Width: {0}px\r\nImage Height: {1}px\r\n" +
                    "Image BitsPerPixel: {2}\r\n" +
                    "Resolution (pixels/inch): X={3} Y={4}\r\n",
                    psd.GetWidth(),
                    psd.GetHeight(),
                    psd.GetBitsPerPixel(),
                    psd.GetXResolution(),
                    psd.GetYResolution());
                label1.Text += "Compression: " + strCompression;
                pictureBox1.Image = System.Drawing.Image.FromHbitmap(psd.GetHBitmap());
            }
            else if (nResult == -1)
                MessageBox.Show("Cannot open the File");
            else if (nResult == -2)
                MessageBox.Show("Invalid (or unknown) File Header");
            else if (nResult == -3)
                MessageBox.Show("Invalid (or unknown) ColourMode Data block");
            else if (nResult == -4)
                MessageBox.Show("Invalid (or unknown) Image Resource block");
            else if (nResult == -5)
                MessageBox.Show("Invalid (or unknown) Layer and Mask Information section");
            else if (nResult == -6)
                MessageBox.Show("Invalid (or unknown) Image Data block");
        }
    }
}
