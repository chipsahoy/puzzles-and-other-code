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
            string url = "http://forumserver.twoplustwo.com/newreply.php?do=newreply&p=" + PostNumberTextBox.Text;
            WebBrowserPost.Navigate(url);

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string post = WebBrowserPost.Document.GetElementById("vB_Editor_001_textarea").InnerText;
            postArea.Text = post;
        }

        private void textBox1_TextChanged_2(object sender, EventArgs e)
        {

        }

        private void WebBrowserPost_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
           
        }

        private void GoButtonAgain_Click(object sender, EventArgs e)
        {
            string destination = URLTextBox.Text;
            string foo = "wat";
            WebBrowserPage.Navigate(destination);
            System.Console.WriteLine("destination is: " + destination);
        }

        private void WebBrowserPage_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {

        }

        private void SecondButton_Click(object sender, EventArgs e)
        {
            HtmlElementCollection elements = WebBrowserPage.Document.GetElementsByTagName("1");
            int i = 0;
           
        }
    }
}
