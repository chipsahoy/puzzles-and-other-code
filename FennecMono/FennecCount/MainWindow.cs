using System;
using POG.Forum;
using POG.Werewolf;
//using Gtk;

namespace FennecCount
{
	public partial class MainWindow: Gtk.Window
	{	
		Action<Action> _synchronousInvoker;
		TwoPlusTwoForum _forum;
		IPogDb _db;
		//String _url = @"http://forumserver.twoplustwo.com/59/puzzles-other-games/";
		int childFormNumber;

		public MainWindow (): base (Gtk.WindowType.Toplevel)
		{
			Build ();
		}
		public void Initialize()
		{
			String host = "forumserver.twoplustwo.com";
			_synchronousInvoker = a => Gtk.Application.Invoke(delegate {a.Invoke ();});
			_forum = new TwoPlusTwoForum (_synchronousInvoker, host);
			_forum.LoginEvent += HandleLoginEvent;
			_db = new PogSqlite ();
			String dbPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "/pog";
			System.IO.Directory.CreateDirectory (dbPath);
			String dbName = dbPath + "/pogposts.sqlite";
			_db.Connect (dbName);

			Gtk.Dialog dlg = null;
			try
			{
				dlg = new LoginDialog (_forum);
				dlg.Run ();
			}
			finally
			{
				if(dlg != null)
				{
					dlg.Destroy ();
				}
			}
		}
		private void ShowCounter(String url, Boolean turbo, Int32 day)
		{

			String title = "Window " + childFormNumber++;

			Gtk.Label lbl = new Gtk.Label (title);
			Gtk.Widget childForm = new FennecWidget.VoteCountWidget(_forum, _synchronousInvoker, _db, url, turbo, day);
			tabParent.AppendPage(childForm, lbl);
			childForm.Show ();
			lbl.Show ();
		}

		void HandleLoginEvent (object sender, LoginEventArgs e)
		{
			//Boolean _loggedIn;
			switch (e.LoginEventType)
			{
			case POG.Forum.LoginEventType.LoginFailure:
			{
				//ShowLogin();
			}
				break;
				
			case POG.Forum.LoginEventType.LoginSuccess:
			{
				//_loggedIn = true;
				//openToolStripButton.Enabled = true;
				//tsBtnLogout.Enabled = true;
				{
					//StringCollection games = null; //POG.FennecFox.Properties.Settings.Default.games;
					//if (games == null)
					{
						//games = new StringCollection();
						//POG.FennecFox.Properties.Settings.Default.games = games;
						//POG.FennecFox.Properties.Settings.Default.Save();
						//OpenFile(this, EventArgs.Empty);
					}
					//else
					{
						//foreach (String game in games)
						{
							//String[] parts = game.Split('|');
							//if (parts.Length >= 3)
							{
							//	Boolean turbo = false;
							//	Boolean.TryParse(parts[0], out turbo);
							//	Int32 day = 1;
							//	Int32.TryParse(parts[1], out day);
								//ShowCounter(parts[2], turbo, day);
							}
						}
					}
					//if (URLTextBox.Text == "")
					//{
					//    URLTextBox.Text = POG.FennecFox.Properties.Settings.Default.threadUrl;
					//    if (URLTextBox.Text != "")
					//    {
					//        _day = POG.FennecFox.Properties.Settings.Default.day;
					//        btnStartGame_Click(this, EventArgs.Empty);
					//    }
					//}
				}
			}
				break;
				
			case POG.Forum.LoginEventType.LogoutSuccess:
			{
				//_loggedIn = false;
				//openToolStripButton.Enabled = false;
				//CloseAllToolStripMenuItem_Click(this, EventArgs.Empty);
			}
				break;
			}

		}
		protected void OnDeleteEvent (object sender, Gtk.DeleteEventArgs a)
		{
			Gtk.Application.Quit ();
			a.RetVal = true;
		}

		protected void OnOpenActionActivated (object sender, EventArgs e)
		{
			OpenGame dlg = null;
			try
			{

				dlg = new OpenGame (_forum);
				dlg.Initialize ();
				Gtk.ResponseType rc = (Gtk.ResponseType)dlg.Run ();
				if(rc == Gtk.ResponseType.Ok)
				{
					Boolean turbo;
					String url = dlg.GetURL (out turbo);
					if (url.Length > 0)
					{
						ShowCounter(url, turbo, 1);
					}
				}
			}
			finally
			{
				if(dlg != null)
				{
					dlg.Destroy ();
				}
			}
		}

	}
}