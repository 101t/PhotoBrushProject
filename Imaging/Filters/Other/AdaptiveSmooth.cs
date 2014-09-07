// AForge Image Processing Library
//
// Copyright © Andrew Kirillov, 2005
// andrew.kirillov@gmail.com
//
// Found description in
// "An Edge Detection Technique Using the Facet
// Model and Parameterized Relaxation Labeling"
// by Ioannis Matalas, Student Member, IEEE, Ralph Benjamin, and Richard Kitney
//
namespace AForge.Imaging.Filters
{
	using System;
	using System.Drawing;
	using System.Drawing.Imaging;

	/// <summary>
	/// Adaptive Smoothing
	/// </summary>
	public class AdaptiveSmooth : IFilter
	{
		private float factor = 3.0f;

		// Factor property
		public float Factor
		{
			get { return factor; }
			set { factor = value; }
		}

		// Constructor
		public AdaptiveSmooth()
		{
		}
		public AdaptiveSmooth(float factor)
		{
			this.factor = factor;
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

			int stride = srcData.Stride;
			int pixelSize = (fmt == PixelFormat.Format8bppIndexed) ? 1 : 3;
			int pixelSize2 = pixelSize * 2;
			int offset = stride - width * pixelSize;

			int widthM2 = width - 2;
			int heightM2 = height - 2;
			float f, gx, gy, weight, weightTotal, total;

			f = - 8 * factor * factor;

			// do the job
			unsafe
			{
				byte * src = (byte *) srcData.Scan0.ToPointer() + stride * 2;
				byte * dst = (byte *) dstData.Scan0.ToPointer() + stride * 2;

				// grayscale
				for (int y = 2; y < heightM2; y++)
				{
					src += 2 * pixelSize;
					dst += 2 * pixelSize;

					for (int x = 2; x < widthM2; x++)
					{
						for (int i = 0; i < pixelSize; i++, src++, dst++)
						{
							weightTotal	= 0;
							total		= 0;

							// original formulas for weight calculation:
							// w(x, y) = exp( -1 * (Gx^2 + Gy^2) / (2 * factor^2) )
							// Gx(x, y) = (I(x + 1, y) - I(x - 1, y)) / 2
							// Gy(x, y) = (I(x, y + 1) - I(x, y - 1)) / 2
							//
							// here is a little bit optimized version

							// x - 1, y - 1
							gx		= src[- stride] - src[- pixelSize2 - stride];
							gy		= src[- pixelSize] - src[- pixelSize - 2 * stride];
							weight	= (float) System.Math.Exp((gx * gx + gy * gy) / f);
							total	+= weight * src[- pixelSize - stride];
							weightTotal += weight;

							// x, y - 1
							gx		= src[pixelSize - stride] - src[- pixelSize - stride];
							gy		= *src - src[- 2 * stride];
							weight	= (float) System.Math.Exp((gx * gx + gy * gy) / f);
							total	+= weight * src[- stride];
							weightTotal += weight;

							// x + 1, y - 1
							gx		= src[pixelSize2 - stride] - src[- stride];
							gy		= src[pixelSize] - src[pixelSize - 2 * stride];
							weight	= (float) System.Math.Exp((gx * gx + gy * gy) / f);
							total	+= weight * src[pixelSize - stride];
							weightTotal += weight;

							// x - 1, y
							gx		= *src - src[- pixelSize2];
							gy		= src[- pixelSize + stride] - src[- pixelSize - stride];
							weight	= (float) System.Math.Exp((gx * gx + gy * gy) / f);
							total	+= weight * src[- pixelSize];
							weightTotal += weight;

							// x, y
							gx		= src[pixelSize] - src[- pixelSize];
							gy		= src[stride] - src[- stride];
							weight	= (float) System.Math.Exp((gx * gx + gy * gy) / f);
							total	+= weight * (*src);
							weightTotal += weight;

							// x + 1, y
							gx		= src[pixelSize2] - *src;
							gy		= src[pixelSize + stride] - src[pixelSize - stride];
							weight	= (float) System.Math.Exp((gx * gx + gy * gy) / f);
							total	+= weight * src[pixelSize];
							weightTotal += weight;

							// x - 1, y + 1
							gx		= src[stride] - src[- pixelSize2 + stride];
							gy		= src[- pixelSize + 2 * stride] - src[- pixelSize];
							weight	= (float) System.Math.Exp((gx * gx + gy * gy) / f);
							total	+= weight * src[-pixelSize + stride];
							weightTotal += weight;

							// x, y + 1
							gx		= src[pixelSize + stride] - src[- pixelSize + stride];
							gy		= src[2 * stride] - *src;
							weight	= (float) System.Math.Exp((gx * gx + gy * gy) / f);
							total	+= weight * src[stride];
							weightTotal += weight;

							// x + 1, y + 1
							gx		= src[pixelSize2 + stride] - src[stride];
							gy		= src[pixelSize + 2 * stride] - src[pixelSize];
							weight	= (float) System.Math.Exp((gx * gx + gy * gy) / f);
							total	+= weight * src[pixelSize + stride];
							weightTotal += weight;

							// save destination value
							*dst = (weightTotal == 0.0) ? *src : (byte) System.Math.Min(total / weightTotal, 255.0f);
						}
					}
					src += offset + 2 * pixelSize;
					dst += offset + 2 * pixelSize;
				}
			}
			// unlock both images
			dstImg.UnlockBits(dstData);
			srcImg.UnlockBits(srcData);

			return dstImg;
		}
	}
}
