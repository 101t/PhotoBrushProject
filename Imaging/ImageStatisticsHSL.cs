// AForge Image Processing Library
//
// Copyright © Andrew Kirillov, 2005
// andrew.kirillov@gmail.com
//

namespace AForge.Imaging
{
	using System;
	using System.Drawing;
	using System.Drawing.Imaging;
	using AForge.Math;

	/// <summary>
	/// Gather statistics about the image using HSL color space
	/// </summary>
	public class ImageStatisticsHSL
	{
		private HistogramD luminance;
		private HistogramD saturation;

		private HistogramD luminanceWithoutBlack;
		private HistogramD saturationWithoutBlack;

		private int pixels;
		private int pixelsWithoutBlack;

		// Saturation property
		public HistogramD Saturation
		{
			get { return saturation; }
		}
		// Luminance property
		public HistogramD Luminance
		{
			get { return luminance; }
		}
		// Saturation without black property
		public HistogramD SaturationWithoutBlack
		{
			get { return saturationWithoutBlack; }
		}
		// Luminance without black property
		public HistogramD LuminanceWithoutBlack
		{
			get { return luminanceWithoutBlack; }
		}
		// PixelsCount property
		public int PixelsCount
		{
			get { return pixels; }
		}
		// PixelsCount without black property
		public int PixelsCountWithoutBlack
		{
			get { return pixelsWithoutBlack; }
		}


		// Constructors
		public ImageStatisticsHSL( Bitmap image )
		{
			if ( image.PixelFormat != PixelFormat.Format24bppRgb )
				throw new ArgumentException( );

			// lock bitmap data
			BitmapData imgData = image.LockBits(
				new Rectangle( 0, 0, image.Width, image.Height ),
				ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb );

			// gather statistics
			ProcessImage( imgData, image.Width, image.Height );

			// unlock image
			image.UnlockBits( imgData );
		}
		// This constructor is used when several different statistics are
		// gathering on the same image
		// !!! 24 bit per pixel images are expected only
		public ImageStatisticsHSL( BitmapData imageData, int width, int height )
		{
			ProcessImage( imageData, width, height );
		}

		// Gather statistics
		private void ProcessImage( BitmapData imageData, int width, int height )
		{
			pixels = pixelsWithoutBlack = 0;

			int[]	s = new int[256];
			int[]	l = new int[256];
			int[]	swb = new int[256];
			int[]	lwb = new int[256];
			RGB		rgb = new RGB( );
			HSL		hsl = new HSL( );

			int offset = imageData.Stride - width * 3;

			// do the job
			unsafe
			{
				byte * p = (byte *) imageData.Scan0.ToPointer( );

				// for each line
				for ( int y = 0; y < height; y++ )
				{
					// for each pixel
					for ( int x = 0; x < width; x++, p += 3 )
					{
						rgb.Red		= p[RGB.R];
						rgb.Green	= p[RGB.G];
						rgb.Blue	= p[RGB.B];

						// convert to HSL color space
						AForge.Imaging.ColorConverter.RGB2HSL( rgb, hsl );

						s[(int) ( hsl.Saturation * 255 )]++;
						l[(int) ( hsl.Luminance * 255 )]++;
						pixels++;

						if ( ( hsl.Hue != 0.0 ) || ( hsl.Luminance != 0.0 ) || ( hsl.Saturation != 0.0 ) )
						{
							swb[(int) ( hsl.Saturation * 255 )]++;
							lwb[(int) ( hsl.Luminance * 255 )]++;
							pixelsWithoutBlack++;
						}
					}
					p += offset;
				}
			}

			// create histograms
			saturation = new HistogramD( s, new RangeD( 0, 1 ) );
			luminance = new HistogramD( l, new RangeD( 0, 1 ) );

			saturationWithoutBlack = new HistogramD( swb, new RangeD( 0, 1 ) );
			luminanceWithoutBlack = new HistogramD( lwb, new RangeD( 0, 1 ) );
		}
	}
}
