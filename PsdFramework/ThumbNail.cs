using System;

namespace PsdFramework
{
	/// <summary>
	/// ThumbNail class
	/// </summary>
	public class ThumbNail
	{
		// Adobe Photoshop 5.0 and later stores thumbnail information for preview
		// display in an image resource block. These resource blocks consist of an
		// 28 byte header, followed by a JFIF thumbnail in RGB (red, green, blue)
		// for both Macintosh and Windows. Adobe Photoshop 4.0 stored the
		// thumbnail information in the same format except the data section is
		// (blue, green, red). The Adobe Photoshop 4.0 format is at resource ID
		// and the Adobe Photoshop 5.0 format is at resource ID 1036.
		//  Thumbnail resource header
		//	Type		Name		Description
		//-------------------------------------------
		//	4 bytes		format			= 1 (kJpegRGB). Also supports kRawRGB (0).
		//	4 bytes		width			Width of thumbnail in pixels.
		//	4 bytes		height			Height of thumbnail in pixels.
		//	4 bytes		widthbytes		Padded row bytes as (width * bitspixel + 31) / 32 * 4.
		//	4 bytes		size			Total size as widthbytes * height * planes
		//	4 bytes		compressedsize	Size after compression. Used for consistentcy check.
		//	2 bytes		bitspixel		= 24. Bits per pixel.
		//	2 bytes		planes			= 1. Number of planes.
		//	Variable	Data			JFIF data in RGB format.
		//								Note: For resource ID 1033 the data is in BGR format.
		public int		nFormat;
		public int		nWidth;
		public int		nHeight;
		public int		nWidthBytes;
		public int		nSize;
		public int		nCompressedSize;
		public short	nBitPerPixel;
		public short	nPlanes;
		public byte[]   Data;

		public ThumbNail()
		{
			nFormat = -1;
			nWidth = -1;
			nHeight = -1;
			nWidthBytes = -1;
			nSize = -1;
			nCompressedSize = -1;
			nBitPerPixel = -1;
			nPlanes = -1;
		}
	}
}
