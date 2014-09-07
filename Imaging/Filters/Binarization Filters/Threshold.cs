// AForge Image Processing Library
//
// Copyright � Andrew Kirillov, 2005
// andrew.kirillov@gmail.com
//

namespace AForge.Imaging.Filters
{
	using System;
	using System.Drawing;
	using System.Drawing.Imaging;

	/// <summary>
	/// Threshold binarization
	/// </summary>
	public class Threshold : IFilter
	{
		byte min = 128;
		byte max = 255;

		// Min property
		public byte Min
		{
			get { return min; }
			set { min = value; }
		}
		// Max property
		public byte Max
		{
			get { return max; }
			set { max = value; }
		}

		// Constructors
		public Threshold()
		{
		}
		public Threshold(byte min, byte max)
		{
			this.min = Math.Min(min, max);
			this.max = Math.Max(min, max);
		}
	
		// Apply filter
		public Bitmap Apply(Bitmap srcImg)
		{
			// get source image size
			int width = srcImg.Width;
			int height = srcImg.Height;

			PixelFormat srcFmt = (srcImg.PixelFormat == PixelFormat.Format8bppIndexed) ?
				PixelFormat.Format8bppIndexed : PixelFormat.Format24bppRgb;

			// lock source bitmap data
			BitmapData srcData = srcImg.LockBits(
				new Rectangle(0, 0, width, height),
				ImageLockMode.ReadOnly, srcFmt);

			// create new grayscale image
			Bitmap dstImg = AForge.Imaging.Image.CreateGrayscaleImage(width, height);

			// lock destination bitmap data
			BitmapData dstData = dstImg.LockBits(
				new Rectangle(0, 0, width, height),
				ImageLockMode.ReadWrite, PixelFormat.Format8bppIndexed);

			int srcOffset = srcData.Stride - ((srcFmt == PixelFormat.Format8bppIndexed) ? width : width * 3);
			int dstOffset = dstData.Stride - width;

			// do the job
			unsafe
			{
				byte * src = (byte *) srcData.Scan0.ToPointer();
				byte * dst = (byte *) dstData.Scan0.ToPointer();

				if (srcFmt == PixelFormat.Format8bppIndexed)
				{
					// graysclae binarization
					for (int y = 0; y < height; y++)
					{
						for (int x = 0; x < width; x++, src ++, dst ++)
						{
							*dst = (byte)(((*src >= min) && (*src <= max)) ? 255 : 0);
						}
						src += srcOffset;
						dst += dstOffset;
					}
				}
				else
				{
					byte v;

					// RGB binarization
					for (int y = 0; y < height; y++)
					{
						for (int x = 0; x < width; x++, src += 3, dst ++)
						{
							// grayscale value using BT709
							v = (byte)(0.2125f * src[RGB.R] + 0.7154f * src[RGB.G] + 0.0721f * src[RGB.B]);

							*dst = (byte)(((v >= min) && (v <= max)) ? 255 : 0);
						}
						src += srcOffset;
						dst += dstOffset;
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
