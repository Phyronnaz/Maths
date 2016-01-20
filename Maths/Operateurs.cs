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


	public abstract class Relation : Element {
		
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

		public List<Element> GetGaucheAndDroite () {
			var l = new List<Element> ();

			if (Gauche.GetType ().IsSubclassOf (typeof(Relation)))
				l.AddRange (((Relation)Gauche).GetGaucheAndDroite ());
			else
				l.Add (Gauche);
		
			if (Droite.GetType ().IsSubclassOf (typeof(Relation)))
				l.AddRange (((Relation)Droite).GetGaucheAndDroite ());
			else
				l.Add (Droite);

			return l;
		}

		public List<Relation> GetChildsRelations () {
			var l = new List<Relation> ();

			l.Add (this);

			if (Gauche.GetType ().IsSubclassOf (typeof(Relation))) {
				l.AddRange (((Relation)Gauche).GetChildsRelations ());
				l.Add ((Relation)Gauche);
			}

			if (Droite.GetType ().IsSubclassOf (typeof(Relation))) {
				l.AddRange (((Relation)Droite).GetChildsRelations ());
				l.Add ((Relation)Droite);
			}

			return l;
		}

		public override Element Copy () {
			return (Relation)System.Activator.CreateInstance (this.GetType (), Gauche.Copy (), Droite.Copy ());
		}

		#region Simplification
		public Element GetSimple () {
			bool b;
			return GetSimple (out b);
		}
		public virtual Element GetSimple (out bool hasChanged) {
			hasChanged = false;

			//Si un neutre
			if (Gauche == Gauche.Neutre (Name)) {
				if (Droite.GetType ().IsSubclassOf (typeof(Relation)))
					return ((Relation)Droite).GetSimple ();
				else
					return Droite;
			} else if (Droite == Droite.Neutre (Name)) {
				if (Gauche.GetType ().IsSubclassOf (typeof(Relation)))
					return ((Relation)Gauche).GetSimple ();
				else
					return Gauche;
			}

			//Développement: on développe d'abord à gauche puis à droite si on ne peut pas
			if (Droite.GetType ().IsSubclassOf (typeof(Relation))) {
				if (this.IsDistributiveOn (((Relation)Droite).Name)) {
					return ((Relation)System.Activator.CreateInstance (Droite.GetType (),
						(Relation)System.Activator.CreateInstance (this.GetType (), Gauche, ((Relation)Droite).Gauche),
						(Relation)System.Activator.CreateInstance (this.GetType (), Gauche, ((Relation)Droite).Droite))).GetSimple ();
				}
			}
			if (Droite != Droite.Neutre (this.Name)) {
				if (Gauche.GetType ().IsSubclassOf (typeof(Relation))) {
					if (this.IsDistributiveOn (((Relation)Gauche).Name)) {
						return ((Relation)System.Activator.CreateInstance (Gauche.GetType (),
							(Relation)System.Activator.CreateInstance (this.GetType (), ((Relation)Gauche).Gauche, Droite),
							(Relation)System.Activator.CreateInstance (this.GetType (), ((Relation)Gauche).Droite, Droite))).GetSimple ();
					}
				}
			}

			//Première simplification
			var continuer = true;
			while (continuer) {
				continuer = false;
				if (Gauche.GetType ().IsSubclassOf (typeof(Relation))) {
					bool b;
					Gauche = ((Relation)Gauche).GetSimple (out b);
					continuer = continuer || b;
					hasChanged = hasChanged || b;
				}
				if (Droite.GetType ().IsSubclassOf (typeof(Relation))) {
					bool b;
					Droite = ((Relation)Droite).GetSimple (out b);
					continuer = continuer || b;
					hasChanged = hasChanged || b;
				}
			}

			//Associativité && Commutativité
			if (IsAssociative) {
				List<Element> liste = new List<Element> ();
				if (Gauche.GetType () == this.GetType ()) {
					liste.Add (((Relation)Gauche).Gauche);
					liste.Add (((Relation)Gauche).Droite);
				} else {
					liste.Add (Gauche);
				}
				if (Droite.GetType () == this.GetType ()) {
					liste.Add (((Relation)Droite).Gauche);
					liste.Add (((Relation)Droite).Droite);
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
					Gauche = (Relation)System.Activator.CreateInstance (this.GetType (), liste [0], liste [1]);
					Gauche = (Relation)System.Activator.CreateInstance (this.GetType (), liste [2], liste [2].Neutre (Name));
					break;
				case 4:
					Gauche = (Relation)System.Activator.CreateInstance (this.GetType (), liste [0], liste [1]);
					Gauche = (Relation)System.Activator.CreateInstance (this.GetType (), liste [2], liste [3]);
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


		#region Operator && generic functions
		public static bool operator ==(Relation g, Relation d) {
			return (g.Gauche == d.Gauche) && (g.Droite == d.Droite) && (g.GetType() == d.GetType());
		}

		public static bool operator !=(Relation g, Relation d) {
			return (g.Gauche != d.Gauche) || (g.Droite != d.Droite) || (g.GetType() != d.GetType());
		}


		public override bool Equals (object obj)
		{
			if (obj.GetType () == this.GetType ())
				return (Relation)obj == this;
			else
				return false;
		}

		public override int GetHashCode ()
		{
			return Gauche.GetHashCode () + Droite.GetHashCode ();
		}
		#endregion
	}
		

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
