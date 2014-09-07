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
	/// Rotate an image
	/// </summary>
	public class Rotate : IFilter
	{
		private InterpolationMethod method = InterpolationMethod.Bilinear;
		private float	angle;
		private bool	keepSize = false;
		private Color	fillColor = Color.FromArgb(0, 0, 0);

		// Angle property
		public float Angle
		{
			get { return angle; }
			set { angle = value % 360; }
		}
		// Method property
		public InterpolationMethod Method
		{
			get { return method; }
			set { method = value; }
		}
		// KeepSize property
		public bool KeepSize
		{
			get { return keepSize; }
			set { keepSize = value; }
		}
		// FillColor property
		public Color FillColor
		{
			get { return fillColor; }
			set { fillColor = value; }
		}

		// Constructor
		public Rotate()
		{
		}
		public Rotate(float angle)
		{
			this.angle = angle;
		}
		public Rotate(float angle, InterpolationMethod method)
		{
			this.angle = angle;
			this.method = method;
		}
		public Rotate(float angle, InterpolationMethod method, bool keepSize)
		{
			this.angle = angle;
			this.method = method;
			this.keepSize = keepSize;
		}

		// Apply filter
		public Bitmap Apply(Bitmap srcImg)
		{
			// get source image size
			int width = srcImg.Width;
			int height = srcImg.Height;

			if (angle == 0)
			{
				// just clone the image
				return AForge.Imaging.Image.Clone(srcImg);
			}

			double	angleRad = - angle * Math.PI / 180;
			double	angleCos = Math.Cos(angleRad);
			double	angleSin = Math.Sin(angleRad);

			double	halfWidth = (double) width / 2;
			double	halfHeight = (double) height / 2;

			double	halfNewWidth, halfNewHeight;
			int		newWidth, newHeight;

			if (keepSize)
			{
				halfNewWidth = halfWidth;
				halfNewHeight = halfHeight;

				newWidth = width;
				newHeight = height;
			}
			else
			{
				// rotate corners
				double	cx1 = halfWidth * angleCos;
				double	cy1 = halfWidth * angleSin;

				double	cx2 = halfWidth * angleCos - halfHeight * angleSin;
				double	cy2 = halfWidth * angleSin + halfHeight * angleCos;

				double	cx3 = - halfHeight * angleSin;
				double	cy3 =   halfHeight * angleCos;

				double	cx4 = 0;
				double	cy4 = 0;

				halfNewWidth = Math.Max(Math.Max(cx1, cx2), Math.Max(cx3, cx4)) - Math.Min(Math.Min(cx1, cx2), Math.Min(cx3, cx4));
				halfNewHeight = Math.Max(Math.Max(cy1, cy2), Math.Max(cy3, cy4)) - Math.Min(Math.Min(cy1, cy2), Math.Min(cy3, cy4));

				newWidth = (int)(halfNewWidth * 2 + 0.5);
				newHeight = (int)(halfNewHeight * 2 + 0.5);
			}

			// pixel format
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

			int srcStride = srcData.Stride;
			int dstOffset = dstData.Stride - ((fmt == PixelFormat.Format8bppIndexed) ? newWidth : newWidth * 3);

			byte	fillR = fillColor.R;
			byte	fillG = fillColor.G;
			byte	fillB = fillColor.B;

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
						// rotate using nearest neighbor interpolation
						// -------------------------------------------

						double	cx, cy;
						int		ox, oy;
						byte *	p;

						if (fmt == PixelFormat.Format8bppIndexed)
						{
							// grayscale
							cy = -halfNewHeight;
							for (int y = 0; y < newHeight; y++)
							{
								cx = -halfNewWidth;
								for (int x = 0; x < newWidth; x++, dst ++)
								{
									// coordinate of the nearest point
									ox = (int)( angleCos * cx + angleSin * cy + halfWidth);
									oy = (int)(-angleSin * cx + angleCos * cy + halfHeight);

									if ((ox < 0) || (oy < 0) || (ox >= width) || (oy >= height))
									{
										*dst = fillG;
									}
									else
									{
										*dst = src[oy * srcStride + ox];
									}
									cx ++;
								}
								cy ++;
								dst += dstOffset;
							}
						}
						else
						{
							// RGB
							cy = -halfNewHeight;
							for (int y = 0; y < newHeight; y++)
							{
								cx = -halfNewWidth;
								for (int x = 0; x < newWidth; x++, dst += 3)
								{
									// coordinate of the nearest point
									ox = (int)( angleCos * cx + angleSin * cy + halfWidth);
									oy = (int)(-angleSin * cx + angleCos * cy + halfHeight);

									if ((ox < 0) || (oy < 0) || (ox >= width) || (oy >= height))
									{
										dst[RGB.R] = fillR;
										dst[RGB.G] = fillG;
										dst[RGB.B] = fillB;
									}
									else
									{
										p = src + oy * srcStride + ox * 3;

										dst[RGB.R] = p[RGB.R];
										dst[RGB.G] = p[RGB.G];
										dst[RGB.B] = p[RGB.B];
									}
									cx ++;
								}
								cy ++;
								dst += dstOffset;
							}
						}
						break;
					}

					case InterpolationMethod.Bilinear:
					{
						// ------------------------------------
						// rotate using  bilinear interpolation
						// ------------------------------------

						double	cx, cy;
						float	ox, oy, dx1, dy1, dx2, dy2;
						int		ox1, oy1, ox2, oy2;
						int		ymax = height - 1;
						int		xmax = width - 1;
						byte	v1, v2;
						byte *	p1, p2, p3, p4;

						if (fmt == PixelFormat.Format8bppIndexed)
						{
							// grayscale
							cy = -halfNewHeight;
							for (int y = 0; y < newHeight; y++)
							{
								cx = -halfNewWidth;
								for (int x = 0; x < newWidth; x++, dst ++)
								{
									ox = (float)( angleCos * cx + angleSin * cy + halfWidth);
									oy = (float)(-angleSin * cx + angleCos * cy + halfHeight);

									// top-left coordinate
									ox1	= (int) ox;
									oy1	= (int) oy;

									if ((ox1 < 0) || (oy1 < 0) || (ox1 >= width) || (oy1 >= height))
									{
										*dst = fillG;
									}
									else
									{
										// bottom-right coordinate
										ox2	= (ox1 == xmax) ? ox1 : ox1 + 1;
										oy2	= (oy1 == ymax) ? oy1 : oy1 + 1;

										if ((dx1 = ox - (float) ox1) < 0)
											dx1 = 0;
										dx2	= 1.0f - dx1;

										if ((dy1 = oy - (float) oy1) < 0)
											dy1 = 0;
										dy2 = 1.0f - dy1;

										p1 = src + oy1 * srcStride;
										p2 = src + oy2 * srcStride;

										// interpolate using 4 points
										v1 = (byte)(dx2 * p1[ox1] + dx1 * p1[ox2]);
										v2 = (byte)(dx2 * p2[ox1] + dx1 * p2[ox2]);
										*dst = (byte)(dy2 * v1 + dy1 * v2);
									}
									cx ++;
								}
								cy ++;
								dst += dstOffset;
							}
						}
						else
						{
							// RGB
							cy = -halfNewHeight;
							for (int y = 0; y < newHeight; y++)
							{
								cx = -halfNewWidth;
								for (int x = 0; x < newWidth; x++, dst += 3)
								{
									ox = (float)( angleCos * cx + angleSin * cy + halfWidth);
									oy = (float)(-angleSin * cx + angleCos * cy + halfHeight);

									// top-left coordinate
									ox1	= (int) ox;
									oy1	= (int) oy;

									if ((ox1 < 0) || (oy1 < 0) || (ox1 >= width) || (oy1 >= height))
									{
										dst[RGB.R] = fillR;
										dst[RGB.G] = fillG;
										dst[RGB.B] = fillB;
									}
									else
									{
										// bottom-right coordinate
										ox2	= (ox1 == xmax) ? ox1 : ox1 + 1;
										oy2	= (oy1 == ymax) ? oy1 : oy1 + 1;

										if ((dx1 = ox - (float) ox1) < 0)
											dx1 = 0;
										dx2	= 1.0f - dx1;

										if ((dy1 = oy - (float) oy1) < 0)
											dy1 = 0;
										dy2 = 1.0f - dy1;

										// get four points
										p1 = p2 = src + oy1 * srcStride;
										p1 += ox1 * 3;
										p2 += ox2 * 3;

										p3 = p4 = src + oy2 * srcStride;
										p3 += ox1 * 3;
										p4 += ox2 * 3;

										// interpolate using 4 points

										// red
										v1 = (byte)(dx2 * p1[RGB.R] + dx1 * p2[RGB.R]);
										v2 = (byte)(dx2 * p3[RGB.R] + dx1 * p4[RGB.R]);
										dst[RGB.R] = (byte)(dy2 * v1 + dy1 * v2);

										// green
										v1 = (byte)(dx2 * p1[RGB.G] + dx1 * p2[RGB.G]);
										v2 = (byte)(dx2 * p3[RGB.G] + dx1 * p4[RGB.G]);
										dst[RGB.G] = (byte)(dy2 * v1 + dy1 * v2);

										// blue
										v1 = (byte)(dx2 * p1[RGB.B] + dx1 * p2[RGB.B]);
										v2 = (byte)(dx2 * p3[RGB.B] + dx1 * p4[RGB.B]);
										dst[RGB.B] = (byte)(dy2 * v1 + dy1 * v2);
									}
									cx ++;
								}
								cy ++;
								dst += dstOffset;
							}
						}
						break;
					}

					case InterpolationMethod.Bicubic:
					{
						// ----------------------------------
						// rotate using bicubic interpolation
						// ----------------------------------

						double	cx, cy;
						float	ox, oy, dx, dy, k1, k2;
						float	r, g, b;
						int		ox1, oy1, ox2, oy2;
						int		ymax = height - 1;
						int		xmax = width - 1;
						byte *	p;

						if (fmt == PixelFormat.Format8bppIndexed)
						{
							// grayscale
							cy = -halfNewHeight;
							for (int y = 0; y < newHeight; y++)
							{
								cx = -halfNewWidth;
								for (int x = 0; x < newWidth; x++, dst ++)
								{

									ox = (float)( angleCos * cx + angleSin * cy + halfWidth);
									oy = (float)(-angleSin * cx + angleCos * cy + halfHeight);

									ox1	= (int) ox;
									oy1	= (int) oy;

									if ((ox1 < 0) || (oy1 < 0) || (ox1 >= width) || (oy1 >= height))
									{
										*dst = fillG;
									}
									else
									{
										dx = ox - (float) ox1;
										dy = oy - (float) oy1;

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
									cx++;
								}
								cy++;
								dst += dstOffset;
							}
						}
						else
						{
							// RGB
							cy = -halfNewHeight;
							for (int y = 0; y < newHeight; y++)
							{
								cx = -halfNewWidth;
								for (int x = 0; x < newWidth; x++, dst += 3)
								{

									ox = (float)( angleCos * cx + angleSin * cy + halfWidth);
									oy = (float)(-angleSin * cx + angleCos * cy + halfHeight);

									ox1	= (int) ox;
									oy1	= (int) oy;

									if ((ox1 < 0) || (oy1 < 0) || (ox1 >= width) || (oy1 >= height))
									{
										dst[RGB.R] = fillR;
										dst[RGB.G] = fillG;
										dst[RGB.B] = fillB;
									}
									else
									{
										dx = ox - (float) ox1;
										dy = oy - (float) oy1;

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
									cx++;
								}
								cy++;
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
