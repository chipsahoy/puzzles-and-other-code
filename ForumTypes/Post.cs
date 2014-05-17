using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Web;

namespace POG.Forum
{
    public enum Language
    {
        undefined = 0,
        English,
        Estonian,
    }

    [DataContract()]
    public class Bold
    {
        public Bold(String content)
        {
            Content = content;
        }
        [DataMember]
        public String Content
        {
            get;
            private set;
        }
        [DataMember]
        public Boolean Ignore
        {
            get;
            set;
        }
    }
    [DataContract()]
    public class Post
    {
        String _content;
        List<Bold> _bolded;

        public Post(Int32 threadId, String poster, Int32 posterId, Int32 postNumber, DateTimeOffset ts,
            Int32 postId, String postTitle, String content, List<Bold> bolded, PostEdit edit) : 
            this(threadId, poster, posterId,
                postNumber, ts, "", postTitle, content, bolded, edit)
        {
            PostId = postId;
        }
        public Post(Int32 threadId, String poster, Int32 posterId, Int32 postNumber, DateTimeOffset ts, 
            String postLink, String postTitle, String content, List<Bold> bolded, PostEdit edit)
        {
            Poster = new Poster(poster, posterId);
            PostNumber = postNumber;
            Time = ts.ToUniversalTime();
            PostLink = postLink;
            Title = postTitle;
            Edit = edit;
            _content = content;
            ThreadId = threadId;
            _bolded = bolded;
            if (postLink.Length > 3)
            {
                // showthread.php?12931-Mafia-Convo-Thread&p=316289&viewfull=1#post316289
                string sPost = HttpUtility.ParseQueryString(postLink).Get("p");
                Int32 postId = -1;
                Int32.TryParse(sPost, out postId);
                PostId = postId;
            }
        }
        [DataMember]
        public Poster Poster
        {
            get;
            private set;
        }
        [DataMember]
        public Int32 PostId
        {
            get;
            private set;
        }
        [DataMember]
        public Int32 ThreadId
        {
            get;
            private set;
        }
        [DataMember]
        public String Content
        {
            get
            {
                return _content;
            }
        }
        [DataMember]
        public String Title
        {
            get;
            private set;
        }
        [DataMember]
        public PostEdit Edit
        {
            get;
            private set;
        }
        [DataMember]
        public Int32 PostNumber
        {
            get;
            private set;
        }
        public string PostLink
        {
            get;
            private set;
        }
        [DataMember]
        public DateTimeOffset Time
        {
            get;
            private set;
        }
        [DataMember]
        public IEnumerable<Bold> Bolded
        {
            get
            {
                return _bolded;
            }
        }

    }
    public class Posts : KeyedCollection<int, Post>
    {
        public Posts() :
            base()
        {
        }

        protected override int GetKeyForItem(Post item)
        {
            return item.PostNumber;
        }
        public virtual Post GetByIndex(Int32 index)
        {
            return Items[index];
        }

        public void AddRange(IEnumerable<Post> posts)
        {
            foreach (Post p in posts)
            {
                Add(p);
            }
        }
    }
}
