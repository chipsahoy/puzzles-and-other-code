using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using POG.Forum;
using POG.Utils;
using POG.Werewolf;

namespace POG.FennecFox
{
	public partial class FormVoteCounter : Form
	{
		private enum CounterColumn
		{
			Player = 0,
			VoteCount,
			PostCount,
			PostNumber,
			PostLink,
			PostTime,
			VotesFor,
			Bolded,
		};

		private ElectionInfo _voteCount;
		private VBulletinForum _forum;
        Language _language;
		IPogDb _db;
		Moderator _moderator;
		private Action<Action> _synchronousInvoker;
		private System.Windows.Forms.Timer _timerEODCountdown = new System.Windows.Forms.Timer();
		private Int32 _day = 1;
		String _url;
		Boolean _turbo;

		#region initialization
		public FormVoteCounter()
		{
			InitializeComponent();
		}
		public FormVoteCounter(VBulletinForum forum, Action<Action> synchronousInvoker, IPogDb db,
			String url, Boolean turbo, Int32 day, Language language) : this()
		{
			_forum = forum;
			_synchronousInvoker = synchronousInvoker;
			_db = db;
			_url = url;
			_turbo = turbo;
			_day = day;
            _language = language;
		}

		protected override void OnLoad(EventArgs e)
		{
			DateTime now = DateTime.Now;
			CreateVoteGridColumns();
			SetupVoteGrid();
			BindToNewGame(_url);
			
			
			_timerEODCountdown.Interval = 1000;
			_timerEODCountdown.Tick += new EventHandler(_timerEODCountdown_Tick);
			_timerEODCountdown.Start();

			if (_voteCount.LivePlayers.Count == 0)
			{
				btnRoster_Click(btnRoster, EventArgs.Empty);
			}

		}
		#endregion

		private void SetupVoteGrid()
		{
			if (_voteCount == null)
			{
				return;
			}
			BindingSource bs = new BindingSource();
			bs.DataSource = _voteCount.LivePlayers;
			grdVotes.DataSource = null;

			grdVotes.DataSource = bs;
			grdVotes.Sort(grdVotes.Columns[(Int32)CounterColumn.PostTime], ListSortDirection.Descending);
		}

		private void CreateVoteGridColumns()
		{
			grdVotes.AutoGenerateColumns = false;
			grdVotes.EditMode = DataGridViewEditMode.EditOnEnter;

			DataGridViewColumn col = new DataGridViewTextBoxColumn();
			col.DataPropertyName = "Name";
			col.HeaderText = "Player";
			col.ReadOnly = true;
			col.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
			col.Resizable = DataGridViewTriState.False;
			grdVotes.Columns.Insert((Int32)CounterColumn.Player, col);

			col = new DataGridViewTextBoxColumn();
			col.DataPropertyName = "VoteCount";
			col.HeaderText = "Votes";
			col.ReadOnly = true;
			col.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
			col.Resizable = DataGridViewTriState.False;
			grdVotes.Columns.Insert((Int32)CounterColumn.VoteCount, col);

			col = new DataGridViewTextBoxColumn();
			col.DataPropertyName = "PostCount";
			col.HeaderText = "Posts";
			col.ReadOnly = true;
			col.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
			col.Resizable = DataGridViewTriState.False;
			col.DividerWidth = 3;
			grdVotes.Columns.Insert((Int32)CounterColumn.PostCount, col);

			col = new DataGridViewLinkColumn();
			col.DataPropertyName = "PostNumber";
			col.HeaderText = "Post";
			col.ReadOnly = true;
			col.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
			col.Resizable = DataGridViewTriState.False;
			grdVotes.Columns.Insert((Int32)CounterColumn.PostNumber, col);

			col = new DataGridViewTextBoxColumn();
			col.DataPropertyName = "PostLink";
			col.HeaderText = "Link";
			col.ReadOnly = true;
			col.Visible = false;
			grdVotes.Columns.Insert((Int32)CounterColumn.PostLink, col);

			col = new DataGridViewTextBoxColumn();
			col.DataPropertyName = "PostTime";
			col.HeaderText = "Time";
			col.DefaultCellStyle.Format = "HH:mm";
			col.ReadOnly = true;
			col.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
			col.Resizable = DataGridViewTriState.False;
			grdVotes.Columns.Insert((Int32)CounterColumn.PostTime, col);

            col = new DataGridViewTextBoxColumn();
            col.DataPropertyName = "Votee";
			col.HeaderText = "Votes For";
            col.ReadOnly = true;
			grdVotes.Columns.Insert((Int32)CounterColumn.VotesFor, col);

			col = new DataGridViewTextBoxColumn();
			col.DataPropertyName = "Bolded";
			col.HeaderText = "Content";
			col.ReadOnly = true;
			grdVotes.Columns.Insert((Int32)CounterColumn.Bolded, col);

			grdVotes.CellContentClick += new DataGridViewCellEventHandler(grdVotes_CellContentClick);
			grdVotes.CellFormatting += new DataGridViewCellFormattingEventHandler(grdVotes_CellFormatting);
		}

		private void grdVotes_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
		{
			if (e.RowIndex < 0)
			{
				return;
			}
			switch (e.ColumnIndex)
			{
				case (Int32)CounterColumn.PostNumber:
					{
						if (((Int32)e.Value) < 1)
						{
							e.Value = "";
						}
					}
					break;

				case (Int32)CounterColumn.PostTime:
					{
						if (((DateTimeOffset)e.Value) == DateTimeOffset.MinValue)
						{
							e.Value = "";
						}
					}
					break;

				case (Int32)CounterColumn.VotesFor:
					{
						if (_voteCount != null)
						{
							if (((String)e.Value) == _voteCount.ErrorVote)
							{
								e.CellStyle.BackColor = System.Drawing.Color.Red;
							}
						}
					}
					break;
			}
		}

		private void grdVotes_CellContentClick(object sender, DataGridViewCellEventArgs e)
		{
			if (e.RowIndex < 0)
			{
				return;
			}
			switch (e.ColumnIndex)
			{
				case (Int32)CounterColumn.PostNumber:
					{
						DataGridViewRow row = grdVotes.Rows[e.RowIndex];
						Int32 PostNumber = (Int32)row.Cells[e.ColumnIndex].Value;
						if (PostNumber > 0)
						{
							String url = (String)row.Cells[(Int32)CounterColumn.PostLink].Value;
							if (url != "")
							{
								System.Diagnostics.Process.Start(url);
							}
						}
					}
					break;
			}
		}

		private void _voteCount_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (!Visible)
			{
				return;
			}
			if (e.PropertyName == "LivePlayers")
			{
				SetupVoteGrid();
			}
			if (e.PropertyName == "StartTime")
			{
				String time = String.Empty;
				if (_voteCount.StartTime != null)
				{
					time = _voteCount.StartTime.Value.ToString("g");
				}
				dtStartTime.Text = time;
			}
			if (e.PropertyName == "EndTime")
			{
				String time = String.Empty;
				time = _voteCount.EndTime.ToString("g");
				dtEndTime.Text = time;
			}
			if (e.PropertyName == "Status")
			{
				statusText.Text = _voteCount.Status; // no direct binding support in status strip.
                ShowMajorityStatus();
			}
		}
        void ShowMajorityStatus()
        {
            if (_voteCount.CheckMajority && (_voteCount.MajorityPost > 0))
            {
                chkUseMajority.BackColor = System.Drawing.Color.Red;
            }
            else
            {
                chkUseMajority.BackColor = Control.DefaultBackColor;
            }
        }
		private void _timerEODCountdown_Tick(object sender, EventArgs e)
		{
			DateTime now = DateTime.Now;
			_timerEODCountdown.Interval = 1000 - now.Millisecond;

			if (_voteCount == null)
			{
				txtCountDown.Text = "No Game";
				return;
			}
			TimeSpan tsRemaining = _voteCount.TimeUntilNight;
			if (tsRemaining.Ticks < 0)
			{
				txtCountDown.Text = "Night!";
				return;
			}
			switch (tsRemaining.Days)
			{
				case 0:
					{
						lblDaysToEOD.Visible = false;
					}
					break;

				case 1:
					{
						lblDaysToEOD.Text = "1 day and";
						lblDaysToEOD.Visible = true;
					}
					break;

				default:
					{
						lblDaysToEOD.Text = tsRemaining.Days.ToString() + " days and";
						lblDaysToEOD.Visible = true;
					}
					break;
			
			}
			txtCountDown.Text = String.Format("{0:00}:{1:00}:{2:00}",
					tsRemaining.Hours, tsRemaining.Minutes, tsRemaining.Seconds);
			if (tsRemaining.TotalSeconds == 120)
			{
				FlashWindow.Flash(this);
			}
		}

		private delegate void ErrorDelegate(Exception e);

		private void HandleError(Exception e)
		{
			if (InvokeRequired)
			{
				Invoke(new ErrorDelegate(HandleError), new object[] { e });
				return;
			}

			String file = Directory.GetCurrentDirectory() + @"\error.log";
			File.AppendAllText(file, DateTime.Now + ": " + e.ToString() + "\n\n");
			MessageBox.Show(this,
							String.Format(
								"An error has occurred and a crashdump was created at \n\n{0}\n\nPlease send the log file to the developer.  The message was:\n\n{1}",
								file, e.Message), "Critical Error");
		}

		private void SetComboRange(DataGridViewComboBoxCell cell)
		{
			object v = cell.Value;
			cell.Items.Clear();
			cell.Items.AddRange(_voteCount);
			if (v != null && cell.Items.Contains(v))
			{
				cell.Value = v;
			}
			else
			{
				cell.Value = cell.Items[0];
			}
		}

		private void BindToNewGame(String url)
		{
			url = Utils.Misc.NormalizeUrl(url);
			ThreadReader t = _forum.Reader();
			_voteCount = new ElectionInfo(_synchronousInvoker, t, _db, _forum.ForumURL, url, _forum.PostsPerPage, _language, _forum.VBVersion);
			_voteCount.PropertyChanged += new PropertyChangedEventHandler(_voteCount_PropertyChanged);
			_voteCount.Turbo = _turbo;
			_moderator = new Moderator(_synchronousInvoker, _voteCount, _forum);

			txtLastPost.DataBindings.Clear();
			udStartPost.DataBindings.Clear();
			txtEndPost.DataBindings.Clear();
			dtEndTime.DataBindings.Clear();
			dtStartTime.DataBindings.Clear();
			
			txtLastPost.DataBindings.Add("Text", _voteCount, "LastPost", false, DataSourceUpdateMode.OnPropertyChanged);
			udStartPost.DataBindings.Add("Text", _voteCount, "StartPost", false, DataSourceUpdateMode.OnPropertyChanged);
			txtEndPost.DataBindings.Add("Text", _voteCount, "EndPost", false, DataSourceUpdateMode.OnPropertyChanged);
			_voteCount.ChangeDay(_day);
			_voteCount.Refresh();
            EnableButtons(false);
            _voteCount.CheckThread(() => { EnableButtons(true); });
        }

		private void UnbindFromGame()
		{
			txtLastPost.DataBindings.Clear();
			udStartPost.DataBindings.Clear();
			txtEndPost.DataBindings.Clear();
			dtEndTime.DataBindings.Clear();
			dtStartTime.DataBindings.Clear();
			if (_voteCount != null)
			{
				_voteCount.PropertyChanged -= _voteCount_PropertyChanged;
				_voteCount = null;
			}
		}



		private void btnGetPosts_Click(object sender, EventArgs e)
		{
			if (_voteCount != null)
			{
                EnableButtons(false);
                _voteCount.CheckThread(() => { EnableButtons(true); });
			}
		}

		private void btnRoster_Click(object sender, EventArgs e)
		{
			statusText.Text = String.Empty;
			AutoComplete autoComplete = new AutoComplete(_forum, _synchronousInvoker, _db);
			PlayerList frmPlayers = new PlayerList(_voteCount, autoComplete);
			DialogResult dr = frmPlayers.ShowDialog();
			if (dr == System.Windows.Forms.DialogResult.OK)
			{
			}
			_voteCount.CommitRosterChanges();
			_voteCount.Refresh();

		}
		private void btnEditDay_Click(object sender, EventArgs e)
		{
			statusText.Text = String.Empty;
			DayEditor frm = new DayEditor(_voteCount);
			DialogResult result = frm.ShowDialog();
			if (result == System.Windows.Forms.DialogResult.OK)
			{
				Int32 day;
				Int32 startPost;
				DateTime endTime;
				frm.GetDayBoundaries(out day, out startPost, out endTime);
				_day = day;
				_voteCount.SetDayBoundaries(day, startPost, endTime);
				_voteCount.ChangeDay(day);
				_voteCount.Refresh();
			}
		}


		private void btnCopyIt_Click(object sender, EventArgs e)
		{
			String count = _voteCount.GetPostableVoteCount();
			Clipboard.SetDataObject(count, false);
			statusText.Text = String.Format("Copied vote count to clipboard at {0}.", DateTime.Now.ToShortTimeString());
		}

		private void btnPostIt_Click(object sender, EventArgs e)
		{
			String count = _voteCount.GetPostableVoteCount();
			Int32 threadId = _voteCount.ThreadId;
			Int32 last = _voteCount.LastPost;
			Int32 first = _voteCount.StartPost;
			String title = String.Empty + "┌∩┐(◣_◢)┌∩┐";
			if (_voteCount.TimeUntilNight.Ticks < 0)
			{
				title = "EOD ";
			}
            title += String.Format("Vote Count ┌∩┐(◣_◢)┌∩┐");
			var ok = _forum.MakePost(threadId, title, count + "┌∩┐(◣_◢)┌∩┐", 4, false);
			if (ok.Item1)
			{
				statusText.Text = String.Format("Posted a vote count at {0}.", DateTime.Now.ToShortTimeString());
			}
			else
			{
				statusText.Text = "Failed to post a vote count.";
			}
		}

		public string URL
		{
			get
			{
				return _url;
			}
		}
		public Int32 Day
		{
			get
			{
				return _day;
			}
		}
		public Boolean Turbo
		{
			get
			{
				return _turbo;
			}
		}
		private void btnMod_Click(object sender, EventArgs e)
		{
			Boolean autoPost = _moderator.AutoPostCounts;
			Boolean autoLock = _moderator.LockThread;
			Moderate frm = new Moderate(autoPost, autoLock);
			DialogResult dr = frm.ShowDialog();
			if (dr == System.Windows.Forms.DialogResult.OK)
			{
				frm.GetResults(out autoPost, out autoLock);
				_moderator.AutoPostCounts = autoPost;
				_moderator.LockThread = autoLock;
			}
		}

        private void chkLockedVotes_CheckedChanged(object sender, EventArgs e)
        {
            if (_voteCount != null)
            {
                _voteCount.LockedVotes = chkLockedVotes.Checked;
            }
        }
        void EnableButtons(Boolean enabled)
        {
            chkLockedVotes.Enabled = enabled;
            btnCopyIt.Enabled = enabled;
            btnEditDay.Enabled = enabled;
            btnFixVote.Enabled = enabled;
            btnGetPosts.Enabled = enabled;
            btnMod.Enabled = enabled;
            btnPostIt.Enabled = enabled;
            btnRoster.Enabled = enabled;
        }
        private void btnFixVote_Click(object sender, EventArgs e)
        {
			if (grdVotes.SelectedRows.Count < 1)
			{
				return;
			}
			var item = grdVotes.SelectedRows[0];
			if (item != null)
			{
				var player = (String)item.Cells[(Int32)CounterColumn.Player].Value;
                FixVote dlg = new FixVote(_voteCount, player);
                DialogResult dr = dlg.ShowDialog();
                
            }
        }

        private void chkUseMajority_CheckedChanged(object sender, EventArgs e)
        {
            if (chkUseMajority.Checked)
            {
                chkLockedVotes.Enabled = true;
                _voteCount.CheckMajority = true;
                _voteCount.LockedVotes = chkLockedVotes.Checked;
            }
            else
            {
                _voteCount.CheckMajority = false;
                chkLockedVotes.Enabled = false;
                chkLockedVotes.Checked = false;
            }
            ShowMajorityStatus();
        }
	}

}