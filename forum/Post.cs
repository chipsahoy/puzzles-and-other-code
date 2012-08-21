using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace POG.Forum
{
    public class Bold
    {
        public Bold(String content)
        {
            Content = content;
        }
        public String Content
        {
            get;
            private set;
        }
        public Boolean Ignore
        {
            get;
            set;
        }
    }
    public class Post
    {
        HtmlAgilityPack.HtmlNode _content;
        List<Bold> _bolded;

        public Post(Int32 threadId, String poster, Int32 postNumber, DateTime ts, String postLink, HtmlAgilityPack.HtmlNode content)
        {
            Poster = poster;
            PostNumber = postNumber;
            Time = ts;
            PostLink = postLink;
            _content = content;
            ThreadId = threadId;
            ParseBolded();
            int ixPostStart = postLink.LastIndexOf("?p=") + 3;
            string sPost = postLink.Substring(ixPostStart);
            int ixPostLast = sPost.IndexOf('&');
            sPost = sPost.Substring(0, ixPostLast);
            Int32 postId = -1;
            Int32.TryParse(sPost, out postId);
            PostId = postId;
        }
        public string Poster
        {
            get;
            private set;
        }
        public Int32 PostId
        {
            get;
            private set;
        }
        public Int32 ThreadId
        {
            get;
            private set;
        }
        public String Content
        {
            get
            {
                return _content.OuterHtml;
            }
        }
        public String Title
        {
            get;
            private set;
        }

        public Int32 PostNumber
        {
            get;
            private set;
        }
        public string PostLink
        {
            get;
            private set;
        }
        public DateTime Time
        {
            get;
            private set;
        }
        public IEnumerable<Bold> Bolded
        {
            get
            {
                ParseBolded();
                return _bolded;
            }
        }

        private void ParseBolded()
        {
            if (_bolded != null)
            {
                return;
            }
            _bolded = new List<Bold>();
            HtmlAgilityPack.HtmlNode content = _content.CloneNode("Votes", true);
            RemoveQuotes(content); // strip out quotes
            RemoveColors(content); // strip out colors
            RemoveNewlines(content); // strip out newlines

            HtmlAgilityPack.HtmlNodeCollection bolds = content.SelectNodes("child::b");

            if (bolds != null)
            {
                foreach (HtmlAgilityPack.HtmlNode c in bolds)
                {
                    string bold = HtmlAgilityPack.HtmlEntity.DeEntitize(c.InnerText.Trim());
                    if(bold.StartsWith("Votes as of post"))
                    {
                        continue;
                    }
					if (bold.ToLower() == "in")
					{
						continue;
					}
                    if (bold.Length > 0)
                    {
                        System.Console.WriteLine("{0}\t{1}\t{2}", PostNumber, Poster, bold);
                        _bolded.Add(new Bold(bold));
                    }
                }
            }
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
    }
    public class Posts : KeyedCollection<int, Post>
    {
        public Posts() :
            base()
        {
        }

        protected override int GetKeyForItem(Post item)
        {
            return item.PostNumber;
        }
        public virtual Post GetByIndex(Int32 index)
        {
            return Items[index];
        }

        public void AddRange(IEnumerable<Post> posts)
        {
            foreach (Post p in posts)
            {
                Add(p);
            }
        }
    }
}
