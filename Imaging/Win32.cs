// AForge Image Processing Library
//
// Copyright © Andrew Kirillov, 2005
// andrew.kirillov@gmail.com
//

namespace AForge.Imaging
{
	using System;
	using System.Runtime.InteropServices;
	using System.Security;

	/// <summary>
	/// Windows API functions and structures
	/// </summary>
	internal class Win32
	{
		// memcpy - copy a block of memory
		[DllImport("ntdll.dll")]
		public static extern IntPtr memcpy(
			IntPtr dst,
			IntPtr src,
			int count);
		[DllImport("ntdll.dll")]
		public static extern unsafe byte * memcpy(
			byte * dst,
			byte * src,
			int count);

		// memset - fill memory with specified values
		[DllImport("ntdll.dll")]
		public static extern IntPtr memset(
			IntPtr dst,
			int filler,
			int count);
	}
}
