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
	/// Base class for image grayscaling
	/// </summary>
	public abstract class Grayscale : IFilter
	{
		// RGB coefficients for grayscale transformation
		private float	cr;
		private float	cg;
		private float	cb;

		// Constructor
		public Grayscale(float cr, float cg, float cb)
		{
			this.cr = cr;
			this.cg = cg;
			this.cb = cb;
		}

		// Apply filter
		public Bitmap Apply(Bitmap srcImg)
		{
			if (srcImg.PixelFormat != PixelFormat.Format24bppRgb)
				throw new ArgumentException();

			// get source image size
			int width = srcImg.Width;
			int height = srcImg.Height;

			// lock source bitmap data
			BitmapData srcData = srcImg.LockBits(
				new Rectangle(0, 0, width, height),
				ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);

			// create new grayscale image
			Bitmap dstImg = AForge.Imaging.Image.CreateGrayscaleImage(width, height);

			// lock destination bitmap data
			BitmapData dstData = dstImg.LockBits(
				new Rectangle(0, 0, width, height),
				ImageLockMode.ReadWrite, PixelFormat.Format8bppIndexed);

			int srcOffset = srcData.Stride - width * 3;
			int dstOffset = dstData.Stride - width;

			// do the job
			unsafe
			{
				byte * src = (byte *) srcData.Scan0.ToPointer();
				byte * dst = (byte *) dstData.Scan0.ToPointer();

				// for each line
				for (int y = 0; y < height; y++)
				{
					// for each pixel
					for (int x = 0; x < width; x++, src += 3, dst ++)
					{
						*dst = (byte)(cr * src[RGB.R] + cg * src[RGB.G] + cb * src[RGB.B]);
					}
					src += srcOffset;
					dst += dstOffset;
				}
			}
			// unlock both images
			dstImg.UnlockBits(dstData);
			srcImg.UnlockBits(srcData);

			return dstImg;
		}
	}
}
