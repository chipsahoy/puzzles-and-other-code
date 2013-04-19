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
        public void setRolePM(RolePM role, int rolenum)
        {
            for (int i = 0; i < Teams.Count; i++)
            {
                for (int j = 0; j < Teams[i].Members.Count; j++)
                {
                    if (Teams[i].Members[j].RoleNum == rolenum)
                    {
                        Teams[i].Members[j] = role;
                        return;
                    }
                }
            }
            for (int i = 0; i < Teams.Count; i++)
            {
                if (Teams[i].Equals(role.TeamRole))
                {
                    Teams[i].Members.Add(role);
                }
            }
        }
        public void removeRolePM(int rolenum)
        {
            for (int i = 0; i < Teams.Count; i++)
            {
                for (int j = 0; j < Teams[i].Members.Count; j++)
                {
                    if (Teams[i].Members[j].RoleNum == rolenum)
                    {
                        Teams[i].Members.RemoveAt(j);
                    }
                    else if (Teams[i].Members[j].RoleNum > rolenum)
                    {
                        Teams[i].Members[j].RoleNum--;
                    }
                }
            }
        }

        public Dictionary<string, string> DefaultRoleSets = new Dictionary<string, string>() { 
        {"Vanilla 9'er", @"{
  ""Name"": ""Vanilla 9'er"",
  ""Teams"": [
    {
      ""Members"": [
        {
          ""Players"": [],
          ""RoleNum"": 0,
          ""Role"": ""Vanilla"",
          ""SubRole"": """",
          ""ExtraFlavor"": """",
          ""Count"": 6
        },
        {
          ""Players"": [],
          ""RoleNum"": 2,
          ""Role"": ""Seer"",
          ""SubRole"": """",
          ""ExtraFlavor"": """",
          ""Count"": 1
        }
      ],
      ""Name"": ""Villager"",
      ""WinCon"": ""eliminate all wolves"",
      ""Hidden"": false,
      ""Share"": false
    },
    {
      ""Members"": [
        {
          ""Players"": [],
          ""RoleNum"": 1,
          ""Role"": ""Vanilla"",
          ""SubRole"": """",
          ""ExtraFlavor"": """",
          ""Count"": 2
        }
      ],
      ""Name"": ""Wolf"",
      ""WinCon"": ""reach parity with the village"",
      ""Hidden"": false,
      ""Share"": true
    }
  ]
}"},
  {"Vanilla 13'er", @"{
  ""Name"": ""Vanilla 13'er"",
  ""Teams"": [
    {
      ""Members"": [
        {
          ""Players"": [],
          ""RoleNum"": 0,
          ""Role"": ""Vanilla"",
          ""SubRole"": """",
          ""ExtraFlavor"": """",
          ""Count"": 9
        },
        {
          ""Players"": [],
          ""TeamRole"": {
            ""Members"": [
              {
                ""Players"": [],
                ""RoleNum"": 1,
                ""Role"": ""Seer"",
                ""SubRole"": """",
                ""ExtraFlavor"": """",
                ""Count"": 1
              }
            ],
            ""Name"": ""Wolf"",
            ""WinCon"": ""reach parity with the village"",
            ""Hidden"": false,
            ""Share"": true
          },
          ""RoleNum"": 2,
          ""Role"": ""Vanilla"",
          ""SubRole"": """",
          ""ExtraFlavor"": """",
          ""Count"": 3
        }
      ],
      ""Name"": ""Villager"",
      ""WinCon"": ""eliminate all wolves"",
      ""Hidden"": false,
      ""Share"": false
    },
    {
      ""Members"": [
        {
          ""Players"": [],
          ""TeamRole"": {
            ""Members"": [
              {
                ""Players"": [],
                ""RoleNum"": 0,
                ""Role"": ""Vanilla"",
                ""SubRole"": """",
                ""ExtraFlavor"": """",
                ""Count"": 9
              },
              {
                ""Players"": [],
                ""RoleNum"": 2,
                ""Role"": ""Vanilla"",
                ""SubRole"": """",
                ""ExtraFlavor"": """",
                ""Count"": 3
              }
            ],
            ""Name"": ""Villager"",
            ""WinCon"": ""eliminate all wolves"",
            ""Hidden"": false,
            ""Share"": false
          },
          ""RoleNum"": 1,
          ""Role"": ""Seer"",
          ""SubRole"": """",
          ""ExtraFlavor"": """",
          ""Count"": 1
        }
      ],
      ""Name"": ""Wolf"",
      ""WinCon"": ""reach parity with the village"",
      ""Hidden"": false,
      ""Share"": true
    }
  ]
}"},
  {"Vanilla 17'er", @"{
  ""Name"": ""Vanilla 17'er"",
  ""Teams"": [
    {
      ""Members"": [
        {
          ""Players"": [],
          ""RoleNum"": 0,
          ""Role"": ""Vanilla"",
          ""SubRole"": """",
          ""ExtraFlavor"": """",
          ""Count"": 12
        },
        {
          ""Players"": [],
          ""TeamRole"": {
            ""Members"": [
              {
                ""Players"": [],
                ""RoleNum"": 1,
                ""Role"": ""Seer"",
                ""SubRole"": """",
                ""ExtraFlavor"": """",
                ""Count"": 1
              }
            ],
            ""Name"": ""Wolf"",
            ""WinCon"": ""reach parity with the village"",
            ""Hidden"": false,
            ""Share"": true
          },
          ""RoleNum"": 2,
          ""Role"": ""Vanilla"",
          ""SubRole"": """",
          ""ExtraFlavor"": """",
          ""Count"": 4
        }
      ],
      ""Name"": ""Villager"",
      ""WinCon"": ""eliminate all wolves"",
      ""Hidden"": false,
      ""Share"": false
    },
    {
      ""Members"": [
        {
          ""Players"": [],
          ""TeamRole"": {
            ""Members"": [
              {
                ""Players"": [],
                ""RoleNum"": 0,
                ""Role"": ""Vanilla"",
                ""SubRole"": """",
                ""ExtraFlavor"": """",
                ""Count"": 12
              },
              {
                ""Players"": [],
                ""RoleNum"": 2,
                ""Role"": ""Vanilla"",
                ""SubRole"": """",
                ""ExtraFlavor"": """",
                ""Count"": 4
              }
            ],
            ""Name"": ""Villager"",
            ""WinCon"": ""eliminate all wolves"",
            ""Hidden"": false,
            ""Share"": false
          },
          ""RoleNum"": 1,
          ""Role"": ""Seer"",
          ""SubRole"": """",
          ""ExtraFlavor"": """",
          ""Count"": 1
        }
      ],
      ""Name"": ""Wolf"",
      ""WinCon"": ""reach parity with the village"",
      ""Hidden"": false,
      ""Share"": true
    }
  ]
}"
  }
        };
    }
    public class RolePM
    {
        public RolePM(Team team, string role, string subrole, string extraflavor, int count, int rolenum)
        {
            TeamRole = team;
            Role = role;
            SubRole = subrole;
            ExtraFlavor = extraflavor;
            Count = count;
            RoleNum = rolenum;
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
        public int RoleNum
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
                for (int i = 0; i < TeamRole.Members.Count; i++)
                {
                    teammates += TeamRole.Members[i] + Environment.NewLine;
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
