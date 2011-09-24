

namespace FennecFox
{
    public class Vote
    {
        public Vote(string postNumber, string content)
        {
            Ignore = false;
            Content = content;
            PostNumber = postNumber;
        }
        public string PostNumber
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
