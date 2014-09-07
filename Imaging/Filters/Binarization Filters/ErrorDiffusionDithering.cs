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
	/// Base class for error diffusion dithering
	/// </summary>
	public abstract class ErrorDiffusionDithering : IFilter
	{
		protected int	x, y;				// current position
		protected int	width, height;		// width & height
		protected int	widthM1, heightM1;	// width - 1, height - 1
		protected int	stride;				// line size

		// Diffuse error
		protected abstract unsafe void Diffuse(int error, byte * ptr);

		// Apply filter
		public Bitmap Apply(Bitmap srcImg)
		{
			// get source image size
			width = srcImg.Width;
			height = srcImg.Height;

			// temp bitmap
			Bitmap tmpImg;

			if (srcImg.PixelFormat == PixelFormat.Format8bppIndexed)
			{
				// clone bitmap
				tmpImg = AForge.Imaging.Image.Clone(srcImg);
			}
			else
			{
				// create grayscale image
				IFilter filter = new GrayscaleRMY();
				tmpImg = filter.Apply(srcImg);
			}

			// lock temp bitmap data
			BitmapData tmpData = tmpImg.LockBits(
				new Rectangle(0, 0, width, height),
				ImageLockMode.ReadOnly, PixelFormat.Format8bppIndexed);

			// create new grayscale image
			Bitmap dstImg = AForge.Imaging.Image.CreateGrayscaleImage(width, height);

			// lock destination bitmap data
			BitmapData dstData = dstImg.LockBits(
				new Rectangle(0, 0, width, height),
				ImageLockMode.ReadWrite, PixelFormat.Format8bppIndexed);

			stride		= dstData.Stride;
			widthM1		= width - 1;
			heightM1	= height - 1;

			int	offset = stride - width;
			int	v, e;

			// do the job
			unsafe
			{
				byte * src = (byte *) tmpData.Scan0.ToPointer();
				byte * dst = (byte *) dstData.Scan0.ToPointer();

				// for each line
				for (y = 0; y < height; y++)
				{
					// for each pixels
					for (x = 0; x < width; x++, src ++, dst ++)
					{
						v = *src;

						// fill the next destination pixel
						if (v < 128)
						{
							*dst = 0;
							e = v;
						}
						else
						{
							*dst = 255;
							e = v - 255;
						}

						Diffuse(e, src);
					}
					src += offset;
					dst += offset;
				}
			}
			// unlock both images
			dstImg.UnlockBits(dstData);
			tmpImg.UnlockBits(tmpData);

			// dispose temp bitmap
			tmpImg.Dispose();

			return dstImg;
		}
	}
}
