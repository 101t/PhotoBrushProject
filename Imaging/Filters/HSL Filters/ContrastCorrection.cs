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
	/// Constrast adjusting using luminance value of HSL color space
	/// </summary>
	public class ContrastCorrection : IFilter, IInPlaceFilter
	{
		private HSLLinear	baseFilter = new HSLLinear( );
		private double		factor;

		// Factor property
		public double Factor
		{
			get { return factor; }
			set
			{
				factor = Math.Max( 0.000001, value );

				// create luminance filter
				baseFilter.InLuminance	= new RangeD( 0.0, 1.0 );
				baseFilter.OutLuminance	= new RangeD( 0.0, 1.0 );

				if ( factor > 1 )
				{
					baseFilter.InLuminance = new RangeD( 0.5 - ( 0.5 / factor ), 0.5 + ( 0.5 / factor ) );
				}
				else
				{
					baseFilter.OutLuminance = new RangeD( 0.5 - ( 0.5 * factor ), 0.5 + ( 0.5 * factor ) );
				}
			}
		}

		// Constructors
		public ContrastCorrection( )
		{
			Factor = 1.25;
		}
		public ContrastCorrection( double factor )
		{
			Factor = factor;
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
