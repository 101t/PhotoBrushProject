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
	/// Makes an images grayscale using BT709 algorithm
	/// </summary>
	public sealed class GrayscaleBT709 : Grayscale
	{
		public GrayscaleBT709( ) : base( 0.2125f, 0.7154f, 0.0721f )
		{
		}
	}
}
