using System;

namespace PsdFramework
{
	/// <summary>
	/// Summary description for HeaderInfo class.
	/// </summary>
	public class HeaderInfo
	{
		//Table 2-12: HeaderInfo Color spaces
		//	Color-ID	Name	Description
		//-------------------------------------------
		//		0		Bitmap			// Probably means black & white
		//		1		Grayscale		The first value in the color data is the gray value, from 0...10000.
		//		2		Indexed
		//		3		RGB				The first three values in the color data are red, green, and blue.
		//								They are full unsigned 16–bit values as in Apple’s RGBColor data
		//								structure. Pure red=65535,0,0.
		//		4		CMYK			The four values in the color data are cyan, magenta, yellow, and
		//								black. They are full unsigned 16–bit values. 0=100% ink. Pure
		//								cyan=0,65535,65535,65535.
		//		7		Multichannel	// Have no idea
		//		8		Duotone
		//		9		Lab				The first three values in the color data are lightness, a chrominance,
		//								and b chrominance.
		//								Lightness is a 16–bit value from 0...100. The chromanance components
		//								are each 16–bit values from –128...127. Gray values
		//								are represented by chrominance components of 0. Pure
		//								white=100,0,0.


		public short nChannels;
		public int nHeight;
		public int nWidth;
		public short nBitsPerPixel;
		public short nColourMode;

		public HeaderInfo()
		{
			nChannels = -1;
			nHeight = -1;
			nWidth = -1;
			nBitsPerPixel = -1;
			nColourMode = -1;
		}
	}

}
