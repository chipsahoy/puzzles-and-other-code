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
        VoteCount _game;
        Action<Action> _synchronousInvoker;
        String _name;
        Int32 _postCount;
        Int32 _postId;
        Int32 _postNumber;
        Int32 _boldPosition;

        public Voter(string name, VoteCount game, Action<Action> synchronousInvoker)
        {
            Name = name;
            _game = game;
            _synchronousInvoker = synchronousInvoker;
        }
        public void HideVote()
        {
            _game.HideVote(this, _postNumber, _boldPosition);
        }
        public void UnhideVote()
        {
            _game.UnhideVote(this, _postNumber, _boldPosition);
        }
        [Browsable(true)]
        public String Name
        {
            get
            {
                return _name;
            }
            internal set
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
            get;
            private set;
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
                    OnPropertyChanged("Votee");
                }
            }
        }

        [Browsable(true)]
        public virtual Int32 PostNumber
        {
            get
            {
                return _postNumber;
            }
        }
        [Browsable(true)]
        public virtual DateTime PostTime
        {
            get;
            internal set;
        }
        [Browsable(true)]
        public virtual String PostLink
        {
            get
            {
                String rc = "";
                if (_postId > 0)
                {
                    rc = String.Format("{0}/showpost.php?p={1}&postcount={2}", 
                            TwoPlusTwoForum.BASE_URL, _postId, _postNumber);
                }
                return rc;
            }
        }
        [Browsable(true)]
        public virtual Int32 PostCount
        {
            get
            {
                return _postCount;
            }
            internal set
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
        internal void SetVote(String bolded, Int32 postNumber, DateTime time, Int32 id, Int32 position)
        {
            _postNumber = postNumber;
            _postId = id;
            _boldPosition = position;
            Bolded = bolded;
            PostTime = time;
            Console.WriteLine("My vote[{0}]: '{1}'", postNumber, bolded);
            CurrentVoteChanged();
        }
        internal void ClearVote()
        {
            _postNumber = 0;
            _postId = 0;
            _boldPosition = 0;
            Bolded = "";
            PostTime = new DateTime(2012, 1, 1);
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
    }
}
