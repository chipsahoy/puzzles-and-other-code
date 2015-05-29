#!/usr/bin/env python
# -*- coding: utf-8 -*-import datetime
import re
import datetime
import pytz
__author__ = 'Paul'

def parsepagetime(timestring, now):
    sMatchTZ = r"All times are GMT ([\+\-]\d+(?:\.5)?)\. The time now is (\d\d:\d\d\s[AP]M)"
    sHourFormat = "%I:%M %p"
    if "K�ik ajad" in timestring:
         sMatchTZ = r"K�ik ajad on GMT ([\+\-]\d+(?:\.5)?)\. Kell on praegu (\d\d:\d\d)"
         sHourFormat = "%H:%M"
    tzOffset = 0
    rc = datetime.datetime.utcnow()
    print(sMatchTZ, timestring)
    match = re.search(sMatchTZ, timestring)
    if match:
        tzOffset = int(match.group(1)) * 60
    timeServer = match.group(2)
    rawTime = datetime.strptime(timeServer, sHourFormat)
    tzTime = datetime.timedelta(minutes=tzOffset)
    print(rawTime, tzTime)
    #     DateTimeOffset guess = new DateTimeOffset(rawTime, tzTime);
    #     TimeSpan tsCheck = utcNow - guess.UtcDateTime;
    #     rc = guess;
    #     if (tsCheck.TotalHours > 12)
    #     {
    #         rc = guess.AddDays(1);
    #     }
    #     if (tsCheck.TotalHours < -12)
    #     {
    #         rc = guess.AddDays(-1);
    #     }
    # }
    # //Trace.TraceInformation("Server Time: {0}", rc.DateTime.ToShortTimeString());
    # return rc;


def parseitemtime(pagetime, itemtime):
    #// 09-04-2012 at 11:03 AM
    #// 04-12-2012 07:02 PM
    #// 04-12-2012, 02:10 PM
    #// 2nd February 2014, 05:48 AM
    months = ["January", "February", "March", "April", "May", "June", "July", "August",
        "September", "October", "November", "December"]
    longDate = False;
    for month in months:
        if month in itemtime:
            longDate = True

    dateFormat = "MM-dd-yyyy"
    if longDate:
        dateFormat = "dd MMMMM yyyy"
    # today = datetime.strptime(pagetime, dateFormat)
    # dtYesterday = pagetime.DateTime - new TimeSpan(1, 0, 0, 0);
    # yesterday = dtYesterday.ToString(dateFormat);
    # time = time.Replace("Today", today);
    # time = time.Replace("Yesterday", yesterday);
    # time = time.Replace(",", String.Empty);
    # time = time.Replace("at ", String.Empty);
    # time = time.Replace(".", String.Empty);
    #
		# 	var culture = Thread.CurrentThread.CurrentCulture;
    #         try
    #         {
    #             Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("en-US");
    #             if (longDate)
    #             {
    #                 // 2nd February 2014, 05:48 AM
    #                 time = time.Replace("August", "Augus").Replace("nd ", " ").Replace("st ", " ").Replace("th ", " ").
    #                     Replace("rd ", " ").Replace("Augus", "August");
    #                 rc = new DateTimeOffset(DateTime.ParseExact(time, "d MMMM yyyy hh:mm tt", null), pageTime.Offset);
    #             }
    #             else
    #             {
    #                 rc = new DateTimeOffset(DateTime.ParseExact(time, "MM-d-yyyy hh:mm tt", null, DateTimeStyles.AllowWhiteSpaces), pageTime.Offset);
    #             }
    #         }
    #         finally
    #         {
    #             Thread.CurrentThread.CurrentCulture = culture;
    #         }
		# 	rc = rc.ToUniversalTime();
		# 	return rc;
