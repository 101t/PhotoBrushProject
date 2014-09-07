// AForge Math Library
//
// Copyright © Andrew Kirillov, 2005
// andrew.kirillov@gmail.com
//

namespace AForge.Math
{
	using System;

	// http://www.animeimaging.com/asp/PerlinNoise.aspx
	// http://www.student.kuleuven.ac.be/~m0216922/CG/perlinnoise.html

	/// <summary>
	/// Perlin Noise function
	/// </summary>
	public class PerlinNoise
	{
		private double	initFrequency = 1.0 / 16;
		private double	initAmplitude = 1.0;
		private double	persistance = 0.65;
		private int		octaves = 4;

		// Constructors
		public PerlinNoise( )
		{
		}
		public PerlinNoise( double initFrequency, double initAmplitude, double persistance, int octaves )
		{
			this.initFrequency	= initFrequency;
			this.initAmplitude	= initAmplitude;
			this.persistance	= persistance;
			this.octaves		= octaves;
		}

		/// <summary>
		/// 1-D Perlin noise function
		/// </summary>
		public double Function( double x )
		{
			double	frequency = initFrequency;
			double	amplitude = initAmplitude;
			double	sum = 0;
			
			// octaves
			for ( int i = 0; i < octaves; i++ )
			{
				sum += SmoothedNoise( x * frequency ) * amplitude;

				frequency *= 2;
				amplitude *= persistance;
			}
			return sum;
		}

		/// <summary>
		/// 2-D Perlin noise function
		/// </summary>
		public double Function2D( double x, double y )
		{
			double	frequency = initFrequency;
			double	amplitude = initAmplitude;
			double	sum = 0;
			
			// octaves
			for ( int i = 0; i < octaves; i++ )
			{
				sum += SmoothedNoise( x * frequency, y * frequency ) * amplitude;

				frequency *= 2;
				amplitude *= persistance;
			}
			return sum;
		}


		/// <summary>
		/// Ordinary noise function
		/// </summary>
		protected double Noise( int x )
		{
			int n = ( x << 13 ) ^ x;

			return ( 1.0 - ( ( n * ( n * n * 15731 + 789221 ) + 1376312589 ) & 0x7fffffff ) / 1073741824.0 );
		}
		protected double Noise( int x, int y )
		{
			int n = x + y * 57;
			n = ( n << 13 ) ^ n ;

			return ( 1.0 - ( ( n * ( n * n * 15731 + 789221 ) + 1376312589 ) & 0x7fffffff ) / 1073741824.0 );
		}

		
		/// <summary>
		/// Smoothed noise
		/// </summary>
		protected double SmoothedNoise( double x )
		{
			int		xInt = (int) x;
			double	xFrac = x - xInt;

			return CosineInterpolate( Noise( xInt ) , Noise( xInt + 1 ), xFrac );
		}
		protected double SmoothedNoise( double x, double y )
		{
			int		xInt = (int) x;
			int		yInt = (int) y;
			double	xFrac = x - xInt;
			double	yFrac = y - yInt;

			// get four noise values
			double	x0y0 = Noise( xInt    , yInt );
			double	x1y0 = Noise( xInt + 1, yInt );
			double	x0y1 = Noise( xInt    , yInt + 1 );
			double	x1y1 = Noise( xInt + 1, yInt + 1) ;

			// x interpolation
			double	v1 = CosineInterpolate( x0y0, x1y0, xFrac );
			double	v2 = CosineInterpolate( x0y1, x1y1, xFrac );
			// y interpolation
			return CosineInterpolate( v1, v2, yFrac );
		}


		/// <summary>
		/// Cosine interpolation
		/// </summary>
		protected double CosineInterpolate( double x1, double x2, double a )
		{
			double f = ( 1 - Math.Cos( a * Math.PI ) ) * 0.5;

			return x1 * ( 1 - f ) + x2 * f;
		}
	}
}
