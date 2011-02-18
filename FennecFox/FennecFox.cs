using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FennecFox
{
    public partial class Form1 : Form
    {
        Posts m_posts = new Posts();
        Queue<Post> m_IdsToFetch = new Queue<Post>();
        Timer m_timer = new Timer();

        int m_startPost = 1;
        int m_currentPage = 1;
        int m_postsPerPage = 50;

        public Form1()
        {
            InitializeComponent();
            m_timer.Enabled = false;
            m_timer.Interval = 100;
            m_timer.Tick += new EventHandler(m_timer_Tick);
        }

        void m_timer_Tick(object sender, EventArgs e)
        {
            m_timer.Enabled = false;
            GetNextPostInternal();
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
            int firstPost = m_posts.MaxNumber;
            int userFirst= Convert.ToInt32(txtFirstPost.Text);
            if (userFirst > firstPost)
            {
                firstPost = userFirst;
            }
            if(firstPost <= 0)
            {
                firstPost= 1;
            }
            m_startPost = firstPost;

            string destination = URLTextBox.Text;
            int page = PageFromNumber(firstPost);
            m_currentPage = page;
            if (page > 1)
            {
                destination += "index" + page.ToString() + ".html";
            }

            WebBrowserPage.Navigate(destination);
            statusText.Text = "Fetching page " + m_currentPage.ToString();
        }

        private void GetNextPost()
        {
            m_timer.Enabled = true;
        }

        private void GetNextPostInternal()
        {
            if (0 == m_IdsToFetch.Count)
            {
                // nothing more to fetch. Update stuff.
                m_posts.ApplyFilters();
                OnPostSetChanged();
                return;
            }
            Post post= m_IdsToFetch.Peek();
            if (post != null)
            {
                string url = "http://forumserver.twoplustwo.com/newreply.php?do=newreply&p=" + post.Id.ToString();
                BrowserPost.Navigate(url);
                statusText.Text = "Fetching post " + post.Number.ToString();
            }
        }

        private void WebBrowserPage_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (e.Url.AbsolutePath != WebBrowserPage.Url.AbsolutePath)
            {
                return;
            }
            // enumerate post #s, queueing ids as we go
            int lastPostThisPage = m_postsPerPage * m_currentPage;
            for (int i = m_startPost; i <= lastPostThisPage; i++)
            {
                int postId = PostIdFromPostNumber(WebBrowserPage.Document, i);
                if (postId == 0)
                {
                    m_startPost = i;
                    // Now go get all those posts...
                    GetNextPost();
                    return;
                }
                Post post = new Post(i, postId);
                m_posts.AddPost(post);
                m_IdsToFetch.Enqueue(post);
            }
            m_startPost = lastPostThisPage + 1;
            m_currentPage++;
            string destination = URLTextBox.Text;
            destination += "index" + m_currentPage.ToString() + ".html";

            // if we make it to the end, start on next page
            WebBrowserPage.Navigate(destination);
            statusText.Text = "Fetching page " + m_currentPage.ToString();
        }

        private int PostIdFromPostNumber(HtmlDocument doc, int postNumber)
        {
            int rc = 0;
            HtmlElement element = GetElementFromName(doc, postNumber.ToString(), "a");
            if (element != null)
            {
                // id="postcount24750004"
                string id = element.GetAttribute("id");
                if (id.Length >= 9)
                {
                    rc = Convert.ToInt32(id.Substring(9));
                }
            }
            return rc;
        }

        private HtmlElement GetElementFromName(HtmlDocument doc, string name, string tag = "")
        {
            HtmlElement rc= null;
            HtmlElementCollection elements;
            if(tag == "")
            {
                elements = doc.All;
            }
            else
            {
                elements = doc.GetElementsByTagName(tag);
            }
            foreach (HtmlElement element in elements)
            {
                string nameElement = element.GetAttribute("name");
                if (nameElement == name)
                {
                    rc= element;
                    break;
                }
            }
            return rc;
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            m_IdsToFetch.Clear();
            m_posts.Clear();
        }

        private void BrowserPost_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (e.Url.AbsolutePath != BrowserPost.Url.AbsolutePath)
            {
                return;
            }
            // got the post.
            if (0 == m_IdsToFetch.Count)
            {
                return;
            }
            Post post = m_IdsToFetch.Dequeue();
            if (post == null)
            {
                return;
            }
            HtmlElement element = BrowserPost.Document.GetElementById("vB_Editor_001_textarea");
            if (element != null)
            {
                post.Content = element.InnerText;
                txtLastPost.Text = post.Number.ToString();
                statusText.Text = "Got post " + post.Number.ToString();
            }
            else
            {
                statusText.Text = "Failure getting post " + post.Number.ToString();
            }
            GetNextPost();
        }

        private void udPostNumber_ValueChanged(object sender, EventArgs e)
        {
            ShowSelectedPost();
        }
        private void ShowSelectedPost()
        {
            int postNumber = Convert.ToInt32(udPostNumber.Value);
            Post post = m_posts.FilteredPost(postNumber);
            if (post != null)
            {
                postArea.Text = post.Content;
            }
            else
            {
                postArea.Text = "No Data";
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            string search = txtSearch.Text;
            m_posts.FilterByString(search);
            udPostNumber.Value = 1;
            OnPostSetChanged();
        }
        private void OnPostSetChanged()
        {
            udPostNumber.Minimum = 1;
            udPostNumber.Maximum = m_posts.Count;
            txtCountPosts.Text = m_posts.Count.ToString();
            ShowSelectedPost();
        }
    }
    // This class holds the interesting info about a post.
    class Post
    {
        int m_postNumber;
        int m_Id;
        string m_poster;
        string m_content;
        public Post(int Number, int Id, string poster = "", string content = "")
        {
            m_postNumber = Number;
            m_Id = Id;
            m_poster = poster;
            m_content = content;
        }


        public int Id
        {
            get
            {
                return m_Id;
            }
        }
        public int Number
        {
            get
            {
                return m_postNumber;
            }
        }
        public string Poster
        {
            get
            {
                return m_poster;
            }
            set
            {
                m_poster = value;
            }
        }
        public string Content
        {
            get
            {
                return m_content;
            }
            set
            {
                m_content = value;
            }
        }
        public bool Search(string find)
        {
            bool rc = false;
            int ix = m_content.IndexOf(find, StringComparison.OrdinalIgnoreCase);
            if (ix > 0)
            {
                rc = true;
            }
            return rc;
        }
    }

    // This class holds all the posts
    class Posts
    {
        Dictionary<int, Post> m_Posts = new Dictionary<int, Post>();
        List<Post> m_FilteredPosts= new List<Post>();
        string m_FilterPoster = "";
        string m_FilterString = "";
        int m_minPost = 1;
        int m_maxPost = 0;
        public void AddPost(Post newPost)
        {
            m_Posts[newPost.Id] = newPost;
            if (newPost.Number < m_minPost)
            {
                m_minPost = newPost.Number;
            }
            if (newPost.Number > m_maxPost)
            {
                m_maxPost = newPost.Number;
            }
        }
        public Post GetPostByNumber(int number)
        {
            foreach (Post post in m_Posts.Values)
            {
                if (post.Number == number)
                {
                    return post;
                }
            }
            return null;
        }
        public Post GetPostById(int id)
        {
            Post post = m_Posts[id];
            return post;
        }
        public Post FilteredPost(int ix)
        {
            ix--;
            if (ix > m_FilteredPosts.Count)
            {
                return null;
            }
            Post rc = m_FilteredPosts[ix];
            return rc;
        }
        public void Clear()
        {
            m_Posts.Clear();
        }
        public int MinNumber
        {
            get
            {
                return m_minPost;
            }
        }
        public int MaxNumber
        {
            get
            {
                return m_maxPost;
            }
        }
        public int Count
        {
            get
            {
                int rc = m_FilteredPosts.Count;
                return rc;
            }
        }
        public void ClearFilters()
        {
            m_FilteredPosts = new List<Post>(m_Posts.Values);
        }
        public void ApplyFilters()
        {
            m_FilteredPosts.Clear();
            if (m_FilterString == "")
            {
                if (m_FilterPoster == "") // no filters
                {
                    ClearFilters();
                }
                else // just filter by poster
                {
                    foreach (Post post in m_Posts.Values)
                    {
                        if (post.Poster == m_FilterPoster)
                        {
                            m_FilteredPosts.Add(post);
                        }
                    }
                }
            }
            else
            {
                if (m_FilterPoster == "") // filter by search string
                {
                    foreach (Post post in m_Posts.Values)
                    {
                        if (post.Search(m_FilterString))
                        {
                            m_FilteredPosts.Add(post);
                        }
                    }
                }
                else // have both filters
                {
                    foreach (Post post in m_Posts.Values)
                    {
                        if (post.Poster == m_FilterPoster)
                        {
                            if (post.Search(m_FilterString))
                            {
                                m_FilteredPosts.Add(post);
                            }
                        }
                    }
                }
            }
        }
        public void FilterByPoster(string poster)
        {
            m_FilterPoster = poster;
            ApplyFilters();
        }
        public void FilterByString(string search)
        {
            m_FilterString = search;
            ApplyFilters();
        }
    }
}
