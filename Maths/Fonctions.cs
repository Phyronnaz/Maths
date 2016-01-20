using System;

namespace Maths
{
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

