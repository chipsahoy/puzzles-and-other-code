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
using System.Data.SQLite; 

namespace POG.Werewolf
{
    public class VoteCount : INotifyPropertyChanged
    {
        #region fields
        POG.Forum.ThreadReader _thread;
        StringDictionary Mappings = new StringDictionary();
        private Dictionary<String, Voter> _lookupPoster = new Dictionary<string, Voter>();
        SortableBindingList<Voter> _livePosters = new SortableBindingList<Voter>();
        List<String> _validVotes;
        List<Voter> _allPosters = new List<Voter>();
        Action<Action> _synchronousInvoker;
        Int32 _startPost = 0;
        DateTime? _startTime = null;
        DateTime _endTime;
        Int32? _endPost;
        private Int32 _lastPost = 0;
        object _lock = new object();
        String _url = "http://forumserver.twoplustwo.com/59/puzzles-other-games/13-8-your-dreams-vanilla-game-thread-1233539/";
        String _dbName;
        Int32 _threadId;
        SQLiteConnection _dbWrite;
        SQLiteConnection _dbRead;
        Int32 _postsPerPage = 50;
        public readonly String ErrorVote = "Error!";
        public readonly String Unvote = "unvote";
        public readonly String NoLynch = "no lynch";
        #endregion
        #region constructors
        public VoteCount(Action<Action> synchronousInvoker, ThreadReader t, String url) 
        {
            _synchronousInvoker = synchronousInvoker;
            _url = url;
            _threadId = TwoPlusTwoForum.ThreadIdFromUrl(url);
            DateTime now = DateTime.Now;
            _endTime = new DateTime(now.Year, now.Month, now.Day, 18, 0, 0, now.Kind);
            _thread = t;
            _thread.PageCompleteEvent += new EventHandler<PageCompleteEventArgs>(_thread_PageCompleteEvent);
            _dbName = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\POG\\pogposts.sqlite";
            ConnectToDB();
            DoRefresh();
        }
        ~VoteCount()
        {
            DisconnectFromDB();
        }

        #endregion
        #region public methods
        public void Refresh()
        {
            Boolean checking = false;
            Int32 lastPage = 0;
            lock (_lock)
            {
                lastPage = PageFromNumber(_lastPost);
                if(!_pendingPages.Contains(lastPage))
                {
                    _pendingPages.Add(lastPage);
                    checking = true;
                }
            }
            if (checking)
            {
                Status = "Checking for new posts...";
                _thread.ReadPosts(_url, lastPage, lastPage);
            }
            else
            {
                Status = "Already looking for posts.";
            }
        }
        public void Clear()
        {
            _lookupPoster.Clear();
            _livePosters.Clear();
            _validVotes.Clear();
            _allPosters.Clear();

        }
        //public void AddPlayer(string name)
        //{

        //    _synchronousInvoker.Invoke(
        //        () =>
        //        {
        //            Voter p = LookupOrAddPoster(name);
        //            _livePosters.Add(p);
        //            BuildValidVotesList();
        //            RefreshVoteCount();
        //        }
        //    );
        //}
        //public void KillPlayer(string name)
        //{
        //    Voter p = this[name];
        //    if (p != null)
        //    {
        //        _synchronousInvoker.Invoke(
        //            () =>
        //            {
        //                _livePosters.Remove(p);
        //                _validVotes.Remove(p.Name);
        //                RefreshVoteCount();
        //            }

        //        );
        //    }
        //}
        //public void OnNewDay()
        //{
        //}
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
            DoRefresh();
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
                if (ts >= TimeSpan.FromSeconds(0))
                {
                    sb.AppendFormat("Night in {0}", ts.ToString("g"));
                }
                else
                {
                    sb.Append("It is night");
                }

                sb.AppendLine("[/b][/color]").AppendLine("---")
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
        #region public properties
        //public String URL
        //{
        //    get
        //    {
        //        String rc = "";
        //        if (_thread != null)
        //        {
        //        }
        //        return rc;
        //    }
        //}
        //[System.ComponentModel.Bindable(true)]
        //[System.ComponentModel.Bindable(true)]
        //public Int32 DayNumber
        //{
        //    get;
        //    private set;
        //}
        //[System.ComponentModel.Bindable(true)]
        //public Boolean IsDay
        //{
        //    get;
        //    set;
        //}
        //[System.ComponentModel.Bindable(true)]
        //public Boolean IsNight
        //{
        //    get;
        //    set;
        //}
        //[System.ComponentModel.Bindable(true)]
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
                if (value == _startPost)
                {
                    Console.WriteLine("VC: Start post unchanged.");
                    return;
                }
                _startPost = value;

                String sqlTime = @"SELECT time
                            FROM posts
                            WHERE posts.threadId = @p2 AND
                            (posts.number == @p4);";
                using (SQLiteCommand cmd = new SQLiteCommand(sqlTime, _dbRead))
                {
                    cmd.Parameters.Add(new SQLiteParameter("@p2", _threadId));
                    cmd.Parameters.Add(new SQLiteParameter("@p4", _startPost));
                    using (SQLiteDataReader r = cmd.ExecuteReader())
                    {
                        if (r.Read())
                        {
                            Console.WriteLine("VC: Found new start post " + value.ToString());
                            _startTime = r.GetDateTime(0);
                        }
                        else
                        {
                            Console.WriteLine("VC: Could not find start post " + value.ToString());
                            _startTime = null;
                        }
                    }
                }
                OnPropertyChanged("StartTime");
                OnPropertyChanged("StartPost");
                DoRefresh();
            }
        }
        public Int32? EndPost
        {
            get
            {
                Int32? endPost = null;
                // find nightPost
                // query for night post
                String sqlTime = @"SELECT number
                            FROM posts
                            WHERE posts.threadId = @p2 AND (posts.time > @p3)
                            ORDER BY id ASC LIMIT 1";
                using (SQLiteCommand cmd = new SQLiteCommand(sqlTime, _dbRead))
                {
                    cmd.Parameters.Add(new SQLiteParameter("@p2", _threadId));
                    SQLiteParameter pEndTime = new SQLiteParameter("@p3", System.Data.DbType.DateTime);
                    pEndTime.Value = _endTime;
                    cmd.Parameters.Add(pEndTime);
                    using (SQLiteDataReader r = cmd.ExecuteReader())
                    {
                        if (r.Read())
                        {
                            Int32 nightPost = r.GetInt32(0);
                            endPost = nightPost - 1;
                            Console.WriteLine("VC: first night post: " + nightPost.ToString());
                        }
                    }
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
                value = TruncateTime(value);
                if (value == _endTime)
                {
                    return;
                }
                _endTime = value;
                Int32? ep = EndPost; // side effects.
                OnPropertyChanged("EndTime");
                DoRefresh();
            }
        }
        public TimeSpan TimeUntilNight
        {
            get
            {
                DateTime now = DateTime.Now;
                now = now.AddMilliseconds(-500);
                TimeSpan rc = EndTime - now;
                rc = new TimeSpan(rc.Days, rc.Hours, rc.Minutes, rc.Seconds);
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
                return _lastPost;
            }
            private set
            {
                _lastPost = value;
                OnPropertyChanged("LastPost");
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
                Console.WriteLine("VC Status: " + value);
                OnPropertyChanged("Status");
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
        #region forum event handlers
        HashSet<Int32> _pendingPages = new HashSet<int>();
        Int32 _lastPage = 1;
        void _thread_PageCompleteEvent(object sender, PageCompleteEventArgs e)
        {
            AddPostsToDB(e.Posts);

            Int32 havePage = _lastPage;
            Int32 lastPage = havePage;
            lock (_lock)
            {
                _pendingPages.Remove(e.Page);
                Status = "Finished Page " + e.Page.ToString();
                if (e.TotalPages > _lastPage)
                {
                    _lastPage = e.TotalPages;
                }
                for(Int32 i = havePage + 1; i <= e.TotalPages; i++)
                {
                    _pendingPages.Add(i);
                }
            }
            if(e.TotalPages > havePage)
            {
                _thread.ReadPosts(_url, havePage + 1, e.TotalPages); // release lock before calling random code.
            }
            if (_pendingPages.Count == 0)
            {
                DoRefresh();
                Status = "All Posts Read!";
            }
        }
        #endregion
        #region private methods
        Int32 GetMaxPostDB()
        {
            String sql = @"SELECT number FROM posts WHERE threadId=@p1 ORDER BY id DESC LIMIT 1;";
            object o;
            using (SQLiteCommand cmd = new SQLiteCommand(sql, _dbRead))
            {
                cmd.Parameters.Add(new SQLiteParameter("@p1", _threadId));
                o = cmd.ExecuteScalar();
            }
            long rc;
            if ((o == null) || (o is System.DBNull))
            {
                rc = 0;
            }
            else
            {
                rc = (long)o;
            }
            return (Int32)rc;
        }
        private void DoRefresh()
        {
            Int32? maxPost = GetMaxPostDB();

            LastPost = maxPost.Value;
            String sqlPostCount = @"SELECT COUNT()
                            FROM posts
                            WHERE (posts.poster = @p1) AND (posts.threadId = @p2) AND
                            (posts.number >= @p4) AND (posts.time <= @p3);";
            using (SQLiteCommand cmdCount = new SQLiteCommand(sqlPostCount, _dbRead))
            {
                SQLiteParameter p1 = new SQLiteParameter("@p1");
                cmdCount.Parameters.Add(p1);
                cmdCount.Parameters.Add(new SQLiteParameter("@p2", _threadId));
                SQLiteParameter pEndTime = new SQLiteParameter("@p3", System.Data.DbType.DateTime);
                pEndTime.Value = _endTime;
                cmdCount.Parameters.Add(pEndTime);
                cmdCount.Parameters.Add(new SQLiteParameter("@p4", _startPost));
                foreach (Voter p in _livePosters)
                {
                    //var playerPosts = from post in qry where (String.Equals(post.Poster, p.Name, StringComparison.InvariantCultureIgnoreCase)) select post;
                    //p.SetPosts(playerPosts);
                    p1.Value = p.Name;
                    object o = cmdCount.ExecuteScalar();
                    if (!(o is System.DBNull))
                    {
                        p.PostCount = (Int32)(long)o;
                    }
                    else
                    {
                        p.PostCount = 0;
                    }
                }
               
            }
            String sql = @"SELECT bolds.bolded, posts.number, posts.time, posts.id, bolds.position
                            FROM bolds INNER JOIN posts ON (bolds.postId = posts.id)
                            WHERE (posts.poster = @p1) AND (posts.threadId = @p2) AND
                            (posts.number >= @p4) AND (posts.time <= @p3) AND
                            (bolds.ignore = 0)
                            ORDER BY bolds.postId DESC, bolds.position DESC LIMIT 1";
            using (SQLiteCommand cmd = new SQLiteCommand(sql, _dbRead))
            {
                SQLiteParameter p1 = new SQLiteParameter("@p1");
                cmd.Parameters.Add(p1);
                cmd.Parameters.Add(new SQLiteParameter("@p2", _threadId));
                SQLiteParameter pEndTime = new SQLiteParameter("@p3", System.Data.DbType.DateTime);
                pEndTime.Value = _endTime;
                cmd.Parameters.Add(pEndTime);
                cmd.Parameters.Add(new SQLiteParameter("@p4", _startPost));
                foreach (Voter p in _livePosters)
                {
                    //var playerPosts = from post in qry where (String.Equals(post.Poster, p.Name, StringComparison.InvariantCultureIgnoreCase)) select post;
                    //p.SetPosts(playerPosts);
                    p1.Value = p.Name;
                    using(SQLiteDataReader r = cmd.ExecuteReader())
                    {
                        if (r.Read())
                        {
                        
                            String bolded = r.GetString(0);
                            Int32 postNumber = r.GetInt32(1);
                            DateTime postTime = r.GetDateTime(2);
                            Int32 postId = r.GetInt32(3);
                            Int32 boldPosition = r.GetInt32(4);
                            p.SetVote(bolded, postNumber, postTime, postId, boldPosition);
                        }
                        else
                        {
                            p.ClearVote();
                        }
                    }
                }
            }
            BuildValidVotesList();
            RefreshVoteCount();
        }
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
                if (Mappings.ContainsKey(vote))
                {
                    player =
                        ValidVotes.FirstOrDefault(
                            p => p == Mappings[vote]);
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
            DateTime rc = new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, 0, dt.Kind);
            return rc;
        }
        #region SQLite
        void ConnectToDB()
        {
            if(!File.Exists(_dbName))
            {
                SQLiteConnection.CreateFile(_dbName);
            }
            String connect = String.Format("Data Source={0};Version=3;", _dbName);
            _dbWrite = new SQLiteConnection(connect);
            _dbWrite.Open();
            _dbRead = new SQLiteConnection(connect);
            _dbRead.Open();
            using (SQLiteTransaction trans = _dbWrite.BeginTransaction())
            {
                String[] tables = 
                {
                    @"CREATE TABLE IF NOT EXISTS [posts] (
                        [id] INTEGER NOT NULL PRIMARY KEY,
                        [threadId] INTEGER,
                        [poster] TEXT COLLATE NOCASE,
                        [number] INTEGER,
                        [content] TEXT,
                        [title] TEXT,
                        [time] TIMESTAMP
                    );",
                    @"CREATE TABLE IF NOT EXISTS [threads] (
                        [id] INTEGER NOT NULL PRIMARY KEY,
                        [url] TEXT,
                        [title] TEXT
                    );",
                    @"CREATE TABLE IF NOT EXISTS [bolds] (
                        [postId] INTEGER NOT NULL,
                        [position] INTEGER,
                        [bolded] TEXT,
                        [ignore] INTEGER,
                        PRIMARY KEY(postId, position)
                    );",
                };
                foreach (String sql in tables)
                {
                    SQLiteCommand cmd = new SQLiteCommand(sql, _dbWrite);
                    int e = cmd.ExecuteNonQuery();
                    cmd.Dispose();
                }

                trans.Commit();
            }
        }
        void DisconnectFromDB()
        {
            //_dbConnection.Close();
        }
        void AddPostsToDB(Posts posts)
        {
            lock (_dbWrite)
            {
                using (SQLiteTransaction trans = _dbWrite.BeginTransaction())
                {
                    String sql =

                        @"INSERT OR REPLACE INTO [posts] (
                        id,
                        threadId,
                        poster,
                        number,
                        content,
                        title,
                        time)
                        VALUES (@p1, @p2, @p3, @p4, @p5, @p6, @p7);";

                    using(SQLiteCommand cmd = new SQLiteCommand(sql, _dbWrite, trans))
                    {
                        SQLiteParameter pPostId = new SQLiteParameter("@p1");
                        SQLiteParameter pThreadId = new SQLiteParameter("@p2");
                        SQLiteParameter pPoster = new SQLiteParameter("@p3");
                        SQLiteParameter pPostNumber = new SQLiteParameter("@p4");
                        SQLiteParameter pContent = new SQLiteParameter("@p5");
                        SQLiteParameter pTitle = new SQLiteParameter("@p6");
                        SQLiteParameter pTime = new SQLiteParameter("@p7", System.Data.DbType.DateTime);
                        cmd.Parameters.Add(pPostId);
                        cmd.Parameters.Add(pThreadId);
                        cmd.Parameters.Add(pPoster);
                        cmd.Parameters.Add(pPostNumber);
                        cmd.Parameters.Add(pContent);
                        cmd.Parameters.Add(pTitle);
                        cmd.Parameters.Add(pTime);

                        foreach (Post p in posts)
                        {
                            pPostId.Value = p.PostId;
                            pThreadId.Value = p.ThreadId;
                            pPoster.Value = p.Poster;
                            pPostNumber.Value = p.PostNumber;
                            pContent.Value = p.Content;
                            pTitle.Value = p.Title;
                            pTime.Value = p.Time;
                            int e = cmd.ExecuteNonQuery();

                            int ix = 0;
                            String sqlBold =
                                @"INSERT OR IGNORE INTO [bolds] (
                                    postId,
                                    position,
                                    bolded,
                                    ignore)
                                    VALUES (@p1, @p2, @p3, @p4);";
                            using (SQLiteCommand cmdBold = new SQLiteCommand(sqlBold, _dbWrite, trans))
                            {
                                SQLiteParameter pForeignPostId = new SQLiteParameter("@p1");
                                SQLiteParameter pIx = new SQLiteParameter("@p2");
                                SQLiteParameter pBolded = new SQLiteParameter("@p3");
                                SQLiteParameter pIgnore = new SQLiteParameter("@p4");
                                cmdBold.Parameters.Add(pForeignPostId);
                                cmdBold.Parameters.Add(pIx);
                                cmdBold.Parameters.Add(pBolded);
                                cmdBold.Parameters.Add(pIgnore);

                                foreach (Bold b in p.Bolded)
                                {
                                    pForeignPostId.Value = p.PostId;
                                    pIx.Value = ix;
                                    pBolded.Value = b.Content;
                                    pIgnore.Value = b.Ignore;
                                    e = cmdBold.ExecuteNonQuery();
                                    ix++;
                                }
                            }
                        }
                    }

 
                    trans.Commit();
                }
            }
        }
        #endregion
        #endregion

        internal void UnhideVote(Voter voter, int postNumber, int boldPosition)
        {
            throw new NotImplementedException();
        }

        internal void HideVote(Voter voter, int postNumber, int boldPosition)
        {
            throw new NotImplementedException();
        }
    }
}
