namespace PhotoBrushProject
{
    partial class ImageHandlerForm
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
            if (disposing)// && (components != null))
            {
                //components.Dispose();
                //if (image != null)
                //{
                //    image.Dispose();
                //}
                if (components != null)
                {
                    components.Dispose();
                }
                if (image != null)
                {
                    image.Dispose();
                }
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ImageHandlerForm));
            this.SuspendLayout();
            // 
            // ImageHandlerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::PhotoBrushProject.Properties.Resources.background;
            this.ClientSize = new System.Drawing.Size(654, 406);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ImageHandlerForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ImageHandlerForm_FormClosing);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ImageDoc_MouseDown);
            this.MouseEnter += new System.EventHandler(this.ImageHandlerForm_MouseEnter);
            this.MouseLeave += new System.EventHandler(this.ImageDoc_MouseLeave);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.ImageDoc_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.ImageDoc_MouseUp);
            this.ResumeLayout(false);

        }

        #endregion


    }
}