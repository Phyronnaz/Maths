using System;

namespace Maths
{
	public class Inconnue : Element {
		public string name;

		public Inconnue (string s) {
			name = s;
		}

		public override string ToString ()
		{
			return name;
		}

		public override Element Neutre (Operateurs operateur)
		{
			return new Inconnue("0");
		}

		public override Element Operateur (Operateurs operateur, Element droite)
		{
			if (droite.GetType () != this.GetType ()) {
				return base.Operateur (operateur, droite);
			} else if (this.name != ((Inconnue)droite).name) {
				if (((Inconnue)droite).name == "0") {
					switch (operateur) {
					case Operateurs.Addition:
						return this;
					case Operateurs.Multiplication:
						return droite;
					default:
						return base.Operateur (operateur, droite);
					}
				} else {
					return base.Operateur (operateur, droite);
				}
			} else {
				switch (operateur) {
				case Operateurs.Addition:
					return new Multiplication ((R)2, this);
				default:
					return base.Operateur (operateur, droite);
				}
			}
		}

		public static implicit operator Inconnue (string s) {
			return new Inconnue (s);
		}
	}

	public class Fonction : Element {
		public Element Expression;
		public Inconnue x;

		public Fonction (Element expression, Inconnue x) {
			this.Expression = expression;
			this.x = x;
		}

		public Element GetValue (Element element) {
			if(Expression.GetType().IsSubclassOf(typeof(Relation))) {
				var ExpressionCopy = Expression.Copy ();
				var l = ((Relation)ExpressionCopy).GetChildsRelations ();
				foreach(var k in l) {
					if (k.GetType().IsSubclassOf(typeof(Relation))) {
						if (((Relation)k).Gauche.GetType () == typeof(Inconnue))
							((Relation)k).Gauche = element;
						
						if (((Relation)k).Droite.GetType () == typeof(Inconnue))
							((Relation)k).Droite = element;
					}
				}
				if (ExpressionCopy.GetType ().IsSubclassOf (typeof(Relation)))
					ExpressionCopy = ((Relation)ExpressionCopy).GetSimple ();
				
				return ExpressionCopy;
			} else {
				if (Expression.GetType () == typeof(Inconnue))
					return element;
				else
					return Expression;
			}
		}

		public override string ToString ()
		{
			return Expression.ToString ();
		}
	}
}

