// AForge Image Processing Library
//
// Copyright © Andrew Kirillov, 2005
// andrew.kirillov@gmail.com
//

namespace AForge.Imaging.Filters
{
	using System;

	/// <summary>
	/// BayerOrderedDithering - ordered dithering with Bayer matrix
	/// </summary>
	public sealed class BayerDithering : OrderedDithering
	{
		// Constructor
		public BayerDithering() : base(new byte[,] {
								{  0, 192,  48, 240},
								{128,  64, 176, 112},
								{ 32, 224,  16, 208},
								{160,  96, 144,  80}})
		{
		}
	}
}
