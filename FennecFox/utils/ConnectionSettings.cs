using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace FennecFox
{
     public class ConnectionSettings
    {
        public ConnectionSettings()
        {
            FollowRedirect = true;
            UseAuthentication = false;
            IgnoreErrors = false;
            Headers = new Dictionary<string, string>();
            UseUnsafeAuthenticatedConnectionSharing = false;
            CC = new CookieContainer();
        }

        public String Data { get; set; }
        public String Message { get; set; }
        public String Url { get; set; }
        public bool FollowRedirect { get; set; }
        public bool UseAuthentication { get; set; }
        public bool IgnoreErrors { get; set; }
        public bool UseUnsafeAuthenticatedConnectionSharing { get; set; }
        public Dictionary<String, String> Headers { get; set; }
        public CookieContainer CC { get; set; }
    }
}
