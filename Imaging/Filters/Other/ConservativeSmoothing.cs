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
	/// Conservative Smoothing
	/// </summary>
	public class ConservativeSmoothing : IFilter
	{
		private int	size = 3;

		// Size property
		public int Size
		{
			get { return size; }
			set { size = Math.Max(3, Math.Min(25, value | 1)); }
		}

		// Constructor
		public ConservativeSmoothing()
		{
		}
		public ConservativeSmoothing(int size)
		{
			Size = size;
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

			int pixelSize = (fmt == PixelFormat.Format8bppIndexed) ? 1 : 3;
			int stride = srcData.Stride;
			int offset = stride - width * pixelSize;

			int i, j, t;
			int radius = size >> 1;

			byte minR, maxR, minG, maxG, minB, maxB, v;

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
							minG = 255;
							maxG = 0;

							// for each kernel row
							for (i = -radius; i <= radius; i++)
							{
								t = y + i;

								// skip row
								if (t < 0)
									continue;
								// break
								if (t >= height)
									break;

								// for each kernel column
								for (j = -radius; j <= radius; j++)
								{
									t = x + j;

									// skip column
									if (t < 0)
										continue;

									if ((i != j) && (t < width))
									{
										// find MIN and MAX values
										v = src[i * stride + j];

										if (v < minG)
											minG = v;
										if (v > maxG)
											maxG = v;
									}
								}
							}
							// set destination pixel
							v = *src;
							*dst = (v > maxG) ? maxG : ((v < minG) ? minG : v);
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
							minR = minG = minB = 255;
							maxR = maxG = maxB = 0;

							// for each kernel row
							for (i = -radius; i <= radius; i++)
							{
								t = y + i;

								// skip row
								if (t < 0)
									continue;
								// break
								if (t >= height)
									break;

								// for each kernel column
								for (j = -radius; j <= radius; j++)
								{
									t = x + j;

									// skip column
									if (t < 0)
										continue;

									if ((i != j) && (t < width))
									{
										p = &src[i * stride + j * 3];

										// find MIN and MAX values

										// red
										v = p[RGB.R];

										if (v < minR)
											minR = v;
										if (v > maxR)
											maxR = v;

										// green
										v = p[RGB.G];

										if (v < minG)
											minG = v;
										if (v > maxG)
											maxG = v;

										// blue
										v = p[RGB.B];

										if (v < minB)
											minB = v;
										if (v > maxB)
											maxB = v;
									}
								}
							}
							// set destination pixel

							// red
							v = src[RGB.R];
							dst[RGB.R] = (v > maxR) ? maxR : ((v < minR) ? minR : v);
							// green
							v = src[RGB.G];
							dst[RGB.G] = (v > maxG) ? maxG : ((v < minG) ? minG : v);
							// blue
							v = src[RGB.B];
							dst[RGB.B] = (v > maxB) ? maxB : ((v < minB) ? minB : v);
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
