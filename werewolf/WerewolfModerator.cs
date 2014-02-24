using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using POG.Utils;
using POG.Forum;

namespace POG.Werewolf
{
    class twoplustwo
    {
        delegate void HandlerComplete();
        public void GetPage(string url)
        {
        }
    }
    // Output Events:
    // polling for PMs?
    // StartGameReceived <from, players>
    // SubReceived <from, replaces>
    // KillReceived <from, target>
    // PeekReceived <from, target>
    // VoteCountRequestReceived <from>
    // CancelGameReceived <from>
    // InvalidPMReceived <from, id, contents>

    class PrivateMessagesSM : StateMachine
    {
        PrivateMessagesSM(StateMachineHost host)
            : base("PMs", host)
        {
            
        }
        protected override void OnInitialize()
        {
            SetInitialState(StateIdle);
        }
        public void PollPMs(bool poll)
        {
        }
        State StateTop(Event evt)
        {
            return null;
        }
        State StateIdle(Event evt)
        {
            switch (evt.EventName)
            {
                case "StartPolling":
                    {
                    }
                    break;
            }
            return StateTop;
        }
        State StatePolling(Event evt)
        {
            switch (evt.EventName)
            {
                case "StopPolling":
                    {
                    }
                    break;
            }
            return StateTop;
        }
    }

    // game thread URL
    // live player list
    // next night time / next day time
    // live villa / wolf / seer count
    // sub actions
    // game canceled
    // night actions

    // ThreadVotePosted <voter, votes for, time, #>
    // ThreadLocked
    // ThreadUnlocked
    // ThreadCreated <url>
    // ThreadModPosted <PostType>
    // PostCount
    // PostLynch
    // PostDay
    // PostSub
    // PostCancelGame
    // PostOP
    // PM seer <peek, role>
    // PM role <who, role, info>
    // PM subbedOut <who>
    
    class ModeratorSM : StateMachine
    {
        ElectionInfo _voteCount;
        VBulletinForum _forum;
        Boolean _postingCounts = false;

        public ModeratorSM(Moderator outer, ElectionInfo game, VBulletinForum forum, StateMachineHost host)
            : base("Moderator", host)
        {
            _voteCount = game;
            _forum = forum;
            SetInitialState(StateTop);
        }

        State StateTop(Event evt)
        {
            switch (evt.EventName)
            {
                case "AutoPostOn":
                    {
                        ChangeState(StatePostingCounts);
                    }
                    return null;
            }
            
            return null;
        }
        State StateIdle(Event evt)
        {
            switch (evt.EventName)
            {
                case "StartGameReceived":
                    {
                    }
                    break;
            }
            return StateTop;
        }
        State StateNight(Event evt)
        {
            switch (evt.EventName)
            {
                case "EventEnter":
                    {
                        Event evtPoll = new Event("PollThread");
                        StartOneShotTimer(60000, evtPoll);
                        SetVoteCountTimer();
                    }
                    break;

                case "PollThread":
                    {
                        if (_voteCount.TimeUntilNight.Ticks > 0)
                        {
                            ChangeState(StatePostingCounts);
                        }
                        else
                        {
                            StartOneShotTimer(60000, evt);
                        }
                    }
                    return null;
            }
            return StatePostingCounts;
        }
        State StatePostingCounts(Event evt)
        {
            switch (evt.EventName)
            {
                case "EventEnter":
                    {
                        _postingCounts = true;
                        if (!SetVoteCountTimer())
                        {
                            ChangeState(StateNight);
                        }
                    }
                    break;

                case "EventExit":
                    {
                        _postingCounts = false;
                    }
                    break;

                case "AutoPostOn":
                    {
                    }
                    return null;

                case "AutoPostOff":
                    {
                        ChangeState(StateIdle);
                    }
                    return null;

                case "PostCount":
                    {
                        _voteCount.CheckThread(() =>
                        {
                            Boolean night = (_voteCount.TimeUntilNight.Ticks < 0);
                            String count = _voteCount.GetPostableVoteCount();
                            if (count != String.Empty)
                            {
                                Int32 tid = _voteCount.ThreadId;
                                Int32 first = _voteCount.StartPost;
                                Int32 last = _voteCount.LastPost;
                                String title = String.Empty;
                                if (night)
                                {
                                    title = "EOD ";
                                }
                                title += String.Format("Vote Count {0} to {1} ", first, last);
                                _forum.MakePost(tid, title, count, 4, LockThread && night);
                            }
                            if (night)
                            {
                                ChangeState(StateNight);
                            }
                            else
                            {
                                SetVoteCountTimer();
                            }
                        });
                    }
                    return null;
            }
            return StateTop;
        }

        Boolean SetVoteCountTimer()
        {
            DateTime eod = _voteCount.EndTime;
            eod.AddMinutes(1);
            DateTime now = DateTime.Now;
            DateTime alarm = now;
            TimeSpan daylight = eod - now;
            if (daylight.TotalMinutes < -1)
            {
                return false;
            }
            if (daylight.TotalHours > 24)
            {
                int hour = (int)daylight.TotalHours;
                hour = 4 * (hour / 4);
                alarm = eod - new TimeSpan(hour, 0, 0);
            }
            else if (daylight.TotalHours > 8)
            {
                int hour = (int)daylight.TotalHours;
                hour = 2 * (hour / 2);
                alarm = eod - new TimeSpan(hour, 0, 0);
            }
            else if (daylight.TotalHours > 1)
            {
                // vote counts every hour.
                int hour = (int)daylight.TotalHours;
                alarm = eod - new TimeSpan(hour, 0, 0);
            }
            else if (daylight.Hours == 1)
            {
                alarm = eod - new TimeSpan(1, 0, 0);
            }
            else if (daylight.Minutes >= 30)
            {
                alarm = eod - new TimeSpan(0, 30, 0);
            }
            else if (daylight.Minutes >= 15)
            {
                alarm = eod - new TimeSpan(0, 15, 0);
            }
            else if (daylight.Minutes >= 10)
            {
                alarm = eod - new TimeSpan(0, 10, 0);
            }
            else if (daylight.Minutes >= 5)
            {
                alarm = eod - new TimeSpan(0, 5, 0);
            }
            else if (daylight.Minutes >= 2)
            {
                alarm = eod - new TimeSpan(0, 2, 0);
            }
            else if (daylight.Minutes == 1)
            {
                alarm = eod - new TimeSpan(0, 1, 0);
            }
            else
            {
                alarm = now + new TimeSpan(0, 0, 30);
            }
            TimeSpan duration = alarm - now;
            
            StartOneShotTimer((int)duration.TotalMilliseconds, new Event("PostCount"));
            return true;
        }

        internal bool AutoPostCounts
        {
            get
            {
                return _postingCounts;
            }
            set
            {
                if (value)
                {
                    Event evt = new Event("AutoPostOn");
                    PostEvent(evt);
                }
                else
                {
                    Event evt = new Event("AutoPostOff");
                    PostEvent(evt);
                }
            }
            
        }

        internal bool LockThread
        {
            get;
            set;
        }
    }
    public class Moderator
    {
        #region members
        ModeratorSM _inner;
        #endregion
        #region constructors

        public Moderator(Action<Action> synchronousInvoker, ElectionInfo voteCount, VBulletinForum forum)
        {
            _inner = new ModeratorSM(this, voteCount, forum, new StateMachineHost("ForumHost"));
        }
        #endregion
        public Boolean AutoPostCounts
        {
            get
            {
                return _inner.AutoPostCounts;
            }
            set
            {
                _inner.AutoPostCounts = value;
            }
        }
        public Boolean LockThread
        {
            get
            {
                return _inner.LockThread;
            }
            set
            {
                _inner.LockThread = value;
            }
        }
    }
}
