using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Painting.Shapes
{
    public partial class FormSetImageProperties : Form
    {
        DrawingCanvas refCanvas;
        bool blnSuccess = false;
        public FormSetImageProperties(DrawingCanvas canvas)
        {
            InitializeComponent();
            refCanvas = canvas; //get an object reference
            TextBox01X.Text = refCanvas.shapeManager.BackBitmapBounds.X.ToString();
            TextBox02Y.Text = refCanvas.shapeManager.BackBitmapBounds.Y.ToString();
            TextBox03Width.Text = refCanvas.shapeManager.BackBitmapBounds.Width.ToString();
            TextBox04Height.Text = refCanvas.shapeManager.BackBitmapBounds.Height.ToString();
            lblFileName.Text = refCanvas.shapeManager.BackBmpFilePath;
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog OFD = new OpenFileDialog())
            {
                OFD.Filter = "Image File (*.png;*.jpg;*.jpeg;*.bmp;*.gif)|*.png;*.jpg;*.jpeg;*.bmp;*.gif|All Files (*.*)|*.*";
                if (OFD.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        Bitmap bmp = new Bitmap(OFD.FileName);
                        refCanvas.shapeManager.SetBackBitmap(bmp, OFD.FileName);
                        lblFileName.Text = OFD.FileName;
                        TextBox03Width.Text = bmp.Width.ToString();
                        TextBox04Height.Text = bmp.Height.ToString();
                        bmp = null;
                    }
                    catch (Exception E) { MessageBox.Show(E.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                }
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                refCanvas.shapeManager.BackBitmapBounds = new Rectangle(
                    int.Parse(TextBox01X.Text), int.Parse(TextBox02Y.Text),
                    int.Parse(TextBox03Width.Text), int.Parse(TextBox04Height.Text));
                blnSuccess = true;
            }
            catch (Exception E) { MessageBox.Show(E.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); blnSuccess = false; }
        }

        private void frmSetImageProperties_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = !blnSuccess;
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            refCanvas.shapeManager.SetBackBitmap(null, "");
            lblFileName.Text = "";
        }
    }
}