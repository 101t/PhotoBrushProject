// AForge Image Processing Library
//
// Copyright © Andrew Kirillov, 2005
// andrew.kirillov@gmail.com
//

namespace AForge.Imaging.Filters
{
	/// <summary>
	/// Sharpen filter
	/// </summary>
	public sealed class Sharpen : Correlation
	{
		public Sharpen() : base(new int[,] {
										{0, -1, 0},
										{-1, 5, -1},
										{0, -1, 0}})
		{
		}
	}
}
