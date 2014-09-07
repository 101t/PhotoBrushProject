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
	using AForge.Math;

	/// <summary>
	/// YCbCrFiltering - filters pixels inside or outside specified YCbCr range
	/// </summary>
	public class YCbCrFiltering : IFilter, IInPlaceFilter
	{
		private RangeD yRange  = new RangeD(  0.0, 1.0 );
		private RangeD cbRange = new RangeD( -0.5, 0.5 );
		private RangeD crRange = new RangeD( -0.5, 0.5 );

		private double fillY  = 0.0;
		private double fillCb = 0.0;
		private double fillCr = 0.0;
		private bool fillOutsideRange = true;

		private bool updateY  = true;
		private bool updateCb = true;
		private bool updateCr = true;

		#region Public properties

		// Y property
		public RangeD Y
		{
			get { return yRange; }
			set { yRange = value; }
		}
		// Cb property
		public RangeD Cb
		{
			get { return cbRange; }
			set { cbRange = value; }
		}
		// Cr property
		public RangeD Cr
		{
			get { return crRange; }
			set { crRange = value; }
		}
		// Fill color property
		public YCbCr FillColor
		{
			get { return new YCbCr( fillY, fillCb, fillCr ); }
			set
			{
				fillY  = value.Y;
				fillCb = value.Cb;
				fillCr = value.Cr;
			}
		}
		// FillOutsideRange property
		public bool FillOutsideRange
		{
			get { return fillOutsideRange; }
			set { fillOutsideRange = value; }
		}
		// Update Y value property
		public bool UpdateY
		{
			get { return updateY; }
			set { updateY = value; }
		}
		// Update Cb value property
		public bool UpdateCb
		{
			get { return updateCb; }
			set { updateCb = value; }
		}
		// Update Cr value property
		public bool UpdateCr
		{
			get { return updateCr; }
			set { updateCr = value; }
		}

		#endregion


		// Constructor
		public YCbCrFiltering( )
		{
		}
		public YCbCrFiltering( RangeD yRange, RangeD cbRange, RangeD crRange )
		{
			this.yRange  = yRange;
			this.cbRange = cbRange;
			this.crRange = crRange;
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
			ProcessFilter( dstData );

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
			ProcessFilter( data );

			// unlock image
			img.UnlockBits( data );
		}


		// Process the filter
		private unsafe void ProcessFilter( BitmapData data )
		{
			int width	= data.Width;
			int height	= data.Height;

			RGB		rgb = new RGB( );
			YCbCr	ycbcr = new YCbCr( );
			int		offset = data.Stride - width * 3;
			bool	updated;

			// do the job
			byte * ptr = (byte *) data.Scan0.ToPointer( );

			// for each row
			for ( int y = 0; y < height; y++ )
			{
				// for each pixel
				for ( int x = 0; x < width; x++, ptr += 3 )
				{
					updated		= false;
					rgb.Red		= ptr[RGB.R];
					rgb.Green	= ptr[RGB.G];
					rgb.Blue	= ptr[RGB.B];

					// convert to YCbCr
					AForge.Imaging.ColorConverter.RGB2YCbCr( rgb, ycbcr );

					// check YCbCr values
					if (
						( ycbcr.Y  >= yRange.Min  ) && ( ycbcr.Y  <= yRange.Max ) &&
						( ycbcr.Cb >= cbRange.Min ) && ( ycbcr.Cb <= cbRange.Max ) &&
						( ycbcr.Cr >= crRange.Min ) && ( ycbcr.Cr <= crRange.Max )
						)
					{
						if ( !fillOutsideRange )
						{
							if ( updateY  ) ycbcr.Y  = fillY;
							if ( updateCb ) ycbcr.Cb = fillCb;
							if ( updateCr ) ycbcr.Cr = fillCr;

							updated = true;
						}
					}
					else
					{
						if ( fillOutsideRange )
						{
							if ( updateY  ) ycbcr.Y  = fillY;
							if ( updateCb ) ycbcr.Cb = fillCb;
							if ( updateCr ) ycbcr.Cr = fillCr;

							updated = true;
						}
					}

					if ( updated )
					{
						// convert back to RGB
						AForge.Imaging.ColorConverter.YCbCr2RGB( ycbcr, rgb );

						ptr[RGB.R] = rgb.Red;
						ptr[RGB.G] = rgb.Green;
						ptr[RGB.B] = rgb.Blue;
					}
				}
				ptr += offset;
			}
		}
	}
}
