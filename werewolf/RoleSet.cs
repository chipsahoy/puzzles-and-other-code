﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace POG.Werewolf
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
        [DataMember]
        public string GameURL
        {
            get;
            set;
        }
        [DataMember]
        public string GameName
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

        public List<Player> GetRoster()
        {
            List<Player> roster = new List<Player>();
            for (int k = 0; k < Teams.Count; k++)
            {
                for (int l = 0; l < Teams[k].Members.Count; l++)
                {
                    roster.AddRange(Teams[k].Members[l].Players);
                }
            }
            return roster.OrderBy(o => o.Name).ToList();
        }

        public List<Player> GetAliveRoster()
        {
            List<Player> roster = new List<Player>();
            for (int i = 0; i < Teams.Count; i++)
            {
                for (int j = 0; j < Teams[i].Members.Count; j++)
                {
                    for (int k = 0; k < Teams[i].Members[j].Players.Count; k++)
                    {
                        if(Teams[i].Members[j].Players[k].Alive == true)
                            roster.Add(Teams[i].Members[j].Players[k]);
                    }
                }
            }
            return roster.OrderBy(o => o.Name).ToList();
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
""Color"": ""black"",
          ""SubRole"": """",
          ""ExtraFlavor"": """",
          ""Count"": 6,
          ""n0"": ""no n0 actions""
        },
        {
          ""Players"": [],
          ""RoleNum"": 1,
          ""Role"": ""Seer"",
""Color"": ""purple"",
          ""SubRole"": """",
          ""ExtraFlavor"": """",
          ""Count"": 1,
          ""n0"": ""a random villager peek""
        }
      ],
      ""Name"": ""Villager"",
      ""WinCon"": ""eliminating all wolves"",
      ""Color"": ""green"",
      ""Hidden"": false,
      ""Share"": false
    },
    {
      ""Members"": [
        {
          ""Players"": [],
          ""RoleNum"": 2,
          ""Role"": ""Vanilla"",
""Color"": ""black"",
          ""SubRole"": """",
          ""ExtraFlavor"": """",
          ""Count"": 2,
          ""n0"": ""no n0 actions""
        }
      ],
      ""Name"": ""Wolf"",
      ""WinCon"": ""reaching parity with the village"",
      ""Color"": ""red"",
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
""Color"": ""black"",
          ""n0"": ""no n0 actions""
        },
        {
          ""Players"": [],
          ""RoleNum"": 2,
          ""Role"": ""Seer"",
          ""SubRole"": """",
          ""ExtraFlavor"": """",
          ""Count"": 1,
""Color"": ""purple"",
          ""n0"": ""an n0 action""
        }
      ],
      ""Name"": ""Villager"",
      ""WinCon"": ""eliminating all wolves"",
       ""Color"": ""green"",
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
""Color"": ""black"",
          ""Count"": 3,
          ""n0"": ""no n0 actions""
        }
      ],
      ""Name"": ""Wolf"",
      ""WinCon"": ""reaching parity with the village"",
      ""Color"": ""red"",
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
""Color"": ""black"",
          ""Count"": 12,
          ""n0"": ""no n0 actions""
        },
        {
          ""Players"": [],
          ""RoleNum"": 2,
          ""Role"": ""Seer"",
          ""SubRole"": """",
""Color"": ""purple"",
          ""ExtraFlavor"": """",
          ""Count"": 1,
          ""n0"": ""an n0 action""
        }
      ],
      ""Name"": ""Villager"",
      ""WinCon"": ""eliminating all wolves"",
      ""Hidden"": false,
      ""Color"": ""green"",
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
""Color"": ""black"",
          ""Count"": 4,
          ""n0"": ""no n0 actions""
        }
      ],
      ""Name"": ""Wolf"",
      ""WinCon"": ""reaching parity with the village"",
      ""Hidden"": false,
      ""Color"": ""red"",
      ""Share"": true
    }
  ]
}"
  }
        };
    }
    public class Player
    {
        public Player(string name)
        {
            Name = name;
        }
        [DataMember]
        public string Name
        {
            get;
            set;
        }
        [DataMember]
        public bool Alive
        {
            get;
            set;
        }
    }
    public class RolePM
    {
        public RolePM(string role, string subrole, string color, string extraflavor, string _n0, int count, int rolenum)
        {
            Role = role;
            SubRole = subrole;
            ExtraFlavor = extraflavor;
            Count = count;
            RoleNum = rolenum;
            n0 = _n0;
            Players = new List<Player>();
            Color = color;
        }
        public List<Player> Players
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
        public string Color
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
        public override string ToString()
        {
            return String.Format("{0} {1}", SubRole, Role);
        }
        public string FullPM(string gameURL, RolePMSet gamepms, Team team, Player currentplayer)
        {
            string peek = "";
            string peektype = "";
            if (n0 == "a random villager peek")
            {
                List<Player> villagers = new List<Player>();
                for (int k = 0; k < gamepms.Teams.Count; k++)
                {
                    for (int l = 0; l < gamepms.Teams[k].Members.Count; l++)
                    {
                        if (gamepms.Teams[k].Name == "Villager")
                            villagers.AddRange(gamepms.Teams[k].Members[l].Players);
                    }
                }
                villagers.Remove(currentplayer);
                Random random = new Random();
                int index = random.Next(villagers.Count);
                peek = villagers[index].Name;
                peektype = "villager";
            }
            else if (n0 == "a random peek across entire playerlist")
            {
                Random random = new Random();
                List<Player> roster = gamepms.GetRoster();
                roster.Remove(currentplayer);
                int index = random.Next(roster.Count);
                peek = roster[index].Name;
                peektype = "ERROR";
                for (int k = 0; k < gamepms.Teams.Count; k++)
                {
                    for (int l = 0; l < gamepms.Teams[k].Members.Count; l++)
                    {
                        for (int m = 0; m < gamepms.Teams[k].Members[l].Players.Count; m++)
                        {
                            if (gamepms.Teams[k].Members[l].Players[m].Name == peek)
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
                        teammates += team.Members[i].Players[j].Name + Environment.NewLine;
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
You are {0}on the [b][color={1}]{2}[/color][/b] team. Your role is: [b][color={3}]{4}{5}[/color][/b]! You win by {6}. You have {7}.
{8}{9}
The game thread is here: {10}

Good luck!
*************************************************", extraflavor, team.Color, team.Name, this.Color, subrole, role, team.WinCon, n0, teammates, peek, gameURL);
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
You are {0}on the [b][color={1}]{2}[/color][/b] team. Your role is: [b][color={3}]{4}{5}[/color][/b]! You win by {6}. You have {7}.
{8}{9}
The game thread is here: {10}

Good luck!
*************************************************", extraflavor, team.Color, team.Name, this.Color, subrole, role, team.WinCon, n0, teammates, peek, gameURL);
        }        
    }    
}