using System;

namespace PsdFramework
{
	/// <summary>
	/// ResolutionInfo class
	/// </summary>
	public class ResolutionInfo
	{
		//  ResolutionInfo class
		//	Type		Name	Description
		//-------------------------------------------
		//	Fixed		hRes		Horizontal resolution in pixels per inch.
		//	int			hResUnit	1=display horizontal resolution in pixels per inch;
		//							2=display horizontal resolution in pixels per cm.
		//	short		widthUnit	Display width as 1=inches; 2=cm; 3=points; 4=picas; 5=columns.
		//	Fixed		vRes		Vertical resolution in pixels per inch.
		//	int			vResUnit	1=display vertical resolution in pixels per inch;
		//							2=display vertical resolution in pixels per cm.
		//	short		heightUnit	Display height as 1=inches; 2=cm; 3=points; 4=picas; 5=columns.
		
		public short hRes;
		public int hResUnit;
		public short widthUnit;

		public short vRes;
		public int vResUnit;
		public short heightUnit;

		public ResolutionInfo()
		{
			hRes = -1;
			hResUnit = -1;
			widthUnit = -1;
			vRes = -1;
			vResUnit = -1;
			heightUnit = -1;
		}
	}
}
