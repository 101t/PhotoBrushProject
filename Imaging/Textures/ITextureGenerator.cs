// AForge Image Processing Library
//
// Copyright © Andrew Kirillov, 2005
// andrew.kirillov@gmail.com
//

namespace AForge.Imaging.Textures
{
	using System;

	/// <summary>
	/// Texture generator interface
	/// 
	/// Each texture generator generates a texture of the specified size and returns
	/// a two dimensional array of intensities in the range of [0; 1]
	/// </summary>
	public interface ITextureGenerator
	{
		/// <summary>
		/// Generate texture
		/// </summary>
		float[,] Generate( int width, int height );

		/// <summary>
		/// Reset - regenerate internal random numbers
		/// </summary>
		void Reset( );
	}
}
