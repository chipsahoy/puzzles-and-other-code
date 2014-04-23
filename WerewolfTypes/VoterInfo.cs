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
		public String Name
		{
			get;
			private set;
		}
		public String Bolded
		{
			get;
			private set;
		}

		public Int32 PostNumber
		{
			get;
			private set;
		}
		public Int32 PostId
		{
			get;
			private set;
		}
		public Int32 BoldPosition
		{
			get;
			private set;
		}
		public DateTimeOffset PostTime
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
        public VoterInfo2(String name, Int32 posterId, Int32 postCount)
            : base(name, postCount, null)
        {
            PosterId = posterId;
            _votes = new List<Vote>();
        }
        public Int32 PosterId { get; private set; }
        public IEnumerable<Vote> Votes
        {
            get
            {
                return _votes;
            }
        }
        public void AddVote(Vote v)
        {
            _votes.Add(v);
        }
    }
}
