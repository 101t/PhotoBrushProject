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
	/// Replace channel of RGB color space
	/// </summary>
	public class ReplaceChannel : IFilter, IInPlaceFilter
	{
		private short channel = RGB.R;
		private Bitmap channelImage;
		
		// Channel property
		public short Channel
		{
			get { return channel; }
			set
			{
				if (
					( value != RGB.R ) &&
					( value != RGB.G ) &&
					( value != RGB.B )
					)
				{
					throw new ArgumentException( );
				}
				channel = value;
			}
		}
		// ChannelImage property
		public Bitmap ChannelImage
		{
			get { return channelImage; }
			set
			{
				// check for not null
				if ( value == null )
					throw new NullReferenceException( );
				// check for valid format
				if ( value.PixelFormat != PixelFormat.Format8bppIndexed )
					throw new ArgumentException( );

				channelImage = value;
			}
		}

		// Constructor
		public ReplaceChannel( )
		{
		}
		public ReplaceChannel( Bitmap channelImage )
		{
			if ( channelImage.PixelFormat != PixelFormat.Format8bppIndexed )
				throw new ArgumentException( );

			this.channelImage = channelImage;
		}
		public ReplaceChannel( short channel )
		{
			this.Channel = channel;
		}
		public ReplaceChannel( Bitmap channelImage, short channel ) : this( channelImage )
		{
			this.Channel = channel;
		}

		// Apply filter
		public Bitmap Apply( Bitmap srcImg )
		{
			// get source image size
			int width = srcImg.Width;
			int height = srcImg.Height;

			// 1. source image and overlay must have the same width and height
			// 2. source image must be RGB
			if ( ( srcImg.PixelFormat != PixelFormat.Format24bppRgb ) ||
				( width != channelImage.Width ) || ( height != channelImage.Height ) )
				throw new ArgumentException( );

			// lock source bitmap data
			BitmapData srcData = srcImg.LockBits(
				new Rectangle( 0, 0, width, height ),
				ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb );

			// create new image
			Bitmap dstImg = new Bitmap( width, height, PixelFormat.Format24bppRgb );

			// lock destination bitmap data
			BitmapData dstData = dstImg.LockBits(
				new Rectangle( 0, 0, width, height ),
				ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb );

			// copy image
			Win32.memcpy( dstData.Scan0, srcData.Scan0, srcData.Stride * height );

			// process the filter
			ProcessFilter( dstData );

			// unlock all images
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

			// 1. source image and overlay must have the same width and height
			// 2. source image must be RGB
			if ( ( img.PixelFormat != PixelFormat.Format24bppRgb ) ||
				( width != channelImage.Width ) || ( height != channelImage.Height ) )
				throw new ArgumentException( );

			// lock source bitmap data
			BitmapData data = img.LockBits(
				new Rectangle( 0, 0, width, height ),
				ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb );

			// process the filter
			ProcessFilter( data );

			// unlock image
			img.UnlockBits( data );
		}


		// Process the filter
		private unsafe void ProcessFilter( BitmapData data )
		{
			int width	= data.Width;
			int height	= data.Height;

			// lock overlay image
			BitmapData ovrData = channelImage.LockBits(
				new Rectangle( 0, 0, width, height ),
				ImageLockMode.ReadOnly, PixelFormat.Format8bppIndexed );

			int offset = data.Stride - width * 3;
			int offsetOvr = ovrData.Stride - width;

			// do the job
			unsafe
			{
				byte * dst = (byte *) data.Scan0.ToPointer( );
				byte * ovr = (byte *) ovrData.Scan0.ToPointer( );

				// for each line
				for ( int y = 0; y < height; y++ )
				{
					// for each pixel
					for ( int x = 0; x < width; x++, dst += 3, ovr ++ )
					{
						dst[channel] = *ovr;
					}
					dst += offset;
					ovr += offsetOvr;
				}
			}
			// unlock
			channelImage.UnlockBits( ovrData );
		}
	}
}
