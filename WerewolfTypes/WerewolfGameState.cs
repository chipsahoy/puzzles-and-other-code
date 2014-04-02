using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using POG.Utils;

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

namespace POG.Werewolf
{
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
    class Poster
    {
        public Int32 Id { get; private set; }
        public String Name { get; private set; }
        public Poster(Int32 id, String name)
        {
            Id = id;
            Name = name;
        }
    }
}
