using System;
using System.Runtime.InteropServices;

namespace PsdFramework
{
	/// <summary>
	/// This class shall keep the Win32 APIs being used in 
	/// the program.
	/// </summary>
	public class WinInvoke32
	{

		#region Class Variables
		public const int BI_RGB = 0;
		public const int WHITE_BRUSH = 0;
		#endregion		
		
		#region Class Functions
		
		[DllImport("gdi32.dll",EntryPoint="DeleteDC")]
		public static extern IntPtr DeleteDC(IntPtr hDc);

		[DllImport("gdi32.dll",EntryPoint="DeleteObject")]
		public static extern IntPtr DeleteObject(IntPtr hDc);

		[DllImport ("gdi32.dll",EntryPoint="CreateCompatibleDC")]
		public static extern IntPtr CreateCompatibleDC(IntPtr hdc);

		[DllImport ("gdi32.dll",EntryPoint="SelectObject")]
		public static extern IntPtr SelectObject(IntPtr hdc,IntPtr bmp);

		[DllImport("gdi32.dll", EntryPoint="CreateDIBSection")]
		public static extern IntPtr CreateDIBSection(IntPtr hDC, ref BITMAPINFO pBitmapInfo, int un, IntPtr lplpVoid, IntPtr handle, int offset);
		
		[DllImport("gdi32.dll", EntryPoint="GetStockObject")]
		public static extern IntPtr  GetStockObject(int fnObject);

		[DllImport("gdi32.dll", EntryPoint="SetPixel")]
		public static extern IntPtr SetPixel(IntPtr hDC, int x, int y, int nColor);

		[DllImport("user32.dll",EntryPoint="GetDC")]
		public static extern IntPtr GetDC(IntPtr ptr);

		[DllImport("user32.dll",EntryPoint="ReleaseDC")]
		public static extern IntPtr ReleaseDC(IntPtr hWnd,IntPtr hDc);
	
		[DllImport("user32.dll", EntryPoint="FillRect")]
		public static extern int FillRect(IntPtr hDC, ref RECT lprc, IntPtr hbr);
		
		#endregion

		#region Public Constructor
		public WinInvoke32()
		{
			// 
			// TODO: Add constructor logic here
			//
		}
		#endregion
	}

	[StructLayout(LayoutKind.Sequential, Pack=1)]
	public struct BITMAPINFOHEADER
	{
		public Int32 biSize;
		public Int32 biWidth;
		public Int32 biHeight;
		public short biPlanes;
		public short biBitCount;
		public Int32 biCompression;
		public Int32 biSizeImage;
		public Int32 biXPelsPerMeter;
		public Int32 biYPelsPerMeter;
		public Int32 biClrUsed;
		public Int32 biClrImportant;
	} 
 
	[StructLayout(LayoutKind.Sequential, Pack=1)]
	public struct BITMAPINFO
	{
		public BITMAPINFOHEADER bmiHeader;
		public Int32[] bmiColors;
	}

	[StructLayout(LayoutKind.Sequential, Pack=1)]
	public struct RECT
	{
		public int left;
		public int right;
		public int top;
		public int bottom;
	}
}
