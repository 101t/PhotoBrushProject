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
	/// Saturation adjusting of HSL color space
	/// </summary>
	public class SaturationCorrection : IFilter, IInPlaceFilter
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

				// create saturation filter
				if ( adjustValue > 0 )
				{
					baseFilter.InSaturation		= new RangeD( 0.0, 1.0 - adjustValue );
					baseFilter.OutSaturation	= new RangeD( adjustValue, 1.0 );
				}
				else
				{
					baseFilter.InSaturation		= new RangeD( -adjustValue, 1.0 );
					baseFilter.OutSaturation	= new RangeD( 0.0, 1.0 + adjustValue );
				}
			}
		}

		// Constructors
		public SaturationCorrection( )
		{
			AdjustValue = 0.05;
		}
		public SaturationCorrection( double adjustValue )
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
