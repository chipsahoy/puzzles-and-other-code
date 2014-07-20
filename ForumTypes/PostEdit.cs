using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace POG.Forum
{
    [DataContract()]
    public class PostEdit
    {
        [DataMember]
        public String Who
        {
            get;
            private set;
        }
        [DataMember]
        public DateTimeOffset When
        {
            get;
            private set;
        }
        [DataMember]
        public String Reason
        {
            get;
            private set;
        }
        public PostEdit(String who, DateTimeOffset when, String reason)
        {
            Who = who;
            When = when.ToUniversalTime();
            Reason = reason;
        }
    }
}
