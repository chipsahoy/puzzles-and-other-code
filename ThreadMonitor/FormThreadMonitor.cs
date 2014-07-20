using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using POG.Forum;
using POG.Database;
using System.Diagnostics;

namespace ThreadMonitor
{
    public partial class frmThreadMonitor : Form
    {
        VBulletinForum _forum;
        LobbyReader _lobby;
        CheckMyStats _cms;
        String _username;

        public frmThreadMonitor()
        {
            InitializeComponent();
            Action<Action> synchronousInvoker = a => Invoke(a);
            _forum = new VBulletinForum(synchronousInvoker, "forumserver.twoplustwo.com", "3.8.7", Language.English, "59/puzzles-other-games/");
            _forum.StatusUpdate += new EventHandler<NewStatusEventArgs>(_forum_StatusUpdate);
            _forum.LoginEvent += new EventHandler<LoginEventArgs>(_forum_LoginEvent);
        }

        void _forum_LoginEvent(object sender, LoginEventArgs e)
        {
            switch (e.LoginEventType)
            {
                case LoginEventType.LoginFailure:
                    {
                        MessageBox.Show(this, "Login failed! Check the username and password.");
                        btnLogin.Enabled = true;
                    }
                    break;

                case LoginEventType.LoginSuccess:
                    {
                        button1.Enabled = true;
                        btnLogin.Enabled = false;
                        btnLogout.Enabled = true;
                        txtUsername.ReadOnly = true;
                        txtPassword.ReadOnly = true;
                        txtPassword.PasswordChar = '*';
                        _lobby = _forum.Lobby();
                        _lobby.LobbyPageCompleteEvent += new EventHandler<LobbyPageCompleteEventArgs>(_lobby_LobbyPageCompleteEvent);
                        _cms = new CheckMyStats();
                        _cms.LobbyReadEvent += new EventHandler<LobbyReadEventArgs>(_cms_LobbyReadEvent);
                        _cms.ThreadReadEvent += new EventHandler<ThreadReadEventArgs>(_cms_ThreadReadEvent);
                        //_cms.MessageReceived += new EventHandler<MessageEventArgs>(_cms_MessageReceived);
                        _cms.Login(_username);
                    }
                    break;

                case LoginEventType.LogoutSuccess:
                    {
                        _cms.Logout(_username);
                        button1.Enabled = false;
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

        void thread_PageCompleteEvent(object sender, PageCompleteEventArgs e)
        {
            ThreadReader tr = sender as ThreadReader;
            ThreadReadEventArgs readArgs = e.Cookie as ThreadReadEventArgs;
            foreach (Post p in e.Posts)
            {
                if ((p.PostNumber >= readArgs.StartPost) && (p.PostNumber <= readArgs.EndPost))
                {
                    _cms.PublishPost(readArgs.URL, readArgs.ID, p, e.TimeStamp);
                }
            }
        }
        void RunThreadRequest(ThreadReadEventArgs e)
        {
            _currentThreadReadRequest = e;
            Trace.TraceInformation("Request to read {0}", e.URL);
            Int32 pageStart = (e.StartPost - 1) / _forum.PostsPerPage;
            Int32 pageEnd = (e.EndPost - 1) / _forum.PostsPerPage;
            ThreadReader thread = _forum.Reader();
            thread.PageCompleteEvent += new EventHandler<PageCompleteEventArgs>(thread_PageCompleteEvent);
            thread.ReadCompleteEvent += new EventHandler<ReadCompleteEventArgs>(thread_ReadCompleteEvent);
            thread.ReadPages(e.URL, pageStart, pageEnd, e);
        }

        void thread_ReadCompleteEvent(object sender, ReadCompleteEventArgs e)
        {
            _currentThreadReadRequest = null;
            if (_threadReadRequests.Count > 0)
            {
                ThreadReadEventArgs next = _threadReadRequests.Dequeue();
                if (next != null)
                {
                    RunThreadRequest(next);
                }
            }
        }

        ThreadReadEventArgs _currentThreadReadRequest;
        Queue<ThreadReadEventArgs> _threadReadRequests = new Queue<ThreadReadEventArgs>();
        void _cms_ThreadReadEvent(object sender, ThreadReadEventArgs e)
        {
            if (_currentThreadReadRequest == null)
            {
                RunThreadRequest(e);
            }
            else
            {
                _threadReadRequests.Enqueue(e);
            }
        }

        void _cms_LobbyReadEvent(object sender, LobbyReadEventArgs e)
        {
            _lobby.ReadLobby(e.URL, e.First, e.Last, e.RecentFirst);
        }

        void _lobby_LobbyPageCompleteEvent(object sender, LobbyPageCompleteEventArgs e)
        {
            Trace.TraceInformation("Done Page {0} of {1}", e.Page, e.URL);
            foreach (ForumThread t in e.Threads)
            {
                _cms.PublishLobbyPage("0", t, e.TimeStamp);
            }
        }

        private void ReadForum(String url)
        {
            _lobby.ReadLobby(url, 1, 1, true);
        }

        void _forum_StatusUpdate(object sender, NewStatusEventArgs e)
        {
            statusText.Text = e.Status;
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            btnLogin.Enabled = false;
            _username = txtUsername.Text;
            _forum.Login(_username, txtPassword.Text);

        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            _forum.Logout();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ReadForum("http://forumserver.twoplustwo.com/59/puzzles-other-games/");
            ReadThread("http://forumserver.twoplustwo.com/59/puzzles-other-games/9-6-vanilla-werewolf-signup-1242009/");
        }
        private void ReadThread(String url)
        {
            ThreadReadEventArgs e = new ThreadReadEventArgs(url, 1, 100, "chips");
            _cms_ThreadReadEvent(this, e);
        }

    }
}
