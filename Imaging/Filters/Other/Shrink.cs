// AForge Image Processing Library
//
// Copyright © Andrew Kirillov, 2005
// andrew.kirillov@gmail.com
//

namespace AForge.Imaging.Filters
{
	using System.Drawing;
	using System.Drawing.Imaging;

	/// <summary>
	/// Shrink an image - remove pixels with specified color from bounds
	/// </summary>
	public class Shrink : IFilter
	{
		private Color colorToRemove = Color.FromArgb(0, 0, 0);

		// ColorToRemove property
		public Color ColorToRemove
		{
			get { return colorToRemove; }
			set { colorToRemove = value; }
		}

		// Cinstructor
		public Shrink()
		{
		}
		public Shrink(Color colorToRemove)
		{
			this.colorToRemove = colorToRemove;
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

			int offset = srcData.Stride - ((fmt == PixelFormat.Format8bppIndexed) ? width : width * 3);
			byte r = colorToRemove.R;
			byte g = colorToRemove.G;
			byte b = colorToRemove.B;

			int minX = width;
			int minY = height;
			int maxX = 0;
			int maxY = 0;

			// do the job
			unsafe
			{
				byte * src = (byte *) srcData.Scan0.ToPointer();

				if (fmt == PixelFormat.Format8bppIndexed)
				{
					// grayscale
					for (int y = 0; y < height; y++)
					{
						for (int x = 0; x < width; x++, src ++)
						{
							if (*src != g)
							{
								if (x < minX)
									minX = x;
								if (x > maxX)
									maxX = x;
								if (y < minY)
									minY = y;
								if (y > maxY)
									maxY = y;
							}
						}
						src += offset;
					}
				}
				else
				{
					// RGB
					for (int y = 0; y < height; y++)
					{
						for (int x = 0; x < width; x++, src += 3)
						{
							if ((src[RGB.R] != r) ||
								(src[RGB.G] != g) ||
								(src[RGB.B] != b))
							{
								if (x < minX)
									minX = x;
								if (x > maxX)
									maxX = x;
								if (y < minY)
									minY = y;
								if (y > maxY)
									maxY = y;
							}
						}
						src += offset;
					}
				}
			}

			// check
			if ((minX == width) && (minY == height) && (maxX == 0) && (maxY == 0))
			{
				minX = minY = 0;
			}

			int newWidth = maxX - minX + 1;
			int newHeight = maxY - minY + 1;

			// create new image
			Bitmap dstImg = (fmt == PixelFormat.Format8bppIndexed) ?
				AForge.Imaging.Image.CreateGrayscaleImage(newWidth, newHeight) :
				new Bitmap(newWidth, newHeight, fmt);

			// lock destination bitmap data
			BitmapData dstData = dstImg.LockBits(
				new Rectangle(0, 0, newWidth, newHeight),
				ImageLockMode.ReadWrite, fmt);

			int dstStride = dstData.Stride;
			int srcStride = srcData.Stride;
			int copySize = newWidth;

			// do the job
			unsafe
			{
				byte * src = (byte *) srcData.Scan0.ToPointer();
				byte * dst = (byte *) dstData.Scan0.ToPointer();

				src += (minY * srcData.Stride);

				if (fmt == PixelFormat.Format8bppIndexed)
				{
					src += minX;
				}
				else
				{
					src += minX * 3;
					copySize *= 3;
				}

				// copy image
				for (int y = 0; y < newHeight; y++)
				{
					Win32.memcpy(dst, src, copySize);
					dst += dstStride;
					src += srcStride;
				}
			}

			// unlock both images
			dstImg.UnlockBits(dstData);
			srcImg.UnlockBits(srcData);

			return dstImg;
		}
	}
}
