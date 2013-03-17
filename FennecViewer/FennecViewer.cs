using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SQLite;
using System.Diagnostics;
using POG.Utils;

namespace FennecViewer
{
	public partial class FennecViewer : Form
	{
		String _connect;
		String _dbName;
		Int32 _threadId;

		public FennecViewer()
		{
			InitializeComponent();
			_dbName = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\POG\\pogposts.sqlite";
			_connect = String.Format("Data Source={0};Version=3;", _dbName);
		}

		private void txtURL_TextChanged(object sender, EventArgs e)
		{
		}

		private void btnGo_Click(object sender, EventArgs e)
		{
			String url = Misc.NormalizeUrl(txtURL.Text);
			_threadId = Misc.TidFromURL(url);
			List<String> posters = GetPosters(_threadId);
			cmbPoster.Items.Clear();
			foreach (String poster in posters)
			{
				cmbPoster.Items.Add(poster);
			}
		}
		public List<String> GetPosters(Int32 threadId)
		{
			List<String> posters = new List<string>();
			String sql = @"SELECT DISTINCT posters.Name
						FROM posters
						INNER JOIN posts ON (posters.id = posts.posterId)
						WHERE (posts.threadId = @p1)
						ORDER BY posters.Name ASC;";
			using (SQLiteConnection dbRead = new SQLiteConnection(_connect))
			{
				dbRead.Open();
				using (SQLiteCommand cmd = new SQLiteCommand(sql, dbRead))
				{
					cmd.Parameters.Add(new SQLiteParameter("@p1", threadId));
					using (SQLiteDataReader r = cmd.ExecuteReader())
					{
						while (r.Read())
						{
							String poster = r.GetString(0);
							posters.Add(poster);
						}
					}
				}
			}
			Trace.TraceInformation("after GetPlayerList");
			return posters;
		}

		private void cmbPoster_SelectedIndexChanged(object sender, EventArgs e)
		{
			String player = cmbPoster.Items[cmbPoster.SelectedIndex] as String;
			lbPosts.Items.Clear();
			if (player != String.Empty)
			{
				List<Int32> posts = GetPostList(_threadId, player);
				foreach (Int32 post in posts)
				{
					lbPosts.Items.Add(post.ToString());
				}
			}
		}

		private List<Int32> GetPostList(int _threadId, string poster)
		{
			List<Int32> posts = new List<Int32>();
			String sql = @"SELECT DISTINCT 
posts.id, posts.number, posts.content, posts.title, posts.time
						FROM posts
						INNER JOIN posters ON (posters.id = posts.posterId)
						WHERE (posts.threadId = @p1)
						AND (posters.name = @p2)
						ORDER BY posts.number ASC;";
			using (SQLiteConnection dbRead = new SQLiteConnection(_connect))
			{
				dbRead.Open();
				using (SQLiteCommand cmd = new SQLiteCommand(sql, dbRead))
				{
					cmd.Parameters.Add(new SQLiteParameter("@p1", _threadId));
					cmd.Parameters.Add(new SQLiteParameter("@p2", poster));

					using (SQLiteDataReader r = cmd.ExecuteReader())
					{
						while (r.Read())
						{
							Int32 number = r.GetInt32(1);
							posts.Add(number);
						}
					}
				}
			}
			Trace.TraceInformation("after GetPostList");
			return posts;
		}

		private void lbPosts_SelectedIndexChanged(object sender, EventArgs e)
		{
			String number = lbPosts.SelectedItem as String;
			Int32 postNumber = Int32.Parse(number);
			String html = GetPostContent(postNumber);
			wbPost.DocumentText = html;
		}

		private string GetPostContent(int postNumber)
		{
			String sql = @"SELECT DISTINCT 
						posts.content
						FROM posts
						WHERE (posts.threadId = @p1)
						AND (posts.number = @p2)
						LIMIT 1;";
			String div = "<div>* NO CONTENT *</div>";
			using (SQLiteConnection dbRead = new SQLiteConnection(_connect))
			{
				dbRead.Open();
				using (SQLiteCommand cmd = new SQLiteCommand(sql, dbRead))
				{
					cmd.Parameters.Add(new SQLiteParameter("@p1", _threadId));
					cmd.Parameters.Add(new SQLiteParameter("@p2", postNumber));

					using (SQLiteDataReader r = cmd.ExecuteReader())
					{
						if(r.Read())
						{
							div = r.GetString(0);
						}
					}
				}
			}
			Trace.TraceInformation("after GetPostContent");
			String rc = "<html><body>" + div + "</body></html>";
			return rc;
		}
	}
}
