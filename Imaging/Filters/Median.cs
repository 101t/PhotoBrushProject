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
	/// Median filter
	/// </summary>
	public class Median : IFilter
	{
		private int	size = 3;

		// Size property
		public int Size
		{
			get { return size; }
			set { size = Math.Max( 3, Math.Min( 25, value | 1 ) ); }
		}

		// Constructor
		public Median( )
		{
		}
		public Median( int size )
		{
			Size = size;
		}

		// Apply filter
		public Bitmap Apply( Bitmap srcImg )
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
			int radius = size >> 1;
			int	c;

			byte[]	r = new byte[size * size];
			byte[]	g = new byte[size * size];
			byte[]	b = new byte[size * size];

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
							c = 0;
			
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
										g[c++] = src[i * stride + j];
									}
								}
							}
							// sort elements
							Array.Sort(g, 0, c);
							// get the median
							*dst = g[c >> 1];
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
							c = 0;
			
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

										r[c] = p[RGB.R];
										g[c] = p[RGB.G];
										b[c] = p[RGB.B];
										c++;
									}
								}
							}

							// sort elements
							Array.Sort(r, 0, c);
							Array.Sort(g, 0, c);
							Array.Sort(b, 0, c);
							// get the median
							t = c >> 1;
							dst[RGB.R] = r[t];
							dst[RGB.G] = g[t];
							dst[RGB.B] = b[t];
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
