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
}
