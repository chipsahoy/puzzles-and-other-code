using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace FennecFox
{

    public partial class FormVoteCounter : Form
    {
        private class ObjectInt
        {
            public ObjectInt()
            {
                Value = 0;
            }

            public ObjectInt(Int32 value)
            {
                Value = value;
            }

            public override string ToString()
            {
                return Value.ToString();
            }

            public static implicit operator int(ObjectInt i)
            {
                return i.Value;
            }

            public Int32 Value { get; set; }
        }

        private String[] _players;
        public String[] Players
        {
            get
            {
                if (_players == null)
                {
                    List<String> list = txtPlayers.Text.Split(
                        new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(p => p.Trim())
                        .Distinct().ToList();

                    List<String> dead = txtDeadPlayers.Text.Split(
                        new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(p => p.Trim())
                        .Distinct().ToList();
                    dead.Add(txtModerator.Text);

                    list = list.Except(dead).ToList();

                    list.Sort();
                    list.AddRange(new[] { "unvote", "No Lynch" });
                    _players = list.ToArray();
                }

                return _players;
            }
        }

        public String[] DeadPlayers
        {
            get
            {
                var list = txtDeadPlayers.Text.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).Select(
                        p => p.Trim()).Distinct().ToList();
                list.Sort();
                return list.ToArray();
            }
        }

        public String[] AllPlayers
        {
            get { return Players.ToList().Union(DeadPlayers.ToList()).ToArray(); }
        }

        int _startPost = 1;
        int _currentPage = 1;
        int _postsPerPage = 50;
        private Thread _workerThread;

        private String _username; // if this changes from what user entered previously, need to logout then re-login with new info.

        readonly ConnectionSettings connectionSettings = new ConnectionSettings();

        private const String BASE_URL = "http://forumserver.twoplustwo.com/";

        public FormVoteCounter()
        {
            InitializeComponent();

            grdVotes.Columns[1].ValueType = typeof(Int32);
            grdVotes.Columns[2].ValueType = typeof(Int32);
        }

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
            try
            {
                if (!_shouldStop)
                {
                    statusText.Text = "Fetching page " + _currentPage.ToString();

                    string doc = HtmlHelper.GetUrlResponseString(connectionSettings);
                    int totalPages = 0;
                    int lastRead = ParseVotePage(doc, _startPost, ref totalPages);
                    if (lastRead != 0)
                    {

                        _startPost = lastRead + 1;
                        int lastPostThisPage = _postsPerPage * _currentPage;
                        if (totalPages > _currentPage)
                        {
                            connectionSettings.Url = GetNextUrl(connectionSettings.Url, _currentPage + 1);
                            // if we make it to the end, start on next page
                            DoWork();
                            return;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                HandleError(e);
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
                _players = null;

                _workerThread = new Thread(DoWork);

                // find the min post # to search.
                // Grab that page.
                int firstPost = _startPost;
                int userFirst = Convert.ToInt32(txtFirstPost.Text);
                if (userFirst > firstPost)
                {
                    firstPost = userFirst;
                }
                if (firstPost <= 0)
                {
                    firstPost = 1;
                }
                _startPost = firstPost;

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

                _workerThread.Start();
            }
        }

        static void RemoveComments(HtmlAgilityPack.HtmlNode node)
        {
            foreach (var n in node.SelectNodes("//comment()") ?? new HtmlAgilityPack.HtmlNodeCollection(node))
            {
                n.Remove();
            }
        }

        static void RemoveQuotes(HtmlAgilityPack.HtmlNode node)
        {
            foreach (var n in node.SelectNodes("div/table/tbody/tr/td[@class='alt2']") ?? new HtmlAgilityPack.HtmlNodeCollection(node))
            {
                HtmlAgilityPack.HtmlNode div = n.ParentNode.ParentNode.ParentNode.ParentNode;
                div.Remove();
            }
        }

        static void RemoveColors(HtmlAgilityPack.HtmlNode node)
        {
            foreach (var n in node.SelectNodes("//font") ?? new HtmlAgilityPack.HtmlNodeCollection(node))
            {
                n.Remove();
            }
        }

        static void RemoveNewlines(HtmlAgilityPack.HtmlNode node)
        {
            foreach (var n in node.SelectNodes("//br") ?? new HtmlAgilityPack.HtmlNodeCollection(node))
            {
                n.Remove();
            }
        }

        private delegate Tuple<LinkedList<Vote>, DataGridViewRow, ObjectInt> PlayerTupleDelegate(String player);
        private Tuple<LinkedList<Vote>, DataGridViewRow, ObjectInt> GetPlayerTuple(String player)
        {
            if (InvokeRequired)
            {
                return (Tuple<LinkedList<Vote>, DataGridViewRow, ObjectInt>)Invoke(new PlayerTupleDelegate(GetPlayerTuple), new object[] { player });
            }

            if (!_playerVotes.ContainsKey(player))
            {
                var list = new LinkedList<Vote>();

                Int32 index = grdVotes.Rows.Add(MakeNewRow(player));
                grdVotes.Rows[index].Tag = new Vote();

                var t = new Tuple<LinkedList<Vote>, DataGridViewRow, ObjectInt>(list, grdVotes.Rows[index], new ObjectInt(0));
                _playerVotes.Add(player, t);
            }

            var tPlayer = _playerVotes[player];

            return tPlayer;
        }

        private int ParseVotePage(string doc, int firstPost, ref int totalPages)
        {
            int lastPost = 0;
            var html = new HtmlAgilityPack.HtmlDocument();
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
                    String postLink = null;

                    HtmlAgilityPack.HtmlNode postNumberNode = post.SelectSingleNode("../../../tr[1]/td[2]/a");
                    if (postNumberNode != null)
                    {
                        postNumber = Int32.Parse(postNumberNode.InnerText);
                        postLink = postNumberNode.Attributes["href"].Value;
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

                    if (Players.Contains(poster, StringComparer.CurrentCultureIgnoreCase))
                    {
                        // if the poster's name exists as a different case, replace the player.
                        if (!Players.Contains(poster))
                        {
                            var tPoster = poster;
                            // if we get here, the difference is only by case
                            // just use the version that the mod wants to use - trying to update the list involves delegates and stuff.  meh.
                            poster = Players.First(x => x.ToLower() == tPoster.ToLower());
                        }

                        // add a new
                        var tPlayer = GetPlayerTuple(poster);
                        tPlayer.Item3.Value++;
                        HtmlAgilityPack.HtmlNodeCollection bolds = post.SelectNodes("child::b");

                        if (bolds != null)
                        {
                            foreach (HtmlAgilityPack.HtmlNode c in bolds)
                            {
                                if (c.InnerText.Trim().Length > 0)
                                {
                                    //                            Console.WriteLine(String.Format("{0,8}\t{1,25}\t{2}", postNumber, poster, c.InnerHtml));
                                    Console.WriteLine("{0}\t{1}\t{2}", postNumber, poster, c.InnerHtml);
                                    AddVote(poster, new Vote(postNumber, poster, c.InnerHtml.Trim(), postLink));
                                }
                            }
                        }

                        UpdateLastPost(lastPost);
                        GenerateTable();
                    }
                }
                else
                {
                    break;
                }
            }

            // ensure the timestamp gets re-generated even if there were no new posts
            GenerateTable();
            UpdatePostcounts();
            return lastPost;
        }

        private void UpdateLastPost(Int32 lastPost)
        {
            if (InvokeRequired)
            {
                Invoke(new LastPostDelegate(UpdateLastPost), new object[] { lastPost });
                return;
            }

            txtLastPost.Text = lastPost.ToString();
            statusText.Text = "Got post " + lastPost;
        }

        private delegate void LastPostDelegate(Int32 lastPost);
        private delegate void VoteDelegate(String player, Vote vote, bool sort);

        readonly Dictionary<string, Tuple<LinkedList<Vote>, DataGridViewRow, ObjectInt>> _playerVotes = new Dictionary<string, Tuple<LinkedList<Vote>, DataGridViewRow, ObjectInt>>();
        void AddVote(string player, Vote vote, bool sort = true)
        {
            if (InvokeRequired)
            {
                Invoke(new VoteDelegate(AddVote), new object[] { player, vote, sort });
                return;
            }

            var tPlayer = GetPlayerTuple(player);

            if (tPlayer != null)
            {
                if (vote != null)
                {
                    tPlayer.Item1.AddLast(vote);
                    tPlayer.Item2.Cells[2].Value = vote.PostNumber;
                    tPlayer.Item2.Cells[3].Value = vote.Content;
                    tPlayer.Item2.Tag = vote;
                }

                // visual update
                tPlayer.Item2.Cells[1].Value = (Int32)tPlayer.Item3;
            }

            if (sort)
            {
                grdVotes.Sort();
            }
        }

        private DataGridViewRow MakeNewRow(String player)
        {
            var toRet = new DataGridViewRow();
            toRet.Cells.Add(new DataGridViewTextBoxCell());
            toRet.Cells.Add(new DataGridViewTextBoxCell());
            toRet.Cells.Add(new DataGridViewTextBoxCell());
            toRet.Cells.Add(new DataGridViewTextBoxCell());
            var c = new DataGridViewComboBoxCell();
            SetComboRange(c);
            toRet.Cells.Add(c);

            toRet.Cells[0].Value = player;
            toRet.Cells[1].Value = 0;
            toRet.Cells[1].ValueType = typeof(Int32);
            toRet.Cells[2].Value = 0;
            toRet.Cells[2].ValueType = typeof(Int32);
            toRet.Cells[3].Value = "";

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

        public Vote GetActiveVote(String player)
        {
            Vote toRet = null;
            var tPlayer = _playerVotes[player];
            if (tPlayer != null)
            {
                var list = tPlayer.Item1;
                var node = list.Last;
                // find first shown node...
                while (node != null)
                {
                    if (node.Value.Ignore == false)
                    {
                        toRet = node.Value;
                        break;
                    }

                    node = node.Previous;
                }
            }

            return toRet;
        }

        public void HideVote(string player)
        {
            var tPlayer = _playerVotes[player];
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
                        tPlayer.Item2.Cells[1].Value = (Int32)tPlayer.Item3;
                        tPlayer.Item2.Cells[2].Value = vote.PostNumber;
                        tPlayer.Item2.Cells[3].Value = vote.Content;
                        tPlayer.Item2.Tag = vote;
                        return;
                    }

                    node = node.Previous;
                }

                tPlayer.Item2.Cells[1].Value = 0;
                tPlayer.Item2.Cells[2].Value = 0;
                tPlayer.Item2.Cells[3].Value = "";
                tPlayer.Item2.Tag = new Vote();
            }
        }

        public void UnhideVote(string player)
        {
            var tPlayer = _playerVotes[player];
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

                node = node == null ? list.First : node.Next;

                if (node != null)
                {
                    // show this guy
                    node.Value.Ignore = false;
                    Vote vote = node.Value;
                    tPlayer.Item2.Cells[1].Value = (Int32)tPlayer.Item3;
                    tPlayer.Item2.Cells[2].Value = vote.PostNumber;
                    tPlayer.Item2.Cells[3].Value = vote.Content;
                    tPlayer.Item2.Tag = vote;
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

            var item = grdVotes.SelectedRows[0];
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
            var item = grdVotes.SelectedRows[0];
            if (item != null)
            {
                var player = (String)item.Cells[0].Value;
                UnhideVote(player);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            grdVotes.Rows.Clear();
            _playerVotes.Clear();
            _startPost = 1; // can/will be overridden by user first later
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
            var players = AllPlayers.ToList();
            players.RemoveRange(Players.Count() - 2, 2);
            Properties.Settings.Default.Players.AddRange(players.ToArray());

            Properties.Settings.Default.DeadPlayers = new StringCollection();
            Properties.Settings.Default.DeadPlayers.AddRange(DeadPlayers);
            Properties.Settings.Default.Moderator = txtModerator.Text;
            Properties.Settings.Default.EndOfDay = dtEOD.Value;

            Properties.Settings.Default.Save();
        }

        private void Form_Load(object sender, EventArgs e)
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

            _startPost = Properties.Settings.Default.firstPost;
            txtFirstPost.Text = _startPost.ToString();
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
            dtEOD.Value = Properties.Settings.Default.EndOfDay;
        }

        private void txtPlayers_TextChanged(object sender, EventArgs e)
        {
            _players = null;
            foreach (DataGridViewRow row in grdVotes.Rows)
            {
                var cell = (DataGridViewComboBoxCell)row.Cells[4];
                SetComboRange(cell);
            }
        }

        private void grdVotes_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                var vote = ((String)grdVotes[3, e.RowIndex].Value).ToLower().Replace(" ", "");
                if (vote.StartsWith("vote:"))
                {
                    vote = vote.Substring(5).Trim();
                }

                if (vote.StartsWith("vote"))
                {
                    vote = vote.Substring(4).Trim();
                }

                if (e.ColumnIndex == 3)
                {
                    /*
                    if (String.IsNullOrWhiteSpace(vote) || vote == "unvote")
                    {
                        grdVotes[3, e.RowIndex].Value = ((DataGridViewComboBoxCell)grdVotes[3, e.RowIndex]).Items[0];
                    }
                    else
                    {
                     * */
                    var player = Players.FirstOrDefault(p => p.ToLower().Replace(" ", "") == vote || p.ToLower().Replace(" ", "").StartsWith(vote));
                    if (player != null)
                    {
                        grdVotes[4, e.RowIndex].Value = player;
                    }
                    else
                    {
                        // check if there is a mapping defined for this vote => player
                        if (Properties.Settings.Default.Mappings.ContainsKey(vote))
                        {
                            player =
                                Players.FirstOrDefault(
                                    p => p == Properties.Settings.Default.Mappings[vote]);
                            if (player != null)
                            {
                                grdVotes[4, e.RowIndex].Value = player;
                            }
                        }
                    }
                    //}
                }
                else if (e.ColumnIndex == 4)
                {
                    Properties.Settings.Default.Mappings[vote] = (String)grdVotes[4, e.RowIndex].Value;
                }

                if (e.ColumnIndex == 3 || e.ColumnIndex == 4)
                {
                    GenerateTable();
                }
            }
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
                var voter = ((String)row.Cells[0].Value);
                AddVote(voter, null, false);
            }

            grdVotes.Sort();
            grdVotes.Refresh();
        }

        private void GenerateTable()
        {
            var leftInDay = (dtEOD.Value.TimeOfDay - DateTime.Now.TimeOfDay);
            Int32 hours = leftInDay.Hours;
            Int32 minutes = leftInDay.Minutes;

            if (chkEodTomorrow.Checked)
            {
                hours += 24;
            }

            if (minutes == 59)
            {
                hours++;
                minutes = 0;
            }
            else
            {
                minutes++;
            }

            if (hours < 0)
            {
                hours += 24;
            }

            var leftInDayFormatted = String.Format("{0:00}:{1:00}", hours, minutes);
            var sb = new StringBuilder(@"[b]Votes as of post ");
            String lastPost = txtLastPost.Text;
            sb
                .Append(lastPost)
                .AppendLine();

            if (chkTurbo.Checked)
            {
                sb.Append(txtTurboEnd.Text);
            }
            else
            {
                sb.AppendFormat("Night in {0}", leftInDayFormatted);

            }

            sb.AppendLine("[/b]").AppendLine("---")
            .AppendLine("[table=head][b]Votes[/b]|[b]Lynch[/b]|[b]Voters[/b]");

            // get current votes for each player
            var votedDict = new Dictionary<String, List<String>>();

            // for each LIVE player
            var playerMap = Players.ToDictionary(player => player.ToLower());

            votedDict.Add("not voting", new List<string>());
            playerMap.Add("not voting", "not voting");
            var voting = new List<String>();

            foreach (var voterPair in _playerVotes)
            {
                var vote = GetActiveVote(voterPair.Key);
                var votee = ((String)voterPair.Value.Item2.Cells[4].Value).ToLower();
                if (vote != null && playerMap.ContainsKey(voterPair.Key.ToLower()) && playerMap.ContainsKey(votee.ToLower()))
                {
                    //voteCountDict[voted] += 1;
                    if (votedDict.ContainsKey(votee))
                    {
                        votedDict[votee].Add(playerMap[voterPair.Key.ToLower()]);
                    }
                    else
                    {
                        votedDict.Add(votee, new List<string>());
                        votedDict[votee].Add(playerMap[voterPair.Key.ToLower()]);
                    }

                    voting.Add(voterPair.Key.ToLower());
                }
                else if (playerMap.ContainsKey(voterPair.Key) && (String.IsNullOrWhiteSpace(votee) || votee.ToLower() == "--select player--"))
                {
                    votedDict["not voting"].Add(playerMap[voterPair.Key]);
                    voting.Add(voterPair.Key.ToLower());
                }
            }

            // now add anyone who isn't here
            foreach (var player in Players)
            {
                var loweredPlayer = player.ToLower();
                if (!voting.Contains(loweredPlayer, StringComparer.CurrentCultureIgnoreCase) && loweredPlayer != "unvote" && loweredPlayer != "no lynch")
                {
                    votedDict["not voting"].Add(playerMap[loweredPlayer]);
                }
            }

            // now sort by descending value according to special rules (see VoteComparer)
            var sortedDict = votedDict.OrderBy(t => t, new VoteComparer());

            var unbold = new String[] { "unvote", "not voting", "no lynch" };
            foreach (KeyValuePair<String, List<String>> pair in sortedDict)
            {
                if (unbold.Contains(pair.Key, StringComparer.CurrentCultureIgnoreCase))
                {
                    sb
                        .AppendFormat("{0} | {1} | {2}", pair.Value.Count, playerMap[pair.Key],
                                      VoteLinks(pair, playerMap))
                        .AppendLine();
                }
                else
                {
                    sb
                        .AppendFormat("{0} | [b]{1}[/b] | {2}", pair.Value.Count, playerMap[pair.Key],
                                      VoteLinks(pair, playerMap))
                        .AppendLine();
                }
            }

            sb.Append("[/table]");

            this.Invoke(new PostTableDelegate(PostTable), new object[] { sb });
        }

        private String VoteLinks(KeyValuePair<String, List<String>> pair, Dictionary<String, String> playerMap)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var voter in pair.Value)
            {
                var tPlayer = GetPlayerTuple(playerMap[voter.ToLower()]);
                //var tPlayer = _playerVotes[];
                if (pair.Key == "not voting")
                {
                    sb.AppendFormat(", {0} ({1})", voter, tPlayer.Item3);
                }
                else
                {
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

                        if (node != null)
                        {
                            // show this guy
                            Vote vote = node.Value;
                            sb.AppendFormat(", [url={0}]{1}[/url] ({2})", vote.PostLink, voter, tPlayer.Item3);
                        }

                    }
                }
            }

            if (sb.Length > 2)
            {
                sb.Remove(0, 2);
            }

            return sb.ToString();
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
            var start = 0;
            var end = 0;
            if (chkTurboDay1.Checked)
            {
                var ts = new TimeSpan(0, 0, (Int32)numTurboDay1Length.Value, 0);
                start = DateTime.Now.TimeOfDay.Add(ts).Minutes;

            }
            else
            {
                var ts = new TimeSpan(0, 0, (Int32)numTurboDayNLength.Value, 0);
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

        private void txtMultiline_KeyDown(object sender, KeyEventArgs e)
        {
            var txtBox = sender as TextBox;
            if (txtBox != null && txtBox.Multiline && e.Control && e.KeyCode == Keys.A)
            {
                txtBox.SelectAll();
                e.SuppressKeyPress = true;
            }
        }
    }
}
