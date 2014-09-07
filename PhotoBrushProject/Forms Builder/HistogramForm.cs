using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AForge.Math;
using AForge.Imaging;

namespace PhotoBrushProject
{
    /// <summary>
    /// Singleton Design Pattern Implemented
    /// </summary>
    public partial class HistogramForm : Form, IFormBuilder
    {
        private static HistogramForm instance = null;
        public static HistogramForm Instance { get { return instance == null ? instance = new HistogramForm() : instance; } }
        Rectangle IFormBuilder.Bounds { get { return this.Bounds; } set { this.Bounds = value; } }
        FormBorderStyle IFormBuilder.FormBorderStyled { get { return this.FormBorderStyle; } set { this.FormBorderStyle = value; } }
        Color IFormBuilder.BackColor { get { return this.BackColor; } set { this.BackColor = value; } }
        string IFormBuilder.Text { get { return this.Text; } set { this.Text = value; } }
        bool IFormBuilder.TopLevel { get { return this.TopLevel; } set { this.TopLevel = value; } }

        private int currentImageHash = 0;
        private static Color[] colors = new Color[] {
			Color.FromArgb(192, 0, 0),
			Color.FromArgb(0, 192, 0),
			Color.FromArgb(0, 0, 192),
			Color.FromArgb(128, 128, 128),
		};
        private ImageStatistics stat;
        private AForge.Math.Histogram activeHistogram = null;

        private HistogramForm()
        {
            InitializeComponent();
        }

        // Gather image statistics
        public void GatherStatistics(Bitmap image)
        {
            // avoid calculation in the case of the same image
            if (image != null)
            {
                if (currentImageHash == image.GetHashCode())
                    return;
                currentImageHash = image.GetHashCode();
            }

            if (image != null)
                System.Diagnostics.Debug.WriteLine("=== Gathering hostogram");

            // busy
            Capture = true;
            Cursor = Cursors.WaitCursor;

            // get statistics
            stat = (image == null) ? null : new ImageStatistics(image);

            // free
            Cursor = Cursors.Arrow;
            Capture = false;

            // clean combo
            channelCombo.Items.Clear();
            channelCombo.Enabled = false;

            if (stat != null)
            {
                if (!stat.IsGrayscale)
                {
                    // RGB picture
                    channelCombo.Items.AddRange(new object[] { "Red", "Green", "Blue" });
                    channelCombo.Enabled = true;
                }
                else
                {
                    // grayscale picture
                    channelCombo.Items.Add("Gray");
                }
                channelCombo.SelectedIndex = 0;
            }
            else
            {
                histogram.Values = null;
                meanLabel.Text = String.Empty;
                stdDevLabel.Text = String.Empty;
                medianLabel.Text = String.Empty;
                minLabel.Text = String.Empty;
                maxLabel.Text = String.Empty;
                levelLabel.Text = String.Empty;
                countLabel.Text = String.Empty;
                percentileLabel.Text = String.Empty;
            }
        }

        // selection changed in channels combo
        private void channelCombo_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (stat != null)
            {
                SwitchChannel((stat.IsGrayscale) ? 3 : channelCombo.SelectedIndex);
            }
        }

        // Switch channel
        public void SwitchChannel(int channel)
        {
            if ((channel >= 0) && (channel <= 2))
            {
                if (!stat.IsGrayscale)
                {
                    histogram.Color = colors[channel];
                    activeHistogram = (channel == 0) ? stat.Red : (channel == 1) ? stat.Green : stat.Blue;
                }
            }
            else if (channel == 3)
            {
                if (stat.IsGrayscale)
                {
                    histogram.Color = colors[3];
                    activeHistogram = stat.Gray;
                }
            }

            if (activeHistogram != null)
            {
                histogram.Values = activeHistogram.Values;

                meanLabel.Text = activeHistogram.Mean.ToString("F2");
                stdDevLabel.Text = activeHistogram.StdDev.ToString("F2");
                medianLabel.Text = activeHistogram.Median.ToString();
                minLabel.Text = activeHistogram.Min.ToString();
                maxLabel.Text = activeHistogram.Max.ToString();
            }
        }

        // Cursor position changed over the hostogram
        private void histogram_PositionChanged(object sender, HistogramEventArgs e)
        {
            int pos = e.Position;

            if (pos != -1)
            {
                levelLabel.Text = pos.ToString();
                countLabel.Text = activeHistogram.Values[pos].ToString();
                percentileLabel.Text = ((float)activeHistogram.Values[pos] * 100 / stat.PixelsCount).ToString("F2");
            }
            else
            {
                levelLabel.Text = "";
                countLabel.Text = "";
                percentileLabel.Text = "";
            }
        }

        // Selection changed in the hostogram
        private void histogram_SelectionChanged(object sender, HistogramEventArgs e)
        {
            int min = e.Min;
            int max = e.Max;
            int count = 0;

            levelLabel.Text = min.ToString() + "..." + max.ToString();

            // count pixels
            for (int i = min; i <= max; i++)
            {
                count += activeHistogram.Values[i];
            }
            countLabel.Text = count.ToString();
            percentileLabel.Text = ((float)count * 100 / stat.PixelsCount).ToString("F2");
        }

        // On "Log" check - switch mode
        private void logCheck_CheckedChanged(object sender, System.EventArgs e)
        {
            histogram.LogView = logCheck.Checked;
        }
    }
}
