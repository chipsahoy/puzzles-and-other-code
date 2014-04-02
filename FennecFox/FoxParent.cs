using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using POG.Forum;
using POG.Werewolf;
using POG.Utils;
using System.Collections.Specialized;

namespace POG.FennecFox
{
	public partial class FoxParent : Form
	{
		private const int WM_SETREDRAW = 11;

		[DllImport("user32.dll")]
		public static extern int SendMessage(IntPtr hWnd, Int32 wMsg, bool wParam, Int32 lParam); 

		private VBulletinForum _forum;
        Language _language = Language.English;
		private Action<Action> _synchronousInvoker;
		IPogDb _db;
        String _forumName;

		public FoxParent(String forumName)
		{
            _forumName = forumName;
            InitializeComponent();
		}
		void FoxParent_Load(object sender, EventArgs e)
		{
			_synchronousInvoker = a => Invoke(a);
			//String host = "forumserver.twoplustwo.com";
            String host = _forumName;
            String forumRoot = "";
            String lobby;
            String vbVersion;
            String voteRegex = "";

            switch (host)
            {
                case "foorum.pokkeriprod.com":
                    {
                        vbVersion = "4.2.0";
                        lobby = "forumdisplay.php/9-Võistlused";
                        _language = Language.Estonian;
                    }
                    break;

                case "mindromp.org":
                    {
                        vbVersion = "3.8.7";
                        lobby = "forumdisplay.php?f=17";
                        _language = Language.English;
                        forumRoot = "/forum";
                        voteRegex = "##(.*)##";
                    }
                    break;

                default:
                    {
                        vbVersion = "3.8.7";
                        lobby = "59/puzzles-other-games/";
                        _language = Language.English;
                    }
                    break;
            }
            String dbPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\pog\";
			System.IO.Directory.CreateDirectory(dbPath);
			String dbName = String.Format("{0}posts.{1}.sqlite", dbPath, host);
			_db = new PogSqlite();
			_db.Connect(dbName);
			
			_forum = new VBulletinForum(_synchronousInvoker, host, vbVersion, lobby, forumRoot, voteRegex);
			_forum.LoginEvent += new EventHandler<LoginEventArgs>(_forum_LoginEvent);

			String username = PogSettings.Read("username", String.Empty);
			String password = PogSettings.Read("password", String.Empty);
			if ((username != String.Empty) && (password != String.Empty))
			{
				_forum.Login(username, password);
			}
			else
			{
				ShowLogin();
			}
		}

		private void ShowNewForm(object sender, EventArgs e)
		{
		}
		private void ShowCounter(String url, String OP, Boolean turbo, Int32 day)
		{
			Form childForm = new FormVoteCounter(_forum, _synchronousInvoker, _db, url, turbo, day, _language);
			childForm.MdiParent = this;
			childForm.Text = OP;
			childForm.Show();
		}

		#region generated
		private void OpenFile(object sender, EventArgs e)
		{
            OpenGame frm = new OpenGame(_forum, _forum.ForumURL + _forum.ForumLobby, new List<String> { "Spade", "Club" });
			DialogResult dr = frm.ShowDialog();
			if (dr == System.Windows.Forms.DialogResult.OK)
			{
				Boolean turbo;
                String op;
				String url = frm.GetURL(out op, out turbo);
				if (url.Length > 0)
				{
					ShowCounter(url, op, turbo, 1);
				}
			}
		}
		private void SaveAsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SaveFileDialog saveFileDialog = new SaveFileDialog();
			saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
			saveFileDialog.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
			if (saveFileDialog.ShowDialog(this) == DialogResult.OK)
			{
				string FileName = saveFileDialog.FileName;
			}
		}

		private void ExitToolsStripMenuItem_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		private void CutToolStripMenuItem_Click(object sender, EventArgs e)
		{
		}

		private void CopyToolStripMenuItem_Click(object sender, EventArgs e)
		{
		}

		private void PasteToolStripMenuItem_Click(object sender, EventArgs e)
		{
		}


		private void CascadeToolStripMenuItem_Click(object sender, EventArgs e)
		{
			LayoutMdi(MdiLayout.Cascade);
		}

		private void TileVerticalToolStripMenuItem_Click(object sender, EventArgs e)
		{
			LayoutMdi(MdiLayout.TileVertical);
		}

		private void TileHorizontalToolStripMenuItem_Click(object sender, EventArgs e)
		{
			LayoutMdi(MdiLayout.TileHorizontal);
		}

		private void ArrangeIconsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			LayoutMdi(MdiLayout.ArrangeIcons);
		}

		private void CloseAllToolStripMenuItem_Click(object sender, EventArgs e)
		{
			foreach (Form childForm in MdiChildren)
			{
				childForm.Close();
			}
		}
		private void ActiveMdiChild_FormClosed(object sender,
									FormClosedEventArgs e)
		{
			((sender as Form).Tag as TabPage).Dispose();
		}
		private void FoxParent_MdiChildActivate(object sender, EventArgs e)
		{
			if (this.ActiveMdiChild == null)
			{
				tabForms.Visible = false;
			}
			// If no any child form, hide tabControl 
			else
			{
				this.ActiveMdiChild.WindowState =
				FormWindowState.Maximized;
				// Child form always maximized 

				// If child form is new and no has tabPage, 
				// create new tabPage 
				if (this.ActiveMdiChild.Tag == null)
				{
					// Add a tabPage to tabControl with child 
					// form caption 
					TabPage tp = new TabPage(this.ActiveMdiChild
											 .Text);
					tp.Tag = this.ActiveMdiChild;
					tp.Parent = tabForms;
					tabForms.SelectedTab = tp;

					this.ActiveMdiChild.Tag = tp;
					this.ActiveMdiChild.FormClosed +=
						new FormClosedEventHandler(
										ActiveMdiChild_FormClosed);
				}
				else
				{
					TabPage tp = this.ActiveMdiChild.Tag as TabPage;
					if (tp != null)
					{
						tabForms.SelectedTab = tp;
					}
				}
				if (!tabForms.Visible)
				{
					tabForms.Visible = true;
				}
			}
		}

		private void tabForms_SelectedIndexChanged(object sender, EventArgs e)
		{
			if ((tabForms.SelectedTab != null) &&
						(tabForms.SelectedTab.Tag != null))
			{
				SendMessage(this.Handle, WM_SETREDRAW, false, 0);
				(tabForms.SelectedTab.Tag as Form).Select();
				SendMessage(this.Handle, WM_SETREDRAW, true, 0);
				this.Refresh();
			}
		}
#endregion

		private void FoxParent_FormClosing(object sender, FormClosingEventArgs e)
		{
			_forum.LoginEvent -= _forum_LoginEvent;
		}
		Boolean _inLoginDialog = false;
		Boolean _loggedIn = false;

		private void ShowLogin()
		{
			if (!_inLoginDialog)
			{
				_inLoginDialog = true;
				LoginDialog dlg = new LoginDialog(_forum);
				DialogResult dr = dlg.ShowDialog();
				if (!_loggedIn)
				{
					Application.Exit();
				}
				else
				{
					statusStrip.Text = "Logged in to forum";
				}
				_inLoginDialog = false;
			}
		}
		private void _forum_LoginEvent(object sender, POG.Forum.LoginEventArgs e)
		{
			switch (e.LoginEventType)
			{
				case Forum.LoginEventType.LoginFailure:
					{
						BeginInvoke(new MethodInvoker(ShowLogin));
					}
					break;

				case Forum.LoginEventType.LoginSuccess:
					{
						_loggedIn = true;
						openToolStripButton.Enabled = true;
						tsBtnLogout.Enabled = true;
                        this.Text = String.Format("Fennec Fox Vote Counter -- Logged in as {0}", e.Username);
                    }
					break;

				case Forum.LoginEventType.LogoutSuccess:
					{
						_loggedIn = false;
						openToolStripButton.Enabled = false;
						CloseAllToolStripMenuItem_Click(this, EventArgs.Empty);
					}
					break;
			}
		}

		private void tsBtnLogout_Click(object sender, EventArgs e)
		{
			_forum.Logout();
			ShowLogin();
		}

		private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Form frm = new AboutFennec();
			frm.ShowDialog();
		}

	}
}
