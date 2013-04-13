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
        public Dictionary<string, string> DefaultRoleSets = new Dictionary<string, string>() { 
        {"Vanilla 9'er", @"{
  ""Name"": ""Vanilla 9er"",
  ""Roles"": [
    {
      ""DefaultRoleSets"": {
        ""Vanilla 9er"": """"
      },
      ""Team"": ""Wolf"",
      ""Role"": """",
      ""SubRole"": """",
      ""ExtraFlavor"": """",
      ""WinCon"": """",
      ""Count"": 2
    },
    {
      ""DefaultRoleSets"": {
        ""Vanilla 9er"": """"
      },
      ""Team"": ""Villager"",
      ""Role"": ""Seer"",
      ""SubRole"": ""Full"",
      ""ExtraFlavor"": """",
      ""WinCon"": """",
      ""Count"": 1
    },
    {
      ""DefaultRoleSets"": {
        ""Vanilla 9er"": """"
      },
      ""Team"": ""Villager"",
      ""Role"": ""Vanilla"",
      ""SubRole"": """",
      ""ExtraFlavor"": """",
      ""WinCon"": """",
      ""Count"": 6
    }
  ]
}"},
  {"Vanilla 13'er", @"{
  ""Name"": ""Vanilla 13'er"",
  ""Roles"": [
    {
      ""DefaultRoleSets"": {
        ""Vanilla 9'er"": """"
      },
      ""Team"": ""Wolf"",
      ""Role"": """",
      ""SubRole"": """",
      ""ExtraFlavor"": """",
      ""WinCon"": """",
      ""Count"": 3
    },
    {
      ""DefaultRoleSets"": {
        ""Vanilla 9'er"": """"
      },
      ""Team"": ""Villager"",
      ""Role"": ""Seer"",
      ""SubRole"": ""Full"",
      ""ExtraFlavor"": """",
      ""WinCon"": """",
      ""Count"": 1
    },
    {
      ""DefaultRoleSets"": {
        ""Vanilla 9'er"": """"
      },
      ""Team"": ""Villager"",
      ""Role"": ""Vanilla"",
      ""SubRole"": """",
      ""ExtraFlavor"": """",
      ""WinCon"": """",
      ""Count"": 9
    }
  ]
}"},
  {"Vanilla 17'er", @"{
  ""Name"": ""Vanilla 17'er"",
  ""Roles"": [
    {
      ""DefaultRoleSets"": {
        ""Vanilla 9'er"": """"
      },
      ""Team"": ""Wolf"",
      ""Role"": """",
      ""SubRole"": """",
      ""ExtraFlavor"": """",
      ""WinCon"": """",
      ""Count"": 4
    },
    {
      ""DefaultRoleSets"": {
        ""Vanilla 9'er"": """"
      },
      ""Team"": ""Villager"",
      ""Role"": ""Seer"",
      ""SubRole"": ""Full"",
      ""ExtraFlavor"": """",
      ""WinCon"": """",
      ""Count"": 1
    },
    {
      ""DefaultRoleSets"": {
        ""Vanilla 9'er"": """"
      },
      ""Team"": ""Villager"",
      ""Role"": ""Vanilla"",
      ""SubRole"": """",
      ""ExtraFlavor"": """",
      ""WinCon"": """",
      ""Count"": 12
    }
  ]
}"
  }
        };
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
