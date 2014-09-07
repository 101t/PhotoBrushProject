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
	/// Complex image
	/// </summary>
	public class ComplexImage : ICloneable
	{
		private Complex[,]	data;
		private int			width;
		private int			height;
		private bool		fmode = false;

		// Width property
		public int Width
		{
			get { return width; }
		}
		// Height property
		public int Height
		{
			get { return height; }
		}

		// Clone the object
		public object Clone( )
		{
			// create new complex image
			ComplexImage	dstImg = new ComplexImage( );
			Complex[,]		data = new Complex[height, width];

			dstImg.data		= data;
			dstImg.width	= width;
			dstImg.height	= height;
			dstImg.fmode	= fmode;

			for ( int i = 0; i < height; i++ )
			{
				for ( int j = 0; j < width; j++ )
				{
					data[i, j] = this.data[i, j];
				}
			}

			return dstImg;
		}

		// Create ComplexImage from Bitmap
		public static ComplexImage FromBitmap( Bitmap srcImg )
		{
			// get source image size
			int width = srcImg.Width;
			int height = srcImg.Height;

			// check image size
			if ( 
				( !Tools.IsPowerOf2( width ) ) ||
				( !Tools.IsPowerOf2( height ) )
				)
			{
				throw new ArgumentException( );
			}

			// create new complex image
			ComplexImage	dstImg = new ComplexImage( );
			Complex[,]		data = new Complex[height, width];

			dstImg.data		= data;
			dstImg.width	= width;
			dstImg.height	= height;

			// lock source bitmap data
			BitmapData srcData = srcImg.LockBits(
				new Rectangle( 0, 0, width, height ),
				ImageLockMode.ReadOnly, srcImg.PixelFormat );

			int offset = srcData.Stride - ( ( srcImg.PixelFormat == PixelFormat.Format8bppIndexed ) ? width : width * 3 );

			// do the job
			unsafe
			{
				byte * src = (byte *) srcData.Scan0.ToPointer( );

				if ( srcImg.PixelFormat == PixelFormat.Format8bppIndexed )
				{
					// grayscale image

					// for each line
					for ( int y = 0; y < height; y++ )
					{
						// for each pixel
						for ( int x = 0; x < width; x++, src ++ )
						{
							data[y, x].Re = (float) *src / 255;
						}
						src += offset;
					}
				}
				else
				{
					// RGB image

					// for each line
					for ( int y = 0; y < height; y++ )
					{
						// for each pixel
						for ( int x = 0; x < width; x++, src += 3 )
						{
							data[y, x].Re = (float) ( 0.2125f * src[RGB.R] + 0.7154f * src[RGB.G] + 0.0721f * src[RGB.B] ) / 255;
						}
						src += offset;
					}
				}
			}
			// unlock source images
			srcImg.UnlockBits( srcData );

			return dstImg;
		}

		// Create Bitmap from the complex image
		public Bitmap ToBitmap( )
		{
			// create new image
			Bitmap dstImg = AForge.Imaging.Image.CreateGrayscaleImage( width, height );
			
			// lock destination bitmap data
			BitmapData dstData = dstImg.LockBits(
				new Rectangle( 0, 0, width, height ),
				ImageLockMode.ReadWrite, PixelFormat.Format8bppIndexed );

			int offset = dstData.Stride - width;
			float scale = ( fmode ) ? (float) Math.Sqrt( width * height ) : 1;

			// do the job
			unsafe
			{
				byte * dst = (byte *) dstData.Scan0.ToPointer( );

				for ( int y = 0; y < height; y++ )
				{
					for ( int x = 0; x < width; x++, dst ++ )
					{
						*dst = (byte) System.Math.Max( 0, System.Math.Min( 255, data[y, x].Magnitude * scale * 255 ) );
					}
					dst += offset;
				}
			}
			// unlock destination images
			dstImg.UnlockBits( dstData );

			return dstImg;
		}

		// Farward Fourier Transform
		public void ForwardFourierTransform( )
		{
			if ( !fmode )
			{
				for ( int y = 0; y < height; y++ )
				{
					for ( int x = 0; x < width; x++ )
					{
						if ( ( ( x + y ) & 0x1 ) != 0 )
						{
							data[y, x].Re *= -1;
							data[y, x].Im *= -1;
						}
					}
				}

				FourierTransform.FFT2( data, FourierDirection.Forward );
				fmode = true;
			}
		}

		// Backward Fourier Transform
		public void BackwardFourierTransform( )
		{
			if ( fmode )
			{
				FourierTransform.FFT2( data, FourierDirection.Backward );
				fmode = false;

				for ( int y = 0; y < height; y++ )
				{
					for ( int x = 0; x < width; x++ )
					{
						if ( ( ( x + y ) & 0x1 ) != 0 )
						{
							data[y, x].Re *= -1;
							data[y, x].Im *= -1;
						}
					}
				}
			}
		}

		// Perform frequency filtering
		public void FrequencyFilter( Range range )
		{
			if ( fmode )
			{
				int hw = width >> 1;
				int hh = height >> 1;
				int min = range.Min;
				int max = range.Max;

				// process all data
				for ( int i = 0; i < height; i++ )
				{
					int y = i - hh;

					for ( int j = 0; j < width; j++ )
					{
						int	x = j - hw;
						int d = (int) Math.Sqrt( x * x + y * y );

						if ( ( d > max ) || ( d < min ) )
						{
							data[i, j].Re = 0;
							data[i, j].Im = 0;
						}
					}
				}
			}
		}
	}
}
