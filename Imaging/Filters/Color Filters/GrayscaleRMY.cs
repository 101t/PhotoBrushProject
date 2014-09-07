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
	/// Makes an images grayscale using R-Y algorithm
	/// </summary>
	public sealed class GrayscaleRMY : Grayscale
	{
		public GrayscaleRMY( ) : base( 0.5f, 0.419f, 0.081f )
		{
		}
	}
}
