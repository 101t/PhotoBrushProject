// AForge Image Processing Library
//
// Copyright © Andrew Kirillov, 2005
// andrew.kirillov@gmail.com
//

namespace AForge.Imaging.Textures
{
	using System;

	/// <summary>
	/// Creates marble texture
	/// </summary>
	public class MarbleTexture : ITextureGenerator
	{
		private AForge.Math.PerlinNoise	noise = new AForge.Math.PerlinNoise( 1.0 / 32, 1.0, 0.65, 2 );

		private Random	rand = new Random( );
		private int		r;

		private double	xPeriod;
		private double	yPeriod;

		// XPeriod property
		public double XPeriod
		{
			get { return xPeriod; }
			set { xPeriod = value; }
		}
		// YPeriod property
		public double YPeriod
		{
			get { return yPeriod; }
			set { yPeriod = value; }
		}

		// Constructors
		public MarbleTexture( ) : this( 5.0, 10.0 ) { }
		public MarbleTexture( double xPeriod, double yPeriod )
		{
			this.xPeriod = xPeriod;
			this.yPeriod = yPeriod;
			Reset( );
		}

		// Generate texture
		public float[,] Generate( int width, int height )
		{
			float[,]	texture = new float[height, width];
			double		xFact = xPeriod / width;
			double		yFact = yPeriod / height;

			for ( int y = 0; y < height; y++ )
			{
				for ( int x = 0; x < width; x++ )
				{
					texture[y, x] = 
						Math.Max( 0.0f, Math.Min( 1.0f, (float)
							Math.Abs( Math.Sin( 
								( x * xFact + y * yFact + noise.Function2D( x + r, y + r ) ) * Math.PI
							))
						));

				}
			}
			return texture;
		}


		// Reset - regenerate internal random numbers
		public void Reset( )
		{
			r = rand.Next( 5000 );
		}
	}
}
