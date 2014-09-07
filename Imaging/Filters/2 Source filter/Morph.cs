// AForge Image Processing Library
//
// Copyright © Andrew Kirillov, 2006
// andrew.kirillov@gmail.com
//

namespace AForge.Imaging.Filters
{
	using System;
	using System.Drawing;
	using System.Drawing.Imaging;

	/// <summary>
	/// Morph filter - get specifid percentage of source image and the rest from overlay image
	/// </summary>
	public class Morph : IFilter, IInPlaceFilter
	{
		private double	percent = 0.50;
		private Bitmap	overlayImage;

		// Percent property
		public double Percent
		{
			get { return percent; }
			set { percent = Math.Max( 0.0, Math.Min( 1.0, value ) ); }
		}
		// OverlayImage property
		public Bitmap OverlayImage
		{
			get { return overlayImage; }
			set { overlayImage = value; }
		}


		// Constructor
		public Morph( )
		{
		}
		public Morph( Bitmap overlayImage )
		{
			this.overlayImage = overlayImage;
		}
		public Morph( Bitmap overlayImage, double percent )
		{
			this.overlayImage = overlayImage;
			Percent = percent;
		}
		public Morph( double percent )
		{
			Percent = percent;
		}


		// Apply filter
		// Output image will have the same dimension as source image
		public Bitmap Apply( Bitmap srcImg )
		{
			// get source image size
			int width = srcImg.Width;
			int height = srcImg.Height;

			// 1) source image and overlay must have the same pixel format
			// 2) overlay dimension should not be smaller then source dimension
			if ( ( srcImg.PixelFormat != overlayImage.PixelFormat ) ||
				( width > overlayImage.Width ) || ( height > overlayImage.Height ) )
				throw new ArgumentException( );

			PixelFormat fmt = ( srcImg.PixelFormat == PixelFormat.Format8bppIndexed ) ?
				PixelFormat.Format8bppIndexed : PixelFormat.Format24bppRgb;

			// lock source bitmap data
			BitmapData srcData = srcImg.LockBits(
				new Rectangle( 0, 0, width, height ),
				ImageLockMode.ReadOnly, fmt );

			// create new image
			Bitmap dstImg = ( fmt == PixelFormat.Format8bppIndexed ) ?
				AForge.Imaging.Image.CreateGrayscaleImage( width, height ) :
				new Bitmap(width, height, fmt );

			// lock destination bitmap data
			BitmapData dstData = dstImg.LockBits(
				new Rectangle( 0, 0, width, height ),
				ImageLockMode.ReadWrite, fmt );

			// copy image
			Win32.memcpy( dstData.Scan0, srcData.Scan0, srcData.Stride * height );

			// process the filter
			ProcessFilter( dstData, fmt );

			// unlock all images
			dstImg.UnlockBits( dstData );
			srcImg.UnlockBits( srcData );

			return dstImg;
		}


		// Apply filter on current image
		public void ApplyInPlace( Bitmap img )
		{
			// get source image size
			int width = img.Width;
			int height = img.Height;

			// source image and overlay must have the same pixel format, width and height
			if ( ( img.PixelFormat != overlayImage.PixelFormat ) ||
				( width != overlayImage.Width ) || ( height != overlayImage.Height ) )
				throw new ArgumentException( );

			if (
				( img.PixelFormat != PixelFormat.Format8bppIndexed ) &&
				( img.PixelFormat != PixelFormat.Format24bppRgb )
				)
				throw new ArgumentException( );

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

			// lock overlay image
			BitmapData ovrData = overlayImage.LockBits(
				new Rectangle( 0, 0, width, height ),
				ImageLockMode.ReadOnly, fmt );

			int pixelSize	= ( fmt == PixelFormat.Format8bppIndexed ) ? 1 : 3;
			int lineSize	= width * pixelSize;
			int offsetSrc	= data.Stride - lineSize;
			int offsetOvr	= ovrData.Stride - lineSize;
			
			// percentage of overlay image
			double q = 1.0 - percent;

			// do the job
			byte * ptr = (byte *) data.Scan0.ToPointer( );
			byte * ovr = (byte *) ovrData.Scan0.ToPointer( );

			// for each line
			for ( int y = 0; y < height; y++ )
			{
				// for each pixel
				for ( int x = 0; x < lineSize; x++, ptr++, ovr++ )
				{
					*ptr = (byte) ( ( percent * *ptr ) + ( q * *ovr ) );
				}
				ptr += offsetSrc;
				ovr += offsetOvr;
			}
			overlayImage.UnlockBits( ovrData );
		}
	}
}