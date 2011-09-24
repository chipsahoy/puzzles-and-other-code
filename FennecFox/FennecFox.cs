using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using Timer = System.Windows.Forms.Timer;

namespace FennecFox
{
    public partial class FormVoteCounter : Form
    {
        int m_startPost = 1;
        int m_currentPage = 1;
        int m_postsPerPage = 50;
        ListViewColumnSorter lvwColumnSorter;
        private Thread workerThread;

        private String m_username; // if this changes from what user entered previously, need to logout then re-login with new info.

        ConnectionSettings connectionSettings = new ConnectionSettings();

        private static String BASE_URL = "http://forumserver.twoplustwo.com/";

        public FormVoteCounter()
        {
            InitializeComponent();

            lvwColumnSorter = new ListViewColumnSorter();
            this.listVotes.ListViewItemSorter = lvwColumnSorter;
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

            if (resp == null || !resp.Contains("exec_refresh()"))
            {
                // login failure
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
                if (lastRead == 0)
                {
                    return;
                }

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
                    string poster = "";
                    string postNumber = "";

                    HtmlAgilityPack.HtmlNode postNumberNode = post.SelectSingleNode("../../../tr[1]/td[2]/a");
                    if (postNumberNode != null)
                    {
                        postNumber = postNumberNode.InnerText;
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

                    HtmlAgilityPack.HtmlNodeCollection bolds = post.SelectNodes("child::b");
                    if (bolds != null)
                    {
                        foreach (HtmlAgilityPack.HtmlNode c in bolds)
                        {
                            if (c.InnerText.Trim().Length > 0)
                            {
                                //                            Console.WriteLine(String.Format("{0,8}\t{1,25}\t{2}", postNumber, poster, c.InnerHtml));
                                Console.WriteLine("{0}\t{1}\t{2}", postNumber, poster, c.InnerHtml);
                                AddVote(poster, new Vote(postNumber, c.InnerHtml));
                            }
                        }
                    }

                    UpdateLastPost(lastPost);
                }
            }

            return lastPost;
        }

        private void UpdateLastPost(Int32 lastPost)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new LastPostDelegate(UpdateLastPost), new object[] { lastPost });
                return;
            }

            txtLastPost.Text = lastPost.ToString();
            statusText.Text = "Got post " + lastPost;
        }

        private delegate void LastPostDelegate(Int32 lastPost);
        private delegate void VoteDelegate(String player, Vote vote);
        Dictionary<string, Tuple<LinkedList<Vote>, ListViewItem>> m_PlayerVotes = new Dictionary<string, Tuple<LinkedList<Vote>, ListViewItem>>();
        void AddVote(string player, Vote vote)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new VoteDelegate(AddVote), new object[] { player, vote });
                return;
            }

            if (!m_PlayerVotes.ContainsKey(player))
            {
                LinkedList<Vote> list = new LinkedList<Vote>();
                ListViewItem item = new ListViewItem(player);
                item.SubItems.Add(player);
                item.SubItems.Add("0");
                item.SubItems.Add("");
                ListViewItem itemInList = listVotes.Items.Add(item);
                Tuple<LinkedList<Vote>, ListViewItem> t = new Tuple<LinkedList<Vote>, ListViewItem>(list, itemInList);
                m_PlayerVotes.Add(player, t);
            }

            Tuple<LinkedList<Vote>, ListViewItem> tPlayer = m_PlayerVotes[player];
            if (tPlayer != null)
            {
                tPlayer.Item1.AddLast(vote);
                // visual update
                tPlayer.Item2.SubItems[1].Text = vote.PostNumber;
                tPlayer.Item2.SubItems[2].Text = vote.Content;
            }
        }

        public void HideVote(string player)
        {
            Tuple<LinkedList<Vote>, ListViewItem> tPlayer = m_PlayerVotes[player];
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
                        tPlayer.Item2.SubItems[1].Text = vote.PostNumber;
                        tPlayer.Item2.SubItems[2].Text = vote.Content;
                        return;
                    }
                    node = node.Previous;
                }
                tPlayer.Item2.SubItems[1].Text = "0";
                tPlayer.Item2.SubItems[2].Text = "";
            }
        }

        public void UnhideVote(string player)
        {
            Tuple<LinkedList<Vote>, ListViewItem> tPlayer = m_PlayerVotes[player];
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
                    tPlayer.Item2.SubItems[1].Text = vote.PostNumber;
                    tPlayer.Item2.SubItems[2].Text = vote.Content;
                }
                return;
            }
        }

        private void btnIgnore_Click(object sender, EventArgs e)
        {
            if (listVotes.SelectedItems.Count < 1)
            {
                return;
            }
            ListViewItem item = listVotes.SelectedItems[0];
            if (item != null)
            {
                string player = item.SubItems[0].Text;
                HideVote(player);
            }
        }

        private void btnUnignore_Click(object sender, EventArgs e)
        {
            if (listVotes.SelectedItems.Count < 1)
            {
                return;
            }
            ListViewItem item = listVotes.SelectedItems[0];
            if (item != null)
            {
                string player = item.SubItems[0].Text;
                UnhideVote(player);
            }
        }

        private void listVotes_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            // Determine if clicked column is already the column that is being sorted.
            if (e.Column == lvwColumnSorter.SortColumn)
            {
                // Reverse the current sort direction for this column.
                if (lvwColumnSorter.Order == SortOrder.Ascending)
                {
                    lvwColumnSorter.Order = SortOrder.Descending;
                }
                else
                {
                    lvwColumnSorter.Order = SortOrder.Ascending;
                }
            }
            else
            {
                // Set the column number that is to be sorted; default to ascending.
                lvwColumnSorter.SortColumn = e.Column;
                lvwColumnSorter.Order = SortOrder.Ascending;
            }

            // Perform the sort with these new sort options.
            this.listVotes.Sort();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            listVotes.Items.Clear();
            m_PlayerVotes.Clear();
            m_startPost = 1; // can/will be overridden by user first later
            txtLastPost.Text = "0";

            statusText.Text = "Cleared votes";
        }

        private volatile bool _shouldStop = false;
        private void StopButton_Click(object sender, EventArgs e)
        {
            _shouldStop = true;
            workerThread.Join();

            halt();
        }

        private void halt()
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new MethodInvoker(halt));
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
            if (e.TabPageIndex == 3 || e.TabPageIndex == 4)
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

            Properties.Settings.Default.Save();
        }

        private void Form_Load(object sender, EventArgs e)
        {
            m_startPost = FennecFox.Properties.Settings.Default.firstPost;
            txtFirstPost.Text = m_startPost.ToString();
            m_postsPerPage = FennecFox.Properties.Settings.Default.postsPerPage;
            txtUsername.Text = FennecFox.Properties.Settings.Default.username;
            txtPassword.Text = FennecFox.Properties.Settings.Default.password;
            URLTextBox.Text = FennecFox.Properties.Settings.Default.threadUrl;
        }
    }
}
