using System;

namespace Maths
{
	public class Relation
	{
		
	}

	public class RelationBinaire : Relation {
		
	}

	public abstract class Application : RelationBinaire {
		public abstract Ensemble[] départ { get;}
		public abstract Ensemble[] arrivée { get;}
	}

	public abstract class LoiDeComposition : Application {
		public bool estLoiDeCompositionInterne { get { return (départ == arrivée); } }
	}

	public class Addition : LoiDeComposition {
		public override Ensemble[] départ { get { return new Ensemble[]{Ensemble.C}; } }
		public override Ensemble[] arrivée { get { return new Ensemble[]{Ensemble.C}; } }


	}
}

