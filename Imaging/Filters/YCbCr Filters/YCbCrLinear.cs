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
	/// Linear correction of YCbCr channels
	/// </summary>
	public class YCbCrLinear : IFilter, IInPlaceFilter
	{
		private RangeD	inY = new RangeD( 0.0, 1.0 );
		private RangeD	inCb = new RangeD( -0.5, 0.5 );
		private RangeD	inCr = new RangeD( -0.5, 0.5 );

		private RangeD	outY = new RangeD( 0.0, 1.0 );
		private RangeD	outCb = new RangeD( -0.5, 0.5 );
		private RangeD	outCr = new RangeD( -0.5, 0.5 );

		#region Public Propertis

		// InY property
		public RangeD InY
		{
			get { return inY; }
			set { inY = value; }
		}
		// InCb property
		public RangeD InCb
		{
			get { return inCb; }
			set { inCb = value; }
		}
		// InCr property
		public RangeD InCr
		{
			get { return inCr; }
			set { inCr = value; }
		}

		// OutY property
		public RangeD OutY
		{
			get { return outY; }
			set { outY = value; }
		}
		// OutCb property
		public RangeD OutCb
		{
			get { return outCb; }
			set { outCb = value; }
		}
		// OutCr property
		public RangeD OutCr
		{
			get { return outCr; }
			set { outCr = value; }
		}

		#endregion


		// Constructors
		public YCbCrLinear( )
		{
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
			double	ky  = 0, by  = 0;
			double	kcb = 0, bcb = 0;
			double	kcr = 0, bcr = 0;

			// Y line params
			if ( inY.Max != inY.Min )
			{
				ky = ( outY.Max - outY.Min ) / ( inY.Max - inY.Min );
				by = outY.Min - ky * inY.Min;
			}
			// Cb line params
			if ( inCb.Max != inCb.Min )
			{
				kcb = ( outCb.Max - outCb.Min ) / ( inCb.Max - inCb.Min );
				bcb = outCb.Min - kcb * inCb.Min;
			}
			// Cr line params
			if ( inCr.Max != inCr.Min )
			{
				kcr = ( outCr.Max - outCr.Min ) / ( inCr.Max - inCr.Min );
				bcr = outCr.Min - kcr * inCr.Min;
			}

			// do the job
			byte * ptr = (byte *) data.Scan0.ToPointer( );

			// for each row
			for ( int y = 0; y < height; y++ )
			{
				// for each pixel
				for ( int x = 0; x < width; x++, ptr += 3 )
				{
					rgb.Red		= ptr[RGB.R];
					rgb.Green	= ptr[RGB.G];
					rgb.Blue	= ptr[RGB.B];

					// convert to YCbCr
					AForge.Imaging.ColorConverter.RGB2YCbCr( rgb, ycbcr );

					// correct Y
					if ( ycbcr.Y >= inY.Max )
						ycbcr.Y = outY.Max;
					else if ( ycbcr.Y <= inY.Min )
						ycbcr.Y = outY.Min;
					else
						ycbcr.Y = ky * ycbcr.Y + by;

					// correct Cb
					if ( ycbcr.Cb >= inCb.Max )
						ycbcr.Cb = outCb.Max;
					else if ( ycbcr.Cb <= inCb.Min )
						ycbcr.Cb = outCb.Min;
					else
						ycbcr.Cb = kcb * ycbcr.Cb + bcb;

					// correct Cr
					if ( ycbcr.Cr >= inCr.Max )
						ycbcr.Cr = outCr.Max;
					else if ( ycbcr.Cr <= inCr.Min )
						ycbcr.Cr = outCr.Min;
					else
						ycbcr.Cr = kcr * ycbcr.Cr + bcr;

					// convert back to RGB
					AForge.Imaging.ColorConverter.YCbCr2RGB( ycbcr, rgb );

					ptr[RGB.R] = rgb.Red;
					ptr[RGB.G] = rgb.Green;
					ptr[RGB.B] = rgb.Blue;
				}
				ptr += offset;
			}
		}
	}
}
