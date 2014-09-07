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

	// Hit & Miss modes
	public enum HitAndMissMode
	{
		HitAndMiss = 0,
		Thinning = 1,
		Thickening = 2
	}

	/// <summary>
	/// Hit-And-Miss operator from Mathematical Morphology
	///
	/// Structuring element contains:
	///  1 - foreground
	///  0 - background
	/// -1 - don't care
	/// </summary>
	public class HitAndMiss : IFilter
	{
		// structuring element
		private short[,]	se;
		private int			size;
		private HitAndMissMode mode = HitAndMissMode.HitAndMiss;

		// Mode property
		public HitAndMissMode Mode
		{
			get { return mode; }
			set { mode = value; }
		}

		// Constructor
		public HitAndMiss(short[,] se)
		{
			int s = se.GetLength(0);

			// check structuring element size
			if ((s != se.GetLength(1)) || (s < 3) || (s > 25) || (s % 2 == 0))
				throw new ArgumentException();

			this.se = se;
			this.size = s;
		}
		public HitAndMiss(short[,] se, HitAndMissMode mode) : this(se)
		{
			this.mode = mode;
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
			int ir, jr, i, j, r = size >> 1;
			byte dstValue, v;
			short sv;

			byte[] hitValue = new byte[3] {255, 0, 255};
			byte[] missValue = new byte[3] {0, 0, 0};
			int modeIndex = (int) mode;

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
						missValue[1] = missValue[2] = *src;
						dstValue = 255;

						// for each SE row
						for (i = 0; i < size; i++)
						{
							ir = i - r;

							// for each SE column
							for (j = 0; j < size; j++)
							{
								jr = j - r;

								// get structuring element's value
								sv = se[i, j];

								// skip "don't care" values
								if (sv == -1)
									continue;

								// check, if we outside
								if ((y + ir < 0) || (y + ir >= height) ||
									(x + jr < 0) || (x + jr >= width))
								{
									// if it so, the result is zero,
									// because it was required pixel
									dstValue = 0;
									break;
								}

								// get source image value
								v = src[ir * stride + jr];

								if (((sv != 0) || (v != 0)) &&
									((sv != 1) || (v != 255)))
								{
									// failed structuring element mutch
									dstValue = 0;
									break;
								}
							}

							if (dstValue == 0)
								break;
						}
						// result pixel
						*dst = (dstValue == 255) ? hitValue[modeIndex] : missValue[modeIndex];
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
