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
	/// Linear correction of RGB channels
	/// </summary>
	public class LevelsLinear : IFilter, IInPlaceFilter
	{
		private Range	inRed	= new Range(0, 255);
		private Range	inGreen	= new Range(0, 255);
		private Range	inBlue	= new Range(0, 255);

		private Range	outRed	= new Range(0, 255);
		private Range	outGreen= new Range(0, 255);
		private Range	outBlue	= new Range(0, 255);

		private byte[]	mapRed		= new byte[256];
		private byte[]	mapGreen	= new byte[256];
		private byte[]	mapBlue		= new byte[256];

		#region Public Propertis

		// Input properties
		public Range InRed
		{
			get { return inRed; }
			set
			{
				inRed = value;
				CalculateMap( inRed, outRed, mapRed );
			}
		}
		public Range InGreen
		{
			get { return inGreen; }
			set
			{
				inGreen = value;
				CalculateMap( inGreen, outGreen, mapGreen );
			}
		}
		public Range InBlue
		{
			get { return inBlue; }
			set
			{
				inBlue = value;
				CalculateMap( inBlue, outBlue, mapBlue );
			}
		}
		public Range InGray
		{
			get { return inGreen; }
			set
			{
				inGreen = value;
				CalculateMap( inGreen, outGreen, mapGreen );
			}
		}
		public Range Input
		{
			set
			{
				inRed = inGreen = inBlue = value;
				CalculateMap( inRed, outRed, mapRed );
				CalculateMap( inGreen, outGreen, mapGreen );
				CalculateMap( inBlue, outBlue, mapBlue );
			}
		}

		// Output properties
		public Range OutRed
		{
			get { return outRed; }
			set
			{
				outRed = value;
				CalculateMap( inRed, outRed, mapRed );
			}
		}
		public Range OutGreen
		{
			get { return outGreen; }
			set
			{
				outGreen = value;
				CalculateMap( inGreen, outGreen, mapGreen );
			}
		}
		public Range OutBlue
		{
			get { return outBlue; }
			set
			{
				outBlue = value;
				CalculateMap( inBlue, outBlue, mapBlue );
			}
		}
		public Range OutGray
		{
			get { return outGreen; }
			set
			{
				outGreen = value;
				CalculateMap( inGreen, outGreen, mapGreen );
			}
		}
		public Range Output
		{
			set
			{
				outRed = outGreen = outBlue = value;
				CalculateMap( inRed, outRed, mapRed );
				CalculateMap( inGreen, outGreen, mapGreen );
				CalculateMap( inBlue, outBlue, mapBlue );
			}
		}

		#endregion


		// Constructors
		public LevelsLinear( )
		{
			CalculateMap( inRed, outRed, mapRed );
			CalculateMap( inGreen, outGreen, mapGreen );
			CalculateMap( inBlue, outBlue, mapBlue );
		}

		// Apply filter
		public Bitmap Apply( Bitmap srcImg )
		{
			// get source image size
			int width = srcImg.Width;
			int height = srcImg.Height;
			
			PixelFormat fmt = ( srcImg.PixelFormat == PixelFormat.Format8bppIndexed ) ?
				PixelFormat.Format8bppIndexed : PixelFormat.Format24bppRgb;

			// lock source bitmap data
			BitmapData srcData = srcImg.LockBits(
				new Rectangle( 0, 0, width, height ),
				ImageLockMode.ReadOnly, fmt );

			// create new image
			Bitmap dstImg = ( fmt == PixelFormat.Format8bppIndexed ) ?
				AForge.Imaging.Image.CreateGrayscaleImage( width, height ) :
				new Bitmap( width, height, fmt );

			// lock destination bitmap data
			BitmapData dstData = dstImg.LockBits(
				new Rectangle( 0, 0, width, height ),
				ImageLockMode.ReadWrite, fmt );

			// copy image
			Win32.memcpy( dstData.Scan0, srcData.Scan0, srcData.Stride * height );

			// process the filter
			ProcessFilter( dstData, fmt );

			// unlock both images
			dstImg.UnlockBits( dstData );
			srcImg.UnlockBits( srcData );

			return dstImg;
		}


		// Apply filter on current image
		public void ApplyInPlace( Bitmap img )
		{
			if (
				( img.PixelFormat != PixelFormat.Format8bppIndexed ) &&
				( img.PixelFormat != PixelFormat.Format24bppRgb )
				)
				throw new ArgumentException( );

			// get source image size
			int width = img.Width;
			int height = img.Height;

			// lock source bitmap data
			BitmapData data = img.LockBits(
				new Rectangle( 0, 0, width, height ),
				ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb );

			// process the filter
			ProcessFilter( data, img.PixelFormat );

			// unlock image
			img.UnlockBits( data );
		}


		// Process the filter
		private unsafe void ProcessFilter( BitmapData data, PixelFormat fmt )
		{
			int width	= data.Width;
			int height	= data.Height;
			int offset = data.Stride - ( ( fmt == PixelFormat.Format8bppIndexed ) ? width : width * 3 );

			// do the job
			byte * ptr = (byte *) data.Scan0.ToPointer( );

			if ( fmt == PixelFormat.Format8bppIndexed )
			{
				// grayscale image
				for ( int y = 0; y < height; y++ )
				{
					for ( int x = 0; x < width; x++, ptr ++ )
					{
						// gray
						*ptr = mapGreen[*ptr];
					}
					ptr += offset;
				}
			}
			else
			{
				// RGB image
				for ( int y = 0; y < height; y++ )
				{
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
		}


		// Calculate map
		private void CalculateMap( Range inRange, Range outRange, byte[] map )
		{
			double	k = 0, b = 0;

			if ( inRange.Max != inRange.Min )
			{
				k = (double)( outRange.Max - outRange.Min ) / (double)( inRange.Max - inRange.Min );
				b = (double)( outRange.Min ) - k * inRange.Min;
			}

			for ( int i = 0; i < 256; i++ )
			{
				byte v = (byte) i;

				if ( v >= inRange.Max )
					v = (byte) outRange.Max;
				else if ( v <= inRange.Min )
					v = (byte) outRange.Min;
				else
					v = (byte) ( k * v + b );

				map[i] = v;
			}
		}
	}
}
