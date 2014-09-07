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

	/// <summary>
	/// Makes an images sepia
	/// </summary>
	public sealed class Sepia : IFilter, IInPlaceFilter
	{
		/*
				Main idea of the algorithm:
					1) transform to YIQ color space;
					2) modify it;
					3) transform back to RGB

					1) RGB -> YIQ
					Y = 0.299 * R + 0.587 * G + 0.114 * B
					I = 0.596 * R - 0.274 * G - 0.322 * B
					Q = 0.212 * R - 0.523 * G + 0.311 * B

					2)
					I = 51;
					Q = 0;

					3) YIQ -> RGB
					R = 1.0 * Y + 0.956 * I + 0.621 * Q
					G = 1.0 * Y - 0.272 * I - 0.647 * Q
					B = 1.0 * Y - 1.105 * I + 1.702 * Q

				After some approximations:
			*/



		// Apply filter producing new image
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
			ProcessFilter( dstData );

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
			ProcessFilter( data );

			// unlock image
			img.UnlockBits( data );
		}


		// Process the filter
		private unsafe void ProcessFilter( BitmapData data )
		{
			int width	= data.Width;
			int height	= data.Height;
			int offset	= data.Stride - width * 3;

			// do the job
			byte * ptr = (byte *) data.Scan0.ToPointer( );
			byte t;

			// for each line
			for ( int y = 0; y < height; y++ )
			{
				// for each pixel
				for ( int x = 0; x < width; x++, ptr += 3 )
				{
					t = (byte)( 0.299 * ptr[RGB.R] + 0.587 * ptr[RGB.G] + 0.114 * ptr[RGB.B] );

					// red
					ptr[RGB.R] = (byte)( ( t > 206 ) ? 255 : t + 49 );
					// green
					ptr[RGB.G] = (byte)( ( t < 14 ) ? 0 : t - 14 );
					// blue
					ptr[RGB.B] = (byte)( ( t < 56 ) ? 0 : t - 56 );
				}
				ptr += offset;
			}
		}
	}
}
