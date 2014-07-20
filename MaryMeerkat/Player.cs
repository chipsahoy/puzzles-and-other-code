using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MaryMeerkat
{
    public class Player
    {
        public Int32 Id
        {
            get;
            private set;
        }
        public String Name
        {
            get;
            private set;
        }
        public String Team
        {
            get;
            private set;
        }
        public Boolean Dead
        {
            get;
            set;
        }
        public Boolean HasAttacked
        {
            get;
            set;
        }
        public Boolean Silenced
        {
            get;
            set;
        }
        public Boolean Discombobulated
        {
            get;
            set;
        }
        public Boolean ForceField
        {
            get;
            set;
        }
        public Int32? ArmedGun
        {
            get;
            set;
        }
        public Player(Int32 id, String name, String team, Boolean dead, Boolean hasAttacked, Boolean silenced, Boolean discombobulated,
            Boolean forceField, Int32? armedGun)
        {
            Id = id;
            Name = name;
            Team = team;
            Dead = dead;
            HasAttacked = hasAttacked;
            Silenced = silenced;
            Discombobulated = discombobulated;
            ForceField = forceField;
            ArmedGun = armedGun;
        }
    }
}
