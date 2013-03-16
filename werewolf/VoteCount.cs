using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using System.Globalization;
using System.ComponentModel;
using POG.Utils;
using POG.Forum;
using System.Collections.Specialized;
using System.Configuration;
using System.Threading.Tasks;
using System.Diagnostics; 


namespace POG.Werewolf
{
	public class VoteCount : INotifyPropertyChanged
	{
		#region fields
		IPogDb _db;
		Action<Action> _synchronousInvoker;

		String _url = String.Empty;
		POG.Forum.ThreadReader _thread;
		Int32 _postsPerPage = 100;
		Int32 _threadId;
		Int32 _lastPage = 1;
		private Int32 _lastPost = 0;

		SortableBindingList<Voter> _livePlayers = new SortableBindingList<Voter>();
		Int32 _startPost = 1;
		DateTime? _startTime = null;
		DateTime _endTime;
		Int32? _endPost;
		Int32 _day = 1;

		public readonly String ErrorVote = "Error!";
		public readonly String Unvote = "unvote";
		public readonly String NoLynch = "no lynch";
		#endregion

		#region constructors
		public VoteCount(Action<Action> synchronousInvoker, ThreadReader t, String url, Int32 postsPerPage) 
		{
			_synchronousInvoker = synchronousInvoker;
			_url = url;
			_postsPerPage = postsPerPage;
			_threadId = TwoPlusTwoForum.ThreadIdFromUrl(url);
			_day = 1;
			DateTime now = DateTime.Now;
			_endTime = new DateTime(now.Year, now.Month, now.Day, 18, 0, 0, now.Kind);
			_thread = t;
			_thread.PageCompleteEvent += new EventHandler<PageCompleteEventArgs>(_thread_PageCompleteEvent);
			_thread.ReadCompleteEvent += new EventHandler<ReadCompleteEventArgs>(_thread_ReadCompleteEvent);
			String dbName = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\POG\\pogposts.sqlite";
			_db = new PogSqlite();
			_db.Connect(dbName);
			_db.WriteThreadDefinition(_threadId, url, false);
		}

		~VoteCount()
		{
		}

		#endregion
		#region public methods
		public void CommitRosterChanges()
		{
			_db.WriteRoster(_threadId, _census);
		}
		public void SetDayBoundaries(int day, Int32 startPost, DateTime endTime)
		{
			DateTime? startTimeNull = _db.GetPostTime(_threadId, startPost);
			if (startTimeNull == null)
			{
				startTimeNull = DateTime.MinValue;
			}
			DateTime startTime = startTimeNull.Value.ToUniversalTime();
			_db.WriteDayBoundaries(_threadId, day, startTime, endTime);
			if (day == _day)
			{
				ReadAllFromDB();
			}
		}
		public Int32 GetPlayerId(String name)
		{
			Int32 id = _db.GetPlayerId(name);
			return id;
		}
		public void ChangeDay(int day)
		{
			Day = day;
		}
		public Boolean GetDayBoundaries(int day, out DateTime startTime, out DateTime endTime, out Int32 endPost)
		{
			Boolean rc = _db.GetDayBoundaries(_threadId, day, out startTime, out endTime, out endPost);
			return rc;
		}
		public void IgnoreVote(string player)
		{
			foreach (Voter v in _livePlayers)
			{
				if (v.Name == player)
				{
					_db.SetIgnoreOnBold(v.PostId, v.BoldPosition, true);
					break;
				}
			}
		}

		public void UnIgnoreVote(string player)
		{
			foreach (Voter v in _livePlayers)
			{
				if (v.Name == player)
				{
					_db.WriteUnhide(_threadId, player, v.PostId, _endTime);
					break;
				}
			}
		}

		public void Refresh()
		{
			ReadAllFromDB();
		}

		Boolean _checkingThread;
		public void CheckThread(Action callback)
		{
			Int32 lastPage = 0;
			lastPage = PageFromNumber(_lastPost);
			if (!_checkingThread)
			{
				_checkingThread = true;
				Status = "Checking for new posts...";
				_thread.ReadPages(_url, lastPage, Int32.MaxValue, callback);
			}
			else
			{
				Status = "Already looking for posts.";
			}
		}
		public void CheckThread()
		{
			CheckThread(null);
		}
		public void SetPlayerList(IEnumerable<String> rawList)
		{
			//_db.ReplacePlayerList(_threadId, rawList);
			ReadAllFromDB();
		}
		public void KillPlayer(String player, String cause, DateTimeOffset when, String team)
		{
		}
		public void SubPlayer(String old, String newPlayer, DateTimeOffset when)
		{
		}
		public IEnumerable<String> GetPlayerList()
		{
			IEnumerable<String> rc = _db.GetLivePlayers(_threadId, _startPost);
			return rc;
		}
		public void AddVoteAlias(string bolded, string votee)
		{
			String vote = PrepBolded(bolded);
			_db.WriteAlias(_threadId, bolded, GetPlayerId(votee));
		}
		public string GetPostableVoteCount()
		{
			int? endPost = EndPost;
			int end;
			if (endPost != null)
			{
				end = endPost.Value;
			}
			else
			{
				end = LastPost;
			}
			var sb = new StringBuilder(@"[color=black][b]Votes from post ");
			sb
				.Append(StartPost.ToString())
				.Append(" to post ")
				.Append(end.ToString())
				.AppendLine();

			TimeSpan ts = TimeUntilNight;
			Boolean almostNight = false;
			if (ts > TimeSpan.FromSeconds(0))
			{
				String days;
				switch (ts.Days)
				{
					case 0:
						{
							days = "";
							if (ts.TotalMinutes <= 30)
							{
								almostNight = true;
							}
						}
						break;

					case 1:
						{
							days = "1 day ";
						}
						break;

					default:
						{
							days = ts.Days.ToString() + " days ";
						}
						break;
				}
				sb.AppendFormat("Night in {0}{1}", days, ts.ToString(@"hh\:mm\:ss"));
			}
			else
			{
				sb.Append("It is night");
			}

			sb.AppendLine("[/b][/color]").AppendLine("---")
			.AppendLine("[table=head][b]Votes[/b]\t[b]Lynch[/b]\t[b]Voters[/b]");

			Dictionary<String, List<Voter>> wagons = new Dictionary<string, List<Voter>>();
			List<Voter> listError = new List<Voter>();
			List<Voter> listNoLynch = new List<Voter>();
			List<Voter> listUnvote = new List<Voter>();
			List<Voter> listNotVoting = new List<Voter>();
			string sError = "Error";
			string sNotVoting = "not voting";
			wagons.Add(sError, listError);
			wagons.Add(NoLynch, listNoLynch);
			wagons.Add(Unvote, listUnvote);
			wagons.Add(sNotVoting, listNotVoting);
			// for each live player
			List<Voter> posters;
			posters = new List<Voter>(_livePlayers);
			foreach (Voter p in posters)
			{
				wagons.Add(p.Name, new List<Voter>());
			}
			List<String> legalVoters = new List<string>()
			{
			};
			// find out who they are voting, add vote to that wagon.
			foreach (Voter p in posters)
			{
				if (!(legalVoters.Contains(p.Name)))
				{
//                    continue;
				}
				String votee = p.Votee;
				if (votee == ErrorVote)
				{
					wagons["Error"].Add(p);
				}
				else if (votee == "")
				{
					wagons["not voting"].Add(p);
				}
				else
				{
					wagons[votee].Add(p);
				}
			}
			
			// sort wagons by count
			wagons.Remove(NoLynch);
			wagons.Remove(Unvote);
			wagons.Remove(sNotVoting);
			wagons.Remove(sError);
			foreach (var wagon in wagons)
			{
				Voter v = VoterByName(wagon.Key);
				if (v != null)
				{
					v.VoteCount = wagon.Value.Count;
				}
			}
			var sortedWagons = (from wagon in wagons where (wagon.Value.Count > 0) orderby wagon.Value.Count descending select wagon).ToDictionary(pair => pair.Key, pair => pair.Value);
			// build string with wagons followed by optional {unvote, no lynch, not voting}
			foreach (var wagon in sortedWagons)
			{
				sb
					.AppendFormat("{0} \t [b]{1}[/b] \t {2}", wagon.Value.Count, wagon.Key,
									VoteLinks(wagon.Value, true))
					.AppendLine();
			}
			if (listNoLynch.Count > 0)
			{
				sb
					.AppendFormat("{0} \t {1} \t {2}", listNoLynch.Count, NoLynch,
									VoteLinks(listNoLynch, true))
					.AppendLine();
			}
			if (listUnvote.Count > 0)
			{
				sb
					.AppendFormat("{0} \t {1} \t {2}", listUnvote.Count, Unvote,
									VoteLinks(listUnvote, true))
					.AppendLine();
			}
			if (listNotVoting.Count > 0)
			{
				sb
					.AppendFormat("{0} \t {1} \t {2}", listNotVoting.Count, sNotVoting,
									VoteLinks(listNotVoting, false))
					.AppendLine();
			}
			if (listError.Count > 0)
			{
				sb
					.AppendFormat("{0} \t [color=red][b]{1}[/b][/color] \t {2}", listError.Count, sError,
									VoteLinks(listError, true))
					.AppendLine();
			}
			sb.AppendLine("[/table]");
			if (almostNight)
			{
				DateTime et = _endTime;
				int good = et.Minute;
				int bad = (good + 1) % 60;
				sb.AppendLine();
				sb.AppendFormat("[highlight][color=green]:{0} good[/color] [color=red]:{1} bad[/color][/highlight]",
						good.ToString("00"), bad.ToString("00"));
			}
			return sb.ToString();
		}

		private Voter VoterByName(string p)
		{
			p = p.ToLowerInvariant();

			foreach (Voter v in _livePlayers)
			{
				if (v.Name.ToLowerInvariant() == p)
				{
					return v;
				}
			}
			return null;
		}
		public void WriteCensus(IEnumerable<CensusEntry> censusEntries)
		{
		}
		#endregion
		#region public properties
		public Int32 Day 
		{
			get
			{
				return _day;
			}
			private set
			{
				if (value == _day)
				{
					return;
				}
				_day = value;
				OnPropertyChanged("Day");
				ReadAllFromDB();
			} 
		}

		public SortableBindingList<Voter> LivePlayers
		{
			get
			{
				return _livePlayers;
			}
			private set
			{
				_livePlayers = value;
				OnPropertyChanged("LivePlayers");
			}
		}
		public Int32 ThreadId
		{
			get
			{
				return _threadId;
			}
		}
		public Int32 StartPost
		{
			get
			{
				return _startPost;
			}
			private set
			{
				if (value == _startPost)
				{
					//Trace.TraceInformation("VC: Start post unchanged.");
					return;
				}
				_startPost = value;
				OnPropertyChanged("StartPost");
				StartTime = _db.GetPostTime(_threadId, value);
			}
		}
		public Int32? EndPost
		{
			get
			{
				return _endPost;
			}
			private set
			{
				if (value != _endPost)
				{
					_endPost = value;
					OnPropertyChanged("EndPost");
				}
			}
		}
		public DateTime? StartTime
		{
			get
			{
				DateTime? rc = _startTime;
				return rc;
			}
			private set
			{
				if (value != _startTime)
				{
					_startTime = value;
					OnPropertyChanged("StartTime");
				}
			}
		}
		public DateTime EndTime
		{
			get
			{
				return _endTime;
			}
			private set
			{
				if (value == null)
				{
					value = DateTime.MaxValue;
				}
				value = TruncateTime(value);
				if (value.Kind == DateTimeKind.Utc)
				{
					value = value.ToLocalTime();
				}
				if (value == _endTime)
				{
					return;
				}
				_endTime = value;
				OnPropertyChanged("EndTime");
			}
		}
		public TimeSpan TimeUntilNight
		{
			get
			{
				DateTime now = DateTime.Now;
				now = now.AddMilliseconds(-500);
				DateTime et = _endTime.AddSeconds(60);
				TimeSpan rc = et - now;
				rc = new TimeSpan(rc.Days, rc.Hours, rc.Minutes, rc.Seconds);
				return rc;
			}
		}
		public IEnumerable<String> ValidVotes
		{
			get
			{
				List<String> rc = GetValidVotesList();
				return rc;
			}
		}
		[System.ComponentModel.Bindable(true)]
		public Int32 LastPost
		{
			get
			{
				return _lastPost;
			}
			private set
			{
				if (value != _lastPost)
				{
					_lastPost = value;
					OnPropertyChanged("LastPost");
				}
			}
		}
		String _status;
		public string Status 
		{ 
			get
			{
				return _status;
			}
			private set
			{
				_status = value;
				//Trace.TraceInformation("VC Status: " + value);
				OnPropertyChanged("Status");
			} 
		}
		SortableBindingList<CensusEntry> _census = null;
		public SortableBindingList<CensusEntry> Census
		{
			get
			{
				if (_census == null)
				{
					_census = new SortableBindingList<CensusEntry>();
					foreach (CensusEntry ce in _db.ReadRoster(_threadId))
					{
						_census.Add(ce);
					}
				}
				return _census;
			}
		}
		#endregion
		#region Events
		public event PropertyChangedEventHandler PropertyChanged;
		private void OnPropertyChanged(string propertyName)
		{
			if (PropertyChanged != null)
			{
				_synchronousInvoker.Invoke(
					() => PropertyChanged(this, new PropertyChangedEventArgs(propertyName))
				);
			}
		}
		#endregion
		#region forum event handlers
		void _thread_PageCompleteEvent(object sender, PageCompleteEventArgs e)
		{
			_db.AddPosts(e.Posts);
			Status = "Finished Page " + e.Page.ToString();
		}
		void _thread_ReadCompleteEvent(object sender, ReadCompleteEventArgs e)
		{
			_lastPage = e.pageEnd;
			_checkingThread = false;
			ReadAllFromDB();
			Status = "Finished reading posts at " + DateTime.Now.ToShortTimeString();
			if (e.Cookie != null)
			{
				Action callback = (Action)e.Cookie;
				if (callback != null)
				{
					callback();
				}
			}
		}

		#endregion
		#region private methods
		void ReadAllFromDB()
		{
			DateTime startTime;
			DateTime endTime;
			Int32 endPost;
			_db.GetDayBoundaries(_threadId, _day, out startTime, out endTime, out endPost);
			StartPost = _db.GetPostBeforeTime(_threadId, startTime.ToUniversalTime()) + 1;
			EndTime = endTime;
			EndPost = endPost;
			SortableBindingList<Voter> livePlayers = new SortableBindingList<Voter>();
			foreach (VoterInfo vi in _db.GetVotes(_threadId, startTime.ToUniversalTime(), _endTime.ToUniversalTime(), this))
			{
				Voter v = new Voter(vi, this);
				livePlayers.Add(v);
			}
			LivePlayers = livePlayers;

			Int32? maxPost = _db.GetMaxPost(_threadId);
			if (maxPost != null)
			{
				LastPost = maxPost.Value;
				_lastPage = PageFromNumber(maxPost.Value);
			}
			else
			{
				LastPost = 0;
			}
			GetPostableVoteCount(); // updates counts.

		}

		private List<String> GetValidVotesList()
		{
			List<String> validVotes = new List<string>();
			foreach (Voter p in _livePlayers)
			{
				validVotes.Add(p.Name);
			}
			validVotes.Sort();
			validVotes.Add(Unvote);
			validVotes.Add(NoLynch);
			return validVotes;
		}
		private String VoteLinks(List<Voter> wagon, Boolean linkToVote)
		{
			StringBuilder sb = new StringBuilder();
			foreach (Voter voter in wagon)
			{
				if (linkToVote)
				{
					sb.AppendFormat(", [url={0}]{1}[/url] ({2})", voter.PostLink, voter.Name, voter.PostCount);
				}
				else
				{
					sb.AppendFormat(", {0} ({1})", voter.Name, voter.PostCount);
				}
			}

			if (sb.Length > 2) // get rid of ", " from first item.
			{
				sb.Remove(0, 2);
			}

			return sb.ToString();
		}

		private int PageFromNumber(int number)
		{
			int page = (number / _postsPerPage) + 1;
			return page;
		}
		private String PrepBolded(String bolded)
		{
			String vote = bolded.ToLower().Replace(" ", "");
			if (vote.StartsWith("vote:"))
			{
				vote = vote.Substring(5).Trim();
			}

			if (vote.StartsWith("vote"))
			{
				vote = vote.Substring(4).Trim();
			}
			return vote;
		}
		internal String ParseBoldedToVote(String bolded)
		{
			String vote = PrepBolded(bolded);
			if (vote == "")
			{
				return "";
			}
			String player = ValidVotes.FirstOrDefault(p => p.ToLower().Replace(" ", "") == vote || p.ToLower().Replace(" ", "").StartsWith(vote));
			if (player == null)
			{
				// check if there is a mapping defined for this vote => player
				String suggestion = _db.GetAlias(_threadId, bolded);
				if (suggestion != String.Empty)
				{
					player =
						ValidVotes.FirstOrDefault(
							p => p == suggestion);
				}
				if (player == null)
				{
					player = ErrorVote;
				}
			}
			return player;
		}
		DateTime TruncateTime(DateTime dt)
		{
			DateTime rc = new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, 0, 0);
			return rc;
		}
		#endregion
		#region internal methods
		#endregion




		public bool Turbo { get; set; }

		public Action<Action> SynchronousInvoker
		{
			get
			{
				return _synchronousInvoker;
			}
		}


	}
}
