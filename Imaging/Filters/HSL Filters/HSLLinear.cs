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
	/// Luminance and Saturation linear correction
	/// </summary>
	public class HSLLinear : IFilter, IInPlaceFilter
	{
		private RangeD	inLuminance = new RangeD( 0.0, 1.0 );
		private RangeD	inSaturation = new RangeD( 0.0, 1.0 );
		private RangeD	outLuminance = new RangeD( 0.0, 1.0 );
		private RangeD	outSaturation = new RangeD( 0.0, 1.0 );

		#region Public Propertis

		// LuminanceIn property
		public RangeD InLuminance
		{
			get { return inLuminance; }
			set { inLuminance = value; }
		}
		// LuminanceOut property
		public RangeD OutLuminance
		{
			get { return outLuminance; }
			set { outLuminance = value; }
		}

		// SaturationIn property
		public RangeD InSaturation
		{
			get { return inSaturation; }
			set { inSaturation = value; }
		}
		// SaturationOut property
		public RangeD OutSaturation
		{
			get { return outSaturation; }
			set { outSaturation = value; }
		}

		#endregion


		// Constructors
		public HSLLinear( )
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
			HSL		hsl = new HSL( );
			int		offset = data.Stride - width * 3;
			double	kl = 0, bl = 0;
			double	ks = 0, bs = 0;

			// luminance line params
			if ( inLuminance.Max != inLuminance.Min )
			{
				kl = ( outLuminance.Max - outLuminance.Min ) / ( inLuminance.Max - inLuminance.Min );
				bl = outLuminance.Min - kl * inLuminance.Min;
			}
			// saturation line params
			if ( inSaturation.Max != inSaturation.Min )
			{
				ks = ( outSaturation.Max - outSaturation.Min ) / ( inSaturation.Max - inSaturation.Min );
				bs = outSaturation.Min - ks * inSaturation.Min;
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

					// convert to HSL
					AForge.Imaging.ColorConverter.RGB2HSL( rgb, hsl );

					// correct luminance
					if ( hsl.Luminance >= inLuminance.Max )
						hsl.Luminance = outLuminance.Max;
					else if ( hsl.Luminance <= inLuminance.Min )
						hsl.Luminance = outLuminance.Min;
					else
						hsl.Luminance = kl * hsl.Luminance + bl;

					// correct saturation
					if ( hsl.Saturation >= inSaturation.Max )
						hsl.Saturation = outSaturation.Max;
					else if ( hsl.Saturation <= inSaturation.Min )
						hsl.Saturation = outSaturation.Min;
					else
						hsl.Saturation = ks * hsl.Saturation + bs;

					// convert back to RGB
					AForge.Imaging.ColorConverter.HSL2RGB( hsl, rgb );

					ptr[RGB.R] = rgb.Red;
					ptr[RGB.G] = rgb.Green;
					ptr[RGB.B] = rgb.Blue;
				}
				ptr += offset;
			}
		}
	}
}
