using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;
namespace FennecFox
{
    public partial class FormVoteCounter : Form
    {
        Timer m_timer = new Timer();

        int m_startPost = 1;
        int m_currentPage = 1;
        int m_postsPerPage = 50;
        ListViewColumnSorter lvwColumnSorter;

        public FormVoteCounter()
        {
            InitializeComponent();
            m_timer.Enabled = false;
            m_timer.Interval = 100;
            m_timer.Tick += new EventHandler(m_timer_Tick);

            lvwColumnSorter = new ListViewColumnSorter();
            this.listVotes.ListViewItemSorter = lvwColumnSorter;
        }

        void m_timer_Tick(object sender, EventArgs e)
        {
            m_timer.Enabled = false;
        }


        private int PageFromNumber(int number)
        {
            int ppp = Convert.ToInt32(textPostsPerPage.Text);
            if (ppp <= 0)
            {
                ppp = 50;
            }
            int page = (number / ppp) + 1;
            m_postsPerPage = ppp;
            return page;
        }
        private void GoButtonAgain_Click(object sender, EventArgs e)
        {
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

            string destination = URLTextBox.Text;
            int page = PageFromNumber(firstPost);
            m_currentPage = page;
            if (page > 1)
            {
                destination += "index" + page.ToString() + ".html";
            }
            if ((WebBrowserPage.Url != null) && (destination == WebBrowserPage.Url.AbsolutePath))
            {
                WebBrowserRefreshOption opt = WebBrowserRefreshOption.Completely;
                WebBrowserPage.Refresh(opt);
            }
            else
            {
                WebBrowserPage.Navigate(destination);
            }
            statusText.Text = "Fetching page " + m_currentPage.ToString();
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
            HtmlAgilityPack.HtmlNode pageNode = root.SelectSingleNode("//div[@class='pagenav']/table/tbody/tr/td");
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
            HtmlAgilityPack.HtmlNodeCollection posts = root.SelectNodes("//div[@id='posts']/div/div/div/div/table/tbody/tr[2]/td[2]/div[@class='postbitlinks']");
            if (posts == null)
            {
                return lastPost;
            }
            foreach (HtmlAgilityPack.HtmlNode post in posts)
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
            }
            return lastPost;
        }

        private void WebBrowserPage_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (e.Url.AbsolutePath != WebBrowserPage.Url.AbsolutePath)
            {
                return;
            }
            string doc = WebBrowserPage.Document.Body.InnerHtml;
            int totalPages = 0;
            int lastRead = ParseVotePage(doc, m_startPost, ref totalPages);
            if (lastRead == 0)
            {
                return;
            }
            txtLastPost.Text = lastRead.ToString();
            statusText.Text = "Got post " + lastRead.ToString();
            m_startPost = lastRead + 1;
            int lastPostThisPage = m_postsPerPage * m_currentPage;
            if (totalPages > m_currentPage)
            {
                m_currentPage++;
                string destination = URLTextBox.Text;
                destination += "index" + m_currentPage.ToString() + ".html";

                // if we make it to the end, start on next page
                WebBrowserPage.Navigate(destination);
                statusText.Text = "Fetching page " + m_currentPage.ToString();
            }
        }

        class Vote
        {
            public Vote(string postNumber, string content)
            {
                Ignore = false;
                Content = content;
                PostNumber = postNumber;
            }
            public string PostNumber
            {
                get;
                private set;
            }
            public string Content
            {
                get;
                private set;
            }
            public bool Ignore
            {
                get;
                set;
            }
        }
        Dictionary<string, Tuple<LinkedList<Vote>, ListViewItem>> m_PlayerVotes = new Dictionary<string,Tuple<LinkedList<Vote>,ListViewItem>>();
        void AddVote(string player, Vote vote)
        {
            if(!m_PlayerVotes.ContainsKey(player))
            {
                LinkedList<Vote> list = new LinkedList<Vote>();
                ListViewItem item = new ListViewItem(player);
                item.SubItems.Add(player);
                item.SubItems.Add("0");
                item.SubItems.Add("");
                ListViewItem itemInList = listVotes.Items.Add(item);
                Tuple<LinkedList<Vote>, ListViewItem> t = new Tuple<LinkedList<Vote>,ListViewItem>(list, itemInList);
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
            if(listVotes.SelectedItems.Count < 1)
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
    }

    /// <summary>
    /// This class is an implementation of the 'IComparer' interface.
    /// </summary>
    public class ListViewColumnSorter : IComparer
    {
        /// <summary>
        /// Specifies the column to be sorted
        /// </summary>
        private int ColumnToSort;
        /// <summary>
        /// Specifies the order in which to sort (i.e. 'Ascending').
        /// </summary>
        private SortOrder OrderOfSort;
        /// <summary>
        /// Case insensitive comparer object
        /// </summary>
        private CaseInsensitiveComparer ObjectCompare;

        /// <summary>
        /// Class constructor.  Initializes various elements
        /// </summary>
        public ListViewColumnSorter()
        {
            // Initialize the column to '0'
            ColumnToSort = 0;

            // Initialize the sort order to 'none'
            OrderOfSort = SortOrder.None;

            // Initialize the CaseInsensitiveComparer object
            ObjectCompare = new CaseInsensitiveComparer();
        }

        /// <summary>
        /// This method is inherited from the IComparer interface.  It compares the two objects passed using a case insensitive comparison.
        /// </summary>
        /// <param name="x">First object to be compared</param>
        /// <param name="y">Second object to be compared</param>
        /// <returns>The result of the comparison. "0" if equal, negative if 'x' is less than 'y' and positive if 'x' is greater than 'y'</returns>
        public int Compare(object x, object y)
        {
            int compareResult = 0;
            ListViewItem listviewX, listviewY;

            // Cast the objects to be compared to ListViewItem objects
            listviewX = (ListViewItem)x;
            listviewY = (ListViewItem)y;

            // Compare the two items
            int column = ColumnToSort;
            if (listviewX.ListView.Columns[column].Tag.ToString() == "Numeric")
            {
                try
                {
                    float fl1 = float.Parse(listviewX.SubItems[column].Text);
                    float fl2 = float.Parse(listviewY.SubItems[column].Text);
                    compareResult = ObjectCompare.Compare(fl1, fl2);
                }
                catch
                {
                }
            }
            else
            {
                compareResult = ObjectCompare.Compare(listviewX.SubItems[column].Text, listviewY.SubItems[column].Text);
            }
            // Calculate correct return value based on object comparison
            if (OrderOfSort == SortOrder.Ascending)
            {
                // Ascending sort is selected, return normal result of compare operation
                return compareResult;
            }
            else if (OrderOfSort == SortOrder.Descending)
            {
                // Descending sort is selected, return negative result of compare operation
                return (-compareResult);
            }
            else
            {
                // Return '0' to indicate they are equal
                return 0;
            }
        }

        /// <summary>
        /// Gets or sets the number of the column to which to apply the sorting operation (Defaults to '0').
        /// </summary>
        public int SortColumn
        {
            set
            {
                ColumnToSort = value;
            }
            get
            {
                return ColumnToSort;
            }
        }

        /// <summary>
        /// Gets or sets the order of sorting to apply (for example, 'Ascending' or 'Descending').
        /// </summary>
        public SortOrder Order
        {
            set
            {
                OrderOfSort = value;
            }
            get
            {
                return OrderOfSort;
            }
        }

    }
}
