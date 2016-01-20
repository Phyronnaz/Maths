using System;
using System.Collections;
using System.Collections.Generic;

namespace Maths
{
	public class Polynome
	{
		#region Variables
		private List<C> _coefficients = new List<C>();

		public int Degré { get { return GetDegré(); } }
		public Polynome Dérivée () {
			var poly = new Polynome ();
			for (int k = 1; k <= Degré; k++) {
				poly.AddCoefficient (k - 1, k * GetCoefficient (k));
			}
			return poly;
		}
		#endregion


		#region Constructors
		public Polynome (List<C> coefficients)
		{
			_coefficients = coefficients;
		}

		public Polynome (C[] coefficients) {
			_coefficients = new List<C> (coefficients);
		}

		public Polynome (C coefficient) {
			_coefficients = new List<C> ();
			_coefficients.Add (coefficient);
		}

		public Polynome (R coefficient) {
			_coefficients = new List<C> ();
			_coefficients.Add (new C(coefficient, 0));
		}

		public Polynome (string s) {
			s = s.Replace(" ", "");
			s = s.Replace ("-", "+-");
			s = s.Replace ("X^", "$");
			s = s.Replace ("X", "$1");
			var coeffs = s.Split ("+".ToCharArray());


			foreach (var c in coeffs) {
				R r;
				if (R.TryParse (c, out r)) {
					AddCoefficient (0, r);
				} else {
					var split = c.Split ("$".ToCharArray ());

					R a;
					if (!R.TryParse ( split[0], out a))
						a = 1;

					int d;
					if(int.TryParse (split[1], out d))
						AddCoefficient (d, a);
					else
						Console.WriteLine("Error in conversion to Polynome");

				}
			}
		}

		public Polynome ()
		{
			_coefficients.Add(0);
		}
		#endregion


		#region Public functions
		public void AddCoefficient(int rang, C coefficient) {
			while (rang >= _coefficients.Count) {
				_coefficients.Add (0);
			}
			_coefficients [rang] += coefficient;
		}

		public C GetCoefficient (int rang) {
			if (rang < _coefficients.Count)
				return _coefficients [rang];
			else
				return 0;
		}

		public List<C> GetCoefficients () {
			return _coefficients;
		}

		public C this[C c] {
			get {
				C s = 0;
				for (int k = 0; k <= Degré; k++) {
					C pow = 1;
					for (int l = 0; l < k; l++) {
						pow *= c;
					} 
					s += _coefficients [k] * pow;
				}
				return s;
			}
		}

		public static Polynome PGCD (Polynome A, Polynome B) {
			while (B != 0) {
				var X = B;
				B = A % B;
				A = X;
			}
			return A / A.GetCoefficient (A.Degré);
		}

		public static Polynome PGCD (Polynome[] polynomes) {
			var pgcd = polynomes [0];
			for (int k = 1; k < polynomes.Length; k++) {
				pgcd = PGCD (polynomes [k], pgcd);
			}
			return pgcd;
		}

		public static Polynome[] Bezout (Polynome[] polynomes) {
			if (polynomes.Length == 1)
				return polynomes;

			var bezouts = new Polynome[polynomes.Length];
			for (int i = 0; i < bezouts.Length; i++) {
				bezouts [i] = 1;
			}

			for (int k = 1; k <= polynomes.Length; k++) {
				var pgcd = polynomes [0];
				for (int i = 1; i < polynomes.Length - k; i++) {
					pgcd = PGCD(pgcd, polynomes[i]);
				}
				var b = Bezout (pgcd, polynomes [polynomes.Length - k]);
				bezouts [bezouts.Length - k] *= b [1];
				for (int l = 0; l < bezouts.Length-k; l++) {
					bezouts [l] *= b [0];
				}
			}  
			return bezouts;
		}

		public static Polynome[] Bezout(Polynome A, Polynome B) {
			Polynome Uk = 1;
			Polynome Vk = 0;
			Polynome UkPlus1 = 0;
			Polynome VkPlus1 = 1;

			while (B != 0) {
				var Uk_save = Uk;
				var Vk_save = Vk;
				Uk = UkPlus1;
				Vk = VkPlus1;

				UkPlus1 = Uk_save - UkPlus1 * (A / B);
				VkPlus1 = Vk_save - VkPlus1 * (A / B);

				var X = B;
				B = A % B;
				A = X;
			}
			return new Polynome[] { Uk / A.GetCoefficient (A.Degré), Vk / A.GetCoefficient (A.Degré) };
		}

		public override string ToString ()
		{
			var s = "";

			if(Degré == -1)
				s += "0";

			for (int k = 0; k < _coefficients.Count; k++) {
				if (_coefficients [k] != 0) {
					if(_coefficients[k] != 1 || k == 0)
						s += _coefficients [k].ToString ();

					if(k != 0 && _coefficients[k] != 1)
						s += "*";

					if (k != 0)
						s += "X";

					if (k > 1)
						s += "^" + k.ToString ();

					if (k != Degré)
						s += " + ";
				}
			}
			return s;
		}

		public override bool Equals(Object obj)
		{
			if (obj.GetType () == typeof(Polynome))
				return (Polynome)obj == this;
			else if (obj.GetType () == typeof(C))
				return (C)obj == this;
			else if (obj.GetType () == typeof(R))
				return (R)obj == this;
			else if (obj.GetType () == typeof(int))
				return (int)obj == this;
			else if (obj.GetType () == typeof(double))
				return (double)obj == this;
			else
				return false;
		}

		public override int GetHashCode ()
		{
			int s = 0;
			foreach (var k in _coefficients) {
				s += k.GetHashCode ();
			}
			return s;
		}
		#endregion

		#region Private functions
		private int GetDegré () {
			for(int k = _coefficients.Count-1; k >= 0; k--) {
				if (_coefficients [k] != 0)
					return k;
			}
			return -1;
		}
		#endregion


		#region Operators
		public static Polynome operator +(Polynome A, Polynome B) {
			var poly = new Polynome ();

			for (int k = 0; k <= Math.Max(A.Degré, B.Degré); k++) {
				poly.AddCoefficient (k, A.GetCoefficient(k)+B.GetCoefficient(k));
			}
			return poly;
		}

		public static Polynome operator -(Polynome A, Polynome B) {
			var poly = new Polynome ();

			for (int k = 0; k <= Math.Max(A.Degré, B.Degré); k++) {
				poly.AddCoefficient (k, A.GetCoefficient(k)-B.GetCoefficient(k));
			}
			return poly;
		}

		public static Polynome operator *(Polynome A, Polynome B) {
			var poly = new Polynome ();

			for (int k = 0; k <= A.Degré; k++) {
				for (int l = 0; l <= B.Degré; l++) {
					poly.AddCoefficient (k+l, A.GetCoefficient (k) * B.GetCoefficient (l));
				}
			}
			return poly;
		}

		public static Polynome operator %(Polynome A, Polynome B) {
			var Q = new Polynome ();

			while (A.Degré >= B.Degré) {
				var C = new Polynome ();
				C.AddCoefficient ((A.Degré - B.Degré) * Convert.ToInt32(A.Degré >= B.Degré), A.GetCoefficient (A.Degré) / B.GetCoefficient (B.Degré));
				A -= B * C;
				Q += C;
			}

			return A;
		}

		public static Polynome operator /(Polynome A, Polynome B) {
			var Q = new Polynome ();

			while (A.Degré >= B.Degré) {
				var C = new Polynome ();
				C.AddCoefficient ((A.Degré - B.Degré) * Convert.ToInt32(A.Degré >= B.Degré), A.GetCoefficient (A.Degré) / B.GetCoefficient (B.Degré));
				A -= B * C;
				Q += C;
			}

			return Q;
		}

		public static bool operator ==(Polynome A, Polynome B) {
			for (int k = 0; k <= Math.Max(A.Degré, B.Degré); k++) {
				if (A.GetCoefficient (k) != B.GetCoefficient (k))
					return false;
			}
			return true;
		} 

		public static bool operator !=(Polynome A, Polynome B) {
			for (int k = 0; k <= Math.Max(A.Degré, B.Degré); k++) {
				if (A.GetCoefficient (k) != B.GetCoefficient (k))
					return true;
			}
			return false;
		}

		public static implicit operator Polynome(C c) {
			return new Polynome (c);
		}

		public static implicit operator Polynome(R r) {
			return new Polynome (r);
		}

		public static implicit operator Polynome(double d) {
			return new Polynome ((R)d);
		}

		public static implicit operator Polynome(Int64 i) {
			return new Polynome ((R)i);
		}
		#endregion
	}
}