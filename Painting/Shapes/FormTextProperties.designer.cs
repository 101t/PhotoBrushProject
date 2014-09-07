namespace Painting.Shapes
{
    partial class FormTextProperties
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
            this.btnTextCancel = new System.Windows.Forms.Button();
            this.btnTextAccept = new System.Windows.Forms.Button();
            this.btnAlignRight = new System.Windows.Forms.Button();
            this.btnAlignMiddle = new System.Windows.Forms.Button();
            this.btnAlignLeft = new System.Windows.Forms.Button();
            this.txtContents = new System.Windows.Forms.TextBox();
            this.btnFont = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnTextCancel
            // 
            this.btnTextCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnTextCancel.Location = new System.Drawing.Point(220, 226);
            this.btnTextCancel.Name = "btnTextCancel";
            this.btnTextCancel.Size = new System.Drawing.Size(75, 23);
            this.btnTextCancel.TabIndex = 16;
            this.btnTextCancel.Text = "Cancel";
            this.btnTextCancel.UseVisualStyleBackColor = true;
            this.btnTextCancel.Click += new System.EventHandler(this.btnTextCancel_Click);
            // 
            // btnTextAccept
            // 
            this.btnTextAccept.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnTextAccept.Location = new System.Drawing.Point(125, 226);
            this.btnTextAccept.Name = "btnTextAccept";
            this.btnTextAccept.Size = new System.Drawing.Size(75, 23);
            this.btnTextAccept.TabIndex = 15;
            this.btnTextAccept.Text = "Accept";
            this.btnTextAccept.UseVisualStyleBackColor = true;
            this.btnTextAccept.Click += new System.EventHandler(this.btnTextAccept_Click);
            // 
            // btnAlignRight
            // 
            this.btnAlignRight.Location = new System.Drawing.Point(336, 9);
            this.btnAlignRight.Name = "btnAlignRight";
            this.btnAlignRight.Size = new System.Drawing.Size(66, 35);
            this.btnAlignRight.TabIndex = 14;
            this.btnAlignRight.Tag = "2";
            this.btnAlignRight.Text = "Align Right";
            this.btnAlignRight.UseVisualStyleBackColor = true;
            this.btnAlignRight.Click += new System.EventHandler(this.btnAlignRight_Click);
            // 
            // btnAlignMiddle
            // 
            this.btnAlignMiddle.Location = new System.Drawing.Point(270, 9);
            this.btnAlignMiddle.Name = "btnAlignMiddle";
            this.btnAlignMiddle.Size = new System.Drawing.Size(60, 35);
            this.btnAlignMiddle.TabIndex = 13;
            this.btnAlignMiddle.Tag = "1";
            this.btnAlignMiddle.Text = "Align Middle";
            this.btnAlignMiddle.UseVisualStyleBackColor = true;
            this.btnAlignMiddle.Click += new System.EventHandler(this.btnAlignMiddle_Click);
            // 
            // btnAlignLeft
            // 
            this.btnAlignLeft.Location = new System.Drawing.Point(198, 9);
            this.btnAlignLeft.Name = "btnAlignLeft";
            this.btnAlignLeft.Size = new System.Drawing.Size(66, 35);
            this.btnAlignLeft.TabIndex = 12;
            this.btnAlignLeft.Tag = "0";
            this.btnAlignLeft.Text = "Align Left";
            this.btnAlignLeft.UseVisualStyleBackColor = true;
            this.btnAlignLeft.Click += new System.EventHandler(this.btnAlignLeft_Click);
            // 
            // txtContents
            // 
            this.txtContents.Location = new System.Drawing.Point(36, 74);
            this.txtContents.Multiline = true;
            this.txtContents.Name = "txtContents";
            this.txtContents.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtContents.Size = new System.Drawing.Size(366, 110);
            this.txtContents.TabIndex = 11;
            this.txtContents.Text = "How in the world are you anyway?";
            this.txtContents.TextChanged += new System.EventHandler(this.txtContents_TextChanged);
            // 
            // btnFont
            // 
            this.btnFont.Location = new System.Drawing.Point(36, 15);
            this.btnFont.Name = "btnFont";
            this.btnFont.Size = new System.Drawing.Size(66, 22);
            this.btnFont.TabIndex = 10;
            this.btnFont.Text = "Font";
            this.btnFont.UseVisualStyleBackColor = true;
            this.btnFont.Click += new System.EventHandler(this.btnFont_Click);
            // 
            // frmTextProperties
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(438, 258);
            this.Controls.Add(this.btnTextCancel);
            this.Controls.Add(this.btnTextAccept);
            this.Controls.Add(this.btnAlignRight);
            this.Controls.Add(this.btnAlignMiddle);
            this.Controls.Add(this.btnAlignLeft);
            this.Controls.Add(this.txtContents);
            this.Controls.Add(this.btnFont);
            this.Name = "frmTextProperties";
            this.Text = "Text Properties";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmTextProperties_FormClosed);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnTextCancel;
        private System.Windows.Forms.Button btnTextAccept;
        private System.Windows.Forms.Button btnAlignRight;
        private System.Windows.Forms.Button btnAlignMiddle;
        private System.Windows.Forms.Button btnAlignLeft;
        private System.Windows.Forms.TextBox txtContents;
        private System.Windows.Forms.Button btnFont;
    }
}