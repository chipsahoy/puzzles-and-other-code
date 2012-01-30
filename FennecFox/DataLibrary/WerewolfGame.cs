using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Globalization;
using System.ComponentModel;
using System.Windows.Forms;


namespace FennecFox.DataLibrary
{
    class WerewolfGame : INotifyPropertyChanged
    {
        private Dictionary<String, Poster> _lookupPoster = new Dictionary<string, Poster>();
        private Posts _allPosts = new Posts();
        SortableBindingList<Poster> _livePosters = new SortableBindingList<Poster>();
        List<String> _validVotes;
        List<Poster> _allPosters = new List<Poster>();
        Action<Action> _synchronousInvoker;

        public WerewolfGame(Action<Action> synchronousInvoker)
        {
            _synchronousInvoker = synchronousInvoker;
            BuildValidVotesList();
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
        public Poster this[string name]
        {
            get
            {
                foreach (Poster p in _livePosters)
                {
                    if (p.Name == name)
                    {
                        return p;
                    }
                }
                return null;
            }

        }
        public SortableBindingList<Poster> LivePlayers
        {
            get
            {
                return _livePosters;
            }
        }
        Int32 _startPost = 1;
        DateTime? _startTime = null;
        DateTime _endTime = DateTime.Now;

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
                if(value == _startPost)
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
                UpdateDayFilter();                
            }
        }
        Int32? _endPost;
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
                UpdateDayFilter();
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
        private void UpdateDayFilter()
        {
            foreach (Poster p in LivePlayers)
            {
                p.UpdateDayFilter();
            }
        }
        public readonly String SelectVote = "Error!";
        public readonly String NotAVote = "NOT A VOTE (HIDE THIS)";
        public readonly String Unvote = "unvote";
        public readonly String NoLynch = "no lynch";
        private void BuildValidVotesList()
        {
            List<String> validVotes = new List<string>();
            foreach (Poster p in _livePosters)
            {
                validVotes.Add(p.Name);
            }
            validVotes.Sort();
            validVotes.Add(Unvote);
            validVotes.Add(NoLynch);
            validVotes.Add(NotAVote);
            _validVotes = validVotes;
            OnPropertyChanged("LivePlayers");
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

        private Int32 m_lastPost;
        [System.ComponentModel.Bindable(true)]
        public Int32 LastPost
        {
            get
            {
                return m_lastPost;
            }
            set
            {
                m_lastPost = value;
                OnPropertyChanged("LastPost");
            }
        }
        private void RefreshVoteCount()
        {
            String s = PostableVoteCount; // do it for side effects.
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

                Dictionary<String, List<Poster>> wagons = new Dictionary<string, List<Poster>>();
                List<Poster> listError = new List<Poster>();
                List<Poster> listNoLynch = new List<Poster>();
                List<Poster> listUnvote = new List<Poster>();
                List<Poster> listNotVoting = new List<Poster>();
                string sError = "Error";
                string sNotVoting = "not voting";
                wagons.Add(sError, listError);
                wagons.Add(NoLynch, listNoLynch);
                wagons.Add(Unvote, listUnvote);
                wagons.Add(sNotVoting, listNotVoting);
                // for each live player
                foreach (Poster p in _livePosters)
                {
                    wagons.Add(p.Name, new List<Poster>());
                }
                // find out who they are voting, add vote to that wagon.
                foreach (Poster p in _livePosters)
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
                    this[wagon.Key].VoteCount = wagon.Value.Count;
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
        private String VoteLinks(List<Poster> wagon, Boolean linkToVote)
        {
            StringBuilder sb = new StringBuilder();
            foreach (Poster voter in wagon)
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

        private Poster LookupOrAddPoster(string name)
        {
            Poster p;
            if (_lookupPoster.ContainsKey(name.ToLower()))
            {
                p = _lookupPoster[name.ToLower()];
            }
            else
            {
                p = new Poster(name, this, _synchronousInvoker);
                _lookupPoster.Add(name.ToLower(), p);
            }
            return p;
        }
        public void AddPlayer(string name)
        {

            _synchronousInvoker.Invoke(
                () =>
                {
                    Poster p = LookupOrAddPoster(name);
                    _livePosters.Add(p);
                    BuildValidVotesList();
                    RefreshVoteCount();
                }
            );
        }
        public void KillPlayer(string name)
        {
            Poster p = this[name];
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
        public Boolean OnNewPost(HtmlAgilityPack.HtmlNode post)
        {
            string posterName = "";
            Int32 postNumber = 0;
            String postLink = null;
            DateTime postTime = DateTime.Now;
            HtmlAgilityPack.HtmlNode postNumberNode = post.SelectSingleNode("../../../tr[1]/td[2]/a");

            if (postNumberNode != null)
            {
                postNumber = Int32.Parse(postNumberNode.InnerText);
                postLink = postNumberNode.Attributes["href"].Value;
                Int32 lastPost = Convert.ToInt32(postNumber);
                if (lastPost < LastPost)
                {
                    return false;
                }
                LastPost = lastPost;
            }
            RemoveComments(post);
            HtmlAgilityPack.HtmlNode postTimeNode = post.SelectSingleNode("../../../tr[1]/td[1]");
            if (postTimeNode != null)
            {
                string time = postTimeNode.InnerText.Trim();
                DateTime dtNow = DateTime.Now;
                string today = dtNow.ToString("MM-dd-yyyy");
                time = time.Replace("Today", today);
                DateTime dtYesterday = dtNow - new TimeSpan(1, 0, 0, 0);
                string yesterday = dtYesterday.ToString("MM-dd-yyyy");
                time = time.Replace("Yesterday", yesterday);
                var culture = Thread.CurrentThread.CurrentCulture;
                try
                {
                    Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("en-US");
                    postTime = DateTime.ParseExact(time, "MM-dd-yyyy, hh:mm tt", null);
                }
                finally
                {
                    Thread.CurrentThread.CurrentCulture = culture;
                }
            }

            HtmlAgilityPack.HtmlNode userNode = post.SelectSingleNode("../../td[1]/div/a[@class='bigusername']");
            if (userNode != null)
            {
                posterName = HtmlAgilityPack.HtmlEntity.DeEntitize(userNode.InnerText);
            }
            Poster poster = LookupOrAddPoster(posterName);
            // if the poster's name exists as a different case, replace the player.
            if (poster.Name != posterName)
            {
                poster.Name = posterName;
                BuildValidVotesList();
            }
            Post p = new Post(posterName, postNumber, postTime, postLink, post);
            _allPosts.Add(p);
            poster.AddPost(p);
            RefreshVoteCount();
            return true;
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
            SortableBindingList<Poster> livePlayers = new SortableBindingList<Poster>();
            foreach (String name in rawList)
            {
                Poster p = LookupOrAddPoster(name);
                livePlayers.Add(p);
            }
            _livePosters = livePlayers;
            BuildValidVotesList();
            OnPropertyChanged("LivePlayers");
            RefreshVoteCount();
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
        public String ParseBoldedToVote(String bolded)
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
                if (Properties.Settings.Default.Mappings.ContainsKey(vote))
                {
                    player =
                        ValidVotes.FirstOrDefault(
                            p => p == Properties.Settings.Default.Mappings[vote]);
                }
                if (player == null)
                {
                    player = SelectVote;
                }
            }
            return player;
        }
        public void AddVoteAlias(string bolded, string votee)
        {
            String vote = PrepBolded(bolded);
            Properties.Settings.Default.Mappings[vote] = votee;
            RefreshVoteCount();
        }

        static void RemoveComments(HtmlAgilityPack.HtmlNode node)
        {
            foreach (var n in node.SelectNodes("//comment()") ?? new HtmlAgilityPack.HtmlNodeCollection(node))
            {
                n.Remove();
            }
        }

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
    }
}
