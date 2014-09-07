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
	/// Oil Painting filter
	/// </summary>
	public class OilPainting : IFilter
	{
		private int brushSize = 5;

		// Brush size property
		public int BrushSize
		{
			get { return brushSize; }
			set { brushSize = Math.Max(3, Math.Min(21, value | 1)); }
		}


		// Constructor
		public OilPainting()
		{
		}
		public OilPainting(int brushSize)
		{
			BrushSize = brushSize;
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
			int i, j, t;
			int radius = brushSize >> 1;

			byte intesity, maxIntesity;
			int[] intesities = new int[256];

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
						for (int x = 0; x < width; x++, src++, dst++)
						{
							// clear arrays
							Array.Clear(intesities, 0, 255);

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

									if (t < width)
									{
										intesity = src[i * stride + j];

										intesities[intesity]++;
									}
								}
							}

							// get most frequent intesity
							maxIntesity = 0;
							j = 0;

							for (i = 0; i < 256; i++)
							{
								if (intesities[i] > j)
								{
									maxIntesity = (byte) i;
									j = intesities[i];
								}
							}

							// set destination pixel
							*dst = maxIntesity;
						}
						src += offset;
						dst += offset;
					}
				}
				else
				{
					// RGB image
					int[] red = new int[256];
					int[] green = new int[256];
					int[] blue = new int[256];

					// for each line
					for (int y = 0; y < height; y++)
					{
						// for each pixel
						for (int x = 0; x < width; x++, src += 3, dst += 3)
						{
							// clear arrays
							Array.Clear(intesities, 0, 255);
							Array.Clear(red, 0, 256);
							Array.Clear(green, 0, 256);
							Array.Clear(blue, 0, 256);

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

									if (t < width)
									{
										p = &src[i * stride + j * 3];

										// grayscale value using BT709
										intesity = (byte)(0.2125f * p[RGB.R] + 0.7154f * p[RGB.G] + 0.0721f * p[RGB.B]);

										//
										intesities[intesity] ++;
										// red
										red[intesity]	+= p[RGB.R];
										// green
										green[intesity]	+= p[RGB.G];
										// blue
										blue[intesity]	+= p[RGB.B];
									}
								}
							}

							// get most frequent intesity
							maxIntesity = 0;
							j = 0;

							for (i = 0; i < 256; i++)
							{
								if (intesities[i] > j)
								{
									maxIntesity = (byte) i;
									j = intesities[i];
								}
							}

							// set destination pixel
							dst[RGB.R] = (byte)(red[maxIntesity] / intesities[maxIntesity]);
							dst[RGB.G] = (byte)(green[maxIntesity] / intesities[maxIntesity]);
							dst[RGB.B] = (byte)(blue[maxIntesity] / intesities[maxIntesity]);
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
