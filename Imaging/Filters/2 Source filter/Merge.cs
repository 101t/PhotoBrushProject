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
	/// Merge filter - get MAX of two pixels
	/// </summary>
	public sealed class Merge : IFilter, IInPlaceFilter
	{
		private Bitmap	overlayImage;
		private Point	overlayPos = new Point( 0, 0 );

		// OverlayImage property
		public Bitmap OverlayImage
		{
			get { return overlayImage; }
			set { overlayImage = value; }
		}
		// OverlayPos property
		public Point OverlayPos
		{
			get { return overlayPos; }
			set { overlayPos = value; }
		}

		// Constructor
		public Merge( )
		{
		}
		public Merge( Bitmap overlayImage )
		{
			this.overlayImage = overlayImage;
		}
		public Merge( Bitmap overlayImage, Point position )
		{
			this.overlayImage = overlayImage;
			this.overlayPos = position;
		}


		// Apply filter
		// Output image will have the same dimension as source image
		public Bitmap Apply( Bitmap srcImg )
		{
			// source image and overlay must have the same pixel format
			if ( srcImg.PixelFormat != overlayImage.PixelFormat )
				throw new ArgumentException( );

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
			// source image and overlay must have the same pixel format
			if ( img.PixelFormat != overlayImage.PixelFormat )
				throw new ArgumentException( );

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

			// overlay position and dimension
			int ovrX = overlayPos.X;
			int ovrY = overlayPos.Y;
			int ovrW = overlayImage.Width;
			int ovrH = overlayImage.Height;

			// lock overlay image
			BitmapData ovrData = overlayImage.LockBits(
				new Rectangle( 0, 0, ovrW, ovrH ),
				ImageLockMode.ReadOnly, fmt );

			int pixelSize = ( fmt == PixelFormat.Format8bppIndexed ) ? 1 : 3;
			int stride = data.Stride;
			int offset = stride - pixelSize * width;
			int ovrStide = ovrData.Stride;
			int ovrOffset, lineSize;

			// do the job
			byte * ptr = (byte *) data.Scan0.ToPointer( );
			byte * ovr = (byte *) ovrData.Scan0.ToPointer( );

			if ( ( width == ovrW ) && ( height == ovrH ) && ( ovrX == 0 ) && ( ovrY == 0 ) )
			{
				lineSize = width * pixelSize;

				// for each line
				for ( int y = 0; y < height; y++ )
				{
					// for each pixel
					for ( int x = 0; x < lineSize; x++, ptr++, ovr++ )
					{
						if ( *ovr > *ptr )
							*ptr = *ovr;
					}
					ptr += offset;
					ovr += offset;
				}
			}
			else
			{
				// align Y
				if ( ovrY >= 0 )
				{
					ptr += stride * ovrY;
				}
				else
				{
					ovr -= ovrStide * ovrY;
					ovrH += ovrY;
					ovrY = 0;
				}

				// align X
				if ( ovrX >= 0 )
				{
					ptr += pixelSize * ovrX;
				}
				else
				{
					ovr -= pixelSize * ovrX;
					ovrW += ovrX;
					ovrX = 0;
				}

				// update overlay width and height
				ovrW = Math.Min( ovrW, width - ovrX );
				ovrH = Math.Min( ovrH, height - ovrY );

				// update offset
				ovrOffset = ovrStide - ovrW * pixelSize;
				offset = stride - ovrW * pixelSize;

				if ( ( ovrW > 0 ) && ( ovrH > 0 ) && ( ovrX < width ) && ( ovrY < height ) )
				{
					lineSize = pixelSize * ovrW;

					// for each line
					for ( int y = 0; y < ovrH; y++ )
					{
						// for each pixel
						for ( int x = 0; x < lineSize; x++, ptr++, ovr++ )
						{
							if ( *ovr > *ptr )
								*ptr = *ovr;
						}
						ptr += offset;
						ovr += ovrOffset;
					}
				}
			}
			overlayImage.UnlockBits( ovrData );
		}
	}
}
