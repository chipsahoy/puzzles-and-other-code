using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace RickyRaccoon
{
    [DataContract()]
    public class RolePMSet
    {
        public RolePMSet(string name)
        {
            Name = name;
            Roles = new List<RolePM>();
        }
        [DataMember]
        public string Name
        {
            get;
            set;
        }
        [DataMember]
        public List<RolePM> Roles
        {
            get;
            private set;
        }
    }
    public class RolePM
    {
        public RolePM(string team, string role, string subrole, string extraflavor, string wincon, int count)
        {
            Team = team;
            Role = role;
            SubRole = subrole;
            ExtraFlavor = extraflavor;
            WinCon = wincon;
            Count = count;
        }

        [DataMember]
        public string Team
        {
            get;
            set;
        }
        [DataMember]
        public string Role
        {
            get;
            set;
        }
        [DataMember]
        public string SubRole
        {
            get;
            set;
        }
        [DataMember]
        public string ExtraFlavor
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
        public int Count
        {
            get;
            set;
        }
        public string FullPM(string gameURL)
        {
            return String.Format(@"*************************************************
You are {0} a {1} {2} {3}! You win by {4}.

The game thread is here: {5}

Good luck!
*************************************************", ExtraFlavor, Team, SubRole, Role, WinCon, gameURL);
        }
    }
}
