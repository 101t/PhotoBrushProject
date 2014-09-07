// AForge Image Processing Library
//
// Copyright © Andrew Kirillov, 2005
// andrew.kirillov@gmail.com
//

namespace AForge.Imaging.Filters
{
	/// <summary>
	/// Simple edge detector
	/// </summary>
	public sealed class Edges : Correlation
	{
		public Edges() : base(new int[,] {
										{0, -1, 0},
										{-1, 4, -1},
										{0, -1, 0}})
		{
		}
	}
}
