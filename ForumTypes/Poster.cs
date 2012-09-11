using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace POG.Forum
{
    [DataContract()]
    public class Poster
    {
        [DataMember]
        public String Name
        {
            get;
            private set;
        }
        [DataMember]
        public Int32 Id
        {
            get;
            private set;
        }
        public Poster(String name, Int32 id)
        {
            Name = name;
            Id = id;
        }
    }
}
