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
        virtual internal void OnPageComplete(String url, Int32 page, Int32 totalPages, DateTime ts, Posts posts)
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
            DateTime ts;
            ParseThreadPage(local, doc, out totalPages, out ts, ref posts);
            OnPageComplete(local, pageNumber, totalPages, ts, posts);
        }
        private void ParseThreadPage(String url, String doc, out Int32 lastPageNumber, out DateTime ts, ref Posts postList)
        {
            ts = DateTime.Now;
            Int32 threadId = TwoPlusTwoForum.ThreadIdFromUrl(url);
            lastPageNumber = 0;
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
                Post p = HtmlToPost(threadId, post);
                if (p != null)
                {
                    postList.Add(p);
                }
            }
        }
        private Post HtmlToPost(Int32 threadId, HtmlAgilityPack.HtmlNode html)
        {

            string posterName = "";
            Int32 postNumber = 0;
            String postLink = null;
            DateTime postTime = DateTime.Now;
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
            Post p = new Post(threadId, posterName, postNumber, postTime, postLink, postTitle, html, postEdit);
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

    }
}
