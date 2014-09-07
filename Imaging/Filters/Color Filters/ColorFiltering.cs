// AForge Image Processing Library
//
// Copyright © Andrew Kirillov, 2005
// andrew.kirillov@gmail.com
//

namespace AForge.Imaging.Filters
{
	using System;
	using System.Drawing;
	using System.Drawing.Imaging;
	using AForge.Math;
	
	/// <summary>
	/// ColorFiltering - filters pixels inside or outside specified color range
	/// </summary>
	public class ColorFiltering : IFilter
	{
		private Range red	= new Range( 0, 255 );
		private Range green	= new Range( 0, 255 );
		private Range blue	= new Range( 0, 255 );

		private byte fillR = 0;
		private byte fillG = 0;
		private byte fillB = 0;
		private bool fillOutsideRange = true;

		#region Public properties

		// Red property
		public Range Red
		{
			get { return red; }
			set { red = value; }
		}
		// Green property
		public Range Green
		{
			get { return green; }
			set { green = value; }
		}
		// Blue property
		public Range Blue
		{
			get { return blue; }
			set { blue = value; }
		}
		// Fill color property
		public RGB FillColor
		{
			get { return new RGB( fillR, fillG, fillB ); }
			set
			{
				fillR = value.Red;
				fillG = value.Green;
				fillB = value.Blue;
			}
		}
		// FillOutsideRange property
		public bool FillOutsideRange
		{
			get { return fillOutsideRange; }
			set { fillOutsideRange = value; }
		}

		#endregion


		// Constructor
		public ColorFiltering( )
		{
		}
		public ColorFiltering( Range red, Range green, Range blue )
		{
			this.red	= red;
			this.green	= green;
			this.blue	= blue;
		}

		// Apply filter
		public Bitmap Apply( Bitmap srcImg )
		{
			if ( srcImg.PixelFormat != PixelFormat.Format24bppRgb )
				throw new ArgumentException( );

			// get source image size
			int width = srcImg.Width;
			int height = srcImg.Height;
			
			// lock source bitmap data
			BitmapData srcData = srcImg.LockBits(
				new Rectangle( 0, 0, width, height ),
				ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb );

			// create new image
			Bitmap dstImg = new Bitmap( width, height, PixelFormat.Format24bppRgb );

			// lock destination bitmap data
			BitmapData dstData = dstImg.LockBits(
				new Rectangle( 0, 0, width, height ),
				ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb );

			// copy image
			Win32.memcpy( dstData.Scan0, srcData.Scan0, srcData.Stride * height );

			// process the filter
			ProcessFilter( dstData, width, height );

			// unlock both images
			dstImg.UnlockBits( dstData );
			srcImg.UnlockBits( srcData );

			return dstImg;
		}


		// Apply filter on current image
		public void ApplyInPlace( Bitmap img )
		{
			if ( img.PixelFormat != PixelFormat.Format24bppRgb )
				throw new ArgumentException( );

			// get source image size
			int width = img.Width;
			int height = img.Height;

			// lock source bitmap data
			BitmapData data = img.LockBits(
				new Rectangle( 0, 0, width, height ),
				ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb );

			// process the filter
			ProcessFilter( data, width, height );

			// unlock image
			img.UnlockBits( data );
		}


		// Process the filter
		private unsafe void ProcessFilter( BitmapData data, int width, int height )
		{
			int offset = data.Stride - width * 3;

			// do the job
			byte * ptr = (byte *) data.Scan0.ToPointer( );
			byte r, g, b;

			// for each row
			for ( int y = 0; y < height; y++ )
			{
				// for each pixel
				for ( int x = 0; x < width; x++, ptr += 3 )
				{
					r = ptr[RGB.R];
					g = ptr[RGB.G];
					b = ptr[RGB.B];

					// check pixel
					if (
						( r >= red.Min ) && ( r <= red.Max ) &&
						( g >= green.Min ) && ( g <= green.Max ) &&
						( b >= blue.Min ) && ( b <= blue.Max)
						)
					{
						if ( !fillOutsideRange )
						{
							ptr[RGB.R] = fillR;
							ptr[RGB.G] = fillG;
							ptr[RGB.B] = fillB;
						}
					}
					else
					{
						if ( fillOutsideRange )
						{
							ptr[RGB.R] = fillR;
							ptr[RGB.G] = fillG;
							ptr[RGB.B] = fillB;
						}
					}
				}
				ptr += offset;
			}
		}
	}
}
