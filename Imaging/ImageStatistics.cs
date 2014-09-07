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
	/// Gather statistics about the image
	/// </summary>
	public class ImageStatistics
	{
		private Histogram red;
		private Histogram green;
		private Histogram blue;
		private Histogram gray;

		private Histogram redWithoutBlack;
		private Histogram greenWithoutBlack;
		private Histogram blueWithoutBlack;
		private Histogram grayWithoutBlack;

		private int pixels;
		private int pixelsWithoutBlack;
		private bool grayscale;

		// Red property
		public Histogram Red
		{
			get { return red; }
		}
		// Green property
		public Histogram Green
		{
			get { return green; }
		}
		// Blue property
		public Histogram Blue
		{
			get { return blue; }
		}
		// Gray property
		public Histogram Gray
		{
			get { return gray; }
		}
		// PixelsCount property
		public int PixelsCount
		{
			get { return pixels; }
		}
		// Red withoit black property
		public Histogram RedWithoutBlack
		{
			get { return redWithoutBlack; }
		}
		// Green without black property
		public Histogram GreenWithoutBlack
		{
			get { return greenWithoutBlack; }
		}
		// Blue without black property
		public Histogram BlueWithoutBlack
		{
			get { return blueWithoutBlack; }
		}
		// Gray without black property
		public Histogram GrayWithoutBlack
		{
			get { return grayWithoutBlack; }
		}
		// PixelsCount without black property
		public int PixelsCountWithoutBlack
		{
			get { return pixelsWithoutBlack; }
		}
		// IsGrayscale property
		public bool IsGrayscale
		{
			get { return grayscale; }
		}


		// Constructors
		public ImageStatistics( Bitmap image )
		{
			PixelFormat fmt = ( image.PixelFormat == PixelFormat.Format8bppIndexed ) ?
				PixelFormat.Format8bppIndexed : PixelFormat.Format24bppRgb;

			// lock bitmap data
			BitmapData imgData = image.LockBits(
				new Rectangle( 0, 0, image.Width, image.Height ),
				ImageLockMode.ReadOnly, fmt );

			// gather statistics
			ProcessImage( imgData, image.Width, image.Height, fmt );

			// unlock image
			image.UnlockBits( imgData );
		}
		// This constructor is used when several different statistics are
		// gathering on the same image
		// !!! 24 bit per pixel or 8 bit indexed (grayscale) images are expected only
		public ImageStatistics( BitmapData imageData, int width, int height, PixelFormat pixelFormat )
		{
			ProcessImage( imageData, width, height, pixelFormat );
		}


		// Gather statistics
		private void ProcessImage( BitmapData imageData, int width, int height, PixelFormat pixelFormat )
		{
			pixels = pixelsWithoutBlack = 0;

			if ( grayscale = ( pixelFormat == PixelFormat.Format8bppIndexed ) )
			{
				// alloc arrays
				int[] g = new int[256];
				int[] gwb = new int[256];

				byte gv;

				int offset = imageData.Stride - width;

				// do the job
				unsafe
				{
					byte * p = (byte *) imageData.Scan0.ToPointer( );

					// for each pixel
					for ( int y = 0; y < height; y++ )
					{
						// for each pixel
						for ( int x = 0; x < width; x++, p++ )
						{
							// get pixel value
							gv = *p;

							g[gv]++;
							pixels++;

							if ( gv != 0 )
							{
								gwb[gv]++;
								pixelsWithoutBlack++;
							}
						}
						p += offset;
					}
				}

				// create historgram for gray level
				gray = new Histogram( g );
				grayWithoutBlack = new Histogram( gwb );
			}
			else
			{
				// alloc arrays
				int[]	r = new int[256];
				int[]	g = new int[256];
				int[]	b = new int[256];

				int[]	rwb = new int[256];
				int[]	gwb = new int[256];
				int[]	bwb = new int[256];

				byte	rv, gv, bv;

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
							// get pixel values
							rv = p[RGB.R]; 
							gv = p[RGB.G];
							bv = p[RGB.B];

							r[rv]++;
							g[gv]++;
							b[bv]++;
							pixels++;

							if ( ( rv != 0 ) || ( gv != 0 ) || ( bv != 0 ) )
							{
								rwb[rv]++;
								gwb[gv]++;
								bwb[bv]++;
								pixelsWithoutBlack++;
							}
						}
						p += offset;
					}
				}

				// create histograms
				red		= new Histogram( r );
				green	= new Histogram( g );
				blue	= new Histogram( b );

				redWithoutBlack		= new Histogram( rwb );
				greenWithoutBlack	= new Histogram( gwb );
				blueWithoutBlack	= new Histogram( bwb );
			}
		}
	}
}
