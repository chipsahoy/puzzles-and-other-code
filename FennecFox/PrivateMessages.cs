using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FennecFox
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
        public override void Initialize()
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
        ModeratorSM(StateMachineHost host)
            : base("Moderator", host)
        {
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
    }
}
