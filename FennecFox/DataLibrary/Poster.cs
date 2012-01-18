using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FennecFox.DataLibrary
{
    class Poster : IEnumerable<Vote>
    {
        public void AddVote(Vote v)
        {
        }
        public void HideVote(Vote v)
        {
        }
        public void UnhideVote(Vote v)
        {
        }
        public Vote NewestBold
        {
            get
            {
                Vote v = null;
                return v;
            }
        }
        public Vote ActiveVote
        {
            get
            {
                Vote v = null;
                return v;
            }
        }

        public IEnumerator<Vote> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
