// AForge Image Processing Library
//
// Copyright © Andrew Kirillov, 2005
// andrew.kirillov@gmail.com
//
// Original idea found in Paint.NET project
// http://www.eecs.wsu.edu/paint.net/
//
namespace AForge.Imaging.Filters
{
	using System;
	using System.Drawing;
	using System.Drawing.Imaging;
	
	/// <summary>
	/// Sharpen
	/// </summary>
	public class SharpenEx : IFilter
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
		public SharpenEx()
		{
			CreateFilter();
		}
		public SharpenEx(double sigma)
		{
			Sigma = sigma;
		}
		public SharpenEx(double sigma, int size)
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

			// create Gaussian kernel
			int[,] kernel = gaus.KernelDiscret2D(size);

			// calculte sum of the kernel
			int sum = 0;

			for (int i = 0; i < size; i++)
			{
				for (int j = 0; j < size; j++)
				{
					sum += kernel[i, j];
				}
			}

			// recalc kernel
			int c = size >> 1;

			for (int i = 0; i < size; i++)
			{
				for (int j = 0; j < size; j++)
				{
					if ((i == c) && (j == c))
					{
						// calculate central value
						kernel[i, j] = 2 * sum - kernel[i, j];
					}
					else
					{
						// invert value
						kernel[i, j] = -kernel[i, j];
					}
				}
			}

			// create filter
			filter = new Correlation(kernel);
		}
		#endregion
	}
}
