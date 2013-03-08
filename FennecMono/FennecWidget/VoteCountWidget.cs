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
		Gtk.ListStore _votes = new Gtk.ListStore (typeof(bool),
			typeof(String), typeof(int), typeof(int),
            typeof(String), typeof(String), typeof(String),
		    typeof(String)
		);
		AutoComplete _autoComplete;
		VoteCount _voteCount;
		TwoPlusTwoForum _forum;
		Action<Action> _synchronousInvoker;
		String _url; 
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
		                        String url, 
		                        Boolean turbo, 
		                        Int32 day)
		{
			_forum = forum;
			_synchronousInvoker = synchronousInvoker;
			_url = url;
			_turbo = turbo;
			_day = day;

			this.Build ();
			_autoComplete = new AutoComplete(_forum, _synchronousInvoker);
			BindToNewGame (_url);
			SetupVoteGrid ();
			FillVoteGrid ();
		}
		private void BindToNewGame(String url)
		{
			url = POG.Utils.Misc.NormalizeUrl(url);
			ThreadReader t = _forum.Reader();
			_voteCount = new VoteCount(_synchronousInvoker, t, url, _forum.PostsPerPage);
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
			grdVotes.ModifyFont (Pango.FontDescription.FromString ("normal 8"));

			Gtk.CellRendererToggle crToggle = new Gtk.CellRendererToggle ();
			crToggle.Activatable = true;
			grdVotes.AppendColumn ("Dead", crToggle, "active", 0);

			Gtk.CellRendererText crt = new Gtk.CellRendererText ();
			grdVotes.AppendColumn ("player", crt, "text", 1);

			crt = new Gtk.CellRendererText ();
			grdVotes.AppendColumn ("votes", crt, "text", 2);

			crt = new Gtk.CellRendererText ();
			grdVotes.AppendColumn ("posts", crt, "text", 3);

			crt = new Gtk.CellRendererText ();
			grdVotes.AppendColumn ("post", crt, "text", 4);

			crt = new Gtk.CellRendererText ();
			grdVotes.AppendColumn ("time", crt, "text", 5);

			crt = new Gtk.CellRendererText ();
			grdVotes.AppendColumn ("votes for", crt, "text", 6);
		
			crt = new Gtk.CellRendererText ();
			grdVotes.AppendColumn ("content", crt, "text", 7);
		}
		void FillVoteGrid()
		{
			_votes.Clear ();
			foreach (Voter v in _voteCount.LivePlayers) {
				_votes.AppendValues(false, v.Name, v.VoteCount, v.PostCount,
				                    v.PostNumber, v.PostTime.ToString ("HH:mm"), v.Votee, v.Bolded);
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
			if (e.PropertyName == "StartTime")
			{
				//String time = String.Empty;
				if (_voteCount.StartTime != null)
				{
					//time = _voteCount.StartTime.Value.ToString("g");
					udStartPost.Text = _voteCount.StartPost.ToString ();
				}
				//dtStartTime.Text = time;
			}
			if (e.PropertyName == "EndTime")
			{
				String time = String.Empty;
				time = _voteCount.EndTime.ToString("g");
				TxtEndPost.Text = time;
			}
			if (e.PropertyName == "Status")
			{

				lblStatus.Text = _voteCount.Status; // no direct binding support in status strip.
			}
			if (e.PropertyName == "Day")
			{
				//udDay.Value = _voteCount.Day;
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

	}
}

