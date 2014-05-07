using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace POG.Werewolf
{
	public class VoterInfo
	{
		public VoterInfo(String name, Int32 postCount, Int32? postId)
		{
			PostNumber = 0;
			PostId = 0;
			BoldPosition = 0;
			Bolded = String.Empty;
			PostTime = DateTimeOffset.UtcNow;

			Name = name;
			PostCount = postCount;
			PostTime = DateTime.Now;
			if (postId != null)
			{
				PostId = postId.Value;
			}
		}
        public virtual String Name
		{
			get;
			private set;
		}
        public virtual String Bolded
		{
			get;
			private set;
		}

        public virtual Int32 PostNumber
		{
			get;
			private set;
		}
        public virtual Int32 PostId
		{
			get;
			private set;
		}
        public virtual Int32 BoldPosition
		{
			get;
			private set;
		}
        public virtual DateTimeOffset PostTime
		{
			get;
			private set;
		}
		public virtual Int32 PostCount
		{
			get;
			private set;
		}
		public void SetVote(String bolded, Int32 postNumber, DateTimeOffset time, Int32 id, Int32 position)
		{
			Bolded = bolded;
			PostNumber = postNumber;
			PostTime = time;
			PostId = id;
			BoldPosition = position;
		}
	}
    public class VoterInfo2 : VoterInfo
    {
        List<Vote> _votes = new List<Vote>();
        public VoterInfo2(Int32 roleId, String name, Int32 posterId, Int32 postCount)
            : base(name, postCount, null)
        {
            RoleId = roleId;
            PosterId = posterId;
            _votes = new List<Vote>();
        }
        public Int32 PosterId { get; private set; }
        public Int32 RoleId { get; private set; }
        public virtual IEnumerable<Vote> Votes
        {
            get
            {
                return _votes;
            }
        }
        public virtual IEnumerable<String> Names
        {
            get
            {
                return new List<String> { Name };
            }
        }
        public void AddVote(Vote v)
        {
            _votes.Add(v);
        }
    }
    public class Hydra : VoterInfo2
    {
        VoterInfo2 _player1;
        VoterInfo2 _player2;
        public Hydra(VoterInfo2 player1, VoterInfo2 player2) : 
            base(player1.RoleId, player1.Name + "/" + player2.Name, player1.PosterId, player1.PostCount + player2.PostCount)
        {
            _player1 = player1;
            _player2 = player2;
        }
        public override IEnumerable<string> Names
        {
            get
            {
                List<String> names = new List<string>() {Name, _player1.Name, _player2.Name, _player2.Name + "/" + _player1.Name};
                return names;
            }
        }
        public override int PostCount
        {
            get
            {
                return _player1.PostCount + _player2.PostCount;
            }
        }
        /*
        private VoterInfo2 ActiveVoter
        {
            get
            {
                if (_player1.PostId > _player2.PostId)
                {
                    return _player1;
                }
                return _player2;
            }
        }
        public override string Bolded
        {
            get
            {
                return ActiveVoter.Bolded;
            }
        }
        public override int BoldPosition
        {
            get
            {
                return ActiveVoter.BoldPosition;
            }
        }
        public override int PostId
        {
            get
            {
                return ActiveVoter.PostId;
            }
        }
        public override int PostNumber
        {
            get
            {
                return ActiveVoter.PostNumber;
            }
        }
        public override DateTimeOffset PostTime
        {
            get
            {
                return ActiveVoter.PostTime;
            }
        }*/
        public override IEnumerable<Vote> Votes
        {
            get
            {
                List<Vote> _votes = new List<Vote>();
                foreach (var v in _player1.Votes)
                {
                    _votes.Add(new Vote(Name, v.Bolded, v.PostNumber, v.PostId, v.BoldPosition, v.PostTime));
                }
                foreach (var v in _player2.Votes)
                {
                    _votes.Add(new Vote(Name, v.Bolded, v.PostNumber, v.PostId, v.BoldPosition, v.PostTime));
                }
                _votes.Sort((x, y) => { return x.PostId.CompareTo(y.PostId); });
                return _votes;
            }
        }
    }
}
