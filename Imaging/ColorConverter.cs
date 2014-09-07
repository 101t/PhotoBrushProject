// AForge Image Processing Library
//
// Copyright © Andrew Kirillov, 2005
// andrew.kirillov@gmail.com
//

namespace AForge.Imaging
{
	using System;
	using System.Drawing;

	/// <summary>
	/// RGB components
	/// Note: PixelFormat.Format24bppRgb actually means BGR format
	/// </summary>
	public class RGB
	{
		public const short R = 2;
		public const short G = 1;
		public const short B = 0;

		public byte	Red;
		public byte Green;
		public byte Blue;

		// Color property
		public System.Drawing.Color Color
		{
			get { return Color.FromArgb( Red, Green, Blue ); }
			set
			{
				Red		= value.R;
				Green	= value.G;
				Blue	= value.B;
			}
		}


		// Constructors
		public RGB( )
		{
		}
		public RGB( byte red, byte green, byte blue )
		{
			this.Red	= red;
			this.Green	= green;
			this.Blue	= blue;
		}
		public RGB( System.Drawing.Color color )
		{
			this.Red	= color.R;
			this.Green	= color.G;
			this.Blue	= color.B;
		}
	};

	/// <summary>
	/// HSL components
	/// </summary>
	public class HSL
	{
		public int		Hue;			// 0-359 : hue value
		public double	Saturation;		// 0-1 : saturation value
		public double	Luminance;		// 0-1 : luminance value

		// Constructors
		public HSL( )
		{
		}
		public HSL( int hue, double saturation, double luminance )
		{
			this.Hue		= hue;
			this.Saturation	= saturation;
			this.Luminance	= luminance;
		}
	};

	/// <summary>
	/// YCbCr components
	/// </summary>
	public class YCbCr
	{
		public const short YIndex  = 0;
		public const short CbIndex = 1;
		public const short CrIndex = 2;

		public double Y;
		public double Cb;
		public double Cr;

		// Constructors
		public YCbCr( )
		{
		}
		public YCbCr( double y, double cb, double cr )
		{
			this.Y	= Math.Max(  0.0, Math.Min( 1.0, y ) );
			this.Cb	= Math.Max( -0.5, Math.Min( 0.5, cb ) );
			this.Cr	= Math.Max( -0.5, Math.Min( 0.5, cr ) );
		}
	}



	/// <summary>
	/// Color converter - converts colors from different color spaces
	/// </summary>
	public class ColorConverter
	{
		// Convert from RGB to HSL color space
		public static void RGB2HSL( RGB rgb, HSL hsl )
		{
			double	r = ( rgb.Red   / 255.0 );
			double	g = ( rgb.Green / 255.0 );
			double	b = ( rgb.Blue  / 255.0 );

			double	min = Math.Min( Math.Min( r, g ), b );
			double	max = Math.Max( Math.Max( r, g ), b );
			double	delta = max - min;

			// get luminance value
			hsl.Luminance = ( max + min ) / 2;

			if ( delta == 0 )
			{
				// gray color
				hsl.Hue = 0;
				hsl.Saturation = 0.0;
			}
			else
			{
				// get saturation value
				hsl.Saturation = ( hsl.Luminance < 0.5 ) ? ( delta / ( max + min ) ) : ( delta / ( 2 - max - min ) );

				// get hue value
				double	del_r = ( ( ( max - r ) / 6 ) + ( delta / 2 ) ) / delta;
				double	del_g = ( ( ( max - g ) / 6 ) + ( delta / 2 ) ) / delta;
				double	del_b = ( ( ( max - b ) / 6 ) + ( delta / 2 ) ) / delta;
				double	hue;

				if ( r == max )
					hue = del_b - del_g;
				else if ( g == max )
					hue = ( 1.0 / 3 ) + del_r - del_b;
				else
					hue = ( 2.0 / 3 ) + del_g - del_r;

				// correct hue if needed
				if ( hue < 0 )
					hue += 1;
				if ( hue > 1 )
					hue -= 1;

				hsl.Hue = (int) ( hue * 360 );
			}
		}

		// Convert from HSL to RGB color space
		public static void HSL2RGB( HSL hsl, RGB rgb )
		{
			if ( hsl.Saturation == 0 )
			{
				// gray values
				rgb.Red = rgb.Green = rgb.Blue = (byte) ( hsl.Luminance * 255 );
			}
			else
			{
				double	v1, v2;
				double	hue = (double) hsl.Hue / 360;

				v2 = ( hsl.Luminance < 0.5 ) ?
					( hsl.Luminance * ( 1 + hsl.Saturation ) ) :
					( ( hsl.Luminance + hsl.Saturation ) - ( hsl.Luminance * hsl.Saturation ) );
				v1 = 2 * hsl.Luminance - v2;

				rgb.Red		= (byte)( 255 * Hue_2_RGB( v1, v2, hue + ( 1.0 / 3 ) ) );
				rgb.Green	= (byte)( 255 * Hue_2_RGB( v1, v2, hue ) );
				rgb.Blue	= (byte)( 255 * Hue_2_RGB( v1, v2, hue - ( 1.0 / 3 ) ) );
			}
		}

		// Convert from RGB to YCbCr color space (Rec 601-1 specification)
		public static void RGB2YCbCr( RGB rgb, YCbCr ycbcr )
		{
			double r = (double) rgb.Red   / 255;
			double g = (double) rgb.Green / 255;
			double b = (double) rgb.Blue  / 255;

			ycbcr.Y		=  0.2989 * r + 0.5866 * g + 0.1145 * b;
			ycbcr.Cb	= -0.1687 * r - 0.3313 * g + 0.5000 * b;
			ycbcr.Cr	=  0.5000 * r - 0.4184 * g - 0.0816 * b;
		}

		// Convert from YCbCr to RGB color space
		public static void YCbCr2RGB( YCbCr ycbcr, RGB rgb )
		{
			// don't warry about zeros. compiler will remove them
			double r = Math.Max( 0.0, Math.Min( 1.0, ycbcr.Y + 0.0000 * ycbcr.Cb + 1.4022 * ycbcr.Cr ) );
			double g = Math.Max( 0.0, Math.Min( 1.0, ycbcr.Y - 0.3456 * ycbcr.Cb - 0.7145 * ycbcr.Cr ) );
			double b = Math.Max( 0.0, Math.Min( 1.0, ycbcr.Y + 1.7710 * ycbcr.Cb + 0.0000 * ycbcr.Cr ) );

			rgb.Red		= (byte) ( r * 255 );
			rgb.Green	= (byte) ( g * 255 );
			rgb.Blue	= (byte) ( b * 255 );
		}

		#region Private members
		// HSL to RGB helper routine
		private static double Hue_2_RGB( double v1, double v2, double vH )
		{
			if ( vH < 0 )
				vH += 1;
			if ( vH > 1 )
				vH -= 1;
			if ( ( 6 * vH ) < 1 )
				return ( v1 + ( v2 - v1 ) * 6 * vH );
			if ( ( 2 * vH ) < 1 )
				return v2;
			if ( ( 3 * vH ) < 2 )
				return ( v1 + ( v2 - v1 ) * ( ( 2.0 / 3 ) - vH ) * 6);
			return v1;
		}
		#endregion
	}
}
