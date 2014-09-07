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
	/// TestFilter
	/// </summary>
	public class TestFilter : IFilter
	{
		private byte	threshold = 10;

		// Constructor
		public TestFilter()
		{
		}


		// Apply filter
		public Bitmap Apply(Bitmap srcImg)
		{
			// get source image size
			int width = srcImg.Width;
			int height = srcImg.Height;
			
			PixelFormat fmt = (srcImg.PixelFormat == PixelFormat.Format8bppIndexed) ?
				PixelFormat.Format8bppIndexed : PixelFormat.Format24bppRgb;

			if (fmt != PixelFormat.Format8bppIndexed)
				throw new ArgumentException();

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
			int step = 1;

			// copy source image to destination
			Win32.memcpy(dstData.Scan0, srcData.Scan0, height * srcData.Stride);

			// do the job
			unsafe
			{
				byte * p1;
				byte * p2;

				if (fmt == PixelFormat.Format8bppIndexed)
				{
					// grayscale
					byte[,] oldNodes = null;

					int w = width >> step;
					int h = height >> step;

					// check we have enough pixels for the next step
					while ((w > 0) && (h > 0))
					{
						byte * src = (byte *) dstData.Scan0.ToPointer();
						byte * dst = (byte *) dstData.Scan0.ToPointer();

						byte[,]	nodes = new byte[h, w];
						int[]	tmp = new int[w];
						int		s = step << 1;

						// walk through rows
						for (int y = 0; y < h; y++)
						{
							// clear previos values
							Array.Clear(tmp, 0, w);

							// walk through source pixels
							p1 = src;
							p2 = src + (stride * step);
							for (int x = 0; x <	w; x ++, p1 += s, p2 += s)
							{
								tmp[x] = (int) *p1 + (int) p1[step] + (int) *p2 + (int) p2[step];
							}
							src += (stride * s);

							// get average values
							for (int j = 0; j < w; j++)
								tmp[j] >>= 2;

							// walk through destination pixels
							p1 = dst;
							p2 = dst + (stride * step);
							for (int x = 0; x <	w; x ++, p1 += s, p2 += s)
							{
								int avg = tmp[x];

								// check all 4 pixel
								if (
									(Math.Abs((int) *p1 - avg) < threshold) &&
									(Math.Abs((int) p1[step] - avg) < threshold) &&
									(Math.Abs((int) *p2 - avg) < threshold) &&
									(Math.Abs((int) p2[step] - avg) < threshold))
								{
									if (oldNodes == null)
									{
										// it's the first step
										*p1 = p1[step] = *p2 = p2[step] = (byte) avg;
										nodes[y, x] = 1;
									}
									else
									{
										int ny = y << 1;
										int nx = x << 1;

										if ((oldNodes[ny, nx] & oldNodes[ny, nx + 1] & oldNodes[ny + 1, nx] & oldNodes[ny + 1, nx + 1]) == 1)
										{
											int o = stride - s;
											byte * p = (byte *) dstData.Scan0.ToPointer();

											p += (y * s * stride + x * s);

											for (int ty = 0; ty < s; ty++)
											{
												for (int tx = 0; tx < s; tx++, p++)
												{
													*p = (byte) avg;
												}
												p += o;
											}

											nodes[y, x] = 1;
										}
									}
								}
							}
							dst += (stride * s);
						}

						oldNodes = nodes;
						step <<= 1;
						w >>= 1;
						h >>= 1;
					}
				}
				else
				{
				}
			}
			// unlock both images
			dstImg.UnlockBits(dstData);
			srcImg.UnlockBits(srcData);

			return dstImg;
		}
	}
}
