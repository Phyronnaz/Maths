using System;
using System.Collections.Generic;

namespace Maths
{
	public class Addition : Relation {
		public override int Priority { get { return 0; } }
		public override bool IsAssociative { get { return true; } }
		public override bool IsCommutative { get { return true; } }


		public Addition (Element gauche, Element droite) {
			Gauche = gauche;
			Droite = droite;
		}

		public override string ToString ()
		{
			return "(" + Gauche.ToString () + "+" + Droite.ToString () + ")";
		}
	}

	public class Multiplication : Relation {
		public override int Priority { get { return 1; } }
		public override bool IsAssociative { get { return true; } }
		public override bool IsCommutative { get { return true; } }
		protected override List<Operateurs> distributiveSur { get { return new List<Operateurs> { Operateurs.Addition }; } }

		public Multiplication (Element gauche, Element droite) {
			Gauche = gauche;
			Droite = droite;
		}

		public override string ToString ()
		{
			return "(" + Gauche.ToString () + "*" + Droite.ToString () + ")";
		}
	}
}

