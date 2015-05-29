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
		[ThreadStatic] static Random _random;
		public static Random RNG
		{
			get
			{
				if (_random == null)
				{
					_random = new Random(unchecked(Environment.TickCount * 31 + Thread.CurrentThread.ManagedThreadId));
				}
				return _random;
			}
		}
		public static Int32 RandomItemFromRange(Int32 minValue, Int32 maxValue)
		{
			Int32 rc = _random.Next(minValue, maxValue);
			return rc;
		}
		public static T RandomItemFromList<T>(IEnumerable<T> seq)
		{
			List<T> list = seq.ToList();
			Int32 ix = RNG.Next(list.Count);
			return list[ix];
		}

		public delegate DateTimeOffset ParseItemTimeDelegate(DateTimeOffset pageTime, String date);
		public static Int32 TidFromURL(String url)
		{
            int ixTidStart;
            int length;
            if (url.Contains('-'))
            {
                ixTidStart = url.LastIndexOf('-') + 1;
                length = url.Length - (ixTidStart + 1);
            }
            else
            {
                ixTidStart = url.LastIndexOf('=') + 1;
                length = url.Length - ixTidStart;
            }
			if (length < 0) return -1;
			string tid = url.Substring(ixTidStart, length);
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

				if (!url.EndsWith("/") && !url.Contains('?'))
				{
					url += "/";
				}
			}
			return url;
		}
		public static DateTimeOffset ParsePageTime(String pageTime, DateTime utcNow)
		{
			String sMatchTZ = @"All times are GMT ([\+\-]\d+(?:\.5)?)\. The time now is (\d\d:\d\d\s[AP]M)";
			String sHourFormat = "hh:mm tt";
			if (pageTime.Contains("K�ik ajad"))
			{
				sMatchTZ = @"K�ik ajad on GMT ([\+\-]\d+(?:\.5)?)\. Kell on praegu (\d\d:\d\d)";
				sHourFormat = "HH:mm";
			}
			Int32 tzOffset = 0;
			DateTimeOffset rc = DateTime.UtcNow;
			Match m = Regex.Match(pageTime, sMatchTZ);
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
					rawTime = DateTime.SpecifyKind(DateTime.ParseExact(timeServer, sHourFormat, null), DateTimeKind.Unspecified);
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
		public static DateTimeOffset ParseItemTimeEnglish(DateTimeOffset pageTime, String time)
		{
			// 09-04-2012 at 11:03 AM
			// 04-12-2012 07:02 PM
			// 04-12-2012, 02:10 PM
            // 2nd February 2014, 05:48 AM
            List<String> months = new List<string>() {"January", "February", "March", "April", "May", "June", "July", "August",
                "September", "October", "November", "December"};
            Boolean longDate = false;
            foreach (var month in months)
            {
                if (time.Contains(month))
                {
                    longDate = true;
                    break;
                }
            }
            DateTimeOffset rc;
            String dateFormat = "MM-dd-yyyy";
            if (longDate) dateFormat = "dd MMMMM yyyy";
            string today = pageTime.ToString(dateFormat);
            DateTime dtYesterday = pageTime.DateTime - new TimeSpan(1, 0, 0, 0);
            string yesterday = dtYesterday.ToString(dateFormat);
            time = time.Replace("Today", today);
            time = time.Replace("Yesterday", yesterday);
            time = time.Replace(",", String.Empty);
            time = time.Replace("at ", String.Empty);
            time = time.Replace(".", String.Empty);

			var culture = Thread.CurrentThread.CurrentCulture;
            try
            {
                Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("en-US");
                if (longDate)
                {
                    // 2nd February 2014, 05:48 AM
                    time = time.Replace("August", "Augus").Replace("nd ", " ").Replace("st ", " ").Replace("th ", " ").
                        Replace("rd ", " ").Replace("Augus", "August");
                    rc = new DateTimeOffset(DateTime.ParseExact(time, "d MMMM yyyy hh:mm tt", null), pageTime.Offset);
                }
                else
                {
                    rc = new DateTimeOffset(DateTime.ParseExact(time, "MM-d-yyyy hh:mm tt", null, DateTimeStyles.AllowWhiteSpaces), pageTime.Offset);
                }
            }
            finally
            {
                Thread.CurrentThread.CurrentCulture = culture;
            }
			rc = rc.ToUniversalTime();
			return rc;
		}
		public static DateTimeOffset ParseItemTimeEstonia(DateTimeOffset pageTime, String time)
		{
			// 09-04-2012 at 11:03 AM
			// 04-12-2012 07:02 PM
			// 04-12-2012, 02:10 PM

			DateTimeOffset rc;
			string today = pageTime.ToString("dd-MM-yy");
			time = time.Replace("T�na", today);
			DateTime dtYesterday = pageTime.DateTime - new TimeSpan(1, 0, 0, 0);
			string yesterday = dtYesterday.ToString("dd-MM-yy");
			time = time.Replace("Eile", yesterday);
			time = time.Replace(",", String.Empty);
			time = time.Replace("at ", String.Empty);
			time = time.Replace('.', '-');
			time = time.Replace(' ', ' '); // $nbsp; looks like space but isn't!
			var culture = Thread.CurrentThread.CurrentCulture;
			try
			{
				Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("en-US");
				rc = new DateTimeOffset(DateTime.ParseExact(time, "dd-MM-yy HH:mm", null), pageTime.Offset);
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
            // "member.php?u=545"
			Int32 posterId = -1;
            Int32 ixU = profileUrl.IndexOf("?u=");
            if (ixU >= 0)
            {
                String uid = profileUrl.Substring(ixU + "?u=".Length);
                if (uid.Contains('\'')) uid =  uid.Substring(0, uid.IndexOf('\''));
                Int32.TryParse(uid, out posterId);
                return posterId;
            }
			Match m = Regex.Match(profileUrl, @".*/members/(\d*)/");
			if (m.Success)
			{
				String sId = m.Groups[1].Value;
				if (sId != String.Empty)
				{
					posterId = Int32.Parse(sId);
				}
			}
			else // Bestonia
			{
				m = Regex.Match(profileUrl, @".*member.php[/\?](\d*)-");
				if (m.Success)
				{
					String sId = m.Groups[1].Value;
					if (sId != String.Empty)
					{
						posterId = Int32.Parse(sId);
					}
				}
			}
			return posterId;
		}
	}

}
