using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace POG.Forum
{
    public class NameCompletionEventArgs : EventArgs
    {
        public String Fragment
        {
            get;
            private set;
        }
        public IEnumerable<Poster> Names
        {
            get;
            private set;
        }

        public NameCompletionEventArgs(String fragment, IEnumerable<Poster> names)
        {
            Fragment = fragment;
            Names = names;
        }
    }
}
