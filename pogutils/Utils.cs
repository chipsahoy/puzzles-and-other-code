using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pogutils
{
    public class Utils
    {
        public static String NormalizeUrl(String url)
        {
            if (url == null)
            {
                url = String.Empty;
            }
            url = url.Trim();
            if (url.Length > 0)
            {
                if (url.EndsWith(".html") || url.EndsWith(".htm"))
                {
                    url = url.Substring(0, url.LastIndexOf("index"));
                }

                if (!url.EndsWith("/"))
                {
                    url += "/";
                }
            }
            return url;
        }
    }
}
