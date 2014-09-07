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
	/// MaskedFilter - filter an image using binary mask
	/// </summary>
	public class MaskedFilter : IFilter
	{
		private Bitmap	mask;
		private IFilter	filter1 = null;
		private IFilter	filter2 = null;

		// Mask property
		public Bitmap Mask
		{
			get { return mask; }
			set { mask = value; }
		}
		// Filter1 property
		public IFilter Filter1
		{
			get { return filter1; }
			set { filter1 = value; }
		}
		// Filter1 property
		public IFilter Filter2
		{
			get { return filter2; }
			set { filter2 = value; }
		}

		// Constructor
		public MaskedFilter()
		{
		}
		public MaskedFilter(Bitmap mask, IFilter filter1)
		{
			this.mask = mask;
			this.filter1 = filter1;
		}
		public MaskedFilter(Bitmap mask, IFilter filter1, IFilter filter2)
		{
			this.mask = mask;
			this.filter1 = filter1;
			this.filter2 = filter2;
		}

		// Apply filter using mask
		// filter1 is applied to all black regions of the mask
		// filter2 is applied to all white regions of the mask
		//
		public Bitmap Apply(Bitmap srcImg)
		{
			int width = mask.Width;
			int	height = mask.Height;

			// check source size
			if ((width != srcImg.Width) || (height != srcImg.Height))
			{
				// sorry, but source image must have the same dimension as mask image
				throw new ArgumentException();
			}

			Bitmap	dstImg = filter1.Apply(srcImg);
			bool	disposeSrc = false;

			// check destination size
			if ((width != dstImg.Width) || (height != dstImg.Height))
			{
				dstImg.Dispose();
				// we are not handling such situations yet
				throw new ApplicationException();
			}

			// apply filter2 also, if it is
			if (filter2 != null)
			{
				srcImg = filter2.Apply(srcImg);
				disposeSrc = true;

				// check source size
				if ((width != srcImg.Width) || (height != srcImg.Height))
				{
					srcImg.Dispose();
					dstImg.Dispose();
					// we are not handling such situations yet
					throw new ApplicationException();
				}
			}

			// check pixel formats
			if (dstImg.PixelFormat != srcImg.PixelFormat)
			{
				IFilter f = new GrayscaleToRGB();

				// convert temp image to RGB format
				if (dstImg.PixelFormat == PixelFormat.Format8bppIndexed)
				{
					Bitmap t = f.Apply(dstImg);
					dstImg.Dispose();
					dstImg = t;
				}
				// convert source image to RGB format
				if (srcImg.PixelFormat == PixelFormat.Format8bppIndexed)
				{
					Bitmap t = f.Apply(srcImg);
					if (disposeSrc)
						srcImg.Dispose();
					srcImg = t;
					disposeSrc = true;
				}
			}

			// lock source bitmap data
			BitmapData srcData = srcImg.LockBits(
				new Rectangle(0, 0, width, height),
				ImageLockMode.ReadOnly, srcImg.PixelFormat);

			// lock destination bitmap data
			BitmapData dstData = dstImg.LockBits(
				new Rectangle(0, 0, width, height),
				ImageLockMode.ReadWrite, dstImg.PixelFormat);

			// lock mask bitmap data
			BitmapData maskData = mask.LockBits(
				new Rectangle(0, 0, width, height),
				ImageLockMode.ReadOnly, mask.PixelFormat);

			int pixelSize = (dstImg.PixelFormat == PixelFormat.Format8bppIndexed) ? 1 : 3;
			int offset = dstData.Stride - width * pixelSize;
			int maskInc = (mask.PixelFormat == PixelFormat.Format8bppIndexed) ? 1 : 3;
			int maskOffset = maskData.Stride - width * maskInc;

			// do the job
			unsafe
			{
				byte * src = (byte *) srcData.Scan0.ToPointer();
				byte * dst = (byte *) dstData.Scan0.ToPointer();
				byte * m = (byte *) maskData.Scan0.ToPointer();

				// for each line
				for (int y = 0; y < height; y++)
				{
					// for each pixel
					for (int x = 0; x < width; x++, m += maskInc)
					{
						if (*m != 0)
						{
							for (int i = 0; i < pixelSize; i++, src++, dst++ )
							{
								*dst = *src;
							}
						}
						else
						{
							src += pixelSize;
							dst += pixelSize;
						}
					}
					src += offset;
					dst += offset;
					m += maskOffset;
				}
			}

			// unlock all images
			dstImg.UnlockBits(dstData);
			srcImg.UnlockBits(srcData);
			mask.UnlockBits(maskData);

			// dispose source ?
			if (disposeSrc)
				srcImg.Dispose();

			// return result
			return dstImg;
		}
	}
}
