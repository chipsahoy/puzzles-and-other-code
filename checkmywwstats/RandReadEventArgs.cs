using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace POG.Database
{

    public class RandReadEventArgs : EventArgs
    {
        public RandReadEventArgs(String playerlist, String gamename, String username, String password, Int32 signupthreadid)
        {
            GameName = gamename;
            Playerlist = playerlist;
            Username = username;
            Password = password;
            SignUpThreadID = signupthreadid;
        }

        public string GameName { get; private set; }

        public string Username { get; private set; }

        public string Password { get; private set; }

        public string Playerlist { get; private set; }

        public Int32 SignUpThreadID { get; private set; }
    }
}
