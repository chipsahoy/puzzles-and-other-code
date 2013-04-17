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
        public RolePMSet(string name, List<Team> teams)
        {
            Name = name;
            Roles = new List<RolePM>();
            Teams = teams;
        }
        [DataMember]
        public string Name
        {
            get;
            set;
        }
        [DataMember]
        public List<Team> Teams
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
        public Dictionary<string, string> DefaultRoleSets = new Dictionary<string, string>() { 
        {"Vanilla 9'er", @"{
  ""Name"": ""Vanilla9er"",
  ""Teams"": [
    {
      ""Name"": ""Villager"",
      ""WinCon"": ""eliminating all wolves"",
      ""Hidden"": false,
      ""Share"": false
    },
    {
      ""Name"": ""Wolf"",
      ""WinCon"": ""reaching parity with the village"",
      ""Hidden"": false,
      ""Share"": true
    }
  ],
  ""Roles"": [
    {
      ""Players"": [],
      ""TeamRole"": {
        ""Name"": ""Villager"",
        ""WinCon"": ""eliminating all wolves"",
        ""Hidden"": false,
        ""Share"": false
      },
      ""Role"": ""Vanilla"",
      ""SubRole"": """",
      ""ExtraFlavor"": """",
      ""Count"": 6
    },
    {
      ""Players"": [],
      ""TeamRole"": {
        ""Name"": ""Wolf"",
        ""WinCon"": ""reaching parity with the village"",
        ""Hidden"": false,
        ""Share"": true
      },
      ""Role"": ""Vanilla"",
      ""SubRole"": """",
      ""ExtraFlavor"": """",
      ""Count"": 2
    },
    {
      ""Players"": [],
      ""TeamRole"": {
        ""Name"": ""Villager"",
        ""WinCon"": ""eliminating all wolves"",
        ""Hidden"": false,
        ""Share"": false
      },
      ""Role"": ""Seer"",
      ""SubRole"": """",
      ""ExtraFlavor"": """",
      ""Count"": 1
    }
  ]
}"},
  {"Vanilla 13'er", @"{
  ""Name"": ""Vanilla13er"",
  ""Teams"": [
    {
      ""Name"": ""Villager"",
      ""WinCon"": ""eliminating all wolves"",
      ""Hidden"": false,
      ""Share"": false
    },
    {
      ""Name"": ""Wolf"",
      ""WinCon"": ""reaching parity with the village"",
      ""Hidden"": false,
      ""Share"": true
    }
  ],
  ""Roles"": [
    {
      ""Players"": [],
      ""TeamRole"": {
        ""Name"": ""Villager"",
        ""WinCon"": ""eliminating all wolves"",
        ""Hidden"": false,
        ""Share"": false
      },
      ""Role"": ""Vanilla"",
      ""SubRole"": """",
      ""ExtraFlavor"": """",
      ""Count"": 9
    },
    {
      ""Players"": [],
      ""TeamRole"": {
        ""Name"": ""Wolf"",
        ""WinCon"": ""reaching parity with the village"",
        ""Hidden"": false,
        ""Share"": true
      },
      ""Role"": ""Vanilla"",
      ""SubRole"": """",
      ""ExtraFlavor"": """",
      ""Count"": 3
    },
    {
      ""Players"": [],
      ""TeamRole"": {
        ""Name"": ""Villager"",
        ""WinCon"": ""eliminating all wolves"",
        ""Hidden"": false,
        ""Share"": false
      },
      ""Role"": ""Seer"",
      ""SubRole"": """",
      ""ExtraFlavor"": """",
      ""Count"": 1
    }
  ]
}"},
  {"Vanilla 17'er", @"{
  ""Name"": ""Vanilla17er"",
  ""Teams"": [
    {
      ""Name"": ""Villager"",
      ""WinCon"": ""eliminating all wolves"",
      ""Hidden"": false,
      ""Share"": false
    },
    {
      ""Name"": ""Wolf"",
      ""WinCon"": ""reaching parity with the village"",
      ""Hidden"": false,
      ""Share"": true
    }
  ],
  ""Roles"": [
    {
      ""Players"": [],
      ""TeamRole"": {
        ""Name"": ""Villager"",
        ""WinCon"": ""eliminating all wolves"",
        ""Hidden"": false,
        ""Share"": false
      },
      ""Role"": ""Vanilla"",
      ""SubRole"": """",
      ""ExtraFlavor"": """",
      ""Count"": 12
    },
    {
      ""Players"": [],
      ""TeamRole"": {
        ""Name"": ""Wolf"",
        ""WinCon"": ""reaching parity with the village"",
        ""Hidden"": false,
        ""Share"": true
      },
      ""Role"": ""Vanilla"",
      ""SubRole"": """",
      ""ExtraFlavor"": """",
      ""Count"": 4
    },
    {
      ""Players"": [],
      ""TeamRole"": {
        ""Name"": ""Villager"",
        ""WinCon"": ""eliminating all wolves"",
        ""Hidden"": false,
        ""Share"": false
      },
      ""Role"": ""Seer"",
      ""SubRole"": """",
      ""ExtraFlavor"": """",
      ""Count"": 1
    }
  ]
}"
  }
        };
    }
    public class RolePM
    {
        public RolePM(Team team, string role, string subrole, string extraflavor, int count)
        {
            TeamRole = team;
            Role = role;
            SubRole = subrole;
            ExtraFlavor = extraflavor;
            Count = count;
            Players = new List<string>();
        }
        public List<string> Players
        {
            get;
            set;
        }
        [DataMember]
        public Team TeamRole
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
        public int Count
        {
            get;
            set;
        }
        public string FullPM(string gameURL, RolePMSet gamepms, string peek, string peekteam)
        {
            if (peek != "") peek = String.Format("Your n0 random peek is {0}, {1}", peek, peekteam);
            string teammates = "";
            Console.WriteLine(TeamRole.Share);
            if (TeamRole.Share == true)
            {
                Console.WriteLine("HERE");
                teammates += "Your Team is:" + Environment.NewLine;
                for (int i = 0; i < gamepms.Roles.Count; i++)
                {
                    if (TeamRole.Equals(gamepms.Roles[i].TeamRole))
                    {
                        for (int j = 0; j < gamepms.Roles[i].Players.Count; j++)
                        {
                            teammates += gamepms.Roles[i].Players[j] + Environment.NewLine;
                        }
                    }
                }
            }
            Console.WriteLine(peek);
            string subrole = "";
            if (SubRole != "") subrole = SubRole + " ";
            string role = "";
            if (Role != "") role = Role;
            string team = "";
            if (TeamRole.Name != "") team = TeamRole.Name + " ";
            string extraflavor = "";
            if (ExtraFlavor != "") extraflavor = ExtraFlavor + " ";
            return String.Format(@"*************************************************
You are {0}a {1}{2}{3}! You win by {4}.
{5}{6}
The game thread is here: {7}

Good luck!
*************************************************", extraflavor, team, subrole, role, TeamRole.WinCon, teammates, peek, gameURL);
        }

        public string EditedPM(string gameURL)
        {
            string teammates = "";
            if (TeamRole.Share == true)
            {
                teammates += "Your Team is: XXX" + Environment.NewLine;
            }
            string subrole = "";
            if (SubRole != null) subrole = SubRole + " ";
            string role = "";
            if (Role != null) role = Role;
            string team = "";
            if (TeamRole != null) team = TeamRole.Name + " ";
            string extraflavor = "";
            if (ExtraFlavor != "") extraflavor = ExtraFlavor + " ";
            return String.Format(@"*************************************************
You are {0}a {1}{2}{3}! You win by {4}.

The game thread is here: {5}

Good luck!
*************************************************", extraflavor, team, subrole, role, TeamRole.WinCon, gameURL);
        }
        
    }
    
}
