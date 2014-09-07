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
	/// Filter interface
	/// </summary>
	public interface IFilter
	{
		Bitmap Apply( Bitmap img );
	}
}
			