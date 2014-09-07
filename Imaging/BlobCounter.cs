// AForge Image Processing Library
//
// Copyright © Andrew Kirillov, 2005
// andrew.kirillov@gmail.com
//

namespace AForge.Imaging
{
	using System;
	using System.Drawing;
	using System.Drawing.Imaging;


	/// <summary>
	/// Blob counter - counts objects on binrary image
	/// </summary>
	public class BlobCounter
	{
		private int		objectsCount;
		private int[]	objectLabels;

		// Objects count property
		public int ObjectsCount
		{
			get { return objectsCount; }
		}
		// Obeject labels property
		public int[] ObjectLabels
		{
			get { return objectLabels; }
		}

		// ProcessImage is only builds object labels map and count objects
		public void ProcessImage(Bitmap srcImg)
		{
			// check for grayscale image
			// actually we need binary image, but binary images are 
			if (srcImg.PixelFormat != PixelFormat.Format8bppIndexed)
				throw new ArgumentException();

			// get source image size
			int width = srcImg.Width;
			int height = srcImg.Height;

			// no-no-no, we don't want one pixel width images
			if (width == 1)
				throw new ArgumentException("Too small image");

			// allocate labels array
			objectLabels = new int[width * height];
			// initial labels count
			int labelsCount = 0;

			// create map
			int		maxObjects = ((width / 2) + 1) * ((height / 2) + 1) + 1;
			int[]	map = new int[maxObjects];

			// initially map all labels to themself
			for (int i = 0; i < maxObjects; i++)
			{
				map[i] = i;
			}

			// lock source bitmap data
			BitmapData srcData = srcImg.LockBits(
				new Rectangle(0, 0, width, height),
				ImageLockMode.ReadOnly, PixelFormat.Format8bppIndexed);

			int srcStride = srcData.Stride;
			int srcOffset = srcStride - width;

			// do the job
			unsafe
			{
				byte*	src = (byte *) srcData.Scan0.ToPointer();
				int		p = 0;

				// 1 - for pixels of the first row
				if (*src != 0)
				{
					objectLabels[p] = ++labelsCount;
				}
				++src;
				++p;

				for (int x = 1; x < width; x++, src ++, p++)
				{
					// check if we need to label current pixel
					if (*src != 0)
					{
						// check if the previous pixel already labeled
						if (src[-1] != 0)
						{
							// label current pixel, as the previous
							objectLabels[p] = objectLabels[p - 1];
						}
						else
						{
							// create new label
							objectLabels[p] = ++labelsCount;
						}
					}
				}
				src += srcOffset;

				// 2 - for other rows
				// for each row
				for (int y = 1; y < height; y++)
				{
					// for the first pixel of the row, we need to check
					// only upper and upper-right pixels
					if (*src != 0)
					{
						// check surrounding pixels
						if (src[-srcStride] != 0)
						{
							// label current pixel, as the above
							objectLabels[p] = objectLabels[p - width];
						}
						else if (src[1 - srcStride] != 0)
						{
							// label current pixel, as the above right
							objectLabels[p] = objectLabels[p + 1 - width];
						}
						else
						{
							// create new label
							objectLabels[p] = ++labelsCount;
						}
					}
					++src;
					++p;

					// check left pixel and three upper pixels
					for (int x = 1; x < width - 1; x++, src ++, p++)
					{
						if (*src != 0)
						{
							// check surrounding pixels
							if (src[-1] != 0)
							{
								// label current pixel, as the left
								objectLabels[p] = objectLabels[p - 1];
							}
							else if (src[-1 - srcStride] != 0)
							{
								// label current pixel, as the above left
								objectLabels[p] = objectLabels[p - 1 - width];
							}
							else if (src[-srcStride] != 0)
							{
								// label current pixel, as the above
								objectLabels[p] = objectLabels[p - width];
							}

							if (src[1 - srcStride] != 0)
							{
								if (objectLabels[p] == 0)
								{
									// label current pixel, as the above right
									objectLabels[p] = objectLabels[p + 1 - width];
								}
								else
								{
									int	l1 = objectLabels[p];
									int l2 = objectLabels[p + 1 - width];

									if ((l1 != l2) && (map[l1] != map[l2]))
									{
										// merge
										if (map[l1] == l1)
										{
											// map left value to the right
											map[l1] = map[l2];
										}
										else if (map[l2] == l2)
										{
											// map right value to the left
											map[l2] = map[l1];
										}
										else
										{
											// both values already mapped
											map[map[l1]] = map[l2];
											map[l1] = map[l2];
										}

										// reindex
										for (int i = 1; i <= labelsCount; i++)
										{
											if (map[i] != i)
											{
												// reindex
												int j = map[i];
												while (j != map[j])
												{
													j = map[j];
												}
												map[i] = j;
											}
										}
									}
								}
							}

							if (objectLabels[p] == 0)
							{
								// create new label
								objectLabels[p] = ++labelsCount;
							}
						}
					}

					// for the last pixel of the row, we need to check
					// only upper and upper-left pixels
					if (*src != 0)
					{
						// check surrounding pixels
						if (src[-1] != 0)
						{
							// label current pixel, as the left
							objectLabels[p] = objectLabels[p - 1];
						}
						else if (src[-1 - srcStride] != 0)
						{
							// label current pixel, as the above left
							objectLabels[p] = objectLabels[p - 1 - width];
						}
						else if (src[-srcStride] != 0)
						{
							// label current pixel, as the above
							objectLabels[p] = objectLabels[p - width];
						}
						else
						{
							// create new label
							objectLabels[p] = ++labelsCount;
						}
					}
					++src;
					++p;

					src += srcOffset;
				}
			}
			// unlock source images
			srcImg.UnlockBits(srcData);

			// allocate remapping array
			int[] reMap = new int[map.Length];

			// count objects and prepare remapping array
			objectsCount = 0;
			for (int i = 1; i <= labelsCount; i++)
			{
				if (map[i] == i)
				{
					// increase objects count
					reMap[i] = ++objectsCount;
				}
			}
			// second pass to compete remapping
			for (int i = 1; i <= labelsCount; i++)
			{
				if (map[i] != i)
				{
					reMap[i] = reMap[map[i]];
				}
			}

			// repair object labels
			for (int i = 0, n = objectLabels.Length; i < n; i++)
			{
				objectLabels[i] = reMap[objectLabels[i]];
			}
		}

		// Get array of objects rectangles
		public static Rectangle[] GetObjectRectangles(Bitmap srcImg)
		{
			BlobCounter blobCounter = new BlobCounter();

			// process the image
			blobCounter.ProcessImage(srcImg);

			int[]	labels = blobCounter.ObjectLabels;
			int		count = blobCounter.ObjectsCount;

			// image size
			int		width = srcImg.Width;
			int		height = srcImg.Height;
			int		i = 0, label;

			// create object coordinates arrays
			int[]	x1 = new int[count + 1];
			int[]	y1 = new int[count + 1];
			int[]	x2 = new int[count + 1];
			int[]	y2 = new int[count + 1];

			for (int j = 1; j <= count; j++)
			{
				x1[j] = width;
				y1[j] = height;
			}

			// walk through labels array
			for (int y = 0; y < height; y++)
			{
				for (int x = 0; x < width; x++, i++)
				{
					// get current label
					label = labels[i];

					// skip unlabeled pixels
					if (label == 0)
						continue;

					// check and update all coordinates

					if (x < x1[label])
					{
						x1[label] = x;
					}
					if (x > x2[label])
					{
						x2[label] = x;
					}
					if (y < y1[label])
					{
						y1[label] = y;
					}
					if (y > y2[label])
					{
						y2[label] = y;
					}
				}
			}

			// create rectangles
			Rectangle[] rects = new Rectangle[count];

			for (int j = 1; j <= count; j++)
			{
				rects[j - 1] = new Rectangle(x1[j], y1[j], x2[j] - x1[j] + 1, y2[j] - y1[j] + 1);
			}

			return rects;
		}

		// Get array of objects images
		public static Blob[] GetObjects(Bitmap srcImg)
		{
			BlobCounter blobCounter = new BlobCounter();

			// process the image
			blobCounter.ProcessImage(srcImg);

			int[]	labels = blobCounter.ObjectLabels;
			int		count = blobCounter.ObjectsCount;

			// image size
			int		width = srcImg.Width;
			int		height = srcImg.Height;
			int		i = 0, label;

			// --- STEP 1 - find each objects coordinates

			// create object coordinates arrays
			int[]	x1 = new int[count + 1];
			int[]	y1 = new int[count + 1];
			int[]	x2 = new int[count + 1];
			int[]	y2 = new int[count + 1];

			for (int k = 1; k <= count; k++)
			{
				x1[k] = width;
				y1[k] = height;
			}

			// walk through labels array
			for (int y = 0; y < height; y++)
			{
				for (int x = 0; x < width; x++, i++)
				{
					// get current label
					label = labels[i];

					// skip unlabeled pixels
					if (label == 0)
						continue;

					// check and update all coordinates

					if (x < x1[label])
					{
						x1[label] = x;
					}
					if (x > x2[label])
					{
						x2[label] = x;
					}
					if (y < y1[label])
					{
						y1[label] = y;
					}
					if (y > y2[label])
					{
						y2[label] = y;
					}
				}
			}

			// --- STEP 2 - get each object
			Blob[] objects = new Blob[count];

			// lock source bitmap data
			BitmapData srcData = srcImg.LockBits(
				new Rectangle(0, 0, width, height),
				ImageLockMode.ReadOnly, PixelFormat.Format8bppIndexed);

			int srcStride = srcData.Stride;

			// create each image
			for (int k = 1; k <= count; k++)
			{
				int xmin = x1[k];
				int xmax = x2[k];
				int ymin = y1[k];
				int ymax = y2[k];
				int objectWidth = xmax - xmin + 1;
				int objectHeight = ymax - ymin + 1;

				// create new image
				Bitmap dstImg = AForge.Imaging.Image.CreateGrayscaleImage(objectWidth, objectHeight);

				// lock destination bitmap data
				BitmapData dstData = dstImg.LockBits(
					new Rectangle(0, 0, objectWidth, objectHeight),
					ImageLockMode.ReadOnly, PixelFormat.Format8bppIndexed);

				// copy image
				unsafe
				{
					byte * src = (byte *) srcData.Scan0.ToPointer() + ymin * srcStride + xmin;
					byte * dst = (byte *) dstData.Scan0.ToPointer();
					int p = ymin * width + xmin;

					int srcOffset = srcStride - objectWidth;
					int dstOffset = dstData.Stride - objectWidth;
					int labelsOffset = width - objectWidth;

					// for each line
					for (int y = ymin; y <= ymax; y++)
					{
						// copy each pixel
						for (int x = xmin; x <= xmax; x++, src++, dst++, p++)
						{
							if (labels[p] == k)
								*dst = *src;
						}
						src += srcOffset;
						dst += dstOffset;
						p += labelsOffset;
					}
				}
				// unlock destination image
				dstImg.UnlockBits(dstData);

				objects[k - 1] = new Blob(dstImg, new Point(xmin, ymin), srcImg);
			}

			// unlock source image
			srcImg.UnlockBits(srcData);

			return objects;
		}
	}
}
