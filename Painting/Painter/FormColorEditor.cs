using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using ColorChooserCSharp;

namespace Painting.Painter
{
    public partial class FormColorEditor : Form
    {
        private Painters sentPainter, _painter;
        private ColorChooser colorChooser = new ColorChooser();
        private bool blnSupress;
        private bool blnSolidEdited, blnGradientEdited;

        public FormColorEditor(Painters painter)
        {
            InitializeComponent();
            this.SetStyle(ControlStyles.DoubleBuffer |
              ControlStyles.UserPaint |
              ControlStyles.AllPaintingInWmPaint,
              true);

            _painter = new Painters(ref painter);
            sentPainter = painter; //copy the reference
            
            if (_painter.ColorCount > 1)
                tpEditor.SelectedIndex = 1;

            TabChanged();
        }

        private void tpEditor_SelectedIndexChanged(object sender, EventArgs e)
        {
            TabChanged();
        }

        private void TabChanged()
        {
            blnSupress = true;
            if (tpEditor.SelectedIndex != -1)
            {
                switch (tpEditor.SelectedIndex)
                {
                    case 0:
                        chkPaintFill.Checked = _painter.PaintFill;
                        chkPaintLine.Checked = _painter.PaintBorder;
                        cboLineThickness.Text =  _painter.BorderThickness.ToString("F");
                        break;
                    case 1:
                        chkPaintFill_2.Checked = _painter.PaintFill;
                        chkPaintLine_2.Checked = _painter.PaintBorder;
                        SetGradientCombobox();
                        tbBlendSmoothing.Value = _painter.BlendSmoothness;
                        tbCoverageArea.Value = _painter.Coverage;
                        cboLineThickness2.Text = _painter.BorderThickness.ToString("F");
                        break;
                }
            }
            blnSupress = false;
            PaintPreview();
        }


        private void tpSolid_Paint(object sender, PaintEventArgs e)
        {
            if (!_painter.PaintBorder && !_painter.PaintFill)
                e.Graphics.DrawRectangle(Pens.Gray, 154, 106, 174, 84);
            else
            {
                if (_painter.PaintFill)
                {
                    SolidBrush br = new SolidBrush(_painter.GetColor(0));
                    e.Graphics.FillRectangle(br, 154, 106, 174, 84);
                    br.Dispose();
                }
                if (_painter.PaintBorder)
                {
                    Pen linePen = new Pen(_painter.BorderColor, _painter.IntBorderThickness);
                    e.Graphics.DrawRectangle(linePen, 154+(_painter.IntBorderThickness/2),
                                             106+(_painter.IntBorderThickness/2), 
                                             174 - _painter.IntBorderThickness,
                                             84 - _painter.IntBorderThickness);
                    linePen.Dispose();
                }
            }
        }

        private void chkPaintFill_CheckedChanged(object sender, EventArgs e)
        {
            _painter.PaintFill = chkPaintFill.Checked;
            
            PaintPreview();
        }

        private void chkPaintLine_CheckedChanged(object sender, EventArgs e)
        {
            _painter.PaintBorder = chkPaintLine.Checked;
            PaintPreview();
        }

        private void btnSelectLineColor_Click(object sender, EventArgs e)
        {
            colorChooser.Color = _painter.BorderColor;
            colorChooser.ShowDialog();
            _painter.BorderColor = colorChooser.Color;
            PaintPreview();
        }

        private void btnSelectFillColor_Click(object sender, EventArgs e)
        {
            colorChooser.Color = _painter.GetColor(0);
            colorChooser.ShowDialog();
            _painter.ChangeColor(0, colorChooser.Color);
            PaintPreview();
        }

        private void btnAccept_Click(object sender, EventArgs e)
        {
            if (tpEditor.SelectedIndex == 0 && _painter.ColorCount > 1)
                 _painter.ClearColors(_painter.ColorCount - 1, 1);

            sentPainter.CopyProperties(ref _painter);
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        #region Gradient Code
        //*************************************************
        //***************   Gradient Code   ***************

        private void btnLineColor_Click(object sender, EventArgs e)
        {
            colorChooser.Color = _painter.BorderColor;
            colorChooser.ShowDialog();
            _painter.BorderColor = colorChooser.Color;
            PaintPreview();
        }

        private void chkPaintLine_2_CheckedChanged(object sender, EventArgs e)
        {
           _painter.PaintBorder = chkPaintLine_2.Checked;
            PaintPreview();
        }

        private void btnPrimaryColor_Click(object sender, EventArgs e)
        {
            colorChooser.Color = _painter.GetColor(0);
            colorChooser.ShowDialog();
            _painter.ChangeColor(0, colorChooser.Color);
            colorTabEdited(true);
            PaintPreview();
        }

        private void btnSecondaryColor_Click(object sender, EventArgs e)
        {
            if (_painter.ColorCount == 1)
                _painter.AddColor(Color.White);
            colorChooser.Color = _painter.GetColor(1);
            colorChooser.ShowDialog();
            _painter.ChangeColor(1, colorChooser.Color);
            colorTabEdited(true);
            PaintPreview();
        }

        private void chkPaintFill_2_CheckedChanged(object sender, EventArgs e)
        {
            _painter.PaintFill = chkPaintFill_2.Checked;
            colorTabEdited(true);
            PaintPreview();
        }

        private void tbBlendSmoothing_ValueChanged(object sender, EventArgs e)
        {
            _painter.BlendSmoothness = (byte)tbBlendSmoothing.Value;
            lblBlendSmoothing.Text = "Blend Smoothness: " + _painter.BlendSmoothness.ToString() + "%";
            colorTabEdited(true);
            PaintPreview();
        }

        private void tbCoverageArea_ValueChanged(object sender, EventArgs e)
        {
            _painter.Coverage = (byte)tbCoverageArea.Value;
            lblCoverageArea.Text = "Coverage: " + _painter.Coverage.ToString() +"%";
            colorTabEdited(true);
            PaintPreview();
        }

        private void PaintPreview()
        {
            if (blnSupress) return;
            if (tpEditor.SelectedIndex == 0)
                tpSolid.Invalidate(new Rectangle(154, 106, 178, 88));
            else
                tpGradient.Invalidate(new Rectangle(195, 70, 150, 106));
        }

        private void cboGradientStyle_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (cboGradientStyle.SelectedIndex)
            {
                case 0: //Horizontal
                    _painter.LinearMode = LinearGradientMode.Horizontal;
                    break;
                case 1: //Vertical
                    _painter.LinearMode = LinearGradientMode.Vertical;
                    break;
                case 2: //ForwardDiagonal
                    _painter.LinearMode = LinearGradientMode.ForwardDiagonal;
                    break;
                case 3: //BackwardDiagonal
                    _painter.LinearMode = LinearGradientMode.BackwardDiagonal;
                    break;
            }
            PaintPreview();
        }

        private void SetGradientCombobox()
        {
            switch (_painter.LinearMode)
            {
                case LinearGradientMode.Horizontal:
                    cboGradientStyle.SelectedIndex = 0;
                    break;
                case LinearGradientMode.Vertical:
                    cboGradientStyle.SelectedIndex = 1;
                    break;
                case LinearGradientMode.ForwardDiagonal:
                    cboGradientStyle.SelectedIndex = 2;
                    break;
                case LinearGradientMode.BackwardDiagonal:
                    cboGradientStyle.SelectedIndex = 3;
                    break;
            }
            PaintPreview();
        }

        private void tpGradient_Paint(object sender, PaintEventArgs e)
        {

            if (!_painter.PaintBorder && !_painter.PaintFill)
                e.Graphics.DrawRectangle(Pens.Gray, 195, 70, 150, 106);
            else
            {
                Rectangle rect = new Rectangle(195+(_painter.IntBorderThickness / 2),
                                             70+(_painter.IntBorderThickness / 2),
                                             150 - _painter.IntBorderThickness,
                                             106 - _painter.IntBorderThickness);
                //Bitmap backBuffer = new Bitmap(rect.Width+_painter.IntBorderThickness, 
                //                               rect.Height+_painter.IntBorderThickness);
                //Graphics gr = Graphics.FromImage(backBuffer);
                _painter.Paint(e.Graphics, rect);
                //e.Graphics.DrawImageUnscaled(backBuffer, 195,70);
                //backBuffer.Dispose();
             }

        }



        //******************  End Gradient Code   ****************
        #endregion

        private void colorTabEdited(bool isGradientTab)
        {
            blnSolidEdited = !isGradientTab;
            blnGradientEdited = isGradientTab;
        }


        #region LineThickness Code
        private void SetLineThickness(string strValue)
        {
            float val = 0.0f;
            if (!float.TryParse(strValue, out val))
                MessageBox.Show("[ " + strValue + " ] is not a valid number.  Try a format like: 2.0", "Incorrect Value", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            else
            {
                _painter.BorderThickness = val;
                PaintPreview();
            }
        }
        private void cboLineThickness_Leave(object sender, EventArgs e)
        {
            SetLineThickness(cboLineThickness.Text);
        }

        private void cboLineThickness_SelectedIndexChanged(object sender, EventArgs e)
        {
           if (cboLineThickness.SelectedIndex != -1)
               SetLineThickness(cboLineThickness.Text);
        }

        private void cboLineThickness_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                cboLineThickness_Leave(null, null);
        }


        private void cboLineThickness2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboLineThickness2.SelectedIndex != -1)
                SetLineThickness(cboLineThickness2.Text);
        }

        private void cboLineThickness2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                cboLineThickness2_Leave(null, null);
        }
    
        private void cboLineThickness2_Leave(object sender, EventArgs e)
        {
            SetLineThickness(cboLineThickness2.Text);
        }
        #endregion







    }
}