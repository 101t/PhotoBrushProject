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
	/// HSLFiltering - filters pixels inside or outside specified HSL range
	/// </summary>
	public class HSLFiltering : IFilter, IInPlaceFilter
	{
		private Range hue			= new Range( 0, 359 );
		private RangeD saturation	= new RangeD( 0.0, 1.0 );
		private RangeD luminance	= new RangeD( 0.0, 1.0 );

		private int fillH = 0;
		private double fillS = 0.0;
		private double fillL = 0.0;
		private bool fillOutsideRange = true;

		private bool updateH = true;
		private bool updateS = true;
		private bool updateL = true;

		#region Public properties

		// Hue property
		public Range Hue
		{
			get { return hue; }
			set { hue = value; }
		}
		// Saturation property
		public RangeD Saturation
		{
			get { return saturation; }
			set { saturation = value; }
		}
		// Luminance property
		public RangeD Luminance
		{
			get { return luminance; }
			set { luminance = value; }
		}
		// Fill color property
		public HSL FillColor
		{
			get { return new HSL( fillH, fillS, fillL ); }
			set
			{
				fillH = value.Hue;
				fillS = value.Saturation;
				fillL = value.Luminance;
			}
		}
		// FillOutsideRange property
		public bool FillOutsideRange
		{
			get { return fillOutsideRange; }
			set { fillOutsideRange = value; }
		}
		// Update Hue value property
		public bool UpdateHue
		{
			get { return updateH; }
			set { updateH = value; }
		}
		// Update Saturation value property
		public bool UpdateSaturation
		{
			get { return updateS; }
			set { updateS = value; }
		}
		// Update Luminance value property
		public bool UpdateLuminance
		{
			get { return updateL; }
			set { updateL = value; }
		}

		#endregion


		// Constructor
		public HSLFiltering( )
		{
		}
		public HSLFiltering( Range hue, RangeD saturation, RangeD luminance )
		{
			this.hue		= hue;
			this.saturation	= saturation;
			this.luminance	= luminance;
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

					// convert to HSL
					AForge.Imaging.ColorConverter.RGB2HSL( rgb, hsl );

					// check HSL values
					if (
						( hsl.Saturation >= saturation.Min ) && ( hsl.Saturation <= saturation.Max ) &&
						( hsl.Luminance >= luminance.Min ) && ( hsl.Luminance <= luminance.Max ) &&
						(
						( ( hue.Min < hue.Max ) && ( hsl.Hue >= hue.Min ) && ( hsl.Hue <= hue.Max ) ) ||
						( ( hue.Min > hue.Max ) && ( ( hsl.Hue >= hue.Min ) || ( hsl.Hue <= hue.Max ) ) )
						)
						)
					{
						if ( !fillOutsideRange )
						{
							if ( updateH ) hsl.Hue			= fillH;
							if ( updateS ) hsl.Saturation	= fillS;
							if ( updateL ) hsl.Luminance	= fillL;

							updated = true;
						}
					}
					else
					{
						if ( fillOutsideRange )
						{
							if ( updateH ) hsl.Hue			= fillH;
							if ( updateS ) hsl.Saturation	= fillS;
							if ( updateL ) hsl.Luminance	= fillL;

							updated = true;
						}
					}

					if ( updated )
					{
						// convert back to RGB
						AForge.Imaging.ColorConverter.HSL2RGB( hsl, rgb );

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
