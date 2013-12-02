using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MaryMeerkat
{
    public class Gun
    {
        public Int32 Id
        {
            get;
            private set;
        }
        public Int32 Type
        {
            get;
            private set;
        }
        public Int32 Owner
        {
            get;
            set;
        }
        public Gun(Int32 id, Int32 type, Int32 owner)
        {
            Id = id;
            Type = type;
            Owner = owner;
        }
    }
}
