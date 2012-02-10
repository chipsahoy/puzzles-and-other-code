using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using POG.Forum;

namespace POG.Werewolf
{
    public class Voter : INotifyPropertyChanged
    {
        Posts _posts = new Posts();
        VoteCount _game;
        Action<Action> _synchronousInvoker;
        String _name;
        Int32 _postCount;

        public Voter(string name, VoteCount game, Action<Action> synchronousInvoker)
        {
            Name = name;
            _game = game;
            _synchronousInvoker = synchronousInvoker;
        }
        public void HideVote()
        {
            Bold v = ActiveVote;
            if (v != null)
            {
                v.Ignore = true;
                CurrentVoteChanged();
            }
        }
        public void UnhideVote()
        {
            Bold hidden = null;
            foreach (Post p in _posts.Reverse())
            {
                foreach (Bold b in p.Bolded)
                {
                    if (b.Ignore)
                    {
                        hidden = b;
                    }
                    else
                    {
                        break;
                    }
                }
            }
            if (hidden != null)
            {
                hidden.Ignore = false;
                CurrentVoteChanged();
            }
        }
        [Browsable(true)]
        public String Name
        {
            get
            {
                return _name;
            }
            set
            {
                if (value != _name)
                {
                    _name = value;
                    OnPropertyChanged("Name");
                }
            }
        }
        [Browsable(true)]
        public virtual String Bolded
        {
            get
            {
                Bold v = ActiveVote;
                if(v != null)
                {
                        return v.Content;
                }
                return "";
            }
        }

        [Browsable(true)]
        public virtual String Votee
        {
            get
            {
                String content = "";
                Bold v = ActiveVote;
                if (v != null)
                {
                    content = v.Content;
                }
                String votee = _game.ParseBoldedToVote(content);
                if (votee == "")
                {
                    return "not voting";
                }
                return votee;
            }
            set
            {
                Bold v = ActiveVote;
                if (v != null)
                {
                    _game.AddVoteAlias(v.Content, value);
                    OnPropertyChanged("Votee");
                }
            }
        }

        [Browsable(true)]
        public virtual Int32 PostNumber
        {
            get
            {
                Post p = ActivePost;
                if (p != null)
                {
                    return p.PostNumber;
                }
                return 0;
            }
        }
        [Browsable(true)]
        public virtual DateTime PostTime
        {
            get
            {
                Post p = ActivePost;
                if (p != null)
                {
                    return p.Time;
                }
                return DateTime.MinValue;
            }
        }
        [Browsable(true)]
        public virtual String PostLink
        {
            get
            {
                Post p = ActivePost;
                if (p != null)
                {
                    return p.PostLink;
                }
                return "";
            }
        }
        [Browsable(true)]
        public virtual Int32 PostCount
        {
            get
            {
                Int32 count = _posts.Count;
                return count;
            }
            private set
            {
                if (_postCount != value)
                {
                    _postCount = value;
                    OnPropertyChanged("PostCount");
                }
            }
        }
        private Int32 _votes;
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
                    OnPropertyChanged("Votes");
                }
            }
        }
        internal void SetPosts(IEnumerable<Post> posts)
        {
            _posts.Clear();
            _posts.AddRange(posts);
            PostCount = _posts.Count;
            if (_posts.Count > 0)
            {
                Name = _posts.First().Poster; // fix case issues
            }
            CurrentVoteChanged();   
        }

        private void CurrentVoteChanged()
        {
            OnPropertyChanged("Bolded");
            OnPropertyChanged("Votee");
            OnPropertyChanged("PostLink");
            OnPropertyChanged("PostNumber");
            OnPropertyChanged("PostTime");
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                _synchronousInvoker.Invoke(
                    () => PropertyChanged(this, new PropertyChangedEventArgs(propertyName))
                );
            }
        }


        private Boolean FindActiveBold(out Post pActive, out Bold bActive)
        {
            for(Int32 i = _posts.Count - 1; i >= 0; i--)
            {

                Post p = _posts.GetByIndex(i);
                if (p.Bolded == null)
                {
                    _posts.Remove(p);
                    continue;
                }

                foreach (Bold bold in p.Bolded)
                {
                    if (!bold.Ignore)
                    {
                        pActive = p;
                        bActive = bold;
                        return true;
                    }
                }
            }
            pActive = null;
            bActive = null;
            return false;
        }
        private Bold ActiveVote
        {
            get
            {
                Post p;
                Bold b;
                FindActiveBold(out p, out b);
                return b;
            }
        }
        private Post ActivePost
        { 
            get
            {
                Post p;
                Bold b;
                FindActiveBold(out p, out b);
                return p;
            } 
        }
    }
    class SpecialVote : Voter
    {
        public SpecialVote(string name, VoteCount game, Action<Action> synchronousInvoker) :
            base(name, game, synchronousInvoker)
        {
        }
        [Browsable(true)]
        public override String Bolded
        {
            get
            {
                return "";
            }
        }

        [Browsable(true)]
        public override String Votee
        {
            get
            {
                return "";
            }
            set
            {
            }
        }

        [Browsable(true)]
        public override Int32 PostNumber
        {
            get
            {
                return 0;
            }
        }
        [Browsable(true)]
        public override DateTime PostTime
        {
            get
            {
                return DateTime.MinValue;
            }
        }
        [Browsable(true)]
        public override String PostLink
        {
            get
            {
                return "";
            }
        }
        [Browsable(true)]
        public override Int32 PostCount
        {
            get
            {
                return 0;
            }
        }
    }
}
