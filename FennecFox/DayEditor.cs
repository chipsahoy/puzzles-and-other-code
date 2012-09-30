using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace POG.FennecFox
{
    public partial class DayEditor : Form
    {
        private Werewolf.VoteCount _voteCount;

        public DayEditor()
        {
            InitializeComponent();
        }

        public DayEditor(Werewolf.VoteCount _voteCount)
        {
            this._voteCount = _voteCount;
            InitializeComponent();
            dtEodTime.Format = DateTimePickerFormat.Custom;
            dtEodTime.CustomFormat = Application.CurrentCulture.DateTimeFormat.ShortTimePattern;

            udDay.Value = _voteCount.Day;
            udStartPost.Value = Math.Max(1, _voteCount.StartPost);
            DateTime eod = _voteCount.EndTime;
            dtEodDate.Value = eod;
            dtEodTime.Value = eod;

        }
        public void GetDayBoundaries(out Int32 day, out Int32 startPost, out DateTime endTime)
        {
            day = (Int32)udDay.Value;
            startPost = (Int32)udStartPost.Value;
            DateTime eodDate = dtEodDate.Value;
            DateTime eodTime = dtEodTime.Value;
            endTime = new DateTime(eodDate.Year, eodDate.Month, eodDate.Day, 
                    eodTime.Hour, eodTime.Minute, 0, DateTimeKind.Local);
        }

        private void udDay_ValueChanged(object sender, EventArgs e)
        {
            if (_voteCount != null)
            {
                Int32 day = (Int32)udDay.Value;
                DateTime startPost;
                DateTime endTime;
                Int32 endPost;
                if (_voteCount.GetDayBoundaries(day, out startPost, out endTime, out endPost))
                {
                    //udStartPost.Value = startPost;
                    dtEodDate.Value = endTime;
                    dtEodTime.Value = endTime;
                }
                else
                {
                    // try to load previous day
                    if ((day > 1) && _voteCount.GetDayBoundaries(day - 1, out startPost, out endTime, out endPost))
                    {
                        udStartPost.Value = endPost + 1;
                        DateTime eod;
                        if (_voteCount.Turbo)
                        {
                            eod = endTime.AddMinutes(25);
                        }
                        else
                        {
                            eod = endTime.AddDays(1);
                        }
                        dtEodDate.Value = eod;
                        dtEodTime.Value = eod;
                    }
                    else
                    {
                        udStartPost.Value = 1;
                        DateTime now = DateTime.Now;
                        DateTime eod = new DateTime(now.Year, now.Month, now.Day, 18, 0, 0);
                        dtEodDate.Value = eod;
                        dtEodTime.Value = eod;
                    }
                }
            }
        }
    }
}
