namespace PhotoBrushProject
{
    partial class PaintHandlerForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PaintHandlerForm));
            this.drawingCanvas1 = new Painting.Shapes.DrawingCanvas();
            this.shapeControlPanel1 = new PhotoBrushProject.ShapeControlPanel(drawingCanvas1);
            this.SuspendLayout();
            // 
            // drawingCanvas1
            // 
            this.drawingCanvas1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.drawingCanvas1.BackColor = System.Drawing.SystemColors.Window;
            this.drawingCanvas1.Location = new System.Drawing.Point(114, 12);
            this.drawingCanvas1.Name = "drawingCanvas1";
            this.drawingCanvas1.Size = new System.Drawing.Size(560, 459);
            this.drawingCanvas1.TabIndex = 0;
            // 
            // shapeControlPanel1
            // 
            this.shapeControlPanel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.shapeControlPanel1.Location = new System.Drawing.Point(12, 12);
            this.shapeControlPanel1.Name = "shapeControlPanel1";
            this.shapeControlPanel1.Size = new System.Drawing.Size(96, 462);
            this.shapeControlPanel1.TabIndex = 2;
            // 
            // PaintHandlerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.ClientSize = new System.Drawing.Size(686, 484);
            this.Controls.Add(this.shapeControlPanel1);
            this.Controls.Add(this.drawingCanvas1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "PaintHandlerForm";
            this.ResumeLayout(false);

        }
        #endregion
    }
}