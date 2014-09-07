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
	/// Gather statistics about the image using YCbCr color space
	/// </summary>
	public class ImageStatisticsYCbCr
	{
		private HistogramD yHistogram;
		private HistogramD cbHistogram;
		private HistogramD crHistogram;

		private HistogramD yHistogramWithoutBlack;
		private HistogramD cbHistogramWithoutBlack;
		private HistogramD crHistogramWithoutBlack;

		private int pixels;
		private int pixelsWithoutBlack;

		// Y property
		public HistogramD Y
		{
			get { return yHistogram; }
		}
		// Cb property
		public HistogramD Cb
		{
			get { return cbHistogram; }
		}
		// Cr property
		public HistogramD Cr
		{
			get { return crHistogram; }
		}
		// Y without black property
		public HistogramD YWithoutBlack
		{
			get { return yHistogramWithoutBlack; }
		}
		// Cb without black property
		public HistogramD CbWithoutBlack
		{
			get { return cbHistogramWithoutBlack; }
		}
		// Cr without black property
		public HistogramD CrWithoutBlack
		{
			get { return crHistogramWithoutBlack; }
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
		public ImageStatisticsYCbCr( Bitmap image )
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
		public ImageStatisticsYCbCr( BitmapData imageData, int width, int height )
		{
			ProcessImage( imageData, width, height );
		}


		// Gather statistics
		private void ProcessImage( BitmapData imageData, int width, int height )
		{
			pixels = pixelsWithoutBlack = 0;

			int[]	yhisto  = new int[256];
			int[]	cbhisto = new int[256];
			int[]	crhisto = new int[256];

			int[]	yhistoWB	= new int[256];
			int[]	cbhistoWB	= new int[256];
			int[]	crhistoWB	= new int[256];

			RGB		rgb = new RGB( );
			YCbCr	ycbcr = new YCbCr( );

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

						// convert to YCbCr color space
						AForge.Imaging.ColorConverter.RGB2YCbCr( rgb, ycbcr );

						yhisto [(int) ( ycbcr.Y * 255 )]++;
						cbhisto[(int) ( ( ycbcr.Cb + 0.5 ) * 255 )]++;
						crhisto[(int) ( ( ycbcr.Cr + 0.5 ) * 255 )]++;

						pixels++;

						if ( ( ycbcr.Y != 0.0 ) || ( ycbcr.Cb != 0.0 ) || ( ycbcr.Cr != 0.0 ) )
						{
							yhistoWB [(int) ( ycbcr.Y * 255 )]++;
							cbhistoWB[(int) ( ( ycbcr.Cb + 0.5 ) * 255 )]++;
							crhistoWB[(int) ( ( ycbcr.Cr + 0.5 ) * 255 )]++;

							pixelsWithoutBlack++;
						}
					}
					p += offset;
				}
			}

			// create histograms
			yHistogram  = new HistogramD( yhisto , new RangeD(  0.0, 1.0 ) );
			cbHistogram = new HistogramD( cbhisto, new RangeD( -0.5, 0.5 ) );
			crHistogram = new HistogramD( crhisto, new RangeD( -0.5, 0.5 ) );

			yHistogramWithoutBlack  = new HistogramD( yhistoWB , new RangeD(  0.0, 1.0 ) );
			cbHistogramWithoutBlack = new HistogramD( cbhistoWB, new RangeD( -0.5, 0.5 ) );
			crHistogramWithoutBlack = new HistogramD( crhistoWB, new RangeD( -0.5, 0.5 ) );
		}
	}
}
