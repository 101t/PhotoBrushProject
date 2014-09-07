namespace Painting.Painter
{
    partial class FormColorEditor
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tpEditor = new System.Windows.Forms.TabControl();
            this.tpSolid = new System.Windows.Forms.TabPage();
            this.label3 = new System.Windows.Forms.Label();
            this.cboLineThickness = new System.Windows.Forms.ComboBox();
            this.chkPaintFill = new System.Windows.Forms.CheckBox();
            this.chkPaintLine = new System.Windows.Forms.CheckBox();
            this.btnSelectLineColor = new System.Windows.Forms.Button();
            this.btnSelectFillColor = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnAccept = new System.Windows.Forms.Button();
            this.tpGradient = new System.Windows.Forms.TabPage();
            this.label4 = new System.Windows.Forms.Label();
            this.cboLineThickness2 = new System.Windows.Forms.ComboBox();
            this.btnCancel2 = new System.Windows.Forms.Button();
            this.btnAccept2 = new System.Windows.Forms.Button();
            this.btnSecondaryColor = new System.Windows.Forms.Button();
            this.chkPaintFill_2 = new System.Windows.Forms.CheckBox();
            this.chkPaintLine_2 = new System.Windows.Forms.CheckBox();
            this.btnLineColor = new System.Windows.Forms.Button();
            this.btnPrimaryColor = new System.Windows.Forms.Button();
            this.lblCoverageArea = new System.Windows.Forms.Label();
            this.tbCoverageArea = new System.Windows.Forms.TrackBar();
            this.lblBlendSmoothing = new System.Windows.Forms.Label();
            this.tbBlendSmoothing = new System.Windows.Forms.TrackBar();
            this.label2 = new System.Windows.Forms.Label();
            this.cboGradientStyle = new System.Windows.Forms.ComboBox();
            this.tpEditor.SuspendLayout();
            this.tpSolid.SuspendLayout();
            this.tpGradient.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tbCoverageArea)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbBlendSmoothing)).BeginInit();
            this.SuspendLayout();
            // 
            // tpEditor
            // 
            this.tpEditor.Controls.Add(this.tpSolid);
            this.tpEditor.Controls.Add(this.tpGradient);
            this.tpEditor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tpEditor.Location = new System.Drawing.Point(0, 0);
            this.tpEditor.Name = "tpEditor";
            this.tpEditor.SelectedIndex = 0;
            this.tpEditor.Size = new System.Drawing.Size(505, 307);
            this.tpEditor.TabIndex = 0;
            this.tpEditor.SelectedIndexChanged += new System.EventHandler(this.tpEditor_SelectedIndexChanged);
            // 
            // tpSolid
            // 
            this.tpSolid.Controls.Add(this.label3);
            this.tpSolid.Controls.Add(this.cboLineThickness);
            this.tpSolid.Controls.Add(this.chkPaintFill);
            this.tpSolid.Controls.Add(this.chkPaintLine);
            this.tpSolid.Controls.Add(this.btnSelectLineColor);
            this.tpSolid.Controls.Add(this.btnSelectFillColor);
            this.tpSolid.Controls.Add(this.label1);
            this.tpSolid.Controls.Add(this.btnCancel);
            this.tpSolid.Controls.Add(this.btnAccept);
            this.tpSolid.Location = new System.Drawing.Point(4, 22);
            this.tpSolid.Name = "tpSolid";
            this.tpSolid.Padding = new System.Windows.Forms.Padding(3);
            this.tpSolid.Size = new System.Drawing.Size(497, 281);
            this.tpSolid.TabIndex = 0;
            this.tpSolid.Text = "Solid";
            this.tpSolid.UseVisualStyleBackColor = true;
            this.tpSolid.Paint += new System.Windows.Forms.PaintEventHandler(this.tpSolid_Paint);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(383, 73);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(56, 13);
            this.label3.TabIndex = 13;
            this.label3.Text = "Thickness";
            // 
            // cboLineThickness
            // 
            this.cboLineThickness.FormattingEnabled = true;
            this.cboLineThickness.Items.AddRange(new object[] {
            "0.1",
            "0.2",
            "0.4",
            "0.5",
            "0.6",
            "0.8",
            "1.0",
            "1.2",
            "1.4",
            "1.5",
            "1.6",
            "1.8",
            "2.0",
            "2.25",
            "2.5",
            "2.75",
            "3.5",
            "4.0",
            "4.75",
            "5.0",
            "6.0",
            "7.0",
            "8.0",
            "9.0",
            "10.0",
            "15.0",
            "20.0",
            "25.0",
            "35.0",
            "45.0",
            "50.0"});
            this.cboLineThickness.Location = new System.Drawing.Point(297, 70);
            this.cboLineThickness.Name = "cboLineThickness";
            this.cboLineThickness.Size = new System.Drawing.Size(80, 21);
            this.cboLineThickness.TabIndex = 12;
            this.cboLineThickness.SelectedIndexChanged += new System.EventHandler(this.cboLineThickness_SelectedIndexChanged);
            this.cboLineThickness.Leave += new System.EventHandler(this.cboLineThickness_Leave);
            this.cboLineThickness.KeyDown += new System.Windows.Forms.KeyEventHandler(this.cboLineThickness_KeyDown);
            // 
            // chkPaintFill
            // 
            this.chkPaintFill.AutoSize = true;
            this.chkPaintFill.Location = new System.Drawing.Point(103, 72);
            this.chkPaintFill.Name = "chkPaintFill";
            this.chkPaintFill.Size = new System.Drawing.Size(50, 17);
            this.chkPaintFill.TabIndex = 11;
            this.chkPaintFill.Text = "Paint";
            this.chkPaintFill.UseVisualStyleBackColor = true;
            this.chkPaintFill.CheckedChanged += new System.EventHandler(this.chkPaintFill_CheckedChanged);
            // 
            // chkPaintLine
            // 
            this.chkPaintLine.AutoSize = true;
            this.chkPaintLine.Location = new System.Drawing.Point(386, 41);
            this.chkPaintLine.Name = "chkPaintLine";
            this.chkPaintLine.Size = new System.Drawing.Size(50, 17);
            this.chkPaintLine.TabIndex = 10;
            this.chkPaintLine.Text = "Paint";
            this.chkPaintLine.UseVisualStyleBackColor = true;
            this.chkPaintLine.CheckedChanged += new System.EventHandler(this.chkPaintLine_CheckedChanged);
            // 
            // btnSelectLineColor
            // 
            this.btnSelectLineColor.Location = new System.Drawing.Point(297, 33);
            this.btnSelectLineColor.Name = "btnSelectLineColor";
            this.btnSelectLineColor.Size = new System.Drawing.Size(80, 31);
            this.btnSelectLineColor.TabIndex = 9;
            this.btnSelectLineColor.Text = "Line Color";
            this.btnSelectLineColor.UseVisualStyleBackColor = true;
            this.btnSelectLineColor.Click += new System.EventHandler(this.btnSelectLineColor_Click);
            // 
            // btnSelectFillColor
            // 
            this.btnSelectFillColor.Location = new System.Drawing.Point(91, 33);
            this.btnSelectFillColor.Name = "btnSelectFillColor";
            this.btnSelectFillColor.Size = new System.Drawing.Size(75, 31);
            this.btnSelectFillColor.TabIndex = 8;
            this.btnSelectFillColor.Text = "Fill Color";
            this.btnSelectFillColor.UseVisualStyleBackColor = true;
            this.btnSelectFillColor.Click += new System.EventHandler(this.btnSelectFillColor_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(0, 13);
            this.label1.TabIndex = 7;
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(253, 228);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 6;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnAccept
            // 
            this.btnAccept.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnAccept.Location = new System.Drawing.Point(158, 228);
            this.btnAccept.Name = "btnAccept";
            this.btnAccept.Size = new System.Drawing.Size(75, 23);
            this.btnAccept.TabIndex = 5;
            this.btnAccept.Text = "Accept";
            this.btnAccept.UseVisualStyleBackColor = true;
            this.btnAccept.Click += new System.EventHandler(this.btnAccept_Click);
            // 
            // tpGradient
            // 
            this.tpGradient.Controls.Add(this.label4);
            this.tpGradient.Controls.Add(this.cboLineThickness2);
            this.tpGradient.Controls.Add(this.btnCancel2);
            this.tpGradient.Controls.Add(this.btnAccept2);
            this.tpGradient.Controls.Add(this.btnSecondaryColor);
            this.tpGradient.Controls.Add(this.chkPaintFill_2);
            this.tpGradient.Controls.Add(this.chkPaintLine_2);
            this.tpGradient.Controls.Add(this.btnLineColor);
            this.tpGradient.Controls.Add(this.btnPrimaryColor);
            this.tpGradient.Controls.Add(this.lblCoverageArea);
            this.tpGradient.Controls.Add(this.tbCoverageArea);
            this.tpGradient.Controls.Add(this.lblBlendSmoothing);
            this.tpGradient.Controls.Add(this.tbBlendSmoothing);
            this.tpGradient.Controls.Add(this.label2);
            this.tpGradient.Controls.Add(this.cboGradientStyle);
            this.tpGradient.Location = new System.Drawing.Point(4, 22);
            this.tpGradient.Name = "tpGradient";
            this.tpGradient.Padding = new System.Windows.Forms.Padding(3);
            this.tpGradient.Size = new System.Drawing.Size(497, 281);
            this.tpGradient.TabIndex = 1;
            this.tpGradient.Text = "Gradient";
            this.tpGradient.UseVisualStyleBackColor = true;
            this.tpGradient.Paint += new System.Windows.Forms.PaintEventHandler(this.tpGradient_Paint);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(119, 73);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(56, 13);
            this.label4.TabIndex = 20;
            this.label4.Text = "Thickness";
            // 
            // cboLineThickness2
            // 
            this.cboLineThickness2.FormattingEnabled = true;
            this.cboLineThickness2.Items.AddRange(new object[] {
            "0.1",
            "0.2",
            "0.4",
            "0.5",
            "0.6",
            "0.8",
            "1.0",
            "1.2",
            "1.4",
            "1.5",
            "1.6",
            "1.8",
            "2.0",
            "2.25",
            "2.5",
            "2.75",
            "3.5",
            "4.0",
            "4.75",
            "5.0",
            "6.0",
            "7.0",
            "8.0",
            "9.0",
            "10.0",
            "15.0",
            "20.0",
            "25.0",
            "35.0",
            "45.0",
            "50.0"});
            this.cboLineThickness2.Location = new System.Drawing.Point(33, 70);
            this.cboLineThickness2.Name = "cboLineThickness2";
            this.cboLineThickness2.Size = new System.Drawing.Size(80, 21);
            this.cboLineThickness2.TabIndex = 19;
            this.cboLineThickness2.SelectedIndexChanged += new System.EventHandler(this.cboLineThickness2_SelectedIndexChanged);
            this.cboLineThickness2.Leave += new System.EventHandler(this.cboLineThickness2_Leave);
            this.cboLineThickness2.KeyDown += new System.Windows.Forms.KeyEventHandler(this.cboLineThickness2_KeyDown);
            // 
            // btnCancel2
            // 
            this.btnCancel2.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel2.Location = new System.Drawing.Point(195, 229);
            this.btnCancel2.Name = "btnCancel2";
            this.btnCancel2.Size = new System.Drawing.Size(75, 23);
            this.btnCancel2.TabIndex = 18;
            this.btnCancel2.Text = "Cancel";
            this.btnCancel2.UseVisualStyleBackColor = true;
            this.btnCancel2.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnAccept2
            // 
            this.btnAccept2.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnAccept2.Location = new System.Drawing.Point(100, 229);
            this.btnAccept2.Name = "btnAccept2";
            this.btnAccept2.Size = new System.Drawing.Size(75, 23);
            this.btnAccept2.TabIndex = 17;
            this.btnAccept2.Text = "Accept";
            this.btnAccept2.UseVisualStyleBackColor = true;
            this.btnAccept2.Click += new System.EventHandler(this.btnAccept_Click);
            // 
            // btnSecondaryColor
            // 
            this.btnSecondaryColor.Location = new System.Drawing.Point(33, 159);
            this.btnSecondaryColor.Name = "btnSecondaryColor";
            this.btnSecondaryColor.Size = new System.Drawing.Size(86, 35);
            this.btnSecondaryColor.TabIndex = 16;
            this.btnSecondaryColor.Text = "Secondary Color";
            this.btnSecondaryColor.UseVisualStyleBackColor = true;
            this.btnSecondaryColor.Click += new System.EventHandler(this.btnSecondaryColor_Click);
            // 
            // chkPaintFill_2
            // 
            this.chkPaintFill_2.AutoSize = true;
            this.chkPaintFill_2.Location = new System.Drawing.Point(125, 147);
            this.chkPaintFill_2.Name = "chkPaintFill_2";
            this.chkPaintFill_2.Size = new System.Drawing.Size(50, 17);
            this.chkPaintFill_2.TabIndex = 15;
            this.chkPaintFill_2.Text = "Paint";
            this.chkPaintFill_2.UseVisualStyleBackColor = true;
            this.chkPaintFill_2.CheckedChanged += new System.EventHandler(this.chkPaintFill_2_CheckedChanged);
            // 
            // chkPaintLine_2
            // 
            this.chkPaintLine_2.AutoSize = true;
            this.chkPaintLine_2.Location = new System.Drawing.Point(125, 41);
            this.chkPaintLine_2.Name = "chkPaintLine_2";
            this.chkPaintLine_2.Size = new System.Drawing.Size(50, 17);
            this.chkPaintLine_2.TabIndex = 14;
            this.chkPaintLine_2.Text = "Paint";
            this.chkPaintLine_2.UseVisualStyleBackColor = true;
            this.chkPaintLine_2.CheckedChanged += new System.EventHandler(this.chkPaintLine_2_CheckedChanged);
            // 
            // btnLineColor
            // 
            this.btnLineColor.Location = new System.Drawing.Point(33, 33);
            this.btnLineColor.Name = "btnLineColor";
            this.btnLineColor.Size = new System.Drawing.Size(86, 31);
            this.btnLineColor.TabIndex = 13;
            this.btnLineColor.Text = "Line Color";
            this.btnLineColor.UseVisualStyleBackColor = true;
            this.btnLineColor.Click += new System.EventHandler(this.btnLineColor_Click);
            // 
            // btnPrimaryColor
            // 
            this.btnPrimaryColor.Location = new System.Drawing.Point(33, 114);
            this.btnPrimaryColor.Name = "btnPrimaryColor";
            this.btnPrimaryColor.Size = new System.Drawing.Size(86, 39);
            this.btnPrimaryColor.TabIndex = 12;
            this.btnPrimaryColor.Text = "Primary Color";
            this.btnPrimaryColor.UseVisualStyleBackColor = true;
            this.btnPrimaryColor.Click += new System.EventHandler(this.btnPrimaryColor_Click);
            // 
            // lblCoverageArea
            // 
            this.lblCoverageArea.BackColor = System.Drawing.Color.Transparent;
            this.lblCoverageArea.Location = new System.Drawing.Point(426, 229);
            this.lblCoverageArea.Name = "lblCoverageArea";
            this.lblCoverageArea.Size = new System.Drawing.Size(61, 47);
            this.lblCoverageArea.TabIndex = 7;
            this.lblCoverageArea.Text = "Coverage:  100%";
            this.lblCoverageArea.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tbCoverageArea
            // 
            this.tbCoverageArea.BackColor = System.Drawing.Color.Gainsboro;
            this.tbCoverageArea.Location = new System.Drawing.Point(429, 3);
            this.tbCoverageArea.Maximum = 100;
            this.tbCoverageArea.Name = "tbCoverageArea";
            this.tbCoverageArea.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.tbCoverageArea.Size = new System.Drawing.Size(45, 223);
            this.tbCoverageArea.TabIndex = 6;
            this.tbCoverageArea.TickFrequency = 5;
            this.tbCoverageArea.TickStyle = System.Windows.Forms.TickStyle.Both;
            this.tbCoverageArea.ValueChanged += new System.EventHandler(this.tbCoverageArea_ValueChanged);
            // 
            // lblBlendSmoothing
            // 
            this.lblBlendSmoothing.BackColor = System.Drawing.Color.Transparent;
            this.lblBlendSmoothing.Location = new System.Drawing.Point(351, 229);
            this.lblBlendSmoothing.Name = "lblBlendSmoothing";
            this.lblBlendSmoothing.Size = new System.Drawing.Size(69, 47);
            this.lblBlendSmoothing.TabIndex = 5;
            this.lblBlendSmoothing.Text = "Blend Smoothness:  100%";
            this.lblBlendSmoothing.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tbBlendSmoothing
            // 
            this.tbBlendSmoothing.BackColor = System.Drawing.Color.Gainsboro;
            this.tbBlendSmoothing.Location = new System.Drawing.Point(364, 3);
            this.tbBlendSmoothing.Maximum = 100;
            this.tbBlendSmoothing.Name = "tbBlendSmoothing";
            this.tbBlendSmoothing.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.tbBlendSmoothing.Size = new System.Drawing.Size(45, 223);
            this.tbBlendSmoothing.TabIndex = 4;
            this.tbBlendSmoothing.TickFrequency = 5;
            this.tbBlendSmoothing.TickStyle = System.Windows.Forms.TickStyle.Both;
            this.tbBlendSmoothing.ValueChanged += new System.EventHandler(this.tbBlendSmoothing_ValueChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(30, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(76, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Gradient Style:";
            // 
            // cboGradientStyle
            // 
            this.cboGradientStyle.FormattingEnabled = true;
            this.cboGradientStyle.Items.AddRange(new object[] {
            "Horizontal",
            "Vertical",
            "ForwardDiagonal",
            "BackwardDiagonal"});
            this.cboGradientStyle.Location = new System.Drawing.Point(112, 6);
            this.cboGradientStyle.Name = "cboGradientStyle";
            this.cboGradientStyle.Size = new System.Drawing.Size(184, 21);
            this.cboGradientStyle.TabIndex = 1;
            this.cboGradientStyle.SelectedIndexChanged += new System.EventHandler(this.cboGradientStyle_SelectedIndexChanged);
            // 
            // frmColorEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(505, 307);
            this.Controls.Add(this.tpEditor);
            this.Name = "frmColorEditor";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Color Editor";
            this.tpEditor.ResumeLayout(false);
            this.tpSolid.ResumeLayout(false);
            this.tpSolid.PerformLayout();
            this.tpGradient.ResumeLayout(false);
            this.tpGradient.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tbCoverageArea)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbBlendSmoothing)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tpEditor;
        private System.Windows.Forms.TabPage tpSolid;
        private System.Windows.Forms.TabPage tpGradient;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnAccept;
        private System.Windows.Forms.Button btnSelectFillColor;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox chkPaintFill;
        private System.Windows.Forms.CheckBox chkPaintLine;
        private System.Windows.Forms.Button btnSelectLineColor;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cboGradientStyle;
        private System.Windows.Forms.TrackBar tbBlendSmoothing;
        private System.Windows.Forms.Label lblBlendSmoothing;
        private System.Windows.Forms.Label lblCoverageArea;
        private System.Windows.Forms.TrackBar tbCoverageArea;
        private System.Windows.Forms.Button btnSecondaryColor;
        private System.Windows.Forms.CheckBox chkPaintFill_2;
        private System.Windows.Forms.CheckBox chkPaintLine_2;
        private System.Windows.Forms.Button btnLineColor;
        private System.Windows.Forms.Button btnPrimaryColor;
        private System.Windows.Forms.Button btnCancel2;
        private System.Windows.Forms.Button btnAccept2;
        private System.Windows.Forms.ComboBox cboLineThickness;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cboLineThickness2;
    }
}