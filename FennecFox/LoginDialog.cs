using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using POG.Forum;
using System.Reflection;

namespace POG.FennecFox
{
    public partial class LoginDialog : Form
    {
        private TwoPlusTwoForum _forum;
        public LoginDialog()
        {
            InitializeComponent();
        }

        public LoginDialog(TwoPlusTwoForum _forum) : this()
        {
            // TODO: Complete member initialization
            this._forum = _forum;
        }

        private void LoginDialog_Load(object sender, EventArgs e)
        {
            txtVersion.Text = String.Format("Fennic Fox Vote Counter Version " + Assembly.GetExecutingAssembly().GetName().Version.ToString());
            _forum.LoginEvent += new EventHandler<LoginEventArgs>(_forum_LoginEvent);

            POG.FennecFox.Properties.Settings.Default.username = String.Empty;
            POG.FennecFox.Properties.Settings.Default.password = String.Empty;
            POG.FennecFox.Properties.Settings.Default.Save();
        }

        void _forum_LoginEvent(object sender, LoginEventArgs e)
        {
            switch (e.LoginEventType)
            {
                case Forum.LoginEventType.LoginFailure:
                    {
                        MessageBox.Show(this, "Login failed! Check the username and password.");
                        btnLogin.Enabled = true;
                    }
                    break;

                case Forum.LoginEventType.LoginSuccess:
                    {
                        btnLogin.Enabled = false;
                        txtUsername.ReadOnly = true;
                        txtPassword.ReadOnly = true;
                        txtPassword.PasswordChar = '*';
                        if (chkRememberMe.Checked)
                        {
                            POG.FennecFox.Properties.Settings.Default.username = txtUsername.Text.Trim();
                            POG.FennecFox.Properties.Settings.Default.password = txtPassword.Text.Trim();

                            POG.FennecFox.Properties.Settings.Default.Save();
                        }
                        DialogResult = System.Windows.Forms.DialogResult.OK;
                    }
                    break;

                case Forum.LoginEventType.LogoutSuccess:
                    {
                        btnLogin.Enabled = true;
                        txtUsername.Text = "";
                        txtPassword.Text = "";
                        txtUsername.ReadOnly = false;
                        txtPassword.ReadOnly = false;
                        txtPassword.PasswordChar = '\0';
                        
                    }
                    break;
            }
        }
        private void Form_FormClosing(object sender, FormClosingEventArgs e)
        {
            _forum.LoginEvent -= _forum_LoginEvent;
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            btnLogin.Enabled = false;
            _forum.Login(txtUsername.Text, txtPassword.Text);

        }

    }
}
