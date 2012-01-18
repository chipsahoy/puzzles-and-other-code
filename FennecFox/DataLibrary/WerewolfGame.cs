using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FennecFox.DataLibrary
{
    class WerewolfGame : IEnumerable<Poster>
    {
        public Int32 DayNumber
        {
            get;
            private set;
        }
        public Boolean IsDay
        {
            get;
            set;
        }
        public Boolean IsNight
        {
            get;
            set;
        }
        public Poster this[Int32 ix]
        {
            get
            {
                Poster p = null;
                return p;
            }

        }
        public Poster this[string name]
        {
            get
            {
                Poster p = null;
                return p;
            }

        }
        public string PostableVoteCount
        {
            get
            {
                string s = "";
                return s;
            }
        }
        public void AddPlayer(string name)
        {
        }
        public void KillPlayer(string name)
        {
        }
        public void SubPlayer(string oldPlayer, string newPlayer)
        {
        }

        public IEnumerator<Poster> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
