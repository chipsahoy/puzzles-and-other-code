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
        VoteCount _game;
        VBulletin_3_8_7 _forum;
        Moderator _outer;
        Int32 _countNumber = 1;

        public ModeratorSM(Moderator outer, VoteCount game, VBulletin_3_8_7 forum, StateMachineHost host)
            : base("Moderator", host)
        {
            _outer = outer;
            _game = game;
            _forum = forum;
            SetInitialState(StateTop);
        }
        public void StartGame(string sender, params string[] players)
        {
        }
        public void Peek(string sender, string target)
        {
        }
        public void Kill(string sender, string target)
        {
        }
        public void Sub(string sender, string target)
        {
        }
        public void CancelGame(string sender)
        {
        }

        State StateTop(Event evt)
        {
            switch (evt.EventName)
            {
                case "PostCount":
                    {
                        _game.MakePost("Vote Count #" + _countNumber.ToString());
                        _countNumber++;
                        SetVoteCountTimer();
                    }
                    break;
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
        State StatePlaying(Event evt)
        {
            return StateTop;
        }
        State StateRanding(Event evt)
        {
            return StatePlaying;
        }
        State StateStartThread(Event evt)
        {
            return StatePlaying;
        }
        State StatePollingThread(Event evt)
        {
            return StatePlaying;
        }
        State StateNight(Event evt)
        {
            return StatePollingThread;
        }
        State LynchReveal(Event evt)
        {
            return StateNight;
        }
        State NightReveal(Event evt)
        {
            return StateNight;
        }
        State StateDay(Event evt)
        {
            return StatePollingThread;
        }
        internal void StartAutoPost()
        {
            SetVoteCountTimer();
        }

        Boolean SetVoteCountTimer()
        {
            DateTime eod = _game.EndTime;
            DateTime now = DateTime.Now;
            DateTime alarm = now;
            TimeSpan daylight = eod - now;
            if (daylight.Milliseconds < 0)
            {
                return false;
            }
            if (daylight.Hours > 1)
            {
                // vote counts every 2 hours.
                int hour = (daylight.Hours / 2) * 2;
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
            else if (daylight.Minutes == 0)
            {
                alarm = eod - new TimeSpan(0, 15, 0);
            }
            TimeSpan duration = alarm - now;
            
            StartOneShotTimer((int)duration.TotalMilliseconds, new Event("PostCount"));
            return true;
        }
    }
    public class Moderator
    {
        #region members
        ModeratorSM _inner;
        Action<Action> _synchronousInvoker;
        #endregion
        #region constructors

        public Moderator(Action<Action> synchronousInvoker, VoteCount voteCount, VBulletin_3_8_7 forum)
        {
            _synchronousInvoker = synchronousInvoker;
            _inner = new ModeratorSM(this, voteCount, forum, new StateMachineHost("ForumHost"));
        }
        public void StartAutoPost()
        {
            _inner.StartAutoPost();
        }
        #endregion
    }
}
