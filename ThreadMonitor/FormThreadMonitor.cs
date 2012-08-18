using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using POG.Forum;

namespace ThreadMonitor
{
    public partial class frmThreadMonitor : Form
    {
        VBulletin_3_8_7 _forum;
        public frmThreadMonitor()
        {
            InitializeComponent();
            Action<Action> synchronousInvoker = a => Invoke(a);
            _forum = new VBulletin_3_8_7(synchronousInvoker);
            _forum.StatusUpdate += new EventHandler<NewStatusEventArgs>(_forum_StatusUpdate);
            _forum.PropertyChanged += new PropertyChangedEventHandler(_forum_PropertyChanged);
            //_forum.NewPostsAvailable += new EventHandler<NewPostsAvailableEventArgs>(_forum_NewPostsAvailable);
            _forum.LoginEvent += new EventHandler<LoginEventArgs>(_forum_LoginEvent);
            _forum.FinishedReadingThread += new EventHandler(_forum_FinishedReadingThread);
        }

        void _forum_FinishedReadingThread(object sender, EventArgs e)
        {
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
                        btnLogin.Enabled = false;
                        btnLogout.Enabled = true;
                        txtUsername.ReadOnly = true;
                        txtPassword.ReadOnly = true;
                        txtPassword.PasswordChar = '*';
                    }
                    break;

                case LoginEventType.LogoutSuccess:
                    {
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

        void _forum_NewPostsAvailable(object sender, PostEventArgs e)
        {
        }

        void _forum_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
        }

        void _forum_StatusUpdate(object sender, NewStatusEventArgs e)
        {
            statusText.Text = e.Status;
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            btnLogin.Enabled = false;
            _forum.Login(txtUsername.Text, txtPassword.Text);

        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            _forum.Logout();
        }

    }
}
