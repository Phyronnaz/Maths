using System;
using System.Collections;
using System.Collections.Generic;

namespace Maths
{
	public class R : Element
	{
		#region Variables
		private Int64 a_value;
		private Int64 b_value;

		public Int64 a {
			get { return a_value; }
			set {
				a_value = value;
				Reduct ();
			}
		}
		public Int64 b {
			get { return b_value; }
			set {
				b_value = value;
				Reduct ();
			}
		}

		public double r;

		public bool isFraction;


		public double Value {
			get {
				if (isFraction)
					return (double)a / (double)b;
				else
					return r;
			}
		}
		#endregion


		#region Construtors
		public R(double r) {
			this.a_value = 0;
			this.b_value = 0;
			this.r = r;
			this.a = 0;
			this.b = 0;
			isFraction = false;
		}

		public R(Int64 a, Int64 b) {
			this.a_value = 0;
			this.b_value = 0;
			this.a = a * (Math.Abs (b) / b);
			this.b = Math.Abs (b);
			this.r = 0;
			isFraction = true;
		}

		public R(Int64 a) {
			this.a_value = 0;
			this.b_value = 0;
			this.a = a;
			this.b = 1;
			this.r = 0;
			isFraction = true;
		}

		#endregion


		#region Functions
		private void Reduct () {
			if (a == 0 || b == 0)
				return;

			var a1 = a;
			var b1 = b;

			if (b1 > a1) {
				var x = b1;
				b1 = a1;
				a1 = x;
			}

			while (b1 != 0) {
				var x = a1 % b1;
				a1 = b1;
				b1 = x;
			}

			a_value /= a1;
			b_value /= a1;
		}

		public override Element Operateur (Operateurs operateur, Element droite) {
			if (droite.GetType () != this.GetType ()) {
				return null;
			} else {
				switch (operateur) {
				case Operateurs.Addition:
					return this + (R) droite;

				case Operateurs.Multiplication:
					return this * (R) droite;

				default:
					Console.WriteLine ("Operator not handled");
					return null;
				}
			}
		}

		public override Element Neutre (Operateurs operateur)
		{
			switch (operateur) {
			case Operateurs.Addition:
				return (R)0;
			case Operateurs.Multiplication:
				return (R)1;
			default:
				return (R)0;
			}
		}

		public override Element Copy ()
		{
			if (this.isFraction)
				return new R (a, b);
			else
				return new R (r);
		}

		public override string ToString ()
		{
			if (isFraction) {
				if (a != 0 && b != 1)
					return a.ToString () + "/" + b.ToString ();
				else if (a == 0)
					return "0";
				else
					return a.ToString ();
			} else {
				return r.ToString ();
			}
		}

		public override bool Equals(Object obj)
		{
			if (obj.GetType () == typeof(C))
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
			return this.Value.GetHashCode();
		}

		public static bool TryParse (string s, out R r) {
			s = s.Replace (" ", "");
			if(s.Contains("/")) {
				var split = s.Split ("/".ToCharArray ());
				Int64 a, b;
				double c, d;
				if(Int64.TryParse(split[0], out a) && Int64.TryParse(split[1], out b)) {
					r = new R (a, b);
					return true;
				} else if(double.TryParse(split[0], out c) && double.TryParse(split[1], out d)) {
					r = new R (c / d);
					return true;
				} else {
					r = 0;
					return false;
				}
			} else {
				Int64 i;
				double d;
				if(Int64.TryParse(s, out i)) {
					r = new R (i);
					return true;
				} else if(double.TryParse(s, out d)) {
					r = new R (d);
					return true;
				} else {
					r = 0;
					return false;
				}
			}
		}
		#endregion


		#region Operators
		public static R operator +(R g, R d) {
			if (g.isFraction && d.isFraction) {
				return new R (g.a * d.b + d.a * g.b, g.b * d.b);
			} else {
				return g.Value + d.Value;
			}
		}

		public static R operator -(R g, R d) {
			if (g.isFraction && d.isFraction) {
				return g + new R (-d.a, d.b);
			} else {
				return g.Value - d.Value;
			}
		}

		public static R operator *(R g, R d) {
			if (g.isFraction && d.isFraction) {
				return new R (g.a * d.a, g.b * d.b);
			} else {
				return g.Value * d.Value;
			}
		}

		public static R operator /(R g, R d) {
			if (g.isFraction && d.isFraction)
				return new R (g.a * d.b, g.b * d.a);
			else
				return g.Value / d.Value;
		} 

		public static bool operator ==(R g, R d) {
			if (g.isFraction && d.isFraction) {
				if (g.a * d.b == g.b * d.a)
					return true;
				else
					return false;
			} else {
				return g.Value == d.Value;
			}
		}

		public static bool operator !=(R g, R d) {
			if (g.isFraction && d.isFraction) {
				if (g.a * d.b == g.b * d.a)
					return false;
				else
					return true;
			} else {
				return g.Value != d.Value;
			}
		}

		public static implicit operator R(Int64 i) {
			return new R (i);
		}

		public static implicit operator R(double d) {
			return new R (d);
		}
		#endregion
	}

	public class C : Element
	{
		#region Variables
		private R a;
		private R b;

		public R[] Value { get { return new R[] { a, b }; } }
		public R Re { get { return a; } }
		public R Im { get { return b; } }
		#endregion


		#region Construtors
		public C(R a, R b) {
			this.a = a;
			this.b = b;
		}

		public C (R a) {
			this.a = a;
			this.b = 0;
		}
		#endregion


		#region Public functions
		public override string ToString ()
		{
			if (b == 0)
				return a.ToString ();
			else if (a == 0)
				return b.ToString()+"i";
			else
				return a.ToString ()+"+"+b.ToString()+"i";
		}

		public override bool Equals(Object obj)
		{
			if (obj.GetType () == typeof(C))
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
			return a.GetHashCode () * b.GetHashCode ();
		}

		public static bool TryParse(string s, out C c) {
			s = s.Replace (" ", "");
			if(s.Contains("+")) {
				var split = s.Split ("+".ToCharArray ());
				R a, b;
				if(R.TryParse(split[0], out a) && R.TryParse(split[1].Replace("i", ""), out b)) {
					c = new C (a, b);
					return true;
				} else {
					c = 0;
					return false;
				}
			} else {
				if(s.Contains("i")) {
					R r;
					if (R.TryParse (s.Replace ("i", ""), out r)) {
						c = new C (0, r);
						return true;
					} else {
						c = 0;
						return false;
					}
				} else {
					R r;
					if(R.TryParse(s, out r)) {
						c = new C (r);
						return true;
					} else {
						c = 0;
						return false;
					}
				}
			}
		}

		public override Element Operateur (Operateurs operateur, Element droite) {
			if (droite.GetType () != this.GetType ()) {
				return null;
			} else {
				switch (operateur) {
				case Operateurs.Addition:
					return this + (C) droite;

				case Operateurs.Multiplication:
					return this * (C) droite;

				default:
					Console.WriteLine ("Operator not handled");
					return null;
				}
			}
		}

		public override Element Copy ()
		{
			return new C ((R)a.Copy (), (R)b.Copy ());
		}
		#endregion


		#region Operators
		public static C operator +(C g, C d) {
			return new C (g.Re + d.Re, g.Im + d.Im);
		}

		public static C operator -(C g, C d) {
			return new C(g.Re - d.Re, g.Im - d.Im);
		}

		public static C operator *(C g, C d) {
			return new C (g.Re * d.Re - g.Im * d.Im, g.Re * d.Im + d.Re * g.Im);
		}

		public static C operator /(C g, C d) {
			return new C ((g.Re * d.Re + g.Im * d.Im) / (d.Re * d.Re + d.Im * d.Im), (g.Im * d.Re - g.Re * d.Im) / (d.Re * d.Re + d.Im * d.Im));
		}

		public static bool operator ==(C g, C d) {
			if (d.Re == g.Re && d.Im == g.Im)
				return true;
			else
				return false;
		}

		public static bool operator !=(C g, C d) {
			if (d.Re == g.Re && d.Im == g.Im)
				return false;
			else
				return true;
		}

		public static implicit operator C(R r) {
			return new C (r);
		}

		public static implicit operator C(double d) {
			return new C ((R)d, 0);
		}

		public static implicit operator C(int i) {
			return new C ((R)i, 0);
		}

		public static implicit operator C(string s) {
			C c;
			if (C.TryParse (s, out c)) {
				return c;
			} else {
				Console.WriteLine("Cast error to C from "+s);
				return new C (0);
			}
		}
		#endregion
	}
}
