

using System;

namespace FennecFox
{
    public class Vote
    {
        public Vote(Int32 postNumber, string content)
        {
            Ignore = false;
            Content = content;
            PostNumber = postNumber;
        }
        public Int32 PostNumber
        {
            get;
            private set;
        }
        public string Content
        {
            get;
            private set;
        }
        public bool Ignore
        {
            get;
            set;
        }
    }
}
