using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using POG.Forum;

namespace FoxTester
{
    public partial class FoxTester : Form
    {
        POG.Forum.VBulletin_3_8_7 _forum;
        Int32 _nextPost = 1;
        public FoxTester()
        {
            InitializeComponent();
        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            _forum = new POG.Forum.VBulletin_3_8_7((x) => Invoke(x));
            _forum.NewPostsAvailable += new EventHandler<POG.Forum.NewPostsAvailableEventArgs>(_forum_NewPostEvent);
            _forum.PropertyChanged += new PropertyChangedEventHandler(_forum_PropertyChanged);
            _forum.StatusUpdate += new EventHandler<POG.Forum.NewStatusEventArgs>(_forum_StatusUpdate);
            _forum.FinishedReadingThread += new EventHandler(_forum_FinishedReadingThread);
            
        }

        void _forum_FinishedReadingThread(object sender, EventArgs e)
        {
           
        }

        void _forum_StatusUpdate(object sender, POG.Forum.NewStatusEventArgs e)
        {
            txtStatus.Text = e.Status;
        }

        void _forum_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Console.WriteLine("Property Changed: " + e.PropertyName);
            if (e.PropertyName == "ThreadURL")
            {
                lbPosts.Items.Clear();
            }
        }

        void _forum_PMReadEvent(object sender, POG.Forum.PMReadEventArgs e)
        {
            throw new NotImplementedException();
        }

        void _forum_NewPostEvent(object sender, POG.Forum.NewPostsAvailableEventArgs e)
        {
            Post[] posts = _forum.GetPosts(_nextPost, e.NewestPostNumber);
            foreach (Post post in posts)
            {
                lbPosts.Items.Add(post.PostNumber.ToString() + " " + post.Poster);
            }
            _nextPost = e.NewestPostNumber + 1;
        }

        void _forum_NewPMEvent(object sender, POG.Forum.NewPMEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            _forum.ThreadURL = textBox1.Text;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            _forum.RefreshNow();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            _forum.Start();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            _forum.Stop();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            _forum.ThreadURL = textBox1.Text;
        }
    }
}
