﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using POG.Utils;
using System.Text.RegularExpressions;
using System.Net;
using System.Globalization;
using System.ComponentModel;
using System.Windows.Forms;
using Newtonsoft;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Web;

namespace POG.Forum
{
    public class ThreadReader_4_2_0 : ThreadReader
    {
        public ThreadReader_4_2_0(ConnectionSettings connectionSettings, Action<Action> synchronousInvoker, String voteRegex, String voteColor, Language language)
            : base(connectionSettings, synchronousInvoker, voteRegex, voteColor, language)
        {
        }
        protected override void ParseThreadPage(String url, String doc, out Int32 lastPageNumber, out DateTimeOffset serverTime, ref Posts postList)
        {
            Int32 threadId = VBulletinForum.ThreadIdFromUrl(url);
            lastPageNumber = 0;
            var html = new HtmlAgilityPack.HtmlDocument();
            html.LoadHtml(doc);
            HtmlAgilityPack.HtmlNode root = html.DocumentNode;

            serverTime = DateTime.Now;
            //(//div[class="smallfont", align="center'])[last()] All times are GMT ... The time is now <span class="time">time</span>"."
            HtmlAgilityPack.HtmlNode timeNode = root.SelectNodes("//div[@id='footer_time']").Last();
            if (timeNode != null)
            {
                String timeText = timeNode.InnerText;
                serverTime = Utils.Misc.ParsePageTime(timeText, DateTime.UtcNow);
                
            }

            
            // Results 301 to 400 of 1195
            HtmlAgilityPack.HtmlNode threadNode = root.SelectSingleNode("//input[@name='t']");
            if (threadNode != null)
            {
                HtmlAgilityPack.HtmlNode pageNode = threadNode.SelectSingleNode("../span[1]/a[1]");
                if (pageNode != null)
                {
                    string pages = pageNode.InnerText;
                    String pagexofy = @"Page (\d+) of (\d+)";
                    if(_language == Language.Estonian) pagexofy = @"Lehek�lg (\d+), kokku (\d+)";
                    Match m = Regex.Match(pages, pagexofy);
                    if (m.Success)
                    {
                        //Trace.TraceInformation("{0}/{1}", m.Groups[1].Value, m.Groups[2].Value);
                        lastPageNumber = Convert.ToInt32(m.Groups[2].Value);
                    }
                }
            }

            // //div[@id='posts']/div/div/div/div/table/tbody/tr[2]
            // td[1]/div[1] has (id with post #, <a> with user id, user name.)
            // td[2]/div[1] has title
            // td[2]/div[2] has post
            // "/html[1]/body[1]/table[2]/tr[2]/td[1]/td[1]/div[2]/div[1]/div[1]/div[1]/div[1]/table[1]/tr[2]/td[2]/div[2]" is a post
            HtmlAgilityPack.HtmlNodeCollection posts = root.SelectNodes("//ol[@id='posts']/li");
            if (posts == null)
            {
                return;
            }
            postList = new Posts();
            foreach (HtmlAgilityPack.HtmlNode post in posts)
            {
                Post p = HtmlToPost(threadId, post, serverTime);
                if (p != null)
                {
                    postList.Add(p);
                }
            }
        }
        protected override Post HtmlToPost(Int32 threadId, HtmlAgilityPack.HtmlNode html, DateTimeOffset pageTime)
        {

            string posterName = "";
            Int32 postNumber = 0;
            String postLink = null;
			Int32 postId = -1;
            DateTimeOffset postTime = DateTime.Now;
            HtmlAgilityPack.HtmlNode postNumberNode = html.SelectSingleNode("div[@class='posthead']/span[@class='nodecontrols']/a[2]");

            if (postNumberNode != null)
            {
                String postNumberText = postNumberNode.Attributes["name"].Value;
                postNumber = Int32.Parse(postNumberText);
            }

            HtmlAgilityPack.HtmlNode postLinkNode = html.SelectSingleNode("div[@class='posthead']/span[@class='nodecontrols']/a[1]");
            if (postLinkNode != null)
            {
                postLink = HtmlAgilityPack.HtmlEntity.DeEntitize(postLinkNode.Attributes["href"].Value);
				if (postLink.Length > 3)
				{
					// showthread.php?12931-Mafia-Convo-Thread&p=316289&viewfull=1#post316289
					int startQuery = postLink.IndexOf('?');
					if (startQuery > -1) postLink = postLink.Substring(startQuery);
					string sPost = HttpUtility.ParseQueryString(postLink).Get("p");
					Int32.TryParse(sPost, out postId);
				}
			}

            RemoveComments(html);
            HtmlAgilityPack.HtmlNode postTimeNode = html.SelectSingleNode("div[@class='posthead']/span[1]/span[@class='date']");
            if (postTimeNode != null)
            {
                string date = postTimeNode.InnerText;
                date = HtmlAgilityPack.HtmlEntity.DeEntitize(date);
                //string time = postTimeNode.ChildNodes[1].InnerText;
                postTime = ParseItemTime(pageTime, date);
                //Trace.TraceInformation("Post time: {0}", postTime.DateTime.ToShortTimeString());
            }
            String postTitle = "";
            HtmlAgilityPack.HtmlNode titleNode = html.SelectSingleNode("div[@class='postdetails']/div[@class='postbody']/div[1]/h2");
            if (titleNode != null)
            {
                postTitle = HtmlAgilityPack.HtmlEntity.DeEntitize(titleNode.InnerText).Trim();
                //Trace.TraceInformation("title[{0}]:{1} ", postNumber, postTitle);
            }

            HtmlAgilityPack.HtmlNode editNode = html.SelectSingleNode("../div[@class='smallfont']/em");
            PostEdit edit = null;
            if (editNode != null)
            {
                String postEdit = HtmlAgilityPack.HtmlEntity.DeEntitize(editNode.InnerText);
                postEdit = postEdit.Trim();
                // Last edited by well named; 09-03-2012 at 08:50 PM. Reason: people who are out will receive the special ******* role
                String regex = @"Last edited by (.*); (.*)\.(?:\s*Reason: (.*))?";
                Match m = Regex.Match(postEdit, regex);
                if (m.Success)
                {
                    String editor = m.Groups[1].Value;
                    String when = m.Groups[2].Value;
                    DateTimeOffset editTime = ParseItemTime(pageTime, when);
                    String editText = null;
                    if (m.Groups.Count > 2)
                    {
                        editText = m.Groups[3].Value.Trim();
                    }
                    edit = new PostEdit(editor, editTime, editText);
                }
            }

            Int32 posterId = -1;
            HtmlAgilityPack.HtmlNode userNode = html.SelectSingleNode("div[@class='postdetails']/div[@class='userinfo']/div[@class='username_container']/div[@class='popupmenu memberaction']/a");
            if (userNode != null)
            {
                posterName = HtmlAgilityPack.HtmlEntity.DeEntitize(userNode.InnerText);
                String profile = userNode.Attributes["href"].Value;
                posterId = Misc.ParseMemberId(profile);
            }
            HtmlAgilityPack.HtmlNode postContent = html.SelectSingleNode("div[@class='postdetails']/div[@class='postbody']/div[1]/div[@class='content']/div/blockquote");
            List<Bold> bolded = ParseBolded(postContent);
            Post p = new Post(threadId, postId, posterName, posterId, postNumber, postTime, postLink, postTitle,
                    postContent.OuterHtml, bolded, edit);
            return p;
        }
        protected override Boolean GetPage(String url, Int32 pageNumber, object o, out Int32 outPages)
        {
            outPages = 0;
            String local = url;
            if (pageNumber > 1)
            {
                local += "/page" + pageNumber;
            }
            string doc = null;
            for (int i = 0; i < 10; i++)
            {
                ConnectionSettings cs = _connectionSettings.Clone();
                cs.Url = local;
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
            if (doc == null)
            {
                OnPageError(url, pageNumber, o);
                return false;
            }
            Posts posts = new Posts();
            Int32 totalPages;
            DateTimeOffset serverTime;
            ParseThreadPage(local, doc, out totalPages, out serverTime, ref posts);
            outPages = totalPages;
            OnPageComplete(local, pageNumber, totalPages, serverTime, posts, o);
            return true;
        }
    }
    public class ThreadReader
    {
        #region fields
        protected readonly ConnectionSettings _connectionSettings;
        Action<Action> _synchronousInvoker;
        protected Misc.ParseItemTimeDelegate ParseItemTime = Misc.ParseItemTimeEnglish;
        protected String _voteRegex;
        protected String _voteColor;
        protected Language _language;
        #endregion
        #region constructors
        public ThreadReader(ConnectionSettings connectionSettings, Action<Action> synchronousInvoker, String voteRegex, String voteColor, Language language)
        {
            _connectionSettings = connectionSettings;
            _synchronousInvoker = synchronousInvoker;
            _voteRegex = voteRegex;
            _voteColor = voteColor;
            _language = language;
            switch (language)
            {
                case Language.English:
                    {
                        ParseItemTime = Misc.ParseItemTimeEnglish;
                    }
                    break;

                case Language.Estonian:
                    {
                        ParseItemTime = Misc.ParseItemTimeEstonia;
                    }
                    break;
            }
        }
        private ThreadReader()
        {
        }
        #endregion
        #region public properties
        #endregion
        #region public methods
        public void ReadPages(String url, Int32 pageStart, Int32 pageEnd, object o)
        {
            Task t = new Task(() => GetPages(url, pageStart, pageEnd, o));
            t.Start();
        }
        #endregion
        #region public events
        public event EventHandler<PageCompleteEventArgs> PageCompleteEvent;
        public event EventHandler<PageErrorEventArgs> PageErrorEvent;
        public event EventHandler<ReadCompleteEventArgs> ReadCompleteEvent;
        #endregion
        #region event helpers
        void OnReadComplete(String url, Int32 pageStart, Int32 pageEnd, object o)
        {
            var handler = ReadCompleteEvent;
            if (handler != null)
            {
                _synchronousInvoker.Invoke(
                    () => handler(this, new ReadCompleteEventArgs(url, pageStart, pageEnd, o))
                );
            }
        }

        protected void OnPageError(String url, int pageNumber, object o)
        {
            try
            {
                var handler = PageErrorEvent;
                if (handler != null)
                {
                    _synchronousInvoker.Invoke(
                        () => handler(this, new PageErrorEventArgs(url, pageNumber, o))
                    );
                }
            }
            catch
            {
            }
        }

        virtual internal void OnPageComplete(String url, Int32 page, Int32 totalPages, DateTimeOffset ts, 
                Posts posts, object o)
        {
            try
            {
                var handler = PageCompleteEvent;
                if (handler != null)
                {
                    _synchronousInvoker.Invoke(
                        () => handler(this, new PageCompleteEventArgs(url, page, totalPages, ts, posts, o))
                    );
                }
            }
            catch
            {
            }
        }
        #endregion
        #region private methods
        void GetPages(String url, Int32 pageStart, Int32 pageEnd, object o)
        {
            object lockOutput = new object();
            Int32? errorPage = null;
            if (pageStart <= pageEnd)
            {
                Int32 totalPages;
                Boolean ok = GetPage(url, pageStart, o, out totalPages);
                if (ok)
                {
                    pageEnd = Math.Min(totalPages, pageEnd);

                    Parallel.For(pageStart + 1, pageEnd + 1, (page, loopState) =>
                    {
                        Int32 outPages;
                        Boolean gotPage = GetPage(url, page, o, out outPages);
                        if (!gotPage)
                        {
                            loopState.Break();
                            lock (lockOutput)
                            {
                                if (errorPage == null)
                                {
                                    errorPage = page;
                                }
                                errorPage = Math.Min(errorPage.Value, page);
                            }
                        }
                    });
                    if (errorPage != null)
                    {
                        pageEnd = errorPage.Value;
                    }
                }
                else
                {
                    pageEnd = pageStart;
                }

            } 
            OnReadComplete(url, pageStart, pageEnd, o);
        }
        protected virtual Boolean GetPage(String url, Int32 pageNumber, object o, out Int32 outPages)
        {
            outPages = 0;
            String local = url;
            if (pageNumber > 1)
            {
                if (url.ToLowerInvariant().Contains("showthread.php?"))
                {
                    local += "&page=" + pageNumber;
                }
                else
                {
                    local += "index" + pageNumber + ".html";
                }
            }
            string doc = null;
            for (int i = 0; i < 10; i++)
            {
                ConnectionSettings cs = _connectionSettings.Clone();
                cs.Url = local;
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
            if (doc == null)
            {
                OnPageError(url, pageNumber, o);
                return false;
            }
            Posts posts = new Posts();
            Int32 totalPages;
            DateTimeOffset serverTime;
            ParseThreadPage(local, doc, out totalPages, out serverTime, ref posts);
            outPages = totalPages;
            OnPageComplete(local, pageNumber, totalPages, serverTime, posts, o);
            return true;
        }

        protected virtual void ParseThreadPage(String url, String doc, out Int32 lastPageNumber, out DateTimeOffset serverTime, ref Posts postList)
        {
            Int32 threadId = VBulletinForum.ThreadIdFromUrl(url);
            lastPageNumber = 0;
            var html = new HtmlAgilityPack.HtmlDocument();
            html.LoadHtml(doc);
            HtmlAgilityPack.HtmlNode root = html.DocumentNode;

            serverTime = DateTime.Now;
            //(//div[class="smallfont", align="center'])[last()] All times are GMT ... The time is now <span class="time">time</span>"."

            HtmlAgilityPack.HtmlNode timeNode = root.SelectNodes("//div[@class='smallfont'][@align='center']/span[@class='time']/..").Last();
            if (timeNode != null)
            {
                String timeText = timeNode.InnerText;
                serverTime = Utils.Misc.ParsePageTime(timeText, DateTime.UtcNow);

            }


            // find total posts: /table/tr[1]/td[2]/div[@class="pagenav"]/table[1]/tr[1]/td[1] -- Page 106 of 106
            HtmlAgilityPack.HtmlNode pageNode = root.SelectSingleNode("//div[@class='pagenav']/table/tr/td");
            if (pageNode != null)
            {
                string pages = pageNode.InnerText;
                Match m = Regex.Match(pages, @"Page (\d+) of (\d+)");
                if (m.Success)
                {
                    //Trace.TraceInformation("{0}/{1}", m.Groups[1].Value, m.Groups[2].Value);
                    lastPageNumber = Convert.ToInt32(m.Groups[2].Value);
                }
            }

            // //div[@id='posts']/div/div/div/div/table/tbody/tr[2]
            // td[1]/div[1] has (id with post #, <a> with user id, user name.)
            // td[2]/div[1] has title
            // td[2]/div[2] has post
            // "/html[1]/body[1]/table[2]/tr[2]/td[1]/td[1]/div[2]/div[1]/div[1]/div[1]/div[1]/table[1]/tr[2]/td[2]/div[2]" is a post
            HtmlAgilityPack.HtmlNodeCollection posts = root.SelectNodes("//div[@id='posts']//div[contains(@id, 'edit')]/table/tr[2]/td[2]/div[contains(@id, 'post_message_')]"); 
            if (posts == null)
            {
                return;
            }
            postList = new Posts();
            foreach (HtmlAgilityPack.HtmlNode post in posts)
            {
                Post p = HtmlToPost(threadId, post, serverTime);
                if (p != null)
                {
                    postList.Add(p);
                }
            }
        }
        protected virtual Post HtmlToPost(Int32 threadId, HtmlAgilityPack.HtmlNode html, DateTimeOffset pageTime)
        {

            string posterName = "";
            Int32 postNumber = 0;
            String postLink = null;
			Int32 postId = -1;
            DateTimeOffset postTime = DateTime.Now;
            HtmlAgilityPack.HtmlNode postNumberNode = html.SelectSingleNode("../../../tr[1]/td[2]/a[last()]");

            if (postNumberNode != null)
            {
                postNumber = Int32.Parse(postNumberNode.InnerText);
                postLink = HtmlAgilityPack.HtmlEntity.DeEntitize(postNumberNode.Attributes["href"].Value);
				if (postLink.Length > 3)
				{
					// showthread.php?12931-Mafia-Convo-Thread&p=316289&viewfull=1#post316289
					String sPost = postLink;
					int startQuery = sPost.IndexOf('?');
					if (startQuery > -1) sPost = sPost.Substring(startQuery);
					sPost = HttpUtility.ParseQueryString(sPost).Get("p");
					Int32.TryParse(sPost, out postId);
					int startPage = postLink.LastIndexOf('/');
					if (startPage > -1) postLink = postLink.Substring(startPage + 1);
				}
			}
            RemoveComments(html);
            HtmlAgilityPack.HtmlNode postTimeNode = html.SelectSingleNode("../../../tr[1]/td[1]");
            if (postTimeNode != null)
            {
                string time = postTimeNode.InnerText.Trim();
                postTime = ParseItemTime(pageTime, time);
                //Trace.TraceInformation("Post time: {0}", postTime.DateTime.ToShortTimeString());
            }
            String postTitle = "";
            HtmlAgilityPack.HtmlNode titleNode = html.SelectSingleNode("../div[@class='smallfont']/strong");
            if (titleNode != null)
            {
                postTitle = HtmlAgilityPack.HtmlEntity.DeEntitize(titleNode.InnerText);
                //Trace.TraceInformation("title[{0}]:{1} ", postNumber, postTitle);
            }

            HtmlAgilityPack.HtmlNode editNode = html.SelectSingleNode("../div[@class='smallfont']/em");
            PostEdit edit = null;
            if (editNode != null)
            {
                String postEdit = HtmlAgilityPack.HtmlEntity.DeEntitize(editNode.InnerText);
                postEdit = postEdit.Trim();
                // Last edited by well named; 09-03-2012 at 08:50 PM. Reason: people who are out will receive the special ******* role
                String regex = @"Last edited by (.*); (.*)\.(?:\s*Reason: (.*))?";
                Match m = Regex.Match(postEdit, regex);
                if (m.Success)
                {
                    String editor = m.Groups[1].Value;
                    String when = m.Groups[2].Value;
                    DateTimeOffset editTime = ParseItemTime(pageTime, when);
                    String editText = null;
                    if (m.Groups.Count > 2)
                    {
                        editText = m.Groups[3].Value.Trim();
                    }
                    edit = new PostEdit(editor, editTime, editText);
                }
            }

            Int32 posterId = -1;
            HtmlAgilityPack.HtmlNode userNode = html.SelectSingleNode("../../td[1]/div/a[starts-with(@class,'bigusername')]");
            if (userNode != null)
            {
                posterName = HtmlAgilityPack.HtmlEntity.DeEntitize(userNode.InnerText);
                String profile = userNode.Attributes["href"].Value;
                posterId = Misc.ParseMemberId(profile);
            }
            List<Bold> bolded = ParseBolded(html);
            Post p = new Post(threadId, postId, posterName, posterId, postNumber, postTime, postLink, postTitle,
                    html.OuterHtml, bolded, edit);
            return p;
        }
        public static void RemoveComments(HtmlAgilityPack.HtmlNode node)
        {
            foreach (var n in node.SelectNodes("//comment()") ?? new HtmlAgilityPack.HtmlNodeCollection(node))
            {
                n.Remove();
            }
        }
        void BoldsFromSet(HtmlAgilityPack.HtmlNodeCollection bolds, List<Bold> bolded)
        {
            if (bolds != null)
            {
                foreach (HtmlAgilityPack.HtmlNode c in bolds)
                {
                    string bold = HtmlAgilityPack.HtmlEntity.DeEntitize(c.InnerText.Trim());
                    if (bold.StartsWith("Votes as of post"))
                    {
                        continue;
                    }
                    if (bold.ToLower() == "in")
                    {
                        continue;
                    }
                    if (_voteRegex.Length > 0)
                    {
                        Match m = Regex.Match(bold, _voteRegex);
                        if (!m.Success) continue;
                        bold = m.Groups[1].Value;
                    }
                    if (bold.Length > 0)
                    {
                        
                        //System.Trace.TraceInformation("{0}\t{1}\t{2}", PostNumber, Poster, bold);
                        Bold b = new Bold(bold);
                        bolded.Add(b);
                    }
                }
            }
        }
        protected List<Bold> ParseBolded(HtmlAgilityPack.HtmlNode original)
        {
            List<Bold> bolded = new List<Bold>();
            HtmlAgilityPack.HtmlNode content = original.CloneNode("Votes", true);
            RemoveQuotes(content); // strip out quotes
            List<String> goodColors = new List<string>() { _voteColor};
                //"darkolivegreen", "darkgreen", "yellowgreen", "seagreen", 
                //"lime", "palegreen", "olive", "green" 
            RemoveColors(content, goodColors); // strip out colors
            RemoveNewlines(content); // strip out newlines

            if (_voteColor == "")
            {
                // look for plain bold
                HtmlAgilityPack.HtmlNodeCollection bolds = content.SelectNodes("child::b");
                if (bolds != null)
                {
                    BoldsFromSet(bolds, bolded);
                }

            }
            else
            {
                // look for color,bold.
                foreach (var n in content.SelectNodes("descendant::font") ?? new HtmlAgilityPack.HtmlNodeCollection(content))
                {
                    HtmlAgilityPack.HtmlNodeCollection colorbolds = n.SelectNodes("child::b");
                    if (colorbolds != null)
                    {
                        BoldsFromSet(colorbolds, bolded);
                    }
                }
                // look for bold,color.
                HtmlAgilityPack.HtmlNodeCollection bolds = content.SelectNodes("descendant::b");
                foreach (var n in bolds ?? new HtmlAgilityPack.HtmlNodeCollection(content))
                {
                    HtmlAgilityPack.HtmlNodeCollection boldcolors = n.SelectNodes("child::font");
                    if (boldcolors != null)
                    {
                        BoldsFromSet(boldcolors, bolded);
                    }
                }
                // look for span w/color
                HtmlAgilityPack.HtmlNodeCollection boldspan = content.SelectNodes("descendant::span[starts-with(@style,\"color:red;font-weight:bold;\")]");
                if (boldspan != null)
                {
                    BoldsFromSet(boldspan, bolded);
                }
            }

            return bolded;
        }
        public static void RemoveQuotes(HtmlAgilityPack.HtmlNode node)
        {
            foreach (var n in node.SelectNodes("descendant::td[@class='alt2']") ?? new HtmlAgilityPack.HtmlNodeCollection(node))
            {
                HtmlAgilityPack.HtmlNode div = n.ParentNode.ParentNode.ParentNode;
                div.Remove();
            }
        }

        public static void RemoveColors(HtmlAgilityPack.HtmlNode node, IEnumerable<String> exemptList)
        {
            foreach (var n in node.SelectNodes("descendant::font") ?? new HtmlAgilityPack.HtmlNodeCollection(node))
            {
                String color = n.GetAttributeValue("color", "nocolor");
                color = color.ToLower();
                if (!exemptList.Contains(color))
                {
                    n.Remove();
                }
            }
        }

        public static void RemoveNewlines(HtmlAgilityPack.HtmlNode node)
        {
            foreach (var n in node.SelectNodes("descendant::br") ?? new HtmlAgilityPack.HtmlNodeCollection(node))
            {
                n.Remove();
            }
        }

        #endregion

    }
}
