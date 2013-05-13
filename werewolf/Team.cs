using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace POG.Werewolf
{
    [DataContract()]
    public class Team
    {
        public Team(string name, string wincon, bool share, string color)
        {
            Name = name;
            WinCon = wincon;
            Share = share;
            Members = new List<RolePM>();
            Color = color;
        }
        [DataMember]
        public List<RolePM> Members
        {
            get;
            set;
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
        [DataMember]
        public string Color
        {
            get;
            set;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
