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
			Relation r = new Multiplication ((R)5, new Addition(new Inconnue("x"), (R)2));
//			r = new Addition (new Inconnue("x"), new Inconnue ("x"));
//			r = (Relation)r.GetSimple ();
//			r = new Multiplication (r, (R)5);
			Console.WriteLine (r.GetSimple());
			
			Application.Run ();
		}
	}
}
