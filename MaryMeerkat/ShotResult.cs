using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MaryMeerkat
{
    class ShotResult
    {
        public GunSuitType Gun
        {
            get;
            private set;
        }
        public GunSuitType Suit
        {
            get;
            private set;
        }
        public Boolean Doubled
        {
            get;
            private set;
        }
        public String Result
        {
            get;
            private set;
        }
        public ShotResult(GunSuitType gun, GunSuitType suit, Boolean doubled, String result)
        {
            Gun = gun;
            Suit = suit;
            Doubled = doubled;
            Result = result;
        }
    }
}
