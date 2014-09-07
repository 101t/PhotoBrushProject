// AForge Image Processing Library
//
// Copyright © Andrew Kirillov, 2005
// andrew.kirillov@gmail.com
//

namespace AForge.Imaging.Filters
{
	using System;
	using System.Drawing;
	using System.Drawing.Imaging;

	/// <summary>
	/// FilterIterator - perform specified count of filter's iterations
	/// </summary>
	public class FilterIterator : IFilter
	{
		private IFilter	baseFilter;
		private int		iterations = 1;

		// Base filter
		public IFilter BaseFilter
		{
			get { return baseFilter; }
			set { baseFilter = value; }
		}

		// Iterations count
		public int Iterations
		{
			get { return iterations; }
			set
			{
				iterations = Math.Max(1, Math.Min(255, value));
			}
		}

		// Constructor
		public FilterIterator()
		{
		}
		public FilterIterator(IFilter baseFilter)
		{
			this.baseFilter = baseFilter;
		}
		public FilterIterator(IFilter baseFilter, int iterations)
		{
			this.baseFilter	= baseFilter;
			this.iterations	= iterations;
		}

		// Apply filter using mask
		public Bitmap Apply(Bitmap srcImg)
		{
			if (baseFilter == null)
				throw new NullReferenceException("Base filter is not set");

			// initial iteration
			Bitmap	dstImg = baseFilter.Apply(srcImg);
			Bitmap	tmpImg;

			// continue iterate
			for (int i = 1; i < iterations; i++)
			{
				tmpImg = dstImg;
				dstImg = baseFilter.Apply(tmpImg);
				tmpImg.Dispose();
			}

			return dstImg;
		}
	}
}
