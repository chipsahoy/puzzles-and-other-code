using System;
using System.Collections.Generic;
using System.Text;

namespace FennecWidget
{
	public partial class PlayerList : Gtk.Dialog
	{
		public PlayerList (List<String> players)
		{
			this.Build ();
			StringBuilder list = new StringBuilder();
			foreach (String s in players) {
				list.AppendLine (s);
			}
			txtList.Buffer.Text = list.ToString ();
		}
		public List<String> GetPlayers()
		{
			String s = txtList.Buffer.Text;
			List<String> rc = new List<String>();		
			String[] lines = s.Split(new String[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
			foreach (String line in lines)
			{
				String name = line.Trim();
				rc.Add (name);
			}
			return rc;
		}
	}
}
