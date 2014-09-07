using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using AForge.Math;
using AForge.Imaging;
using AForge.Imaging.Filters;
using AForge.Imaging.Textures;

namespace PhotoBrushProject
{
    public partial class ImageHandlerForm : Form, IFormBuilder
    {
        Rectangle IFormBuilder.Bounds { get { return this.Bounds; } set { this.Bounds = value; } }
        FormBorderStyle IFormBuilder.FormBorderStyled { get { return this.FormBorderStyle; } set { this.FormBorderStyle = value; } }
        Color IFormBuilder.BackColor { get { return this.BackColor; } set { this.BackColor = value; } }
        string IFormBuilder.Text { get { return this.Text; } set { this.Text = value; } }
        bool IFormBuilder.TopLevel { get { return this.TopLevel; } set { this.TopLevel = value; } }

        private System.Drawing.Bitmap backup = null;
        private System.Drawing.Bitmap image = null;
        private string fileName = null;
        private string safeFileName = "";
        private int width;
        private int height;
        private float zoom = 1;
        private IDocumentsHost host = null;

        private bool cropping = false;
        private bool dragging = false;
        private Point start, end, startW, endW;

        // Image property
        public Bitmap Image
        {
            get { return image; }
        }
        // Width property
        public int ImageWidth
        {
            get { return width; }
        }
        // Height property
        public int ImageHeight
        {
            get { return height; }
        }
        // Zoom property
        public float Zoom
        {
            get { return zoom; }
        }
        // FileName property
        // return file name if the document was created from file or null
        public string FileName
        {
            get { return fileName; }
        }
        //Safe File Name property
        public string SafeFileName { get { return safeFileName; } set { fileName = value; } }


        // Events
        public delegate void SelectionEventHandler(object sender, SelectionEventArgs e);

        public event EventHandler DocumentChanged;
        public event EventHandler ZoomChanged;
        public event SelectionEventHandler MouseImagePosition;
        public event SelectionEventHandler SelectionChanged;

        // Constructors
        private ImageHandlerForm(IDocumentsHost host)
        {
            this.host = host;
            ((MainForm)host).IHFList.Add(this);
        }

        // Construct from file
        public ImageHandlerForm(string fileName, IDocumentsHost host) : this(host)
        {
            try
            {
                // load image
                image = (Bitmap)System.Drawing.Bitmap.FromFile(fileName);
                
                // format image
                AForge.Imaging.Image.FormatImage(ref image);

                this.fileName = fileName;
            }
            catch (Exception) { ((MainForm)host).ReadyToolStripStatusLabel.Text = "Failed loading image"; }
            Init();
        }

        // Construct from image
		public ImageHandlerForm(Bitmap image, IDocumentsHost host) : this(host)
		{
			this.image = image;
            AForge.Imaging.Image.FormatImage(ref this.image);

			Init();
		}

        // Init the document
        private void Init()
        {
            InitializeComponent();

            // form style
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.DoubleBuffer | ControlStyles.ResizeRedraw, true);

            // init scroll bars
            this.AutoScroll = true;

            UpdateSize();
            //UpdateNewImage();
        }

        // Update document size 
        private void UpdateSize()
        {
            // image dimension
            width = image.Width;
            height = image.Height;
            // scroll bar size
            this.AutoScrollMinSize = new Size((int)(width * zoom), (int)(height * zoom));
        }

        // Execute command
        public void ExecuteCommand(ImageDocCommands cmd)
        {
            switch (cmd)
            {
                case ImageDocCommands.Clone:		// clone the image
                    Clone();
                    break;
                case ImageDocCommands.Crop:			// crop the image
                    Crop();
                    break;
                case ImageDocCommands.ZoomIn:		// zoom in
                    ZoomIn();
                    break;
                case ImageDocCommands.ZoomOut:		// zoom out
                    ZoomOut();
                    break;
                case ImageDocCommands.ZoomOriginal:	// original size
                    zoom = 1;
                    UpdateZoom();
                    break;
                case ImageDocCommands.FitToSize:	// fit to screen
                    FitToScreen();
                    break;
                case ImageDocCommands.Levels:		// levels
                    Levels();
                    break;
                case ImageDocCommands.Grayscale:	// grayscale
                    Grayscale();
                    break;
                case ImageDocCommands.Threshold:	// threshold
                    Threshold();
                    break;
                case ImageDocCommands.Morphology:	// morphology
                    Morphology();
                    break;
                case ImageDocCommands.Convolution:	// convolution
                    Convolution();
                    break;
                case ImageDocCommands.Resize:		// resize the image
                    ResizeImage();
                    break;
                case ImageDocCommands.Rotate:		// rotate the image
                    RotateImage();
                    break;
                case ImageDocCommands.Brightness:	// adjust brightness
                    Brightness();
                    break;
                case ImageDocCommands.Contrast:		// modify contrast
                    Contrast();
                    break;
                case ImageDocCommands.Saturation:	// adjust saturation
                    Saturation();
                    break;
                case ImageDocCommands.Fourier:		// fourier transformation
                    ForwardFourierTransformation();
                    break;
            }
        }

        // Update document and notify client about changes
        private void UpdateNewImage()
        {
            // update size
            UpdateSize();
            // repaint
            Invalidate();
            // notify host
            if (DocumentChanged != null)
                DocumentChanged(this, null);
        }

        // Reload image from file
        public void Reload()
        {
            if (fileName != null)
            {
                try
                {
                    // load image
                    Bitmap newImage = (Bitmap)Bitmap.FromFile(fileName);

                    // Release current image
                    image.Dispose();
                    // set document image to just loaded
                    image = newImage;

                    // format image
                    AForge.Imaging.Image.FormatImage(ref image);
                }
                catch (Exception){ ((MainForm)host).ReadyToolStripStatusLabel.Text = "Failed reloading image";}

                // update
                UpdateNewImage();
            }
        }

        // Center image in the document
        public void Center()
        {
            Rectangle rc = ClientRectangle;
            Point p = this.AutoScrollPosition;
            int width = (int)(this.width * zoom);
            int height = (int)(this.height * zoom);

            if (rc.Width < width)
                p.X = (width - rc.Width) >> 1;
            if (rc.Height < height)
                p.Y = (height - rc.Height) >> 1;

            this.AutoScrollPosition = p;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (image != null)
            {
                Graphics g = e.Graphics;
                Rectangle rc = ClientRectangle;
                Pen pen = new Pen(Color.FromArgb(0, 0, 0));

                int width = (int)(this.width * zoom);
                int height = (int)(this.height * zoom);
                int x = (rc.Width < width) ? this.AutoScrollPosition.X : (rc.Width - width) / 2;
                int y = (rc.Height < height) ? this.AutoScrollPosition.Y : (rc.Height - height) / 2;

                // draw rectangle around the image
                g.DrawRectangle(pen, x - 1, y - 1, width + 1, height + 1);

                // set nearest neighbor interpolation to avoid image smoothing
                g.InterpolationMode = InterpolationMode.NearestNeighbor;

                // draw image
                g.DrawImage(image, x, y, width, height);

                pen.Dispose();
            }
        }

        protected override void OnClick(EventArgs e)
        {
            this.Focus();
        }

        #region < Image & Filter Menu Event Methods >
        // Apply filter on the image
        private void ApplyFilter(IFilter filter)
        {
            try
            {
                // set wait cursor
                this.Cursor = Cursors.WaitCursor;

                // apply filter to the image
                Bitmap newImage = filter.Apply(image);

                if (host.CreateNewDocumentOnChange)
                {
                    // open new image in new document
                    host.NewDocument(newImage);
                }
                else
                {
                    if (host.RememberOnChange)
                    {
                        // backup current image
                        if (backup != null)
                            backup.Dispose();

                        backup = image;
                    }
                    else
                    {
                        // release current image
                        image.Dispose();
                    }

                    image = newImage;

                    // update
                    UpdateNewImage();
                }
            }
            catch (ArgumentException)
            {
                MessageBox.Show("Selected filter can not be applied to the image", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // restore cursor
                this.Cursor = Cursors.Default;
            }
        }

        public void imageItem_Popup(object sender, System.EventArgs e)
        {
            ((MainForm)host).backToolStripMenuItem.Enabled = (backup != null);
            ((MainForm)host).cropToolStripMenuItem.Checked = cropping;
        }

        // Restore image to previous
        public void backImageItem_Click(object sender, System.EventArgs e)
        {
            if (backup != null)
            {
                // release current image
                image.Dispose();
                // restore
                image = backup;
                backup = null;

                // update
                UpdateNewImage();
            }
        }

        // Clone the image
        private void Clone()
        {
            if (host != null)
            {
                Bitmap clone = AForge.Imaging.Image.Clone(image);

                if (!host.NewDocument(clone))
                {
                    clone.Dispose();
                }
            }
        }

        // On "Image->Clone" item click
        public void cloneImageItem_Click(object sender, System.EventArgs e)
        {
            Clone();
        }

        // Update zoom factor
        private void UpdateZoom()
        {
            try
            {
                this.AutoScrollMinSize = new Size((int)(width * zoom), (int)(height * zoom));
                this.Invalidate();

                // notify host
                if (ZoomChanged != null)
                    ZoomChanged(this, null);
            }
            catch (Exception E) { MessageBox.Show(E.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
        }

        // Zoom image
        public void zoomItem_Click(object sender, System.EventArgs e)
        {
            // get menu item text
            String t = ((ToolStripMenuItem)sender).Text;
            // parse it`s value
            int i = int.Parse(t.Remove(t.Length - 1, 1));
            // calc zoom factor
            zoom = (float)i / 100;

            UpdateZoom();
        }

        // Zoom In image
        private void ZoomIn()
        {
            float z = zoom * 1.5f;

            if (z <= 10)
            {
                zoom = z;
                UpdateZoom();
            }
        }

        // On "Image->Zoom->Zoom In" item click
        public void zoomInImageItem_Click(object sender, System.EventArgs e)
        {
            ZoomIn();
        }

        // Zoom Out image
        private void ZoomOut()
        {
            float z = zoom / 1.5f;

            if (z >= 0.05)
            {
                zoom = z;
                UpdateZoom();
            }
        }

        // On "Image->Zoom->Zoom out" item click
        public void zoomOutImageItem_Click(object sender, System.EventArgs e)
        {
            ZoomOut();
        }

        // Fit to size
        private void FitToScreen()
        {
            Rectangle rc = ClientRectangle;

            zoom = Math.Min((float)rc.Width / (width + 2), (float)rc.Height / (height + 2));

            UpdateZoom();
        }

        // On "Image->Zoom->Fit To Screen" item click
        public void zoomFitImageItem_Click(object sender, System.EventArgs e)
        {
            FitToScreen();
        }

        // Flip image
        public void flipImageItem_Click(object sender, System.EventArgs e)
        {
            image.RotateFlip(RotateFlipType.RotateNoneFlipY);

            Invalidate();
        }

        // Mirror image
        public void mirrorItem_Click(object sender, System.EventArgs e)
        {
            image.RotateFlip(RotateFlipType.RotateNoneFlipX);

            Invalidate();
        }

        // Rotate image 90 degree
        public void rotateImageItem_Click(object sender, System.EventArgs e)
        {
            image.RotateFlip(RotateFlipType.Rotate90FlipNone);

            // update
            UpdateNewImage();
        }

        // Invert image
        public void invertColorFiltersItem_Click(object sender, System.EventArgs e)
        {
            ApplyFilter(new Invert());
        }

        // Rotatet colors
        public void rotateColorFiltersItem_Click(object sender, System.EventArgs e)
        {
            ApplyFilter(new RotateChannels());
        }

        // Sepia image
        public void sepiaColorFiltersItem_Click(object sender, System.EventArgs e)
        {
            ApplyFilter(new Sepia());
        }

        // Grayscale image
        private void Grayscale()
        {
            if (image.PixelFormat == PixelFormat.Format8bppIndexed)
            {
                MessageBox.Show("The image is already grayscale", "Message", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            ApplyFilter(new GrayscaleBT709());
        }

        // On "Filter->Color->Grayscale"
        public void grayscaleColorFiltersItem_Click(object sender, System.EventArgs e)
        {
            Grayscale();
        }

        // Converts grayscale image to RGB
        public void toRgbColorFiltersItem_Click(object sender, System.EventArgs e)
        {
            if (image.PixelFormat == PixelFormat.Format24bppRgb)
            {
                MessageBox.Show("The image is already RGB", "Message", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            ApplyFilter(new GrayscaleToRGB());
        }

        // Remove green and blue channels
        public void redColorFiltersItem_Click(object sender, System.EventArgs e)
        {
            ApplyFilter(new ChannelFiltering(new Range(0, 255), new Range(0, 0), new Range(0, 0)));
        }

        // Remove red and blue channels
        public void greenColorFiltersItem_Click(object sender, System.EventArgs e)
        {
            ApplyFilter(new ChannelFiltering(new Range(0, 0), new Range(0, 255), new Range(0, 0)));
        }

        // Remove red and green channels
        public void blueColorFiltersItem_Click(object sender, System.EventArgs e)
        {
            ApplyFilter(new ChannelFiltering(new Range(0, 0), new Range(0, 0), new Range(0, 255)));
        }

        // Remove green channel
        public void cyanColorFiltersItem_Click(object sender, System.EventArgs e)
        {
            ApplyFilter(new ChannelFiltering(new Range(0, 0), new Range(0, 255), new Range(0, 255)));
        }

        // Remove green channel
        public void magentaColorFiltersItem_Click(object sender, System.EventArgs e)
        {
            ApplyFilter(new ChannelFiltering(new Range(0, 255), new Range(0, 0), new Range(0, 255)));
        }

        // Remove blue channel
        public void yellowColorFiltersItem_Click(object sender, System.EventArgs e)
        {
            ApplyFilter(new ChannelFiltering(new Range(0, 255), new Range(0, 255), new Range(0, 0)));
        }

        // Color filtering
        public void colorFilteringColorFiltersItem_Click(object sender, System.EventArgs e)
        {
            // check pixel format
            if (image.PixelFormat != PixelFormat.Format24bppRgb)
            {
                MessageBox.Show("Color filtering can be applied to RGB images only", "Message", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            ColorFilteringForm form = new ColorFilteringForm();
            form.Image = image;

            if (form.ShowDialog() == DialogResult.OK)
            {
                ApplyFilter(form.Filter);
            }
        }

        // Euclidean color filtering
        public void euclideanFilteringColorFiltersItem_Click(object sender, System.EventArgs e)
        {
            // check pixel format
            if (image.PixelFormat != PixelFormat.Format24bppRgb)
            {
                MessageBox.Show("Euclidean color filtering can be applied to RGB images only", "Message", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            EuclideanColorFilteringForm form = new EuclideanColorFilteringForm();
            form.Image = image;

            if (form.ShowDialog() == DialogResult.OK)
            {
                ApplyFilter(form.Filter);
            }
        }

        // Channels filtering
        public void channelsFilteringColorFiltersItem_Click(object sender, System.EventArgs e)
        {
            // check pixel format
            if (image.PixelFormat != PixelFormat.Format24bppRgb)
            {
                MessageBox.Show("Channels filtering can be applied to RGB images only", "Message", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            ChannelFilteringForm form = new ChannelFilteringForm();
            form.Image = image;

            if (form.ShowDialog() == DialogResult.OK)
            {
                ApplyFilter(form.Filter);
            }
        }

        // Extract red channel of image
        public void extractRedColorFiltersItem_Click(object sender, System.EventArgs e)
        {
            ApplyFilter(new ExtractChannel(RGB.R));
        }

        // Extract green channel of image
        public void extractGreenColorFiltersItem_Click(object sender, System.EventArgs e)
        {
            ApplyFilter(new ExtractChannel(RGB.G));
        }

        // Extract blue channel of image
        public void extractRedBlueFiltersItem_Click(object sender, System.EventArgs e)
        {
            ApplyFilter(new ExtractChannel(RGB.B));
        }

        // Replace red channel
        public void replaceRedColorFiltersItem_Click(object sender, System.EventArgs e)
        {
            // check pixel format
            if (image.PixelFormat != PixelFormat.Format24bppRgb)
            {
                MessageBox.Show("Channels replacement can be applied to RGB images only", "Message", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            Bitmap channelImage = host.GetImage(this, "Select an image which will replace the red channel in the current image", new Size(width, height), PixelFormat.Format8bppIndexed);

            if (channelImage != null)
                ApplyFilter(new ReplaceChannel(channelImage, RGB.R));
        }

        // Replace green channel
        public void replaceGreenColorFiltersItem_Click(object sender, System.EventArgs e)
        {
            // check pixel format
            if (image.PixelFormat != PixelFormat.Format24bppRgb)
            {
                MessageBox.Show("Channels replacement can be applied to RGB images only", "Message", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            Bitmap channelImage = host.GetImage(this, "Select an image which will replace the green channel in the current image", new Size(width, height), PixelFormat.Format8bppIndexed);

            if (channelImage != null)
                ApplyFilter(new ReplaceChannel(channelImage, RGB.G));
        }

        // Replace blue channel
        public void replaceBlueColorFiltersItem_Click(object sender, System.EventArgs e)
        {
            // check pixel format
            if (image.PixelFormat != PixelFormat.Format24bppRgb)
            {
                MessageBox.Show("Channels replacement can be applied to RGB images only", "Message", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            Bitmap channelImage = host.GetImage(this, "Select an image which will replace the blue channel in the current image", new Size(width, height), PixelFormat.Format8bppIndexed);

            if (channelImage != null)
                ApplyFilter(new ReplaceChannel(channelImage, RGB.B));
        }

        // Adjust brighness using HSL
        private void Brightness()
        {
            if (image.PixelFormat != PixelFormat.Format24bppRgb)
            {
                MessageBox.Show("Brightness filter using HSL color space is available for color images only", "Message", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            BrightnessForm form = new BrightnessForm();
            form.Image = image;

            if (form.ShowDialog() == DialogResult.OK)
            {
                ApplyFilter(form.Filter);
            }
        }

        // On "Filters->HSL Color space->Brighness" menu item click
        public void brightnessHslFiltersItem_Click(object sender, System.EventArgs e)
        {
            Brightness();
        }

        // Modify contrast
        private void Contrast()
        {
            if (image.PixelFormat != PixelFormat.Format24bppRgb)
            {
                MessageBox.Show("Contrast filter using HSL color space is available for color images only", "Message", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            ContrastForm form = new ContrastForm();
            form.Image = image;

            if (form.ShowDialog() == DialogResult.OK)
            {
                ApplyFilter(form.Filter);
            }
        }

        // On "Filters->HSL Color space->Contrast" menu item click
        public void contrastHslFiltersItem_Click(object sender, System.EventArgs e)
        {
            Contrast();
        }

        // Adjust saturation using HSL
        private void Saturation()
        {
            if (image.PixelFormat != PixelFormat.Format24bppRgb)
            {
                MessageBox.Show("Saturation filter using HSL color space is available for color images only", "Message", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            SaturationForm form = new SaturationForm();
            form.Image = image;

            if (form.ShowDialog() == DialogResult.OK)
            {
                ApplyFilter(form.Filter);
            }
        }

        // On "Filters->HSL Color space->Saturation" menu item click
        public void saturationHslFiltersItem_Click(object sender, System.EventArgs e)
        {
            Saturation();
        }

        // HSL linear correction
        public void linearHslFiltersItem_Click(object sender, System.EventArgs e)
        {
            if (image.PixelFormat != PixelFormat.Format24bppRgb)
            {
                MessageBox.Show("HSL linear correction is available for color images only", "Message", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            HSLLinearForm form = new HSLLinearForm(new ImageStatisticsHSL(image));
            form.Image = image;

            if (form.ShowDialog() == DialogResult.OK)
            {
                ApplyFilter(form.Filter);
            }
        }

        // HSL filtering
        public void filteringHslFiltersItem_Click(object sender, System.EventArgs e)
        {
            if (image.PixelFormat != PixelFormat.Format24bppRgb)
            {
                MessageBox.Show("HSL filtering is available for color images only", "Message", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            HSLFilteringForm form = new HSLFilteringForm();
            form.Image = image;

            if (form.ShowDialog() == DialogResult.OK)
            {
                ApplyFilter(form.Filter);
            }
        }

        // Hue modifier
        public void hueHslFiltersItem_Click(object sender, System.EventArgs e)
        {
            if (image.PixelFormat != PixelFormat.Format24bppRgb)
            {
                MessageBox.Show("Hue modifier is available for color images only", "Message", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            HueModifierForm form = new HueModifierForm();
            form.Image = image;

            if (form.ShowDialog() == DialogResult.OK)
            {
                ApplyFilter(form.Filter);
            }
        }

        // Linear correction of YCbCr channels
        public void linearYCbCrFiltersItem_Click(object sender, System.EventArgs e)
        {
            if (image.PixelFormat != PixelFormat.Format24bppRgb)
            {
                MessageBox.Show("YCbCr linear correction is available for color images only", "Message", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            YCbCrLinearForm form = new YCbCrLinearForm(new ImageStatisticsYCbCr(image));
            form.Image = image;

            if (form.ShowDialog() == DialogResult.OK)
            {
                ApplyFilter(form.Filter);
            }
        }

        // Filtering of YCbCr channels
        public void filteringYCbCrFiltersItem_Click(object sender, System.EventArgs e)
        {
            if (image.PixelFormat != PixelFormat.Format24bppRgb)
            {
                MessageBox.Show("YCbCr filtering is available for color images only", "Message", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            YCbCrFilteringForm form = new YCbCrFilteringForm();
            form.Image = image;

            if (form.ShowDialog() == DialogResult.OK)
            {
                ApplyFilter(form.Filter);
            }
        }

        // Extract Y channel of YCbCr color space
        public void extracYFiltersItem_Click(object sender, System.EventArgs e)
        {
            ApplyFilter(new YCbCrExtractChannel(YCbCr.YIndex));
        }

        // Extract Cb channel of YCbCr color space
        public void extracCbFiltersItem_Click(object sender, System.EventArgs e)
        {
            ApplyFilter(new YCbCrExtractChannel(YCbCr.CbIndex));
        }

        // Extract Cr channel of YCbCr color space
        public void extracCrFiltersItem_Click(object sender, System.EventArgs e)
        {
            ApplyFilter(new YCbCrExtractChannel(YCbCr.CrIndex));
        }

        // Replace Y channel of YCbCr color space
        public void replaceYFiltersItem_Click(object sender, System.EventArgs e)
        {
            // check pixel format
            if (image.PixelFormat != PixelFormat.Format24bppRgb)
            {
                MessageBox.Show("Channels replacement can be applied to RGB images only", "Message", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            Bitmap channelImage = host.GetImage(this, "Select an image which will replace the Y channel in the current image", new Size(width, height), PixelFormat.Format8bppIndexed);

            if (channelImage != null)
                ApplyFilter(new YCbCrReplaceChannel(channelImage, YCbCr.YIndex));
        }

        // Replace Cb channel of YCbCr color space
        public void replaceCbFiltersItem_Click(object sender, System.EventArgs e)
        {
            // check pixel format
            if (image.PixelFormat != PixelFormat.Format24bppRgb)
            {
                MessageBox.Show("Channels replacement can be applied to RGB images only", "Message", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            Bitmap channelImage = host.GetImage(this, "Select an image which will replace the Cb channel in the current image", new Size(width, height), PixelFormat.Format8bppIndexed);

            if (channelImage != null)
                ApplyFilter(new YCbCrReplaceChannel(channelImage, YCbCr.CbIndex));
        }

        // Replace Cr channel of YCbCr color space
        public void replaceCrFiltersItem_Click(object sender, System.EventArgs e)
        {
            // check pixel format
            if (image.PixelFormat != PixelFormat.Format24bppRgb)
            {
                MessageBox.Show("Channels replacement can be applied to RGB images only", "Message", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            Bitmap channelImage = host.GetImage(this, "Select an image which will replace the Cr channel in the current image", new Size(width, height), PixelFormat.Format8bppIndexed);

            if (channelImage != null)
                ApplyFilter(new YCbCrReplaceChannel(channelImage, YCbCr.CrIndex));
        }

        // Threshold binarization
        private void Threshold()
        {
            ThresholdForm form = new ThresholdForm();

            // set image to preview
            form.Image = image;

            if (form.ShowDialog() == DialogResult.OK)
            {
                ApplyFilter(form.Filter);
            }
        }

        // On "Filters->Binarization->Threshold" menu item click
        public void thresholdBinaryFiltersItem_Click(object sender, System.EventArgs e)
        {
            Threshold();
        }

        // Threshold binarization with carry
        public void thresholdCarryBinaryFiltersItem_Click(object sender, System.EventArgs e)
        {
            ApplyFilter(new ThresholdCarry());
        }

        // Ordered dithering
        public void orderedDitherBinaryFiltersItem_Click(object sender, System.EventArgs e)
        {
            ApplyFilter(new OrderedDithering());
        }

        // Bayer ordered dithering
        public void bayerDitherBinaryFiltersItem_Click(object sender, System.EventArgs e)
        {
            ApplyFilter(new BayerDithering());
        }

        // Binarization using Floyd-Steinverg dithering algorithm
        public void floydBinaryFiltersItem_Click(object sender, System.EventArgs e)
        {
            ApplyFilter(new FloydSteinbergDithering());
        }

        // Binarization using Burkes dithering algorithm
        public void burkesBinaryFiltersItem_Click(object sender, System.EventArgs e)
        {
            ApplyFilter(new BurkesDithering());
        }

        // Binarization using Stucki dithering algorithm
        public void stuckiBinaryFiltersItem_Click(object sender, System.EventArgs e)
        {
            ApplyFilter(new StuckiDithering());
        }

        // Binarization using Jarvis, Judice and Ninke dithering algorithm
        public void jarvisBinaryFiltersItem_Click(object sender, System.EventArgs e)
        {
            ApplyFilter(new JarvisJudiceNinkeDithering());
        }

        // Binarization using Sierra dithering algorithm
        public void sierraBinaryFiltersItem_Click(object sender, System.EventArgs e)
        {
            ApplyFilter(new SierraDithering());
        }

        // Binarization using Stevenson and Arce dithering algorithm
        public void stevensonBinaryFiltersItem_Click(object sender, System.EventArgs e)
        {
            ApplyFilter(new StevensonArceDithering());
        }

        // Threshold using Simple Image Statistics
        public void sisThresholdBinaryFiltersItem_Click(object sender, System.EventArgs e)
        {
            ApplyFilter(new SISThreshold());
        }

        // Errosion (Mathematical Morphology)
        public void erosionMorphologyFiltersItem_Click(object sender, System.EventArgs e)
        {
            ApplyFilter(new Erosion());
        }

        // Dilatation (Mathematical Morphology)
        public void dilatationMorphologyFiltersItem_Click(object sender, System.EventArgs e)
        {
            ApplyFilter(new Dilatation());
        }

        // Opening (Mathematical Morphology)
        public void openingMorphologyFiltersItem_Click(object sender, System.EventArgs e)
        {
            ApplyFilter(new Opening());
        }

        // Closing (Mathematical Morphology)
        public void closingMorphologyFiltersItem_Click(object sender, System.EventArgs e)
        {
            ApplyFilter(new Closing());
        }

        // Custom morphology operator
        private void Morphology()
        {
            if (image.PixelFormat != PixelFormat.Format8bppIndexed)
            {
                MessageBox.Show("Mathematical morpholgy filters can by applied to grayscale image only", "Message", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            MathMorphologyForm form = new MathMorphologyForm(MathMorphologyForm.FilterTypes.Simple);
            form.Image = image;

            if (form.ShowDialog() == DialogResult.OK)
            {
                ApplyFilter(form.Filter);
            }
        }

        // On "Filters->Morphology->Custom" menu item click
        public void customMorphologyFiltersItem_Click(object sender, System.EventArgs e)
        {
            Morphology();
        }

        // Hit & Miss mathematical morphology operator
        public void hitAndMissFiltersItem_Click(object sender, System.EventArgs e)
        {
            if (image.PixelFormat != PixelFormat.Format8bppIndexed)
            {
                MessageBox.Show("Hit & Miss morpholgy filters can by applied to binary image only", "Message", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            MathMorphologyForm form = new MathMorphologyForm(MathMorphologyForm.FilterTypes.HitAndMiss);
            form.Image = image;

            if (form.ShowDialog() == DialogResult.OK)
            {
                ApplyFilter(form.Filter);
            }
        }

        // Mean
        public void meanConvolutionFiltersItem_Click(object sender, System.EventArgs e)
        {
            ApplyFilter(new Mean());
        }

        // Blur
        public void blurConvolutionFiltersItem_Click(object sender, System.EventArgs e)
        {
            ApplyFilter(new Blur());
        }

        // Gaussian smoothing
        public void gaussianConvolutionFiltersItem_Click(object sender, System.EventArgs e)
        {
            GaussianForm form = new GaussianForm();

            form.Image = image;

            if (form.ShowDialog() == DialogResult.OK)
            {
                ApplyFilter(form.Filter);
            }
        }

        // Extended sharpening
        public void sharpenExConvolutionFiltersItem_Click(object sender, System.EventArgs e)
        {
            SharpenExForm form = new SharpenExForm();

            form.Image = image;

            if (form.ShowDialog() == DialogResult.OK)
            {
                ApplyFilter(form.Filter);
            }
        }

        // Sharpen
        public void sharpenConvolutionFiltersItem_Click(object sender, System.EventArgs e)
        {
            ApplyFilter(new Sharpen());
        }

        // Edges
        public void edgesConvolutionFiltersItem_Click(object sender, System.EventArgs e)
        {
            ApplyFilter(new Edges());
        }

        // Custom convolution filter
        private void Convolution()
        {
            ConvolutionForm form = new ConvolutionForm();

            form.Image = image;

            if (form.ShowDialog() == DialogResult.OK)
            {
                ApplyFilter(form.Filter);
            }
        }

        // On "Filters->Convolution & Correlation->Custom" menu item click
        public void customConvolutionFiltersItem_Click(object sender, System.EventArgs e)
        {
            Convolution();
        }

        // Merge two images
        public void mergeTwosrcFiltersItem_Click(object sender, System.EventArgs e)
        {
            Bitmap overlayImage = host.GetImage(this, "Select an image to merge with the curren image", new Size(-1, -1), image.PixelFormat);

            if (overlayImage != null)
                ApplyFilter(new Merge(overlayImage));
        }

        // Intersect
        public void intersectTwosrcFiltersItem_Click(object sender, System.EventArgs e)
        {
            Bitmap overlayImage = host.GetImage(this, "Select an image to intersect with the curren image", new Size(-1, -1), image.PixelFormat);

            if (overlayImage != null)
                ApplyFilter(new Intersect(overlayImage));
        }

        // Add
        public void addTwosrcFiltersItem_Click(object sender, System.EventArgs e)
        {
            Bitmap overlayImage = host.GetImage(this, "Select an image to add to the curren image", new Size(-1, -1), image.PixelFormat);

            if (overlayImage != null)
                ApplyFilter(new Add(overlayImage));
        }

        // Subtract
        public void subtractTwosrcFiltersItem_Click(object sender, System.EventArgs e)
        {
            Bitmap overlayImage = host.GetImage(this, "Select an image to subtract from the curren image", new Size(-1, -1), image.PixelFormat);

            if (overlayImage != null)
                ApplyFilter(new Subtract(overlayImage));
        }

        // Difference
        public void differenceTwosrcFiltersItem_Click(object sender, System.EventArgs e)
        {
            Bitmap overlayImage = host.GetImage(this, "Select an image to get difference with the curren image", new Size(width, height), image.PixelFormat);

            if (overlayImage != null)
                ApplyFilter(new Difference(overlayImage));
        }

        // Move towards
        public void moveTowardsTwosrcFiltersItem_Click(object sender, System.EventArgs e)
        {
            Bitmap overlayImage = host.GetImage(this, "Select an image to which the curren image will be moved", new Size(width, height), image.PixelFormat);

            if (overlayImage != null)
                ApplyFilter(new MoveTowards(overlayImage, 10));
        }

        // Morph an image
        public void morphTwosrcFiltersItem_Click(object sender, System.EventArgs e)
        {
            // get overlay image
            Bitmap overlayImage = host.GetImage(this, "Select an image to which the curren image will be morphed", new Size(width, height), image.PixelFormat);

            if (overlayImage != null)
            {
                // show filter setting dialog
                MorphForm form = new MorphForm(overlayImage);

                form.Image = image;

                // get filter settings
                if (form.ShowDialog() == DialogResult.OK)
                {
                    ApplyFilter(form.Filter);
                }
            }
        }

        // Homogenity edge detector
        public void homogenityEdgeFiltersItem_Click(object sender, System.EventArgs e)
        {
            ApplyFilter(new HomogenityEdgeDetector());
        }

        // Difference edge detector
        public void differenceEdgeFiltersItem_Click(object sender, System.EventArgs e)
        {
            ApplyFilter(new DifferenceEdgeDetector());
        }

        // Sobel edge detector
        public void sobelEdgeFiltersItem_Click(object sender, System.EventArgs e)
        {
            ApplyFilter(new SobelEdgeDetector());
        }

        // Canny edge detector
        public void cannyEdgeFiltersItem_Click(object sender, System.EventArgs e)
        {
            CannyDetectorForm form = new CannyDetectorForm();

            form.Image = image;

            if (form.ShowDialog() == DialogResult.OK)
            {
                ApplyFilter(form.Filter);
            }
        }

        // Adaptive smoothing
        public void adaptiveSmoothingFiltersItem_Click(object sender, System.EventArgs e)
        {
            AdaptiveSmoothForm form = new AdaptiveSmoothForm();

            form.Image = image;

            if (form.ShowDialog() == DialogResult.OK)
            {
                ApplyFilter(form.Filter);
            }
        }

        // Conservative smoothing
        public void conservativeSmoothingFiltersItem_Click(object sender, System.EventArgs e)
        {
            ApplyFilter(new ConservativeSmoothing());
        }

        // Perlin noise effects
        public void perlinNoiseFiltersItem_Click(object sender, System.EventArgs e)
        {
            PerlinNoiseForm form = new PerlinNoiseForm();

            form.Image = image;

            if (form.ShowDialog() == DialogResult.OK)
            {
                ApplyFilter(form.Filter);
            }
        }

        // Oil painting filter
        public void oilPaintingFiltersItem_Click(object sender, System.EventArgs e)
        {
            OilPaintingForm form = new OilPaintingForm();

            form.Image = image;

            if (form.ShowDialog() == DialogResult.OK)
            {
                ApplyFilter(form.Filter);
            }
        }

        // Random jitter filter
        public void jitterFiltersItem_Click(object sender, System.EventArgs e)
        {
            ApplyFilter(new Jitter(1));
        }

        // Pixellate filter
        public void pixellateFiltersItem_Click(object sender, System.EventArgs e)
        {
            PixelateForm form = new PixelateForm();

            form.Image = image;

            if (form.ShowDialog() == DialogResult.OK)
            {
                ApplyFilter(form.Filter);
            }
        }

        // Simple skeletonization
        public void simpleSkeletonizationFiltersItem_Click(object sender, System.EventArgs e)
        {
            ApplyFilter(new SimpleSkeletonization());
        }

        // Shrink the image, removing specified color from it`s borders
        public void shrinkFiltersItem_Click(object sender, System.EventArgs e)
        {
            ShrinkForm form = new ShrinkForm();

            if (form.ShowDialog() == DialogResult.OK)
            {
                ApplyFilter(form.Filter);
            }
        }

        // Conected components labeling
        public void labelingFiltersItem_Click(object sender, System.EventArgs e)
        {
            if (image.PixelFormat != PixelFormat.Format8bppIndexed)
            {
                MessageBox.Show("Connected components labeling can be applied to binary images only", "Message", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            ApplyFilter(new ConnectedComponentsLabeling());
        }

        // Extract separate blobs
        public void blobExtractorFiltersItem_Click(object sender, System.EventArgs e)
        {
            if (image.PixelFormat != PixelFormat.Format8bppIndexed)
            {
                MessageBox.Show("Blob extractor can be applied to binary images only", "Message", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            Blob[] blobs = BlobCounter.GetObjects(image);

            foreach (Blob blob in blobs)
            {
                host.NewDocument(blob.Image);
            }
        }

        // Resize the image
        private void ResizeImage()
        {
            ResizeForm form = new ResizeForm();

            form.OriginalSize = new Size(width, height);

            if (form.ShowDialog() == DialogResult.OK)
            {
                ApplyFilter(form.Filter);
            }
        }

        // On "Filters->Resize" menu item click
        public void resizeFiltersItem_Click(object sender, System.EventArgs e)
        {
            ResizeImage();
        }

        // Rotate the image
        private void RotateImage()
        {
            RotateForm form = new RotateForm();

            if (form.ShowDialog() == DialogResult.OK)
            {
                ApplyFilter(form.Filter);
            }
        }

        // On "Filters->Rotate" menu item click
        public void rotateFiltersItem_Click(object sender, System.EventArgs e)
        {
            RotateImage();
        }

        // Levels
        private void Levels()
        {
            LevelsLinearForm form = new LevelsLinearForm(new ImageStatistics(image));

            form.Image = image;

            if (form.ShowDialog() == DialogResult.OK)
            {
                ApplyFilter(form.Filter);
            }
        }

        // On "Filter->Levels" menu item click
        public void levelsFiltersItem_Click(object sender, System.EventArgs e)
        {
            Levels();
        }

        // Median filter
        public void medianFiltersItem_Click(object sender, System.EventArgs e)
        {
            ApplyFilter(new Median());
        }

        // Gamma correction
        public void gammaFiltersItem_Click(object sender, System.EventArgs e)
        {
            GammaForm form = new GammaForm();

            form.Image = image;

            if (form.ShowDialog() == DialogResult.OK)
            {
                ApplyFilter(form.Filter);
            }
        }

        // Fourier transformation
        private void ForwardFourierTransformation()
        {
            if ((!AForge.Math.Tools.IsPowerOf2(width)) ||
                (!AForge.Math.Tools.IsPowerOf2(height)))
            {
                MessageBox.Show("Fourier trasformation can be applied to an image with width and height of power of 2", "Message", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            ComplexImage cImage = ComplexImage.FromBitmap(image);

            cImage.ForwardFourierTransform();
            host.NewDocument(cImage);
        }

        // On "Filters->Fourier Transformation" click
        public void fourierFiltersItem_Click(object sender, System.EventArgs e)
        {
            ForwardFourierTransformation();
        }

        // Calculate image and screen coordinates of the point
        private void GetImageAndScreenPoints(Point point, out Point imgPoint, out Point screenPoint)
        {
            Rectangle rc = this.ClientRectangle;
            int width = (int)(this.width * zoom);
            int height = (int)(this.height * zoom);
            int x = (rc.Width < width) ? this.AutoScrollPosition.X : (rc.Width - width) / 2;
            int y = (rc.Height < height) ? this.AutoScrollPosition.Y : (rc.Height - height) / 2;

            int ix = Math.Min(Math.Max(x, point.X), x + width - 1);
            int iy = Math.Min(Math.Max(y, point.Y), y + height - 1);

            ix = (int)((ix - x) / zoom);
            iy = (int)((iy - y) / zoom);

            // image point
            imgPoint = new Point(ix, iy);
            // screen point
            screenPoint = this.PointToScreen(new Point((int)(ix * zoom + x), (int)(iy * zoom + y)));
        }

        // Normalize points so, that pt1 becomes top-left point of rectangle and pt2 becomes right-bottom
        private void NormalizePoints(ref Point pt1, ref Point pt2)
        {
            Point t1 = pt1;
            Point t2 = pt2;

            pt1.X = Math.Min(t1.X, t2.X);
            pt1.Y = Math.Min(t1.Y, t2.Y);
            pt2.X = Math.Max(t1.X, t2.X);
            pt2.Y = Math.Max(t1.Y, t2.Y);
        }

        // Draw selection rectangle
        private void DrawSelectionFrame(Graphics g)
        {
            Point sp = startW;
            Point ep = endW;

            // Normalize points
            NormalizePoints(ref sp, ref ep);
            // Draw reversible frame
            ControlPaint.DrawReversibleFrame(new Rectangle(sp.X, sp.Y, ep.X - sp.X + 1, ep.Y - sp.Y + 1), Color.White, FrameStyle.Dashed);
        }

        // Crop the image
        private void Crop()
        {
            if (!cropping)
            {
                // turn on
                cropping = true;
                this.Cursor = Cursors.Cross;

            }
            else
            {
                // turn off
                cropping = false;
                this.Cursor = Cursors.Default;
            }
        }
        #endregion 

        // On "Image->Crop" - turn on/off cropping mode
        public void cropImageItem_Click(object sender, System.EventArgs e)
        {
            Crop();
        }

        // On mouse down
        private void ImageDoc_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                // turn off cropping mode
                if (!dragging)
                {
                    cropping = false;
                    this.Cursor = Cursors.Default;
                }
            }
            else if (e.Button == MouseButtons.Left)
            {
                if (cropping)
                {
                    // start dragging
                    dragging = true;
                    // set mouse capture
                    this.Capture = true;

                    // get selection start point
                    GetImageAndScreenPoints(new Point(e.X, e.Y), out start, out startW);

                    // end point is the same as start
                    end = start;
                    endW = startW;

                    // draw frame
                    Graphics g = this.CreateGraphics();
                    DrawSelectionFrame(g);
                    g.Dispose();
                }
            }
        }

        // On mouse up
        private void ImageDoc_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (dragging)
            {
                // stop dragging and cropping
                dragging = cropping = false;
                // release capture
                this.Capture = false;
                // set default mouse pointer
                this.Cursor = Cursors.Default;

                // erase frame
                Graphics g = this.CreateGraphics();
                DrawSelectionFrame(g);
                g.Dispose();

                // normalize start and end points
                NormalizePoints(ref start, ref end);

                // crop tge image
                ApplyFilter(new Crop(new Rectangle(start.X, start.Y, end.X - start.X + 1, end.Y - start.Y + 1)));
            }
        }

        // On mouse move
        private void ImageDoc_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (dragging)
            {
                Graphics g = this.CreateGraphics();

                // erase frame
                DrawSelectionFrame(g);

                // get selection end point
                GetImageAndScreenPoints(new Point(e.X, e.Y), out end, out endW);

                // draw frame
                DrawSelectionFrame(g);

                g.Dispose();

                if (SelectionChanged != null)
                {
                    Point sp = start;
                    Point ep = end;

                    // normalize start and end points
                    NormalizePoints(ref sp, ref ep);

                    SelectionChanged(this, new SelectionEventArgs(
                        sp, new Size(ep.X - sp.X + 1, ep.Y - sp.Y + 1)));
                }
            }
            else
            {
                if (MouseImagePosition != null)
                {
                    Rectangle rc = this.ClientRectangle;
                    int width = (int)(this.width * zoom);
                    int height = (int)(this.height * zoom);
                    int x = (rc.Width < width) ? this.AutoScrollPosition.X : (rc.Width - width) / 2;
                    int y = (rc.Height < height) ? this.AutoScrollPosition.Y : (rc.Height - height) / 2;

                    if ((e.X >= x) && (e.Y >= y) &&
                        (e.X < x + width) && (e.Y < y + height))
                    {
                        // mouse is over the image
                        MouseImagePosition(this, new SelectionEventArgs(
                            new Point((int)((e.X - x) / zoom), (int)((e.Y - y) / zoom))));
                    }
                    else
                    {
                        // mouse is outside image region
                        MouseImagePosition(this, new SelectionEventArgs(new Point(-1, -1)));
                    }
                }
            }
        }

        // On mouse leave
        private void ImageDoc_MouseLeave(object sender, System.EventArgs e)
        {
            if ((!dragging) && (MouseImagePosition != null))
            {
                MouseImagePosition(this, new SelectionEventArgs(new Point(-1, -1)));
            }
        }

        private void ImageHandlerForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            ((MainForm)host).IHFList.Remove(this);
            ((MainForm)host).EnableActivatedEventDictionary[FormStyle.ImageHandleForm]--;
        }

        public void SaveFile_Click(object sender, EventArgs e)
        {
            SaveFileDialog SFD = new SaveFileDialog();
            SFD.FileName = @"MyPicture.png";
            SFD.Title = "Save As Picture";
            SFD.Filter = "Bitmap File (*.bmp)|*.bmp|JPEG File Interchange Format (*.jpg)|*.jpg|Graphics Interchange Format (*.gif)|*.gif|Portable Network Graphics PNG (*.png)|*.png|Tag Image File Format (*.tiff)|*.tiff";
            SFD.FilterIndex = 1;
            if (SFD.ShowDialog() != DialogResult.OK)
                return;
            this.Image.Save(SFD.FileName);
        }

        public void SaveAsFile_Click(object sender, EventArgs e)
        {
 
        }

        public void CopyShape_Click(object sender, EventArgs e)
        {
 
        }

        public void PastShape_Click(object sender, EventArgs e)
        {
 
        }

        public bool EnableActive = true;
        private void ImageHandlerForm_MouseEnter(object sender, EventArgs e)
        {
            ((MainForm)host).ChildFormEnableTools(FormStyle.ImageHandleForm);
            ((MainForm)host).ActiveIHF = this;
            if (EnableActive)
            {
                ((MainForm)host).ChildFormDesigner(FormStyle.ImageHandleForm);
                EnableActive = false;
            }
        }
    }

    // Selection arguments
    public class SelectionEventArgs : EventArgs
    {
        private Point location;
        private Size size;

        // Constructors
        public SelectionEventArgs(Point location)
        {
            this.location = location;
        }
        public SelectionEventArgs(Point location, Size size)
        {
            this.location = location;
            this.size = size;
        }

        // Location property
        public Point Location
        {
            get { return location; }
        }
        // Size property
        public Size Size
        {
            get { return size; }
        }
    }

    // Commands
    public enum ImageDocCommands
    {
        Clone,
        Crop,
        ZoomIn,
        ZoomOut,
        ZoomOriginal,
        FitToSize,
        Levels,
        Grayscale,
        Threshold,
        Morphology,
        Convolution,
        Resize,
        Rotate,
        Brightness,
        Contrast,
        Saturation,
        Fourier
    }
}
