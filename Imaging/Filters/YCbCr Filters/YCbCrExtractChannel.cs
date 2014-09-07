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
	/// Extract YCbCr channel from image
	/// </summary>
	public class YCbCrExtractChannel : IFilter
	{
		private short channel = YCbCr.YIndex;

		// Channel property
		public short Channel
		{
			get { return channel; }
			set
			{
				if (
					( value != YCbCr.YIndex ) &&
					( value != YCbCr.CbIndex ) &&
					( value != YCbCr.CrIndex )
					)
				{
					throw new ArgumentException( );
				}
				channel = value;
			}
		}

		// Constructor
		public YCbCrExtractChannel( )
		{
		}
		public YCbCrExtractChannel( short channel )
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
			RGB rgb = new RGB( );
			YCbCr ycbcr = new YCbCr( );

			// do the job
			unsafe
			{
				byte * src = (byte *) srcData.Scan0.ToPointer( );
				byte * dst = (byte *) dstData.Scan0.ToPointer( );
				byte v = 0;

				// for each row
				for ( int y = 0; y < height; y++ )
				{
					// for each pixel
					for ( int x = 0; x < width; x++, src += 3, dst ++ )
					{
						rgb.Red		= src[RGB.R];
						rgb.Green	= src[RGB.G];
						rgb.Blue	= src[RGB.B];

						// convert to YCbCr
						AForge.Imaging.ColorConverter.RGB2YCbCr( rgb, ycbcr );

						switch ( channel )
						{
							case YCbCr.YIndex:
								v = (byte) ( ycbcr.Y * 255 );
								break;

							case YCbCr.CbIndex:
								v = (byte) ( ( ycbcr.Cb + 0.5 ) * 255 );
								break;

							case YCbCr.CrIndex:
								v = (byte) ( ( ycbcr.Cr + 0.5 ) * 255 );
								break;
						}

						*dst = v;
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
