using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace POG.Forum
{
    public class ReadCompleteEventArgs : EventArgs
    {
        public string URL
        {
            get;
            private set;
        }
        public int pageStart
        {
            get;
            private set;
        }
        public int pageEnd
        {
            get;
            private set;
        }
        public object Cookie
        {
            get;
            private set;
        }

        public ReadCompleteEventArgs(string url, int pageStart, int pageEnd, object o)
        {
            this.URL = url;
            this.pageStart = pageStart;
            this.pageEnd = pageEnd;
            this.Cookie = o;
        }
    }
}
