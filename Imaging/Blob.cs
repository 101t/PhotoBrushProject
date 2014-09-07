// AForge Image Processing Library
//
// Copyright © Andrew Kirillov, 2005
// andrew.kirillov@gmail.com
//

namespace AForge.Imaging
{
	using System;
	using System.Drawing;
	using System.Drawing.Imaging;


	/// <summary>
	/// Represents a blob
	/// </summary>
	public class Blob : IDisposable
	{
		private Bitmap	image;		// blobs image
		private Point	location;	// blobs location
		private Bitmap	owner;		// image containing the blob

		private bool	disposed = false;


		// Image property
		public Bitmap Image
		{
			get { return image; }
		}
		// Location property
		public Point Location
		{
			get { return location; }
		}
		// Owner property
		public Bitmap Owner
		{
			get { return owner; }
		}

		// Constructor
		public Blob(Bitmap image, Point location)
		{
			this.image		= image;
			this.location	= location;
		}
		public Blob(Bitmap image, Point location, Bitmap owner)
		{
			this.image		= image;
			this.location	= location;
			this.owner		= owner;
		}

		// Destructor
		~Blob()
		{
			Dispose(false);
		}

		// Dispose the object
		public void Dispose()
		{
			Dispose(true);
			// Take yourself off of the Finalization queue 
			// to prevent finalization code for this object
			// from executing a second time.
			GC.SuppressFinalize(this);
		}

		// Actual object disposing
		protected virtual void Dispose(bool disposing)
		{
			if (!disposed)
			{
				if (disposing)
				{
					image.Dispose();
				}
			}
		}
	}
}
