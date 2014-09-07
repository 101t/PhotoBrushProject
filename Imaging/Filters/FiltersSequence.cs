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
	using System.Collections;

	/// <summary>
	/// FiltersSequence class
	/// </summary>
	public class FiltersSequence : CollectionBase, IFilter
	{
		// Constructor
		public FiltersSequence()
		{
		}
		public FiltersSequence(params IFilter[] filters)
		{
			InnerList.AddRange(filters);
		}

		// Get filter at the specified index
		public IFilter this[int index]
		{
			get { return ((IFilter) InnerList[index]); }
		}

		// Add new filter to the sequance
		public void Add(IFilter filter)
		{
			InnerList.Add(filter);
		}

		// Apply filters sequence
		public Bitmap Apply(Bitmap srcImg)
		{
			Bitmap	dstImg = null;
			Bitmap	tmpImg = null;
			int		i, n = InnerList.Count;

			// check for empty sequence
			if (n == 0)
				throw new ApplicationException();

			// apply first filter
			dstImg = ((IFilter) InnerList[0]).Apply(srcImg);

			// apply other filters
			for (i = 1; i < n; i++)
			{
				tmpImg = dstImg;
				dstImg = ((IFilter) InnerList[i]).Apply(tmpImg);
				tmpImg.Dispose();
			}

			return dstImg;
		}
	}
}
