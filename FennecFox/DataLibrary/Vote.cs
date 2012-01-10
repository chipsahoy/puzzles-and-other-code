

using System;

namespace FennecFox
{
    public class Vote
    {
        public Vote()
        {
            Ignore = false;
        }
        /*
        public Vote(Int32 postNumber, string content)
        {
            Ignore = false;
            Content = content;
            PostNumber = postNumber;
        }*/

        public Vote(Int32 postNumber, String voter, string content, String postLink, DateTime time)
        {
            Ignore = false;
            Content = content;
            PostNumber = postNumber;
            PostLink = postLink;
            Voter = voter;
            Time = time;
        }

        public Int32 PostNumber { get; private set; }

        public string Voter { get; private set; }

        public string Content { get; private set; }

        public bool Ignore { get; set; }

        public String PostLink { get; private set; }

        public DateTime Time { get; private set; }
    }
}
