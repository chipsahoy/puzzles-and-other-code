using System;
using Gtk;
using POG.Forum;

namespace FennecCount
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			Application.Init ();

			MainWindow win = new MainWindow ();
			win.Show ();
			win.Initialize ();
			Application.Run ();
		}
	}
}
