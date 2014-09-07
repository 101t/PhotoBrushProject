// AForge Image Processing Library
//
// Copyright © Andrew Kirillov, 2005
// andrew.kirillov@gmail.com
//

namespace AForge.Imaging
{
	using System;
	using System.Collections;
	using System.Drawing;
	using System.Drawing.Imaging;

	public class HoughLine
	{
		public short	Theta;
		public short	R;
		public short	Value;

		public HoughLine( )
		{
		}
		public HoughLine( short theta, short r, short val )
		{
			Theta	= theta;
			R		= r;
			Value	= val;
		}
	}

	/// <summary>
	/// Line hough transform
	/// </summary>
	public class LineHoughTransform
	{
		private short[,]	houghMap;
		private double[]	sinMap;
		private double[]	cosMap;
		private short		maxMapValue;

		private short		localPeakRadius = 4;
		private const int	houghHeight = 180;
		private const double thetaStep = Math.PI / houghHeight;

		// Max Value property
		public short MaxValue
		{
			get { return maxMapValue; }
		}


		// Constructor
		public LineHoughTransform( )
		{
			sinMap = new double[houghHeight];
			cosMap = new double[houghHeight];

			for ( int i = 0; i < houghHeight; i++ )
			{
				sinMap[i] = Math.Sin( i * thetaStep );
				cosMap[i] = Math.Cos( i * thetaStep );
			}
		}

		// Process image
		public void ProcessImage( Bitmap srcImg )
		{
			if ( srcImg.PixelFormat != PixelFormat.Format8bppIndexed )
				throw new ArgumentException( );

			// get source image size
			int width = srcImg.Width;
			int height = srcImg.Height;

			// lock source bitmap data
			BitmapData srcData = srcImg.LockBits(
				new Rectangle( 0, 0, width, height ),
				ImageLockMode.ReadOnly, PixelFormat.Format8bppIndexed );

			int srcOffset = srcData.Stride - width;
			int halfWidth = width / 2;
			int halfHeight = height / 2;
			int toWidth = width - halfWidth;
			int toHeight = height - halfHeight;

			int halfHoughWidth = (int) ( Math.Sqrt( 2 ) * Math.Max( width, height ) );
			int	houghWidth = halfHoughWidth * 2;

			houghMap = new short[houghHeight, houghWidth];

			// do the job
			unsafe
			{
				byte * src = (byte *) srcData.Scan0.ToPointer( );

				// for each row
				for ( int y = -halfHeight; y < toHeight; y++ )
				{
					// for each pixel
					for ( int x = -halfWidth; x < toWidth; x++, src++ )
					{
						if ( *src != 0 )
						{
							// for each Theta value
							for ( int t = 0; t < houghHeight; t++ )
							{
								int r = (int) ( cosMap[t] * x - sinMap[t] * y ) + halfHoughWidth;

								if ( ( r < 0 ) || ( r >= houghWidth ) )
									continue;

								houghMap[t, r]++;
							}
						}
					}
					src += srcOffset;
				}
			}
			srcImg.UnlockBits( srcData );

			// find max value in Hough map
			maxMapValue = 0;
			for ( int i = 0; i < houghHeight; i++ )
			{
				for ( int j = 0; j < houghWidth; j++ )
				{
					if ( houghMap[i, j] > maxMapValue )
					{
						maxMapValue = houghMap[i, j];
					}
				}
			}
		}

		// Create Bitmap from the complex image
		public Bitmap ToBitmap( )
		{
			// check if Hough transform was made already
			if ( houghMap == null )
			{
				throw new ApplicationException( );
			}

			int width = houghMap.GetLength( 1 );
			int height = houghMap.GetLength( 0 );

			// create new image
			Bitmap dstImg = AForge.Imaging.Image.CreateGrayscaleImage( width, height );
			
			// lock destination bitmap data
			BitmapData dstData = dstImg.LockBits(
				new Rectangle( 0, 0, width, height ),
				ImageLockMode.ReadWrite, PixelFormat.Format8bppIndexed );

			int offset = dstData.Stride - width;
			float scale = 255.0f / maxMapValue;

			// do the job
			unsafe
			{
				byte * dst = (byte *) dstData.Scan0.ToPointer( );

				for ( int y = 0; y < height; y++ )
				{
					for ( int x = 0; x < width; x++, dst ++ )
					{
						*dst = (byte) System.Math.Max( 0, System.Math.Min( 255, (int) ( scale * houghMap[y, x] ) ) );
					}
					dst += offset;
				}
			}

			// unlock destination images
			dstImg.UnlockBits( dstData );

			return dstImg;
		}


		// Get lines using theirs intensity value [0, 1]
		public HoughLine[] GetLinesByIntensity( double intensity )
		{
			short	minIntensity = (short) ( maxMapValue * intensity );
			int		maxTheta = houghMap.GetLength( 0 );
			int		maxR = houghMap.GetLength( 1 );
			short	v;
			bool	foundMax;

			int		halfHoughWidth = maxR >> 1;


			ArrayList linesList = new ArrayList( );

			// for each Theta value
			for ( int theta = 0; theta < maxTheta; theta++ )
			{
				// for each R value
				for ( int r = 0; r < maxR; r++ )
				{
					// get current value
					v = houghMap[theta, r];
					foundMax = false;

					if ( v < minIntensity )
						continue;

					// check neighboors
					for ( int tt = theta - localPeakRadius, ttMax = theta + localPeakRadius; tt < ttMax; tt++ )
					{
						if ( tt < 0 )
							continue;
						if ( ( foundMax == true ) || ( tt >= maxTheta ) )
							break;

						for ( int tr = r - localPeakRadius, trMax = r + localPeakRadius; tr < trMax; tr++ )
						{
							if ( ( tr < 0 ) || ( ( tr == r ) && ( tt == theta ) ) )
								continue;
							if ( tr >= maxR )
								break;

							// compare the neighboor with current value
							if ( houghMap[tt, tr] > v )
							{
								foundMax = true;
								break;
							}
						}
					}

					//
					if ( !foundMax )
					{
						// we have local maximum
						linesList.Add( new HoughLine( (short) theta, (short) ( r - halfHoughWidth ), v ) );
					}
				}
			}

			//
			int linesCount = linesList.Count;
			HoughLine[] lines = new HoughLine[linesCount];

			for ( int i = 0; i < linesCount; i++)
			{
				lines[i] = (HoughLine) linesList[i];
			}

			return lines;
		}


		// Process image
/*		public static void DrawLines( Bitmap image, HoughLine[] lines )
		{
			// get source image size
			int width = srcImg.Width;
			int height = srcImg.Height;

			// get pixel format
			PixelFormat fmt = ( image.PixelFormat == PixelFormat.Format8bppIndexed ) ?
				PixelFormat.Format8bppIndexed : PixelFormat.Format24bppRgb;

			// lock source bitmap data
			BitmapData imgData = image.LockBits(
				new Rectangle( 0, 0, width, height ),
				ImageLockMode.ReadOnly, fmt );

			int stride = imgData.Stride;
			int pixelSize = ( fmt == PixelFormat.Format8bppIndexed ) ? 1 : 3;

			// do the job
			unsafe
			{
				for ( int i = 0, n = lines.Length; i < n; i++ )
				{
				}
			}

			// unlock image
			image.UnlockBits( imgData );
		}*/
	}
}
