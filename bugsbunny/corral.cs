using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using POG.Forum;
using System.Text.RegularExpressions;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Diagnostics;

namespace POG.Automation
{
    [ComVisible(true)]
    [Guid("60013706-2826-483D-A581-2EEE4DEEC022")]
    public enum CorralAction
    {
        Toss = 0,
        Arm,
        Shoot,
        Trade,
        Steal,
        UnArm,
        NoAction,
    }

    [ComVisible(true)]
    [Guid("45477E8A-1DDD-4A92-A62A-908CA7377DA3")]
    public class CorralEvents : IEnumerable
    {
        List<CorralEvent> _events;
        [DispId(-4)]
        public IEnumerator GetEnumerator()
        {
            return _events.GetEnumerator();
        }
        public CorralEvents()
        {
        }
        internal CorralEvents(IEnumerable<CorralEvent> events)
        {
            _events = events.ToList();
        }
    }
    [ComVisible(true)]
    [Guid("FE9D8F2F-074A-4E94-9663-04BD0D892E28")]
    [ClassInterface(ClassInterfaceType.None)]
    public class CorralEvent : POG.Automation.ICorralEvent
    {
        public string Actor
        {
            get;
            private set;
        }
        public CorralAction Action
        {
            get;
            private set;
        }
        public string Target
        {
            get;
            private set;
        }
        public string Parameter
        {
            get;
            private set;
        }
        public String TimeString
        {
            get
            {
                return TimeStamp.ToUniversalTime().ToString();
            }
        }
        public string Title
        {
            get;
            private set;
        }
        public string Content
        {
            get;
            private set;
        }
        public Int32 Id
        {
            get;
            private set;
        }
        public Int32 PostNumber
        {
            get;
            private set;
        }
        public DateTimeOffset TimeStamp
        {
            get;
            private set;
        }
        internal CorralEvent(string actor, CorralAction action, string target, string parameter, DateTimeOffset time, string title,
            string content, Int32 id, Int32 postNumber)
        {
            Actor = actor;
            Action = action;
            Target = target;
            Parameter = parameter;
            TimeStamp = time;
            Title = title;
            Content = content;
            Id = id;
            PostNumber = postNumber;
        }
        public CorralEvent()
        {
        }
    }

    [ComVisible(true)]
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    [Guid("29218543-7AA7-4215-87F5-971F82233BD2")]
    public interface corralevents
    {
        void OnLoginResult([In] string username, [In] Boolean success);
        void OnPlayerActions([In] CorralEvents actions);
    }

    [ComVisible(true)]
    [Guid("041E4959-B1B0-46E1-BAFB-C7335E2B1C81")]
    [ProgId("POG.Corral")]
    [ComSourceInterfaces(typeof(corralevents))]
    [ClassInterface(ClassInterfaceType.None)]
    public class PogCorral : POG.Automation.IPogCorral
    {
        #region fields
        String _url;
        private VBulletinForum _forum;
        private Action<Action> _synchronousInvoker;
        System.Timers.Timer _timer = new System.Timers.Timer();
        ThreadReader _thread;
        Int32 _threadId;
        Control _invokeControl;
        DateTimeOffset _minTime;
        DateTimeOffset _maxTime;
        Int32 _pageStart = 1;
        List<Post> _posts = new List<Post>();
        Boolean _readingPosts = false;
        Int32 _lastPostProcessed = 0;
        Int32 _lastPMProcessed = 0;
        #endregion
        #region events
        [ComVisible(false)]
        public delegate void LoginResultDelegate(string username, Boolean success);
        [ComVisible(false)]
        public delegate void PlayerActionsDelegate(CorralEvents actions);
        public event LoginResultDelegate OnLoginResult = delegate { };
        public event PlayerActionsDelegate OnPlayerActions = delegate { };
        #endregion
        public PogCorral()
        {
            _timer.AutoReset = false;
            _timer.Elapsed += new System.Timers.ElapsedEventHandler(_timer_Elapsed);
        }
        #region public methods
        public void Login(string url, string username, string password)
        {
            _invokeControl = new Control();
            IntPtr hwnd = _invokeControl.Handle;
            _synchronousInvoker = (x) => _invokeControl.Invoke(x);
            _url = POG.Utils.Misc.NormalizeUrl(url);
            _threadId = POG.Utils.Misc.TidFromURL(url);
            _forum = new VBulletinForum(_synchronousInvoker, "forumserver.twoplustwo.com", "3.8.7", "59/puzzles-other-games/");
            _forum.LoginEvent+=new EventHandler<LoginEventArgs>(_forum_LoginEvent);
            _forum.Login(username, password);
        }
        public void Stop()
        {
        }
        public Int32 LastPostNumber
        {
            get;
            set;
        }
        public Int32 LastPMId
        {
            get;
            set;
        }
        public void MakePost(string title, string content)
        {
            _forum.MakePost(_threadId, title, content, 0, false);
        }
        public void SendPMToGroup(string[] to, string title, string content)
        {
        }
        public void SendPM(string to, string title, string content)
        {
            PrivateMessage pm = new PrivateMessage(new String[] { to }, null, title, content);
            _forum.SendPM(pm);
        }
        public void StartPolling(int startPost, object startTime, object stopTime)
        {
            startPost = Math.Max(startPost, 1);
            _pageStart = startPost / _forum.PostsPerPage;
            _lastPostProcessed = startPost - 1;
            _lastPMProcessed = 0;
            DateTime? timeStart = startTime as DateTime?;
            DateTime? timeStop = stopTime as DateTime?;
            if (timeStart != null)
            {
                _minTime = timeStart.Value;
            }
            else
            {
                _minTime = DateTime.MinValue;
            }
            if (timeStop != null)
            {
                _maxTime = timeStop.Value;
            }
            else
            {
                _maxTime = DateTime.MaxValue;
            }

            _timer.Interval = GetTimerInterval();
            _timer.Start();

        }
        #endregion
        //
        Queue<CorralEvent> _pendingActions = new Queue<CorralEvent>();
        private double GetTimerInterval()
        {
            DateTime now = DateTime.Now;
            double rc = ((60 - now.Second) * 1000) - now.Millisecond;
            return rc;
        }
        void PollPostsPMs()
        {
            List<PrivateMessage> pms = new List<PrivateMessage>();
            _posts.Clear();
            _readingPosts = true;
            _thread.ReadPages(_url, _pageStart, Int32.MaxValue, null);
            _forum.CheckPMs(0, 1, null, (folderpage, errMessage, cookie) =>
            {
                for (int i = 0; i < folderpage.MessagesThisPage; i++)
                {
                    PMHeader header = folderpage[i];
                    if (header.Unread && (_minTime <= header.Timestamp) && (_maxTime >= header.Timestamp))
                    {
                        _forum.ReadPM(header.Id, null, (id, pm, cake) =>
                        {
                            pms.Add(pm);
                        });
                    }
                    
                }
            }
            );
            while (_readingPosts)
            {
                System.Threading.Thread.Sleep(50);
            }
            String msg = String.Empty;
            List<CorralEvent> actions = new List<CorralEvent>();
            Int32 lastPost = 0;
            foreach (var post in _posts)
            {
                if (post.PostNumber <= _lastPostProcessed)
                {
                    continue;
                }
                if ((post.Time < _minTime) || (post.Time > _maxTime))
                {
                    continue;
                }
                msg += String.Format("{0} {1}:\r{2}", post.Time, post.Poster.Name, post.Content);
                lastPost = Math.Max(lastPost, post.PostNumber);
                CorralEvent action = PostToAction(post);
                actions.Add(action);
            }
            if (lastPost > _lastPostProcessed)
            {
                _lastPostProcessed = lastPost;
            }
            Int32 lastPMId = 0;
            foreach (var pm in pms)
            {
                if (pm.Id <= _lastPMProcessed)
                {
                    continue;
                }
                msg += String.Format("{0} {1}: {2}\r{3}", pm.TimeStamp, pm.From, pm.Title, pm.Content);
                lastPMId = Math.Max(lastPMId, pm.Id);
                CorralEvent action = PMToAction(pm);
                actions.Add(action);
            }
            if (lastPMId > _lastPMProcessed)
            {
                _lastPMProcessed = lastPMId;
            }
            actions.Sort((x, y) => 
                {
                    int rc = x.TimeStamp.CompareTo(y.TimeStamp);
                    if (rc == 0)
                    {
                        // same timestamp.
                        rc = x.Action.CompareTo(y.Action);
                        if (rc == 0)
                        {
                            rc = x.Id.CompareTo(y.Id);
                        }
                    }
                    return rc;
                });
            CorralEvents oActions = new CorralEvents(actions);
            try
            {
                OnPlayerActions(oActions);
            }
            catch(Exception)
            {
                // don't sweat it. Writing clients is hard.
            }
        }
        void _timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Trace.TraceInformation("Begin timer event at {0}", DateTime.Now);
            PollPostsPMs();
            _timer.Interval = GetTimerInterval();
            _timer.Start();
            Trace.TraceInformation("End timer event at {0}", DateTime.Now);
        }
        string TrimLead(string msg, string lead)
        {
            String trimmed = msg.TrimStart();
            Match m = Regex.Match(msg, lead.ToLower() + @"\s");
            if (m.Success)
            {
                msg = trimmed.Substring(lead.Length).Trim();
            }
            return msg;
        }
        String ParseFirstWord(ref string msg)
        {
            String word = String.Empty;
            String remainder = String.Empty;
            String trimmed = msg.TrimStart();
            Match m = Regex.Match(trimmed, @"(\w+)");
            if (m.Success)
            {
                word = m.Groups[1].Value;
                msg = trimmed.Substring(word.Length).TrimStart();
            }
            else
            {
                msg = String.Empty;
            }
            return word;
        }

        CorralEvent PMToAction(PrivateMessage pm)
        {
            String actor = pm.From;
            CorralAction verb = CorralAction.NoAction;
            String target = "no target";
            String parameter = "no parameter";
            String title = pm.Title;
            String msg = pm.Content.Trim();
            Int32 id = pm.Id;
            Int32 postNumber = 0;
            Match m = Regex.Match(msg, @"(\w+)\s+(.*)", RegexOptions.Singleline);
            // action: arm, unarm, shoot, trade suit, steal suit, steal gun
            if (m.Success)
            {
                String action = m.Groups[1].Value.ToLower();
                String rem = m.Groups[2].Value.Trim();
                switch (action)
                {
                    case "arm":
                        {
                            verb = CorralAction.Arm;
                            parameter = TrimLead(rem, "with");
                        }
                        break;

                    case "unarm":
                        {
                            verb = CorralAction.UnArm;
                        }
                        break;

                    case "shoot":
                        {
                            target = rem;
                            verb = CorralAction.Shoot;
                            Console.WriteLine("shooting '{0}'", target);
                        }
                        break;

                    case "trade":
                        {
                            rem = TrimLead(rem, "suit");
                            target = TrimLead(rem, "with");
                            verb = CorralAction.Trade;
                        }
                        break;

                    case "steal":
                        {
                            parameter = ParseFirstWord(ref rem);
                            target = TrimLead(rem, "from");
                            verb = CorralAction.Steal;
                        }
                        break;

                    case "toss":
                        {
                            parameter = TrimLead(rem, "gun");
                            verb = CorralAction.Toss;
                        }
                        break;

                    default:
                        {
                            Console.WriteLine("unknown action '{0}'", action);
                        }
                        break;
                }
            }
            CorralEvent ga = new CorralEvent(actor, verb, target, parameter, pm.TimeStamp.Value, title, msg, id, postNumber);
            return ga;
        }
        CorralEvent PostToAction(Post post)
        {
            String actor = post.Poster.Name;
            CorralAction verb = CorralAction.NoAction;
            String target = "no target";
            String parameter = "no parameter";
            String title = post.Title;
            String msg = post.Content.Trim();
            Int32 id = post.PostId;
            Int32 postNumber = post.PostNumber;
            foreach (Bold b in post.Bolded)
            {
                String content = b.Content.Trim();
                String shoot = "shoot ";
                if (content.StartsWith(shoot))
                {
                    target = content.Substring(shoot.Length).Trim();
                    verb = CorralAction.Shoot;
                    break;
                }
            }
            CorralEvent ga = new CorralEvent(actor, verb, target, parameter, post.Time, title, msg, id, postNumber);
            return ga;
        }
        private void LoadGame()
        {
            _thread = _forum.Reader();
            _thread.PageCompleteEvent += new EventHandler<PageCompleteEventArgs>(_thread_PageCompleteEvent);
            _thread.ReadCompleteEvent += new EventHandler<ReadCompleteEventArgs>(_thread_ReadCompleteEvent);
            _thread.PageErrorEvent += new EventHandler<PageErrorEventArgs>(_thread_PageErrorEvent);
        }

        void _thread_PageErrorEvent(object sender, PageErrorEventArgs e)
        {
            throw new NotImplementedException();
        }

        void _thread_ReadCompleteEvent(object sender, ReadCompleteEventArgs e)
        {
            _pageStart = e.pageEnd;
            _readingPosts = false;
        }

        void _thread_PageCompleteEvent(object sender, PageCompleteEventArgs e)
        {
            _posts.AddRange(e.Posts);
        }

        private void _forum_LoginEvent(object sender, POG.Forum.LoginEventArgs e)
        {
            switch (e.LoginEventType)
            {
                case POG.Forum.LoginEventType.LoginFailure:
                    {
                        _synchronousInvoker(() => OnLoginResult(e.Username, false));
                    }
                    break;

                case POG.Forum.LoginEventType.LoginSuccess:
                    {
                        LoadGame();
                        _synchronousInvoker( () => OnLoginResult(e.Username, true));
                    }
                    break;

                case POG.Forum.LoginEventType.LogoutSuccess:
                    {
                    }
                    break;
            }
        }
    }
}
