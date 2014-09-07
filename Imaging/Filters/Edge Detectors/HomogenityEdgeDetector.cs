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
	/// Homogenity edge detector
	/// </summary>
	public class HomogenityEdgeDetector : IFilter
	{
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
			int	widthM1 = width - 1;
			int heightM1 = height - 1;
			int d, max, v;

			// do the job
			unsafe
			{
				byte * src = (byte *) srcData.Scan0.ToPointer();
				byte * dst = (byte *) dstData.Scan0.ToPointer();

				// skip one stride
				src += stride;
				dst += stride;

				// for each line
				for (int y = 1; y < heightM1; y++)
				{
					src ++;
					dst ++;

					// for each pixel
					for (int x = 1; x <	widthM1; x++, src ++, dst ++)
					{
						max = 0;
						v = *src;

						// top-left
						d = v - src[-stride - 1];
						if (d < 0)
							d = -d;
						if (d > max)
							max = d;
						// top
						d = v - src[-stride];
						if (d < 0)
							d = -d;
						if (d > max)
							max = d;
						// top-right
						d = v - src[-stride + 1];
						if (d < 0)
							d = -d;
						if (d > max)
							max = d;
						// left
						d = v - src[-1];
						if (d < 0)
							d = -d;
						if (d > max)
							max = d;
						// right
						d = v - src[1];
						if (d < 0)
							d = -d;
						if (d > max)
							max = d;
						// bottom-left
						d = v - src[stride - 1];
						if (d < 0)
							d = -d;
						if (d > max)
							max = d;
						// bottom
						d = v - src[stride];
						if (d < 0)
							d = -d;
						if (d > max)
							max = d;
						// bottom-right
						d = v - src[stride + 1];
						if (d < 0)
							d = -d;
						if (d > max)
							max = d;

						*dst = (byte) max;
					}
					src += offset + 1;
					dst += offset + 1;
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
