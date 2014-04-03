﻿using System;
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
using System.Text.RegularExpressions; 


namespace POG.Werewolf
{
    public class VoteCount
    {
        public VoteCount()
        {
        }
        public Int32 StartPost { get; private set; }
        public Int32 EndPost { get; private set; }
        public Dictionary<String, List<Voter>> Wagons { get; private set; }
        public List<String> LivePlayers { get; private set; }
        public String PostableCount()
        {
            return "";
        }
    }
	public class ElectionInfo : INotifyPropertyChanged
	{
		#region fields
		IPogDb _db;
		Action<Action> _synchronousInvoker;

		String _url = String.Empty;
		String _forumURL = String.Empty;
		POG.Forum.Language _language = Language.English;
		POG.Forum.ThreadReader _thread;
		Int32 _postsPerPage = 100;
		Int32 _threadId;
		private Int32 _lastPost = 0;

		SortableBindingList<Voter> _livePlayers = new SortableBindingList<Voter>();
		Int32 _startPost = 1;
		DateTime? _startTime = null;
		DateTime _endTime;
		Int32? _endPost;
		Int32 _day = 1;
		String _postableCount;
        Boolean _final = false; // lock votes with " final" at end.
		List<String> _leaders =  new List<string>();
		Int32 _leaderVotes = 0;

		public readonly String ErrorVote = "Error!";
		public readonly String Unvote = "unvote";
		public readonly String NoLynch = "no lynch";
		#endregion

		#region constructors
		public ElectionInfo(Action<Action> synchronousInvoker, ThreadReader t, IPogDb db, String forum, String url, Int32 postsPerPage, Language language) 
		{
			_synchronousInvoker = synchronousInvoker;
			_forumURL = forum;
			_url = url;
			_postsPerPage = postsPerPage;
			_language = language;
			_threadId = VBulletinForum.ThreadIdFromUrl(url);
			_day = 1;
			DateTime now = DateTime.Now;
			_endTime = new DateTime(now.Year, now.Month, now.Day, 18, 0, 0, now.Kind);
			_thread = t;
			_thread.PageCompleteEvent += new EventHandler<PageCompleteEventArgs>(_thread_PageCompleteEvent);
			_thread.ReadCompleteEvent += new EventHandler<ReadCompleteEventArgs>(_thread_ReadCompleteEvent);
			_db = db;
			_db.WriteThreadDefinition(_threadId, url, false);
            if (forum.Contains("mindromp.org")) _final = true;
		}

		~ElectionInfo()
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
			_db.WriteDayBoundaries(_threadId, day, startPost, endTime);
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
		public Boolean GetDayBoundaries(int day, out DateTime? startTime, out Int32 startPost, out DateTime endTime, out Int32 endPost)
		{
			Boolean rc = _db.GetDayBoundaries(_threadId, day, out startPost, out endTime, out endPost);
			startTime = _db.GetPostTime(_threadId, startPost);
			return rc;
		}
		public void IgnoreVote(string player)
		{
			foreach (Voter v in _livePlayers)
			{
				if (v.Name == player)
				{
					_db.SetIgnoreOnBold(v.PostId, v.BoldPosition, true);
					ReadAllFromDB();
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
					ReadAllFromDB();
					break;
				}
			}
		}
		public void KillPlayer(String name)
		{
			_db.KillPlayer(_threadId, name, LastPost);
			ReadAllFromDB();
		}
		public void SubPlayer(String oldPlayer, String newPlayer)
		{
			_db.SubPlayer(_threadId, oldPlayer, newPlayer);
			ReadAllFromDB();
				
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
				_postableCount = String.Empty;
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
		public IEnumerable<String> GetPlayerList()
		{
			IEnumerable<String> rc = _db.GetLivePlayers(_threadId, _startPost);
			return rc;
		}
		public void AddVoteAlias(string bolded, string votee)
		{
			_db.WriteAlias(_threadId, bolded, GetPlayerId(votee));
		}
		public string GetPostableVoteCount()
		{
			return _postableCount;
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
        Boolean _checkMajority = false;
        public Boolean CheckMajority
        {
            get
            {
                return _checkMajority;
            }
            set
            {
                if (_checkMajority != value)
                {
                    _checkMajority = value;
                    ReadAllFromDB();
                }
            }
        }
        public Int32 MajorityPost
        {
            get;
            private set;
        }
		[System.ComponentModel.Bindable(true)]
		Boolean _lockedVotes;
		public Boolean LockedVotes
		{
			get
			{
				return _lockedVotes;
			}
			set
			{
				if (_lockedVotes != value)
				{
					_lockedVotes = value;
					ReadAllFromDB();
				}
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
		private void CreatePostableVoteCount()
		{
			_leaders.Clear();
			_leaderVotes = 0;

			string sError = "Error";
			string sNotVoting = "not voting";
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
			String sStart = "[color=black][b]Votes from post {0} to post {1}";
			String sTimeToNight = "Night in {0}";
			String sNight = "It is night";
			String startTable = "[table=head][b]Votes[/b]\t[b]Lynch[/b]\t[b]Voters[/b]";
			String sWagonLine = "{0} \t [b]{1}[/b] \t {2}";
			String sNoVoteLine = "{0} \t {1} \t {2}";
			String showNotVoting = sNotVoting;
			String sErrorLine = "{0} \t [color=red][b]{1}[/b][/color] \t {2}";
			String redError = sError;
			String sGoodBad = "[highlight][color=green]:{0} good[/color] [color=red]:{1} bad[/color][/highlight]";
			String sOneDay = "1 day ";
			String sDays = "{0} days ";
			String sLockedVotes = "[highlight][color=red]Caution! Votes today are locked in. No vote changing or unvoting today.[/color][/highlight]";

			switch (_language)
			{
				case Language.Estonian:
					{
						sStart = "[color=black][b]Hääled seisuga post {0} kuni {1}";
						sTimeToNight = "Öö saabub {0} pärast";
						sNight = "On öö";
						startTable = "[table= class: grid][tr][td][b]Hääli[/b][/td][td][b]Lynch[/b][/td][td][b]Hääletajad[/b][/td][/tr]";
						sWagonLine = "[tr][td]{0} [/td][td] [b]{1}[/b] [/td][td] {2}[/td][/tr]";
						sNoVoteLine = "[tr][td]{0} [/td][td] {1} [/td][td] {2}[/td][/tr]";
						sErrorLine = "[tr][td]{0} [/td][td] [color=red][b]{1}[/b][/color] [/td][td] {2}[/td][/tr]";
						showNotVoting = "ei ole hääletanud";
						redError = "Viga";
						sGoodBad = "[highlight][color=green]:{0}  loeb[/color] [color=red]:{1} ei loe[/color][/highlight]";
						sOneDay = "1 päeva ja ";
						sDays = "{0} päeva ja ";
					}
					break;
			}
			var sb = new StringBuilder();
			sb.AppendFormat(sStart, StartPost, end);
			sb.AppendLine();

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
							days = sOneDay;
						}
						break;

					default:
						{
							days = String.Format(sDays, ts.Days);
						}
						break;
				}
				if (ts.TotalHours < 72.0)
				{
					String formatted = (int)ts.TotalHours + ts.ToString(@"\:mm\:ss");
					sb.AppendFormat(sTimeToNight, formatted);
				}
				else
				{
					sb.AppendFormat(sTimeToNight, days);
				}
			}
			else
			{
				sb.Append(sNight);
			}

			sb.AppendLine("[/b][/color]").AppendLine("---")
			.AppendLine(startTable);

			Dictionary<String, List<Voter>> wagons = new Dictionary<string, List<Voter>>();
			List<Voter> listError = new List<Voter>();
			List<Voter> listNoLynch = new List<Voter>();
			List<Voter> listUnvote = new List<Voter>();
			List<Voter> listNotVoting = new List<Voter>();
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
 
			wagons.Remove(Unvote);
			wagons.Remove(sNotVoting);
			wagons.Remove(sError);

			_leaderVotes = wagons.Max(w => w.Value.Count);
			_leaders = (from w in wagons where w.Value.Count == _leaderVotes select w.Key).ToList();
			if ((_leaderVotes == 0))
			{
				_leaders = (from p in LivePlayers select p.Name).ToList();
			}
			wagons.Remove(NoLynch);


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
					.AppendFormat(sWagonLine, wagon.Value.Count, wagon.Key,
									VoteLinks(wagon.Value, true))
					.AppendLine();
			}
			if (listNoLynch.Count > 0)
			{
				sb
					.AppendFormat(sNoVoteLine, listNoLynch.Count, NoLynch,
									VoteLinks(listNoLynch, true))
					.AppendLine();
			}
			if (listUnvote.Count > 0)
			{
				sb
					.AppendFormat(sNoVoteLine, listUnvote.Count, Unvote,
									VoteLinks(listUnvote, true))
					.AppendLine();
			}
			if (listNotVoting.Count > 0)
			{
				sb
					.AppendFormat(sNoVoteLine, listNotVoting.Count, showNotVoting,
									VoteLinks(listNotVoting, false))
					.AppendLine();
			}
			if (listError.Count > 0)
			{
				sb
					.AppendFormat(sErrorLine, listError.Count, redError,
									VoteLinks(listError, true))
					.AppendLine();
			}
			sb.AppendLine("[/table]");
			if (_lockedVotes)
			{
				sb.AppendLine();
				sb.Append(sLockedVotes);
			}
			if (almostNight)
			{
				DateTime et = _endTime;
				int good = et.Minute;
				int bad = (good + 1) % 60;
				sb.AppendLine();
				sb.AppendFormat(sGoodBad,
						good.ToString("00"), bad.ToString("00"));
			}
            if (MajorityPost > 0)
            {
                Int32 postId = _db.GetPostId(_threadId, MajorityPost);
                String postLink = String.Format("{0}showpost.php?p={1}&postcount={2}",
                    ForumURL, postId, MajorityPost);

                sb.AppendFormat("\n[highlight]*** MAJORITY LYNCH IN [url={1}]POST {0}[/url] ***[/highlight]\n", MajorityPost, postLink);
            }
			_postableCount = sb.ToString();
		}
		void ReadAllFromDB()
		{
			Int32 startPost;
			DateTime endTime;
			Int32 endPost;
			_db.GetDayBoundaries(_threadId, _day, out startPost, out endTime, out endPost);
			StartPost = startPost;
			
			EndTime = endTime;
			EndPost = endPost;
			SortableBindingList<Voter> livePlayers = new SortableBindingList<Voter>();
            IEnumerable<VoterInfo2> voters = _db.GetAllVotes(_threadId, startPost, _endTime.ToUniversalTime(), this);
            MajorityInfo maj = GetActiveVote(voters, _checkMajority, _lockedVotes);
            if (maj != null)
            {
                MajorityPost = maj.PostNumber;
            }
            else
            {
                MajorityPost = 0;
            }
            foreach (VoterInfo2 vi in voters)
			{
				Voter v = new Voter(vi, this);
				livePlayers.Add(v);
			}
			LivePlayers = livePlayers;

			Int32? maxPost = _db.GetMaxPost(_threadId);
			if (maxPost != null)
			{
				LastPost = maxPost.Value;
			}
			else
			{
				LastPost = 0;
			}
			CreatePostableVoteCount(); // updates counts.

		}
        private MajorityInfo GetActiveVote(IEnumerable<VoterInfo2> voters, Boolean checkMajority, Boolean lockedVotes)
        {
            MajorityInfo rc = null;
            Int32 majCount = (1 + voters.Count()) / 2;
            Dictionary<String, String> voterVote = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
            Dictionary<String, HashSet<String>> wagons = new Dictionary<string, HashSet<string>>(StringComparer.InvariantCultureIgnoreCase);
            Dictionary<String, VoterInfo2> voterInfos = new Dictionary<string, VoterInfo2>();
            List<Vote> votes = new List<Vote>();
            foreach (var vi in voters)
            {
                voterInfos.Add(vi.Name, vi);
                votes.AddRange(vi.Votes);
                voterVote.Add(vi.Name, "");
                wagons.Add(vi.Name, new HashSet<string>(StringComparer.InvariantCultureIgnoreCase));
            }
            if (!lockedVotes)
            {
                wagons.Add(NoLynch, new HashSet<string>());
            }
            votes.Sort((a, b) =>
            {
                if (a.PostId == b.PostId) return a.BoldPosition.CompareTo(b.BoldPosition);
                return a.PostId.CompareTo(b.PostId);
            });
            foreach (var v in votes)
            {
                String oldVote = voterVote[v.Voter];
                if (wagons.ContainsKey(oldVote))
                {
                    if (lockedVotes)
                    {
                        // you can't change votes and previously had a valid vote.
                        continue;
                    }
                    wagons[oldVote].Remove(v.Voter);
                    voterVote[v.Voter] = "";
                }
                String votee = ParseInputToVote(v.Bolded);
                voterInfos[v.Voter].SetVote(v.Bolded, v.PostNumber, v.PostTime, v.PostId, v.BoldPosition);
                if (wagons.ContainsKey(votee))
                {
                    voterVote[v.Voter] = votee;
                    wagons[votee].Add(v.Voter);
                }
                if (checkMajority)
                {
                    var lynch = (from w in wagons where (w.Value.Count >= majCount) select w.Key).FirstOrDefault();
                    if (lynch != null)
                    {
                        rc = new MajorityInfo(v.PostNumber, lynch);
                        return rc;
                    }
                }
            }
            return rc;
        }

        private string ParseInputToVote(string p)
        {
            return ParseBoldedToVote(p);
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
			String vote = bolded.Trim().ToLowerInvariant();
			if (vote.StartsWith("vote:"))
			{
				vote = vote.Substring(5).Trim();
			}

			if (vote.StartsWith("vote"))
			{
				vote = vote.Substring(4).Trim();
			}
            if (_final)
            {
                String final = " final";
                if (vote.EndsWith(final))
                {
                    vote = vote.Substring(0, vote.Length - final.Length);
                }
            }
			return vote;
		}
        public class Alias
        {
            public String Original {get; private set;}
            public String MapsTo {get; private set;}
            public Alias(String original, String mapsTo)
            {
                Original = original;
                MapsTo = mapsTo;
            }
        }
        public String ParseInputToChoice(String input, IEnumerable<Alias> choices)
        {
            // check for exact match.
            String rc = (from c in choices 
                         where String.Compare(input, c.Original, StringComparison.InvariantCultureIgnoreCase) == 0 
                         select c.MapsTo).FirstOrDefault();
            if (rc != null) return rc;
            // vote prefix
            if (input.StartsWith("vote:", StringComparison.InvariantCultureIgnoreCase))
            {
                input = input.Substring("vote:".Length).Trim();
            }
            else if (input.StartsWith("vote", StringComparison.InvariantCultureIgnoreCase))
            {
                input = input.Substring("vote".Length).Trim();
            }
            //aliases
            String suggestion = _db.GetAlias(_threadId, input);
            if (suggestion != String.Empty)
            {
                rc =
                    (from c in choices
                     where String.Compare(suggestion, c.Original, StringComparison.InvariantCultureIgnoreCase) == 0
                     select c.MapsTo).FirstOrDefault();
                if (rc != null) return rc;
            }
            //punctuation
            //whitespace
            List<Alias> newChoices = new List<Alias>();
            foreach (var c in choices)
            {
                String newOriginal = new String(c.Original.Where(ch => char.IsLetterOrDigit(ch)).ToArray());
                Alias a = new Alias(newOriginal, c.MapsTo);
                if (newOriginal.Length >= 2)
                {
                    newChoices.Add(a);
                }
                else
                {
                    newChoices.Add(c);
                }
            }
            String newInput = new String(input.Where(ch => char.IsLetterOrDigit(ch)).ToArray());
            rc = (from c in newChoices
                         where String.Compare(newInput, c.Original, StringComparison.InvariantCultureIgnoreCase) == 0
                         select c.MapsTo).FirstOrDefault();
            if (rc != null) return rc;

			//initials
			foreach (var c in newChoices)
			{
				if (c.Original[0].ToString().Equals(newInput[0].ToString()) == false) continue;
				int ixMatch = -1;
				foreach (char ch in newInput)
				{
					int ix = c.Original.IndexOf(ch.ToString(), ixMatch + 1, StringComparison.InvariantCultureIgnoreCase);
					ixMatch = ix;
					if (ix == -1) break;
				}
				if (ixMatch > 0)
				{
					return c.MapsTo;
				}
			}
			// omitted letters
			foreach (var c in newChoices)
			{
				int ixMatch = -1;
				foreach (char ch in newInput)
				{
					int ix = c.Original.IndexOf(ch.ToString(), ixMatch + 1, StringComparison.InvariantCultureIgnoreCase);
					ixMatch = ix;
					if (ix == -1) break;
				}
				if (ixMatch > 0)
				{
					return c.MapsTo;
				}
			}

			
            //repeated letters
            Regex r = new Regex("(.)(?<=\\1\\1)", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled);

            List<Alias> newChoices2 = new List<Alias>();
            foreach (var c in newChoices)
            {
                String newOriginal = r.Replace(c.Original, String.Empty);
                Alias a = new Alias(newOriginal, c.MapsTo);
                if (newOriginal.Length >= 2)
                {
                    newChoices2.Add(a);
                }
                else
                {
                    newChoices2.Add(c);
                }
            }
            String newInput2 = r.Replace(newInput, String.Empty);
			if (newInput2.Length < 2) newInput2 = newInput;
            rc = (from c in newChoices2
                  where String.Compare(newInput2, c.Original, StringComparison.InvariantCultureIgnoreCase) == 0
                  select c.MapsTo).FirstOrDefault();
            if (rc != null) return rc;
            //subset
            rc = (from c in newChoices2
                  where c.Original.Contains(newInput2)
                  select c.MapsTo).FirstOrDefault();
            if (rc != null) return rc;
            //initials
            // omitted letters
            foreach (var c in newChoices2)
            {
				int ixMatch = -1;
                foreach (char ch in newInput2)
                {
					int ix = c.Original.IndexOf(ch.ToString(), ixMatch + 1, StringComparison.InvariantCultureIgnoreCase);
                    ixMatch = ix;
                    if (ix == -1) break;
                }
                if (ixMatch > 0)
                {
                    return c.MapsTo;
                }

            }
            // off by one
            List<String> inputs = new List<string>();
            inputs.Add(newInput2);
            if (newInput2.Length > 3)
            {
                for(int i = 0; i < newInput2.Length; i++)
                {
                    String missing = newInput2.Remove(i) + newInput2.Substring(i + 1);
                    inputs.Add(missing);
                }
            }
            foreach (var c in newChoices2)
            {
                foreach (string test in inputs)
                {
                    int ixMatch = 0;
                    foreach (char ch in test)
                    {
                        int ix = c.Original.IndexOf(ch.ToString(), ixMatch, StringComparison.InvariantCultureIgnoreCase);
                        ixMatch = ix;
                        if (ix == -1) break;
                    }
                    if (ixMatch > 0)
                    {
                        return c.MapsTo;
                    }
                }

            }
            // GOAT / WOAT / WOLF
			newInput2 = newInput2.Replace("GOAT", "", StringComparison.InvariantCultureIgnoreCase).
				Replace("WOAT", "", StringComparison.InvariantCultureIgnoreCase).
				Replace("WOLF", "", StringComparison.InvariantCultureIgnoreCase).
				Replace("AIDS", "", StringComparison.InvariantCultureIgnoreCase);
            if (newInput2.Length <= 2)
            {
                return "";
            }
            foreach (var c in newChoices2)
            {
				int ixMatch = -1;
                foreach (char ch in newInput2)
                {
					int ix = c.Original.IndexOf(ch.ToString(), ixMatch + 1, StringComparison.InvariantCultureIgnoreCase);
                    ixMatch = ix;
                    if (ix == -1) break;
                }
                if (ixMatch > 0)
                {
                    return c.MapsTo;
                }

            }
            return "";
        }
		internal String ParseBoldedToVote(String bolded)
		{
			String vote = PrepBolded(bolded);
			if (vote == "")
			{
				return "";
			}
            List<Alias> aliases = new List<Alias>();
            foreach (var w in ValidVotes)
            {
                aliases.Add(new Alias(w, w));
            }
            String player = ParseInputToChoice(vote, aliases);
			if ((player == "") || (player == null))
			{
				player = ErrorVote;
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



		public string ForumURL 
		{ 
			get
			{
				return _forumURL;
			}
		}

		public IEnumerable<string> GetVoteLeaders(out Int32 cnt)
		{
			cnt = _leaderVotes;
			return _leaders;
		}
        public IEnumerable<Tuple<String, Int32>> GetPostCounts()
        {
            List<Tuple<String, Int32>> counts = new List<Tuple<string, int>>();
            foreach (var player in LivePlayers)
            {
                counts.Add(new Tuple<string, int>(player.Name, player.PostCount));
            }
            
            return counts;
        }
	}
    public class MajorityInfo
    {
        public MajorityInfo(Int32 postNumber, String lynch)
        {
            PostNumber = postNumber;
            Lynch = lynch;
        }
        public Int32 PostNumber { get; private set; }
        public String Lynch { get; private set; }
    }
}
