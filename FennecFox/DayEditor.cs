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
        private Werewolf.ElectionInfo _voteCount;

        public DayEditor()
        {
            InitializeComponent();
        }

        public DayEditor(Werewolf.ElectionInfo _voteCount)
        {
            this._voteCount = _voteCount;
            InitializeComponent();
            dtEodTime.Format = DateTimePickerFormat.Custom;
            dtEodTime.CustomFormat = Application.CurrentCulture.DateTimeFormat.ShortTimePattern;
    
            udStartPost.Value = Math.Max(1, _voteCount.StartPost);
            DateTime eod = _voteCount.EndTime;
            dtEodDate.Value = eod;
            dtEodTime.Value = eod;

        }
        public void GetDayBoundaries(out Int32 day, out Int32 startPost, out DateTime endTime)
        {
            day = 1;
            startPost = (Int32)udStartPost.Value;
            DateTime eodDate = dtEodDate.Value;
            DateTime eodTime = dtEodTime.Value;
            endTime = new DateTime(eodDate.Year, eodDate.Month, eodDate.Day, 
                    eodTime.Hour, eodTime.Minute, 0, DateTimeKind.Local);
        }

        private void btnStartNow_Click(object sender, EventArgs e)
        {
            udStartPost.Value = Math.Max(1, _voteCount.LastPost);
        }

        private void btnPlusDay_Click(object sender, EventArgs e)
        {
            dtEodDate.Value = dtEodDate.Value.AddDays(1);
        }

        private void btnPlusTwenty_Click(object sender, EventArgs e)
        {
            dtEodTime.Value = dtEodTime.Value.AddMinutes(20);
        }

    }
}
