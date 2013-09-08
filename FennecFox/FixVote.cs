using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using POG.Werewolf;

namespace POG.FennecFox
{
    public partial class FixVote : Form
    {
        private Werewolf.VoteCount _voteCount;
        private string _player;

        public FixVote()
        {
            InitializeComponent();
        }

        public FixVote(Werewolf.VoteCount _voteCount, string player) : this()
        {
            // TODO: Complete member initialization
            this._voteCount = _voteCount;
            this._player = player;
//            cmbValidVotes.Items.AddRange(_voteCount.ValidVotes.ToArray());
            cmbValidVotes.Items.AddRange(_voteCount.LivePlayers.Select((t) => t.Name).ToArray());
//            cmbValidVotes.Items.Add("not voting");
//            cmbValidVotes.Items.Add("error");
            RefreshVotee();
        }
        void RefreshVotee()
        {
            foreach (Voter v in _voteCount.LivePlayers)
            {
                if (v.Name == _player)
                {
                    txtBolded.Text = v.Bolded;
                    txtVotee.Text = v.Votee;
                    break;
                }
            }
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            if (radioAlias.Checked)
            {
                String alias = cmbValidVotes.SelectedItem.ToString();
                _voteCount.AddVoteAlias(txtBolded.Text, alias);
            }
            if (radioIgnore.Checked)
            {
                _voteCount.IgnoreVote(_player);
            }
            if (radioNoChange.Checked)
            {
            }
            if (radioOverride.Checked)
            {
            }
            if (radioUnignore.Checked)
            {
                _voteCount.UnIgnoreVote(_player);
            }
            _voteCount.Refresh();
            Refresh();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {

        }
    }
}
