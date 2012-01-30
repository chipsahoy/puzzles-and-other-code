using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FennecFox.DataLibrary
{
    class Bold
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
    class Post
    {
        HtmlAgilityPack.HtmlNode _content;
        List<Bold> _bolded;

        public Post(String poster, Int32 postNumber, DateTime ts, String postLink, HtmlAgilityPack.HtmlNode content)
        {
            Poster = poster;
            PostNumber = postNumber;
            Time = ts;
            PostLink = postLink;
            _content = content;
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

            RemoveQuotes(_content); // strip out quotes
            RemoveColors(_content); // strip out colors
            RemoveNewlines(_content); // strip out newlines

            HtmlAgilityPack.HtmlNodeCollection bolds = _content.SelectNodes("child::b");

            if (bolds != null)
            {
                foreach (HtmlAgilityPack.HtmlNode c in bolds)
                {
                    string bold = HtmlAgilityPack.HtmlEntity.DeEntitize(c.InnerText.Trim());
                    if (bold.Length > 0)
                    {
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
}
