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
	/// ChannelFiltering
	/// </summary>
	public class ChannelFiltering : IFilter
	{
		private Range red	= new Range( 0, 255 );
		private Range green	= new Range( 0, 255 );
		private Range blue	= new Range( 0, 255 );

		private byte fillR = 0;
		private byte fillG = 0;
		private byte fillB = 0;

		private bool redFillOutsideRange	= true;
		private bool greenFillOutsideRange	= true;
		private bool blueFillOutsideRange	= true;

		private byte[]	mapRed		= new byte[256];
		private byte[]	mapGreen	= new byte[256];
		private byte[]	mapBlue		= new byte[256];

		#region Public properties

		// Red property
		public Range Red
		{
			get { return red; }
			set
			{
				red = value;
				CalculateMap( red, fillR, redFillOutsideRange, mapRed );
			}
		}
		// FillRed property
		public byte FillRed
		{
			get { return fillR; }
			set
			{
				fillR = value;
				CalculateMap( red, fillR, redFillOutsideRange, mapRed );
			}
		}
		// Green property
		public Range Green
		{
			get { return green; }
			set
			{
				green = value;
				CalculateMap( green, fillG, greenFillOutsideRange, mapGreen );
			}
		}
		// FillGreen property
		public byte FillGreen
		{
			get { return fillG; }
			set
			{
				fillG = value;
				CalculateMap( green, fillG, greenFillOutsideRange, mapGreen );
			}
		}
		// Blue property
		public Range Blue
		{
			get { return blue; }
			set
			{
				blue = value;
				CalculateMap( blue, fillB, blueFillOutsideRange, mapBlue );
			}
		}
		// FillBlue property
		public byte FillBlue
		{
			get { return fillB; }
			set
			{
				fillB = value;
				CalculateMap( blue, fillB, blueFillOutsideRange, mapBlue );
			}
		}
		// RedFillOutsideRange property
		public bool RedFillOutsideRange
		{
			get { return redFillOutsideRange; }
			set
			{
				redFillOutsideRange = value;
				CalculateMap( red, fillR, redFillOutsideRange, mapRed );
			}
		}
		// GreenFillOutsideRange property
		public bool GreenFillOutsideRange
		{
			get { return greenFillOutsideRange; }
			set
			{
				greenFillOutsideRange = value;
				CalculateMap( green, fillG, greenFillOutsideRange, mapGreen );
			}
		}
		// BlueFillOutsideRange property
		public bool BlueFillOutsideRange
		{
			get { return blueFillOutsideRange; }
			set
			{
				blueFillOutsideRange = value;
				CalculateMap( blue, fillB, blueFillOutsideRange, mapBlue );
			}
		}

		#endregion


		// Constructor
		public ChannelFiltering( )
		{
			CalculateMap( red, fillR, redFillOutsideRange, mapRed );
			CalculateMap( green, fillG, greenFillOutsideRange, mapGreen );
			CalculateMap( blue, fillB, blueFillOutsideRange, mapBlue );
		}
		public ChannelFiltering( Range red, Range green, Range blue )
		{
			Red		= red;
			Green	= green;
			Blue	= blue;
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

			// for each row
			for ( int y = 0; y < height; y++ )
			{
				// for each pixel
				for ( int x = 0; x < width; x++, ptr += 3 )
				{
					// red
					ptr[RGB.R] =  mapRed[ptr[RGB.R]];
					// green
					ptr[RGB.G] =  mapGreen[ptr[RGB.G]];
					// blue
					ptr[RGB.B] =  mapBlue[ptr[RGB.B]];
				}
				ptr += offset;
			}
		}


		// Calculate map
		private void CalculateMap( Range range, byte fill, bool fillOutsideRange, byte[] map )
		{
			for (int i = 0; i < 256; i++)
			{
				if ( ( i >= range.Min ) && ( i <= range.Max ) )
				{
					map[i] = ( fillOutsideRange ) ? (byte) i : fill;
				}
				else
				{
					map[i] = ( fillOutsideRange ) ? fill : (byte) i;
				}
			}
		}
	}
}
