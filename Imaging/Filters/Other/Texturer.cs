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
	/// Adjust pixel colors using factors from texture
	/// </summary>
	public class Texturer : IFilter, IInPlaceFilter
	{
		private AForge.Imaging.Textures.ITextureGenerator textureGenerator;
		private float[,] texture = null;

		private float	preserveLevel = 0.5f;
		private float	filterLevel = 0.5f;

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


		// Constructor
		public Texturer( float[,] texture )
		{
			this.texture = texture;
		}
		public Texturer( float[,] texture, float preserveLevel, float filterLevel )
		{
			this.texture = texture;
			this.preserveLevel = preserveLevel;
			this.filterLevel = filterLevel;
		}
		public Texturer( AForge.Imaging.Textures.ITextureGenerator generator )
		{
			this.textureGenerator = generator;
		}
		public Texturer( AForge.Imaging.Textures.ITextureGenerator generator, float preserveLevel, float filterLevel )
		{
			this.textureGenerator = generator;
			this.preserveLevel = preserveLevel;
			this.filterLevel = filterLevel;
		}


		// Apply filter
		public Bitmap Apply( Bitmap srcImg )
		{
			// get source image size
			int width = srcImg.Width;
			int height = srcImg.Height;
			
			PixelFormat fmt = ( srcImg.PixelFormat == PixelFormat.Format8bppIndexed ) ?
				PixelFormat.Format8bppIndexed : PixelFormat.Format24bppRgb;

			// lock source bitmap data
			BitmapData srcData = srcImg.LockBits(
				new Rectangle( 0, 0, width, height ),
				ImageLockMode.ReadOnly, fmt );

			// create new image
			Bitmap dstImg = ( fmt == PixelFormat.Format8bppIndexed ) ?
				AForge.Imaging.Image.CreateGrayscaleImage( width, height ) :
				new Bitmap( width, height, fmt );

			// lock destination bitmap data
			BitmapData dstData = dstImg.LockBits(
				new Rectangle( 0, 0, width, height ),
				ImageLockMode.ReadWrite, fmt );

			// copy image
			Win32.memcpy( dstData.Scan0, srcData.Scan0, srcData.Stride * height );

			// process the filter
			ProcessFilter( dstData, fmt );

			// unlock both images
			dstImg.UnlockBits( dstData );
			srcImg.UnlockBits( srcData );

			return dstImg;
		}


		// Apply filter on current image
		public void ApplyInPlace( Bitmap img )
		{
			if (
				( img.PixelFormat != PixelFormat.Format8bppIndexed ) &&
				( img.PixelFormat != PixelFormat.Format24bppRgb )
				)
				throw new ArgumentException( );

			// get source image size
			int width = img.Width;
			int height = img.Height;

			// lock source bitmap data
			BitmapData data = img.LockBits(
				new Rectangle( 0, 0, width, height ),
				ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb );

			// process the filter
			ProcessFilter( data, img.PixelFormat );

			// unlock image
			img.UnlockBits( data );
		}


		// Process the filter
		private unsafe void ProcessFilter( BitmapData data, PixelFormat fmt )
		{
			int width	= data.Width;
			int height	= data.Height;

			int widthToProcess = width;
			int heightToProcess = height;

			// if generator was specified, then generate a texture
			// otherwise use provided texture
			if ( textureGenerator != null )
			{
				texture = textureGenerator.Generate( width, height );
			}
			else
			{
				widthToProcess = Math.Min( width, texture.GetLength( 1 ) );
				heightToProcess = Math.Min( height, texture.GetLength( 0 ) );
			}

			int pixelSize = ( fmt == PixelFormat.Format8bppIndexed ) ? 1 : 3;
			int offset = data.Stride - widthToProcess * pixelSize;

			// do the job
			byte * ptr = (byte *) data.Scan0.ToPointer( );

			// texture
			for ( int y = 0; y < heightToProcess; y++ )
			{
				for ( int x = 0; x < widthToProcess; x++ )
				{
					double t = texture[y, x];
					// process each pixel
					for ( int i = 0; i < pixelSize; i++, ptr++ )
					{
						*ptr = (byte) Math.Min( 255.0f, ( preserveLevel * *ptr ) + ( filterLevel * *ptr ) * t );
					}
				}
				ptr += offset;
			}
		}
	}
}
