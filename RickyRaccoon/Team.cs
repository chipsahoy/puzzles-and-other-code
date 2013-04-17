using System;
using System.Runtime.Serialization;

namespace RickyRaccoon
{
    [DataContract()]
    public class Team
    {
        public Team(string name, string wincon, bool hidden, bool share)
        {
            Name = name;
            WinCon = wincon;
            Hidden = hidden;
            Share = share;
        }
        [DataMember]
        public string Name
        {
            get;
            set;
        }
        [DataMember]
        public string WinCon
        {
            get;
            set;
        }
        [DataMember]
        public bool Hidden
        {
            get;
            set;
        }
        [DataMember]
        public bool Share
        {
            get;
            set;
        }
        [DataMember]
        public Team Self
        {
            get
            {
                return this;
            }
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
