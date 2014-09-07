// AForge Image Processing Library
//
// Copyright © Andrew Kirillov, 2005
// andrew.kirillov@gmail.com
//

namespace AForge.Imaging.Filters
{
	/// <summary>
	/// Blur filter
	/// </summary>
	public sealed class Blur : Correlation
	{
		public Blur() : base(new int[,] {
								{1, 2, 3, 2, 1},
								{2, 4, 5, 4, 2},
								{3, 5, 6, 5, 3},
								{2, 4, 5, 4, 2},
								{1, 2, 3, 2, 1}})
		{
		}
	}
}
