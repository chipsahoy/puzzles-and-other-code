using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Diagnostics;
using POG.Utils;

namespace POG.Forum
{
    public class LobbyReader
    {
        #region fields
        readonly ConnectionSettings _connectionSettings;
        Action<Action> _synchronousInvoker;
        #endregion
        #region constructors
        public LobbyReader(ConnectionSettings connectionSettings, Action<Action> synchronousInvoker)
        {
            _connectionSettings = connectionSettings;
            _synchronousInvoker = synchronousInvoker;
        }
        private LobbyReader()
        {
        }
        #endregion
        #region public methods
        public void ReadLobby(String url, Int32 pageStart, Int32 pageEnd, Boolean recentFirst)
        {
            Task t = new Task(() => GetPages(url, pageStart, pageEnd, recentFirst));
            t.Start();
        }
        #endregion
        #region public events
        public event EventHandler<LobbyPageCompleteEventArgs> LobbyPageCompleteEvent;
        #endregion
        #region event helpers
        virtual internal void OnLobbyPageComplete(String url, Int32 page, DateTimeOffset ts, Boolean recentFirst,  
                List<ForumThread> threads)
        {
            try
            {
                var handler = LobbyPageCompleteEvent;
                if (handler != null)
                {
                    _synchronousInvoker.Invoke(
                        () => handler(this, new LobbyPageCompleteEventArgs(url, page, ts, recentFirst, threads))
                    );
                }
            }
            catch
            {
            }
        }
        #endregion
        #region private methods
        void GetPages(String url, Int32 pageStart, Int32 pageEnd, Boolean recentFirst)
        {
            // beginning of time:
            // http://forumserver.twoplustwo.com/59/puzzles-other-games/?pp=25&sort=dateline&order=asc&daysprune=-1
            // http://forumserver.twoplustwo.com/59/puzzles-other-games/index2.html?sort=dateline&order=asc&daysprune=-1
            // normal:
            // http://forumserver.twoplustwo.com/59/puzzles-other-games/
            // http://forumserver.twoplustwo.com/59/puzzles-other-games/index2.html

            Parallel.For(pageStart, pageEnd + 1, (Int32 page) => { GetPage(url, page, recentFirst); });
        }
        void GetPage(String url, Int32 pageNumber, Boolean recentFirst)
        {
            if (pageNumber > 1)
            {
                url += "index" + pageNumber + ".html";
            }
            if (!recentFirst)
            {
                url += "?sort=dateline&order=asc&daysprune=-1";
            }
            string doc = null;
            for (int i = 0; i < 10; i++)
            {
                ConnectionSettings cs = _connectionSettings.Clone();
                cs.Url = url;
                doc = HtmlHelper.GetUrlResponseString(cs);
                if (doc != null)
                {
                    break;
                }
                else
                {
                    Trace.TraceInformation("*** Error fetching page " + pageNumber.ToString());
                }
            }
            List<ForumThread> threadList = new List<ForumThread>();
            if (doc == null)
            {
                OnLobbyPageComplete(url, pageNumber, DateTime.Now, recentFirst, threadList);
                return;
            }
            DateTimeOffset ts;
            ParseLobbyPage(url, doc, out ts, ref threadList);
            OnLobbyPageComplete(url, pageNumber, ts, recentFirst, threadList);
        }

        private void ParseLobbyPage(string url, string doc, out DateTimeOffset serverTime, ref List<ForumThread> threadList)
        {
            Int32 threadId = TwoPlusTwoForum.ThreadIdFromUrl(url);
            var html = new HtmlAgilityPack.HtmlDocument();
            html.LoadHtml(doc);
            HtmlAgilityPack.HtmlNode root = html.DocumentNode;

            serverTime = DateTime.Now;
            //(//div[class="smallfont", align="center'])[last()] All times are GMT ... The time is now <span class="time">time</span>"."

            HtmlAgilityPack.HtmlNode timeNode = root.SelectNodes("//div[@class='smallfont'][@align='center']").Last();
            if (timeNode != null)
            {
                String timeText = timeNode.InnerText;
                serverTime = Utils.Misc.ParsePageTime(timeText, DateTime.UtcNow);
            }

            HtmlAgilityPack.HtmlNodeCollection threads = root.SelectNodes("//tbody[contains(@id, 'threadbits_forum_')]/tr[contains(@id, 'vbpostrow_')]");
            if (threads == null)
            {
                return;
            }
            foreach (HtmlAgilityPack.HtmlNode thread in threads)
            {
                ForumThread t = HtmlToThread(threadId, thread, serverTime);
                if (t != null)
                {
                    threadList.Add(t);
                }
            }
        }

        private ForumThread HtmlToThread(int threadId, HtmlAgilityPack.HtmlNode thread,
            DateTimeOffset pageTime)
        {
            ForumThread ft = new ForumThread();
            // td[1] = thread status icon
            HtmlAgilityPack.HtmlNode node = thread.SelectSingleNode("td[1]/img");
            Boolean locked = false;
            if (node != null)
            {
                String img = node.Attributes["src"].Value;
                if(img.Contains("lock"))
                {
                    locked = true;
                }

            }
            ft.Locked = locked;

            // td[2]/img = thread icon
            node = thread.SelectSingleNode("td[2]/img");
            String threadIcon = String.Empty;
            if (node != null)
            {
                threadIcon = node.Attributes["alt"].Value;
                ft.ThreadIconText = threadIcon;
            }
            // td[3] = title area
            //      div[1]/a[id like thread_title_] Title, link
            String threadURL = String.Empty;
            String threadTitle = String.Empty;
            node = thread.SelectSingleNode("td[3]/div[1]/a[contains(@id, 'thread_title_')]");
            if (node != null)
            {
                threadTitle = HtmlAgilityPack.HtmlEntity.DeEntitize(node.InnerText);
                threadURL = HtmlAgilityPack.HtmlEntity.DeEntitize(node.Attributes["href"].Value);
                ft.Title = threadTitle;
                ft.URL = threadURL;
                ft.ThreadId = Misc.TidFromURL(ft.URL);
            }
            //      div[2]/span OP name
            node = thread.SelectSingleNode("td[3]/div[2]/span[@style='cursor:pointer']");
            if (node != null)
            {
                Int32 opId = -1;
                String threadOP = HtmlAgilityPack.HtmlEntity.DeEntitize(node.InnerText);
                String profile = node.Attributes["onclick"].Value;
                if (profile != string.Empty)
                {
                    opId = Misc.ParseMemberId(profile);
                }
                ft.OP = new Poster(threadOP, opId);
            }
            // td[4] = last post area
            //   title=Replies: x, Views: Y
            String lastPostTime = String.Empty;
            String lastPoster = String.Empty;

            node = thread.SelectSingleNode("td[4]/div");
            if (node != null)
            {
                String lastInfo = HtmlAgilityPack.HtmlEntity.DeEntitize(node.InnerText.Trim());
                String[] lines = lastInfo.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);
                if (lines.Count() >= 2)
                {
                    lastPostTime = lines[0].Trim();
                    DateTimeOffset dtLastPost = Misc.ParseItemTime(pageTime, lastPostTime);
                    lastPoster = lines[1].Trim().Substring(3);
                    ft.LastPostTime = dtLastPost.ToUniversalTime();
                }
                Int32 lastId = -1;
                node = node.SelectSingleNode("a[1]");
                if (node != null)
                {
                    String profile = node.Attributes["href"].Value;
                    lastId = Misc.ParseMemberId(profile);
                }
                ft.LastPoster = new Poster(lastPoster, lastId);
            }

            String sReplies = String.Empty;
            node = thread.SelectSingleNode("td[5]/a");
            if (node != null)
            {
                sReplies = node.InnerText;
                Int32 replies;
                if(Int32.TryParse(sReplies, NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out replies))
                {
                    ft.ReplyCount = replies;
                }
            }
            String sViews = String.Empty;
            node = thread.SelectSingleNode("td[6]");
            if (node != null)
            {
                sViews = node.InnerText;
                Int32 views;
                if (Int32.TryParse(sViews, NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out views))
                {
                    ft.Views = views;
                }
            }
            return ft;
        }
        #endregion
    }
    public class LobbyPageCompleteEventArgs : EventArgs
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
        public Boolean RecentFirst
        {
            get;
            private set;
        }
        public DateTimeOffset TimeStamp
        {
            get;
            private set;
        }
        public List<ForumThread> Threads
        {
            get;
            private set;
        }
        public LobbyPageCompleteEventArgs(String url, Int32 page, DateTimeOffset ts, Boolean recentFirst, 
                List<ForumThread> threads)
        {
            URL = url;
            Page = page;
            TimeStamp = ts;
            RecentFirst = recentFirst;
            Threads = threads;
        }
    }
}
