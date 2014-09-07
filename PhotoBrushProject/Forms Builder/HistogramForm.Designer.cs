namespace PhotoBrushProject
{
    partial class HistogramForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HistogramForm));
            this.panel1 = new System.Windows.Forms.Panel();
            this.maxLabel = new System.Windows.Forms.Label();
            this.minLabel = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.percentileLabel = new System.Windows.Forms.Label();
            this.countLabel = new System.Windows.Forms.Label();
            this.levelLabel = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.medianLabel = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.stdDevLabel = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.meanLabel = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.logCheck = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.channelCombo = new System.Windows.Forms.ComboBox();
            this.histogram = new PhotoBrushProject.Histogram();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.maxLabel);
            this.panel1.Controls.Add(this.minLabel);
            this.panel1.Controls.Add(this.label9);
            this.panel1.Controls.Add(this.label8);
            this.panel1.Controls.Add(this.percentileLabel);
            this.panel1.Controls.Add(this.countLabel);
            this.panel1.Controls.Add(this.levelLabel);
            this.panel1.Controls.Add(this.label7);
            this.panel1.Controls.Add(this.label6);
            this.panel1.Controls.Add(this.label5);
            this.panel1.Controls.Add(this.medianLabel);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.stdDevLabel);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.meanLabel);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.logCheck);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.channelCombo);
            this.panel1.Controls.Add(this.histogram);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(295, 301);
            this.panel1.TabIndex = 0;
            // 
            // maxLabel
            // 
            this.maxLabel.Location = new System.Drawing.Point(208, 267);
            this.maxLabel.Name = "maxLabel";
            this.maxLabel.Size = new System.Drawing.Size(40, 24);
            this.maxLabel.TabIndex = 59;
            this.maxLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // minLabel
            // 
            this.minLabel.Location = new System.Drawing.Point(72, 267);
            this.minLabel.Name = "minLabel";
            this.minLabel.Size = new System.Drawing.Size(40, 24);
            this.minLabel.TabIndex = 58;
            this.minLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label9
            // 
            this.label9.Location = new System.Drawing.Point(143, 267);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(30, 20);
            this.label9.TabIndex = 57;
            this.label9.Text = "Max:";
            // 
            // label8
            // 
            this.label8.Location = new System.Drawing.Point(23, 267);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(30, 20);
            this.label8.TabIndex = 56;
            this.label8.Text = "Min:";
            // 
            // percentileLabel
            // 
            this.percentileLabel.Location = new System.Drawing.Point(208, 244);
            this.percentileLabel.Name = "percentileLabel";
            this.percentileLabel.Size = new System.Drawing.Size(60, 21);
            this.percentileLabel.TabIndex = 55;
            // 
            // countLabel
            // 
            this.countLabel.Location = new System.Drawing.Point(208, 224);
            this.countLabel.Name = "countLabel";
            this.countLabel.Size = new System.Drawing.Size(60, 21);
            this.countLabel.TabIndex = 54;
            // 
            // levelLabel
            // 
            this.levelLabel.Location = new System.Drawing.Point(208, 204);
            this.levelLabel.Name = "levelLabel";
            this.levelLabel.Size = new System.Drawing.Size(60, 21);
            this.levelLabel.TabIndex = 53;
            // 
            // label7
            // 
            this.label7.Location = new System.Drawing.Point(143, 244);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(59, 22);
            this.label7.TabIndex = 52;
            this.label7.Text = "Percentile:";
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(143, 224);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(53, 21);
            this.label6.TabIndex = 51;
            this.label6.Text = "Count:";
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(143, 204);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(44, 19);
            this.label5.TabIndex = 50;
            this.label5.Text = "Level:";
            // 
            // medianLabel
            // 
            this.medianLabel.Location = new System.Drawing.Point(68, 244);
            this.medianLabel.Name = "medianLabel";
            this.medianLabel.Size = new System.Drawing.Size(40, 20);
            this.medianLabel.TabIndex = 49;
            this.medianLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(23, 244);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(47, 18);
            this.label4.TabIndex = 48;
            this.label4.Text = "Median:";
            // 
            // stdDevLabel
            // 
            this.stdDevLabel.Location = new System.Drawing.Point(68, 224);
            this.stdDevLabel.Name = "stdDevLabel";
            this.stdDevLabel.Size = new System.Drawing.Size(40, 20);
            this.stdDevLabel.TabIndex = 47;
            this.stdDevLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(23, 224);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(47, 18);
            this.label3.TabIndex = 46;
            this.label3.Text = "Std Dev:";
            // 
            // meanLabel
            // 
            this.meanLabel.Location = new System.Drawing.Point(68, 204);
            this.meanLabel.Name = "meanLabel";
            this.meanLabel.Size = new System.Drawing.Size(40, 20);
            this.meanLabel.TabIndex = 45;
            this.meanLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(23, 204);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(37, 18);
            this.label2.TabIndex = 44;
            this.label2.Text = "Mean:";
            // 
            // logCheck
            // 
            this.logCheck.Location = new System.Drawing.Point(233, 11);
            this.logCheck.Name = "logCheck";
            this.logCheck.Size = new System.Drawing.Size(50, 19);
            this.logCheck.TabIndex = 43;
            this.logCheck.Text = "Log";
            this.logCheck.CheckedChanged += new System.EventHandler(this.logCheck_CheckedChanged);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(20, 14);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(51, 16);
            this.label1.TabIndex = 41;
            this.label1.Text = "Channel:";
            // 
            // channelCombo
            // 
            this.channelCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.channelCombo.Location = new System.Drawing.Point(75, 10);
            this.channelCombo.Name = "channelCombo";
            this.channelCombo.Size = new System.Drawing.Size(130, 21);
            this.channelCombo.TabIndex = 42;
            this.channelCombo.SelectedIndexChanged += new System.EventHandler(this.channelCombo_SelectedIndexChanged);
            // 
            // histogram
            // 
            this.histogram.AllowSelection = true;
            this.histogram.BackColor = System.Drawing.SystemColors.Window;
            this.histogram.Location = new System.Drawing.Point(12, 37);
            this.histogram.Name = "histogram";
            this.histogram.Size = new System.Drawing.Size(271, 163);
            this.histogram.TabIndex = 40;
            this.histogram.Text = "histogram1";
            this.histogram.PositionChanged += new PhotoBrushProject.Histogram.HistogramEventHandler(this.histogram_PositionChanged);
            this.histogram.SelectionChanged += new PhotoBrushProject.Histogram.HistogramEventHandler(this.histogram_SelectionChanged);
            // 
            // HistogramForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(295, 301);
            this.Controls.Add(this.panel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "HistogramForm";
            this.Text = "HistogramForm";
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label maxLabel;
        private System.Windows.Forms.Label minLabel;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label percentileLabel;
        private System.Windows.Forms.Label countLabel;
        private System.Windows.Forms.Label levelLabel;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label medianLabel;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label stdDevLabel;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label meanLabel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox logCheck;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox channelCombo;
        private Histogram histogram;
        public System.Windows.Forms.Panel panel1;

    }
}