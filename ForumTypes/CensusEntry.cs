using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace POG.Forum
{
    public class CensusEntry
    {
        public CensusEntry()
        {
            Alive = "Alive";
            Name = String.Empty;
        }

        public String Name
        {
            get;
            set;
        }
        public String Alive
        {
            get;
            set;
        }
        public DateTime? EndPostTime
        {
            get;
            set;
        }
        public String Replacement
        {
            get;
            set;
        }
    }
}
