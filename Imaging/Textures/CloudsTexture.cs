// AForge Image Processing Library
//
// Copyright © Andrew Kirillov, 2005
// andrew.kirillov@gmail.com
//

namespace AForge.Imaging.Textures
{
	using System;

	/// <summary>
	/// Creates clouds texture
	/// </summary>
	public class CloudsTexture : ITextureGenerator
	{
		private AForge.Math.PerlinNoise	noise = new AForge.Math.PerlinNoise( 1.0 / 32, 1.0, 0.5, 8 );

		private Random	rand = new Random( );
		private int		r;

		// Constructors
		public CloudsTexture( )
		{
			Reset( );
		}


		// Generate texture
		public float[,] Generate( int width, int height )
		{
			float[,]	texture = new float[height, width];

			for ( int y = 0; y < height; y++ )
			{
				for ( int x = 0; x < width; x++ )
				{
					texture[y, x] = 
						Math.Max( 0.0f, Math.Min( 1.0f,
							(float) noise.Function2D( x + r, y + r ) * 0.5f + 0.5f
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
