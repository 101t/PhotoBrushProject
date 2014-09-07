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
	/// Gaussian filter
	/// </summary>
	public sealed class GaussianBlur : IFilter
	{
		private Correlation	filter;
		private double		sigma = 1.4;
		private int			size = 5;

		// Sigma property
		public double Sigma
		{
			get { return sigma; }
			set
			{
				// get new sigma value
				sigma = Math.Max(0.5, Math.Min(5.0, value));
				// create filter
				CreateFilter();
			}
		}
		// Size property
		public int Size
		{
			get { return size; }
			set
			{
				size = Math.Max(3, Math.Min(21, value | 1));
				CreateFilter();
			}
		}

		// Constructor
		public GaussianBlur()
		{
			CreateFilter();
		}
		public GaussianBlur(double sigma)
		{
			Sigma = sigma;
		}
		public GaussianBlur(double sigma, int size)
		{
			Sigma = sigma;
			Size = size;
		}


		// Apply filter
		public Bitmap Apply(Bitmap srcImg)
		{
			return filter.Apply(srcImg);
		}

		// Private members
		#region Private Members

		// Create Gaussian filter
		private void CreateFilter()
		{
			// create Gaussian function
			AForge.Math.Gaussian gaus = new AForge.Math.Gaussian(sigma);
			// create kernel
			int[,] kernel = gaus.KernelDiscret2D(size);
			// create filter
			filter = new Correlation(kernel);
		}
		#endregion
	}
}
