using System;
using Gtk;

namespace Maths
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			Gtk.Application.Init ();
			MainWindow win = new MainWindow ();
			win.Show ();

//			Fonction f = new Fonction (new Addition ((Inconnue)"x", new Multiplication(new Multiplication((Inconnue)"x",(Inconnue)"x"),(R)3)), (Inconnue)"x");
//			Console.WriteLine (f.GetSimple ());
//			Console.WriteLine (f.GetValue (new R(3,4)));
//			Console.WriteLine (f.Expression);
//			Console.WriteLine (f.Derivee((Inconnue)"x"));

			Console.WriteLine((R)"4");

			Gtk.Application.Run ();
		}
	}
}
