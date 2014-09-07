// AForge Image Processing Library
//
// Copyright © Andrew Kirillov, 2005
// andrew.kirillov@gmail.com
//

namespace AForge.Imaging.Filters
{
	using System.Drawing;
	using System.Drawing.Imaging;

	/// <summary>
	/// Makes an images grayscale using Y algorithm
	/// </summary>
	public sealed class GrayscaleY : Grayscale
	{
		public GrayscaleY( ) : base( 0.299f, 0.587f, 0.114f )
		{
		}
	}
}
