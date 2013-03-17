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
using System.Collections.Specialized;

namespace POG.FennecFox
{
	public partial class FoxParent : Form
	{
		private const int WM_SETREDRAW = 11;

		[DllImport("user32.dll")]
		public static extern int SendMessage(IntPtr hWnd, Int32 wMsg, bool wParam, Int32 lParam); private int childFormNumber = 0;

		private TwoPlusTwoForum _forum;
		private Action<Action> _synchronousInvoker;
		IPogDb _db;

		public FoxParent()
		{
			InitializeComponent();
			if (POG.FennecFox.Properties.Settings.Default.updateSettings)
			{
				POG.FennecFox.Properties.Settings.Default.Upgrade();
				POG.FennecFox.Properties.Settings.Default.updateSettings = false;
				POG.FennecFox.Properties.Settings.Default.Save();
			}
		}
		void FoxParent_Load(object sender, EventArgs e)
		{
			_synchronousInvoker = a => Invoke(a);
			
			_db = new PogSqlite();
			String dbPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "/pog";
			System.IO.Directory.CreateDirectory(dbPath);
			String dbName = dbPath + "/pogposts.sqlite";
			_db.Connect(dbName);
			
			_forum = new TwoPlusTwoForum(_synchronousInvoker);
			_forum.LoginEvent += new EventHandler<LoginEventArgs>(_forum_LoginEvent);

			String username = POG.FennecFox.Properties.Settings.Default.username;
			String password = POG.FennecFox.Properties.Settings.Default.password;
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
		private void ShowCounter(String url, Boolean turbo, Int32 day)
		{
			Form childForm = new FormVoteCounter(_forum, _synchronousInvoker, _db, url, turbo, day);
			childForm.MdiParent = this;
			childForm.Text = "Window " + childFormNumber++;
			childForm.Show();
		}

		#region generated
		private void OpenFile(object sender, EventArgs e)
		{
			OpenGame frm = new OpenGame(_forum);
			DialogResult dr = frm.ShowDialog();
			if (dr == System.Windows.Forms.DialogResult.OK)
			{
				Boolean turbo;
				String url = frm.GetURL(out turbo);
				if (url.Length > 0)
				{
					ShowCounter(url, turbo, 1);
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
			StringCollection games = new StringCollection();
			foreach (Form child in MdiChildren)
			{
				FormVoteCounter counter = child as FormVoteCounter;
				if (counter != null)
				{
					String gameInfo = String.Format("{0}|{1}|{2}", counter.Turbo, counter.Day, counter.URL);
					games.Add(gameInfo);
				}
			}
			POG.FennecFox.Properties.Settings.Default.games = games;
			POG.FennecFox.Properties.Settings.Default.Save();


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
						ShowLogin();
					}
					break;

				case Forum.LoginEventType.LoginSuccess:
					{
						_loggedIn = true;
						openToolStripButton.Enabled = true;
						tsBtnLogout.Enabled = true;
						{
							StringCollection games = POG.FennecFox.Properties.Settings.Default.games;
							if (games == null)
							{
								games = new StringCollection();
								POG.FennecFox.Properties.Settings.Default.games = games;
								POG.FennecFox.Properties.Settings.Default.Save();
								OpenFile(this, EventArgs.Empty);
							}
							else
							{
								foreach (String game in games)
								{
									String[] parts = game.Split('|');
									if (parts.Length >= 3)
									{
										Boolean turbo = false;
										Boolean.TryParse(parts[0], out turbo);
										Int32 day = 1;
										Int32.TryParse(parts[1], out day);
										ShowCounter(parts[2], turbo, day);
									}
								}
							}
							//if (URLTextBox.Text == "")
							//{
							//    URLTextBox.Text = POG.FennecFox.Properties.Settings.Default.threadUrl;
							//    if (URLTextBox.Text != "")
							//    {
							//        _day = POG.FennecFox.Properties.Settings.Default.day;
							//        btnStartGame_Click(this, EventArgs.Empty);
							//    }
							//}
						}
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
