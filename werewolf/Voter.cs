using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using POG.Forum;

namespace POG.Werewolf
{
	public class Voter 
	{
		VoteCount _game;
		Action<Action> _synchronousInvoker;
		VoterInfo _info;
		private Int32 _votes;

		public Voter(VoterInfo vi, VoteCount game)
		{
			_info = vi;
			_game =game;
			_synchronousInvoker = _game.SynchronousInvoker;
		}
		[Browsable(true)]
		public String Name
		{
			get
			{
				return _info.Name;
			}
		}
		[Browsable(true)]
		public virtual String Bolded
		{
			get
			{
				return _info.Bolded;
			}
		}

		[Browsable(true)]
		public virtual String Votee
		{
			get
			{
				String content = Bolded;
				String votee = _game.ParseBoldedToVote(content);
				if (votee == "")
				{
					return "not voting";
				}
				return votee;
			}
			set
			{
				String content = Bolded;
				if (content != "")
				{
					_game.AddVoteAlias(content, value);
				}
			}
		}

		[Browsable(true)]
		public virtual Int32 PostNumber
		{
			get
			{
				return _info.PostNumber;
			}
		}
		internal Int32 PostId
		{
			get
			{
				return _info.PostId;
			}
		}
		internal Int32 BoldPosition
		{
			get
			{
				return _info.BoldPosition;
			}
		}
		[Browsable(true)]
		public virtual DateTimeOffset PostTime
		{
			get
			{
				return _info.PostTime.ToLocalTime();
			}
		}
		[Browsable(true)]
		public virtual String PostLink
		{
			get
			{
				String rc = "";
				if (_info.PostId > 0)
				{
					rc = String.Format("{0}showpost.php?p={1}&postcount={2}",
							_game.ForumURL, _info.PostId, _info.PostNumber);
				}
				return rc;
			}
		}
		[Browsable(true)]
		public virtual Int32 PostCount
		{
			get
			{
				return _info.PostCount;
			}
		}
		[Browsable(true)]
		public virtual Int32 VoteCount
		{
			get
			{
				return _votes;
			}
			internal set
			{
				if (value != _votes)
				{
					_votes = value;
				}
			}
		}

	}
}
