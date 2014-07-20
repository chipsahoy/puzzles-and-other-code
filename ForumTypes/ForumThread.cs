using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace POG.Forum
{
    [DataContract()]
    public class ForumThread
    {
        [DataMember]
        public string ThreadIconText { get; set; }

        [DataMember]
        public string URL { get; set; }
        [DataMember]
        public string Title { get; set; }
        [DataMember]
        public Poster OP { get; set; }
        [DataMember]
        public Poster LastPoster { get; set; }
        [DataMember]
        public DateTimeOffset LastPostTime { get; set; }
        [DataMember]
        public int ReplyCount { get; set; }
        [DataMember]
        public int Views { get; set; }
        [DataMember]
        public bool Locked { get; set; }
        [DataMember]
        public Int32 ThreadId { get; set; }
    }
}
