using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Titan
{
    class Player
    {
        public Int32 Id { get; private set; }
        public Boolean Dead { get; set; }
        public String Name { get; set; }
        public String Team { get; private set; }
        public String Role { get; private set; }
        public Int32 VotesFor { get; set; }
        public Player(Int32 id, Boolean dead, String name, String team, String role)
        {
            Id = id;
            Dead = dead;
            Name = name;
            Team = team;
            Role = role;
        }
    }
}
