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
        public void setRolePM(RolePM role, Team team, int rolenum)
        {
            for (int i = 0; i < Teams.Count; i++)
            {
                for (int j = 0; j < Teams[i].Members.Count; j++)
                {
                    if (Teams[i].Members[j].RoleNum == rolenum)
                    {
                        Teams[i].Members.RemoveAt(j);
                    }
                }
            }
            for (int i = 0; i < Teams.Count; i++)
            {
                if (Teams[i].Equals(team))
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
          ""Count"": 6,
          ""n0"": ""no n0 actions""
        },
        {
          ""Players"": [],
          ""RoleNum"": 1,
          ""Role"": ""Seer"",
          ""SubRole"": """",
          ""ExtraFlavor"": """",
          ""Count"": 1,
          ""n0"": ""a random villager peek""
        }
      ],
      ""Name"": ""Villager"",
      ""WinCon"": ""eliminating all wolves"",
      ""Hidden"": false,
      ""Share"": false
    },
    {
      ""Members"": [
        {
          ""Players"": [],
          ""RoleNum"": 2,
          ""Role"": ""Vanilla"",
          ""SubRole"": """",
          ""ExtraFlavor"": """",
          ""Count"": 2,
          ""n0"": ""no n0 actions""
        }
      ],
      ""Name"": ""Wolf"",
      ""WinCon"": ""reaching parity with the village"",
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
          ""Count"": 9,
          ""n0"": ""no n0 actions""
        },
        {
          ""Players"": [],
          ""RoleNum"": 2,
          ""Role"": ""Seer"",
          ""SubRole"": """",
          ""ExtraFlavor"": """",
          ""Count"": 1,
          ""n0"": ""an n0 action""
        }
      ],
      ""Name"": ""Villager"",
      ""WinCon"": ""eliminating all wolves"",
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
          ""Count"": 3,
          ""n0"": ""no n0 actions""
        }
      ],
      ""Name"": ""Wolf"",
      ""WinCon"": ""reaching parity with the village"",
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
          ""Count"": 12,
          ""n0"": ""no n0 actions""
        },
        {
          ""Players"": [],
          ""RoleNum"": 2,
          ""Role"": ""Seer"",
          ""SubRole"": """",
          ""ExtraFlavor"": """",
          ""Count"": 1,
          ""n0"": ""an n0 action""
        }
      ],
      ""Name"": ""Villager"",
      ""WinCon"": ""eliminating all wolves"",
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
          ""Count"": 4,
          ""n0"": ""no n0 actions""
        }
      ],
      ""Name"": ""Wolf"",
      ""WinCon"": ""reaching parity with the village"",
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
        public RolePM(string role, string subrole, string extraflavor, string _n0, int count, int rolenum)
        {
            Role = role;
            SubRole = subrole;
            ExtraFlavor = extraflavor;
            Count = count;
            RoleNum = rolenum;
            n0 = _n0;
            Players = new List<string>();
        }
        public List<string> Players
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
        public string n0
        {
            get;
            set;
        }
        public string FullPM(string gameURL, RolePMSet gamepms, Team team, string playername)
        {
            string peek = "";
            string peektype = "";
            if (n0 == "a random villager peek")
            {
                List<string> villagers = new List<string>();
                for (int k = 0; k < gamepms.Teams.Count; k++)
                {
                    for (int l = 0; l < gamepms.Teams[k].Members.Count; l++)
                    {
                        if (gamepms.Teams[k].Name == "Villager")
                            villagers.AddRange(gamepms.Teams[k].Members[l].Players);
                    }
                }
                villagers.Remove(playername);
                Random random = new Random();
                int index = random.Next(villagers.Count);
                peek = villagers[index];
                peektype = "villager";
            }
            else if (n0 == "a random peek across entire playerlist")
            {
                Random random = new Random();
                List<string> roster = new List<string>();
                for (int k = 0; k < gamepms.Teams.Count; k++)
                {
                    for (int l = 0; l < gamepms.Teams[k].Members.Count; l++)
                    {
                        roster.AddRange(gamepms.Teams[k].Members[l].Players);
                    }
                }
                roster.Remove(playername);
                int index = random.Next(roster.Count);
                peek = roster[index];
                peektype = "ERROR";
                for (int k = 0; k < gamepms.Teams.Count; k++)
                {
                    for (int l = 0; l < gamepms.Teams[k].Members.Count; l++)
                    {
                        for (int m = 0; m < gamepms.Teams[k].Members[l].Players.Count; m++)
                        {
                            if (gamepms.Teams[k].Members[l].Players[m] == peek)
                                peektype = gamepms.Teams[k].Name;
                        }
                    }
                }
            }
            if (peek != "") peek = String.Format("Your n0 random peek is {0}, {1}", peek, peektype);
            string teammates = "";
            if (team.Share == true)
            {
                teammates += "Your Team is:" + Environment.NewLine;
                for (int i = 0; i < team.Members.Count; i++)
                {
                    for (int j = 0; j < team.Members[i].Count; j++)
                    {
                        teammates += team.Members[i].Players[j] + Environment.NewLine;
                    }
                }
            }
            Console.WriteLine(teammates);
            string subrole = "";
            if (SubRole != "") subrole = SubRole + " ";
            string role = "";
            if (Role != "") role = Role;
            string teamname = "";
            if (team.Name != "") teamname = team.Name + " ";
            string extraflavor = "";
            if (ExtraFlavor != "") extraflavor = ExtraFlavor + " ";
            return String.Format(@"*************************************************
You are {0}on the {1} team. Your role is: {2}{3}! You win by {4}. You have {5}.
{6}{7}
The game thread is here: {8}

Good luck!
*************************************************", extraflavor, team.Name, subrole, role, team.WinCon, n0, teammates, peek, gameURL);
        }

        public string EditedPM(string gameURL, Team team)
        {
            string peek = "";
            if (n0 == "a random villager peek")
            {
                peek = "Your n0 random peek is XXX" + Environment.NewLine;
            }
            string teammates = "";
            if (team.Share == true)
            {
                teammates += "Your Team is: XXX" + Environment.NewLine;
            }
            string subrole = "";
            if (SubRole != null) subrole = SubRole + " ";
            string role = "";
            if (Role != null) role = Role;
            string extraflavor = "";
            if (ExtraFlavor != "") extraflavor = ExtraFlavor + " ";
            return String.Format(@"*************************************************
You are {0}on the {1} team. Your role is: {2}{3}! You win by {4}. You have {5}.
{6}{7}
The game thread is here: {6}

Good luck!
*************************************************", extraflavor, team.Name, subrole, role, team.WinCon, n0, teammates, peek, gameURL);
        }
        
    }
    
}
