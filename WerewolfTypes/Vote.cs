using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace POG.Werewolf
{
    public class Vote
    {
        public Vote(string voter, string bolded, int postNumber, int postId, int boldPosition, DateTimeOffset postTime)
        {
            Voter = voter;
            Bolded = bolded;
            PostNumber = postNumber;
            PostId = postId;
            BoldPosition = boldPosition;
            PostTime = postTime;
        }
        public String Voter { get; private set; }
        public String Votee
        {
            get;
            set;
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

    }
}
