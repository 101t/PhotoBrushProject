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
	/// GammaCorrection filter
	/// </summary>
	public class GammaCorrection : IFilter, IInPlaceFilter
	{
		private double	gamma;
		private byte[]	table = new byte[256];

		// Gamma property
		public double Gamma
		{
			get { return gamma; }
			set
			{
				// get gamma value
				gamma = Math.Max( 0.1, Math.Min( 5.0, value ) );

				// calculate tranformation table
				double g = 1 / gamma;
				for ( int i = 0; i < 256; i++ )
				{
					table[i] = (byte) Math.Min( 255, (int) ( Math.Pow( i / 255.0, g ) * 255 + 0.5 ) );
				}
			}
		}

		// Constructor
		public GammaCorrection( )
		{
			Gamma = 2.2f;
		}
		public GammaCorrection( double gamma )
		{
			Gamma = gamma;
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

			int lineSize = width * ( ( fmt == PixelFormat.Format8bppIndexed ) ? 1 : 3 );
			int offset = data.Stride - lineSize;

			// do the job
			byte * ptr = (byte *) data.Scan0.ToPointer( );

			// gamma correction
			for ( int y = 0; y < height; y++ )
			{
				for ( int x = 0; x < lineSize; x++, ptr ++ )
				{
					// process each pixel
					*ptr = table[*ptr];
				}
				ptr += offset;
			}
		}
	}
}
