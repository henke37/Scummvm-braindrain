using System;

namespace DrainLib {
	public struct Rational {
		public int Numerator;
		public int Denominator;

		public Rational(int v) : this() {
			Numerator = v;
			Denominator = 1;
		}

		public Rational(int n, int d) {
			Numerator = n;
			Denominator = d;
		}

		public float ToFloat() {
			return (float)Numerator / Denominator;
		}
		public double ToDouble() {
			return (double)Numerator / Denominator;
		}

		public static explicit operator float(Rational r) { return r.ToFloat(); }
		public static explicit operator double(Rational r) { return r.ToDouble(); }


		public static Rational operator +(Rational a, Rational b)
			=> new Rational(a.Numerator * b.Denominator + b.Numerator * a.Denominator, a.Denominator * b.Denominator);

		public static Rational operator -(Rational a, Rational b)
			=> new Rational(a.Numerator* (-b.Denominator) - b.Numerator* a.Denominator, a.Denominator* (-b.Denominator));

		public static Rational operator *(Rational a, Rational b)
			=> new Rational(a.Numerator * b.Numerator, a.Denominator * b.Denominator);

		public static Rational operator /(Rational a, Rational b) {
			if(b.Numerator == 0) {
				throw new DivideByZeroException();
			}
			return new Rational(a.Numerator * b.Denominator, a.Denominator * b.Numerator);
		}

		public static Rational operator *(Rational a, int b)
			=> new Rational(a.Numerator * b, a.Denominator);

		public static Rational operator /(Rational a, int b) {
			if(b == 0) {
				throw new DivideByZeroException();
			}
			return new Rational(a.Numerator, a.Denominator * b);
		}
	}
}
