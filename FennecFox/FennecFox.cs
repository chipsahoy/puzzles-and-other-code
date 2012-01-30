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

namespace FennecFox
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
        private DataLibrary.WerewolfGame m_game;

        int _startPost = 1;
        int _currentPage = 1;
        int _postsPerPage = 50;
        private Thread _workerThread;

        private String _username; // if this changes from what user entered previously, need to logout then re-login with new info.

        readonly ConnectionSettings connectionSettings = new ConnectionSettings();

        private const String BASE_URL = "http://forumserver.twoplustwo.com/";

        private void SetupVoteGrid()
        {
            BindingSource bs = new BindingSource();
            bs.DataSource = m_game.LivePlayers;
            grdVotes.DataSource = null;

            //bs = new BindingSource();
            //bs.DataSource = m_game;
            List<String> validVotes = new List<string>();
            String notVoting = "not voting";
            validVotes.Add(notVoting);
            validVotes.Add(m_game.SelectVote);
            validVotes.AddRange(m_game.ValidVotes.ToArray());
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

            m_game = new DataLibrary.WerewolfGame(a => Invoke(a));
            m_game.PropertyChanged += new PropertyChangedEventHandler(m_game_PropertyChanged);            
        }


        void m_game_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "LivePlayers")
            {
                SetupVoteGrid();
            }
        }

        //void TimerEODCountdown_Tick(object sender, EventArgs e)
        //{
        //    DateTime dtNow = DateTime.Now;
        //    TimeSpan tsNow = new TimeSpan(dtNow.Hour, dtNow.Minute, dtNow.Second); // truncate to second.
        //    if (tsNow != tsCurrentTime)
        //    {
        //        tsCurrentTime = tsNow;
        //        TimeSpan tsRemaining = tsBadTime.Subtract(tsCurrentTime);
        //        if (tsRemaining.Ticks < 0)
        //        {
        //            tsRemaining = tsRemaining.Add(new TimeSpan(1, 0, 0, 0));
        //        }
        //        txtCountDown.Text = String.Format("EOD in {0:00}:{1:00}:{2:00}", tsRemaining.Hours, tsRemaining.Minutes, tsRemaining.Seconds);
        //        if (tsRemaining.TotalSeconds == 120)
        //        {
        //            FlashWindow.Flash(this);
        //        }
        //    }
        //}

        private int PageFromNumber(int number)
        {
            int page = (number / _postsPerPage) + 1;
            return page;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="username">username of gimmick account</param>
        /// <param name="password">password of gimmick account</param>
        /// <returns></returns>
        private String DoLogin(String username, String password)
        {
            if (_username != null && username != _username)
            {
                // user switched accounts.  logout first
                DoLogout();
            }

            if (username == _username)
            {
                // don't need to do anything, we're already logged in
                return null;
            }

            _username = null;
            String passToken = SecurityUtils.md5(password);

            connectionSettings.Url = String.Format("{0}login.php?do=login", BASE_URL);
            connectionSettings.Data =
                String.Format("vb_login_username={0}&cookieuser=1&vb_login_password=&s=&securitytoken=guest&do=login&vb_login_md5password={1}&vb_login_md5password_utf={1}", username, passToken);
            String resp = HtmlHelper.PostToUrl(connectionSettings);

            if (resp == null)
            {
                // login failure
                return "The following error occurred while logging in:\n\n" + connectionSettings.Message;
            }

            if (!resp.Contains("exec_refresh()"))
            {
                return "Error logging in.  Please verify login information.";
            }


            // set posts/page
            connectionSettings.Url = String.Format("{0}profile.php?do=editoptions", BASE_URL);
            _username = username;
            resp = HtmlHelper.GetUrlResponseString(connectionSettings);
            if (resp != null)
            {
                Match m = Regex.Match(resp, "umaxposts.*?value=\"(-?\\d+)\"[ ](class=\"[A-z0-9]*\")?[ ]*selected=\"selected\"", RegexOptions.Singleline);
                if (m.Success)
                {
                    _postsPerPage = Int32.Parse(m.Groups[1].Value);
                    if (_postsPerPage == -1)
                    {
                        _postsPerPage = 15;
                    }
                }
                else
                {

                    _postsPerPage = 100;
                    return
                        "Login success, but there was an error setting the posts/page.  Please set your settings to 100 posts/page.";
                }

            }
            else
            {
                _postsPerPage = 100;
                return
                    "Login success, but there was an error setting the posts/page.  Please set your settings to 100 posts/page.";
            }

            // logging in sets the cookies in connectionSettings so we don't have to do anything else.
            return null;
        }

        private void DoLogout()
        {
            // get the page once to find the logout url
            connectionSettings.Url = BASE_URL;
            String resp = HtmlHelper.GetUrlResponseString(connectionSettings);
            Match m = Regex.Match(resp, "logouthash=([A-z0-9-])");
            if (m.Success)
            {
                String hash = m.Groups[1].Value;
                connectionSettings.Url = String.Format("http://forumserver.twoplustwo.com/login.php?do=logout&amp;logouthash={0}", hash);
                HtmlHelper.GetUrlResponseString(connectionSettings);
                connectionSettings.CC = new CookieContainer(); // just in case
            }
        }

        private void DoWork()
        {
            //try
            {
                Boolean foundNewPosts = true;
                while (foundNewPosts)
                {
                    statusText.Text = "Fetching page " + _currentPage.ToString();

                    string doc = HtmlHelper.GetUrlResponseString(connectionSettings);
                    int totalPages = 0;
                    foundNewPosts = ParseVotePage(doc, _startPost, ref totalPages);
                    if (foundNewPosts)
                    {
                        _startPost = m_game.LastPost + 1;
                        int lastPostThisPage = _postsPerPage * _currentPage;
                        if (totalPages > _currentPage)
                        {
                            connectionSettings.Url = GetNextUrl(connectionSettings.Url, _currentPage + 1);
                            // if we make it to the end, start on next page
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                statusText.Text = "Finished on page " + _currentPage.ToString();
            }
            //catch (Exception e)
            {
                //HandleError(e);
            }

            // when we get here, we're all done or aborted.
            halt();
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

        private String GetNextUrl(String destination, Int32 page)
        {
            if (destination.EndsWith(".html") || destination.EndsWith(".htm"))
            {
                destination = destination.Substring(0, destination.LastIndexOf("index"));
            }

            if (!destination.EndsWith("/"))
            {
                destination += "/";
            }

            _currentPage = page;
            if (page > 1)
            {
                destination += "index" + page + ".html";
            }

            return destination;
        }

        private void GoButtonAgain_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrWhiteSpace(URLTextBox.Text))
            {
                MessageBox.Show(this, "URL cannot be empty.", "Error");
            }
            else
            {
                StopButton.Enabled = true;
                GoButton.Enabled = false;
                _shouldStop = false;

                _workerThread = new Thread(DoWork);

                _startPost = m_game.LastPost;

                int page = PageFromNumber(_startPost);
                string destination = GetNextUrl(URLTextBox.Text.Trim(), page);

                String resp = DoLogin(txtUsername.Text, txtPassword.Text);

                if (resp != null)
                {
                    halt();
                    MessageBox.Show(this, resp, "Failure");
                    return;
                }

                connectionSettings.Url = destination;

                _workerThread.Start();
            }
        }



        private Boolean ParseVotePage(string doc, int firstPost, ref int totalPages)
        {
            Boolean foundNewPosts = false;
            var html = new HtmlAgilityPack.HtmlDocument();
            html.LoadHtml(doc);
            HtmlAgilityPack.HtmlNode root = html.DocumentNode;
            // find total posts: /table/tr[1]/td[2]/div[@class="pagenav"]/table[1]/tr[1]/td[1] -- Page 106 of 106
            HtmlAgilityPack.HtmlNode pageNode = root.SelectSingleNode("//div[@class='pagenav']/table/tr/td");
            if (pageNode != null)
            {
                string pages = pageNode.InnerText;
                Match m = Regex.Match(pages, @"Page (\d+) of (\d+)");
                if (m.Success)
                {
                    //Console.WriteLine("{0}/{1}", m.Groups[1].Value, m.Groups[2].Value);
                    totalPages = Convert.ToInt32(m.Groups[2].Value);
                }
            }

            // //div[@id='posts']/div/div/div/div/table/tbody/tr[2]
            // td[1]/div[1] has (id with post #, <a> with user id, user name.)
            // td[2]/div[1] has title
            // td[2]/div[2] has post
            // "/html[1]/body[1]/table[2]/tr[2]/td[1]/td[1]/div[2]/div[1]/div[1]/div[1]/div[1]/table[1]/tr[2]/td[2]/div[2]" is a post
            HtmlAgilityPack.HtmlNodeCollection posts = root.SelectNodes("//div[@id='posts']/div/div/div/div/table/tr[2]/td[2]/div[contains(@id, 'post_message_')]");
            if (posts == null)
            {
                return false;
            }

            foreach (HtmlAgilityPack.HtmlNode post in posts)
            {
                if (_shouldStop)
                {
                    break;
                }
                foundNewPosts |= m_game.OnNewPost(post);
            }

            // ensure the timestamp gets re-generated even if there were no new posts
            UpdatePostcounts();
            return foundNewPosts;
        }

        private void SetComboRange(DataGridViewComboBoxCell cell)
        {
            object v = cell.Value;
            cell.Items.Clear();
            cell.Items.Add("--Select Player--");
            cell.Items.AddRange(m_game);
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
            var tPlayer = m_game[player];
            if (tPlayer != null)
            {
                tPlayer.HideVote();
            }
        }
        //private void FillInRow(DataGridViewRow row, Vote vote, Int32 postCount)
        //{
        //    row.Cells[(Int32)CounterColumn.Posts].Value = postCount;
        //    row.Cells[(Int32)CounterColumn.PostNumber].Value = vote.PostNumber;
        //    row.Cells[(Int32)CounterColumn.PostTime].Value = vote.Time.ToString("HH:mm");
        //    TimeSpan ts = tsBadTime - new TimeSpan(vote.Time.Hour, vote.Time.Minute, 0);
        //    if (ts.Ticks < 0)
        //    {
        //        ts = ts.Add(new TimeSpan(1, 0, 0, 0));
        //    }
        //    if (((ts.Ticks == 0) || (ts.Hours == 23)) && !chkEodFarAway.Checked)
        //    {
        //        // matches exact :01 votes or votes up to an hour after EOD.
        //        row.Cells[(Int32)CounterColumn.PostTime].Style.BackColor = System.Drawing.Color.Red;
        //        row.Cells[(Int32)CounterColumn.PostTime].Style.SelectionBackColor = System.Drawing.Color.Red;
        //    }
        //    else
        //    {
        //        row.Cells[(Int32)CounterColumn.PostTime].Style.BackColor = row.Cells[(Int32)CounterColumn.Player].Style.BackColor;
        //        row.Cells[(Int32)CounterColumn.PostTime].Style.SelectionBackColor = row.Cells[(Int32)CounterColumn.Player].Style.SelectionBackColor;
        //    }
        //    row.Cells[(Int32)CounterColumn.Bolded].Value = vote.Content;
        //    row.Tag = vote;
        //}

        public void UnhideVote(string player)
        {
            var tPlayer = m_game[player];
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


        private volatile bool _shouldStop = false;
        private void StopButton_Click(object sender, EventArgs e)
        {
            _shouldStop = true;
        }

        private void halt()
        {
            if (InvokeRequired)
            {
                Invoke(new MethodInvoker(halt));
                return;
            }

            GoButton.Enabled = true;
            StopButton.Enabled = false;
        }

        private void btnTestSettings_Click(object sender, EventArgs e)
        {
            String resp = DoLogin(txtUsername.Text, txtPassword.Text);
            if (resp == null)
            {
                MessageBox.Show(this, "Login successful.", "Success");
            }
            else
            {
                MessageBox.Show(this, resp, "Failure");
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
            Properties.Settings.Default.username = txtUsername.Text.Trim();
            Properties.Settings.Default.password = txtPassword.Text.Trim();
            Properties.Settings.Default.threadUrl = URLTextBox.Text.Trim();
            Properties.Settings.Default.firstPost = _startPost;
            Properties.Settings.Default.postsPerPage = _postsPerPage;
            Properties.Settings.Default.Players = new StringCollection();
            foreach (DataLibrary.Poster p in m_game.LivePlayers)
            {
                Properties.Settings.Default.Players.Add(p.Name);
            }
            Properties.Settings.Default.Moderator = txtModerator.Text;

            Properties.Settings.Default.Save();
        }

        protected override void OnLoad(EventArgs e)
        {
            if (Properties.Settings.Default.Mappings == null)
            {
                Properties.Settings.Default.Mappings = new StringDictionary();
            }

            if (Properties.Settings.Default.Players == null)
            {
                Properties.Settings.Default.Players = new StringCollection();
            }

            if (Properties.Settings.Default.DeadPlayers == null)
            {
                Properties.Settings.Default.DeadPlayers = new StringCollection();
            }
            CreateVoteGridColumns();
            SetupVoteGrid();

            _startPost = Properties.Settings.Default.firstPost;
            _postsPerPage = Properties.Settings.Default.postsPerPage;
            txtUsername.Text = Properties.Settings.Default.username;
            txtPassword.Text = Properties.Settings.Default.password;
            URLTextBox.Text = Properties.Settings.Default.threadUrl;
            var tmp = new string[Properties.Settings.Default.Players.Count];
            Properties.Settings.Default.Players.CopyTo(tmp, 0);
            var list = new List<String>(tmp);
            list.Sort();
            txtModerator.Text = Properties.Settings.Default.Moderator;
            txtPlayers.Lines = list.ToArray();
            tmp = new string[Properties.Settings.Default.DeadPlayers.Count];
            Properties.Settings.Default.DeadPlayers.CopyTo(tmp, 0);
            list = new List<string>(tmp);
            list.Sort();
            txtDeadPlayers.Lines = list.ToArray();


            txtVersion.Text = String.Format("Fennic Fox Vote Counter Version " + Assembly.GetExecutingAssembly().GetName().Version.ToString());
            txtLastPost.DataBindings.Add("Text", m_game, "LastPost", false, DataSourceUpdateMode.OnPropertyChanged);

            udStartPost.DataBindings.Add("Value", m_game, "StartPost", true, DataSourceUpdateMode.OnPropertyChanged);
            txtEndPost.DataBindings.Add("Text", m_game, "EndPost", false, DataSourceUpdateMode.OnPropertyChanged);
            dtEndTime.DataBindings.Add("Value", m_game, "EndTime", false, DataSourceUpdateMode.OnPropertyChanged);
            dtStartTime.DataBindings.Add("Value", m_game, "StartTime", true, DataSourceUpdateMode.OnPropertyChanged);
            Console.WriteLine("OnLoad complete");
        }

        private void txtPlayers_TextChanged(object sender, EventArgs e)
        {
            m_game.SetPlayerList(txtPlayers.Text);
            //DataGridViewComboBoxColumn colCB = (DataGridViewComboBoxColumn)grdVotes.Columns[(Int32)CounterColumn.VotesFor];
            //grdVotes.Refresh();
        }


        private void grdVotes_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            //if (e.RowIndex >= 0)
            //{
            //    String bolded = (String)grdVotes[(Int32)CounterColumn.Bolded, e.RowIndex].Value;
                

            //    if (e.ColumnIndex == (Int32)CounterColumn.Bolded)
            //    {
            //        DataLibrary.Poster player = m_game.ParseBoldedToVote(bolded);
            //        if (player != null)
            //        {
            //            grdVotes[(Int32)CounterColumn.VotesFor, e.RowIndex].Value = player;
            //            grdVotes[(Int32)CounterColumn.Bolded, e.RowIndex].Style.BackColor= grdVotes[(Int32)CounterColumn.Player, e.RowIndex].Style.BackColor;
            //            grdVotes[(Int32)CounterColumn.Bolded, e.RowIndex].Style.SelectionBackColor = grdVotes[(Int32)CounterColumn.Player, e.RowIndex].Style.SelectionBackColor;
            //        }
            //        else
            //        {
            //            grdVotes[(Int32)CounterColumn.Bolded, e.RowIndex].Style.BackColor = System.Drawing.Color.Red;
            //            grdVotes[(Int32)CounterColumn.Bolded, e.RowIndex].Style.SelectionBackColor = System.Drawing.Color.Red;
            //        }
            //    }
            //    else if (e.ColumnIndex == (Int32)CounterColumn.VotesFor)
            //    {
            //        m_game.AddVoteAlias(bolded, (String)grdVotes[(Int32)CounterColumn.VotesFor, e.RowIndex].Value);
            //        grdVotes[(Int32)CounterColumn.Bolded, e.RowIndex].Style.BackColor = grdVotes[(Int32)CounterColumn.Player, e.RowIndex].Style.BackColor;
            //    }
            //}
        }

        private void UpdatePostcounts()
        {
            if (InvokeRequired)
            {
                Invoke(new MethodInvoker(UpdatePostcounts));
                return;
            }

            foreach (DataGridViewRow row in grdVotes.Rows)
            {
                //var voter = ((String)row.Cells[(Int32)CounterColumn.Player].Value);
                //AddVote(voter, null, false);
            }

            //grdVotes.Sort();
            //grdVotes.Refresh();
        }

        private delegate void PostTableDelegate(StringBuilder sb);
        private void PostTable(StringBuilder sb)
        {
            txtPostTable.Text = sb.ToString();
        }

        private void txtPostTable_Click(object sender, EventArgs e)
        {
            txtPostTable.Text = m_game.PostableVoteCount;
            txtPostTable.SelectAll();
            Clipboard.SetDataObject(txtPostTable.Text, false);
            statusText.Text = "Copied vote count to clipboard.";
        }

        private void btnSetEOD_Click(object sender, EventArgs e)
        {
            Int32 dayLength = 20;
            if (chkTurboDay1.Checked)
            {
                dayLength = (Int32)numTurboDay1Length.Value;
            }
            else
            {
                dayLength = (Int32)numTurboDayNLength.Value;
            }
            if (dayLength < 1)
            {
                dayLength = 20;
            }
            DateTime dt = DateTime.Now;
            dt = dt.AddMinutes(dayLength);
            dt = dt.AddSeconds(-dt.Second);
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
                m_game.Turbo = true;
                btnSetEOD.Enabled = true;
                chkTurboDay1.Enabled = true;
                numTurboDay1Length.Enabled = true;
                numTurboDayNLength.Enabled = true;
            }
            else
            {
                m_game.Turbo = false;
                btnSetEOD.Enabled = false;
                chkTurboDay1.Enabled = false;
                numTurboDay1Length.Enabled = false;
                numTurboDayNLength.Enabled = false;
            }
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
