using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace POG.Forum
{
    public class ForumThread
    {
        public string ThreadIconText { get; set; }

        public string URL { get; set; }

        public string Title { get; set; }

        public string OP { get; set; }

        public string LastPoster { get; set; }

        public DateTime LastPostTime { get; set; }

        public int ReplyCount { get; set; }

        public int Views { get; set; }
    }
}
