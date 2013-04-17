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
      ""Hidden"": false
    },
    {
      ""Name"": ""Wolf"",
      ""WinCon"": ""reaching parity with the village"",
      ""Hidden"": false
    }
  ],
  ""Roles"": [
    {
      ""TeamRole"": {
        ""Name"": ""Villager"",
        ""WinCon"": ""eliminating all wolves"",
        ""Hidden"": false
      },
      ""Role"": ""Vanilla"",
      ""SubRole"": """",
      ""ExtraFlavor"": """",
      ""Count"": 6
    },
    {
      ""TeamRole"": {
        ""Name"": ""Wolf"",
        ""WinCon"": ""reaching parity with the village"",
        ""Hidden"": false
      },
      ""Role"": ""Vanilla"",
      ""SubRole"": """",
      ""ExtraFlavor"": """",
      ""Count"": 2
    },
    {
      ""TeamRole"": {
        ""Name"": ""Villager"",
        ""WinCon"": ""eliminating all wolves"",
        ""Hidden"": false
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
      ""Hidden"": false
    },
    {
      ""Name"": ""Wolf"",
      ""WinCon"": ""reaching parity with the village"",
      ""Hidden"": false
    }
  ],
  ""Roles"": [
    {
      ""TeamRole"": {
        ""Name"": ""Villager"",
        ""WinCon"": ""eliminating all wolves"",
        ""Hidden"": false
      },
      ""Role"": ""Vanilla"",
      ""SubRole"": """",
      ""ExtraFlavor"": """",
      ""Count"": 9
    },
    {
      ""TeamRole"": {
        ""Name"": ""Wolf"",
        ""WinCon"": ""reaching parity with the village"",
        ""Hidden"": false
      },
      ""Role"": ""Vanilla"",
      ""SubRole"": """",
      ""ExtraFlavor"": """",
      ""Count"": 3
    },
    {
      ""TeamRole"": {
        ""Name"": ""Villager"",
        ""WinCon"": ""eliminating all wolves"",
        ""Hidden"": false
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
      ""Hidden"": false
    },
    {
      ""Name"": ""Wolf"",
      ""WinCon"": ""reaching parity with the village"",
      ""Hidden"": false
    }
  ],
  ""Roles"": [
    {
      ""TeamRole"": {
        ""Name"": ""Villager"",
        ""WinCon"": ""eliminating all wolves"",
        ""Hidden"": false
      },
      ""Role"": ""Vanilla"",
      ""SubRole"": """",
      ""ExtraFlavor"": """",
      ""Count"": 12
    },
    {
      ""TeamRole"": {
        ""Name"": ""Wolf"",
        ""WinCon"": ""reaching parity with the village"",
        ""Hidden"": false
      },
      ""Role"": ""Vanilla"",
      ""SubRole"": """",
      ""ExtraFlavor"": """",
      ""Count"": 4
    },
    {
      ""TeamRole"": {
        ""Name"": ""Villager"",
        ""WinCon"": ""eliminating all wolves"",
        ""Hidden"": false
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
        public string FullPM(string gameURL)
        {
            string subrole = "";
            if (SubRole != null) subrole = SubRole + " ";
            string role = "";
            if (SubRole != null) role = Role;
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
