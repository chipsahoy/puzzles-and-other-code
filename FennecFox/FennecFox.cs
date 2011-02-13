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
        Dictionary<int, int> m_PostNumberToPostId = new Dictionary<int, int>();
        Dictionary<int, string> m_PostIdToString = new Dictionary<int, string>();
        int m_idPostFetching = 0;

        public Form1()
        {
            InitializeComponent();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void goButton_Click(object sender, EventArgs e)
        {
            webBrowser1.Navigate(addressBar.Text);
        }

        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            htmlSource.Text = webBrowser1.Document.Body.InnerHtml;
        }

        private void textBox1_TextChanged_1(object sender, EventArgs e)
        {

        }

        private void postGobutton_Click(object sender, EventArgs e)
        {
            int postNumber= Convert.ToInt32(udPostNumber.Value);
            int postId = m_PostNumberToPostId[postNumber];
            if (postId <= 0)
            {
                return;
            }
            string url = "http://forumserver.twoplustwo.com/newreply.php?do=newreply&p=" + postId.ToString();
            m_idPostFetching = postId;
            WebBrowserPost.Navigate(url);

        }
        private void WebBrowserPost_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            HtmlElement element= WebBrowserPost.Document.GetElementById("vB_Editor_001_textarea");
            if(element != null)
            {
                string post= element.InnerText;
                postArea.Text = post;
                m_PostIdToString[m_idPostFetching] = post;
                m_idPostFetching = 0;
            }
        }

        private void GoButtonAgain_Click(object sender, EventArgs e)
        {
            int ppp = Convert.ToInt32(textPostsPerPage.Text);
            if(ppp <= 0)
            {
                ppp= 50;
            }
            int firstPost = Convert.ToInt32(txtFirstPost.Text);
            if(firstPost <= 0)
            {
                firstPost= 1;
            }
            m_startPost = firstPost;

            m_endPost = Convert.ToInt32(txtLastPost.Text);
            if (m_endPost < m_startPost)
            {
                m_endPost = m_startPost;
            }
            string destination = URLTextBox.Text;
            int page = (firstPost / ppp) + 1;
            if (page > 1)
            {
                destination += "index" + page.ToString() + ".html";
            }

            WebBrowserPage.Navigate(destination);
            System.Console.WriteLine("destination is: " + destination);
        }

        int m_startPost = 1;
        int m_endPost = 50;
        private void WebBrowserPage_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            string answer = "";
            for (int i = m_startPost; i <= m_endPost; i++)
            {
                int postId = PostIdFromPostNumber(WebBrowserPage.Document, i);
                if (postId == 0)
                {
                    break;
                }
                m_PostNumberToPostId[i] = postId;
                answer += i.ToString() + ": " + postId + "\r\n";
            }
            AnswerTextBox.Text = answer;
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
            m_PostIdToString.Clear();
            m_PostNumberToPostId.Clear();
        }

    }
}
