// AForge Math Library
//
// Copyright © Andrew Kirillov, 2005
// andrew.kirillov@gmail.com
//

namespace AForge.Math
{
	using System;
	
	/// <summary>
	/// Complex number
	/// </summary>
	public struct Complex
	{
		public float	Re;		// real component
		public float	Im;		// imaginary component
		
		// Get zero value
		public static Complex Zero
		{
			get { return new Complex( 0, 0 ); }
		}
		
		// Get magnitude (absolute) value
		public float Magnitude
		{
			get { return (float) System.Math.Sqrt( Re * Re + Im * Im ); }
		}

		// Get phase value
		public float Phase
		{
			get { return (float) System.Math.Atan( Im / Re ); }
		}
		
		// Get squred magnitude value
		public float SquaredMagnitude
		{
			get { return ( Re * Re + Im * Im ); }
		}
	
	
		// Constructors
		public Complex( float re, float im )
		{
			this.Re = re;
			this.Im = im;
		}
		public Complex( Complex c )
		{
			this.Re = c.Re;
			this.Im = c.Im;
		}
		
		// Get complex number as string
		public override string ToString( )
		{
			return String.Format( "{0}{1}{2}i", Re, ( ( Im < 0 ) ? '-' : '+' ), Math.Abs( Im ) );
		}
		
		// Add two complex numbers
		public static Complex operator+( Complex a, Complex b )
		{
			return new Complex( a.Re + b.Re, a.Im + b.Im );
		}

		// Subtract two complex numbers
		public static Complex operator-( Complex a, Complex b )
		{
			return new Complex( a.Re - b.Re, a.Im - b.Im );
		}
		
		// Multiply two complex numbers
		public static Complex operator*( Complex a, Complex b )
		{
			return new Complex(
				a.Re * b.Re - a.Im * b.Im,
				a.Re * b.Im + a.Im * b.Re );
		}

		// Divide one complex number by another
		public static Complex operator/( Complex a, Complex b )
		{
			float divider = b.Re * b.Re + b.Im * b.Im;
			
			if ( divider == 0 )
				throw new DivideByZeroException( );
		
			return new Complex(
				(a.Re * b.Re + a.Im * b.Im) / divider,
				(a.Im * b.Re - a.Re * b.Im) / divider );
		}
	}
}