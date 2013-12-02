using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MaryMeerkat
{
    public class CorralGunSuitType
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
        public CorralGunSuitType(Int32 id, String name)
        {
            Id = id;
            Name = name;
        }
    }
}
