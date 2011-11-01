using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FennecFox
{
    public class VoteComparer : IComparer<KeyValuePair<String, List<String>>>
    {
        // put unvote and not voting last
        private static String UNVOTE = "unvote";
        private static String NOT_VOTING = "not voting";

        /// <summary>
        /// Compares the two vote candidates.  Not Voting goes last always, followed by Unvote.
        /// After that, values are arranged in descending order from most votes to least.  Ties are alphabetically ordered.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public int Compare(KeyValuePair<String, List<String>> x, KeyValuePair<String, List<String>> y)
        {
            if (x.Key == y.Key) return 0;

            if (x.Key == NOT_VOTING) return 1;
            if (y.Key == NOT_VOTING) return -1;

            if (x.Key == UNVOTE) return 1;
            if (y.Key == UNVOTE) return -1;

            if(y.Value.Count == x.Value.Count)
            {
                return x.Key.CompareTo(y.Key);
            }

            return y.Value.Count.CompareTo(x.Value.Count);
        }
    }
}
