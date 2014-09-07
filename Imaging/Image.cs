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


	/// <summary>
	/// Core functions
	/// </summary>
	public class Image
	{
		/// <summary>
		/// Check if the image is grayscale
		/// </summary>
		public static bool IsGrayscale( Bitmap bmp )
		{
			bool ret = false;

			// check pixel format
			if ( bmp.PixelFormat == PixelFormat.Format8bppIndexed )
			{
				ret = true;
				// check palette
				ColorPalette cp = bmp.Palette;
				Color c;
				// init palette
				for ( int i = 0; i < 256; i++ )
				{
					c = cp.Entries[i];
					if ( ( c.R != i ) || ( c.G != i ) || ( c.B != i ) )
					{
						ret = false;
						break;
					}
				}
			}
			return ret;
		}

		/// <summary>
		/// Create and initialize grayscale image
		/// </summary>
		public static Bitmap CreateGrayscaleImage( int width, int height )
		{
			// create new image
			Bitmap bmp = new Bitmap( width, height, PixelFormat.Format8bppIndexed );
			// set palette to grayscale
			SetGrayscalePalette( bmp );
			// return new image
			return bmp;
		}

		/// <summary>
		/// Set pallete of the image to grayscale
		/// </summary>
		public static void SetGrayscalePalette( Bitmap srcImg )
		{
			// check pixel format
			if ( srcImg.PixelFormat != PixelFormat.Format8bppIndexed )
				throw new ArgumentException( );

			// get palette
			ColorPalette cp = srcImg.Palette;
			// init palette
			for ( int i = 0; i < 256; i++ )
			{
				cp.Entries[i] = Color.FromArgb( i, i, i );
			}
			// set palette back
			srcImg.Palette = cp;
		}

		/// <summary>
		/// Clone image
		/// Note: It looks like Bitmap.Clone() with specified PixelFormat does not
		/// produce expected result
		/// </summary>
		public static Bitmap Clone( Bitmap src, PixelFormat format )
		{
			// copy image if pixel format is the same
			if ( src.PixelFormat == format )
				return Clone( src );

			int width	= src.Width;
			int height	= src.Height;

			// create new image with desired pixel format
			Bitmap bmp = new Bitmap( width, height, format );

			// draw source image on the new one using Graphics
			Graphics g = Graphics.FromImage( bmp );
			g.DrawImage( src, 0, 0, width, height );
			g.Dispose( );

			return bmp;
		}
		// and with unspecified PixelFormat it works strange too
		public static Bitmap Clone( Bitmap src )
		{
			// get source image size
			int width = src.Width;
			int height = src.Height;

			// lock source bitmap data
			BitmapData srcData = src.LockBits(
				new Rectangle( 0, 0, width, height ),
				ImageLockMode.ReadWrite, src.PixelFormat );

			// create new image
			Bitmap dst = new Bitmap( width, height, src.PixelFormat );

			// lock destination bitmap data
			BitmapData dstData = dst.LockBits(
				new Rectangle( 0, 0, width, height ),
				ImageLockMode.ReadWrite, dst.PixelFormat );

			Win32.memcpy( dstData.Scan0, srcData.Scan0, height * srcData.Stride );

			// unlock both images
			dst.UnlockBits( dstData );
			src.UnlockBits( srcData );

			//
			if (
				( src.PixelFormat == PixelFormat.Format1bppIndexed ) ||
				( src.PixelFormat == PixelFormat.Format4bppIndexed ) ||
				( src.PixelFormat == PixelFormat.Format8bppIndexed ) ||
				( src.PixelFormat == PixelFormat.Indexed ) )
			{
				ColorPalette srcPalette = src.Palette;
				ColorPalette dstPalette = dst.Palette;

				int n = srcPalette.Entries.Length;

				// copy pallete
				for ( int i = 0; i < n; i++ )
				{
					dstPalette.Entries[i] = srcPalette.Entries[i];
				}

				dst.Palette = dstPalette;
			}
			
			return dst;
		}


		/// <summary>
		/// Format an input image
		/// Convert it to 24 RGB or leave untouched if it's a grayscale image
		/// </summary>
		public static void FormatImage( ref Bitmap src )
		{
			if (
				( src.PixelFormat != PixelFormat.Format24bppRgb ) &&
				( IsGrayscale( src ) == false )
				)
			{
				Bitmap tmp = src;
				// convert to 24 bits per pixel
				src = Clone( tmp, PixelFormat.Format24bppRgb );

				// delete old image
				tmp.Dispose( );
			}
		}
	}
}
