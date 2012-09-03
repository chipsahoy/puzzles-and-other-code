using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Text.RegularExpressions;
using System.Net;
using System.Globalization;
using System.ComponentModel;
using System.Windows.Forms;
using POG.Utils;
using Newtonsoft;

namespace POG.Forum
{
	internal class VBulletinSM : StateMachine
	{
		#region members
		TwoPlusTwoForum _outer;
		readonly ConnectionSettings _connectionSettings = new ConnectionSettings(TwoPlusTwoForum.BASE_URL);
        Action<Action> _synchronousInvoker;
        private String _username = ""; // if this changes from what user entered previously, need to logout then re-login with new info.
        int _postsPerPage = 50;

		#endregion

        internal VBulletinSM(TwoPlusTwoForum outer, StateMachineHost host, Action<Action> synchronousInvoker) :
			base("VBulletin", host)
		{
			_outer = outer;
            _synchronousInvoker = synchronousInvoker;
			SetInitialState(StateLoggedOut);
		}
		#region Properties
        internal ThreadReader Reader()
        {
            ThreadReader t = new ThreadReader(_connectionSettings, _synchronousInvoker);
            return t;
        }
        internal LobbyReader Lobby()
        {
            LobbyReader lr = new LobbyReader(_connectionSettings, _synchronousInvoker);
            return lr;
        }

        public Int32 PostsPerPage {
            get
            {
                return _postsPerPage;
            }
        }
        #endregion
		#region States
        private State StateTop(Event e)
        {
            return null;
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
					}
					return null;

				case "EventExit":
					{
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
		#endregion
		#region private methods
		/// <summary>
		/// 
		/// </summary>
		/// <param name="username">username of gimmick account</param>
		/// <param name="password">password of gimmick account</param>
		/// <returns></returns>
		private String DoLogin(String username, String password)
		{
			// Get some needed cookies!
            _connectionSettings.Url = TwoPlusTwoForum.BASE_URL;
			String page = HtmlHelper.GetUrlResponseString(_connectionSettings);
			if (page == null)
			{
				return "Error loggin in. Could not reach " + TwoPlusTwoForum.BASE_URL;
			}
			// They are hidden in javascript...
			Regex reg = new Regex(Regex.Escape("setCookie('") + "([^\']*)\', \'([^\']*)\'");
			Match match = reg.Match(page);
			if (match.Success)
			{
				string cookieName = match.Groups[1].Value;
				string cookieValue = match.Groups[2].Value;
				Cookie cookie = new Cookie(cookieName, cookieValue, "/", "forumserver.twoplustwo.com");
				_connectionSettings.CC.Add(cookie);
				Cookie cReferrer = new Cookie("DOAReferrer", "http://forumserver.twoplustwo.com", "/", "forumserver.twoplustwo.com");
				_connectionSettings.CC.Add(cReferrer);
			}
			_username = null;
			String hashedPassword = SecurityUtils.md5(password);

            _connectionSettings.Url = String.Format("{0}login.php?do=login", TwoPlusTwoForum.BASE_URL);
			_connectionSettings.Data =
				String.Format("vb_login_username={0}&cookieuser=1&vb_login_password=&s=&securitytoken=guest&do=login&vb_login_md5password={1}&vb_login_md5password_utf={1}", username, hashedPassword);
			String resp = HtmlHelper.PostToUrl(_connectionSettings);

			if (resp == null)
			{
				// login failure
				return "The following error occurred while logging in:\n\n" + _connectionSettings.Message;
			}

			if (!resp.Contains("exec_refresh()"))
			{
				return "Error logging in.  Please verify login information.";
			}


			// set posts/page
            _connectionSettings.Url = String.Format("{0}profile.php?do=editoptions", TwoPlusTwoForum.BASE_URL);
			_username = username;
			resp = HtmlHelper.GetUrlResponseString(_connectionSettings);
			if (resp != null)
			{
				Match m = Regex.Match(resp, "umaxposts.*?value=\"(-?\\d+)\"[ ](class=\"[A-z0-9]*\")?[ ]*selected=\"selected\"", RegexOptions.Singleline);
				if (m.Success)
				{
					Int32 postsPerPage = Int32.Parse(m.Groups[1].Value);
					if (postsPerPage == -1)
					{
						_postsPerPage = 15;
					}
					else
					{
                        _postsPerPage = postsPerPage;
					}
				}
				else
				{

                    _postsPerPage = 100;
					return
						"Login success, but there was an error setting the posts/page.  Please set your settings to 100 posts/page.";
				}

			}
			else
			{
                _postsPerPage = 100;
				return
					"Login success, but there was an error setting the posts/page.  Please set your settings to 100 posts/page.";
			}

			// logging in sets the cookies in connectionSettings so we don't have to do anything else.
			return null;
		}

		private void DoLogout()
		{
			// get the page once to find the logout url
            _connectionSettings.Url = TwoPlusTwoForum.BASE_URL;
			String resp = HtmlHelper.GetUrlResponseString(_connectionSettings);
			if (resp != null)
			{
				Match m = Regex.Match(resp, "logouthash=([A-z0-9-])");
				if (m.Success)
				{
					String hash = m.Groups[1].Value;
					_connectionSettings.Url = String.Format("http://forumserver.twoplustwo.com/login.php?do=logout&amp;logouthash={0}", hash);
					HtmlHelper.GetUrlResponseString(_connectionSettings);
					_connectionSettings.CC = new CookieContainer(); // just in case
					_username = null;
				}
			}
		}
		internal void Login(string username, string password)
		{
			PostEvent(new Event<string, string>("DoLogin", username, password));
		}
		internal void Logout()
		{
			PostEvent(new Event("DoLogout"));
		}
		internal void GetAlias()
		{
			ConnectionSettings cs = _connectionSettings.Clone();
			cs.Url = "http://www.checkmywwstats.com/.hidden/ajax/lookupalias.php";
			cs.Data = @"data={""votes"":[""Chips""],""playerlist"":[""aksdal"", ""CPHoya"", ""Larry Legend"", ""Chips Ahoy"", ""Thingyman""]}";
			// "submitter=oreos"
			//cs.Data = "x=oreos";
			cs.Message = cs.Data;
			Console.WriteLine("Posting: " + cs.Data);
			String resp = HtmlHelper.PostToUrl(cs);
			if (resp == null)
			{
				// failure
			}

		}
		#endregion
        internal Boolean MakePost(Int32 ThreadId, String content, Int32 icon = 0)
        {
            /* headers
                POST /newreply.php?do=postreply&t=1198532 HTTP/1.1
                Host: forumserver.twoplustwo.com
                Connection: keep-alive
                Content-Length: 299
                Origin: http://forumserver.twoplustwo.com
                X-Requested-With: XMLHttpRequest
                User-Agent: Mozilla/5.0 (Windows NT 5.1) AppleWebKit/536.5 (KHTML, like Gecko) Chrome/19.0.1084.52 Safari/536.5
                Content-Type: application/x-www-form-urlencoded; charset=UTF-8
                Accept: * /* <-- space added
                Referer: http://forumserver.twoplustwo.com/59/puzzles-other-games/pog-pub-may-2012-lc-now-even-more-gimmicks-nsfw-1198532/index62.html
                Accept-Encoding: gzip,deflate,sdch
                Accept-Language: en-US,en;q=0.8
                Accept-Charset: ISO-8859-1,utf-8;q=0.7,*;q=0.3
                Cookie: __utmz=126502536.1325477731.1.1.utmcsr=(direct)|utmccn=(direct)|utmcmd=(none); Listing15=Open; Listing8=Open; Listing2=Open; bblastvisit=1325477737; bblastactivity=0; bbuserid=81718; bbpassword=bed5d97301ccb4e84503ce49bedcf76a; __utma=126502536.1375902440.1325477731.1337991697.1338028399.110; __utmc=126502536; bbsessionhash=2c6af250e2d1881d045bd64c74a8626f; vbseo_loggedin=yes; Listing9=Open; __utmb=126502536

             * querystring do=postreply&t=1198532
             * body --
securitytoken blah
ajax 1
ajax_lastpost 1338033352
message test
wysiwyg 0 
styleid 0
fromquickreply 1
s
securitytoken blah
do postreply
t 1198532
p who care
specifiedpost 0
parseurl 1
loggedinuser 81788
             * openclose 1 <-- lock thread
             * */
            ConnectionSettings cs = _connectionSettings.Clone();
            cs.Url = ThreadId.ToString();
            String doc = HtmlHelper.GetUrlResponseString(cs);
            if (doc == null)
            {
                return false;
            }
            // parse out securitytoken
            Regex reg = new Regex("var SECURITYTOKEN = \"(.+)\"");
            String securityToken = "";
            Match match = reg.Match(doc);
            if (match.Success)
            {
                securityToken = match.Groups[1].Value;
            }
            //			var ajax_last_post = 1338251071;
            String ajaxLastPost = "";
            reg = new Regex("var ajax_last_post = (.+);");
            match = reg.Match(doc);
            if (match.Success)
            {
                ajaxLastPost = match.Groups[1].Value;
            }
            String threadId = "";
            reg = new Regex("<input type=\"hidden\" name=\"t\" value=\"(.+)\" id=\"qr_threadid\" />");
            match = reg.Match(doc);
            if (match.Success)
            {
                threadId = match.Groups[1].Value;
            }
            /*				<input type="hidden" name="fromquickreply" value="1" />
                <input type="hidden" name="s" value="" />
                <input type="hidden" name="securitytoken" value="1338251187-cd0e85748ac090ba7cb5281011b49332c0ba455a" />
                <input type="hidden" name="do" value="postreply" />
                <input type="hidden" name="t" value="1204368" id="qr_threadid" />
                <input type="hidden" name="p" value="who cares" id="qr_postid" />
                <input type="hidden" name="specifiedpost" value="0" id="qr_specifiedpost" />
                <input type="hidden" name="parseurl" value="1" />
                <input type="hidden" name="loggedinuser" value="198669" />
             * 
             * openclose=1
             * open:
             * POST http://forumserver.twoplustwo.com/postings.php?t=1204368&pollid= 
             * do=openclosethread&s=&securitytoken=1338253787-f7a244e5e823e6d88002169f4f314e97febf4dce&t=1204368&pollid=
             * close:
             * POST /postings.php?t=1204368&pollid= HTTP/1.1
             * do=openclosethread&s=&securitytoken=1338253981-5657f92aed9e901b4292a60cab0e9efaac4be1fe&t=1204368&pollid=
*/
            StringBuilder msg = new StringBuilder();
            msg.AppendFormat("{0}={1}&", "securitytoken", securityToken);
            msg.AppendFormat("{0}={1}&", "ajax", "1");
            msg.AppendFormat("{0}={1}&", "ajax_lastpost", ajaxLastPost);
            msg.AppendFormat("{0}={1}&", "message", content + " " + DateTime.Now.Millisecond);
            msg.AppendFormat("{0}={1}&", "wysiwyg", "0");
            msg.AppendFormat("{0}={1}&", "styleid", "0");
            msg.AppendFormat("{0}={1}&", "fromquickreply", "1");
            msg.AppendFormat("{0}={1}&", "s", "");
            msg.AppendFormat("{0}={1}&", "securitytoken", securityToken);
            msg.AppendFormat("{0}={1}&", "do", "postreply");
            msg.AppendFormat("{0}={1}&", "t", threadId);
            msg.AppendFormat("{0}={1}&", "p", "who cares");
            msg.AppendFormat("{0}={1}&", "specifiedpost", "0");
            msg.AppendFormat("{0}={1}&", "parseurl", "1");
            msg.AppendFormat("{0}={1}", "loggedinuser", cs.CC.GetCookies(new System.Uri(TwoPlusTwoForum.BASE_URL))["bbuserid"]);
            cs.Url = String.Format("{0}newreply.php?do=postreply&t={1}", TwoPlusTwoForum.BASE_URL, threadId);
            cs.Data = msg.ToString();
            Console.WriteLine("Posting: " + cs.Data);
            String resp = HtmlHelper.PostToUrl(cs);
            if (resp == null)
            {
                // failure
            }

            return false;
        }



    }
	public class TwoPlusTwoForum
	{
		#region members
		VBulletinSM _inner;
		Action<Action> _synchronousInvoker;
        public static String BASE_URL = "http://forumserver.twoplustwo.com/";
		#endregion
		#region constructors
		public TwoPlusTwoForum(Action<Action> synchronousInvoker)
		{
			_synchronousInvoker = synchronousInvoker;
			_inner = new VBulletinSM(this, new StateMachineHost("ForumHost"), synchronousInvoker);
		}
		#endregion
		#region events
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
		
		#endregion
        public Int32 PostsPerPage
        {
            get
            {
                return _inner.PostsPerPage;
            }
        }

        #region public methods
        public ThreadReader Reader()
        {
            ThreadReader t = _inner.Reader();
            return t;
        }
        public LobbyReader Lobby()
        {
            LobbyReader lr = _inner.Lobby();
            return lr;
        }
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
        public Boolean MakePost(Int32 ThreadId, String title, String message, Int32 PostIcon, Boolean LockThread)
        {
            return false;
        }
        public static Int32 ThreadIdFromUrl(String url)
        {
            // Thread: -.../
            url = url.Trim();
            if (url.Length == 0)
            {
                return 0;
            }
            if (url.EndsWith(".html") || url.EndsWith(".htm"))
            {
                url = url.Substring(0, url.LastIndexOf("index"));
            }
            int ixTidStart = url.LastIndexOf('-') + 1;
            string tid = url.Substring(ixTidStart, url.Length - (ixTidStart + 1));
            Int32 threadId = 0;
            Int32.TryParse(tid, out threadId);
            return threadId;
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
    public class PageCompleteEventArgs : EventArgs
    {
        public String URL
        {
            get;
            private set;
        }
        public Int32 Page
        {
            get;
            private set;
        }
        public Int32 TotalPages
        {
            get;
            private set;
        }
        public DateTime TimeStamp
        {
            get;
            private set;
        }
        public Posts Posts
        {
            get;
            private set;
        }
        public PageCompleteEventArgs(String url, Int32 page, Int32 totalPages, DateTime ts, Posts posts)
        {
            URL = url;
            Page = page;
            TotalPages = totalPages;
            TimeStamp = ts;
            Posts = posts;
        }
    }
	public class PostEventArgs : EventArgs
	{
        public String URL
        {
            get;
            private set;
        }
        public Post Post
		{
			get;
			private set;
		}
		public PostEventArgs(String url, Post post)
		{
            URL = url;
			Post = post;
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
