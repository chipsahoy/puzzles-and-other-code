using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace POG.Database
{
    public class ThreadReadEventArgs : EventArgs
    {
        public string URL
        {
            get;
            private set;
        }
        public int StartPost
        {
            get;
            private set;
        }
        public int EndPost
        {
            get;
            private set;
        }
        public Boolean FoundLastPage
        {
            get;
            set;
        }

        public ThreadReadEventArgs(string url, int startPost, int endPost)
        {
            // TODO: Complete member initialization
            this.URL = url;
            this.StartPost = startPost;
            this.EndPost = endPost;
        }
    }
}
