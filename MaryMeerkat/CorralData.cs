using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MaryMeerkat
{

    public enum GunSuitType
    {
        missing = 0,
        flamethrower = 1,
        icegun,
        sniperrifle,
        stfugun,
        discombobulator,
        godivagun,
        xraygun,
        q36,
        forcefieldgun,
        Martiansuicidevest,
        Icesuit,
        Magmasuit,
        Fiberglasssuit,
        Rubbersuit,
        Invisiblesuit,
        Magneticsuit,
        Mirrorsuit,
        Randomizersuit,
        boobytrapsuit,
        BugsBunnysuit,
    }
    public class CorralData
    {
        public List<Player> Players
        {
            get;
            private set;
        }
        public List<Gun> Guns
        {
            get;
            private set;
        }
        public List<Suit> Suits
        {
            get;
            private set;
        }
        public String Url
        {
            get;
            private set;
        }
        public String Username
        {
            get;
            private set;
        }
        public Int32 LastPostRead
        {
            get;
            set;
        }
        public DateTimeOffset StartOfDay
        {
            get;
            set;
        }
        public DateTimeOffset EndOfDay
        {
            get;
            set;
        }
        public CorralData()
        {
            Players = new List<Player>();
            Guns = new List<Gun>();
            Suits = new List<Suit>();
            Url = "http://forumserver.twoplustwo.com/59/puzzles-other-games/return-pog-corral-1390183/";
            Username = "TimeLady";
            LastPostRead = 1457;
            StartOfDay = new DateTime(2013, 11, 18, 5, 0, 0, DateTimeKind.Local);
            EndOfDay = new DateTime(2013, 11, 18, 17, 0, 0, DateTimeKind.Local);
        }
        public String TypeToString(Int32 type)
        {
            String rc = "[error]";
            String[] types = new String[] {
"NO ITEM",
"flamethrower",
"ice gun",
"sniper rifle",
"stfu gun",
"discombobulator",
"godiva gun",
"xray gun",
"q36",
"force field gun",
"Martian suicide vest",
"Ice suit",
"Magma suit",
"Fiberglass suit",
"Rubber suit",
"Invisible suit",
"Magnetic suit",
"Mirror suit",
"Randomizer suit",
"boobytrap suit",
"Bugs Bunny suit",
            };
            if (type < types.Length)
            {
                rc = types[type];
            }
            return rc;
        }

    }
}
