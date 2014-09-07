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
	/// Opening operator from Mathematical Morphology
	/// </summary>
	public class Opening : IFilter
	{
		IFilter errosion = new Erosion();
		IFilter dilatation = new Dilatation();

		// Constructor
		public Opening()
		{
		}
		public Opening(short[,] se)
		{
			errosion = new Erosion(se);
			dilatation = new Dilatation(se);
		}

		// Apply filter
		public Bitmap Apply(Bitmap srcImg)
		{
			Bitmap	tmpImg = errosion.Apply(srcImg);
			Bitmap	dstImg = dilatation.Apply(tmpImg);

			tmpImg.Dispose();

			return dstImg;
		}
	}
}
