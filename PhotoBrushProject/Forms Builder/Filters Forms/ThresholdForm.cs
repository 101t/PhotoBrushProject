using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using AForge.Imaging.Filters;

namespace PhotoBrushProject
{
	/// <summary>
	/// Summary description for ThresholdForm.
	/// </summary>
	public class ThresholdForm : System.Windows.Forms.Form
	{
		private Threshold filter = new Threshold();
		private byte min = 128, max = 255;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox minBox;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox maxBox;
		private ColorSlider slider;
		private System.Windows.Forms.PictureBox pictureBox2;
		private System.Windows.Forms.Button okButton;
		private System.Windows.Forms.Button cancelButton;
		private FilterPreview filterPreview;
		private System.Windows.Forms.GroupBox groupBox1;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		// Image property
		public Bitmap Image
		{
			set { filterPreview.Image = value; }
		}
		// Filter property
		public IFilter Filter
		{
			get { return filter; }
		}

		// Constructor
		public ThresholdForm()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
			minBox.Text	= min.ToString();
			maxBox.Text	= max.ToString();
			slider.Min	= min;
			slider.Max	= max;

			// initial filter values
			filter.Min = min;
			filter.Max = max;

			filterPreview.Filter = filter;
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
            this.label1 = new System.Windows.Forms.Label();
            this.minBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.maxBox = new System.Windows.Forms.TextBox();
            this.slider = new PhotoBrushProject.ColorSlider();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.filterPreview = new PhotoBrushProject.FilterPreview();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(10, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(30, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "M&in:";
            // 
            // minBox
            // 
            this.minBox.Location = new System.Drawing.Point(45, 10);
            this.minBox.Name = "minBox";
            this.minBox.Size = new System.Drawing.Size(50, 20);
            this.minBox.TabIndex = 1;
            this.minBox.TextChanged += new System.EventHandler(this.minBox_TextChanged);
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(183, 13);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(30, 15);
            this.label2.TabIndex = 2;
            this.label2.Text = "M&ax:";
            // 
            // maxBox
            // 
            this.maxBox.Location = new System.Drawing.Point(218, 10);
            this.maxBox.Name = "maxBox";
            this.maxBox.Size = new System.Drawing.Size(50, 20);
            this.maxBox.TabIndex = 3;
            this.maxBox.TextChanged += new System.EventHandler(this.maxBox_TextChanged);
            // 
            // slider
            // 
            this.slider.Location = new System.Drawing.Point(8, 40);
            this.slider.Name = "slider";
            this.slider.Size = new System.Drawing.Size(262, 23);
            this.slider.TabIndex = 4;
            this.slider.Type = PhotoBrushProject.ColorSliderType.Threshold;
            this.slider.ValuesChanged += new System.EventHandler(this.slider_ValuesChanged);
            // 
            // pictureBox2
            // 
            this.pictureBox2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox2.Location = new System.Drawing.Point(10, 120);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(258, 2);
            this.pictureBox2.TabIndex = 11;
            this.pictureBox2.TabStop = false;
            // 
            // okButton
            // 
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point(57, 140);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 6;
            this.okButton.Text = "&Ok";
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(147, 140);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 7;
            this.cancelButton.Text = "&Cancel";
            // 
            // filterPreview
            // 
            this.filterPreview.Image = null;
            this.filterPreview.Location = new System.Drawing.Point(10, 15);
            this.filterPreview.Name = "filterPreview";
            this.filterPreview.Size = new System.Drawing.Size(140, 140);
            this.filterPreview.TabIndex = 12;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.filterPreview);
            this.groupBox1.Location = new System.Drawing.Point(290, 5);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(160, 165);
            this.groupBox1.TabIndex = 13;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Preview";
            // 
            // ThresholdForm
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(459, 178);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.slider);
            this.Controls.Add(this.maxBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.minBox);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ThresholdForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Threshold";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		// Min edit box changed
		private void minBox_TextChanged(object sender, System.EventArgs e)
		{
			try
			{
				slider.Min = min = byte.Parse(minBox.Text);

				RefreshPreview();
			}
			catch (Exception)
			{
			}
		}

		// Max edit box changed
		private void maxBox_TextChanged(object sender, System.EventArgs e)
		{
			try
			{
				slider.Max = max = byte.Parse(maxBox.Text);

				RefreshPreview();
			}
			catch (Exception)
			{
			}
		}

		// Slider position changed
		private void slider_ValuesChanged(object sender, System.EventArgs e)
		{
			min = (byte) slider.Min;
			max = (byte) slider.Max;
			minBox.Text = min.ToString();
			maxBox.Text = max.ToString();
		}

		// Refresh filter
		private void RefreshPreview()
		{
			filter.Min = min;
			filter.Max = max;
			filterPreview.RefreshFilter();
		}
	}
}
