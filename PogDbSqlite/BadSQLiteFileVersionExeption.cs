using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace POG.Werewolf
{
    public class BadSQLiteFileVersionException : Exception
    {
        public Int32 FileVersion
        {
            get;
            private set;
        }
        public Int32 SchemaVersion
        {
            get;
            private set;
        }

        internal BadSQLiteFileVersionException(int dbVersion, int schemaVersion)
        {
            FileVersion = dbVersion;
            SchemaVersion = schemaVersion;
        }
    }
}
