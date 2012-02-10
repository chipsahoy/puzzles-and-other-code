using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Globalization;
using System.ComponentModel;
using POG.Utils;
using POG.Forum;
using System.Collections.Specialized;
using System.Configuration;

namespace POG.Werewolf
{
    public class VoteCount : INotifyPropertyChanged
    {
        #region fields
        StringDictionary Mappings = new StringDictionary();
        private Dictionary<String, Voter> _lookupPoster = new Dictionary<string, Voter>();
        private Posts _allPosts = new Posts();
        SortableBindingList<Voter> _livePosters = new SortableBindingList<Voter>();
        List<String> _validVotes;
        List<Voter> _allPosters = new List<Voter>();
        Action<Action> _synchronousInvoker;
        VBulletin_3_8_7 _forum;
        Int32 _startPost = 1;
        DateTime? _startTime = null;
        DateTime _endTime = DateTime.Now;
        Int32? _endPost;
        private Int32 m_lastPost;
        public readonly String SelectVote = "Error!";
        public readonly String Unvote = "unvote";
        public readonly String NoLynch = "no lynch";
        #endregion
        #region constructors
        public VoteCount(Action<Action> synchronousInvoker) 
        {
            _synchronousInvoker = synchronousInvoker;
            _forum = new VBulletin_3_8_7(synchronousInvoker);
            _forum.NewPostsAvailable += new EventHandler<NewPostsAvailableEventArgs>(_forum_NewPostsAvailable);
            _forum.PropertyChanged += new PropertyChangedEventHandler(_forum_PropertyChanged);
            _forum.StatusUpdate += new EventHandler<NewStatusEventArgs>(_forum_StatusUpdate);
            _forum.LoginEvent += new EventHandler<LoginEventArgs>(_forum_LoginEvent);
            Refresh();
        }
        #endregion


        #region forum event handlers
        void _forum_LoginEvent(object sender, LoginEventArgs e)
        {
            OnLoginEvent(e);
        }
        void _forum_StatusUpdate(object sender, NewStatusEventArgs e)
        {
            Status = e.Status;
            OnPropertyChanged("Status");
        }

        void _forum_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
        }

        void _forum_NewPostsAvailable(object sender, NewPostsAvailableEventArgs e)
        {
            Post[] posts = _forum.GetPosts(m_lastPost + 1, e.NewestPostNumber);
            foreach (Post post in posts)
            {
                _allPosts.Add(post);
            }
            LastPost = e.NewestPostNumber;
            Refresh();
            OnPropertyChanged("LastPost");
        }
        private void Refresh()
        {
            var qry = from post in _allPosts where (post.PostNumber >= _startPost) && (post.Time <= _endTime) select (post);
            foreach (Voter p in _livePosters)
            {
                var playerPosts = from post in qry where (String.Equals(post.Poster, p.Name, StringComparison.InvariantCultureIgnoreCase)) select post;                
                p.SetPosts(playerPosts);
            }
            BuildValidVotesList();
            RefreshVoteCount();
        }
        #endregion
        #region public properties
        public String URL
        {
            get
            {
                String rc = "";
                if (_forum != null)
                {
                    rc = _forum.ThreadURL;
                }
                return rc;
            }
            set
            {
                _forum.ThreadURL = value;
            }
        }
        [System.ComponentModel.Bindable(true)]
        public Boolean Turbo
        {
            get;
            set;
        }
        [System.ComponentModel.Bindable(true)]
        public Int32 DayNumber
        {
            get;
            private set;
        }
        [System.ComponentModel.Bindable(true)]
        public Boolean IsDay
        {
            get;
            set;
        }
        [System.ComponentModel.Bindable(true)]
        public Boolean IsNight
        {
            get;
            set;
        }
        [System.ComponentModel.Bindable(true)]
        public Voter this[string name]
        {
            get
            {
                foreach (Voter p in _livePosters)
                {
                    if (p.Name == name)
                    {
                        return p;
                    }
                }
                return null;
            }

        }
        public SortableBindingList<Voter> LivePlayers
        {
            get
            {
                return _livePosters;
            }
        }
        public Int32 StartPost
        {
            get
            {
                return _startPost;
            }
            set
            {
                if (value > LastPost)
                {
                    value = LastPost;
                }
                if (value == _startPost)
                {
                    return;
                }
                _startPost = value;
                var firstPost = (from p in _allPosts where (p.PostNumber == value) select p).FirstOrDefault();
                if (firstPost != null)
                {
                    _startTime = firstPost.Time;
                }
                else
                {
                    _startTime = null;
                }
                OnPropertyChanged("StartTime");
                OnPropertyChanged("StartPost");
                Refresh();
            }
        }
        public Int32? EndPost
        {
            get
            {
                Int32? endPost = null;
                var nightPost = (from p in _allPosts where (p.Time > EndTime) select p).FirstOrDefault();
                if (nightPost != null)
                {
                    endPost = nightPost.PostNumber - 1;
                }
                if (endPost != _endPost)
                {
                    _endPost = endPost;
                    OnPropertyChanged("EndPost");
                }
                return _endPost;
            }
        }
        public DateTime? StartTime
        {
            get
            {
                return _startTime;
            }
        }
        public DateTime EndTime
        {
            get
            {
                return _endTime;
            }
            set
            {
                if (value == null)
                {
                    value = DateTime.MaxValue;
                }
                if (value == _endTime)
                {
                    return;
                }
                _endTime = value;
                Int32? ep = EndPost; // side effects.
                OnPropertyChanged("EndTime");
                Refresh();
            }
        }
        public TimeSpan TimeUntilNight
        {
            get
            {
                TimeSpan rc = EndTime - DateTime.Now;
                return rc;
            }
        }
        public IEnumerable<String> ValidVotes
        {
            get
            {
                if (_validVotes == null)
                {
                    BuildValidVotesList();
                }
                return _validVotes;
            }
        }
        [System.ComponentModel.Bindable(true)]
        public Int32 LastPost
        {
            get
            {
                return m_lastPost;
            }
            private set
            {
                m_lastPost = value;
                OnPropertyChanged("LastPost");
            }
        }
        public string Status { get; private set; }
        #endregion


        #region private methods
        private void BuildValidVotesList()
        {
            List<String> validVotes = new List<string>();
            foreach (Voter p in _livePosters)
            {
                validVotes.Add(p.Name);
            }
            validVotes.Sort();
            validVotes.Add(Unvote);
            validVotes.Add(NoLynch);
            _validVotes = validVotes;
            OnPropertyChanged("LivePlayers");
        }
        private void RefreshVoteCount()
        {
            String s = PostableVoteCount; // do it for side effects.
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

        private Voter LookupOrAddPoster(string name)
        {
            Voter p;
            if (_lookupPoster.ContainsKey(name.ToLower()))
            {
                p = _lookupPoster[name.ToLower()];
            }
            else
            {
                p = new Voter(name, this, _synchronousInvoker);
                _lookupPoster.Add(name.ToLower(), p);
            }
            return p;
        }
        #endregion

        #region public methods
        public void Clear()
        {
            _lookupPoster.Clear();
            _allPosts.Clear();
            _livePosters.Clear();
            _validVotes.Clear();
            _allPosters.Clear();
            _forum.Clear();

        }
        public void Login(string user, string password)
        {
            _forum.Login(user, password);
        }
        public void Logout()
        {
            _forum.Logout();
        }
        public void AddPlayer(string name)
        {

            _synchronousInvoker.Invoke(
                () =>
                {
                    Voter p = LookupOrAddPoster(name);
                    _livePosters.Add(p);
                    BuildValidVotesList();
                    RefreshVoteCount();
                }
            );
        }
        public void KillPlayer(string name)
        {
            Voter p = this[name];
            if (p != null)
            {
                _synchronousInvoker.Invoke(
                    () =>
                    {
                        _livePosters.Remove(p);
                        _validVotes.Remove(p.Name);
                        RefreshVoteCount();
                    }

                );
            }
        }
        public void OnNewDay()
        {
        }
        public void SetPlayerList(string players)
        {
            List<String> rawList = players.Split(
                new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(p => p.Trim())
                .Distinct().ToList();
            SortableBindingList<Voter> livePlayers = new SortableBindingList<Voter>();
            foreach (String name in rawList)
            {
                Voter p = LookupOrAddPoster(name);
                livePlayers.Add(p);
            }
            _livePosters = livePlayers;
            Refresh();
            OnPropertyChanged("LivePlayers");
        }
        public void AddVoteAlias(string bolded, string votee)
        {
            String vote = PrepBolded(bolded);
            Mappings[vote] = votee;
            RefreshVoteCount();
        }
        public string PostableVoteCount
        {
            get
            {
                var sb = new StringBuilder(@"[highlight]Votes from post ");
                sb
                    .Append(StartPost.ToString())
                    .Append(" to post ")
                    .Append(EndPost.ToString())
                    .AppendLine();

                TimeSpan ts = TimeUntilNight;
                if (ts >= TimeSpan.FromSeconds(0))
                {
                    sb.AppendFormat("Night in {0}", ts);
                }
                else
                {
                    sb.Append("It is night");
                }

                sb.AppendLine("[/highlight]").AppendLine("---")
                .AppendLine("[table=head][b]Votes[/b]|[b]Lynch[/b]|[b]Voters[/b]");

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
                foreach (Voter p in _livePosters)
                {
                    wagons.Add(p.Name, new List<Voter>());
                }
                // find out who they are voting, add vote to that wagon.
                foreach (Voter p in _livePosters)
                {
                    String votee = p.Votee;
                    if (votee == SelectVote)
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
                    Voter v = _lookupPoster[wagon.Key.ToLower()];
                    v.VoteCount = wagon.Value.Count;
                }
                var sortedWagons = (from wagon in wagons where (wagon.Value.Count > 0) orderby wagon.Value.Count descending select wagon).ToDictionary(pair => pair.Key, pair => pair.Value);
                // build string with wagons followed by optional {unvote, no lynch, not voting}
                foreach (var wagon in sortedWagons)
                {
                    sb
                        .AppendFormat("{0} | [b]{1}[/b] | {2}", wagon.Value.Count, wagon.Key,
                                      VoteLinks(wagon.Value, true))
                        .AppendLine();
                }
                if (listNoLynch.Count > 0)
                {
                    sb
                        .AppendFormat("{0} | {1} | {2}", listNoLynch.Count, NoLynch,
                                      VoteLinks(listNoLynch, true))
                        .AppendLine();
                }
                if (listUnvote.Count > 0)
                {
                    sb
                        .AppendFormat("{0} | {1} | {2}", listUnvote.Count, Unvote,
                                      VoteLinks(listUnvote, true))
                        .AppendLine();
                }
                if (listNotVoting.Count > 0)
                {
                    sb
                        .AppendFormat("{0} | {1} | {2}", listNotVoting.Count, sNotVoting,
                                      VoteLinks(listNotVoting, false))
                        .AppendLine();
                }
                if (listError.Count > 0)
                {
                    sb
                        .AppendFormat("{0} | [color=red][b]{1}[/b][/color] | {2}", listError.Count, sError,
                                      VoteLinks(listError, true))
                        .AppendLine();
                }
                sb.AppendLine("[/table]");
                return sb.ToString();
            }
        }
        #endregion
        #region internal methods
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
                if (Mappings.ContainsKey(vote))
                {
                    player =
                        ValidVotes.FirstOrDefault(
                            p => p == Mappings[vote]);
                }
                if (player == null)
                {
                    player = SelectVote;
                }
            }
            return player;
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
        public event EventHandler<Forum.LoginEventArgs> LoginEvent;
        private void OnLoginEvent(Forum.LoginEventArgs e)
        {
            var handler = LoginEvent;
            if (handler != null)
            {
                _synchronousInvoker.Invoke(
                    () => handler(this, e)
                );
            }
        }
        #endregion

    }
}
