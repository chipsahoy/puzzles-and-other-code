using System;
using POG.Forum;

namespace FennecCount
{
	public partial class LoginDialog : Gtk.Dialog
	{
		private TwoPlusTwoForum _forum;
		public LoginDialog (TwoPlusTwoForum forum)
		{
			_forum = forum;
			this.Build ();
		}

		void _forum_LoginEvent(object sender, LoginEventArgs e)
		{
			switch (e.LoginEventType)
			{
			case POG.Forum.LoginEventType.LoginFailure:
			{
				Gtk.MessageDialog msg = new Gtk.MessageDialog(this, 
					Gtk.DialogFlags.DestroyWithParent, Gtk.MessageType.Error, Gtk.ButtonsType.Close,  
					"Login failed! Check the username and password.");
				msg.Run ();
				msg.Destroy ();
				btnLogin.Sensitive = true;
			}
				break;
				
			case POG.Forum.LoginEventType.LoginSuccess:
			{
				btnLogin.Sensitive = false;
				//txtUsername.ReadOnly = true;
				//txtPassword.ReadOnly = true;
				//txtPassword.PasswordChar = '*';
				if (chkRememberMe.Active)
				{
				}
				//DialogResult = System.Windows.Forms.DialogResult.OK;
			}
				break;
				
			case POG.Forum.LoginEventType.LogoutSuccess:
			{
				btnLogin.Sensitive = true;
				txtUsername.Text = "";
				txtPassword.Text = "";
				//txtUsername.ReadOnly = false;
				//txtPassword.ReadOnly = false;
				//txtPassword.PasswordChar = '\0';
				
			}
				break;
			}
		}

		protected void OnBtnLoginClicked (object sender, EventArgs e)
		{
			_forum.Login (txtUsername.Text, txtPassword.Text);
		}
	}
}

