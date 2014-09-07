//ColorChooser is adapted from Ken Getz's article:
//GDI+ A Primer on Building a Color Picker User Control
//with GDI+ in Visual Basic _NET or C# -- MSDN Magazine, July 2003
//The original code did not allow for the alpha component so I've
//modified the color structures and controls to compensate for it.

using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace ColorChooserCSharp
{
	/// <summary>
	/// Summary description for ColorChooser2.
	/// </summary>
	public class ColorChooser : System.Windows.Forms.Form
    {
        #region Vars
        internal System.Windows.Forms.Label lblBlue;
		internal System.Windows.Forms.Label lblGreen;
		internal System.Windows.Forms.Label lblRed;
		internal System.Windows.Forms.Label lblBrightness;
		internal System.Windows.Forms.Label lblSaturation;
		internal System.Windows.Forms.Label lblHue;
		internal System.Windows.Forms.HScrollBar hsbBlue;
		internal System.Windows.Forms.HScrollBar hsbGreen;
		internal System.Windows.Forms.HScrollBar hsbRed;
		internal System.Windows.Forms.HScrollBar hsbBrightness;
		internal System.Windows.Forms.HScrollBar hsbSaturation;
		internal System.Windows.Forms.HScrollBar hsbHue;
		internal System.Windows.Forms.Button btnCancel;
		internal System.Windows.Forms.Button btnOK;
		internal System.Windows.Forms.Label Label3;
		internal System.Windows.Forms.Label Label7;
		internal System.Windows.Forms.Panel pnlColor;
		internal System.Windows.Forms.Label Label6;
		internal System.Windows.Forms.Label Label1;
		internal System.Windows.Forms.Label Label5;
		internal System.Windows.Forms.Panel pnlSelectedColor;
		internal System.Windows.Forms.Panel pnlBrightness;
		internal System.Windows.Forms.Label Label2;
        internal HScrollBar hsbAlpha;
        internal Label label4;
        internal Label lblAlpha;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
        #endregion
        public ColorChooser()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.lblBlue = new System.Windows.Forms.Label();
            this.lblGreen = new System.Windows.Forms.Label();
            this.lblRed = new System.Windows.Forms.Label();
            this.lblBrightness = new System.Windows.Forms.Label();
            this.lblSaturation = new System.Windows.Forms.Label();
            this.lblHue = new System.Windows.Forms.Label();
            this.hsbBlue = new System.Windows.Forms.HScrollBar();
            this.hsbGreen = new System.Windows.Forms.HScrollBar();
            this.hsbRed = new System.Windows.Forms.HScrollBar();
            this.hsbBrightness = new System.Windows.Forms.HScrollBar();
            this.hsbSaturation = new System.Windows.Forms.HScrollBar();
            this.hsbHue = new System.Windows.Forms.HScrollBar();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.Label3 = new System.Windows.Forms.Label();
            this.Label7 = new System.Windows.Forms.Label();
            this.pnlColor = new System.Windows.Forms.Panel();
            this.Label6 = new System.Windows.Forms.Label();
            this.Label1 = new System.Windows.Forms.Label();
            this.Label5 = new System.Windows.Forms.Label();
            this.pnlSelectedColor = new System.Windows.Forms.Panel();
            this.pnlBrightness = new System.Windows.Forms.Panel();
            this.Label2 = new System.Windows.Forms.Label();
            this.hsbAlpha = new System.Windows.Forms.HScrollBar();
            this.label4 = new System.Windows.Forms.Label();
            this.lblAlpha = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblBlue
            // 
            this.lblBlue.Location = new System.Drawing.Point(312, 360);
            this.lblBlue.Name = "lblBlue";
            this.lblBlue.Size = new System.Drawing.Size(40, 23);
            this.lblBlue.TabIndex = 54;
            this.lblBlue.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblGreen
            // 
            this.lblGreen.Location = new System.Drawing.Point(312, 336);
            this.lblGreen.Name = "lblGreen";
            this.lblGreen.Size = new System.Drawing.Size(40, 23);
            this.lblGreen.TabIndex = 53;
            this.lblGreen.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblRed
            // 
            this.lblRed.Location = new System.Drawing.Point(312, 312);
            this.lblRed.Name = "lblRed";
            this.lblRed.Size = new System.Drawing.Size(40, 23);
            this.lblRed.TabIndex = 52;
            this.lblRed.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblBrightness
            // 
            this.lblBrightness.Location = new System.Drawing.Point(312, 280);
            this.lblBrightness.Name = "lblBrightness";
            this.lblBrightness.Size = new System.Drawing.Size(40, 23);
            this.lblBrightness.TabIndex = 51;
            this.lblBrightness.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblSaturation
            // 
            this.lblSaturation.Location = new System.Drawing.Point(312, 256);
            this.lblSaturation.Name = "lblSaturation";
            this.lblSaturation.Size = new System.Drawing.Size(40, 23);
            this.lblSaturation.TabIndex = 50;
            this.lblSaturation.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblHue
            // 
            this.lblHue.Location = new System.Drawing.Point(312, 232);
            this.lblHue.Name = "lblHue";
            this.lblHue.Size = new System.Drawing.Size(40, 23);
            this.lblHue.TabIndex = 49;
            this.lblHue.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // hsbBlue
            // 
            this.hsbBlue.LargeChange = 1;
            this.hsbBlue.Location = new System.Drawing.Point(80, 360);
            this.hsbBlue.Maximum = 255;
            this.hsbBlue.Name = "hsbBlue";
            this.hsbBlue.Size = new System.Drawing.Size(224, 18);
            this.hsbBlue.TabIndex = 48;
            this.hsbBlue.Scroll += new System.Windows.Forms.ScrollEventHandler(this.HandleRGBScroll);
            // 
            // hsbGreen
            // 
            this.hsbGreen.LargeChange = 1;
            this.hsbGreen.Location = new System.Drawing.Point(80, 336);
            this.hsbGreen.Maximum = 255;
            this.hsbGreen.Name = "hsbGreen";
            this.hsbGreen.Size = new System.Drawing.Size(224, 18);
            this.hsbGreen.TabIndex = 47;
            this.hsbGreen.Scroll += new System.Windows.Forms.ScrollEventHandler(this.HandleRGBScroll);
            // 
            // hsbRed
            // 
            this.hsbRed.LargeChange = 1;
            this.hsbRed.Location = new System.Drawing.Point(80, 312);
            this.hsbRed.Maximum = 255;
            this.hsbRed.Name = "hsbRed";
            this.hsbRed.Size = new System.Drawing.Size(224, 18);
            this.hsbRed.TabIndex = 46;
            this.hsbRed.Scroll += new System.Windows.Forms.ScrollEventHandler(this.HandleRGBScroll);
            // 
            // hsbBrightness
            // 
            this.hsbBrightness.LargeChange = 1;
            this.hsbBrightness.Location = new System.Drawing.Point(80, 280);
            this.hsbBrightness.Maximum = 255;
            this.hsbBrightness.Name = "hsbBrightness";
            this.hsbBrightness.Size = new System.Drawing.Size(224, 18);
            this.hsbBrightness.TabIndex = 45;
            this.hsbBrightness.Scroll += new System.Windows.Forms.ScrollEventHandler(this.HandleHSVScroll);
            // 
            // hsbSaturation
            // 
            this.hsbSaturation.LargeChange = 1;
            this.hsbSaturation.Location = new System.Drawing.Point(80, 256);
            this.hsbSaturation.Maximum = 255;
            this.hsbSaturation.Name = "hsbSaturation";
            this.hsbSaturation.Size = new System.Drawing.Size(224, 18);
            this.hsbSaturation.TabIndex = 44;
            this.hsbSaturation.Scroll += new System.Windows.Forms.ScrollEventHandler(this.HandleHSVScroll);
            // 
            // hsbHue
            // 
            this.hsbHue.LargeChange = 1;
            this.hsbHue.Location = new System.Drawing.Point(80, 232);
            this.hsbHue.Maximum = 255;
            this.hsbHue.Name = "hsbHue";
            this.hsbHue.Size = new System.Drawing.Size(224, 18);
            this.hsbHue.TabIndex = 43;
            this.hsbHue.Scroll += new System.Windows.Forms.ScrollEventHandler(this.HandleHSVScroll);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancel.Location = new System.Drawing.Point(296, 40);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(64, 24);
            this.btnCancel.TabIndex = 42;
            this.btnCancel.Text = "Cancel";
            // 
            // btnOK
            // 
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnOK.Location = new System.Drawing.Point(296, 8);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(64, 24);
            this.btnOK.TabIndex = 41;
            this.btnOK.Text = "OK";
            // 
            // Label3
            // 
            this.Label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label3.Location = new System.Drawing.Point(8, 360);
            this.Label3.Name = "Label3";
            this.Label3.Size = new System.Drawing.Size(72, 18);
            this.Label3.TabIndex = 34;
            this.Label3.Text = "Blue";
            this.Label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // Label7
            // 
            this.Label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label7.Location = new System.Drawing.Point(8, 280);
            this.Label7.Name = "Label7";
            this.Label7.Size = new System.Drawing.Size(72, 18);
            this.Label7.TabIndex = 37;
            this.Label7.Text = "Brightness";
            this.Label7.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // pnlColor
            // 
            this.pnlColor.Location = new System.Drawing.Point(8, 8);
            this.pnlColor.Name = "pnlColor";
            this.pnlColor.Size = new System.Drawing.Size(224, 216);
            this.pnlColor.TabIndex = 38;
            this.pnlColor.Visible = false;
            // 
            // Label6
            // 
            this.Label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label6.Location = new System.Drawing.Point(8, 256);
            this.Label6.Name = "Label6";
            this.Label6.Size = new System.Drawing.Size(72, 18);
            this.Label6.TabIndex = 36;
            this.Label6.Text = "Saturation";
            this.Label6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // Label1
            // 
            this.Label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label1.Location = new System.Drawing.Point(8, 312);
            this.Label1.Name = "Label1";
            this.Label1.Size = new System.Drawing.Size(72, 18);
            this.Label1.TabIndex = 32;
            this.Label1.Text = "Red";
            this.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // Label5
            // 
            this.Label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label5.Location = new System.Drawing.Point(8, 232);
            this.Label5.Name = "Label5";
            this.Label5.Size = new System.Drawing.Size(72, 18);
            this.Label5.TabIndex = 35;
            this.Label5.Text = "Hue";
            this.Label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // pnlSelectedColor
            // 
            this.pnlSelectedColor.Location = new System.Drawing.Point(296, 72);
            this.pnlSelectedColor.Name = "pnlSelectedColor";
            this.pnlSelectedColor.Size = new System.Drawing.Size(64, 42);
            this.pnlSelectedColor.TabIndex = 40;
            this.pnlSelectedColor.Visible = false;
            // 
            // pnlBrightness
            // 
            this.pnlBrightness.Location = new System.Drawing.Point(240, 8);
            this.pnlBrightness.Name = "pnlBrightness";
            this.pnlBrightness.Size = new System.Drawing.Size(24, 216);
            this.pnlBrightness.TabIndex = 39;
            this.pnlBrightness.Visible = false;
            // 
            // Label2
            // 
            this.Label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label2.Location = new System.Drawing.Point(8, 336);
            this.Label2.Name = "Label2";
            this.Label2.Size = new System.Drawing.Size(72, 18);
            this.Label2.TabIndex = 33;
            this.Label2.Text = "Green";
            this.Label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // hsbAlpha
            // 
            this.hsbAlpha.LargeChange = 1;
            this.hsbAlpha.Location = new System.Drawing.Point(80, 391);
            this.hsbAlpha.Maximum = 255;
            this.hsbAlpha.Name = "hsbAlpha";
            this.hsbAlpha.Size = new System.Drawing.Size(224, 18);
            this.hsbAlpha.TabIndex = 56;
            this.hsbAlpha.Value = 255;
            this.hsbAlpha.ValueChanged += new System.EventHandler(this.hsbAlpha_ValueChanged);
            this.hsbAlpha.Scroll += new System.Windows.Forms.ScrollEventHandler(this.hsbAlpha_Scroll);
            // 
            // label4
            // 
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(8, 391);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(72, 18);
            this.label4.TabIndex = 55;
            this.label4.Text = "Alpha";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblAlpha
            // 
            this.lblAlpha.Location = new System.Drawing.Point(312, 389);
            this.lblAlpha.Name = "lblAlpha";
            this.lblAlpha.Size = new System.Drawing.Size(40, 23);
            this.lblAlpha.TabIndex = 57;
            this.lblAlpha.Text = "255";
            this.lblAlpha.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // ColorChooser
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(368, 415);
            this.Controls.Add(this.lblAlpha);
            this.Controls.Add(this.hsbAlpha);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.lblBlue);
            this.Controls.Add(this.lblGreen);
            this.Controls.Add(this.lblRed);
            this.Controls.Add(this.lblBrightness);
            this.Controls.Add(this.lblSaturation);
            this.Controls.Add(this.lblHue);
            this.Controls.Add(this.hsbBlue);
            this.Controls.Add(this.hsbGreen);
            this.Controls.Add(this.hsbRed);
            this.Controls.Add(this.hsbBrightness);
            this.Controls.Add(this.hsbSaturation);
            this.Controls.Add(this.hsbHue);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.Label3);
            this.Controls.Add(this.Label7);
            this.Controls.Add(this.pnlColor);
            this.Controls.Add(this.Label6);
            this.Controls.Add(this.Label1);
            this.Controls.Add(this.Label5);
            this.Controls.Add(this.pnlSelectedColor);
            this.Controls.Add(this.pnlBrightness);
            this.Controls.Add(this.Label2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "ColorChooser";
            this.Text = "Select Color";
            this.Load += new System.EventHandler(this.ColorChooser2_Load);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.frmMain_MouseUp);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.ColorChooser2_Paint);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.HandleMouse);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.HandleMouse);
            this.ResumeLayout(false);

		}
		#endregion

		private enum ChangeStyle
		{
			MouseMove,
			RGB,
			HSV,
			None
		}

		private ChangeStyle changeType = ChangeStyle.None;
		private Point selectedPoint;

		private ColorWheel myColorWheel;
		private ColorHandler.RGB RGB;
		private ColorHandler.HSV HSV;

        public Color ColorWithAlpha
        {
            get
            {
                return Color.FromArgb(hsbAlpha.Value,
                                      hsbRed.Value,
                                      hsbGreen.Value,
                                      hsbBlue.Value);
            }
        }


		private void ColorChooser2_Load(object sender, System.EventArgs e)
		{
			// Turn on double-buffering, so the form looks better. 
			this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			this.SetStyle(ControlStyles.UserPaint, true);
			this.SetStyle(ControlStyles.DoubleBuffer, true);

			// These properties are set in design view, as well, but they
			// have to be set to false in order for the Paint
			// event to be able to display their contents.
			// Never hurts to make sure they're invisible.
			pnlSelectedColor.Visible = false;
			pnlBrightness.Visible = false;
			pnlColor.Visible = false;

			// Calculate the coordinates of the three
			// required regions on the form.
			Rectangle SelectedColorRectangle =  new Rectangle(pnlSelectedColor.Location, pnlSelectedColor.Size);
			Rectangle BrightnessRectangle = new Rectangle(pnlBrightness.Location, pnlBrightness.Size);
			Rectangle ColorRectangle = new Rectangle(pnlColor.Location, pnlColor.Size);

			// Create the new ColorWheel class, indicating
			// the locations of the color wheel itself, the
			// brightness area, and the position of the selected color.
			myColorWheel = new ColorWheel(ColorRectangle, BrightnessRectangle, SelectedColorRectangle);
			myColorWheel.ColorChanged += 
				new ColorWheel.ColorChangedEventHandler(this.myColorWheel_ColorChanged);

			// Set the RGB and HSV values 
			// of the NumericUpDown controls.
			SetRGB(RGB);
			SetHSV(HSV);
            hsbAlpha.Value = RGB.Alpha;
            lblAlpha.Text = hsbAlpha.Value.ToString();
            this.Invalidate();
		}

		private void HandleMouse(object sender,  MouseEventArgs e)
		{
			// If you have the left mouse button down, 
			// then update the selectedPoint value and 
			// force a repaint of the color wheel.
			if ( e.Button == MouseButtons.Left ) 
			{
				changeType = ChangeStyle.MouseMove;
				selectedPoint = new Point(e.X, e.Y);
				this.Invalidate();
			}
		}

		private void frmMain_MouseUp(object sender,  MouseEventArgs e)
		{
			myColorWheel.SetMouseUp();
			changeType = ChangeStyle.None;
		}

		private void SetRGBLabels(ColorHandler.RGB RGB) 
		{
			RefreshText(lblRed, RGB.Red);
			RefreshText(lblBlue, RGB.Blue);
			RefreshText(lblGreen, RGB.Green);
		}

		private void SetHSVLabels(ColorHandler.HSV HSV) 
		{
			RefreshText(lblHue, HSV.Hue);
			RefreshText(lblSaturation, HSV.Saturation);
			RefreshText(lblBrightness, HSV.value);
		}

		private void SetRGB(ColorHandler.RGB RGB) 
		{
			// Update the RGB values on the form.
			RefreshValue(hsbRed, RGB.Red);
			RefreshValue(hsbBlue, RGB.Blue);
			RefreshValue(hsbGreen, RGB.Green);
			SetRGBLabels(RGB);
			}

		private void SetHSV( ColorHandler.HSV HSV) 
		{
			// Update the HSV values on the form.
			RefreshValue(hsbHue, HSV.Hue);
			RefreshValue(hsbSaturation, HSV.Saturation);
			RefreshValue(hsbBrightness, HSV.value);
			SetHSVLabels(HSV);
			}

		private void RefreshValue(HScrollBar hsb, int value) 
		{
			hsb.Value = value;
		}

		private void RefreshText(Label lbl, int value) 
		{
			lbl.Text = value.ToString();
		}

		public Color Color  
		{
			// Get or set the color to be
			// displayed in the color wheel.
			get 
			{
				return myColorWheel.Color;
			}

			set 
			{
				// Indicate the color change type. Either RGB or HSV
				// will cause the color wheel to update the position
				// of the pointer.
				changeType = ChangeStyle.RGB;
				RGB = new ColorHandler.RGB(value.A, value.R, value.G, value.B);
				HSV = ColorHandler.RGBtoHSV(RGB);
			}
		}

		private void myColorWheel_ColorChanged(object sender,  ColorChangedEventArgs e)  
		{
			SetRGB(e.RGB);
			SetHSV(e.HSV);
		}

		private void HandleHSVScroll(object sender,  ScrollEventArgs e)  
			// If the H, S, or V values change, use this 
			// code to update the RGB values and invalidate
			// the color wheel (so it updates the pointers).
			// Check the isInUpdate flag to avoid recursive events
			// when you update the NumericUpdownControls.
		{
			changeType = ChangeStyle.HSV;
			HSV = new ColorHandler.HSV(hsbAlpha.Value, hsbHue.Value, hsbSaturation.Value, hsbBrightness.Value);
			SetRGB(ColorHandler.HSVtoRGB(HSV));
			SetHSVLabels(HSV);
			this.Invalidate();
		}

		private void HandleRGBScroll(object sender, ScrollEventArgs e)
		{
			// If the R, G, or B values change, use this 
			// code to update the HSV values and invalidate
			// the color wheel (so it updates the pointers).
			// Check the isInUpdate flag to avoid recursive events
			// when you update the NumericUpdownControls.
			changeType = ChangeStyle.RGB;
			RGB = new ColorHandler.RGB(hsbAlpha.Value, hsbRed.Value, hsbGreen.Value, hsbBlue.Value);
			SetHSV(ColorHandler.RGBtoHSV(RGB));
			SetRGBLabels(RGB);
			this.Invalidate();
		}

		private void ColorChooser2_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
		{
			// Depending on the circumstances, force a repaint
			// of the color wheel passing different information.
			switch (changeType)
			{
				case ChangeStyle.HSV:
					myColorWheel.Draw(e.Graphics, HSV);
					break;
				case ChangeStyle.MouseMove:
				case ChangeStyle.None:
					myColorWheel.Draw(e.Graphics, selectedPoint);
					break;
				case ChangeStyle.RGB:
					myColorWheel.Draw(e.Graphics, RGB);
					break;
			}
        }

        private void hsbAlpha_Scroll(object sender, ScrollEventArgs e)
        {

        }

        private void hsbAlpha_ValueChanged(object sender, EventArgs e)
        {
            lblAlpha.Text = hsbAlpha.Value.ToString();
            RGB.Alpha = hsbAlpha.Value;
            RGB.Blue = hsbBlue.Value;
            RGB.Green = hsbGreen.Value;
            RGB.Red = hsbRed.Value;
            SetRGB(RGB);
        }
	
	}
}

