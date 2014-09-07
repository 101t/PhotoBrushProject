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
	/// Canny edge detector
	/// </summary>
	public class CannyEdgeDetector : IFilter
	{
		private IFilter			grayscaleFilter = new GrayscaleBT709();
		private GaussianBlur	gaussianFilter = new GaussianBlur();
		private byte			lowThreshold = 20;
		private byte			highThreshold = 100;

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

		// Low threshold property
		public byte LowThreshold
		{
			get { return lowThreshold; }
			set { lowThreshold = value; }
		}
		// High threshold property
		public byte HighThreshold
		{
			get { return highThreshold; }
			set { highThreshold = value; }
		}
		// Gaussian sigma property (sigma value for Gaussian blurring)
		public double GaussianSigma
		{
			get { return gaussianFilter.Sigma; }
			set { gaussianFilter.Sigma = value; }
		}
		// Gaussian size property (size value for Gaussian blurring)
		public int GaussianSize
		{
			get { return gaussianFilter.Size; }
			set { gaussianFilter.Size = value; }
		}

		// Constructor
		public CannyEdgeDetector()
		{
		}
		public CannyEdgeDetector(byte lowThreshold, byte highThreshold)
		{
			this.lowThreshold	= lowThreshold;
			this.highThreshold	= highThreshold;
		}
		public CannyEdgeDetector(byte lowThreshold, byte highThreshold, double sigma)
		{
			this.lowThreshold	= lowThreshold;
			this.highThreshold	= highThreshold;
			gaussianFilter.Sigma = sigma;
		}


		// Apply filter
		public Bitmap Apply(Bitmap srcImg)
		{
			// Step 1 - grayscale initial image
			Bitmap grayImage = (srcImg.PixelFormat == PixelFormat.Format8bppIndexed) ?
				srcImg : grayscaleFilter.Apply(srcImg);

			// Step 2 - blur image
			Bitmap blurredImage = gaussianFilter.Apply(grayImage);

			// get source image size
			int width = srcImg.Width;
			int height = srcImg.Height;

			// lock source bitmap data
			BitmapData srcData = blurredImage.LockBits(
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
			double v, gx, gy;
			double orientation, toPI = 180.0 / System.Math.PI;
			byte leftPixel = 0, rightPixel = 0;

			// orientation array
			byte[] orients = new byte[width * height];

			// do the job
			unsafe
			{
				byte * src = (byte *) srcData.Scan0.ToPointer() + stride;
				byte * dst = (byte *) dstData.Scan0.ToPointer() + stride;
				int p = width;

				// Step 3 - calculate magnitude and edge orientation

				// for each line
				for (int y = 1; y < heightM1; y ++)
				{
					src++;
					dst++;
					p++;

					// for each pixel
					for (int x = 1; x < widthM1; x ++, src ++, dst ++, p ++)
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
						*dst = (byte) Math.Min(Math.Abs(gx) + Math.Abs(gy), 255);

						// --- get orientation
						// can not devide by zero
						if (gx == 0)	
						{
							orientation = (gy == 0) ? 0 : 90;
						}
						else
						{
							double div = gy / gx;

							// handle angles of the 2nd and 4th quads
							if (div < 0)
							{
								orientation = 180 - System.Math.Atan(- div) * toPI;
							}
							// handle angles of the 1st and 3rd quads
							else
							{
								orientation = System.Math.Atan(div) * toPI;
							}

							// get closest angle from 0, 45, 90, 135 set
							if (orientation < 22.5)
								orientation = 0;
							else if (orientation < 67.5)
								orientation = 45;
							else if (orientation < 112.5)
								orientation = 90;
							else if (orientation < 157.5)
								orientation = 135;
							else orientation = 0;
						}

						// save orientation
						orients[p] = (byte) orientation;
					}
					src += (offset + 1);
					dst += (offset + 1);
					p++;
				}

				// Step 4 - suppres non maximums
				dst = (byte *) dstData.Scan0.ToPointer() + stride;
				p = width;

				// for each line
				for (int y = 1; y < heightM1; y ++)
				{
					dst++;
					p++;

					// for each pixel
					for (int x = 1; x <	widthM1; x ++, dst ++, p ++)
					{
						// get two adjacent pixels
						switch (orients[p])
						{
							case 0:
								leftPixel = dst[-1];
								rightPixel = dst[1];
								break;
							case 45:
								leftPixel = dst[width - 1];
								rightPixel = dst[-width + 1];
								break;
							case 90:
								leftPixel = dst[width];
								rightPixel = dst[-width];
								break;
							case 135:
								leftPixel = dst[width + 1];
								rightPixel = dst[-width - 1];
								break;
						}
						// compare current pixels value with adjacent pixels
						if ((*dst < leftPixel) || (*dst < rightPixel))
						{
							*dst = 0;
						}
					}
					dst += (offset + 1);
					p++;
				}

				// Step 5 - hysteresis
				dst = (byte *) dstData.Scan0.ToPointer() + stride;
				p = width;

				// for each line
				for (int y = 1; y < heightM1; y ++)
				{
					dst++;
					p++;

					// for each pixel
					for (int x = 1; x < widthM1; x ++, dst ++, p ++)
					{
						if (*dst < highThreshold)
						{
							if (*dst < lowThreshold)
							{
								// non edge
								*dst = 0;
							}
							else
							{
								// check 8 neighboring pixels
								if ((dst[-1] < highThreshold) &&
									(dst[1] < highThreshold) &&
									(dst[-width - 1] < highThreshold) &&
									(dst[-width] < highThreshold) &&
									(dst[-width + 1] < highThreshold) &&
									(dst[width - 1] < highThreshold) &&
									(dst[width] < highThreshold) &&
									(dst[width + 1] < highThreshold))
								{
									*dst = 0;
								}
							}
						}
					}
					dst += (offset + 1);
					p++;
				}
			}

			// unlock images
			dstImg.UnlockBits(dstData);
			blurredImage.UnlockBits(srcData);

			// release temporary objects
			blurredImage.Dispose();
			if (grayImage != srcImg)
				grayImage.Dispose();

			return dstImg;
		}
	}
}
