using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using Timer = System.Windows.Forms.Timer;

namespace FennecFox
{
    public partial class FormVoteCounter : Form
    {
        public String[] Players
        {
            get
            {
                return txtPlayers.Text.Split(
                    new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(p => p.Trim())
                    .Distinct()
                    .ToArray();
            }
        }
        /*
        private Dictionary<String, String> mappings = new Dictionary<string, string>();
        public Dictionary<String, String> Mappings
        {
            get { return mappings; }
        }*/

        int m_startPost = 1;
        int m_currentPage = 1;
        int m_postsPerPage = 50;
        private Thread workerThread;

        private String m_username; // if this changes from what user entered previously, need to logout then re-login with new info.

        ConnectionSettings connectionSettings = new ConnectionSettings();

        private static String BASE_URL = "http://forumserver.twoplustwo.com/";

        public FormVoteCounter()
        {
            InitializeComponent();

            grdVotes.Columns[1].ValueType = typeof(Int32);
        }

        private int PageFromNumber(int number)
        {
            int page = (number / m_postsPerPage) + 1;
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
            if (m_username != null && username != m_username)
            {
                // user switched accounts.  logout first
                DoLogout();
            }

            if (username == m_username)
            {
                // don't need to do anything, we're already logged in
                return null;
            }

            m_username = null;
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
            m_username = username;
            resp = HtmlHelper.GetUrlResponseString(connectionSettings);
            if (resp != null)
            {
                Match m = Regex.Match(resp, "umaxposts.*?value=\"(-?\\d+)\"[ ](class=\"[A-z0-9]*\")?[ ]*selected=\"selected\"", RegexOptions.Singleline);
                if (m.Success)
                {
                    m_postsPerPage = Int32.Parse(m.Groups[1].Value);
                    if (m_postsPerPage == -1)
                    {
                        m_postsPerPage = 15;
                    }
                }
                else
                {

                    m_postsPerPage = 100;
                    return
                        "Login success, but there was an error setting the posts/page.  Please set your settings to 100 posts/page.";
                }

            }
            else
            {
                m_postsPerPage = 100;
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
            if (!_shouldStop)
            {
                statusText.Text = "Fetching page " + m_currentPage.ToString();

                string doc = HtmlHelper.GetUrlResponseString(connectionSettings);
                int totalPages = 0;
                int lastRead = ParseVotePage(doc, m_startPost, ref totalPages);
                if (lastRead != 0)
                {

                    m_startPost = lastRead + 1;
                    int lastPostThisPage = m_postsPerPage * m_currentPage;
                    if (totalPages > m_currentPage)
                    {
                        connectionSettings.Url = GetNextUrl(connectionSettings.Url, m_currentPage + 1);
                        // if we make it to the end, start on next page
                        DoWork();
                        return;
                    }
                }
            }

            // when we get here, we're all done or aborted.
            halt();
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

            m_currentPage = page;
            if (page > 1)
            {
                destination += "index" + page.ToString() + ".html";
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

                workerThread = new Thread(DoWork);

                // find the min post # to search.
                // Grab that page.
                int firstPost = m_startPost;
                int userFirst = Convert.ToInt32(txtFirstPost.Text);
                if (userFirst > firstPost)
                {
                    firstPost = userFirst;
                }
                if (firstPost <= 0)
                {
                    firstPost = 1;
                }
                m_startPost = firstPost;

                int page = PageFromNumber(firstPost);
                string destination = GetNextUrl(URLTextBox.Text.Trim(), page);

                String resp = DoLogin(txtUsername.Text, txtPassword.Text);

                if (resp != null)
                {
                    halt();
                    MessageBox.Show(this, resp, "Failure");
                    return;
                }

                connectionSettings.Url = destination;

                workerThread.Start();
            }
        }

        void RemoveComments(HtmlAgilityPack.HtmlNode node)
        {
            foreach (var n in node.SelectNodes("//comment()") ?? new HtmlAgilityPack.HtmlNodeCollection(node))
            {
                n.Remove();
            }
        }

        void RemoveQuotes(HtmlAgilityPack.HtmlNode node)
        {
            foreach (var n in node.SelectNodes("div/table/tbody/tr/td[@class='alt2']") ?? new HtmlAgilityPack.HtmlNodeCollection(node))
            {
                HtmlAgilityPack.HtmlNode div = n.ParentNode.ParentNode.ParentNode.ParentNode;
                div.Remove();
            }
        }

        void RemoveColors(HtmlAgilityPack.HtmlNode node)
        {
            foreach (var n in node.SelectNodes("//font") ?? new HtmlAgilityPack.HtmlNodeCollection(node))
            {
                n.Remove();
            }
        }

        void RemoveNewlines(HtmlAgilityPack.HtmlNode node)
        {
            foreach (var n in node.SelectNodes("//br") ?? new HtmlAgilityPack.HtmlNodeCollection(node))
            {
                n.Remove();
            }
        }

        private int ParseVotePage(string doc, int firstPost, ref int totalPages)
        {
            int lastPost = 0;
            HtmlAgilityPack.HtmlDocument html = new HtmlAgilityPack.HtmlDocument();
            html.LoadHtml(doc);
            HtmlAgilityPack.HtmlNode root = html.DocumentNode;
            RemoveComments(root);
            // find total posts: /table/tr[1]/td[2]/div[@class="pagenav"]/table[1]/tr[1]/td[1] -- Page 106 of 106
            HtmlAgilityPack.HtmlNode pageNode = root.SelectSingleNode("//div[@class='pagenav']/table/tr/td");
            if (pageNode != null)
            {
                string pages = pageNode.InnerText;
                Match m = Regex.Match(pages, @"Page (\d+) of (\d+)");
                if (m.Success)
                {
                    //Console.WriteLine("{0}/{1}", m.Groups[1].Value, m.Groups[2].Value);
                    int currentPage = Convert.ToInt32(m.Groups[1].Value);
                    totalPages = Convert.ToInt32(m.Groups[2].Value);
                }
            }

            // //div[@id='posts']/div/div/div/div/table/tbody/tr[2]
            // td[1]/div[1] has (id with post #, <a> with user id, user name.)
            // td[2]/div[1] has title
            // td[2]/div[2] has post
            HtmlAgilityPack.HtmlNodeCollection posts = root.SelectNodes("//div[@id='posts']/div/div/div/div/table/tr[2]/td[2]/div[@class='postbitlinks']");
            if (posts == null)
            {
                return lastPost;
            }

            foreach (HtmlAgilityPack.HtmlNode post in posts)
            {
                if (!_shouldStop)
                {
                    // strip out quotes
                    RemoveQuotes(post);

                    // strip out colors
                    RemoveColors(post);

                    // strip out newlines
                    RemoveNewlines(post);
                    string poster = "";
                    Int32 postNumber = 0;

                    HtmlAgilityPack.HtmlNode postNumberNode = post.SelectSingleNode("../../../tr[1]/td[2]/a");
                    if (postNumberNode != null)
                    {
                        postNumber = Int32.Parse(postNumberNode.InnerText);
                        lastPost = Convert.ToInt32(postNumber);
                        if (lastPost < firstPost)
                        {
                            continue;
                        }
                    }

                    HtmlAgilityPack.HtmlNode userNode = post.SelectSingleNode("../../td[1]/div/a[@class='bigusername']");
                    if (userNode != null)
                    {
                        poster = userNode.InnerText;
                    }
                    if (lastPost == 17828)
                    {
                        Console.WriteLine(post.InnerHtml);
                    }

                    if (txtModerator.Text != poster)
                    {
                        HtmlAgilityPack.HtmlNodeCollection bolds = post.SelectNodes("child::b");
                        if (bolds != null)
                        {
                            foreach (HtmlAgilityPack.HtmlNode c in bolds)
                            {
                                if (c.InnerText.Trim().Length > 0)
                                {
                                    //                            Console.WriteLine(String.Format("{0,8}\t{1,25}\t{2}", postNumber, poster, c.InnerHtml));
                                    Console.WriteLine("{0}\t{1}\t{2}", postNumber, poster, c.InnerHtml);
                                    AddVote(poster, new Vote(postNumber, c.InnerHtml.Trim()));
                                }
                            }
                        }
                    }

                    UpdateLastPost(lastPost);
                    GenerateTable();
                }
                else
                {
                    break;
                }
            }

            // ensure the timestamp gets re-generated even if there were no new posts
            GenerateTable();
            return lastPost;
        }

        private void UpdateLastPost(Int32 lastPost)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new LastPostDelegate(UpdateLastPost), new object[] { lastPost });
                return;
            }

            txtLastPost.Text = lastPost.ToString();
            statusText.Text = "Got post " + lastPost;
        }

        private delegate void LastPostDelegate(Int32 lastPost);
        private delegate void VoteDelegate(String player, Vote vote);
        Dictionary<string, Tuple<LinkedList<Vote>, DataGridViewRow>> m_PlayerVotes = new Dictionary<string, Tuple<LinkedList<Vote>, DataGridViewRow>>();
        void AddVote(string player, Vote vote)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new VoteDelegate(AddVote), new object[] { player, vote });
                return;
            }

            if (!m_PlayerVotes.ContainsKey(player))
            {
                LinkedList<Vote> list = new LinkedList<Vote>();

                Int32 index = grdVotes.Rows.Add(MakeNewRow(player));

                Tuple<LinkedList<Vote>, DataGridViewRow> t = new Tuple<LinkedList<Vote>, DataGridViewRow>(list, grdVotes.Rows[index]);
                m_PlayerVotes.Add(player, t);
            }

            Tuple<LinkedList<Vote>, DataGridViewRow> tPlayer = m_PlayerVotes[player];
            if (tPlayer != null)
            {
                tPlayer.Item1.AddLast(vote);
                // visual update
                tPlayer.Item2.Cells[1].Value = vote.PostNumber;
                tPlayer.Item2.Cells[2].Value = vote.Content;
            }

            if (grdVotes.SortedColumn != null)
            {
                if (grdVotes.SortOrder == SortOrder.Descending)
                {
                    grdVotes.Sort(grdVotes.SortedColumn, ListSortDirection.Descending);
                }
                else
                {
                    grdVotes.Sort(grdVotes.SortedColumn, ListSortDirection.Ascending);
                }
            }
            else
            {
                grdVotes.Sort(grdVotes.Columns[0], ListSortDirection.Ascending);
            }
        }

        private DataGridViewRow MakeNewRow(String player)
        {
            var toRet = new DataGridViewRow();
            toRet.Cells.Add(new DataGridViewTextBoxCell());
            toRet.Cells.Add(new DataGridViewTextBoxCell());
            toRet.Cells.Add(new DataGridViewTextBoxCell());
            DataGridViewComboBoxCell c = new DataGridViewComboBoxCell();
            SetComboRange(c);
            toRet.Cells.Add(c);

            toRet.Cells[0].Value = player;
            toRet.Cells[1].Value = 0;
            toRet.Cells[1].ValueType = typeof(Int32);
            toRet.Cells[2].Value = "";

            return toRet;
        }

        private void SetComboRange(DataGridViewComboBoxCell cell)
        {
            object v = cell.Value;
            cell.Items.Clear();
            cell.Items.Add("--Select Player--");
            cell.Items.AddRange(Players);
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
            Tuple<LinkedList<Vote>, DataGridViewRow> tPlayer = m_PlayerVotes[player];
            if (tPlayer != null)
            {
                LinkedList<Vote> list = tPlayer.Item1;
                LinkedListNode<Vote> node = list.Last;
                // find first shown node...
                while (node != null)
                {
                    if (node.Value.Ignore == false)
                    {
                        node.Value.Ignore = true;
                        break;
                    }
                    node = node.Previous;
                }
                // now find a node to show
                while (node != null)
                {
                    if (node.Value.Ignore == false)
                    {
                        // winner, show this
                        Vote vote = node.Value;
                        tPlayer.Item2.Cells[1].Value = vote.PostNumber;
                        tPlayer.Item2.Cells[2].Value = vote.Content;
                        return;
                    }
                    node = node.Previous;
                }

                tPlayer.Item2.Cells[1].Value = "0";
                tPlayer.Item2.Cells[2].Value = "";
            }
        }

        public void UnhideVote(string player)
        {
            Tuple<LinkedList<Vote>, DataGridViewRow> tPlayer = m_PlayerVotes[player];
            if (tPlayer != null)
            {
                LinkedList<Vote> list = tPlayer.Item1;
                LinkedListNode<Vote> node = list.Last;
                // find first shown node...
                while (node != null)
                {
                    if (node.Value.Ignore == false)
                    {
                        break;
                    }
                    node = node.Previous;
                }
                if (node == null)
                {
                    node = list.First;
                }
                else
                {
                    // now back up one.
                    node = node.Next;
                }
                if (node != null)
                {
                    // show this guy
                    node.Value.Ignore = false;
                    Vote vote = node.Value;
                    tPlayer.Item2.Cells[1].Value = vote.PostNumber;
                    tPlayer.Item2.Cells[2].Value = vote.Content;
                }
                return;
            }
        }

        private void btnIgnore_Click(object sender, EventArgs e)
        {
            if (grdVotes.SelectedRows.Count < 1)
            {
                return;
            }

            DataGridViewRow item = grdVotes.SelectedRows[0];
            if (item != null)
            {
                var player = (String)item.Cells[0].Value;
                HideVote(player);
            }
        }

        private void btnUnignore_Click(object sender, EventArgs e)
        {
            if (grdVotes.SelectedRows.Count < 1)
            {
                return;
            }
            DataGridViewRow item = grdVotes.SelectedRows[0];
            if (item != null)
            {
                string player = (String)item.Cells[0].Value;
                UnhideVote(player);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            grdVotes.Rows.Clear();
            m_PlayerVotes.Clear();
            m_startPost = 1; // can/will be overridden by user first later
            txtLastPost.Text = "0";

            statusText.Text = "Cleared votes";
        }

        private volatile bool _shouldStop = false;
        private void StopButton_Click(object sender, EventArgs e)
        {
            _shouldStop = true;
        }

        private void halt()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new MethodInvoker(halt));
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
            Properties.Settings.Default.firstPost = m_startPost;
            Properties.Settings.Default.postsPerPage = m_postsPerPage;
            Properties.Settings.Default.Players = new StringCollection();
            Properties.Settings.Default.Players.AddRange(Players);
            Properties.Settings.Default.Moderator = txtModerator.Text;
            Properties.Settings.Default.EndOfDay = dtEOD.Value;

            Properties.Settings.Default.Save();
        }

        private void Form_Load(object sender, EventArgs e)
        {
            if (FennecFox.Properties.Settings.Default.Mappings == null)
            {
                FennecFox.Properties.Settings.Default.Mappings = new StringDictionary();
            }

            if (FennecFox.Properties.Settings.Default.Players == null)
            {
                FennecFox.Properties.Settings.Default.Players = new StringCollection();
            }

            m_startPost = FennecFox.Properties.Settings.Default.firstPost;
            txtFirstPost.Text = m_startPost.ToString();
            m_postsPerPage = FennecFox.Properties.Settings.Default.postsPerPage;
            txtUsername.Text = FennecFox.Properties.Settings.Default.username;
            txtPassword.Text = FennecFox.Properties.Settings.Default.password;
            URLTextBox.Text = FennecFox.Properties.Settings.Default.threadUrl;
            String[] tmp = new string[FennecFox.Properties.Settings.Default.Players.Count];
            FennecFox.Properties.Settings.Default.Players.CopyTo(tmp, 0);
            txtModerator.Text = FennecFox.Properties.Settings.Default.Moderator;
            txtPlayers.Lines = tmp;
            dtEOD.Value = FennecFox.Properties.Settings.Default.EndOfDay;
        }

        private void txtPlayers_TextChanged(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in grdVotes.Rows)
            {
                DataGridViewComboBoxCell cell = (DataGridViewComboBoxCell)row.Cells[3];
                SetComboRange(cell);
            }
        }

        private void grdVotes_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                var vote = ((String)grdVotes[2, e.RowIndex].Value).ToLower().Replace(" ", "");
                if (vote.StartsWith("vote:"))
                {
                    vote = vote.Substring(5).Trim();
                }

                if (vote.StartsWith("vote"))
                {
                    vote = vote.Substring(4).Trim();
                }

                if (e.ColumnIndex == 2)
                {
                    if (String.IsNullOrWhiteSpace(vote) || vote == "unvote")
                    {
                        grdVotes[3, e.RowIndex].Value = ((DataGridViewComboBoxCell)grdVotes[3, e.RowIndex]).Items[0];
                    }
                    else
                    {
                        var player = Players.FirstOrDefault(p => p.ToLower().Replace(" ", "") == vote || p.ToLower().Replace(" ", "").StartsWith(vote));
                        if (player != null)
                        {
                            grdVotes[3, e.RowIndex].Value = player;
                        }
                        else
                        {
                            // check if there is a mapping defined for this vote => player
                            if (FennecFox.Properties.Settings.Default.Mappings.ContainsKey(vote))
                            {
                                player =
                                    Players.FirstOrDefault(
                                        p => p == FennecFox.Properties.Settings.Default.Mappings[vote]);
                                if (player != null)
                                {
                                    grdVotes[3, e.RowIndex].Value = player;
                                }
                            }
                        }
                    }
                }
                else if (e.ColumnIndex == 3)
                {
                    FennecFox.Properties.Settings.Default.Mappings[vote] = (String)grdVotes[3, e.RowIndex].Value;
                }

                if (e.ColumnIndex == 2 || e.ColumnIndex == 3)
                {
                    GenerateTable();
                }
            }
        }

        private void GenerateTable()
        {
            var leftInDay = (dtEOD.Value.TimeOfDay - DateTime.Now.TimeOfDay);
            var leftInDayFormatted = String.Format("{0:00}:{1:00}", leftInDay.Hours, (leftInDay.Minutes == 59 ? 0 : leftInDay.Minutes + 1));
            StringBuilder sb = new StringBuilder(@"[b]As of #");
            sb
                .Append(txtLastPost.Text)
                .AppendLine().AppendLine();

            if (chkTurbo.Checked)
            {
                sb.Append(txtTurboEnd.Text);
            }
            else
            {
                sb.AppendFormat("Night in {0}", leftInDayFormatted);

            }

            sb.AppendLine("[/b]").AppendLine()
            .AppendLine("[table=head][b]#[/b] | [b]Player[/b] | [b]votes for[/b]");

            // get current votes for each player
            var voteCountDict = new Dictionary<String, Int32>();
            var voteDict = new Dictionary<String, String>();
            var playerMap = new Dictionary<String, String>();

            // for each LIVE player (todo)
            foreach (var player in Players)
            {
                voteCountDict.Add(player.ToLower(), 0);
                voteDict.Add(player.ToLower(), "");
                playerMap.Add(player.ToLower(), player);
            }

            foreach (DataGridViewRow row in grdVotes.Rows)
            {
                var voted = (String)row.Cells[3].Value;
                if (voteCountDict.ContainsKey(voted.ToLower()) && voteDict.ContainsKey(((String)row.Cells[0].Value).ToLower()))
                {
                    voteCountDict[voted.ToLower()] += 1;
                }

                if (voteDict.ContainsKey(((String)row.Cells[0].Value).ToLower()))
                {
                    voteDict[((String)row.Cells[0].Value).ToLower()] =
                        (row.Cells[3].Value == ((DataGridViewComboBoxCell)row.Cells[3]).Items[0])
                            ? ""
                            : (String)row.Cells[3].Value;
                }
            }

            // now sort by descending value
            var sortedDict =
                (from entry in voteCountDict orderby entry.Value descending select entry).ToDictionary(pair => pair.Key,
                                                                                                  pair => pair.Value);

            foreach (KeyValuePair<String, Int32> pair in sortedDict)
            {
                sb
                    .AppendFormat("{0} | {1} | [color='red']{2}[/color]", pair.Value, playerMap[pair.Key], voteDict[pair.Key])
                    .AppendLine();
            }

            sb.Append("[/table]");

            this.Invoke(new PostTableDelegate(PostTable), new object[] { sb });
        }

        private delegate void PostTableDelegate(StringBuilder sb);
        private void PostTable(StringBuilder sb)
        {
            txtPostTable.Text = sb.ToString();
        }

        private void txtPostTable_Click(object sender, EventArgs e)
        {
            txtPostTable.SelectAll();
            Clipboard.SetDataObject(txtPostTable.Text, false);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Int32 start = 0;
            Int32 end = 0;
            if (chkTurboDay1.Checked)
            {
                TimeSpan ts = new TimeSpan(0, 0, (Int32)numTurboDay1Length.Value, 0);
                start = DateTime.Now.TimeOfDay.Add(ts).Minutes;

            }
            else
            {
                TimeSpan ts = new TimeSpan(0, 0, (Int32)numTurboDayNLength.Value, 0);
                start = DateTime.Now.TimeOfDay.Add(ts).Minutes;
            }

            if (start == 60)
            {
                start = 0;
            }

            end = start + 1;

            if (end == 60)
            {
                end = 0;
            }

            txtTurboEnd.Text = String.Format(":{0:00} good :{1:00} bad", start, end);
        }
    }
}
