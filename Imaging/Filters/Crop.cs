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
	/// Crop an image
	/// </summary>
	public class Crop : IFilter
	{
		private Rectangle rect;

		// Rectangle property
		public Rectangle Rectangle
		{
			get { return rect; }
			set { rect = value; }
		}

		// Constructor
		public Crop()
		{
			this.rect = new Rectangle(0, 0, 100, 100);
		}
		public Crop(Rectangle rect)
		{
			this.rect = rect;
		}

		// Apply filter
		public Bitmap Apply(Bitmap srcImg)
		{
			// get source image size
			int width = srcImg.Width;
			int height = srcImg.Height;

			// destination image dimension
			int xmin = Math.Max(0, Math.Min(width - 1, rect.Left));
			int ymin = Math.Max(0, Math.Min(height - 1, rect.Top));
			int xmax = Math.Min(width - 1, xmin + rect.Width - 1 + ((rect.Left < 0) ? rect.Left : 0));
			int ymax = Math.Min(height - 1, ymin + rect.Height - 1 + ((rect.Top < 0) ? rect.Top : 0));

			int dstWidth = xmax - xmin + 1;
			int dstHeight = ymax - ymin + 1;

			// pixel format
			PixelFormat fmt = (srcImg.PixelFormat == PixelFormat.Format8bppIndexed) ?
				PixelFormat.Format8bppIndexed : PixelFormat.Format24bppRgb;

			// lock source bitmap data
			BitmapData srcData = srcImg.LockBits(
				new Rectangle(0, 0, width, height),
				ImageLockMode.ReadOnly, fmt);

			// create new image
			Bitmap dstImg = (fmt == PixelFormat.Format8bppIndexed) ?
				AForge.Imaging.Image.CreateGrayscaleImage(dstWidth, dstHeight) :
				new Bitmap(dstWidth, dstHeight, fmt);

			// lock destination bitmap data
			BitmapData dstData = dstImg.LockBits(
				new Rectangle(0, 0, dstWidth, dstHeight),
				ImageLockMode.ReadWrite, fmt);

			int srcStride = srcData.Stride;
			int dstStride = dstData.Stride;
			int pixelSize = (fmt == PixelFormat.Format8bppIndexed) ? 1 : 3;
			int copySize = dstWidth * pixelSize;

			// do the job
			unsafe
			{
				byte * src = (byte *) srcData.Scan0.ToPointer() + ymin * srcStride + xmin * pixelSize;
				byte * dst = (byte *) dstData.Scan0.ToPointer();

				// for each line
				for (int y = ymin; y <= ymax; y++)
				{
					Win32.memcpy(dst, src, copySize);
					src += srcStride;
					dst += dstStride;
				}
			}
			// unlock both images
			dstImg.UnlockBits(dstData);
			srcImg.UnlockBits(srcData);

			return dstImg;
		}
	}
}
