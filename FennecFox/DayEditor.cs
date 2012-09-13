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
            // TODO: Complete member initialization
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
    }
}
