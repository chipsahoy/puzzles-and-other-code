using System;
using POG.Werewolf;

namespace FennecWidget
{
	public partial class DayTimes : Gtk.Dialog
	{
		VoteCount _voteCount;
		public DayTimes (VoteCount voteCount)
		{
			_voteCount = voteCount;
			this.Build ();
			txtStartPost.Text = Math.Max(1, _voteCount.StartPost).ToString ();
			DateTime eod = _voteCount.EndTime;
			EodDate.Date = eod;
			spinHour.Value = eod.Hour;
			spinMinute.Value = eod.Minute;
		}
		public void GetDayBoundaries(out Int32 day, out Int32 startPost, out DateTime endTime)
		{
			day = 1;
			startPost = 1;
			Int32.TryParse (txtStartPost.Text, out startPost);
			DateTime eodDate = EodDate.Date;
			endTime = new DateTime(eodDate.Year, eodDate.Month, 
			    eodDate.Day, (Int32)spinHour.Value, (Int32)spinMinute.Value, 0, DateTimeKind.Local);
		}
	}
}

