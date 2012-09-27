using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using POG.Forum;
using POG.Utils;
using POG.Werewolf;

namespace POG.FennecFox
{
    public partial class FormVoteCounter : Form
    {
        private enum CounterColumn
        {
            Player = 0,
            VoteCount,
            PostCount,
            PostNumber,
            PostLink,
            PostTime,
            VotesFor,
            Bolded,
        };

        private VoteCount _voteCount;
        private TwoPlusTwoForum _forum;
        private Action<Action> _synchronousInvoker;
        private System.Windows.Forms.Timer _timerEODCountdown = new System.Windows.Forms.Timer();
        private Int32 _day = 1;
        String _url;
        Boolean _turbo;

        #region initialization
        public FormVoteCounter()
        {
            InitializeComponent();
        }
        public FormVoteCounter(TwoPlusTwoForum forum, Action<Action> synchronousInvoker, 
            String url, Boolean turbo) : this()
        {
            _forum = forum;
            _synchronousInvoker = synchronousInvoker;
            _url = url;
            _turbo = turbo;
        }

        protected override void OnLoad(EventArgs e)
        {
            DateTime now = DateTime.Now;
            CreateVoteGridColumns();
            SetupVoteGrid();
            BindToNewGame(_url);
            
            
            _timerEODCountdown.Interval = 1000;
            _timerEODCountdown.Tick += new EventHandler(_timerEODCountdown_Tick);
            _timerEODCountdown.Start();

        }
        #endregion

        private void SetupVoteGrid()
        {
            if (_voteCount == null)
            {
                return;
            }
            BindingSource bs = new BindingSource();
            bs.DataSource = _voteCount.LivePlayers;
            grdVotes.DataSource = null;

            //bs = new BindingSource();
            //bs.DataSource = m_game;
            List<String> validVotes = new List<string>();
            String notVoting = "not voting";
            validVotes.Add(notVoting);
            validVotes.Add(_voteCount.ErrorVote);
            validVotes.AddRange(_voteCount.ValidVotes.ToArray());
            validVotes.Add("");
            DataGridViewComboBoxColumn colCB = (DataGridViewComboBoxColumn)grdVotes.Columns[(Int32)CounterColumn.VotesFor];
            colCB.DataSource = validVotes.ToArray();
            colCB.DefaultCellStyle.NullValue = notVoting;

            //colCB.DataSource = bs;
            grdVotes.DataSource = bs;
            grdVotes.Sort(grdVotes.Columns[(Int32)CounterColumn.PostTime], ListSortDirection.Descending);
        }

        private void CreateVoteGridColumns()
        {
            grdVotes.AutoGenerateColumns = false;
            grdVotes.EditMode = DataGridViewEditMode.EditOnEnter;

            DataGridViewColumn col = new DataGridViewTextBoxColumn();
            col.DataPropertyName = "Name";
            col.HeaderText = "Player";
            col.ReadOnly = true;
            col.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            col.Resizable = DataGridViewTriState.False;
            grdVotes.Columns.Insert((Int32)CounterColumn.Player, col);

            col = new DataGridViewTextBoxColumn();
            col.DataPropertyName = "VoteCount";
            col.HeaderText = "Votes";
            col.ReadOnly = true;
            col.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            col.Resizable = DataGridViewTriState.False;
            grdVotes.Columns.Insert((Int32)CounterColumn.VoteCount, col);

            col = new DataGridViewTextBoxColumn();
            col.DataPropertyName = "PostCount";
            col.HeaderText = "Posts";
            col.ReadOnly = true;
            col.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            col.Resizable = DataGridViewTriState.False;
            col.DividerWidth = 3;
            grdVotes.Columns.Insert((Int32)CounterColumn.PostCount, col);

            col = new DataGridViewLinkColumn();
            col.DataPropertyName = "PostNumber";
            col.HeaderText = "Post";
            col.ReadOnly = true;
            col.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            col.Resizable = DataGridViewTriState.False;
            grdVotes.Columns.Insert((Int32)CounterColumn.PostNumber, col);

            col = new DataGridViewTextBoxColumn();
            col.DataPropertyName = "PostLink";
            col.HeaderText = "Link";
            col.ReadOnly = true;
            col.Visible = false;
            grdVotes.Columns.Insert((Int32)CounterColumn.PostLink, col);

            col = new DataGridViewTextBoxColumn();
            col.DataPropertyName = "PostTime";
            col.HeaderText = "Time";
            col.DefaultCellStyle.Format = "HH:mm";
            col.ReadOnly = true;
            col.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            col.Resizable = DataGridViewTriState.False;
            grdVotes.Columns.Insert((Int32)CounterColumn.PostTime, col);

            DataGridViewComboBoxColumn colCB = new DataGridViewComboBoxColumn();
            colCB.DataPropertyName = "Votee";
            colCB.HeaderText = "Votes For";
            colCB.DisplayStyleForCurrentCellOnly = true;
            colCB.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            colCB.Resizable = DataGridViewTriState.False;

            grdVotes.Columns.Insert((Int32)CounterColumn.VotesFor, colCB);

            col = new DataGridViewTextBoxColumn();
            col.DataPropertyName = "Bolded";
            col.HeaderText = "Content";
            col.ReadOnly = true;
            grdVotes.Columns.Insert((Int32)CounterColumn.Bolded, col);

            grdVotes.CellContentClick += new DataGridViewCellEventHandler(grdVotes_CellContentClick);
            grdVotes.CellFormatting += new DataGridViewCellFormattingEventHandler(grdVotes_CellFormatting);
        }

        private void grdVotes_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0)
            {
                return;
            }
            switch (e.ColumnIndex)
            {
                case (Int32)CounterColumn.PostNumber:
                    {
                        if (((Int32)e.Value) < 1)
                        {
                            e.Value = "";
                        }
                    }
                    break;

                case (Int32)CounterColumn.PostTime:
                    {
                        if (((DateTimeOffset)e.Value) == DateTime.MinValue)
                        {
                            e.Value = "";
                        }
                    }
                    break;

                case (Int32)CounterColumn.VotesFor:
                    {
                        if (_voteCount != null)
                        {
                            if (((String)e.Value) == _voteCount.ErrorVote)
                            {
                                e.CellStyle.BackColor = System.Drawing.Color.Red;
                            }
                        }
                    }
                    break;
            }
        }

        private void grdVotes_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
            {
                return;
            }
            switch (e.ColumnIndex)
            {
                case (Int32)CounterColumn.PostNumber:
                    {
                        DataGridViewRow row = grdVotes.Rows[e.RowIndex];
                        Int32 PostNumber = (Int32)row.Cells[e.ColumnIndex].Value;
                        if (PostNumber > 0)
                        {
                            String url = (String)row.Cells[(Int32)CounterColumn.PostLink].Value;
                            if (url != "")
                            {
                                System.Diagnostics.Process.Start(url);
                            }
                        }
                    }
                    break;
            }
        }

        private void _voteCount_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "LivePlayers")
            {
                SetupVoteGrid();
            }
            if (e.PropertyName == "StartTime")
            {
                String time = String.Empty;
                if (_voteCount.StartTime != null)
                {
                    time = _voteCount.StartTime.Value.ToString("g");
                }
                dtStartTime.Text = time;
            }
            if (e.PropertyName == "EndTime")
            {
                String time = String.Empty;
                time = _voteCount.EndTime.ToString("g");
                dtEndTime.Text = time;
            }
            if (e.PropertyName == "Status")
            {
                statusText.Text = _voteCount.Status; // no direct binding support in status strip.
            }
            if (e.PropertyName == "Day")
            {
                udDay.Value = _voteCount.Day;
            }
        }

        private void _timerEODCountdown_Tick(object sender, EventArgs e)
        {
            DateTime now = DateTime.Now;
            _timerEODCountdown.Interval = 1000 - now.Millisecond;

            if (_voteCount == null)
            {
                txtCountDown.Text = "No Game";
                return;
            }
            TimeSpan tsRemaining = _voteCount.TimeUntilNight;
            if (tsRemaining.Ticks < 0)
            {
                txtCountDown.Text = "Night!";
                return;
            }
            switch (tsRemaining.Days)
            {
                case 0:
                    {
                        lblDaysToEOD.Visible = false;
                    }
                    break;

                case 1:
                    {
                        lblDaysToEOD.Text = "1 day and";
                        lblDaysToEOD.Visible = true;
                    }
                    break;

                default:
                    {
                        lblDaysToEOD.Text = tsRemaining.Days.ToString() + " days and";
                        lblDaysToEOD.Visible = true;
                    }
                    break;
            
            }
            txtCountDown.Text = String.Format("{0:00}:{1:00}:{2:00}",
                    tsRemaining.Hours, tsRemaining.Minutes, tsRemaining.Seconds);
            if (tsRemaining.TotalSeconds == 120)
            {
                FlashWindow.Flash(this);
            }
        }

        private delegate void ErrorDelegate(Exception e);

        private void HandleError(Exception e)
        {
            if (InvokeRequired)
            {
                Invoke(new ErrorDelegate(HandleError), new object[] { e });
                return;
            }

            String file = Directory.GetCurrentDirectory() + @"\error.log";
            File.AppendAllText(file, DateTime.Now + ": " + e.ToString() + "\n\n");
            MessageBox.Show(this,
                            String.Format(
                                "An error has occurred and a crashdump was created at \n\n{0}\n\nPlease send the log file to the developer.  The message was:\n\n{1}",
                                file, e.Message), "Critical Error");
        }

        private void SetComboRange(DataGridViewComboBoxCell cell)
        {
            object v = cell.Value;
            cell.Items.Clear();
            cell.Items.AddRange(_voteCount);
            if (v != null && cell.Items.Contains(v))
            {
                cell.Value = v;
            }
            else
            {
                cell.Value = cell.Items[0];
            }
        }

        public void HideVote(string player)
        {
            _voteCount.IgnoreVote(player);
        }

        public void UnhideVote(string player)
        {
            _voteCount.UnIgnoreVote(player);
        }

        private void btnIgnore_Click(object sender, EventArgs e)
        {
            if (grdVotes.SelectedRows.Count < 1)
            {
                return;
            }

            var item = grdVotes.SelectedRows[0];
            if (item != null)
            {
                var player = (String)item.Cells[(Int32)CounterColumn.Player].Value;
                HideVote(player);
            }
        }

        private void btnUnignore_Click(object sender, EventArgs e)
        {
            if (grdVotes.SelectedRows.Count < 1)
            {
                return;
            }
            var item = grdVotes.SelectedRows[0];
            if (item != null)
            {
                var player = (String)item.Cells[(Int32)CounterColumn.Player].Value;
                UnhideVote(player);
            }
        }

        private void mnuHide_Click(object sender, EventArgs e)
        {
            btnIgnore_Click(sender, e);
        }

        private void mnuUnhide_Click(object sender, EventArgs e)
        {
            btnUnignore_Click(sender, e);
        }


        private void BindToNewGame(String url)
        {
            url = Utils.Misc.NormalizeUrl(url);
            ThreadReader t = _forum.Reader();
            _voteCount = new VoteCount(_synchronousInvoker, t, url, _forum.PostsPerPage);
            _voteCount.PropertyChanged += new PropertyChangedEventHandler(_voteCount_PropertyChanged);
            _voteCount.Turbo = _turbo;

            txtLastPost.DataBindings.Clear();
            udStartPost.DataBindings.Clear();
            txtEndPost.DataBindings.Clear();
            dtEndTime.DataBindings.Clear();
            dtStartTime.DataBindings.Clear();
            
            txtLastPost.DataBindings.Add("Text", _voteCount, "LastPost", false, DataSourceUpdateMode.OnPropertyChanged);
            udStartPost.DataBindings.Add("Text", _voteCount, "StartPost", false, DataSourceUpdateMode.OnPropertyChanged);
            txtEndPost.DataBindings.Add("Text", _voteCount, "EndPost", false, DataSourceUpdateMode.OnPropertyChanged);
            _voteCount.Refresh();
        }

        private void UnbindFromGame()
        {
            txtLastPost.DataBindings.Clear();
            udStartPost.DataBindings.Clear();
            txtEndPost.DataBindings.Clear();
            dtEndTime.DataBindings.Clear();
            dtStartTime.DataBindings.Clear();
            if (_voteCount != null)
            {
                _voteCount.PropertyChanged -= _voteCount_PropertyChanged;
                _voteCount = null;
            }
        }



        private void btnGetPosts_Click(object sender, EventArgs e)
        {
            if (_voteCount != null)
            {
                _voteCount.CheckThread();
            }
        }

        private void btnRoster_Click(object sender, EventArgs e)
        {
            statusText.Text = String.Empty;
            AutoComplete autoComplete = new AutoComplete(_forum, _synchronousInvoker);
            PlayerList frmPlayers = new PlayerList(_voteCount, autoComplete);
            DialogResult dr = frmPlayers.ShowDialog();
            if (dr == System.Windows.Forms.DialogResult.OK)
            {
            }
            _voteCount.CommitRosterChanges();
            _voteCount.Refresh();

        }
        private void btnEditDay_Click(object sender, EventArgs e)
        {
            statusText.Text = String.Empty;
            DayEditor frm = new DayEditor(_voteCount);
            DialogResult result = frm.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                Int32 day;
                Int32 startPost;
                DateTime endTime;
                frm.GetDayBoundaries(out day, out startPost, out endTime);
                _day = day;
                _voteCount.SetDayBoundaries(day, startPost, endTime);
                _voteCount.ChangeDay(day);
                _voteCount.Refresh();
            }
        }

        private void udDay_ValueChanged(object sender, EventArgs e)
        {
            Int32 day = (Int32)udDay.Value;
            Int32 startPost;
            DateTime endTime;
            Int32 endPost;
            if (_voteCount.GetDayBoundaries(day, out startPost, out endTime, out endPost))
            {
                _day = day;
                _voteCount.ChangeDay(day);
            }
            else
            {
                if (_voteCount.GetDayBoundaries(_day, out startPost, out endTime, out endPost))
                {
                    udDay.Value = _day;
                }
                this.statusText.Text = "Not a valid day! Use edit day button to add it first.";               
            }
        }

        private void btnCopyIt_Click(object sender, EventArgs e)
        {
            String count = _voteCount.GetPostableVoteCount();
            Clipboard.SetDataObject(count, false);
            statusText.Text = String.Format("Copied vote count to clipboard at {0}.", DateTime.Now.ToShortTimeString());
        }

        private void btnPostIt_Click(object sender, EventArgs e)
        {
            String count = _voteCount.GetPostableVoteCount();
            Int32 threadId = _voteCount.ThreadId;
            Boolean ok = _forum.MakePost(threadId, "Posted by Fennec Fox", count, 0, false);
            if (ok)
            {
                statusText.Text = String.Format("Posted a vote count at {0}.", DateTime.Now.ToShortTimeString());
            }
            else
            {
                statusText.Text = "Failed to post a vote count.";
            }
        }
    }

}