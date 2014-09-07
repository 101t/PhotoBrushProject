using System;

namespace PsdFramework
{
	/// <summary>
	/// This structure contains display information about each channel
	/// </summary>
	public class DisplayInfo
	{
		// This structure contains display information about each channel.
		//  DisplayInfo Color spaces
		//	Color-ID	Name	Description
		//-------------------------------------------
		//		0		RGB			The first three values in the color data are red, green, and blue.
		//							They are full unsigned 16–bit values as in Apple’s RGBColor data
		//							structure. Pure red=65535,0,0.
		//		1		HSB			The first three values in the color data are hue, saturation, and
		//							brightness. They are full unsigned 16–bit values as in Apple’s
		//							HSVColor data structure. Pure red=0,65535, 65535.
		//		2		CMYK		The four values in the color data are cyan, magenta, yellow, and
		//							black. They are full unsigned 16–bit values. 0=100% ink. Pure
		//							cyan=0,65535,65535,65535.
		//		7		Lab			The first three values in the color data are lightness, a chrominance,
		//							and b chrominance.
		//							Lightness is a 16–bit value from 0...10000. The chromanance components
		//							are each 16–bit values from –12800...12700. Gray values
		//							are represented by chrominance components of 0. Pure
		//							white=10000,0,0.
		//		8		grayscale	The first value in the color data is the gray value, from 0...10000.
		
		public short ColourSpace;
		public short[] Colour = new short[4];
		public short Opacity;			// 0..100
		public bool kind;				// selected = false, protected = true
		public byte padding;	        // should be zero

		public DisplayInfo()
		{
			ColourSpace = -1;
			for (int n = 0; n < 4; ++n)	Colour[n] = 0;
			Opacity = -1;
			kind = false;
			padding = 0x00;
		}
	}
}
