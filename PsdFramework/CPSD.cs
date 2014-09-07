using System;
using System.IO;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace PsdFramework
{
	/// <summary>
	/// Main class is for opening Adobe Photoshop files
	/// </summary>
	public class CPSD
	{

		private HeaderInfo       m_HeaderInfo;
		private ColorModeData    m_ColorModeData;
		private ImageResource    m_ImageResource;
		private ResolutionInfo   m_ResolutionInfo;
		private DisplayInfo      m_DisplayInfo;
		private ThumbNail        m_ThumbNail;

		private bool	m_bResolutionInfoFilled;
		private bool	m_bThumbnailFilled;
		private bool    m_bCopyright;
		
		private short   m_nColourCount;
		private short   m_nTransparentIndex;
		private int		m_nGlobalAngle;
		private int		m_nCompression;
		private	IntPtr  m_hBitmap;
		
		public CPSD()
		{
			m_bResolutionInfoFilled = false;
			m_bThumbnailFilled = false;
			m_bCopyright = false;
			m_nColourCount = -1;
			m_nTransparentIndex = -1;
			m_nGlobalAngle = 30;
			m_nCompression = -1;
			m_hBitmap = IntPtr.Zero;
		}
		// construction, destruction
		
		public int Load(string strPathName)
		{
			int nErrorCode = 0;	// No errors

			FileStream stream = new FileStream(strPathName, FileMode.Open, FileAccess.Read, FileShare.Read);

			if(!stream.Handle.Equals(0))
			{
				bool bSuccessHeader = false;
				bool bSuccessColourModeData = false;
				bool bSuccessImageResource = false;
				bool bSuccessLayerMaskInfo = false;

				try
				{
					bSuccessHeader = ReadHeader(stream);
					if(bSuccessHeader == false)
						nErrorCode = -2;	// Error in header
				}
				catch(Exception)
				{
					bSuccessHeader = false;
					nErrorCode = -2;
				}
				
				if(bSuccessHeader)
				{
					try
					{
						bSuccessColourModeData = ReadColourModeData(stream);
						if (bSuccessColourModeData == false)
							nErrorCode = -3; // Error in ColourMode Data
					}
					catch(Exception)
					{
						bSuccessColourModeData = false;
						nErrorCode = -3;
					}
				}
				
				if(bSuccessColourModeData)
				{
					try
					{
						bSuccessImageResource = ReadImageResource(stream);
						if (bSuccessImageResource == false)
							nErrorCode = -4;	// Error in Image Resource
					}
					catch(Exception)
					{
						bSuccessImageResource = false;
						nErrorCode = -4;
					}
				}

				if(bSuccessImageResource)
				{
					try
					{
						bSuccessLayerMaskInfo = ReadLayerAndMaskInfoSection(stream);
						if ( false == bSuccessLayerMaskInfo )
							nErrorCode = -5;	// Error in Mask Info
					}
					catch (Exception)
					{
						bSuccessLayerMaskInfo = false;
						nErrorCode = -5;
					}
				}

				if(bSuccessImageResource)
				{
					try
					{
						nErrorCode = ReadImageData(stream);
					}
					catch(Exception)
					{
						nErrorCode = -6;	// Error in Image Data
					}
				}
			}
			else
				nErrorCode = -1;	// Cannot open file

			return nErrorCode;
		}

		public bool IsThumbnailIncluded()
		{
			return m_bThumbnailFilled; 
		}

		public int GetBitsPerPixel()
		{
			return m_HeaderInfo.nBitsPerPixel; 
		}

		public int GlobalAngle()
		{
			return m_nGlobalAngle; 
		}

		public bool IsCopyrighted()
		{
			return m_bCopyright; 
		}

		public IntPtr GetHBitmap()
		{
			return m_hBitmap;
		}

		public int GetWidth()
		{
			return m_HeaderInfo.nWidth;
		}
		
		public int GetHeight()
		{
			return m_HeaderInfo.nHeight;
		}

		public int GetXResolution()
		{
			return m_ResolutionInfo.hRes;
		}

		public int GetYResolution()
		{
			return m_ResolutionInfo.vRes;
		}

		public int GetCompression()
		{
			return m_nCompression;
		}
		
		protected bool ReadHeader(FileStream stream)
		{
			bool bSuccess = false;

			BinaryReader binReader = new BinaryReader(stream);
			try
			{
				// Set Position to the beginning of the stream.
				binReader.BaseStream.Position = 0;
				byte [] Signature  = binReader.ReadBytes(4); // always equal 8BPS, do not read file if not
				byte [] Version    = binReader.ReadBytes(2); // always equal 1, do not read file if not
				byte [] Reserved   = binReader.ReadBytes(6); // must be zero
				byte [] Channels   = binReader.ReadBytes(2); // numer of channels including any alpha channels, supported range 1 to 24
				byte [] Rows       = binReader.ReadBytes(4); // height in PIXELS, supported range 1 to 30000
				byte [] Columns    = binReader.ReadBytes(4); // width in PIXELS, supported range 1 to 30000
				byte [] Depth      = binReader.ReadBytes(2); // number of bpp
				byte [] Mode       = binReader.ReadBytes(2); // colour mode of the file
				// Btmap=0, Grayscale=1, Indexed=2, RGB=3,
				// CMYK=4, Multichannel=7, Duotone=8, Lab=9
				ASCIIEncoding encoding = new ASCIIEncoding();

				if( encoding.GetString(Signature).Equals("8BPS") && Version[1] == 0x01)
				{
					m_HeaderInfo = new HeaderInfo(); 

					if(SwapBytes(Channels,2)) m_HeaderInfo.nChannels = BitConverter.ToInt16(Channels,0);
					if(SwapBytes(Rows,4)) m_HeaderInfo.nHeight = BitConverter.ToInt32(Rows,0);
					if(SwapBytes(Columns,4)) m_HeaderInfo.nWidth = BitConverter.ToInt32(Columns,0);
					if(SwapBytes(Depth,2)) m_HeaderInfo.nBitsPerPixel = BitConverter.ToInt16(Depth,0);
					if(SwapBytes(Mode,2)) m_HeaderInfo.nColourMode = BitConverter.ToInt16(Mode,0);

					if(m_HeaderInfo.nChannels != -1 && m_HeaderInfo.nHeight != -1 && m_HeaderInfo.nWidth != -1 && m_HeaderInfo.nBitsPerPixel != -1 && m_HeaderInfo.nColourMode != -1) bSuccess = true;
				}
			}
			catch(EndOfStreamException e)
			{
				System.Windows.Forms.MessageBox.Show("Exception occurs while reading file header: {0}", e.GetType().Name);
			}

			return bSuccess;
		}

		protected bool ReadColourModeData(FileStream stream)
		{
			bool bSuccess = false;
			
			// Only indexed colour and duotone have colour mode data,
			// for all other modes this section is 4 bytes length, the length field is set to zero

			// For indexed color images, the length will be equal to 768, and the color
			// will contain the color table for the image, in non–interleaved order.

			// For duotone images, the color data will contain the duotone specification,
			// the format of which is not documented. Other applications that read
			// Photoshop files can treat a duotone image as a grayscale image, and just
			// preserve the contents of the duotone information when reading and writing
			// the file.
			
			BinaryReader binReader = new BinaryReader(stream);
			try
			{
				// Set Position to the beginning of the stream + size of HeaderInfo.
				binReader.BaseStream.Position = 26;
				byte [] ColorMode  = binReader.ReadBytes(4);

				int nLength = 0;

				m_ColorModeData = new ColorModeData();
				if(SwapBytes(ColorMode,4)) nLength = BitConverter.ToInt32(ColorMode,0);
				m_ColorModeData.nLength = nLength;

				if(nLength>0)
				{
					m_ColorModeData.ColourData = new byte[nLength];
					m_ColorModeData.ColourData = binReader.ReadBytes(nLength);
				}
				
				bSuccess = true;
			}
			catch(EndOfStreamException e)
			{
				MessageBox.Show("Exception occurs while reading color mode data: {0}", e.GetType().Name);
			}

			return bSuccess;
		}

		protected bool ReadImageResource(FileStream stream)
		{
			bool bSuccess = false;
			
			BinaryReader binReader = new BinaryReader(stream);
			try
			{
				int nLength = 0;

				byte [] Length  = binReader.ReadBytes(4);

				m_ImageResource = new ImageResource();
				m_ResolutionInfo = new ResolutionInfo();
				m_DisplayInfo = new DisplayInfo();
				m_ThumbNail = new ThumbNail();
 
				if(SwapBytes(Length,4)) nLength = BitConverter.ToInt32(Length,0);
				m_ImageResource.nLength = nLength;

				int nBytesRead = 0;
				int nTotalBytes = m_ImageResource.nLength;
				long nStreamLen = stream.Length;

				while(stream.Position < nStreamLen && nBytesRead < nTotalBytes)
				{
					m_ImageResource.Reset();
					m_ImageResource.OSType  = binReader.ReadBytes(4);
					nBytesRead += 4;

					ASCIIEncoding encoding = new ASCIIEncoding();
					
					if(encoding.GetString(m_ImageResource.OSType).Equals("8BIM"))
					{
						byte [] ID = binReader.ReadBytes(2);
						nBytesRead += 2;

						if(SwapBytes(ID,2)) m_ImageResource.nID = BitConverter.ToInt16(ID,0);

						byte SizeOfName = binReader.ReadByte();
						nBytesRead += 1;
						
						int nSizeOfName = (int)SizeOfName;
						if(nSizeOfName>0)
						{
							if((nSizeOfName % 2)!=0)	// must read 1 more byte to make size even
							{
								SizeOfName = binReader.ReadByte();
								nBytesRead += 1;
							}
							
							m_ImageResource.Name = new byte[nSizeOfName];
							m_ImageResource.Name = binReader.ReadBytes(nSizeOfName);
							nBytesRead += nSizeOfName;
						}
						
						SizeOfName = binReader.ReadByte();
						nBytesRead += 1;

						byte [] Size = binReader.ReadBytes(4);
						if(SwapBytes(Size,4)) m_ImageResource.nSize = BitConverter.ToInt32(Size,0);
						nBytesRead += 4;

						if((m_ImageResource.nSize % 2) != 0)	// resource data must be even
							m_ImageResource.nSize++;
						
						if(m_ImageResource.nSize>0)
						{
							byte [] IntValue = new byte[4];
							byte [] ShortValue = new byte[2];

							switch(m_ImageResource.nID)
							{
								case 1005:
								{
									m_bResolutionInfoFilled = true;

									ShortValue = binReader.ReadBytes(2);
									if(SwapBytes(ShortValue,2)) m_ResolutionInfo.hRes = BitConverter.ToInt16(ShortValue,0);
									nBytesRead += 2;
									IntValue = binReader.ReadBytes(4);
									if(SwapBytes(IntValue,4)) m_ResolutionInfo.hResUnit = BitConverter.ToInt32(IntValue,0);
									nBytesRead += 4;
									ShortValue = binReader.ReadBytes(2);
									if(SwapBytes(ShortValue,2)) m_ResolutionInfo.widthUnit = BitConverter.ToInt16(ShortValue,0);
									nBytesRead += 2;
									ShortValue = binReader.ReadBytes(2);
									if(SwapBytes(ShortValue,2)) m_ResolutionInfo.vRes = BitConverter.ToInt16(ShortValue,0);
									nBytesRead += 2;
									IntValue = binReader.ReadBytes(4);
									if(SwapBytes(IntValue,4)) m_ResolutionInfo.vResUnit = BitConverter.ToInt32(IntValue,0);
									nBytesRead += 4;
									ShortValue = binReader.ReadBytes(2);
									if(SwapBytes(ShortValue,2)) m_ResolutionInfo.heightUnit = BitConverter.ToInt16(ShortValue,0);
									nBytesRead += 2;
								}
									break;
								case 1007:
								{
									ShortValue = binReader.ReadBytes(2);
									nBytesRead += 2;
									if(SwapBytes(ShortValue,2)) m_DisplayInfo.ColourSpace = BitConverter.ToInt16(ShortValue,0);

									for(int n=0; n<4; ++n)
									{
										ShortValue = binReader.ReadBytes(2);
										nBytesRead += 2;
										if(SwapBytes(ShortValue,2)) m_DisplayInfo.Colour[n] = BitConverter.ToInt16(ShortValue,0);
									}
					
									ShortValue = binReader.ReadBytes(2);
									nBytesRead += 2;
									if(SwapBytes(ShortValue,2)) m_DisplayInfo.Opacity = BitConverter.ToInt16(ShortValue,0);
									if(m_DisplayInfo.Opacity<0 || m_DisplayInfo.Opacity>100) m_DisplayInfo.Opacity = 100;

									byte c = binReader.ReadByte();
									nBytesRead += 1;
									if(c == 0x00) m_DisplayInfo.kind = false;
									else m_DisplayInfo.kind = true;

									nBytesRead += 1;
									m_DisplayInfo.padding = binReader.ReadByte();
								}
									break;
								case 1034:
								{
									ShortValue = binReader.ReadBytes(2);
									nBytesRead += 2;
									int nCopyright = 0;
									if(SwapBytes(ShortValue,2)) nCopyright = BitConverter.ToInt16(ShortValue,0);
									if(nCopyright>0) m_bCopyright = true;
									else m_bCopyright = false;
								}
									break;
								case 1033:
								case 1036:
								{
									m_bThumbnailFilled = true;

									IntValue = binReader.ReadBytes(4);
									if(SwapBytes(IntValue,4)) m_ThumbNail.nFormat = BitConverter.ToInt32(IntValue,0);
									nBytesRead += 4;

									IntValue = binReader.ReadBytes(4);
									if(SwapBytes(IntValue,4)) m_ThumbNail.nWidth = BitConverter.ToInt32(IntValue,0);
									nBytesRead += 4;

									IntValue = binReader.ReadBytes(4);
									if(SwapBytes(IntValue,4)) m_ThumbNail.nHeight = BitConverter.ToInt32(IntValue,0);
									nBytesRead += 4;

									IntValue = binReader.ReadBytes(4);
									if(SwapBytes(IntValue,4)) m_ThumbNail.nWidthBytes = BitConverter.ToInt32(IntValue,0);
									nBytesRead += 4;

									IntValue = binReader.ReadBytes(4);
									if(SwapBytes(IntValue,4)) m_ThumbNail.nSize = BitConverter.ToInt32(IntValue,0);
									nBytesRead += 4;

									IntValue = binReader.ReadBytes(4);
									if(SwapBytes(IntValue,4)) m_ThumbNail.nCompressedSize = BitConverter.ToInt32(IntValue,0);
									nBytesRead += 4;
									
									ShortValue = binReader.ReadBytes(2);
									nBytesRead += 2;
									if(SwapBytes(ShortValue,2)) m_ThumbNail.nBitPerPixel = BitConverter.ToInt16(ShortValue,0);

									ShortValue = binReader.ReadBytes(2);
									nBytesRead += 2;
									if(SwapBytes(ShortValue,2)) m_ThumbNail.nPlanes = BitConverter.ToInt16(ShortValue,0);
									
									int nTotalData = m_ImageResource.nSize - 28; // header

									byte [] buffer = new byte[nTotalData];
									byte c;
									if(m_ImageResource.nID == 1033)
									{
										// In BGR format
										for(int n=0; n<nTotalData; n=n+3)
										{
											c = binReader.ReadByte();
											nBytesRead += 1;
											buffer[n+2] = c;
											c = binReader.ReadByte();
											nBytesRead += 1;
											buffer[n+1] = c;
											c = binReader.ReadByte();
											nBytesRead += 1;
											buffer[n] = c;
										}
									}
									else if(m_ImageResource.nID == 1036)
									{
										// In RGB format										
										for (int n=0; n<nTotalData; ++n)
										{
											c = binReader.ReadByte();
											nBytesRead += 1;
											buffer[n] = c;
										}
									}
								}
									break;
								case 1037:
								{
									IntValue = binReader.ReadBytes(4);
									if(SwapBytes(IntValue,4)) m_nGlobalAngle = BitConverter.ToInt32(IntValue,0);
									nBytesRead += 4;
								}
									break;
								case 1046:
								{
									ShortValue = binReader.ReadBytes(2);
									nBytesRead += 2;
									if(SwapBytes(ShortValue,2)) m_nColourCount = BitConverter.ToInt16(ShortValue,0);
								}
									break;
								case 1047:
								{
									ShortValue = binReader.ReadBytes(2);
									nBytesRead += 2;
									if(SwapBytes(ShortValue,2)) m_nTransparentIndex = BitConverter.ToInt16(ShortValue,0);
								}
									break;
								default:
								{
									byte c;
									for(int n=0; n<m_ImageResource.nSize; ++n )
									{
										c = binReader.ReadByte();
										nBytesRead += 1;
									}
								}
									break;
							}
						}
					}
				}
				bSuccess = true;
			}
			catch(EndOfStreamException e)
			{
				bSuccess = false;
				MessageBox.Show("Exception occurs while reading image resources: {0}", e.GetType().Name);
			}
			
			
			return bSuccess;
		}

		protected bool ReadLayerAndMaskInfoSection(FileStream stream) // currently ignore it
		{
			bool bSuccess = false;
			
			BinaryReader binReader = new BinaryReader(stream);
			try
			{
				byte [] DataLength = new byte[4];
				int nTotalBytes =  0;

				DataLength = binReader.ReadBytes(4);
				if(SwapBytes(DataLength,4)) nTotalBytes = BitConverter.ToInt32(DataLength,0);
				
				if(stream.Position+nTotalBytes < stream.Length) stream.Position += nTotalBytes;

				//				byte c = 0x00;
				//				for(int i=0;i<nTotalBytes;i++)
				//				{
				//					if(binReader.PeekChar() == -1) break;
				//					c = binReader.ReadByte();
				//				}

				bSuccess = true;
			}
			catch(EndOfStreamException e)
			{
				bSuccess = false;
				MessageBox.Show("Exception occurs while reading layer and mask section: {0}", e.GetType().Name);
			}
			
			return bSuccess;
		}

		protected int ReadImageData(FileStream stream)
		{
			int nErrorCode = 0;	// No Errors
			
			BinaryReader binReader = new BinaryReader(stream);
			try
			{
				byte [] ShortValue = new byte[2];
				int nBytesRead = 0;
				short nCompression = 0;
				
				ShortValue = binReader.ReadBytes(2);
				if(SwapBytes(ShortValue,2)) nCompression = BitConverter.ToInt16(ShortValue,0);

				m_nCompression = nCompression;

				switch(nCompression)
				{
					case 0: // raw data
					{
						int nWidth = m_HeaderInfo.nWidth;
						int nHeight = m_HeaderInfo.nHeight;
						int bytesPerPixelPerChannel = m_HeaderInfo.nBitsPerPixel/8;
						
						int nPixels = nWidth * nHeight;
						int nTotalBytes = nPixels * bytesPerPixelPerChannel * m_HeaderInfo.nChannels;

						byte [] pData = new byte[nTotalBytes];
						byte [] pImageData = new byte[nTotalBytes];
						
						switch(m_HeaderInfo.nColourMode)
						{
							case 1:		// Grayscale
							case 2:		// Indexed
							case 8:		// Duotone
							{
								pData = new byte[nTotalBytes];
								pImageData = new byte[bytesPerPixelPerChannel];
								
								for(int i=0;i<nTotalBytes;i++) pData[i] = 254;
								for(int i=0;i<bytesPerPixelPerChannel;i++) pImageData[i] = 254;
								
								while(nBytesRead<nTotalBytes)
								{
									pImageData = binReader.ReadBytes(bytesPerPixelPerChannel);
									
									for(int j=0;j<bytesPerPixelPerChannel;j++)
										pData[nBytesRead+j] = pImageData[j];

									nBytesRead += bytesPerPixelPerChannel;
								}
							}
								break;
							case 3:		// RGB
							{
								int nBytesToReadPerPixelPerChannel = bytesPerPixelPerChannel;
								if (nBytesToReadPerPixelPerChannel == 2)
								{
									nBytesToReadPerPixelPerChannel = 1;
									nTotalBytes = nPixels * nBytesToReadPerPixelPerChannel * m_HeaderInfo.nChannels;
								}

								pImageData = new byte[nBytesToReadPerPixelPerChannel];
								pData = new byte[nTotalBytes];
								for(int i=0;i<nTotalBytes;i++) pData[i] = 254;
								for(int i=0;i<nBytesToReadPerPixelPerChannel;i++) pImageData[i] = 254;
								
								int nPixelCounter = 0;
								
								for(int nColour = 0; nColour<3; ++nColour)
								{
									nPixelCounter = nColour;
									for(int nPos=0; nPos<nPixels; ++nPos)
									{
										if(nBytesRead < nTotalBytes)
										{
											pImageData = binReader.ReadBytes(nBytesToReadPerPixelPerChannel);

											for(int j=0;j<nBytesToReadPerPixelPerChannel;j++)
												pData[nPixelCounter+j] = pImageData[j];

											nBytesRead += nBytesToReadPerPixelPerChannel;
											nPixelCounter += 3;
											if(bytesPerPixelPerChannel == 2)
												pImageData = binReader.ReadBytes(nBytesToReadPerPixelPerChannel);
										}
									}
								}
							}
								break;
							case 4:	// CMYK
							{
								pImageData = new byte[bytesPerPixelPerChannel];
								pData = new byte[nTotalBytes];
								for(int i=0;i<nTotalBytes;i++) pData[i] = 254;
								for(int i=0;i<bytesPerPixelPerChannel;i++) pImageData[i] = 254;

								int nPixelCounter = 0;

								for(int nColour = 0; nColour<4; ++nColour)
								{
									nPixelCounter = nColour*bytesPerPixelPerChannel;
									for(int nPos=0; nPos<nPixels; ++nPos)
									{
										if(nBytesRead<nTotalBytes)
										{
											pImageData = binReader.ReadBytes(bytesPerPixelPerChannel);

											for(int j=0;j<bytesPerPixelPerChannel;j++)
												pData[nPixelCounter+j] = pImageData[j];

											nBytesRead += bytesPerPixelPerChannel;
											nPixelCounter += 4*bytesPerPixelPerChannel;
										}
									}
								}
							}
								break;
							case 9:	// Lab
							{
								pImageData = new byte[bytesPerPixelPerChannel];
								pData = new byte[nTotalBytes];
								for(int i=0;i<nTotalBytes;i++) pData[i] = 254;
								for(int i=0;i<bytesPerPixelPerChannel;i++) pImageData[i] = 254;

								int nPixelCounter = 0;
								
								for(int nColour = 0; nColour<3; ++nColour)
								{
									nPixelCounter = nColour*bytesPerPixelPerChannel;
									
									for(int nPos=0; nPos<nPixels; ++nPos)
									{
										if(nBytesRead<nTotalBytes)
										{
											pImageData = binReader.ReadBytes(bytesPerPixelPerChannel);
											
											for(int j=0;j<bytesPerPixelPerChannel;j++)
												pData[nPixelCounter+j] = pImageData[j];
											
											nBytesRead += bytesPerPixelPerChannel;
											
											nPixelCounter += 3*bytesPerPixelPerChannel;
										}
									}
								}
							}
								break;
						}

						if(nBytesRead == nTotalBytes)
						{
							int ppm_x = 3780;	// 96 dpi
							int ppm_y = 3780;	// 96 dpi

							if(m_bResolutionInfoFilled)
							{
								int nHorzResolution = (int)m_ResolutionInfo.hRes;
								int nVertResolution = (int)m_ResolutionInfo.vRes;

								ppm_x = ( nHorzResolution * 10000 ) / 254;
								ppm_y = ( nVertResolution * 10000 ) / 254;
							}

							switch(m_HeaderInfo.nBitsPerPixel)
							{
								case 1:
								{
									nErrorCode = -7; // Not yet implemented
								}
									break;
								case 8:
								case 16:
								{
									CreateDIBSection(nWidth, nHeight, ppm_x, ppm_y, 24);
								}
									break;
								default:
								{
									nErrorCode = -8;	// Unsupported format
								}
									break;
							}
							
							IntPtr hBitmap = m_hBitmap;

							if(hBitmap == IntPtr.Zero)
							{
								nErrorCode = -9;	// Cannot create hBitmap
								return nErrorCode;
							}

							ProccessBuffer(pData);
						}
					}
						break;
					case 1:	// RLE compression
					{
						int nWidth = m_HeaderInfo.nWidth;
						int nHeight = m_HeaderInfo.nHeight;
						int bytesPerPixelPerChannel = m_HeaderInfo.nBitsPerPixel/8;
						
						int nPixels = nWidth * nHeight;
						int nTotalBytes = nPixels * bytesPerPixelPerChannel * m_HeaderInfo.nChannels;

						byte [] pDest = new byte[nTotalBytes];
						byte [] pData = new byte[nTotalBytes];
						for(long i=0;i<nTotalBytes;i++) pData[i] = 254;
						for(long i=0;i<nTotalBytes;i++) pDest[i] = 254;

						byte ByteValue = 0x00;

						int Count = 0;

						int nPointer = 0;

						// The RLE-compressed data is proceeded by a 2-byte data count for each row in the data,
						// which we're going to just skip.
						stream.Position += nHeight * m_HeaderInfo.nChannels*2;


						for(int channel=0; channel<m_HeaderInfo.nChannels; channel++)
						{
							// Read the RLE data.
							Count = 0;
							while(Count<nPixels)
							{
								ByteValue = binReader.ReadByte();

								int len = (int)ByteValue;
								if(len < 128)
								{
									len++;
									Count += len;

									while(len!=0)
									{
										ByteValue = binReader.ReadByte();

										pData[nPointer] = ByteValue;
										nPointer++;
										len--;
									}
								}
								else if(len > 128)
								{
									// Next -len+1 bytes in the dest are replicated from next source byte.
									// (Interpret len as a negative 8-bit int.)
									len ^= 0x0FF;
									len += 2;
									ByteValue = binReader.ReadByte();

									Count += len;

									while(len!=0)
									{
										pData[nPointer] = ByteValue;
										nPointer++;
										len--;
									}
								}
								else if ( 128 == len )
								{
									// Do nothing
								}
							}
						}

						int nPixelCounter = 0;
						nPointer = 0;

						for(int nColour = 0; nColour<m_HeaderInfo.nChannels; ++nColour)
						{
							nPixelCounter = nColour*bytesPerPixelPerChannel;
							for(int nPos=0; nPos<nPixels; ++nPos)
							{
								for(int j=0;j<bytesPerPixelPerChannel;j++)
									pDest[nPixelCounter+j] = pData[nPointer+j];

								nPointer++;

								nPixelCounter += m_HeaderInfo.nChannels*bytesPerPixelPerChannel;
							}
						}

						for(int i=0;i<nTotalBytes;i++) pData[i]=pDest[i];
						
						int ppm_x = 3780;	// 96 dpi
						int ppm_y = 3780;	// 96 dpi

						if(m_bResolutionInfoFilled)
						{
							int nHorResolution = (int)m_ResolutionInfo.hRes;
							int nVertResolution = (int)m_ResolutionInfo.vRes;

							ppm_x = (nHorResolution * 10000 )/254;
							ppm_y = (nVertResolution * 10000 )/254;
						}

						switch (m_HeaderInfo.nBitsPerPixel)
						{
							case 1:
							{
								nErrorCode = -7; // Not yet implemented
							}
								break;
							case 8:
							case 16:
							{
								CreateDIBSection(nWidth, nHeight, ppm_x, ppm_y, 24);
							}
								break;
							default:
							{
								nErrorCode = -8;	// Unsupported format
							}
								break;
						}

						IntPtr hBitmap = m_hBitmap;

						if(hBitmap == IntPtr.Zero)
						{
							nErrorCode = -9;	// Cannot create hBitmap
							return nErrorCode;
						}

						ProccessBuffer(pData);
					}
						break;
					case 2:	// ZIP without prediction
					{
						nErrorCode = -10;	// ZIP without prediction, no specification
					}
						break;
					case 3:	// ZIP with prediction
					{
						nErrorCode = -11;	// ZIP with prediction, no specification
					}
						break;
					default:
					{
						nErrorCode = -12;	// Unknown format
					}
						break;
				}
			}
			catch(EndOfStreamException e)
			{
				nErrorCode = -6;
				MessageBox.Show("Exception occurs while reading image data: {0}", e.GetType().Name);
			}

			return nErrorCode;
		}

		protected void CreateDIBSection (int cx, int cy, int ppm_x, int ppm_y, short BitCount)
		{
			IntPtr hDC = WinInvoke32.GetDC(IntPtr.Zero);
		
			if(hDC.Equals(IntPtr.Zero)) return;

			IntPtr pvBits = IntPtr.Zero;
			BITMAPINFO BitmapInfo = new BITMAPINFO();
			BitmapInfo.bmiHeader = new BITMAPINFOHEADER();

			BitmapInfo.bmiHeader.biSize               = 40;
			BitmapInfo.bmiHeader.biWidth              = cx;
			BitmapInfo.bmiHeader.biHeight             = cy;
			BitmapInfo.bmiHeader.biPlanes             = 1;
			BitmapInfo.bmiHeader.biBitCount           = BitCount;
			BitmapInfo.bmiHeader.biCompression        = WinInvoke32.BI_RGB;
			BitmapInfo.bmiHeader.biSizeImage          = 0;
			BitmapInfo.bmiHeader.biXPelsPerMeter      = ppm_x; 
			BitmapInfo.bmiHeader.biYPelsPerMeter      = ppm_y;
			BitmapInfo.bmiHeader.biClrImportant       = 0;
			BitmapInfo.bmiHeader.biClrUsed            = 0;

			m_hBitmap = WinInvoke32.CreateDIBSection(hDC, ref BitmapInfo, 0, pvBits, IntPtr.Zero, 0);

			if(m_hBitmap.Equals(IntPtr.Zero))  
			{
				WinInvoke32.ReleaseDC(IntPtr.Zero, hDC);
				return;
			}
			else
			{
				IntPtr hdcMemory = WinInvoke32.CreateCompatibleDC(hDC);
				IntPtr hbmpOld = WinInvoke32.SelectObject( hdcMemory, m_hBitmap );
				RECT rc;
	
				rc.left = rc.top = 0;
				rc.right = cx;
				rc.bottom = cy;
				IntPtr hBrush = WinInvoke32.GetStockObject(WinInvoke32.WHITE_BRUSH);
				WinInvoke32.FillRect(hdcMemory, ref rc, hBrush);
				WinInvoke32.SelectObject(hdcMemory, hbmpOld);
				WinInvoke32.DeleteDC(hdcMemory);
			}

			WinInvoke32.ReleaseDC(IntPtr.Zero, hDC);
		}

		protected void ProccessBuffer(byte [] pData )
		{
			if(m_hBitmap.Equals(IntPtr.Zero)) return;

			IntPtr hBitmap = m_hBitmap;

			IntPtr hdcMemory = IntPtr.Zero;
			IntPtr hbmpOld = IntPtr.Zero;

			int nHeight = m_HeaderInfo.nHeight;
			int nWidth = m_HeaderInfo.nWidth;
			short bytesPerPixelPerChannel = (short)(m_HeaderInfo.nBitsPerPixel/8);
			int nPixels = nWidth * nHeight;
			int nTotalBytes = nPixels * bytesPerPixelPerChannel * m_HeaderInfo.nChannels;
			
			switch (m_HeaderInfo.nColourMode)
			{
				case 1:		// Grayscale
				case 8:		// Duotone
				{
					hdcMemory = WinInvoke32.CreateCompatibleDC(IntPtr.Zero);
					hbmpOld = WinInvoke32.SelectObject(hdcMemory, hBitmap);

					int nCounter = 0;
					int nValue = 0;
					int nColor = 0;


					byte [] ColorValue = new byte[64];

					for(int nRow=0; nRow<nHeight; ++nRow)
					{
						for(int nCol=0; nCol<nWidth; ++nCol)
						{
							for(int i=0;i<bytesPerPixelPerChannel;i++)
								ColorValue[i] = pData[nCounter+i];

							SwapBytes(ColorValue,bytesPerPixelPerChannel);
							
							nValue = BitConverter.ToInt32(ColorValue,0);
							if(m_HeaderInfo.nBitsPerPixel == 16)
								nValue = nValue/256;

							if(nValue>255) nValue = 255;
							else if(nValue<0) nValue = 0;

							nColor = ColorTranslator.ToWin32(Color.FromArgb(nValue, nValue, nValue));
							WinInvoke32.SetPixel(hdcMemory, nCol, nRow, nColor);
							
							nCounter += bytesPerPixelPerChannel;
						}
					}
					WinInvoke32.SelectObject(hdcMemory, hbmpOld);
					WinInvoke32.DeleteDC(hdcMemory);
				}
					break;
				case 2:		// Indexed
				{
					hdcMemory = WinInvoke32.CreateCompatibleDC(IntPtr.Zero);
					hbmpOld = WinInvoke32.SelectObject(hdcMemory, hBitmap);
					// pData holds the indices of loop through the palette and set the correct RGB
					// 8bpp are supported
					if(m_ColorModeData.nLength==768 && m_nColourCount>0)
					{
						int nRow = 0;
						int nCol = 0;
						int nRed = 0;
						int nGreen = 0;
						int nBlue = 0;
						int nIndex = 0;
						int nColor = 0;

						for(int nCounter=0; nCounter<nTotalBytes; ++nCounter)
						{
							nIndex = (int)pData[nCounter];
							nRed = (int)m_ColorModeData.ColourData[nIndex];
							nGreen = (int)m_ColorModeData.ColourData[nIndex+256];
							nBlue = (int)m_ColorModeData.ColourData[nIndex+2*256];

							nColor = ColorTranslator.ToWin32(Color.FromArgb(nRed, nGreen, nBlue));
							WinInvoke32.SetPixel(hdcMemory, nCol, nRow, nColor);
							nCol++;
							if(nWidth <= nCol)
							{
								nCol = 0;
								nRow++;
							}
						}
					}

					WinInvoke32.SelectObject(hdcMemory, hbmpOld);
					WinInvoke32.DeleteDC(hdcMemory);
				}
					break;
				case 3:	// RGB
				{
					hdcMemory = WinInvoke32.CreateCompatibleDC(IntPtr.Zero);
					hbmpOld = WinInvoke32.SelectObject(hdcMemory, hBitmap);

					int nBytesToRead = m_HeaderInfo.nBitsPerPixel/8;
					if(nBytesToRead == 2)
						nBytesToRead = 1;

					int nRow = 0;
					int nCol = 0;
					int nRed = 0;
					int nGreen = 0;
					int nBlue = 0;
					int nColor = 0;
					byte [] ColorValue = new byte[8];

					for(int nCounter = 0; nCounter < nTotalBytes; nCounter += m_HeaderInfo.nChannels * nBytesToRead)
					{
						Array.Copy(pData,nCounter,ColorValue,0,nBytesToRead);
						SwapBytes(ColorValue,nBytesToRead);
						nRed = BitConverter.ToInt32(ColorValue,0);
						
						Array.Copy(pData,nCounter+nBytesToRead,ColorValue,0,nBytesToRead);
						SwapBytes(ColorValue,nBytesToRead);
						nGreen = BitConverter.ToInt32(ColorValue,0);
						
						Array.Copy(pData,nCounter+2*nBytesToRead,ColorValue,0,nBytesToRead);
						SwapBytes(ColorValue,nBytesToRead);
						nBlue = BitConverter.ToInt32(ColorValue,0);

						nColor = ColorTranslator.ToWin32(Color.FromArgb(nRed, nGreen, nBlue));
						WinInvoke32.SetPixel(hdcMemory, nCol, nRow, nColor);
						nCol++;
						if ( nWidth <= nCol )
						{
							nCol = 0;
							nRow++;
						}
					}

					WinInvoke32.SelectObject(hdcMemory, hbmpOld);
					WinInvoke32.DeleteDC(hdcMemory);
				}
					break;
				case 4:	// CMYK
				{
					hdcMemory = WinInvoke32.CreateCompatibleDC(IntPtr.Zero);
					hbmpOld = WinInvoke32.SelectObject(hdcMemory, hBitmap);

					double C, M, Y, K;
					double exC, exM, exY, exK;

					int nRow = 0;
					int nCol = 0;
					int nColor = 0;

					byte [] ColorValue = new byte[8];

					double dMaxColours = Math.Pow(2, m_HeaderInfo.nBitsPerPixel);

					Color crPixel = Color.White;

					for(int nCounter = 0; nCounter < nTotalBytes; nCounter += 4*bytesPerPixelPerChannel)
					{
						Array.Copy(pData,nCounter,ColorValue,0,bytesPerPixelPerChannel);
						SwapBytes(ColorValue,bytesPerPixelPerChannel);
						exC = (double)BitConverter.ToUInt32(ColorValue,0);

						Array.Copy(pData,nCounter+bytesPerPixelPerChannel,ColorValue,0,bytesPerPixelPerChannel);
						SwapBytes(ColorValue,bytesPerPixelPerChannel);
						exM = (double)BitConverter.ToUInt32(ColorValue,0);

						Array.Copy(pData,nCounter+2*bytesPerPixelPerChannel,ColorValue,0,bytesPerPixelPerChannel);
						SwapBytes(ColorValue,bytesPerPixelPerChannel);
						exY = (double)BitConverter.ToUInt32(ColorValue,0);

						Array.Copy(pData,nCounter+3*bytesPerPixelPerChannel,ColorValue,0,bytesPerPixelPerChannel);
						SwapBytes(ColorValue,bytesPerPixelPerChannel);
						exK = (double)BitConverter.ToUInt32(ColorValue,0);

						C = (1.0 - exC/dMaxColours);
						M = (1.0 - exM/dMaxColours);
						Y = (1.0 - exY/dMaxColours);
						K = (1.0 - exK/dMaxColours);

						crPixel = CMYKToRGB(C, M, Y, K);

						nColor = ColorTranslator.ToWin32(crPixel);
						WinInvoke32.SetPixel(hdcMemory, nCol, nRow, nColor);
						nCol++;
						if(nWidth<= nCol)
						{
							nCol = 0;
							nRow++;
						}
					}

					WinInvoke32.SelectObject(hdcMemory, hbmpOld);
					WinInvoke32.DeleteDC(hdcMemory);
				}
					break;
				case 7:		// Multichannel
				{
					hdcMemory = WinInvoke32.CreateCompatibleDC(IntPtr.Zero);
					hbmpOld = WinInvoke32.SelectObject(hdcMemory, hBitmap);

					double C, M, Y, K;
					double exC, exM, exY, exK;

					int nRow = 0;
					int nCol = 0;
					int nColor = 0;

					byte [] ColorValue = new byte[8];

					double dMaxColours = Math.Pow(2, m_HeaderInfo.nBitsPerPixel);

					Color crPixel = Color.White;

					// assume format is in either CMY or CMYK
					if(m_HeaderInfo.nChannels>=3)
					{
						for(int nCounter = 0; nCounter < nTotalBytes; nCounter += m_HeaderInfo.nChannels * bytesPerPixelPerChannel)
						{
							Array.Copy(pData,nCounter,ColorValue,0,bytesPerPixelPerChannel);
							SwapBytes(ColorValue,bytesPerPixelPerChannel);
							exC = (double)BitConverter.ToUInt32(ColorValue,0);

							Array.Copy(pData,nCounter+bytesPerPixelPerChannel,ColorValue,0,bytesPerPixelPerChannel);
							SwapBytes(ColorValue,bytesPerPixelPerChannel);
							exM = (double)BitConverter.ToUInt32(ColorValue,0);

							Array.Copy(pData,nCounter+2*bytesPerPixelPerChannel,ColorValue,0,bytesPerPixelPerChannel);
							SwapBytes(ColorValue,bytesPerPixelPerChannel);
							exY = (double)BitConverter.ToUInt32(ColorValue,0);
							
							C = (1.0 - exC/dMaxColours);
							M = (1.0 - exM/dMaxColours);
							Y = (1.0 - exY/dMaxColours);
							K = 0;
							
							if(m_HeaderInfo.nChannels == 4)
							{
								Array.Copy(pData,nCounter+3*bytesPerPixelPerChannel,ColorValue,0,bytesPerPixelPerChannel);
								SwapBytes(ColorValue,bytesPerPixelPerChannel);
								exK = (double)BitConverter.ToUInt32(ColorValue,0);

								K = (1.0 - exK/dMaxColours);
							}

							crPixel = CMYKToRGB(C, M, Y, K);

							nColor = ColorTranslator.ToWin32(crPixel);
							WinInvoke32.SetPixel(hdcMemory, nCol, nRow, nColor);
							nCol++;
							if ( nWidth <= nCol )
							{
								nCol = 0;
								nRow++;
							}
						}
					}

					WinInvoke32.SelectObject(hdcMemory, hbmpOld);
					WinInvoke32.DeleteDC(hdcMemory);
				}
					break;
				case 9:	// Lab
				{
					hdcMemory = WinInvoke32.CreateCompatibleDC(IntPtr.Zero);
					hbmpOld = WinInvoke32.SelectObject(hdcMemory, hBitmap);

					int L, a, b;

					int nRow = 0;
					int nCol = 0;
					int nColor = 0;

					byte [] ColorValue = new byte[64];

					double exL, exA, exB;
					double L_coef, a_coef, b_coef;
					double dMaxColours = Math.Pow(2, m_HeaderInfo.nBitsPerPixel);

					L_coef = dMaxColours/100.0;
					a_coef = dMaxColours/256.0;
					b_coef = dMaxColours/256.0;

					Color crPixel = Color.White;
					
					for(int nCounter = 0; nCounter < nTotalBytes; nCounter += 3 * bytesPerPixelPerChannel)
					{
						Array.Copy(pData,nCounter,ColorValue,0,bytesPerPixelPerChannel);
						SwapBytes(ColorValue,bytesPerPixelPerChannel);
						exL = (double)BitConverter.ToUInt32(ColorValue,0);
						
						Array.Copy(pData,nCounter+bytesPerPixelPerChannel,ColorValue,0,bytesPerPixelPerChannel);
						SwapBytes(ColorValue,bytesPerPixelPerChannel);
						exA = (double)BitConverter.ToUInt32(ColorValue,0);

						Array.Copy(pData,nCounter+2*bytesPerPixelPerChannel,ColorValue,0,bytesPerPixelPerChannel);
						SwapBytes(ColorValue,bytesPerPixelPerChannel);
						exB = (double)BitConverter.ToUInt32(ColorValue,0);

						L = (int)(exL/L_coef);
						a = (int)(exA/a_coef - 128.0);
						b = (int)(exB/b_coef - 128.0);

						crPixel = LabToRGB(L, a, b);

						nColor = ColorTranslator.ToWin32(crPixel);
						WinInvoke32.SetPixel(hdcMemory, nCol, nRow, nColor);
						nCol++;
						if(nWidth<=nCol)
						{
							nCol = 0;
							nRow++;
						}
					}

					WinInvoke32.SelectObject(hdcMemory, hbmpOld);
					WinInvoke32.DeleteDC(hdcMemory);
				}
					break;
			}
		}

		protected Color LabToRGB(int L, int a, int b)
		{
			// For the conversion we first convert values to XYZ and then to RGB
			// Standards used Observer = 2, Illuminant = D65
			
			const double ref_X = 95.047;
			const double ref_Y = 100.000;
			const double ref_Z = 108.883;

			double var_Y = ( (double)L + 16.0 ) / 116.0;
			double var_X = (double)a / 500.0 + var_Y;
			double var_Z = var_Y - (double)b / 200.0;

			if ( Math.Pow(var_Y, 3) > 0.008856 )
			var_Y = Math.Pow(var_Y, 3);
			else
			var_Y = ( var_Y - 16 / 116 ) / 7.787;

			if ( Math.Pow(var_X, 3) > 0.008856 )
			var_X = Math.Pow(var_X, 3);
			else
			var_X = ( var_X - 16 / 116 ) / 7.787;

			if ( Math.Pow(var_Z, 3) > 0.008856 )
			var_Z = Math.Pow(var_Z, 3);
			else
			var_Z = ( var_Z - 16 / 116 ) / 7.787;

			double X = ref_X * var_X;
			double Y = ref_Y * var_Y;
			double Z = ref_Z * var_Z;

			return XYZToRGB(X, Y, Z);
		}
		
		protected Color XYZToRGB(double X, double Y, double Z)
		{
			// Standards used Observer = 2, Illuminant = D65
			// ref_X = 95.047, ref_Y = 100.000, ref_Z = 108.883

			double var_X = X / 100.0;
			double var_Y = Y / 100.0;
			double var_Z = Z / 100.0;

			double var_R = var_X * 3.2406 + var_Y * (-1.5372) + var_Z * (-0.4986);
			double var_G = var_X * (-0.9689) + var_Y * 1.8758 + var_Z * 0.0415;
			double var_B = var_X * 0.0557 + var_Y * (-0.2040) + var_Z * 1.0570;

			if ( var_R > 0.0031308 )
			var_R = 1.055 * ( Math.Pow(var_R, 1/2.4) ) - 0.055;
			else
			var_R = 12.92 * var_R;

			if ( var_G > 0.0031308 )
			var_G = 1.055 * ( Math.Pow(var_G, 1/2.4) ) - 0.055;
			else
			var_G = 12.92 * var_G;

			if ( var_B > 0.0031308 )
			var_B = 1.055 * ( Math.Pow(var_B, 1/2.4) )- 0.055;
			else
			var_B = 12.92 * var_B;

			int nRed = (int)(var_R * 256.0);
			int nGreen = (int)(var_G * 256.0);
			int nBlue = (int)(var_B * 256.0);

			if(nRed<0) nRed = 0;
			else if(nRed>255) nRed = 255;
			if(nGreen<0) nGreen = 0;
			else if(nGreen>255) nGreen = 255;
			if(nBlue<0)	nBlue = 0;
			else if(nBlue>255) nBlue = 255;

			return Color.FromArgb(nRed,nGreen,nBlue);
		}

		protected Color CMYKToRGB(double C, double M, double Y, double K)
		{
			int nRed = (int)(( 1.0 - ( C *( 1 - K ) + K ) ) * 255);
			int nGreen = (int)(( 1.0 - ( M *( 1 - K ) + K ) ) * 255);
			int nBlue = (int)(( 1.0 - ( Y *( 1 - K ) + K ) ) * 255);

			if(nRed<0) nRed = 0;
			else if(nRed>255) nRed = 255;
			if(nGreen<0) nGreen = 0;
			else if(nGreen>255) nGreen = 255;
			if(nBlue<0)	nBlue = 0;
			else if(nBlue>255) nBlue = 255;

			return Color.FromArgb(nRed,nGreen,nBlue);
		}
		
		protected bool SwapBytes(byte [] array, int nLength)
		{
			bool bReturn = true;
			try
			{
				for(long i=0; i<nLength/2; ++i) 
				{
					byte t = array[i];
					array[i] = array[nLength - i - 1];
					array[nLength - i - 1] = t;
				}
			}
			catch(IndexOutOfRangeException e)
			{
				MessageBox.Show(e.ToString(),"Exception has been thrown:");
				bReturn = false;
			}

			return bReturn;
		}
	};

}
