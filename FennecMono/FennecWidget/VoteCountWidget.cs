using System;
using System.ComponentModel;
using System.Collections.Generic;

using POG.Forum;
using POG.Werewolf;

namespace FennecWidget
{
	[System.ComponentModel.ToolboxItem(true)]
	public partial class VoteCountWidget : Gtk.Bin
	{
		Gtk.ListStore _votes = new Gtk.ListStore (
			typeof(String), typeof(int), typeof(int),
            typeof(String), typeof(String), typeof(String),
		    typeof(String)
		);
		Gtk.ListStore _validVotes = new Gtk.ListStore (
			typeof(String)
		);
		AutoComplete _autoComplete;
		VoteCount _voteCount;
		IPogDb _db;
		TwoPlusTwoForum _forum;
		Action<Action> _synchronousInvoker;
		String _url;
		String _forumURL;
		Boolean _turbo; 
		Int32 _day;
		private enum CounterColumn
		{
			Player = 0,
			VoteCount,
			PostCount,
			PostNumber,
			PostLink,
			PostTime,
			VotesFor,
			Bolded,
		};
		

		public VoteCountWidget (TwoPlusTwoForum forum, 
		                        Action<Action> synchronousInvoker,
		                        IPogDb db,
		                        String url, 
		                        Boolean turbo, 
		                        Int32 day)
		{
			_forum = forum;
			_synchronousInvoker = synchronousInvoker;
			_db = db;
			_url = url;
			_turbo = turbo;
			_day = day;
			_forumURL = _forum.ForumURL;
			this.Build ();
			_autoComplete = new AutoComplete(_forum, _synchronousInvoker, _db);
			BindToNewGame (_url);
			SetupVoteGrid ();
			FillVoteGrid ();
		}
		private void BindToNewGame(String url)
		{
			url = POG.Utils.Misc.NormalizeUrl(url);
			ThreadReader t = _forum.Reader();
			_voteCount = new VoteCount(_synchronousInvoker, t, _db, _forumURL, url, _forum.PostsPerPage);
			_voteCount.PropertyChanged += new PropertyChangedEventHandler(_voteCount_PropertyChanged);

			_voteCount.Turbo = _turbo;

			_voteCount.ChangeDay(_day);
			_voteCount.Refresh();
			_voteCount.CheckThread();
		}
		
		private void UnbindFromGame()
		{
			if (_voteCount != null)
			{
				_voteCount.PropertyChanged -= _voteCount_PropertyChanged;
				_voteCount = null;
			}
		}
		void SetupVoteGrid()
		{
			grdVotes.Model = _votes;
			//grdVotes.ModifyFont (Pango.FontDescription.FromString ("normal 8"));

			Gtk.CellRendererText crt = new Gtk.CellRendererText ();
			grdVotes.AppendColumn ("player", crt, "text", 0);

			crt = new Gtk.CellRendererText ();
			grdVotes.AppendColumn ("votes", crt, "text", 1);

			crt = new Gtk.CellRendererText ();
			grdVotes.AppendColumn ("posts", crt, "text", 2);

			crt = new Gtk.CellRendererText ();
			grdVotes.AppendColumn ("post", crt, "text", 3);

			crt = new Gtk.CellRendererText ();
			grdVotes.AppendColumn ("time", crt, "text", 4);

			Gtk.CellRendererCombo crcb = new Gtk.CellRendererCombo ();
			crcb.Editable = true;
			crcb.Model = _validVotes;
			crcb.TextColumn = 0;
			crcb.Edited += VoteFixed;

			Gtk.TreeViewColumn tvColumn = new Gtk.TreeViewColumn ();
			tvColumn.PackStart (crcb, true);
			tvColumn.Title = "votes for";
			tvColumn.AddAttribute (crcb, "text", 5);
			grdVotes.AppendColumn (tvColumn);
		
			crt = new Gtk.CellRendererText ();
			grdVotes.AppendColumn ("content", crt, "text", 6);
		}
		void VoteFixed(object o, Gtk.EditedArgs args)
		{
			Gtk.TreeSelection sel = grdVotes.Selection;
			Gtk.TreeIter iter;
			if (!sel.GetSelected (out iter)) {
				return;
			}
			String bolded = (String)_votes.GetValue (iter, 6);
			_voteCount.AddVoteAlias (bolded, args.NewText);
			_voteCount.Refresh ();
		}
		void FillVoteGrid()
		{
			_votes.Clear ();
			foreach (Voter v in _voteCount.LivePlayers) {
				_votes.AppendValues(v.Name, v.VoteCount, v.PostCount,
				                    v.PostNumber.ToString (), v.PostTime.ToString ("HH:mm"), v.Votee, v.Bolded);
			}
			_validVotes.Clear ();
			foreach (String wagon in _voteCount.ValidVotes) {
				_validVotes.AppendValues (wagon);
			}
		}
		private void _voteCount_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (!Visible)
			{
				return;
			}
			if (e.PropertyName == "LivePlayers")
			{
				FillVoteGrid();
			}
			{
				String time = String.Empty;
				if (_voteCount.StartTime != null) {
					time = _voteCount.StartTime.Value.ToString ("g");
					udStartPost.Text = _voteCount.StartPost.ToString ();
				}
				txtStartPostTime.Text = time;
			}

			{
				String time = String.Empty;
				time = _voteCount.EndTime.ToString("g");
				Int32? endPost = _voteCount.EndPost;
				if(endPost != null)
				{
					TxtEndPost.Text = endPost.Value.ToString ();
				}
				txtEndPostTime.Text = time;
			}
			{

				lblStatus.Text = _voteCount.Status; // no direct binding support in status strip.
			}
		}

		private void AddToRoster(String name)
		{
			name = name.Split('\t')[0];
			name = name.Trim();
			if (_autoComplete.GetPosterId(name,
			                              (String poster, int id) =>
			                              {
				if (id > 0)
				{
					AddConfirmedPoster(poster);
				}
				else
				{
					String err = String.Format("\"{0}\" is not a poster. Check the spelling.", poster);
					Gtk.MessageDialog msg = new Gtk.MessageDialog(null, 
						Gtk.DialogFlags.DestroyWithParent, Gtk.MessageType.Error, Gtk.ButtonsType.Close,  
					    err);
					msg.Run ();
					msg.Destroy ();
				}
			}
			) > 0)
			{
				AddConfirmedPoster(name);
			}
		}
		void AddConfirmedPoster(String name)
		{
			foreach (CensusEntry e in _voteCount.Census)
			{
				if (e.Name == name)
				{
					return;
				}
			}
			CensusEntry ce = new CensusEntry();
			ce.Name = name;
			ce.Alive = "Alive";
			_voteCount.Census.Add(ce);
		}
		
		protected void OnBtnPlayerListClicked (object sender, EventArgs e)
		{
			PlayerList dlg = null;
			try
			{
				List<String> players = new List<String>(_voteCount.GetPlayerList ());
				dlg = new PlayerList (players);

				Gtk.ResponseType rc = (Gtk.ResponseType)dlg.Run ();
				if(rc == Gtk.ResponseType.Ok)
				{
					_voteCount.Census.Clear ();
					players = dlg.GetPlayers ();
					foreach(String p in players)
					{
						if(p.Length > 0)
						{
							AddToRoster (p);
						}
					}
					_voteCount.CommitRosterChanges();
					_voteCount.Refresh();
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

		protected void OnBtnGetPostsClicked (object sender, EventArgs e)
		{
			_voteCount.CheckThread ();
		}

		protected void OnBtnEODClicked (object sender, EventArgs e)
		{
			DayTimes dlg = null;
			try
			{
				dlg = new DayTimes (_voteCount);
				
				Gtk.ResponseType rc = (Gtk.ResponseType)dlg.Run ();
				if(rc == Gtk.ResponseType.Ok)
				{
					Int32 day;
					Int32 startPost;
					DateTime endTime;
					dlg.GetDayBoundaries(out day, out startPost, out endTime);
					_voteCount.SetDayBoundaries(day, startPost, endTime);
					_voteCount.ChangeDay(day);
					_voteCount.Refresh();
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

		protected void OnBtnPostItClicked (object sender, EventArgs e)
		{
			String count = _voteCount.GetPostableVoteCount ();
			_forum.MakePost (_voteCount.ThreadId, "Posted from a freakin mac", count, 0, false);
		}

		protected void OnBtnCopyCountClicked (object sender, EventArgs e)
		{
			String msg = _voteCount.GetPostableVoteCount ();
			Gtk.Clipboard clip = Gtk.Clipboard.Get (Gdk.Atom.Intern ("CLIPBOARD", false));
			clip.Text = msg;
		}

	}
}

