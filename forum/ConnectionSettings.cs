using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace POG.Forum
{
     public class ConnectionSettings
    {
         private Uri _forum;
         private CookieContainer _cc;
         public ConnectionSettings(String forum)
             : this(new Uri(forum))
        {
        }
        public ConnectionSettings(Uri baseUri)
        {
            _forum = baseUri;
            FollowRedirect = true;
            UseAuthentication = false;
            IgnoreErrors = false;
            Headers = new Dictionary<string, string>();
            PostData = new Dictionary<string, string>();
            UseUnsafeAuthenticatedConnectionSharing = false;
        }
        public ConnectionSettings Clone()
        {
            ConnectionSettings rc = new ConnectionSettings(_forum);
            CookieCollection cookies = CC.GetCookies(_forum);
            rc.CC.Add(cookies);
            return rc;
        }

        public String Data { get; set; }
        public String Message { get; set; }
        public String Url { get; set; }
        public bool FollowRedirect { get; set; }
        public bool UseAuthentication { get; set; }
        public bool IgnoreErrors { get; set; }
        public bool UseUnsafeAuthenticatedConnectionSharing { get; set; }
        public Dictionary<String, String> Headers { get; set; }
        public Dictionary<String, String> PostData { get; private set; }
        public CookieContainer CC 
        {
            get
            {
                if (_cc == null)
                {
                    _cc = new CookieContainer();
                }
                return _cc;
            }
            set 
            {
                _cc = value;
            } 
        }

    }
}
