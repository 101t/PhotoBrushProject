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
	/// Closing operator from Mathematical Morphology
	/// </summary>
	public class Closing : IFilter
	{
		IFilter errosion = new Erosion();
		IFilter dilatation = new Dilatation();

		// Constructor
		public Closing()
		{
		}
		public Closing(short[,] se)
		{
			errosion = new Erosion(se);
			dilatation = new Dilatation(se);
		}

		// Apply filter
		public Bitmap Apply(Bitmap srcImg)
		{
			Bitmap	tmpImg = dilatation.Apply(srcImg);
			Bitmap	dstImg = errosion.Apply(tmpImg);

			tmpImg.Dispose();

			return dstImg;
		}
	}
}
