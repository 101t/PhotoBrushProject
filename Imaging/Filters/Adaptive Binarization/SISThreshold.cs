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
	/// Threshold using Simple Image Statistics (SIS)
	/// </summary>
	public class SISThreshold : IFilter
	{
		private byte threshold;

		// Threshold property
		public byte Threshold
		{
			get { return threshold; }
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

			int widthM1 = width - 1;
			int heightM1 = height - 1;
			double ex, ey, weight, weightTotal = 0, total = 0;

			// do the job
			unsafe
			{
				byte * src = (byte *) srcData.Scan0.ToPointer() + stride;
				byte * dst = (byte *) dstData.Scan0.ToPointer();

				// --- 1st pass - collecting statistics

				// for each line
				for (int y = 1; y < heightM1; y++)
				{
					src++;
					// for each pixels
					for (int x = 1; x < widthM1; x++, src++)
					{
						// the equations are:
						// ex = I(x + 1, y) - I(x - 1, y)
						// ey = I(x, y + 1) - I(x, y - 1)
						// weight = max(ex, ey)
						// weightTotal += weight
						// total += weight * I(x, y)
					
						ex = src[1] - src[-1];
						ey = src[stride] - src[-stride];
						weight = (ex > ey) ? ex : ey;
						weightTotal += weight;
						total += weight * (*src);
					}
					src += offset + 1;
				}

				// calculate threshold
				threshold = (weightTotal == 0) ? (byte) 0 : (byte) (total / weightTotal);

				// --- 2nd pass - thresholding
				src = (byte *) srcData.Scan0.ToPointer();

				// for each line
				for (int y = 0; y < height; y++)
				{
					// for all pixels
					for (int x = 0; x < width; x++, src++, dst++)
					{
						*dst = (byte) ((*src <= threshold) ? 0 : 255);
					}
					src += offset;
					dst += offset;
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
