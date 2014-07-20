using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace POG.Database
{

    public class LobbyReadEventArgs : EventArgs
    {
        public LobbyReadEventArgs(String url, Int32 first, Int32 last, Boolean recentFirst)
        {
            URL = url;
            First = first;
            Last = last;
            RecentFirst = recentFirst;
        }

        public bool RecentFirst { get; private set; }

        public int Last { get; private set; }

        public int First { get; private set; }

        public string URL { get; private set; }
    }
}
