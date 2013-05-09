using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;

namespace POG.Utils
{
    public class PogSettings
    {
        static PogSettings()
        {
        }

        public static void Write(String name, String value)
        {
        }
        public static void Write(String name, StringCollection value)
        {
        }
        public static String Read(String name, String defaultValue = "")
        {
            return "";
        }
        public static void Read(String name, out StringCollection value)
        {
            StringCollection rc = new StringCollection();
            value = rc;
        }
    }
}
