using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace POG.Forum
{
    public class PageErrorEventArgs : EventArgs
    {

        public PageErrorEventArgs(string url, int pageNumber, object o)
        {
        }
    }
}
