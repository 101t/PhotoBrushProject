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
	/// Connected Components Labeling - performs labeling of binary image
	/// </summary>
	public class ConnectedComponentsLabeling : IFilter
	{
		// Color table for coloring objects
		private static Color[] colorTable = new Color[]
		{
			Color.Red,		Color.Green,	Color.Blue,			Color.Yellow,
			Color.Violet,	Color.Brown,	Color.Olive,		Color.Cyan,

			Color.Magenta,	Color.Gold,		Color.Indigo,		Color.Ivory,
			Color.HotPink,	Color.DarkRed,	Color.DarkGreen,	Color.DarkBlue,

			Color.DarkSeaGreen,	Color.Gray,	Color.DarkKhaki,	Color.DarkGray,
			Color.LimeGreen, Color.Tomato,	Color.SteelBlue,	Color.SkyBlue,

			Color.Silver,	Color.Salmon,	Color.SaddleBrown,	Color.RosyBrown,
            Color.PowderBlue, Color.Plum,	Color.PapayaWhip,	Color.Orange
		};

		// blob counter
		private BlobCounter blobCounter = new BlobCounter();

		// Color table property
		public static Color[] ColorTable
		{
			get { return colorTable; }
			set { colorTable = value; }
		}
		// Object count property
		public int ObjectCount
		{
			get { return blobCounter.ObjectsCount; }
		}


		// Apply filter
		public Bitmap Apply(Bitmap srcImg)
		{
			// process the image
			blobCounter.ProcessImage(srcImg);

			// get object labels
			int[] labels = blobCounter.ObjectLabels;

			int width = srcImg.Width;
			int height = srcImg.Height;

			// create new RGB image
			Bitmap dstImg = new Bitmap(width, height, PixelFormat.Format24bppRgb);

			// lock destination bitmap data
			BitmapData dstData = dstImg.LockBits(
				new Rectangle(0, 0, width, height),
				ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

			int dstOffset = dstData.Stride - width * 3;

			// do the job
			unsafe
			{
				byte*	dst = (byte *) dstData.Scan0.ToPointer();
				int		p = 0;

				// for each row
				for (int y = 0; y < height; y++)
				{
					// for each pixel
					for (int x = 0; x < width; x++, dst += 3, p++)
					{
						if (labels[p] != 0)
						{
							Color c = colorTable[(labels[p] - 1) % colorTable.Length];

							dst[RGB.R] = c.R;
							dst[RGB.G] = c.G;
							dst[RGB.B] = c.B;
						}
					}
					dst += dstOffset;
				}
			}
			// unlock both images
			dstImg.UnlockBits(dstData);

			return dstImg;
		}
	}
}
