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
	/// Extract RGB channel from image
	/// </summary>
	public class ExtractChannel : IFilter
	{
		private short channel = RGB.R;

		// Channel property
		public short Channel
		{
			get { return channel; }
			set
			{
				if (
					( value != RGB.R ) &&
					( value != RGB.G ) &&
					( value != RGB.B )
					)
				{
					throw new ArgumentException( );
				}
				channel = value;
			}
		}

		// Constructor
		public ExtractChannel( )
		{
		}
		public ExtractChannel( short channel )
		{
			this.Channel = channel;
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

			// create new grayscale image
			Bitmap dstImg = AForge.Imaging.Image.CreateGrayscaleImage( width, height );

			// lock destination bitmap data
			BitmapData dstData = dstImg.LockBits(
				new Rectangle( 0, 0, width, height ),
				ImageLockMode.ReadWrite, PixelFormat.Format8bppIndexed );

			int srcOffset = srcData.Stride - width * 3;
			int dstOffset = dstData.Stride - width;

			// do the job
			unsafe
			{
				byte * src = (byte *) srcData.Scan0.ToPointer( );
				byte * dst = (byte *) dstData.Scan0.ToPointer( );

				for ( int y = 0; y < height; y++ )
				{
					for ( int x = 0; x < width; x++, src += 3, dst ++ )
					{
						*dst = src[channel];
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
