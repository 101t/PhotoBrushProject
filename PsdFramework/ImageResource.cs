using System;

namespace PsdFramework
{
	/// <summary>
	/// Image resource block
	/// </summary>
	public class ImageResource
	{
		//  Image resource block
		//	Type		Name	Description
		//-------------------------------------------
		//	OSType		Type	Photoshop always uses its signature, 8BIM
		//	int16		ID		Unique identifier
		//	PString		Name	A pascal string, padded to make size even (a null name consists of two bytes of 0)
		//						Pascal style string where the first byte gives the length of the
		//						string and the content bytes follow.
		//	int32		Size	Actual size of resource data. This does not include the
		//						Type, ID, Name, or Size fields.
		//	Variable	Data	Resource data, padded to make size even

		public int nLength;
		public byte[] OSType = new byte[4];
		public short nID;
		public byte[] Name;
		public int	nSize;

		public void Reset()
		{
			nLength = -1;
			for(int i=0;i<4;i++) OSType[i] = 0x00;
			nID = -1;
			nSize = -1;
		}

		public ImageResource()
		{
			Reset();
		}
	}
}
