// AForge Image Processing Library
//
// Copyright © Andrew Kirillov, 2005
// andrew.kirillov@gmail.com
//
// Original idea from CxImage
// http://www.codeproject.com/bitmap/cximage.asp
//

namespace AForge.Imaging.Filters
{
	using System;
	using System.Drawing;
	using System.Drawing.Imaging;

	/// <summary>
	/// Jitter filter
	/// </summary>
	public class Jitter : IFilter
	{
		private int	radius = 2;
		private bool copyBefore = true;

		// Radius property
		public int Radius
		{
			get { return radius; }
			set { radius = Math.Max(1, Math.Min(10, value)); }
		}
		// CopyBefore property
		public bool CopyBefore
		{
			get { return copyBefore; }
			set { copyBefore = value; }
		}

		// Constructor
		public Jitter()
		{
		}
		public Jitter(int radius)
		{
			Radius = radius;
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

			// create new image
			Bitmap dstImg = (fmt == PixelFormat.Format8bppIndexed) ?
				AForge.Imaging.Image.CreateGrayscaleImage(width, height) :
				new Bitmap(width, height, fmt);

			// lock destination bitmap data
			BitmapData dstData = dstImg.LockBits(
				new Rectangle(0, 0, width, height),
				ImageLockMode.ReadWrite, fmt);

			int pixelSize = (fmt == PixelFormat.Format8bppIndexed) ? 1 : 3;
			int stride = srcData.Stride;
			int offset = stride - width * pixelSize;
			int ox, oy;

			int max = radius * 2 + 1;
			Random rnd = new Random();

			// do the job
			unsafe
			{
				byte * src = (byte *) srcData.Scan0.ToPointer();
				byte * dst = (byte *) dstData.Scan0.ToPointer();
				byte * p;

				if (copyBefore)
					Win32.memcpy(dst, src, stride * height);

				// for each line
				for (int y = 0; y < height; y++)
				{
					// for each pixel
					for (int x = 0; x < width; x++)
					{
						ox = x + rnd.Next(max) - radius;
						oy = y + rnd.Next(max) - radius;

						// check if the random pixel is inside our image
						if ((ox >= 0) && (oy >= 0) && (ox < width) && (oy < height))
						{
							p = src + oy * stride + ox * pixelSize;

							for (int i = 0; i < pixelSize; i++, dst++)
							{
								*dst = p[i];
							}
						}
						else
						{
							dst += pixelSize;
						}
					}
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
