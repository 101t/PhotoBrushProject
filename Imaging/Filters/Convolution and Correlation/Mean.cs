// AForge Image Processing Library
//
// Copyright © Andrew Kirillov, 2005
// andrew.kirillov@gmail.com
//

namespace AForge.Imaging.Filters
{
	/// <summary>
	/// Mean filter
	/// </summary>
	public sealed class Mean : Correlation
	{
		public Mean() : base(new int[,] {
										{1, 1, 1},
										{1, 1, 1},
										{1, 1, 1}})
		{
		}
	}
}
