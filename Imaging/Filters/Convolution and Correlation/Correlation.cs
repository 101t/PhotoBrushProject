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
	/// Correlation filter
	/// </summary>
	public class Correlation : IFilter
	{
		// kernel
		protected int[,]	kernel;
		protected int		size;

		// Constructor
		protected Correlation()
		{
		}
		public Correlation(int[,] kernel)
		{
			int s = kernel.GetLength(0);

			// check kernel size
			if ((s != kernel.GetLength(1)) || (s < 3) || (s > 25) || (s % 2 == 0))
				throw new ArgumentException();

			this.kernel = kernel;
			this.size = s;
		}

	
		// Apply filter
		public Bitmap Apply(Bitmap srcImg)
		{
			// get source image size
			int width = srcImg.Width;
			int height = srcImg.Height;
			
			PixelFormat fmt = (srcImg.PixelFormat == PixelFormat.Format8bppIndexed) ?
				PixelFormat.Format8bppIndexed : PixelFormat.Format24bppRgb;

			// lock source bitmap data
			BitmapData srcData = srcImg.LockBits(
				new Rectangle(0, 0, width, height),
				ImageLockMode.ReadOnly, fmt);

			// create new image
			Bitmap dstImg = (fmt == PixelFormat.Format8bppIndexed) ?
				AForge.Imaging.Image.CreateGrayscaleImage(width, height) :
				new Bitmap(width, height, fmt);

			// lock destination bitmap data
			BitmapData dstData = dstImg.LockBits(
				new Rectangle(0, 0, width, height),
				ImageLockMode.ReadWrite, fmt);

			int stride = srcData.Stride;
			int offset = stride - ((fmt == PixelFormat.Format8bppIndexed) ? width : width * 3);

			int i, j, t, k, ir, jr;
			int radius = size >> 1;
			long r, g, b, div;

			// do the job
			unsafe
			{
				byte * src = (byte *) srcData.Scan0.ToPointer();
				byte * dst = (byte *) dstData.Scan0.ToPointer();
				byte * p;

				if (fmt == PixelFormat.Format8bppIndexed)
				{
					// Grayscale image

					// for each line
					for (int y = 0; y < height; y++)
					{
						// for each pixel
						for (int x = 0; x <	width; x++, src ++, dst ++)
						{
							g = div = 0;
			
							// for each kernel row
							for (i = 0; i < size; i++)
							{
								ir = i - radius;
								t = y + ir;

								// skip row
								if (t < 0)
									continue;
								// break
								if (t >= height)
									break;

								// for each kernel column
								for (j = 0; j < size; j++)
								{
									jr = j - radius;
									t = x + jr;

									// skip column
									if (t < 0)
										continue;

									if (t < width)
									{
										k = kernel[i, j];

										div += k;
										g += k * src[ir * stride + jr];
									}
								}
							}

							// check divider
							if (div != 0)
							{
								g /= div;
							}
							*dst = (g > 255) ? (byte) 255 : ((g < 0) ? (byte) 0 : (byte) g);
						}
						src += offset;
						dst += offset;
					}
				}
				else
				{
					// RGB image

					// for each line
					for (int y = 0; y < height; y++)
					{
						// for each pixel
						for (int x = 0; x <	width; x++, src += 3, dst += 3)
						{
							r = g = b = div = 0;
			
							// for each kernel row
							for (i = 0; i < size; i++)
							{
								ir = i - radius;
								t = y + ir;

								// skip row
								if (t < 0)
									continue;
								// break
								if (t >= height)
									break;

								// for each kernel column
								for (j = 0; j < size; j++)
								{
									jr = j - radius;
									t = x + jr;

									// skip column
									if (t < 0)
										continue;

									if (t < width)
									{
										k = kernel[i, j];
										p = &src[ir * stride + jr * 3];

										div += k;

										r += k * p[RGB.R];
										g += k * p[RGB.G];
										b += k * p[RGB.B];
									}
								}
							}

							// check divider
							if (div != 0)
							{
								r /= div;
								g /= div;
								b /= div;
							}
							dst[RGB.R] = (r > 255) ? (byte) 255 : ((r < 0) ? (byte) 0 : (byte) r);
							dst[RGB.G] = (g > 255) ? (byte) 255 : ((g < 0) ? (byte) 0 : (byte) g);
							dst[RGB.B] = (b > 255) ? (byte) 255 : ((b < 0) ? (byte) 0 : (byte) b);
						}
						src += offset;
						dst += offset;
					}
				}
			}
			// unlock both images
			dstImg.UnlockBits(dstData);
			srcImg.UnlockBits(srcData);

			return dstImg;
		}
	}
}
