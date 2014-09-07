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
	/// Resize an image
	/// </summary>
	public class Resize : IFilter
	{
		private InterpolationMethod method = InterpolationMethod.Bilinear;
		private int	newWidth = 0;
		private int newHeight = 0;

		// NewWidth property
		public int NewWidth
		{
			get { return newWidth; }
			set { newWidth = Math.Max(1, Math.Min(5000, value)); }
		}
		// NewHeight property
		public int NewHeight
		{
			get { return newHeight; }
			set { newHeight = Math.Max(1, Math.Min(5000, value)); }
		}
		// Method property
		public InterpolationMethod Method
		{
			get { return method; }
			set { method = value; }
		}

		// Constructor
		public Resize()
		{
		}
		public Resize(int newWidth, int newHeight)
		{
			this.newWidth = newWidth;
			this.newHeight = newHeight;
		}
		public Resize(int newWidth, int newHeight, InterpolationMethod method)
		{
			this.newWidth = newWidth;
			this.newHeight = newHeight;
			this.method = method;
		}

		// Apply filter
		public Bitmap Apply(Bitmap srcImg)
		{
			// get source image size
			int width = srcImg.Width;
			int height = srcImg.Height;

			if ((newWidth == width) && (newHeight == height))
			{
				// just clone the image
				return AForge.Imaging.Image.Clone(srcImg);
			}

			PixelFormat fmt = (srcImg.PixelFormat == PixelFormat.Format8bppIndexed) ?
				PixelFormat.Format8bppIndexed : PixelFormat.Format24bppRgb;

			// lock source bitmap data
			BitmapData srcData = srcImg.LockBits(
				new Rectangle(0, 0, width, height),
				ImageLockMode.ReadOnly, fmt);

			// create new image
			Bitmap dstImg = (fmt == PixelFormat.Format8bppIndexed) ?
				AForge.Imaging.Image.CreateGrayscaleImage(newWidth, newHeight) :
				new Bitmap(newWidth, newHeight, fmt);

			// lock destination bitmap data
			BitmapData dstData = dstImg.LockBits(
				new Rectangle(0, 0, newWidth, newHeight),
				ImageLockMode.ReadWrite, fmt);

			int pixelSize = (fmt == PixelFormat.Format8bppIndexed) ? 1 : 3;
			int srcStride = srcData.Stride;
			int dstOffset = dstData.Stride - pixelSize * newWidth;
			float xFactor = (float) width / newWidth;
			float yFactor = (float) height / newHeight;

			// do the job
			unsafe
			{
				byte * src = (byte *) srcData.Scan0.ToPointer();
				byte * dst = (byte *) dstData.Scan0.ToPointer();

				switch (method)
				{
					case InterpolationMethod.NearestNeighbor:
					{
						// -------------------------------------------
						// resize using nearest neighbor interpolation
						// -------------------------------------------

						int		ox, oy;
						byte *	p;

						// for each line
						for (int y = 0; y < newHeight; y++)
						{
							// Y coordinate of the nearest point
							oy = (int) (y * yFactor);

							// for each pixel
							for (int x = 0; x < newWidth; x++)
							{
								// X coordinate of the nearest point
								ox = (int) (x * xFactor);

								p = src + oy * srcStride + ox * pixelSize;

								for (int i = 0; i < pixelSize; i++, dst++, p++)
								{
									*dst = *p;
								}
							}
							dst += dstOffset;
						}
						break;
					}

					case InterpolationMethod.Bilinear:
					{
						// ------------------------------------
						// resize using  bilinear interpolation
						// ------------------------------------

						float	ox, oy, dx1, dy1, dx2, dy2;
						int		ox1, oy1, ox2, oy2;
						int		ymax = height - 1;
						int		xmax = width - 1;
						byte	v1, v2;
						byte *	tp1, tp2;

						byte *	p1, p2, p3, p4;

						// for each line
						for (int y = 0; y < newHeight; y++)
						{
							// Y coordinates
							oy	= (float) y * yFactor;
							oy1	= (int) oy;
							oy2	= (oy1 == ymax) ? oy1 : oy1 + 1;
							dy1	= oy - (float) oy1;
							dy2 = 1.0f - dy1;

							// get temp pointers
							tp1 = src + oy1 * srcStride;
							tp2 = src + oy2 * srcStride;

							// for each pixel
							for (int x = 0; x < newWidth; x++)
							{
								// X coordinates
								ox	= (float) x * xFactor;
								ox1	= (int) ox;
								ox2	= (ox1 == xmax) ? ox1 : ox1 + 1;
								dx1	= ox - (float) ox1;
								dx2	= 1.0f - dx1;

								// get four points
								p1 = tp1 + ox1 * pixelSize;
								p2 = tp1 + ox2 * pixelSize;
								p3 = tp2 + ox1 * pixelSize;
								p4 = tp2 + ox2 * pixelSize;

								// interpolate using 4 points
								for (int i = 0; i < pixelSize; i++, dst++, p1++, p2++, p3++, p4++)
								{
									v1 = (byte)(dx2 * (*p1) + dx1 * (*p2));
									v2 = (byte)(dx2 * (*p3) + dx1 * (*p4));
									*dst = (byte)(dy2 * v1 + dy1 * v2);
								}
							}
							dst += dstOffset;
						}
						break;
					}

					case InterpolationMethod.Bicubic:
					{
						// ----------------------------------
						// resize using bicubic interpolation
						// ----------------------------------

						float	ox, oy, dx, dy, k1, k2;
						float	r, g, b;
						int		ox1, oy1, ox2, oy2;
						int		ymax = height - 1;
						int		xmax = width - 1;
						byte *	p;

						if (fmt == PixelFormat.Format8bppIndexed)
						{
							// grayscale
							for (int y = 0; y < newHeight; y++)
							{
								// Y coordinates
								oy	= (float) y * yFactor - 0.5f;
								oy1	= (int) oy;
								dy	= oy - (float) oy1;

								for (int x = 0; x < newWidth; x++, dst ++)
								{
									// X coordinates
									ox	= (float) x * xFactor - 0.5f;
									ox1	= (int) ox;
									dx	= ox - (float) ox1;

									g = 0;

									for (int n = -1; n < 3; n++) 
									{
										k1 = Interpolation.BiCubicKernel(dy - (float) n);

										oy2 = oy1 + n;
										if (oy2 < 0)
											oy2 = 0;
										if (oy2 > ymax)
											oy2 = ymax;

										for (int m = -1; m < 3; m++) 
										{
											k2 = k1 * Interpolation.BiCubicKernel((float) m - dx);

											ox2 = ox1 + m;
											if (ox2 < 0)
												ox2 = 0;
											if (ox2 > xmax)
												ox2 = xmax;

											g += k2 * src[oy2 * srcStride + ox2];
										}
									}
									*dst = (byte) g;
								}
								dst += dstOffset;
							}
						}
						else
						{
							// RGB
							for (int y = 0; y < newHeight; y++)
							{
								// Y coordinates
								oy	= (float) y * yFactor - 0.5f;
								oy1	= (int) oy;
								dy	= oy - (float) oy1;

								for (int x = 0; x < newWidth; x++, dst += 3)
								{
									// X coordinates
									ox	= (float) x * xFactor - 0.5f;
									ox1	= (int) ox;
									dx	= ox - (float) ox1;

									r = g = b = 0;

									for (int n = -1; n < 3; n++) 
									{
										k1 = Interpolation.BiCubicKernel(dy - (float) n);

										oy2 = oy1 + n;
										if (oy2 < 0)
											oy2 = 0;
										if (oy2 > ymax)
											oy2 = ymax;

										for (int m = -1; m < 3; m++) 
										{
											k2 = k1 * Interpolation.BiCubicKernel((float) m - dx);

											ox2 = ox1 + m;
											if (ox2 < 0)
												ox2 = 0;
											if (ox2 > xmax)
												ox2 = xmax;

											// get pixel of original image
											p = src + oy2 * srcStride + ox2 * 3;

											r += k2 * p[RGB.R];
											g += k2 * p[RGB.G];
											b += k2 * p[RGB.B];
										}
									}

									dst[RGB.R] = (byte) r;
									dst[RGB.G] = (byte) g;
									dst[RGB.B] = (byte) b;
								}
								dst += dstOffset;
							}
						}
						break;
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
