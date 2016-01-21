using System;
using Gtk;

namespace Maths
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			Application.Init ();
			MainWindow win = new MainWindow ();
			win.Show ();

			Fonction f = new Fonction (new Addition ((Inconnue)"x", new Multiplication(new Multiplication((Inconnue)"x",(Inconnue)"x"),(R)3)), (Inconnue)"x");
			Console.WriteLine (((Relation)f.Expression).GetSimple ());
			Console.WriteLine (f.GetValue ((R)3));
			Console.WriteLine (f.GetValue (new Multiplication ((R)4, (R)3)));
			Console.WriteLine (f.Expression);
			
			Application.Run ();
		}
	}
}
