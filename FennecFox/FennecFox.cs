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

        #region initialization
        public FormVoteCounter()
        {
            InitializeComponent();
            _synchronousInvoker = a => Invoke(a);
            _forum = new TwoPlusTwoForum(_synchronousInvoker);
            _forum.LoginEvent += new EventHandler<POG.Forum.LoginEventArgs>(_forum_LoginEvent);
        }
        private void Form_FormClosing(object sender, FormClosingEventArgs e)
        {
            POG.FennecFox.Properties.Settings.Default.username = txtUsername.Text.Trim();
            POG.FennecFox.Properties.Settings.Default.password = txtPassword.Text.Trim();
            POG.FennecFox.Properties.Settings.Default.threadUrl = URLTextBox.Text.Trim();

            POG.FennecFox.Properties.Settings.Default.Save();
            _forum.LoginEvent -= _forum_LoginEvent;
        }

        protected override void OnLoad(EventArgs e)
        {
            txtVersion.Text = String.Format("Fennic Fox Vote Counter Version " + Assembly.GetExecutingAssembly().GetName().Version.ToString());
            if (POG.FennecFox.Properties.Settings.Default.updateSettings)
            {
                POG.FennecFox.Properties.Settings.Default.Upgrade();
                POG.FennecFox.Properties.Settings.Default.updateSettings = false;
                POG.FennecFox.Properties.Settings.Default.Save();
            }
            DateTime now = DateTime.Now;
            CreateVoteGridColumns();
            SetupVoteGrid();
            
            txtUsername.Text = POG.FennecFox.Properties.Settings.Default.username;
            txtPassword.Text = POG.FennecFox.Properties.Settings.Default.password;
            btnLogin_Click(btnLogin, EventArgs.Empty);
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


        private void _forum_LoginEvent(object sender, POG.Forum.LoginEventArgs e)
        {
            switch (e.LoginEventType)
            {
                case Forum.LoginEventType.LoginFailure:
                    {
                        MessageBox.Show(this, "Login failed! Check the username and password.");
                        btnLogin.Enabled = true;
                    }
                    break;

                case Forum.LoginEventType.LoginSuccess:
                    {
                        btnLogin.Enabled = false;
                        btnLogout.Enabled = true;
                        txtUsername.ReadOnly = true;
                        txtPassword.ReadOnly = true;
                        txtPassword.PasswordChar = '*';
                        {
                            if (URLTextBox.Text == "")
                            {
                                URLTextBox.Text = POG.FennecFox.Properties.Settings.Default.threadUrl;
                                if (URLTextBox.Text != "")
                                {
                                    _day = POG.FennecFox.Properties.Settings.Default.day;
                                    btnStartGame_Click(this, EventArgs.Empty);
                                }
                            }
                        }
                    }
                    break;

                case Forum.LoginEventType.LogoutSuccess:
                    {
                        btnLogin.Enabled = true;
                        btnLogout.Enabled = false;
                        txtUsername.Text = "";
                        txtPassword.Text = "";
                        txtUsername.ReadOnly = false;
                        txtPassword.ReadOnly = false;
                        txtPassword.PasswordChar = '\0';
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

            txtCountDown.Text = String.Format("EOD in {0:00}:{1:00}:{2:00}",
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

        private void tabControl_Selecting(object sender, TabControlCancelEventArgs e)
        {
            if (/*e.TabPageIndex == 3 || */e.TabPageIndex == 4)
            {
                e.Cancel = true;
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


        private void BindToNewGame()
        {
            ThreadReader t = _forum.Reader();
            _voteCount = new VoteCount(_synchronousInvoker, t, URLTextBox.Text, _forum.PostsPerPage);
            _voteCount.PropertyChanged += new PropertyChangedEventHandler(_voteCount_PropertyChanged);

            URLTextBox.ReadOnly = true;
            btnReset.Enabled = true;
            btnStartGame.Enabled = false;

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
            URLTextBox.ReadOnly = false;
            URLTextBox.Text = "";

            //txtPlayers.Text = "";
            btnReset.Enabled = false;
        }

        private void txtPlayers_TextChanged(object sender, EventArgs e)
        {
            if (_voteCount != null)
            {
                //_voteCount.SetPlayerList(txtPlayers.Text);
            }
        }

        private delegate void PostTableDelegate(StringBuilder sb);

        private void PostTable(StringBuilder sb)
        {
            txtPostTable.Text = sb.ToString();
        }

        private void txtPostTable_Click(object sender, EventArgs e)
        {
            txtPostTable.Text = _voteCount.GetPostableVoteCount();
            txtPostTable.SelectAll();
            Clipboard.SetDataObject(txtPostTable.Text, false);
            statusText.Text = "Copied vote count to clipboard.";
        }

        private void txtPlayers_KeyDown(object sender, KeyEventArgs e)
        {
            var txtBox = sender as TextBox;
            if (txtBox != null && txtBox.Multiline && e.Control && e.KeyCode == Keys.A)
            {
                txtBox.SelectAll();
                e.SuppressKeyPress = true;
            }
        }

        private void URLTextBox_TextChanged(object sender, EventArgs e)
        {
            if (URLTextBox.Text != "")
            {
                btnStartGame.Enabled = true;
            }
        }

        private void btnStartGame_Click(object sender, EventArgs e)
        {
            String url = Misc.NormalizeUrl(URLTextBox.Text);
            if (url != URLTextBox.Text)
            {
                URLTextBox.Text = url;
            }
            BindToNewGame();
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            UnbindFromGame();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            btnLogin.Enabled = false;
            _forum.Login(txtUsername.Text, txtPassword.Text);
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            _forum.Logout();
        }

        private void btnGetPosts_Click(object sender, EventArgs e)
        {
            if (_voteCount != null)
            {
                _voteCount.CheckThread();
            }
        }

        private void btnNewPlayerList_Click(object sender, EventArgs e)
        {
            PlayerList frmPlayers = new PlayerList();
            DialogResult dr = frmPlayers.ShowDialog();
            if (dr == System.Windows.Forms.DialogResult.OK)
            {
                IEnumerable<String> players = frmPlayers.Players;
                _voteCount.SetPlayerList(players);
            }
        }

        private void btnRoster_Click(object sender, EventArgs e)
        {

        }
        private void btnEditDay_Click(object sender, EventArgs e)
        {
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
                Console.WriteLine("OK");
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

    }

}