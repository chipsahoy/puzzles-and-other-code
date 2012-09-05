using System;
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

namespace POG.Forum
{
    public class ThreadReader
    {
        #region fields
        readonly ConnectionSettings _connectionSettings;
        Action<Action> _synchronousInvoker;
        #endregion
        #region constructors
        public ThreadReader(ConnectionSettings connectionSettings, Action<Action> synchronousInvoker)
        {
            _connectionSettings = connectionSettings;
            _synchronousInvoker = synchronousInvoker;
        }
        private ThreadReader()
        {
        }
        #endregion
        #region public properties
        public object Tag
        {
            get;
            set;
        }
        #endregion
        #region public methods
        public void ReadPosts(String url, Int32 pageStart, Int32 pageEnd)
        {
            Task t = new Task(() => GetPages(url, pageStart, pageEnd));
            t.Start();
        }
        #endregion
        #region public events
        public event EventHandler<PageCompleteEventArgs> PageCompleteEvent;
        public event EventHandler<PostEventArgs> PostEvent;
        #endregion
        #region event helpers
        virtual internal void OnPageComplete(String url, Int32 page, Int32 totalPages, DateTimeOffset ts, Posts posts)
        {
            try
            {
                var handler = PageCompleteEvent;
                if (handler != null)
                {
                    _synchronousInvoker.Invoke(
                        () => handler(this, new PageCompleteEventArgs(url, page, totalPages, ts, posts))
                    );
                }
            }
            catch
            {
            }
        }
        virtual internal void OnPostEvent(String url, Post post)
        {
            try
            {
                var handler = PostEvent;
                if (handler != null)
                {
                    handler(this, new PostEventArgs(url, post));
                }
            }
            catch
            {
            }
        }
        #endregion
        #region private methods
        void GetPages(String url, Int32 pageStart, Int32 pageEnd)
        {
            Parallel.For(pageStart, pageEnd + 1, (Int32 page) => { GetPage(url, page); }); 
        }
        void GetPage(String url, Int32 pageNumber)
        {
            String local = url;
            if (pageNumber > 1)
            {
                local += "index" + pageNumber + ".html";
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
                    Console.WriteLine("*** Error fetching page " + pageNumber.ToString());
                }
            }
            Posts posts = new Posts();
            if (doc == null)
            {
                OnPageComplete(local, pageNumber, pageNumber - 1, DateTime.Now, posts);
                return;
            }
            Int32 totalPages;
            DateTimeOffset serverTime;
            ParseThreadPage(local, doc, out totalPages, out serverTime, ref posts);
            OnPageComplete(local, pageNumber, totalPages, serverTime, posts);
        }
        private void ParseThreadPage(String url, String doc, out Int32 lastPageNumber, out DateTimeOffset serverTime, ref Posts postList)
        {
            Int32 threadId = TwoPlusTwoForum.ThreadIdFromUrl(url);
            lastPageNumber = 0;
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

            
            // find total posts: /table/tr[1]/td[2]/div[@class="pagenav"]/table[1]/tr[1]/td[1] -- Page 106 of 106
            HtmlAgilityPack.HtmlNode pageNode = root.SelectSingleNode("//div[@class='pagenav']/table/tr/td");
            if (pageNode != null)
            {
                string pages = pageNode.InnerText;
                Match m = Regex.Match(pages, @"Page (\d+) of (\d+)");
                if (m.Success)
                {
                    //Console.WriteLine("{0}/{1}", m.Groups[1].Value, m.Groups[2].Value);
                    lastPageNumber = Convert.ToInt32(m.Groups[2].Value);
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
        private Post HtmlToPost(Int32 threadId, HtmlAgilityPack.HtmlNode html, DateTimeOffset pageTime)
        {

            string posterName = "";
            Int32 postNumber = 0;
            String postLink = null;
            DateTimeOffset postTime = DateTime.Now;
            HtmlAgilityPack.HtmlNode postNumberNode = html.SelectSingleNode("../../../tr[1]/td[2]/a");

            if (postNumberNode != null)
            {
                postNumber = Int32.Parse(postNumberNode.InnerText);
                postLink = postNumberNode.Attributes["href"].Value;
            }
            RemoveComments(html);
            HtmlAgilityPack.HtmlNode postTimeNode = html.SelectSingleNode("../../../tr[1]/td[1]");
            if (postTimeNode != null)
            {
                string time = postTimeNode.InnerText.Trim();
                postTime = Misc.ParseItemTime(pageTime, time);
                Console.WriteLine("Post time: {0}", postTime.DateTime.ToShortTimeString());
            }
            String postTitle = "";
            HtmlAgilityPack.HtmlNode titleNode = html.SelectSingleNode("../div[@class='smallfont']/strong");
            if (titleNode != null)
            {
                postTitle = HtmlAgilityPack.HtmlEntity.DeEntitize(titleNode.InnerText);
                //Console.WriteLine("title[{0}]:{1} ", postNumber, postTitle);
            }

            String postEdit = "";
            HtmlAgilityPack.HtmlNode editNode = html.SelectSingleNode("../div[@class='smallfont']/em");
            if (editNode != null)
            {
                postEdit = HtmlAgilityPack.HtmlEntity.DeEntitize(editNode.InnerText);
                postEdit = postEdit.Trim();
                Console.WriteLine("edit[{0}]:{1}", postNumber, postEdit);
            }

            HtmlAgilityPack.HtmlNode userNode = html.SelectSingleNode("../../td[1]/div/a[@class='bigusername']");
            if (userNode != null)
            {
                posterName = HtmlAgilityPack.HtmlEntity.DeEntitize(userNode.InnerText);
            }
            List<Bold> bolded = ParseBolded(html);
            Post p = new Post(threadId, posterName, postNumber, postTime, postLink, postTitle, 
                    html.OuterHtml, postEdit, bolded);
            return p;
        }
        private void RemoveComments(HtmlAgilityPack.HtmlNode node)
        {
            foreach (var n in node.SelectNodes("//comment()") ?? new HtmlAgilityPack.HtmlNodeCollection(node))
            {
                n.Remove();
            }
        }
        private List<Bold> ParseBolded(HtmlAgilityPack.HtmlNode original)
        {
            List<Bold> bolded = new List<Bold>();
            HtmlAgilityPack.HtmlNode content = original.CloneNode("Votes", true);
            RemoveQuotes(content); // strip out quotes
            RemoveColors(content); // strip out colors
            RemoveNewlines(content); // strip out newlines

            HtmlAgilityPack.HtmlNodeCollection bolds = content.SelectNodes("child::b");

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
                    if (bold.Length > 0)
                    {
                        //System.Console.WriteLine("{0}\t{1}\t{2}", PostNumber, Poster, bold);
                        bolded.Add(new Bold(bold));
                    }
                }
            }
            return bolded;
        }
        static void RemoveQuotes(HtmlAgilityPack.HtmlNode node)
        {
            foreach (var n in node.SelectNodes("div/table/tbody/tr/td[@class='alt2']") ?? new HtmlAgilityPack.HtmlNodeCollection(node))
            {
                HtmlAgilityPack.HtmlNode div = n.ParentNode.ParentNode.ParentNode.ParentNode;
                div.Remove();
            }
        }

        static void RemoveColors(HtmlAgilityPack.HtmlNode node)
        {
            foreach (var n in node.SelectNodes("//font") ?? new HtmlAgilityPack.HtmlNodeCollection(node))
            {
                n.Remove();
            }
        }

        static void RemoveNewlines(HtmlAgilityPack.HtmlNode node)
        {
            foreach (var n in node.SelectNodes("//br") ?? new HtmlAgilityPack.HtmlNodeCollection(node))
            {
                n.Remove();
            }
        }

        #endregion

    }
}
