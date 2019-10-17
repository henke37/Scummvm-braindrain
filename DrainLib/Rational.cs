﻿namespace DrainLib {
	public struct Rational {
		public int Numerator;
		public int Denominator;

		public Rational(int v) : this() {
			Numerator = v;
			Denominator = 1;
		}

		public float ToFloat() {
			return (float)Numerator / Denominator;
		}
		public double ToDouble() {
			return (double)Numerator / Denominator;
		}

		public static explicit operator float(Rational r) { return r.ToFloat(); }
		public static explicit operator double(Rational r) { return r.ToDouble(); }
	}
}
