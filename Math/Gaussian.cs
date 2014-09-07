// AForge Math Library
//
// Copyright © Andrew Kirillov, 2005
// andrew.kirillov@gmail.com
//

namespace AForge.Math
{
	using System;

	/// <summary>
	/// Gaussian function
	/// </summary>
	public class Gaussian
	{
		private double sigma = 1.0;
		private double sqrSigma = 1.0;	// squared sigma

		// Sigma property
		public double Sigma
		{
			get { return sigma; }
			set
			{
				sigma = Math.Max( 0.00000001, value );
				sqrSigma = sigma * sigma;
			}
		}

		// Constructors
		public Gaussian( )
		{
		}
		public Gaussian( double sigma )
		{
			Sigma = sigma;
		}

		/// <summary>
		/// 1-D Gaussian function
		/// </summary>
		public double Function( double x )
		{
			return Math.Exp( x * x / ( -2 * sqrSigma ) ) / ( Math.Sqrt(2 * Math.PI ) * sigma );
		}

		/// <summary>
		/// 2-D Gaussian function
		/// </summary>
		public double Function2D( double x, double y )
		{
			return Math.Exp( ( x * x + y * y ) / ( -2 * sqrSigma ) ) / ( 2 * Math.PI * sqrSigma );
		}

		/// <summary>
		/// 1-D Gaussian kernel
		/// </summary>
		public double[] Kernel( int size )
		{
			// check for evem size and for out of range
			if ( ( (size % 2 ) == 0 ) || ( size < 3 ) || ( size > 101 ) )
			{
				throw new ArgumentException( );
			}

			// raduis
			int r = size / 2;
			// kernel
			double[] kernel = new double[size];

			// compute kernel
			for ( int x = -r, i = 0; i < size; x++, i++ )
			{
				kernel[i] = Function( x );
			}

			return kernel;
		}

		/// <summary>
		/// 2-D Gaussian kernel
		/// </summary>
		public double[,] Kernel2D( int size )
		{
			// check for evem size and for out of range
			if ( ( ( size % 2 ) == 0 ) || ( size < 3 ) || ( size > 101 ) )
			{
				throw new ArgumentException();
			}

			// raduis
			int r = size / 2;
			// kernel
			double[,] kernel = new double[size, size];

			// compute kernel
			for ( int y = -r, i = 0; i < size; y++, i++ )
			{
				for ( int x = -r, j = 0; j < size; x++, j++ )
				{
					kernel[i, j] = Function2D( x, y );
				}
			}

			return kernel;
		}

		/// <summary>
		/// 1-D Gaussian kernel (discret)
		/// </summary>
		public int[] KernelDiscret( int size )
		{
			double[]	kernel = Kernel( size );
			double		min = kernel[0], factor = min;
			double		minError = double.MaxValue;

			// try some factors for more accurate discretization
			for ( int k = 1; k <= 5; k++ )
			{
				double error = 0.0;
				double f = (double) k / min;

				// for all values
				for ( int i = 0; i < size; i++ )
				{
					double v = kernel[i] * f;
					double e = v - (int) v;

					error += e * e;
				}

				// check error
				if ( error < minError )
				{
					minError = error;
					factor = f;
				}
			}

			int[] intKernel = new int[size];

			// discretization
			for ( int i = 0; i < size; i++ )
			{
				intKernel[i] = (int)( kernel[i] * factor );
			}

			return intKernel;
		}


		/// <summary>
		/// 2-D Gaussian kernel (discret)
		/// </summary>
		public int[,] KernelDiscret2D( int size )
		{
			double[,]	kernel = Kernel2D( size );
			double		min = kernel[0, 0], max = kernel[size >> 1, size >> 1];
			double		factor = min;
			double		minError = double.MaxValue;

			// try some factors for more accurate discretization
			for ( int k = 1; k <= 5; k++ )
			{
				double error = 0.0;
				double f = (double) k / min;

				// avoid too large values
				if ( max * f > ushort.MaxValue )
				{
					f = (double) ushort.MaxValue / max;
				}

				// for each row
				for ( int i = 0; i < size; i++ )
				{
					// for each column
					for ( int j = 0; j < size; j++ )
					{
						double v = kernel[i, j] * f;
						double e = v - (int) v;

						error += e * e;
					}
				}

				// check error
				if ( error < minError )
				{
					minError = error;
					factor = f;
				}
			}

			int[,] intKernel = new int[size, size];

			// discretization
			for ( int i = 0; i < size; i++ )
			{
				for ( int j = 0; j < size; j++ )
				{
					intKernel[i, j] = (int)( kernel[i, j] * factor );
				}
			}

			return intKernel;
		}
	}
}
