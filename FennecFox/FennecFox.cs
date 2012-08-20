using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using POG.Werewolf;
using POG.Forum;


namespace POG.FennecFox
{

    public partial class FormVoteCounter : Form
    {
        enum CounterColumn
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
        TwoPlusTwoForum _forum;
        Action<Action> _synchronousInvoker;
        System.Windows.Forms.Timer _timerEODCountdown = new System.Windows.Forms.Timer();




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
            grdVotes.Columns.Insert((Int32)CounterColumn.Player , col);

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
            col.DefaultCellStyle.Format = "hh:mm tt";
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

        void grdVotes_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
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
                        if (((DateTime)e.Value) == DateTime.MinValue)
                        {
                            e.Value = "";
                        }
                    }
                    break;

                case (Int32)CounterColumn.VotesFor:
                    {
                        if (((String)e.Value) == _voteCount.ErrorVote)
                        {
                            e.CellStyle.BackColor = System.Drawing.Color.Red;
                        }
                    }
                    break;

            }
        }


        void grdVotes_CellContentClick(object sender, DataGridViewCellEventArgs e)
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



        public FormVoteCounter()
        {
            InitializeComponent();
            tabVotes.TabPages.Remove(tabPage5);
            _synchronousInvoker = a => Invoke(a);
            _forum = new TwoPlusTwoForum(_synchronousInvoker);
            _forum.LoginEvent += new EventHandler<POG.Forum.LoginEventArgs>(_forum_LoginEvent);

        }

        void _forum_LoginEvent(object sender, POG.Forum.LoginEventArgs e)
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
                                    btnStartGame_Click(this, EventArgs.Empty);
                                    if (txtPlayers.Text == "")
                                    {
                                        var players = new string[POG.FennecFox.Properties.Settings.Default.Players.Count];
                                        POG.FennecFox.Properties.Settings.Default.Players.CopyTo(players, 0);
                                        var list = new List<String>(players);
                                        list.Sort();
                                        txtPlayers.Lines = list.ToArray();
                                    }
                                    udStartPost.Value = POG.FennecFox.Properties.Settings.Default.firstPost;
                                    dtEndTime.Value = POG.FennecFox.Properties.Settings.Default.EndOfDay;
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


        void _voteCount_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "LivePlayers")
            {
                SetupVoteGrid();
            }
            if (e.PropertyName == "Status")
            {
                statusText.Text = _voteCount.Status; // no direct binding support in status strip.
            }
        }

        void _timerEODCountdown_Tick(object sender, EventArgs e)
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
            var tPlayer = _voteCount[player];
            if (tPlayer != null)
            {
                tPlayer.HideVote();
            }
        }

        public void UnhideVote(string player)
        {
            var tPlayer = _voteCount[player];
            if (tPlayer != null)
            {
                tPlayer.UnhideVote();
            }
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
        
        private void Form_FormClosing(object sender, FormClosingEventArgs e)
        {
            POG.FennecFox.Properties.Settings.Default.EndOfDay = dtEndTime.Value;
            POG.FennecFox.Properties.Settings.Default.firstPost = (Int32)udStartPost.Value;
            POG.FennecFox.Properties.Settings.Default.username = txtUsername.Text.Trim();
            POG.FennecFox.Properties.Settings.Default.password = txtPassword.Text.Trim();
            POG.FennecFox.Properties.Settings.Default.threadUrl = URLTextBox.Text.Trim();
            POG.FennecFox.Properties.Settings.Default.Players = new StringCollection();
            if (_voteCount != null)
            {
                foreach (Voter p in _voteCount.LivePlayers)
                {
                    POG.FennecFox.Properties.Settings.Default.Players.Add(p.Name);
                }
            }

            POG.FennecFox.Properties.Settings.Default.Save();
            
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
            if (POG.FennecFox.Properties.Settings.Default.Players == null)
            {
                POG.FennecFox.Properties.Settings.Default.Players = new StringCollection();
            }
            DateTime now = DateTime.Now;
            dtEndTime.Value = new DateTime(now.Year, now.Month, now.Day, 20, 0, 0, now.Kind);

            CreateVoteGridColumns();
            SetupVoteGrid();

            txtUsername.Text = POG.FennecFox.Properties.Settings.Default.username;
            txtPassword.Text = POG.FennecFox.Properties.Settings.Default.password;
            btnLogin_Click(btnLogin, EventArgs.Empty);
            _timerEODCountdown.Interval = 1000;
            _timerEODCountdown.Tick+= new EventHandler(_timerEODCountdown_Tick);
            _timerEODCountdown.Start();
            
        }
        private void BindToNewGame()
        {
            ThreadReader t = _forum.Reader();
            _voteCount = new VoteCount(_synchronousInvoker, t, URLTextBox.Text);
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
            udStartPost.DataBindings.Add("Value", _voteCount, "StartPost", true, DataSourceUpdateMode.OnPropertyChanged);
            txtEndPost.DataBindings.Add("Text", _voteCount, "EndPost", false, DataSourceUpdateMode.OnPropertyChanged);
            dtEndTime.DataBindings.Add("Value", _voteCount, "EndTime", false, DataSourceUpdateMode.OnPropertyChanged);
            dtStartTime.DataBindings.Add("Value", _voteCount, "StartTime", true, DataSourceUpdateMode.OnPropertyChanged);
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
            txtPlayers.Text = "";
            btnReset.Enabled = false;
        }

        private void txtPlayers_TextChanged(object sender, EventArgs e)
        {
            if (_voteCount != null)
            {
                _voteCount.SetPlayerList(txtPlayers.Text);
            }
        }



        private delegate void PostTableDelegate(StringBuilder sb);
        private void PostTable(StringBuilder sb)
        {
            txtPostTable.Text = sb.ToString();
        }

        private void txtPostTable_Click(object sender, EventArgs e)
        {
            txtPostTable.Text = _voteCount.PostableVoteCount;
            txtPostTable.SelectAll();
            Clipboard.SetDataObject(txtPostTable.Text, false);
            statusText.Text = "Copied vote count to clipboard.";
        }

        private void btnSetEOD_Click(object sender, EventArgs e)
        {
            DateTime now = DateTime.Now;
            DateTime eod = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, 0, now.Kind);
            Int32 duration = 20;
            if (chkTurboDay1.Checked)
            {
                duration = (Int32)numTurboDay1Length.Value;
                chkTurboDay1.Checked = false;
                statusText.Text = "Day 1 EOD Set";
            }
            else
            {
                duration = (Int32)numTurboDayNLength.Value;
                statusText.Text = "EOD Set";
            }
            eod = eod.AddMinutes(duration);
            dtEndTime.Value = eod;
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

        private void chkTurbo_CheckedChanged(object sender, EventArgs e)
        {
            if (chkTurbo.Checked)
            {
                btnSetEOD.Enabled = true;
                chkTurboDay1.Enabled = true;
                numTurboDay1Length.Enabled = true;
                numTurboDayNLength.Enabled = true;
            }
            else
            {
                btnSetEOD.Enabled = false;
                chkTurboDay1.Enabled = false;
                numTurboDay1Length.Enabled = false;
                numTurboDayNLength.Enabled = false;
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
            String url = NormalizeUrl(URLTextBox.Text);
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
                _voteCount.Refresh();
            }
        }
        String NormalizeUrl(String url)
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

                if (!url.EndsWith("/"))
                {
                    url += "/";
                }
            }
            return url;
        }





    }
    public static class FlashWindow
    {
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool FlashWindowEx(ref FLASHWINFO pwfi);

        [StructLayout(LayoutKind.Sequential)]
        private struct FLASHWINFO
        {
            /// <summary>
            /// The size of the structure in bytes.
            /// </summary>
            public uint cbSize;
            /// <summary>
            /// A Handle to the Window to be Flashed. The window can be either opened or minimized.
            /// </summary>
            public IntPtr hwnd;
            /// <summary>
            /// The Flash Status.
            /// </summary>
            public uint dwFlags;
            /// <summary>
            /// The number of times to Flash the window.
            /// </summary>
            public uint uCount;
            /// <summary>
            /// The rate at which the Window is to be flashed, in milliseconds. If Zero, the function uses the default cursor blink rate.
            /// </summary>
            public uint dwTimeout;
        }

        /// <summary>
        /// Stop flashing. The system restores the window to its original stae.
        /// </summary>
        public const uint FLASHW_STOP = 0;

        /// <summary>
        /// Flash the window caption.
        /// </summary>
        public const uint FLASHW_CAPTION = 1;

        /// <summary>
        /// Flash the taskbar button.
        /// </summary>
        public const uint FLASHW_TRAY = 2;

        /// <summary>
        /// Flash both the window caption and taskbar button.
        /// This is equivalent to setting the FLASHW_CAPTION | FLASHW_TRAY flags.
        /// </summary>
        public const uint FLASHW_ALL = 3;

        /// <summary>
        /// Flash continuously, until the FLASHW_STOP flag is set.
        /// </summary>
        public const uint FLASHW_TIMER = 4;

        /// <summary>
        /// Flash continuously until the window comes to the foreground.
        /// </summary>
        public const uint FLASHW_TIMERNOFG = 12;


        /// <summary>
        /// Flash the spacified Window (Form) until it recieves focus.
        /// </summary>
        /// <param name="form">The Form (Window) to Flash.</param>
        /// <returns></returns>
        public static bool Flash(System.Windows.Forms.Form form)
        {
            // Make sure we're running under Windows 2000 or later
            if (Win2000OrLater)
            {
                FLASHWINFO fi = Create_FLASHWINFO(form.Handle, FLASHW_ALL | FLASHW_TIMERNOFG, uint.MaxValue, 0);
                return FlashWindowEx(ref fi);
            }
            return false;
        }

        private static FLASHWINFO Create_FLASHWINFO(IntPtr handle, uint flags, uint count, uint timeout)
        {
            FLASHWINFO fi = new FLASHWINFO();
            fi.cbSize = Convert.ToUInt32(Marshal.SizeOf(fi));
            fi.hwnd = handle;
            fi.dwFlags = flags;
            fi.uCount = count;
            fi.dwTimeout = timeout;
            return fi;
        }

        /// <summary>
        /// Flash the specified Window (form) for the specified number of times
        /// </summary>
        /// <param name="form">The Form (Window) to Flash.</param>
        /// <param name="count">The number of times to Flash.</param>
        /// <returns></returns>
        public static bool Flash(System.Windows.Forms.Form form, uint count)
        {
            if (Win2000OrLater)
            {
                FLASHWINFO fi = Create_FLASHWINFO(form.Handle, FLASHW_ALL, count, 0);
                return FlashWindowEx(ref fi);
            }
            return false;
        }

        /// <summary>
        /// Start Flashing the specified Window (form)
        /// </summary>
        /// <param name="form">The Form (Window) to Flash.</param>
        /// <returns></returns>
        public static bool Start(System.Windows.Forms.Form form)
        {
            if (Win2000OrLater)
            {
                FLASHWINFO fi = Create_FLASHWINFO(form.Handle, FLASHW_ALL, uint.MaxValue, 0);
                return FlashWindowEx(ref fi);
            }
            return false;
        }

        /// <summary>
        /// Stop Flashing the specified Window (form)
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        public static bool Stop(System.Windows.Forms.Form form)
        {
            if (Win2000OrLater)
            {
                FLASHWINFO fi = Create_FLASHWINFO(form.Handle, FLASHW_STOP, uint.MaxValue, 0);
                return FlashWindowEx(ref fi);
            }
            return false;
        }

        /// <summary>
        /// A boolean value indicating whether the application is running on Windows 2000 or later.
        /// </summary>
        private static bool Win2000OrLater
        {
            get { return System.Environment.OSVersion.Version.Major >= 5; }
        }
    }
}
