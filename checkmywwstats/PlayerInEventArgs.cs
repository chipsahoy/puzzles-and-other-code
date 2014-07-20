using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace POG.Database
{

    public class PlayerInEventArgs : EventArgs
    {
        public PlayerInEventArgs(String playername, Boolean ingame)
        {
            PlayerName = playername;
            InGame = ingame;
        }

        public string PlayerName { get; private set; }

        public Boolean InGame { get; private set; }
    }
}
