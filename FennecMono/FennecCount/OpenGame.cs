using System;
using POG.Forum;

namespace FennecCount
{
	public partial class OpenGame : Gtk.Dialog
	{
		LobbyReader _lobby;
		Gtk.ListStore _threads = new Gtk.ListStore (typeof(String));
		String _url = @"http://forumserver.twoplustwo.com/59/puzzles-other-games/";

		public OpenGame (POG.Forum.TwoPlusTwoForum forum)
		{
			_lobby = forum.Lobby ();
			this.Build ();
			Gtk.TreeViewColumn c = new Gtk.TreeViewColumn ();
			c.Title = "POG Games";
			treeThreads.AppendColumn (c);
			treeThreads.Model = _threads;
			Gtk.CellRendererText crt = new Gtk.CellRendererText ();
			c.PackStart (crt, true);
			c.AddAttribute (crt, "text", 0);
			//treeThreads.ModifyFont (Pango.FontDescription.FromString ("normal 8"));
		}
		public void Initialize()
		{
			_lobby.LobbyPageCompleteEvent+= HandleLobbyPageCompleteEvent;
			_lobby.ReadLobby(_url, 1, 1, true);

		}
		public String GetURL(out Boolean turbo)
		{
			turbo = false;
			String rc = txtURL.Text;
			return rc;
		}


		void HandleLobbyPageCompleteEvent (object sender, LobbyPageCompleteEventArgs e)
		{
			foreach (ForumThread t in e.Threads)
			{
				if ((t.ThreadIconText == "Spade") || (t.ThreadIconText == "Club"))
				{
					String url = t.URL.Substring (_url.Length);
					_threads.AppendValues (url);
				}
			}
		}

		protected void OnTreeThreadsCursorChanged (object sender, EventArgs e)
		{
			Gtk.TreePath path;
			Gtk.TreeViewColumn column;
			treeThreads.GetCursor (out path, out column);
			Gtk.TreeSelection sel = treeThreads.Selection;
			Gtk.TreeModel model;
			Gtk.TreeIter iter;
			sel.GetSelected (out model, out iter);
			String url = (String)model.GetValue (iter, 0);
			txtURL.Text = _url + url;
		}
	}
}

