using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using POG.Forum;
using POG.Utils;
using POG.Werewolf;
using System.Reflection;

namespace Titan
{
    public partial class Titan : Form
    {
        VBulletinForum _forum;
        Action<Action> _synchronousInvoker;
        StateMachineHost _host = new StateMachineHost("turbo");
        AutoComplete _autoComplete;
        IPogDb _db;
        GameStarter _gameStarter;

        public Titan()
        {
            InitializeComponent();
            txtVersion.Text = String.Format("Version {0}", AssemblyVersion);
            String host = "forumserver.twoplustwo.com";
            _synchronousInvoker = a => Invoke(a);
            _forum = new VBulletinForum(_synchronousInvoker, host, "3.8.7", Language.English, "59/puzzles-other-games/");
            _forum.LoginEvent += new EventHandler<LoginEventArgs>(_forum_LoginEvent);

            txtUsername.Text = PogSettings.Read("TitanModName");
            txtPassword.Text = PogSettings.Read("TitanModPassword");

            String dbPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "/pog/";
            System.IO.Directory.CreateDirectory(dbPath);
            String dbName = String.Format("{0}posts.{1}.sqlite", dbPath, host);
            _db = new PogSqlite();
            _db.Connect(dbName);

            Action<Action> invoker = a => a();
            _autoComplete = new AutoComplete(_forum, invoker, _db);

        }
        public string AssemblyVersion
        {
            get
            {
                return Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
        }


        void LoadGame()
        {
            _gameStarter = new GameStarter(_forum, _host, _db, _autoComplete);
        }

        void _forum_LoginEvent(object sender, LoginEventArgs e)
        {
            switch (e.LoginEventType)
            {
                case LoginEventType.LoginSuccess:
                    {
                        this.Text += " : " + e.Username;
                        if (chkRememberMe.Checked)
                        {
                            PogSettings.Write("TitanModName", e.Username);
                            PogSettings.Write("TitanModPassword", txtPassword.Text);
                        }
                        else
                        {
                            PogSettings.Write("TitanModName", "");
                            PogSettings.Write("TitanModPassword", "");
                        }
                        
                        LoadGame();
                    }
                    break;

                case LoginEventType.LoginFailure:
                    {
                        MessageBox.Show("Login Failure as " + e.Username);
                        txtPassword.ReadOnly = false;
                        txtUsername.ReadOnly = false;
                        chkRememberMe.Enabled = true;
                        btnLogin.Enabled = true;
                    }
                    break;
            }
        }


        private void btnLogin_Click(object sender, EventArgs e)
        {
            btnLogin.Enabled = false;
            txtUsername.ReadOnly = true;
            txtPassword.ReadOnly = true;
            chkRememberMe.Enabled = false;
            String username = txtUsername.Text.Trim();
            String password = txtPassword.Text;
            _forum.Login(username, password);

        }

    }
}
