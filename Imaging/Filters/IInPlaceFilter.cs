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
	/// In Place Filter interface
	/// </summary>
	public interface IInPlaceFilter
	{
		void ApplyInPlace( Bitmap img );
	}
}
			
