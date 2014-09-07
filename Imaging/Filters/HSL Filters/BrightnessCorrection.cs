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
	/// Brightness adjusting using luminance value of HSL color space
	/// </summary>
	public class BrightnessCorrection : IFilter, IInPlaceFilter
	{
		private HSLLinear	baseFilter = new HSLLinear( );
		private double		adjustValue;	// [-1, 1]

		// AdjustValue property
		public double AdjustValue
		{
			get { return adjustValue; }
			set
			{
				adjustValue = Math.Max( -1.0, Math.Min( 1.0, value ) );

				// create luminance filter
				if ( adjustValue > 0 )
				{
					baseFilter.InLuminance	= new RangeD( 0.0, 1.0 - adjustValue );
					baseFilter.OutLuminance	= new RangeD( adjustValue, 1.0 );
				}
				else
				{
					baseFilter.InLuminance	= new RangeD( -adjustValue, 1.0 );
					baseFilter.OutLuminance	= new RangeD( 0.0, 1.0 + adjustValue );
				}
			}
		}
		
		// Constructors
		public BrightnessCorrection( )
		{
			AdjustValue = 0.05;
		}
		public BrightnessCorrection( double adjustValue )
		{
			AdjustValue = adjustValue;
		}


		// Apply filter
		public Bitmap Apply( Bitmap srcImg )
		{
			return baseFilter.Apply( srcImg );
		}


		// Apply filter on current image
		public void ApplyInPlace( Bitmap img )
		{
			baseFilter.ApplyInPlace( img );
		}
	}
}
