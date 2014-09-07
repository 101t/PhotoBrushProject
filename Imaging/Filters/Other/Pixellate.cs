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
	/// Pixellate filter
	/// </summary>
	public class Pixellate : IFilter, IInPlaceFilter
	{
		private int	pixelWidth = 8;
		private int pixelHeight = 8;

		// Pixel Width property
		public int PixelWidth
		{
			get { return pixelWidth; }
			set { pixelWidth = Math.Max( 2, Math.Min( 32, value ) ); }
		}
		// Pixel Height property
		public int PixelHeight
		{
			get { return pixelHeight; }
			set { pixelHeight = Math.Max( 2, Math.Min( 32, value ) ); }
		}
		// Pixel Size property
		public int PixelSize
		{
			set { pixelWidth = pixelHeight = Math.Max( 2, Math.Min( 32, value ) ); }
		}

		// Constructor
		public Pixellate( )
		{
		}
		public Pixellate( int pixelSize )
		{
			PixelSize = pixelSize;
		}
		public Pixellate( int pixelWidth, int pixelHeight )
		{
			PixelWidth	= pixelWidth;
			PixelHeight	= pixelHeight;
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

			int stride = data.Stride;
			int offset = stride - ( ( fmt == PixelFormat.Format8bppIndexed ) ? width : width * 3 );
			int i, j, k, x, t1, t2;
			int len = (int)( ( width - 1 ) / pixelWidth ) + 1;
			int rem = ( ( width - 1 ) % pixelWidth ) + 1;

			// do the job
			byte * src = (byte *) data.Scan0.ToPointer( );
			byte * dst = src;

			if ( fmt == PixelFormat.Format8bppIndexed )
			{
				// Grayscale image
				int[] tmp = new int[len];

				for ( int y1 = 0, y2 = 0; y1 < height; )
				{
					// collect pixels
					Array.Clear( tmp, 0, len );

					// calculate
					for ( i = 0; ( i < pixelHeight ) && ( y1 < height ); i++, y1++ )
					{
						// for each pixel
						for ( x = 0; x < width; x++, src ++ )
						{
							tmp[(int) ( x / pixelWidth )] += (int) *src;
						}
						src += offset;
					}

					// get average values
					t1 = i * pixelWidth;
					t2 = i * rem;

					for ( j = 0; j < len - 1; j++ )
						tmp[j] /= t1;
					tmp[j] /= t2;

					// save average value to destination image
					for ( i = 0; ( i < pixelHeight ) && ( y2 < height ); i++, y2++ )
					{
						// for each pixel
						for ( x = 0; x < width; x++, dst++ )
						{
							*dst = (byte) tmp[(int) ( x / pixelWidth )];
						}
						dst += offset;
					}
				}
			}
			else
			{
				// RGB image
				int[] tmp = new int[len * 3];

				for ( int y1 = 0, y2 = 0; y1 < height; )
				{
					// collect pixels
					Array.Clear( tmp, 0, len * 3 );

					// calculate
					for ( i = 0; ( i < pixelHeight ) && ( y1 < height ); i++, y1++ )
					{
						// for each pixel
						for ( x = 0; x < width; x++, src += 3 )
						{
							k = ( x / pixelWidth ) * 3;
							tmp[k    ] += src[RGB.R];
							tmp[k + 1] += src[RGB.G];
							tmp[k + 2] += src[RGB.B];
						}
						src += offset;
					}

					// get average values
					t1 = i * pixelWidth;
					t2 = i * rem;

					for ( j = 0, k = 0; j < len - 1; j++, k += 3 )
					{
						tmp[k    ] /= t1;
						tmp[k + 1] /= t1;
						tmp[k + 2] /= t1;
					}
					tmp[k    ] /= t2;
					tmp[k + 1] /= t2;
					tmp[k + 2] /= t2;

					// save average value to destination image
					for ( i = 0; ( i < pixelHeight ) && ( y2 < height ); i++, y2++ )
					{
						// for each pixel
						for ( x = 0; x < width; x++, dst += 3 )
						{
							k = ( x / pixelWidth ) * 3;
							dst[RGB.R] = (byte) tmp[k    ];
							dst[RGB.G] = (byte) tmp[k + 1];
							dst[RGB.B] = (byte) tmp[k + 2];
						}
						dst += offset;
					}
				}
			}
		}
	}
}
