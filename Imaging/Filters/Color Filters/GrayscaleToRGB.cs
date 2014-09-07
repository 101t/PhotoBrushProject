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
	/// Convert grayscale image to RGB
	/// </summary>
	public sealed class GrayscaleToRGB : IFilter
	{
		// Apply filter
		public Bitmap Apply( Bitmap srcImg )
		{
			if ( srcImg.PixelFormat != PixelFormat.Format8bppIndexed )
				throw new ArgumentException( );

			// get source image size
			int width = srcImg.Width;
			int height = srcImg.Height;

			// lock source bitmap data
			BitmapData srcData = srcImg.LockBits(
				new Rectangle( 0, 0, width, height ),
				ImageLockMode.ReadOnly, PixelFormat.Format8bppIndexed );

			// create new RGB image
			Bitmap dstImg = new Bitmap( width, height, PixelFormat.Format24bppRgb );

			// lock destination bitmap data
			BitmapData dstData = dstImg.LockBits(
				new Rectangle( 0, 0, width, height ),
				ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb );

			int srcOffset = srcData.Stride - width;
			int dstOffset = dstData.Stride - width * 3;

			// do the job
			unsafe
			{
				byte * src = (byte *) srcData.Scan0.ToPointer( );
				byte * dst = (byte *) dstData.Scan0.ToPointer( );

				// for each row
				for ( int y = 0; y < height; y++ )
				{
					// for each pixel
					for ( int x = 0; x < width; x++, src++, dst += 3 )
					{
						dst[RGB.R] = dst[RGB.G] = dst[RGB.B] = *src;
					}
					src += srcOffset;
					dst += dstOffset;
				}
			}
			// unlock both images
			dstImg.UnlockBits( dstData );
			srcImg.UnlockBits( srcData );

			return dstImg;
		}
	}
}
