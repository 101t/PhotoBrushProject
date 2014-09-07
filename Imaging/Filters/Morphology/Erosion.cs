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
	/// Erosion operator from Mathematical Morphology
	/// </summary>
	public class Erosion : IFilter
	{
		// structuring element
		private short[,]	se = new short[3, 3] {{1, 1, 1}, {1, 1, 1}, {1, 1, 1}};
		private int			size = 3;

		// Constructors
		public Erosion()
		{
		}
		public Erosion(short[,] se)
		{
			int s = se.GetLength(0);

			// check structuring element size
			if ((s != se.GetLength(1)) || (s < 3) || (s > 25) || (s % 2 == 0))
				throw new ArgumentException();

			this.se = se;
			this.size = s;
		}


		// Apply filter
		public Bitmap Apply(Bitmap srcImg)
		{
			if (srcImg.PixelFormat != PixelFormat.Format8bppIndexed)
				throw new ArgumentException();

			// get source image size
			int width = srcImg.Width;
			int height = srcImg.Height;

			// lock source bitmap data
			BitmapData srcData = srcImg.LockBits(
				new Rectangle(0, 0, width, height),
				ImageLockMode.ReadOnly, PixelFormat.Format8bppIndexed);

			// create new grayscale image
			Bitmap dstImg = AForge.Imaging.Image.CreateGrayscaleImage(width, height);

			// lock destination bitmap data
			BitmapData dstData = dstImg.LockBits(
				new Rectangle(0, 0, width, height),
				ImageLockMode.ReadWrite, PixelFormat.Format8bppIndexed);

			int stride = dstData.Stride;
			int offset = stride - width;
			int t, ir, jr, i, j, r = size >> 1;
			byte min, v;

			// do the job
			unsafe
			{
				byte * src = (byte *) srcData.Scan0.ToPointer();
				byte * dst = (byte *) dstData.Scan0.ToPointer();

				// for each line
				for (int y = 0; y < height; y++)
				{
					// for each pixel
					for (int x = 0; x < width; x++, src ++, dst ++)
					{
						min = 255;

						// for each SE row
						for (i = 0; i < size; i++)
						{
							ir = i - r;
							t = y + ir;

							// skip row
							if (t < 0)
								continue;
							// break
							if (t >= height)
								break;

							// for each SE column
							for (j = 0; j < size; j++)
							{
								jr = j - r;
								t = x + jr;

								// skip column
								if (t < 0)
									continue;
								if (t < width)
								{
									if (se[i, j] == 1)
									{
										// get new MIN value
										v = src[ir * stride + jr];
										if (v < min)
											min = v;
									}
								}
							}
						}
						// result pixel
						*dst = min;
					}
					src += offset;
					dst += offset;
				}
			}
			// unlock both images
			dstImg.UnlockBits(dstData);
			srcImg.UnlockBits(srcData);

			return dstImg;
		}
	}
}
