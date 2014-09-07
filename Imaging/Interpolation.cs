// AForge Image Processing Library
//
// Copyright © Andrew Kirillov, 2005
// andrew.kirillov@gmail.com
//

namespace AForge.Imaging
{
	using System;

	// Interpolation methods
	public enum InterpolationMethod
	{
		NearestNeighbor,
		Bilinear,
		Bicubic
	}

	/// <summary>
	/// Interpolation
	/// </summary>
	public class Interpolation
	{
		// BiCubic kernel
		public static float BiCubicKernel(float x)
		{
			if (x > 2.0f)
				return 0.0f;

			float	a, b, c, d;
			float	xm1 = x - 1.0f;
			float	xp1 = x + 1.0f;
			float	xp2 = x + 2.0f;

			a = (xp2 <= 0.0f) ? 0.0f : xp2 * xp2 * xp2;
			b = (xp1 <= 0.0f) ? 0.0f : xp1 * xp1 * xp1;
			c = (x   <= 0.0f) ? 0.0f : x * x * x;
			d = (xm1 <= 0.0f) ? 0.0f : xm1 * xm1 * xm1;

			return (0.16666666666666666667f * (a - (4.0f * b) + (6.0f * c) - (4.0f * d)));
		}
	}
}
