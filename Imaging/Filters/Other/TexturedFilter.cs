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
	/// TexturedFilter - filter an image using texture
	/// </summary>
	public class TexturedFilter : IFilter
	{
		private AForge.Imaging.Textures.ITextureGenerator textureGenerator;
		private float[,] texture = null;
		private IFilter	filter1 = null;
		private IFilter	filter2 = null;

		private float	preserveLevel = 0.0f;
		private float	filterLevel = 1.0f;

		// Preserve level property
		public float PreserveLevel
		{
			get { return preserveLevel; }
			set { preserveLevel = Math.Max( 0.0f, Math.Min( 1.0f, value ) ); }
		}
		// Filter level property
		public float FilterLevel
		{
			get { return filterLevel; }
			set { filterLevel = Math.Max( 0.0f, Math.Min( 1.0f, value ) ); }
		}
		// Texture property
		public float[,] Texture
		{
			get { return texture; }
			set { texture = value; }
		}
		// Texture generator property
		public AForge.Imaging.Textures.ITextureGenerator TextureGenerator
		{
			get { return textureGenerator; }
			set { textureGenerator = value; }
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
		public TexturedFilter( )
		{
		}
		public TexturedFilter( float[,] texture, IFilter filter1 )
		{
			this.texture = texture;
			this.filter1 = filter1;
		}
		public TexturedFilter( float[,] texture, IFilter filter1, IFilter filter2 )
		{
			this.texture = texture;
			this.filter1 = filter1;
			this.filter2 = filter2;
		}
		public TexturedFilter( AForge.Imaging.Textures.ITextureGenerator generator, IFilter filter1 )
		{
			this.textureGenerator = generator;
			this.filter1 = filter1;
		}
		public TexturedFilter( AForge.Imaging.Textures.ITextureGenerator generator, IFilter filter1, IFilter filter2 )
		{
			this.textureGenerator = generator;
			this.filter1 = filter1;
			this.filter2 = filter2;
		}


		// Apply filter using texture
		// The higher intensity in texture - the more filter1 is used
		//
		public Bitmap Apply( Bitmap srcImg )
		{
			int width = srcImg.Width;
			int	height = srcImg.Height;

			if ( textureGenerator != null )
			{
				// create new texture, if generator was provided
				texture = textureGenerator.Generate( width, height );
			}
			else
			{
				// check existing texture
				if ( ( texture.GetLength( 0 ) != height ) || ( texture.GetLength( 1 ) != width ) )
				{
					// sorry, but source image must have the same dimension as texture
					throw new ArgumentException( "Texture size does not match  image size" );
				}
			}

			Bitmap	dstImg = filter1.Apply( srcImg );
			bool	disposeSrc = false;

			// check destination size
			if ( (width != dstImg.Width ) || ( height != dstImg.Height ) )
			{
				dstImg.Dispose( );
				// we are not handling such situations yet
				throw new ApplicationException( );
			}

			// apply filter2 also, if it is
			if ( filter2 != null )
			{
				srcImg = filter2.Apply( srcImg );
				disposeSrc = true;

				// check source size
				if ( ( width != srcImg.Width ) || ( height != srcImg.Height) )
				{
					srcImg.Dispose( );
					dstImg.Dispose( );
					// we are not handling such situations yet
					throw new ApplicationException( );
				}
			}

			// check pixel formats
			if ( dstImg.PixelFormat != srcImg.PixelFormat )
			{
				IFilter f = new GrayscaleToRGB( );

				// convert temp image to RGB format
				if ( dstImg.PixelFormat == PixelFormat.Format8bppIndexed )
				{
					Bitmap t = f.Apply( dstImg );
					dstImg.Dispose( );
					dstImg = t;
				}
				// convert source image to RGB format
				if ( srcImg.PixelFormat == PixelFormat.Format8bppIndexed )
				{
					Bitmap t = f.Apply( srcImg );
					if ( disposeSrc )
						srcImg.Dispose( );
					srcImg = t;
					disposeSrc = true;
				}
			}

			// lock source bitmap data
			BitmapData srcData = srcImg.LockBits(
				new Rectangle( 0, 0, width, height ),
				ImageLockMode.ReadOnly, srcImg.PixelFormat );

			// lock destination bitmap data
			BitmapData dstData = dstImg.LockBits(
				new Rectangle( 0, 0, width, height ),
				ImageLockMode.ReadWrite, dstImg.PixelFormat );

			int pixelSize = (dstImg.PixelFormat == PixelFormat.Format8bppIndexed) ? 1 : 3;
			int offset = dstData.Stride - width * pixelSize;

			// do the job
			unsafe
			{
				byte * src = (byte *) srcData.Scan0.ToPointer( );
				byte * dst = (byte *) dstData.Scan0.ToPointer( );

				if ( preserveLevel != 0.0 )
				{
					// for each line
					for ( int y = 0; y < height; y++ )
					{
						// for each pixel
						for ( int x = 0; x < width; x++ )
						{
							double t = texture[y, x];

							for ( int i = 0; i < pixelSize; i++, src++, dst++ )
							{
								*dst = (byte) Math.Min( 255.0f, ( preserveLevel * *src ) + ( filterLevel * *dst ) * t );
							}
						}
						src += offset;
						dst += offset;
					}
				}
				else
				{
					// for each line
					for ( int y = 0; y < height; y++ )
					{
						// for each pixel
						for ( int x = 0; x < width; x++ )
						{
							double t1 = texture[y, x];
							double t2 = 1 - t1;

							for ( int i = 0; i < pixelSize; i++, src++, dst++ )
							{
								*dst = (byte) Math.Min( 255.0f, t1 * *dst + t2 * *src );
							}
						}
						src += offset;
						dst += offset;
					}
				}
			}

			// unlock all images
			dstImg.UnlockBits( dstData );
			srcImg.UnlockBits( srcData );

			// dispose source ?
			if ( disposeSrc )
				srcImg.Dispose( );

			// return result
			return dstImg;
		}

	}
}
