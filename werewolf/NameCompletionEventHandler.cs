using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace POG.Werewolf
{
    public class NameCompletionEventArgs : EventArgs
    {
        public String Fragment
        {
            get;
            private set;
        }
        public IEnumerable<string> Names
        {
            get;
            private set;
        }

        public NameCompletionEventArgs(String fragment, IEnumerable<string> names)
        {
            Fragment = fragment;
            Names = names;
        }
    }
}
