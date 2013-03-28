using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Globalization;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace POG.Utils
{
    public class Misc
    {
        public static Int32 TidFromURL(String url)
        {
            int ixTidStart = url.LastIndexOf('-') + 1;
            string tid = url.Substring(ixTidStart, url.Length - (ixTidStart + 1));
            Int32 rc = 0;
            Int32.TryParse(tid, out rc);
            return rc;
        }

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
        public static DateTimeOffset ParsePageTime(String pageTime, DateTime utcNow)
        {
            Int32 tzOffset = 0;
            DateTimeOffset rc = DateTime.UtcNow;
            Match m = Regex.Match(pageTime, @"All times are GMT ([\+\-]\d+(?:\.5)?)\. The time now is (\d\d:\d\d\s[AP]M)");
            if (m.Success)
            {
                tzOffset = (Int32)(decimal.Parse(m.Groups[1].Value) * 60); //offset in minutes
                String timeServer = m.Groups[2].Value;
                //Trace.TraceInformation("{0}/{1}", m.Groups[1].Value, m.Groups[2].Value);
                DateTime rawTime; 
                var culture = Thread.CurrentThread.CurrentCulture;
                try
                {
                    Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("en-US");
                    rawTime = DateTime.SpecifyKind(DateTime.ParseExact(timeServer, "hh:mm tt", null), DateTimeKind.Unspecified);
                }
                finally
                {
                    Thread.CurrentThread.CurrentCulture = culture;
                }
                TimeSpan tzTime = new TimeSpan(0, tzOffset, 0);
                DateTimeOffset guess = new DateTimeOffset(rawTime, tzTime);
                TimeSpan tsCheck = utcNow - guess.UtcDateTime;
                rc = guess;
                if (tsCheck.TotalHours > 12)
                {
                    rc = guess.AddDays(1);
                }
                if (tsCheck.TotalHours < -12)
                {
                    rc = guess.AddDays(-1);
                }
            }
            //Trace.TraceInformation("Server Time: {0}", rc.DateTime.ToShortTimeString());
            return rc;
        }
        public static DateTimeOffset ParseItemTime(DateTimeOffset pageTime, String time)
        {
            // 09-04-2012 at 11:03 AM
            // 04-12-2012 07:02 PM
            // 04-12-2012, 02:10 PM

            DateTimeOffset rc;
            string today = pageTime.ToString("MM-dd-yyyy");
            time = time.Replace("Today", today);
            DateTime dtYesterday = pageTime.DateTime - new TimeSpan(1, 0, 0, 0);
            string yesterday = dtYesterday.ToString("MM-dd-yyyy");
            time = time.Replace("Yesterday", yesterday);
            time = time.Replace(",", String.Empty);
            time = time.Replace("at ", String.Empty);
            var culture = Thread.CurrentThread.CurrentCulture;
            try
            {
                Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("en-US");
                rc = new DateTimeOffset(DateTime.ParseExact(time, "MM-dd-yyyy hh:mm tt", null), pageTime.Offset);
            }
            finally
            {
                Thread.CurrentThread.CurrentCulture = culture;
            }
            rc = rc.ToUniversalTime();
            return rc;
        }
        public static Int32 ParseMemberId(String profileUrl)
        {
            Int32 posterId = -1;
            Match m = Regex.Match(profileUrl, @".*/members/(\d*)/");
            if (m.Success)
            {
                String sId = m.Groups[1].Value;
                if (sId != String.Empty)
                {
                    posterId = Int32.Parse(sId);
                }
            }
            return posterId;
        }
    }

}
