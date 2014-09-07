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
	/// SimpleSkeletonization filter
	/// </summary>
	public class SimpleSkeletonization : IFilter
	{
		private byte	bg = 0;
		private byte	fg = 255;

		// Background property
		public byte Background
		{
			get { return bg; }
			set { bg = value; }
		}
		// Foreground property
		public byte Foreground
		{
			get { return fg; }
			set { fg = value; }
		}

		// Cinstructor
		public SimpleSkeletonization()
		{
		}
		public SimpleSkeletonization(byte bg, byte fg)
		{
			this.bg = bg;
			this.fg = fg;
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
			int start;

			// make destination image filled with background color
			Win32.memset(dstData.Scan0, bg, stride * height);

			// do the job
			unsafe
			{
				byte * src0 = (byte *) srcData.Scan0.ToPointer();
				byte * dst0 = (byte *) dstData.Scan0.ToPointer();
				byte * src = src0;
				byte * dst = dst0;

				// horizontal pass

				// for each line
				for (int y = 0; y < height; y++)
				{
					start = -1;
					// for each pixel
					for (int x = 0; x < width; x++, src ++)
					{
						// looking for foreground pixel
						if (start == -1)
						{
							if (*src == fg)
								start = x;
							continue;
						}

						// looking for non black pixel (white)
						if (*src != fg)
						{
							dst[start + ((x - start) >> 1)] = (byte) fg;
							start = -1;
						}
					}
					if (start != -1)
					{
						dst[start + ((width - start) >> 1)] = (byte) fg;
					}
					src += offset;
					dst += stride;
				}

				// vertical pass

				// for each column
				for (int x = 0; x < width; x++)
				{
					src = src0 + x;
					dst = dst0 + x;

					start = -1;
					// for each row
					for (int y = 0; y < height; y++, src += stride)
					{
						// looking for foreground pixel
						if (start == -1)
						{
							if (*src == fg)
								start = y;
							continue;
						}

						// looking for non black pixel (white)
						if (*src != fg)
						{
							dst[stride * (start + ((y - start) >> 1))] = (byte) fg;
							start = -1;
						}
					}
					if (start != -1)
					{
						dst[stride * (start + ((height - start) >> 1))] = (byte) fg;
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
