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
	/// EuclideanColorFiltering
	/// </summary>
	public class EuclideanColorFiltering : IFilter
	{
		private short radius = 100;
		private Color center = Color.FromArgb( 255, 255, 255 );
		private Color fill = Color.FromArgb( 0, 0, 0 );
		private bool fillOutside = true;

		// Radius property
		public short Radius
		{
			get { return radius; }
			set
			{
				radius = System.Math.Max( (short) 0, System.Math.Min( (short) 450, value ) );
			}
		}
		// CenterColor property
		public Color CenterColor
		{
			get { return center; }
			set { center = value; }
		}
		// FillColor property
		public Color FillColor
		{
			get { return fill; }
			set { fill = value; }
		}
		// FillOutside property
		public bool FillOutside
		{
			get { return fillOutside; }
			set { fillOutside = value; }
		}



		// Constructor
		public EuclideanColorFiltering( )
		{
		}
		public EuclideanColorFiltering( Color center, short radius )
		{
			this.center = center;
			this.radius = radius;
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

			// create new image
			Bitmap dstImg = new Bitmap( width, height, PixelFormat.Format24bppRgb );

			// lock destination bitmap data
			BitmapData dstData = dstImg.LockBits(
				new Rectangle( 0, 0, width, height ),
				ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb );

			// copy image
			Win32.memcpy( dstData.Scan0, srcData.Scan0, srcData.Stride * height );

			// process the filter
			ProcessFilter( dstData, width, height );

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
			ProcessFilter( data, width, height );

			// unlock image
			img.UnlockBits( data );
		}


		// Process the filter
		private unsafe void ProcessFilter( BitmapData data, int width, int height )
		{
			int offset = data.Stride - width * 3;
			byte r, g, b;
			byte cR = center.R;
			byte cG = center.G;
			byte cB = center.B;
			byte fR = fill.R;
			byte fG = fill.G;
			byte fB = fill.B;

			// do the job
			byte * ptr = (byte *) data.Scan0.ToPointer();

			// for each row
			for ( int y = 0; y < height; y++ )
			{
				// for each pixel
				for ( int x = 0; x < width; x++, ptr += 3 )
				{
					r = ptr[RGB.R];
					g = ptr[RGB.G];
					b = ptr[RGB.B];

					// calculate the distance
					if ( (int) Math.Sqrt(
						Math.Pow( (int) r - (int) cR, 2 ) +
						Math.Pow( (int) g - (int) cG, 2 ) +
						Math.Pow( (int) b - (int) cB, 2 ) ) <= radius )
					{
						// inside sphere
						if ( !fillOutside )
						{
							ptr[RGB.R] = fR;
							ptr[RGB.G] = fG;
							ptr[RGB.B] = fB;
						}
					}
					else
					{
						// outside sphere
						if ( fillOutside )
						{
							ptr[RGB.R] = fR;
							ptr[RGB.G] = fG;
							ptr[RGB.B] = fB;
						}
					}
				}
				ptr += offset;
			}
		}
	}
}
