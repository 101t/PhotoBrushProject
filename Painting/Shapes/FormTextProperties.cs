using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Painting.Shapes
{
    public partial class FormTextProperties : Form
    {
        ShapeText refShape;
        FontDialog FD = new FontDialog();
        public bool txtChanged;
        public bool fontChanged;
        public FormTextProperties(ShapeText shapeText)
        {
            InitializeComponent();
            txtContents.Text = shapeText.Text;
            refShape = shapeText;
            txtChanged = false;
            fontChanged = false;
            txtContents.Font = refShape.Font;
            FD.Font = refShape.Font;

            switch (refShape.Alignment)
            {
                case StringAlignment.Near:
                    txtContents.TextAlign = HorizontalAlignment.Left;
                    break;
                case StringAlignment.Center:
                    txtContents.TextAlign = HorizontalAlignment.Center;
                    break;
                case StringAlignment.Far:
                    txtContents.TextAlign = HorizontalAlignment.Right;
                    break;
            }
        }

        private void btnTextCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnTextAccept_Click(object sender, EventArgs e)
        {
            if (txtChanged && (refShape.Text != txtContents.Text))
                 refShape.Text = txtContents.Text;
            if (fontChanged)
                 refShape.Font = FD.Font;
            this.Close();
        }

        private void btnFont_Click(object sender, EventArgs e)
        {
            FD.ShowDialog();
            if (FD.Font.Name != refShape.Font.Name)
                fontChanged = true;
            if (FD.Font.Size != refShape.Font.Size)
                fontChanged = true;
            if (FD.Font.Style != refShape.Font.Style)
                fontChanged = true;
            if (FD.Font.Strikeout != refShape.Font.Strikeout)
                fontChanged = true;
            if (FD.Font.Underline != refShape.Font.Underline)
                fontChanged = true;
            if (fontChanged)
                txtContents.Font = FD.Font;
        }

        private void frmTextProperties_FormClosed(object sender, FormClosedEventArgs e)
        {
            FD.Dispose();
            FD = null;
        }

        private void txtContents_TextChanged(object sender, EventArgs e)
        {
            txtChanged = true;
        }

        private void btnAlignLeft_Click(object sender, EventArgs e)
        {
            txtContents.TextAlign = HorizontalAlignment.Left;
            if (refShape.Alignment != StringAlignment.Near)
                txtChanged = true;
            refShape.Alignment = StringAlignment.Near;
        }

        private void btnAlignMiddle_Click(object sender, EventArgs e)
        {
            txtContents.TextAlign = HorizontalAlignment.Center;
            if (refShape.Alignment != StringAlignment.Center)
                txtChanged = true;
            refShape.Alignment = StringAlignment.Center;
        }


        private void btnAlignRight_Click(object sender, EventArgs e)
        {
            txtContents.TextAlign = HorizontalAlignment.Right;
            if (refShape.Alignment != StringAlignment.Far)
                txtChanged = true;
            refShape.Alignment = StringAlignment.Far;
        }
    }
}