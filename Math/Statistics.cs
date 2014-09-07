// AForge Math Library
//
// Copyright © Andrew Kirillov, 2005
// andrew.kirillov@gmail.com
//

namespace AForge.Math
{
	using System;

	/// <summary>
	/// Set of statistics functions
	/// </summary>
	public class Statistics
	{
		/// <summary>
		/// Calculate mean value
		/// 
		/// Input: histogram array
		/// </summary>
		public static double Mean( int[] values )
		{
			int		v;
			int		mean = 0;
			int		total = 0;

			// for all values
			for ( int i = 0, n = values.Length; i < n; i++ )
			{
				v = values[i];

				// accumulate mean
				mean += i * v;
				// accumalate total
				total += v;
			}

			return (double) mean / total;
		}


		/// <summary>
		/// Calculate standard deviation
		/// 
		/// Input: histogram array
		/// </summary>
		public static double StdDev( int[] values )
		{
			double	mean = Mean( values );
			double	stddev = 0;
			double	t;
			int		v;
			int		total = 0;

			// for all values
			for ( int i = 0, n = values.Length; i < n; i++ )
			{
				v = values[i];
				t = (double) i - mean;

				// accumulate mean
				stddev += t * t * v;
				// accumalate total
				total += v;
			}

			return Math.Sqrt( stddev / total );
		}


		/// <summary>
		/// Calculate median value
		/// 
		/// Input: histogram array
		/// </summary>
		public static int Median( int[] values )
		{
			int total = 0, n = values.Length;

			// for all values
			for ( int i = 0; i < n; i++ )
			{
				// accumalate total
				total += values[i];
			}

			int halfTotal = total / 2;
			int median, v;

			// find median value
			for ( median = 0, v = 0; median < n; median++ )
			{
				v += values[median];
				if ( v >= halfTotal )
					break;
			}

			return median;
		}


		/// <summary>
		/// Get range around median containing specified percentile of values
		/// 
		/// Input: histogram array
		/// </summary>
		public static Range GetRange( int[] values, double percent )
		{
			int total = 0, n = values.Length;

			// for all values
			for ( int i = 0; i < n; i++ )
			{
				// accumalate total
				total += values[i];
			}

			int min, max, v;
			int h = (int)( total * ( percent + ( 1 - percent ) / 2 ) );

			// get range min value
			for ( min = 0, v = total; min < n; min++ )
			{
				v -= values[min];
				if ( v < h )
					break;
			}
			// get range max value
			for ( max = n - 1, v = total;  max >= 0; max-- )
			{
				v -= values[max];
				if ( v < h )
					break;
			}
			return new Range( min, max );
		}


		/// <summary>
		/// Calculate an entropy
		/// 
		/// Input: histogram array
		/// </summary>
		public static double Entropy( int[] values )
		{
			int total = 0;

			for ( int i = 0, n = values.Length; i < n; i++ )
			{
				total += values[i];
			}

			return Entropy( values, total );
		}
		public static double Entropy( int[] values, int total )
		{
			int		n = values.Length;
			double	e = 0;
			double	p;

			// for all values
			for ( int i = 0; i < n; i++ )
			{
				// get item probability
				p = (double) values[i] / total;
				// calculate entropy
				if (p != 0)
					e += ( -p * Math.Log( p, 2 ) );
			}
			return e;
		}
	}
}
