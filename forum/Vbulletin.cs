using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Text.RegularExpressions;
using System.Net;
using System.Globalization;
using System.ComponentModel;
using POG.Utils;


namespace POG.Forum
{
    internal class VBulletinSM : StateMachine
    {
        #region members
        VBulletin_3_8_7 _outer;
        readonly ConnectionSettings connectionSettings = new ConnectionSettings(BASE_URL);
        private String _username = ""; // if this changes from what user entered previously, need to logout then re-login with new info.

        private const String BASE_URL = "http://forumserver.twoplustwo.com/";
        Int32 _lastPostRead = 0;
        private String[] _pages = new String[1];
        private Post[] _posts = new Post[100];
        int _postsPerPage = 50;
        private String _threadURL = "";
        #endregion

        internal VBulletinSM(VBulletin_3_8_7 outer, StateMachineHost host) :
            base("VBulletin", host)
        {
            _outer = outer;
            SetInitialState(StateLoggedOut);
        }
        #region Properties
        internal String ThreadURL
        {
            get
            {
                return _threadURL;
            }
            set
            {
                if (value == null)
                {
                    value = String.Empty;
                }
                String url = value.Trim();
                if (url.Length > 0)
                {
                    if (url.EndsWith(".html") || url.EndsWith(".htm"))
                    {
                        url = url.Substring(0, url.LastIndexOf("index"));
                    }

                    if (!url.EndsWith("/"))
                    {
                        url += "/";
                    }
                }
                if (url != _threadURL)
                {
                    _threadURL = url;
                    Reset();
                    _outer.OnPropertyChanged("ThreadURL");
                }
            }
        }
        internal Int32 PostsPerPage
        {
            get
            {
                return _postsPerPage;
            }
            private set
            {
                if (_postsPerPage != value)
                {
                    // just start over...
                    _postsPerPage = value;
                    PageCount = 1;
                }
            }
        }
        Int32 _pageCount = 1;
        internal Int32 PageCount
        {
            get
            {
                return _pageCount;
            }
            set
            {
                if (value != _pageCount)
                {
                    Console.WriteLine("Getting pages {0} to {1}", _pageCount + 1, value);
                    PostEvent(new Event<int, int>("GetNewPages", _pageCount + 1, value));
                    _pageCount = value;
                    Array.Resize(ref _pages, _pageCount + 1);
                    Array.Resize(ref _posts, 1 + (_pageCount * _postsPerPage));
                }
            }
        }
        #endregion
        #region States
        private void ResetThread()
        {
            _pageCount = 1;
            _pages = new String[1];
            _posts = new Post[100];
            _lastPostRead = 0;
        }
        private State StateLoggedOut(Event e)
        {
            switch (e.EventName)
            {
                case "EventEnter":
                    {
                        if (_username != null)
                        {
                            _outer.OnStatusUpdate("Logging out " + _username);
                            DoLogout();
                            _outer.OnStatusUpdate("Logged out " + _username);
                            _outer.OnLoginEvent(_username, LoginEventType.LogoutSuccess);
                        }
                    }
                    return null;

                case "DoLogin":
                    {
                        Event<String, String> evtLogin = e as Event<String, String>;
                        String resp = DoLogin(evtLogin.Param1, evtLogin.Param2);
                        if (resp != null)
                        {
                            _outer.OnStatusUpdate(resp);
                            _outer.OnLoginEvent(_username, LoginEventType.LoginFailure);
                        }
                        else
                        {
                            _outer.OnStatusUpdate("Successful login as " + _username);
                            _outer.OnLoginEvent(_username, LoginEventType.LoginSuccess);
                            ChangeState(StateLoggedIn);
                        }

                    }
                    return null;
            }
            return StateTop;
        }
        private State StateLoggedIn(Event e)
        {
            switch (e.EventName)
            {
                case "EventEnter":
                    {
                        if (_threadURL.Length == 0)
                        {
                            ChangeState(StateIdle);
                        }
                        else
                        {
                            ChangeState(StateMonitoring);
                        }
                    }
                    return null;

                case "EventExit":
                    {
                    }
                    return null;

                case "NewURL":
                    {
                        ResetThread();
                        ChangeState(StateMonitoring);
                    }
                    return null;

                case "DoLogout":
                    {
                        ChangeState(StateLoggedOut);
                    }
                    return null;
            }
            return StateTop;
        }
        private State StateTop(Event e)
        {
            return null;
        }
        private State StateIdle(Event e)
        {
            switch (e.EventName)
            {
                case "EventEnter":
                    {
                        ResetThread();
                        ThreadURL = "";
                    }
                    return null;

                case "EventExit":
                    {
                    }
                    return null;
            }
            return StateLoggedIn;
        }
        private State StateMonitoring(Event e)
        {
            switch (e.EventName)
            {
                case "EventEnter":
                    {
                        PostEvent(new Event("PollThread"));
                    }
                    return null;

                case "EventExit":
                    {
                        CancelTimer("PollThread");
                    }
                    return null;

                case "PollThread":
                    {
                        GetCurrentPage();
                        StartOneShotTimer(_outer.ThreadPollingPeriodSeconds * 1000, e);
                    }
                    return null;

                case "GetNewPages":
                    {
                        var evt = e as Event<Int32, Int32>;
                        GetNewPages(evt.Param1, evt.Param2);
                    }
                    return null;


                case "NewURL":
                    {
                        ChangeState(StateLoggedIn);
                        PostEvent(e);
                    }
                    return null;

                case "Reset":
                    {
                        ChangeState(StateIdle);
                    }
                    return null;
            }
            return StateLoggedIn;
        }
        #endregion
        #region private methods
        private void Reset()
        {
            PostEvent(new Event("NewURL"));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="username">username of gimmick account</param>
        /// <param name="password">password of gimmick account</param>
        /// <returns></returns>
        private String DoLogin(String username, String password)
        {
            // Get some needed cookies!
            connectionSettings.Url = BASE_URL;
            String page = HtmlHelper.GetUrlResponseString(connectionSettings);
            // They are hidden in javascript...
            Regex reg = new Regex(Regex.Escape("setCookie('") + "([^\']*)\', \'([^\']*)\'");
            Match match = reg.Match(page);
            if (match.Success)
            {
                string cookieName = match.Groups[1].Value;
                string cookieValue = match.Groups[2].Value;
                Cookie cookie = new Cookie(cookieName, cookieValue, "/", "forumserver.twoplustwo.com");
                connectionSettings.CC.Add(cookie);
                Cookie cReferrer = new Cookie("DOAReferrer", "http://forumserver.twoplustwo.com", "/", "forumserver.twoplustwo.com");
                connectionSettings.CC.Add(cReferrer);
            }
            _username = null;
            String passToken = SecurityUtils.md5(password);

            connectionSettings.Url = String.Format("{0}login.php?do=login", BASE_URL);
            connectionSettings.Data =
                String.Format("vb_login_username={0}&cookieuser=1&vb_login_password=&s=&securitytoken=guest&do=login&vb_login_md5password={1}&vb_login_md5password_utf={1}", username, passToken);
            String resp = HtmlHelper.PostToUrl(connectionSettings);

            if (resp == null)
            {
                // login failure
                return "The following error occurred while logging in:\n\n" + connectionSettings.Message;
            }

            if (!resp.Contains("exec_refresh()"))
            {
                return "Error logging in.  Please verify login information.";
            }


            // set posts/page
            connectionSettings.Url = String.Format("{0}profile.php?do=editoptions", BASE_URL);
            _username = username;
            resp = HtmlHelper.GetUrlResponseString(connectionSettings);
            if (resp != null)
            {
                Match m = Regex.Match(resp, "umaxposts.*?value=\"(-?\\d+)\"[ ](class=\"[A-z0-9]*\")?[ ]*selected=\"selected\"", RegexOptions.Singleline);
                if (m.Success)
                {
                    Int32 postsPerPage = Int32.Parse(m.Groups[1].Value);
                    if (postsPerPage == -1)
                    {
                        PostsPerPage = 15;
                    }
                    else
                    {
                        PostsPerPage = postsPerPage;
                    }
                }
                else
                {

                    PostsPerPage = 100;
                    return
                        "Login success, but there was an error setting the posts/page.  Please set your settings to 100 posts/page.";
                }

            }
            else
            {
                PostsPerPage = 100;
                return
                    "Login success, but there was an error setting the posts/page.  Please set your settings to 100 posts/page.";
            }

            // logging in sets the cookies in connectionSettings so we don't have to do anything else.
            return null;
        }

        private void DoLogout()
        {
            // get the page once to find the logout url
            connectionSettings.Url = BASE_URL;
            String resp = HtmlHelper.GetUrlResponseString(connectionSettings);
            Match m = Regex.Match(resp, "logouthash=([A-z0-9-])");
            if (m.Success)
            {
                String hash = m.Groups[1].Value;
                connectionSettings.Url = String.Format("http://forumserver.twoplustwo.com/login.php?do=logout&amp;logouthash={0}", hash);
                HtmlHelper.GetUrlResponseString(connectionSettings);
                connectionSettings.CC = new CookieContainer(); // just in case
                _username = null;
            }
        }
        private void GetCurrentPage()
        {
            _outer.OnStatusUpdate("Checking page " + _pageCount.ToString());
            String s = GetPage(_pageCount);
            if (s != null)
            {
                Int32 lastRead;
                ParseThreadPage(s, out lastRead);
                if (lastRead > _lastPostRead)
                {
                    _lastPostRead = lastRead;
                    _outer.OnStatusUpdate("Found new posts on page " + _pageCount.ToString());
                    _outer.OnNewPostsAvailable(lastRead);
                }
                else
                {
                    _outer.OnStatusUpdate("Done, no new posts on page " + _pageCount.ToString());
                }
            }
            else
            {
                _outer.OnStatusUpdate("Failed getting page " + _pageCount.ToString());
            }
        }
        private void GetNewPages(Int32 firstPage, Int32 lastPage)
        {
            _outer.OnStatusUpdate("Getting pages " + firstPage.ToString() + " to " + lastPage.ToString());
            var pages = from pageNumber in Enumerable.Range(firstPage, (lastPage - firstPage) + 1).AsParallel() select pageNumber;
            pages.ForAll( pageNumber => 
            {
                String pageContent = GetPage(pageNumber);
                if (pageContent != null)
                {
                    Int32 lastRead;
                    ParseThreadPage(pageContent, out lastRead);
                    Console.WriteLine("Saving page " + pageNumber.ToString());
                    if (pageNumber > _pages.Count() - 1)
                    {
                        Console.WriteLine("Invalid page number " + pageNumber);
                    }
                    _pages[pageNumber] = pageContent;
                    Console.WriteLine("Done with page {0}", pageNumber);
                    if (pageNumber == lastPage)
                    {
                        _lastPostRead = lastRead;
                    }
                }
            });
            _outer.OnStatusUpdate("Done getting pages " + firstPage.ToString() + " to " + lastPage.ToString());
            _outer.OnNewPostsAvailable(_lastPostRead);
        }

        private String GetPage(Int32 pageNumber)
        {
            String destination = _threadURL;
            if (pageNumber > 1)
            {
                destination += "index" + pageNumber + ".html";
            }
            string doc = null;
            for (int i = 0; i < 10; i++)
            {
                ConnectionSettings cs = connectionSettings.Clone();
                cs.Url = destination;
                doc = HtmlHelper.GetUrlResponseString(cs);
                if (doc != null)
                {
                    break;
                }
                else
                {
                    Console.WriteLine("*** Error fetching page " + pageNumber.ToString());
                }
            }
            return doc;
        }
        private volatile bool _shouldStop = false;
        internal void Login(string username, string password)
        {
            PostEvent(new Event<string, string>("DoLogin", username, password));
        }
        internal void Logout()
        {
            PostEvent(new Event("DoLogout"));
        }

        private int PageFromNumber(int number)
        {
            int page = (number / _postsPerPage) + 1;
            return page;
        }
        private void ParseThreadPage(String doc, out Int32 lastPostNumber)
        {
            lastPostNumber = 0;
            var html = new HtmlAgilityPack.HtmlDocument();
            html.LoadHtml(doc);
            HtmlAgilityPack.HtmlNode root = html.DocumentNode;
            // find total posts: /table/tr[1]/td[2]/div[@class="pagenav"]/table[1]/tr[1]/td[1] -- Page 106 of 106
            HtmlAgilityPack.HtmlNode pageNode = root.SelectSingleNode("//div[@class='pagenav']/table/tr/td");
            if (pageNode != null)
            {
                string pages = pageNode.InnerText;
                Match m = Regex.Match(pages, @"Page (\d+) of (\d+)");
                if (m.Success)
                {
                    //Console.WriteLine("{0}/{1}", m.Groups[1].Value, m.Groups[2].Value);
                    PageCount = Convert.ToInt32(m.Groups[2].Value);
                }
            }

            // //div[@id='posts']/div/div/div/div/table/tbody/tr[2]
            // td[1]/div[1] has (id with post #, <a> with user id, user name.)
            // td[2]/div[1] has title
            // td[2]/div[2] has post
            // "/html[1]/body[1]/table[2]/tr[2]/td[1]/td[1]/div[2]/div[1]/div[1]/div[1]/div[1]/table[1]/tr[2]/td[2]/div[2]" is a post
            HtmlAgilityPack.HtmlNodeCollection posts = root.SelectNodes("//div[@id='posts']/div/div/div/div/table/tr[2]/td[2]/div[contains(@id, 'post_message_')]");
            if (posts == null)
            {
                return;
            }

            foreach (HtmlAgilityPack.HtmlNode post in posts)
            {
                if (_shouldStop)
                {
                    break;
                }
                Post p = HtmlToPost(post);
                if (p != null)
                {
                    lastPostNumber = p.PostNumber;
                    Console.WriteLine("Saving post " + lastPostNumber.ToString());
                    _posts[lastPostNumber] = p;
                }
            }
        }
        private Post HtmlToPost(HtmlAgilityPack.HtmlNode html)
        {

            string posterName = "";
            Int32 postNumber = 0;
            String postLink = null;
            DateTime postTime = DateTime.Now;
            HtmlAgilityPack.HtmlNode postNumberNode = html.SelectSingleNode("../../../tr[1]/td[2]/a");

            if (postNumberNode != null)
            {
                postNumber = Int32.Parse(postNumberNode.InnerText);
                if (_posts[postNumber] != null)
                {
                    // already have this post.
                    return null;
                }
                postLink = postNumberNode.Attributes["href"].Value;
            }
            RemoveComments(html);
            HtmlAgilityPack.HtmlNode postTimeNode = html.SelectSingleNode("../../../tr[1]/td[1]");
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

            HtmlAgilityPack.HtmlNode userNode = html.SelectSingleNode("../../td[1]/div/a[@class='bigusername']");
            if (userNode != null)
            {
                posterName = HtmlAgilityPack.HtmlEntity.DeEntitize(userNode.InnerText);
            }
            Post p = new Post(posterName, postNumber, postTime, postLink, html);
            return p;
        }
        private void RemoveComments(HtmlAgilityPack.HtmlNode node)
        {
            foreach (var n in node.SelectNodes("//comment()") ?? new HtmlAgilityPack.HtmlNodeCollection(node))
            {
                n.Remove();
            }
        }
        #endregion
        #region internal methods
        internal void Clear()
        {
            PostEvent(new Event("Reset"));
        }
        internal Post[] GetPosts(Int32 firstPost, Int32 lastPost)
        {
            firstPost = Math.Max(firstPost, 1);
            lastPost = Math.Min(lastPost, _lastPostRead);
            Int32 count = 1 + (lastPost - firstPost);
            if (count < 1)
            {
                return null;
            }
            Post[] posts = new Post[count];
            Array.Copy(_posts, firstPost, posts, 0, count);
            return posts;
        }
        public Post[] GetPosts(Int32 firstPost, DateTime lastPostTime)
        {
            var postsQuery = (from post in _posts where (post.PostNumber >= firstPost) && (post.Time <= lastPostTime) select post);
            Post[] posts = postsQuery.ToArray();
            return posts;
        }

        #endregion
    }
    public class VBulletin_3_8_7
    {
        #region members
        VBulletinSM _inner;
        Action<Action> _synchronousInvoker;
        #endregion
        #region constructors
        public VBulletin_3_8_7(Action<Action> synchronousInvoker)
        {
            _synchronousInvoker = synchronousInvoker;
            _inner = new VBulletinSM(this, new StateMachineHost("ForumHost"));
            ThreadPollingPeriodSeconds = 30;
        }
        #endregion
        #region events
        public event EventHandler<NewPostsAvailableEventArgs> NewPostsAvailable;
        virtual internal void OnNewPostsAvailable(Int32 postNumber)
        {
            if (postNumber <= 0)
            {
                return;
            }
            var handler = NewPostsAvailable;
            if (handler != null)
            {
                _synchronousInvoker.Invoke(
                    () => handler(this, new NewPostsAvailableEventArgs(postNumber))
                );
            }

        }
        public event EventHandler<LoginEventArgs> LoginEvent;
        virtual internal void OnLoginEvent(String username, LoginEventType let)
        {
            var handler = LoginEvent;
            if (handler != null)
            {
                LoginEventArgs e = new LoginEventArgs(username, let);
                _synchronousInvoker.Invoke(
                    () => handler(this, e)
                );
            }

        }
        //public event EventHandler<NewPMEventArgs> NewPMEvent;
        //public event EventHandler<PMReadEventArgs> PMReadEvent;
        public event EventHandler FinishedReadingThread;
        public event EventHandler<NewStatusEventArgs> StatusUpdate;
        virtual internal void OnStatusUpdate(String status)
        {
            var handler = StatusUpdate;
            if (handler != null)
            {
                _synchronousInvoker.Invoke(
                    () => handler(this, new NewStatusEventArgs(status))
                );
            }
        }
        virtual internal void OnFinishedReadingThread()
        {
            var handler = FinishedReadingThread;
            if (handler != null)
            {
                _synchronousInvoker.Invoke(
                    () => handler(this, EventArgs.Empty)
                );
            }
        }
        
        public event PropertyChangedEventHandler PropertyChanged;
        internal virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                _synchronousInvoker.Invoke(
                    () => PropertyChanged(this, new PropertyChangedEventArgs(propertyName))
                );
            }
        }
        #endregion

        #region public properties
        public String ThreadURL
        {
            get
            {
                return _inner.ThreadURL;
            }
            set
            {
                _inner.ThreadURL = value;
            }
        }
        public Int32 ThreadPollingPeriodSeconds
        {
            get;
            set;
        }
        public Int32 PMPollingPeriodSeconds
        {
            get;
            set;
        }
        public Boolean Locked
        {
            get;
            set;
        }
        #endregion
        #region public methods
        public void Login(string user, string password)
        {
            _inner.Login(user, password);
        }
        public void Logout()
        {
            _inner.Logout();
        }
        public Boolean SendPM(IEnumerable<String> To, IEnumerable<String> bcc, String title, String content, Boolean receipt = true)
        {
            return false;
        }
        public Boolean MakePost(String content, Int32 icon = 0)
        {
            return false;
        }
        public void Start()
        {
        }
        public void Stop()
        { }
        public void RefreshNow()
        { }
        public void Clear()
        {
            _inner.Clear();
        }
        public Post[] GetPosts(Int32 firstPost, Int32 lastPost)
        {
            Post[] posts = _inner.GetPosts(firstPost, lastPost);
            return posts;
        }
        public Post[] GetPosts(Int32 firstPost, DateTime lastPostTime)
        {
            Post[] posts = _inner.GetPosts(firstPost, lastPostTime);
            return posts;
        }
        #endregion
    }
    public class NewStatusEventArgs : EventArgs
    {
        public String Status
        {
            get;
            private set;
        }
        public NewStatusEventArgs(String status)
        {
            Status = status;
        }
    }
    public class NewPostsAvailableEventArgs : EventArgs
    {
        public Int32 NewestPostNumber
        {
            get;
            private set;
        }
        public NewPostsAvailableEventArgs(Int32 postNumber)
        {
            NewestPostNumber = postNumber;
        }
    }
    public class NewPMEventArgs : EventArgs
    {
        public PrivateMessage PM
        {
            get;
            private set;
        }
        public NewPMEventArgs(PrivateMessage pm)
        {
            PM = pm;
        }
    }
    public class PMReadEventArgs : EventArgs
    {
        public PrivateMessage PM
        {
            get;
            private set;
        }
        public String Reader
        {
            get;
            private set;
        }
        public DateTime Time
        {
            get;
            private set;
        }
        public PMReadEventArgs(PrivateMessage pm, String reader, DateTime when)
        {
            PM = pm;
            Reader = reader;
            Time = when;
        }
    }
    public enum LoginEventType
    {
        LoginFailure = 0,
        LoginSuccess = 1,
        LogoutFailure = 2,
        LogoutSuccess = 3,
    }
    public class LoginEventArgs : EventArgs
    {
        public String Username
        {
            get;
            private set;
        }
        public LoginEventType LoginEventType
        {
            get;
            private set;
        }
        public LoginEventArgs(String username, LoginEventType let)
        {
            Username = username;
            LoginEventType = let;
        }
    }

}
