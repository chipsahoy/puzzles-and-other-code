using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace POG.Forum
{
    public class PageErrorEventArgs : EventArgs
    {
        private string url;
        private int pageNumber;
        private object o;

        public PageErrorEventArgs(string url, int pageNumber, object o)
        {
            this.url = url;
            this.pageNumber = pageNumber;
            this.o = o;
        }
    }
}
