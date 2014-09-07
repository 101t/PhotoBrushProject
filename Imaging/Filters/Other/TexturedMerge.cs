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
	/// Merge two images using factors from texture
	/// </summary>
	public class TexturedMerge : IFilter, IInPlaceFilter
	{
		private AForge.Imaging.Textures.ITextureGenerator textureGenerator;
		private float[,] texture = null;
		private Bitmap overlayImage;

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
		// OverlayImage property
		public Bitmap OverlayImage
		{
			get { return overlayImage; }
			set { overlayImage = value; }
		}


		// Constructor
		public TexturedMerge( float[,] texture )
		{
			this.texture = texture;
		}
		public TexturedMerge( AForge.Imaging.Textures.ITextureGenerator generator )
		{
			this.textureGenerator = generator;
		}
		public TexturedMerge( float[,] texture, Bitmap overlayImage )
		{
			this.texture = texture;
			this.overlayImage = overlayImage;
		}
		public TexturedMerge( AForge.Imaging.Textures.ITextureGenerator generator, Bitmap overlayImage )
		{
			this.textureGenerator = generator;
			this.overlayImage = overlayImage;
		}

		// Apply filter
		public Bitmap Apply( Bitmap srcImg )
		{
			// get source image size
			int width = srcImg.Width;
			int height = srcImg.Height;

			// source image and overlay must have the same pixel format, width and height
			if ( ( srcImg.PixelFormat != overlayImage.PixelFormat ) ||
				( width != overlayImage.Width ) || ( height != overlayImage.Height ) )
				throw new ArgumentException( );
			
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
			// get source image size
			int width = img.Width;
			int height = img.Height;

			// source image and overlay must have the same pixel format, width and height
			if ( ( img.PixelFormat != overlayImage.PixelFormat ) ||
				( width != overlayImage.Width ) || ( height != overlayImage.Height ) )
				throw new ArgumentException( );

			if (
				( img.PixelFormat != PixelFormat.Format8bppIndexed ) &&
				( img.PixelFormat != PixelFormat.Format24bppRgb )
				)
				throw new ArgumentException( );

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

			// lock overlay image
			BitmapData ovrData = overlayImage.LockBits(
				new Rectangle( 0, 0, width, height ),
				ImageLockMode.ReadOnly, fmt );

			int pixelSize = ( fmt == PixelFormat.Format8bppIndexed ) ? 1 : 3;
			int offset = data.Stride - widthToProcess * pixelSize;

			// do the job
			byte * ptr = (byte *) data.Scan0.ToPointer( );
			byte * ovr = (byte *) ovrData.Scan0.ToPointer( );

			// for each line
			for ( int y = 0; y < heightToProcess; y++ )
			{
				// for each pixel
				for ( int x = 0; x < widthToProcess; x++ )
				{
					double t1 = texture[y, x];
					double t2 = 1 - t1;

					for ( int i = 0; i < pixelSize; i++, ptr++, ovr++ )
					{
						*ptr = (byte) Math.Min( 255.0f, *ptr * t1 + *ovr * t2 );
					}
				}
				ptr += offset;
				ovr += offset;
			}

			overlayImage.UnlockBits( ovrData );
		}
	}
}
