using System;

namespace Maths
{
//	public class Inconnue : Element {
//		public string name;
//
//		public Inconnue (string s) {
//			name = s;
//		}
//
//		public override string ToString ()
//		{
//			return name;
//		}
//
//		public override Element Neutre (Operateurs operateur)
//		{
//			switch (operateur) {
//			case Operateurs.Addition:
//				return new Inconnue("0");
//			case Operateurs.Multiplication:
//				return new Inconnue ("1");
//			}
//		}
//
//		public override Element Operateur (Operateurs operateur, Element droite)
//		{
//			if (droite.GetType () != this.GetType ()) {
//				
//				return base.Operateur (operateur, droite);
//			} else if (this.name != ((Inconnue)droite).name) {
//				if (((Inconnue)droite).name == "Neutre") {
//					return this;
//				} else {
//					return base.Operateur (operateur, droite);
//				}
//			} else {
//				switch (operateur) {
//				case Operateurs.Addition:
//					return new Multiplication ((R)2, this);
//				default:
//					return base.Operateur (operateur, droite);
//				}
//			}
//		}
//
//		public static implicit operator Inconnue (string s) {
//			return new Inconnue (s);
//		}
//	}
//
//	public class Fonction : Element {
//		public Element Expression;
//		public Inconnue x;
//
//		public Fonction (Element expression, Inconnue x) {
//			this.Expression = expression;
//			this.x = x;
//		}
//
//		public Element GetValue (Element element) {
//			if(Expression.GetType().IsSubclassOf(typeof(Operation))) {
//				var ExpressionCopy = Expression.Copy ();
//				var l = ((Operation)ExpressionCopy).GetChildsOperations ();
//				foreach(var k in l) {
//					if (k.GetType().IsSubclassOf(typeof(Operation))) {
//						if (((Operation)k).Gauche.GetType () == typeof(Inconnue))
//							((Operation)k).Gauche = element;
//						
//						if (((Operation)k).Droite.GetType () == typeof(Inconnue))
//							((Operation)k).Droite = element;
//					}
//				}
//				if (ExpressionCopy.GetType ().IsSubclassOf (typeof(Operation)))
//					ExpressionCopy = ((Operation)ExpressionCopy).GetSimple ();
//				
//				return ExpressionCopy;
//			} else {
//				if (Expression.GetType () == typeof(Inconnue))
//					return element;
//				else
//					return Expression;
//			}
//		}
//
//		public override string ToString ()
//		{
//			return Expression.ToString ();
//		}
//	}
}

