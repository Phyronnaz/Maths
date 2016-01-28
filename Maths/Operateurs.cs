using System;
using System.Collections.Generic;

namespace Maths
{
	public enum Operateurs {Addition, Multiplication};

	public class Element {
		public Element() {}

		public virtual Element Operateur (Operateurs operateur, Element droite) {
			return (Element)System.Activator.CreateInstance (Type.GetType ("Maths." + operateur.ToString ()), this, droite);
		}

		public virtual Element Neutre (Operateurs operateur) {
			return new Element ();
		}

		public virtual Element Copy () {
			return this;
		}
	}

	public abstract class Operation : Element {
		
		#region Variables
		public abstract int Priority { get; }

		public virtual bool IsAssociative { get { return false; } }
		public virtual bool IsCommutative { get { return false; } }

		protected virtual List<Operateurs> distributiveSur { get { return new List<Operateurs> (); } }
		public virtual bool IsDistributiveOn(Operateurs operateur) {
			return distributiveSur.Contains(operateur);
		}

		public Operateurs Name {
			get { 
				Operateurs op;
				if (Enum.TryParse (this.GetType ().ToString ().Replace (this.GetType ().Namespace + ".", ""), out op)) {
					return op;
				} else {
					Console.WriteLine ("Unknown Operateur: " + this.GetType ().ToString ());
					return Operateurs.Addition;
				}
			}
		}

		private Element _droite;
		private Element _gauche;

		public Element Droite {
			get {
				return _droite;
			}
			set {
				_droite = value;
			}
		}
		public Element Gauche {
			get {
				return _gauche;
			}
			set {
				_gauche = value;
			}
		}
		#endregion

		#region Get and Copy
		public List<Element> GetGaucheAndDroite () {
			var l = new List<Element> ();

			if (Gauche.GetType ().IsSubclassOf (typeof(Operation)))
				l.AddRange (((Operation)Gauche).GetGaucheAndDroite ());
			else
				l.Add (Gauche);
		
			if (Droite.GetType ().IsSubclassOf (typeof(Operation)))
				l.AddRange (((Operation)Droite).GetGaucheAndDroite ());
			else
				l.Add (Droite);

			return l;
		}

		public List<Operation> GetChildsOperations () {
			var l = new List<Operation> ();

			l.Add (this);

			if (Gauche.GetType ().IsSubclassOf (typeof(Operation))) {
				l.AddRange (((Operation)Gauche).GetChildsOperations ());
				l.Add ((Operation)Gauche);
			}

			if (Droite.GetType ().IsSubclassOf (typeof(Operation))) {
				l.AddRange (((Operation)Droite).GetChildsOperations ());
				l.Add ((Operation)Droite);
			}

			return l;
		}

		public override Element Copy () {
			return (Operation)System.Activator.CreateInstance (this.GetType (), Gauche.Copy (), Droite.Copy ());
		}
		#endregion

		#region Simplification
		public Element GetSimple () {
			bool b;
			return GetSimple (out b);
		}
		public virtual Element GetSimple (out bool hasChanged) {
			hasChanged = false;

			//Si un neutre
			if (Gauche == Gauche.Neutre (Name)) {
				if (Droite.GetType ().IsSubclassOf (typeof(Operation)))
					return ((Operation)Droite).GetSimple ();
				else
					return Droite;
			} else if (Droite == Droite.Neutre (Name)) {
				if (Gauche.GetType ().IsSubclassOf (typeof(Operation)))
					return ((Operation)Gauche).GetSimple ();
				else
					return Gauche;
			}

			//Développement: on développe d'abord à gauche puis à droite si on ne peut pas
			if (Droite.GetType ().IsSubclassOf (typeof(Operation))) {
				if (this.IsDistributiveOn (((Operation)Droite).Name)) {
					return ((Operation)System.Activator.CreateInstance (Droite.GetType (),
						(Operation)System.Activator.CreateInstance (this.GetType (), Gauche, ((Operation)Droite).Gauche),
						(Operation)System.Activator.CreateInstance (this.GetType (), Gauche, ((Operation)Droite).Droite))).GetSimple ();
				}
			}
			if (Droite != Droite.Neutre (this.Name)) {
				if (Gauche.GetType ().IsSubclassOf (typeof(Operation))) {
					if (this.IsDistributiveOn (((Operation)Gauche).Name)) {
						return ((Operation)System.Activator.CreateInstance (Gauche.GetType (),
							(Operation)System.Activator.CreateInstance (this.GetType (), ((Operation)Gauche).Gauche, Droite),
							(Operation)System.Activator.CreateInstance (this.GetType (), ((Operation)Gauche).Droite, Droite))).GetSimple ();
					}
				}
			}

			//Première simplification
			var continuer = true;
			while (continuer) {
				continuer = false;
				if (Gauche.GetType ().IsSubclassOf (typeof(Operation))) {
					bool b;
					Gauche = ((Operation)Gauche).GetSimple (out b);
					continuer = continuer || b;
					hasChanged = hasChanged || b;
				}
				if (Droite.GetType ().IsSubclassOf (typeof(Operation))) {
					bool b;
					Droite = ((Operation)Droite).GetSimple (out b);
					continuer = continuer || b;
					hasChanged = hasChanged || b;
				}
			}

			//Associativité && Commutativité
			if (IsAssociative) {
				List<Element> liste = new List<Element> ();
				if (Gauche.GetType () == this.GetType ()) {
					liste.Add (((Operation)Gauche).Gauche);
					liste.Add (((Operation)Gauche).Droite);
				} else {
					liste.Add (Gauche);
				}
				if (Droite.GetType () == this.GetType ()) {
					liste.Add (((Operation)Droite).Gauche);
					liste.Add (((Operation)Droite).Droite);
				} else {
					liste.Add (Droite);
				}


				if (IsCommutative) {
					for (var i = 0; i < liste.Count; i++) {
						for (var j = 0; j < liste.Count; j++) {
							if (i != j && liste [i].GetType () == liste [j].GetType ()) {
								liste [i] = liste [i].Operateur (this.Name, liste [j]);
								liste.RemoveAt (j);
							}
						}
					}
				} else {
					for (var k = 0; k < liste.Count - 1; k++) {
						if (liste [k].GetType () == liste [k + 1].GetType ()) {
							liste [k] = liste [k].Operateur (Name, liste [k + 1]);
							liste.RemoveAt (k + 1);
						}
					}
				}
				switch (liste.Count) {
				case 1:
					return liste [0];
				case 2:
					Gauche = liste [0];
					Droite = liste [1];
					break;
				case 3:
					Gauche = (Operation)System.Activator.CreateInstance (this.GetType (), liste [0], liste [1]);
					Gauche = (Operation)System.Activator.CreateInstance (this.GetType (), liste [2], liste [2].Neutre (Name));
					break;
				case 4:
					Gauche = (Operation)System.Activator.CreateInstance (this.GetType (), liste [0], liste [1]);
					Gauche = (Operation)System.Activator.CreateInstance (this.GetType (), liste [2], liste [3]);
					break;
				}
			}

			if (Gauche.GetType () == Droite.GetType ()) {
				Element tmpValue = this;
				Element newValue = Gauche.Operateur (Name, Droite);
				if (newValue != tmpValue) {
					hasChanged = true;
					return newValue;
				}
			}
			return this;
		}
		#endregion

		#region Operators && generic functions
		public static bool operator ==(Operation g, Operation d) {
			return (g.Gauche == d.Gauche) && (g.Droite == d.Droite) && (g.GetType() == d.GetType());
		}

		public static bool operator !=(Operation g, Operation d) {
			return (g.Gauche != d.Gauche) || (g.Droite != d.Droite) || (g.GetType() != d.GetType());
		}


		public override bool Equals (object obj)
		{
			if (obj.GetType () == this.GetType ())
				return (Operation)obj == this;
			else
				return false;
		}

		public override int GetHashCode ()
		{
			return Gauche.GetHashCode () + Droite.GetHashCode ();
		}
		#endregion
	}
}
