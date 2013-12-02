using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MaryMeerkat
{
    public class Suit
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
        public Boolean Doubled
        {

            get;
            set;
        }
        public Suit(Int32 id, Int32 type, Int32 owner, Boolean doubled)
        {
            Id = id;
            Type = type;
            Owner = owner;
            Doubled = doubled;
        }
    }
}
