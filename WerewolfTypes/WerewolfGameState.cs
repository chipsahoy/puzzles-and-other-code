using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using POG.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

/*
game state: purpose... all the info needed to mod a game in progress OR do stats of finished game. Does not require full role info


Game:
Thread URL
Mods
teams
roles
days
 subs
 votes
 lynch result
nights
 subs
 peek
 kill result
ending

SetLineup(frompost)
SetDay(startpost, eodtime)
SetRole()
SubPlayer()
Eod(day, votes, lynch)
Day(day, kills)
EndGame(cause, winner)
 */

namespace POG.Werewolf.Serialize
{

    public enum WerewolfAction
    {
    }
    public struct WerewolfEvent
    {
        DateTimeOffset When;
        Int32 PostNumber;
        WerewolfAction Action;
        String Source;
        String Target;
    }
    public enum GamePhase
    {
        Empty,
        Design,
        Signup,
        ThreadPosted,
        RolesSent,
        Day,
        Night,
        Finished,
    }
    public class WerewolfGameInstance
    {
        public WerewolfGameInstance()
        {
            RandedRoles = new EncryptableProperty<RoleRand>();
            RandedRoles.value.test = "this is some data";
            Design = new EncryptableProperty<WerewolfGameDesign>();
        }
        List<String> _coMods = new List<string>();

        public GamePhase Phase { get; private set; }
        public String Url { get; private set; }
        public String Moderator { get; set; }
        public IEnumerable<String> CoMods { get { return _coMods; } }
       
        public EncryptableProperty<RoleRand> RandedRoles { get; private set; }
        public EncryptableProperty<WerewolfGameDesign> Design { get; private set; }
    }
    class PlayingRole
    {
        public WerewolfPlayer player { get; private set; }
        public Boolean alive { get; private set; }
    }
    public class WerewolfPlayer
    {
        //public Poster poster { get; private set; }
        public Int32 StartPost { get; private set; }
        public Int32 EndPost { get; private set; }
    }
    public class WerewolfGameDesign
    {
        List<WerewolfTeam> _teams = new List<WerewolfTeam>();
        public IEnumerable<WerewolfTeam> Teams { get { return _teams; } }
        List<RoleDesign> _designRoles = new List<RoleDesign>();
    }
    public class RoleRand
    {
        public string test { get; set; }
    }
    class PrivateActions
    {
    }
    public class WerewolfTeam
    {
        public Int32 Id { get; private set; }
        public String Name { get; private set; }
        public String Color { get; private set; }
        public String WinCondition { get; private set; }
        public String Flavor { get; private set; }
        public String PrivateFlavor { get; private set; }
        public Boolean PublicWinCondition { get; private set; }
    }
    class TimingDesign
    {
    }
    class PlayerOrGroup
    {
        public String Name { get; private set; }
        public virtual Boolean IsGroup { get { return false; } }
    }
    class Group : PlayerOrGroup
    {
        public override bool IsGroup
        {
            get
            {
                return true;
            }
        }
    }
    /*
     * groups: villagers, wolves, lost wolf, wolf known to all wolves but lost, vanillagers
     * player member of one group
     * groups can be members of groups
     * player can know groups members without being a member
     * villagers: seers, vanillagers
     * wolves: vanilla, lost wolf, traitor
     * lost wolf knows only self, traitor knows only self
     * regular wolves know all but lost
     * players: 0 (contains g1, g4)
     * villagers: 1 (contains g2, g3)
     * seers: 2 (contains roles 3)
     * vanillagers 3 (contains roles 4,5,6,7,8,9)
     * wolves: 4 (contains roles 1,2)
     * group: members, bool MembersKnowMembers?
     */
    class RoleDesign
    {
        public Int32 RoleId { get; private set; }
        public Int32 TeamId { get; private set; }
        public String Powers { get; private set; }
        public String Color { get; private set; }
    }
    public class Poster
    {
        public Int32 PosterId { get; private set; }
        public String PosterName { get; private set; }
        public Poster(Int32 id, String name)
        {
            PosterId = id;
            PosterName = name;
        }
    }

    
    
    public class ThreadRecord
    {
        public Int32 ThreadId { get; private set; }
        public String Url { get; private set; }
        public String Title { get; private set; }
        public ThreadRecord(Int32 id, String url, String title)
        {
            ThreadId = id;
            Url = url;
            Title = title;
        }
    }
    public enum TeamResult
    {
        NoResult,
        Lose,
        Tie,
        Win
    }
    public class TeamRecord
    {
        public Int32 TeamId { get; private set; }
        public String TeamName { get; private set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public TeamResult Result { get; private set; }
        public String WinCondition { get; private set; }
    }
    public class DayRecord
    {
        public Int32 Day { get; private set; }
        public Int32 WolfCount { get; private set; }
        public Int32 VillageSeerCount { get; private set; }
        public Int32 VillageVanillaCount { get; private set; }
        public Int32 PeekedVillagerCount { get; private set; }
        public Int32 PeekedWolfCount { get; private set; }
    }
    public class PlayerRecord
    {
        public Int32 PlayerId { get; private set; }
        public Int32 RoleId { get; private set; }
        public Int32 RoleRevision { get; private set; }
        public Int32 PosterId { get; private set; }
        public Int32 FirstDay { get; private set; }
    }

    public class RoleRecord
    {
        public Int32 RoleId { get; private set; }
        public Int32 TeamId { get; private set; }
        public String Powers { get; private set; }
    }
    public enum GameAction
    {
        AbortGame,
        WolvesConcede,
        Lynch,
        TeamNightKill,
        IndividualAction,
        Sub,
        Vote,
        Peek,
    }
    public class PeekRecord
    {
        public Int32 ActionId { get; private set; }
        public Int32 Day { get; private set; }
        public Int32 SeerId { get; private set; }
        public Int32 TargetId { get; private set; }
        public Int32 TargetTeam { get; private set; }
    }
    public class VoteRecord
    {
        public Int32 ActionId { get; private set; }
        public Int32 Day { get; private set; }
        public Int32 VoterId { get; private set; }
        public Int32 VoterTeam { get; private set; }
        public Int32 VoterPostCount { get; private set; }
        public Int32 VoteeId { get; private set; }
        public Int32 VoteeTeam { get; private set; }
        public Boolean Lynched { get; private set; }
        public Boolean Peeked { get; private set; }
        public Boolean Seer { get; private set; }
    }
    public class SubRecord
    {
        public Int32 ActionId { get; private set; }
        public Int32 PlayerOutId { get; private set; }
        public Int32 PlayerInId { get; private set; }
        public Int32 Day { get; private set; }
        public Int32 PostNumber { get; private set; }
    }
    public class TeamKillRecord
    {
        public Int32 KillId { get; private set; }
        public Int32 Day { get; private set; }
        public Int32 SourceTeamId { get; private set; }
        public Int32 TargetPlayerId { get; private set; }
    }
    public class PlayerKillRecord
    {
        public Int32 KillerId { get; private set; }
        public Int32 KillId { get; private set; }
    }
    public class LynchRecord
    {
        public Int32 ActionId { get; private set; }
        public Int32 Day { get; private set; }
        public Boolean LockedVotes { get; private set; }
        public Boolean Tie { get; private set; }
        public Boolean Majority { get; private set; }
        public Int32 PlayerId { get; private set; }
        public Int32 TeamId { get; private set; }
        public Int32 VoteCount { get; private set; }
        public Int32 PlayerCount { get; private set; }
    }
    public class GameRecord
    {
        public String Site { get; private set; }
        public ThreadRecord Thread { get; private set; }
        public Poster Moderator { get; private set; }
        public IEnumerable<Poster> Posters { get; private set; }
        public IEnumerable<TeamRecord> Teams { get; private set; }
        public IEnumerable<RoleRecord> Roles { get; private set; }
        public IEnumerable<SubRecord> Substitutions { get; private set; }
        public IEnumerable<DayRecord> CensusByDay { get; private set; }
        public IEnumerable<VoteRecord> Votes { get; private set; }
        public IEnumerable<LynchRecord> Lynches { get; private set; }
        public IEnumerable<TeamKillRecord> TeamKills { get; private set; }
        public IEnumerable<PlayerKillRecord> Killers { get; private set; }
        public IEnumerable<PeekRecord> Peeks { get; private set; }
    }
}
