using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace POG.Forum
{
    public class Bold
    {
        public Bold(String content)
        {
            Content = content;
        }
        public String Content
        {
            get;
            private set;
        }
        public Boolean Ignore
        {
            get;
            set;
        }
    }
    public class Post
    {
        String _content;
        List<Bold> _bolded;

        public Post(Int32 threadId, String poster, Int32 postNumber, DateTimeOffset ts, String postLink, 
            String postTitle, String content, String postEdit, List<Bold> bolded)
        {
            Poster = poster;
            PostNumber = postNumber;
            Time = ts;
            PostLink = postLink;
            Title = postTitle;
            Edit = postEdit;
            _content = content;
            ThreadId = threadId;
            _bolded = bolded;
            int ixPostStart = postLink.LastIndexOf("?p=") + 3;
            string sPost = postLink.Substring(ixPostStart);
            int ixPostLast = sPost.IndexOf('&');
            sPost = sPost.Substring(0, ixPostLast);
            Int32 postId = -1;
            Int32.TryParse(sPost, out postId);
            PostId = postId;
        }
        public string Poster
        {
            get;
            private set;
        }
        public Int32 PostId
        {
            get;
            private set;
        }
        public Int32 ThreadId
        {
            get;
            private set;
        }
        public String Content
        {
            get
            {
                return _content;
            }
        }
        public String Title
        {
            get;
            private set;
        }
        public String Edit
        {
            get;
            private set;
        }

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
        public DateTimeOffset Time
        {
            get;
            private set;
        }
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
