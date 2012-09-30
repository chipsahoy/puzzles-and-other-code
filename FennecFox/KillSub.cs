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
    public partial class KillSub : Form
    {
        private Werewolf.VoteCount _voteCount;

        public KillSub()
        {
            InitializeComponent();
        }

        public KillSub(string name, Werewolf.VoteCount _voteCount) : this()
        {
            // TODO: Complete member initialization
            this.Name = name;
            this._voteCount = _voteCount;
            dtExitTime.Format = DateTimePickerFormat.Custom;
            dtExitTime.CustomFormat = Application.CurrentCulture.DateTimeFormat.ShortTimePattern;
        }

        internal void GetKillSub(out bool isSub, out DateTime when, out string who)
        {
            if (chkSub.Checked)
            {
                isSub = true;
                who = txtReplacement.Text.Trim();
            }
            else
            {
                isSub = false;
                who = String.Empty;
            }
            DateTime exitDate = dtExitDate.Value;
            DateTime exitTime = dtExitTime.Value;
            when = new DateTime(exitDate.Year, exitDate.Month, exitDate.Day,
                    exitTime.Hour, exitTime.Minute, 0, DateTimeKind.Local);
        }
    }
}
