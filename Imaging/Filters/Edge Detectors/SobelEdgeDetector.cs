// AForge Image Processing Library
//
// Copyright � Andrew Kirillov, 2005
// andrew.kirillov@gmail.com
//
// Made using article by Bill Green
// http://www.pages.drexel.edu/~weg22/can_tut.html
//
namespace AForge.Imaging.Filters
{
	using System;
	using System.Drawing;
	using System.Drawing.Imaging;

	/// <summary>
	/// Sobel edge detector
	/// </summary>
	public class SobelEdgeDetector : IFilter
	{
		// Sobel kernels
		private static int[,]	xKernel = new int[,]
		{
			{-1,  0,  1},
			{-2,  0,  2},
			{-1,  0,  1}
		};
		private static int[,]	yKernel = new int[,]
		{
			{ 1,  2,  1},
			{ 0,  0,  0},
			{-1, -2, -1}
		};

		private bool scaleIntensity = true;

		// Scale intensity property
		public bool ScaleIntensity
		{
			get { return scaleIntensity; }
			set { scaleIntensity = value; }
		}


		// Apply filter
		public Bitmap Apply(Bitmap srcImg)
		{
			// get source image size
			int width = srcImg.Width;
			int height = srcImg.Height;

			// convert input to grayscale if it's not yet
			bool disposeSource = false;
			if (srcImg.PixelFormat != PixelFormat.Format8bppIndexed)
			{
				disposeSource = true;
				// create grayscale image
				IFilter filter = new GrayscaleRMY();
				srcImg = filter.Apply(srcImg);
			}

			// lock source bitmap data
			BitmapData srcData = srcImg.LockBits(
				new Rectangle(0, 0, width, height),
				ImageLockMode.ReadOnly, PixelFormat.Format8bppIndexed);

			// create new image
			Bitmap dstImg = AForge.Imaging.Image.CreateGrayscaleImage(width, height);

			// lock destination bitmap data
			BitmapData dstData = dstImg.LockBits(
				new Rectangle(0, 0, width, height),
				ImageLockMode.ReadWrite, PixelFormat.Format8bppIndexed);

			int stride = srcData.Stride;
			int offset = stride - width;
			int	widthM1 = width - 1;
			int heightM1 = height - 1;
			int i, j, ir;
			double	v, gx, gy, g, max = 0;

			// do the job
			unsafe
			{
				byte * src = (byte *) srcData.Scan0.ToPointer() + stride;
				byte * dst = (byte *) dstData.Scan0.ToPointer() + stride;

				// for each line
				for (int y = 1; y < heightM1; y ++)
				{
					src++;
					dst++;

					// for each pixel
					for (int x = 1; x <	widthM1; x++, src ++, dst ++)
					{
						gx = gy = 0;
						// for each kernel row
						for (i = 0; i < 3; i++)
						{
							ir = i - 1;
							// for each kernel column
							for (j = 0; j < 3; j++)
							{
								// source value
								v = src[ir * stride + j - 1];

								gx += v * xKernel[i, j];
								gy += v * yKernel[i, j];
							}
						}
						// get gradient value
//						g = Math.Min(Math.Sqrt(gx * gx + gy * gy), 255);
						g = Math.Min(Math.Abs(gx) + Math.Abs(gy), 255);	// approximation
						if (g > max)
							max = g;
						*dst = (byte) g;
					}
					src += (offset + 1);
					dst += (offset + 1);
				}

				// do we need scaling
				if ((scaleIntensity) && (max != 255))
				{
					// make the second pass for intensity scaling
					double factor = 255.0 / (double) max;
					dst = (byte *) dstData.Scan0.ToPointer() + stride;

					// for each line
					for (int y = 1; y < heightM1; y ++)
					{
						dst++;
						// for each pixel
						for (int x = 1; x <	widthM1; x++, dst ++)
						{
							*dst = (byte) (factor * *dst);
						}
						dst += (offset + 1);
					}
				}
			}
			// unlock both images
			dstImg.UnlockBits(dstData);
			srcImg.UnlockBits(srcData);

			if (disposeSource == true)
			{
				srcImg.Dispose();
			}

			return dstImg;
		}
	}
}
