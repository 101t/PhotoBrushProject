// AForge Image Processing Library
//
// Copyright © Andrew Kirillov, 2005
// andrew.kirillov@gmail.com
//

namespace AForge.Imaging.Textures
{
	using System;

	/// <summary>
	/// Creates wood texture.
	/// </summary>
	public class WoodTexture : ITextureGenerator
	{
		private AForge.Math.PerlinNoise	noise = new AForge.Math.PerlinNoise( 1.0 / 32, 0.05, 0.5, 8 );

		private Random	rand = new Random( );
		private int		r;

		private double	rings;

		// Constructors
		public WoodTexture( ) : this( 12.0 ) { }
		public WoodTexture( double rings )
		{
			this.rings = rings;
			Reset( );
		}


		// Generate texture
		public float[,] Generate( int width, int height )
		{
			float[,]	texture = new float[height, width];
			int			w2 = width / 2;
			int			h2 = height / 2;

			for ( int y = 0; y < height; y++ )
			{
				for ( int x = 0; x < width; x++ )
				{
					double xv = (double) ( x - w2 ) / width;
					double yv = (double) ( y - h2 ) / height;

					texture[y, x] = 
						Math.Max( 0.0f, Math.Min( 1.0f, (float)
						Math.Abs( Math.Sin( 
							( Math.Sqrt( xv * xv + yv * yv ) + noise.Function2D( x + r, y + r ) )
								* Math.PI * 2 * rings
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
