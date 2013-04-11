using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using POG.Forum;

namespace RickyRaccoon
{
    public partial class Raccoon : Form
    {

        List<String> roster = new List<string>();
        List<String> roles = new List<string>();
        private TwoPlusTwoForum _forum;
        private Action<Action> _synchronousInvoker;

        public Raccoon()
        {
            InitializeComponent();
        }

        private void btnPasteRoles_Click(object sender, EventArgs e)
        {
            roles.Clear();
            object o = Clipboard.GetData(DataFormats.Text);
            if (o != null)
            {
                String clip = o as String;
                String[] lines = clip.Split(new String[] { "\r\n", "\r" }, StringSplitOptions.RemoveEmptyEntries);
                foreach (String line in lines)
                {
                    String name = line.Trim();
                    if(Regex.IsMatch(name, @"^[\s]*$")) continue;
                    roles.Add(name);
                }
                txtRoleCount.Text = roles.Count.ToString();
                txtRoleList.Text = String.Join(Environment.NewLine, roles);
            }
            if (roles.Count > 0 && roster.Count > 0 && roster.Count == roles.Count)
            {
                btnDoIt.Enabled = true;
            }
            else btnDoIt.Enabled = false;
        }

        private void btnPasteList_Click(object sender, EventArgs e)
        {
            roster.Clear();
            object o = Clipboard.GetData(DataFormats.Text);
            if (o != null)
            {
                String clip = o as String;
                String[] lines = clip.Split(new String[] { "\r\n", "\r" }, StringSplitOptions.RemoveEmptyEntries);
                foreach (String line in lines)
                {
                    String name = line.Trim();
                    if (Regex.IsMatch(name, @"^[\s]*$")) continue;
                    roster.Add(name);
                }
                txtPlayerCount.Text = roster.Count.ToString();
                txtPlayerList.Text = String.Join(Environment.NewLine, roster);
                if (roles.Count > 0 && roster.Count > 0 && roster.Count == roles.Count)
                {
                    btnDoIt.Enabled = true;
                }
                else btnDoIt.Enabled = false;
            }
        }

        private void btnDoIt_Click(object sender, EventArgs e)
        {
            if (roster.Count == 0)
            {
                MessageBox.Show("Please enter in your playerlist!");
                return;
            }
            else if (roles.Count == 0)
            {
                MessageBox.Show("Please enter in your roles!");
                return;
            }
            else if (roster.Count != roles.Count)
            {
                MessageBox.Show("There needs to be the same amount of players as roles!");
                return;
            }
            
            _synchronousInvoker = a => Invoke(a);
            String username = txtUsername.Text;
            String password = txtPassword.Text;
            String host = "forumserver.twoplustwo.com";

            _forum = new TwoPlusTwoForum(_synchronousInvoker, host);
            _forum.LoginEvent += new EventHandler<LoginEventArgs>(_forum_LoginEvent);

            if ((username != String.Empty) && (password != String.Empty))
            {
                _forum.Login(username, password);
            }
            else
            {
                MessageBox.Show("Please enter your username and password!");
                return;
            }
            
        }

        private void _forum_LoginEvent(object sender, POG.Forum.LoginEventArgs e)
        {
            switch (e.LoginEventType)
            {
                case POG.Forum.LoginEventType.LoginFailure:
                    MessageBox.Show("There was an error logging in!");
                    return;

                case POG.Forum.LoginEventType.LoginSuccess:
                    MessageBox.Show("Login Success! This may take a while...");
                    break;
            }
            btnDoIt.Enabled = false;
            Random rng = new Random();
            int n = roles.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                String value = roles[k];
                roles[k] = roles[n];
                roles[n] = value;
            }

            txtRoleList.Text = "";
            List<String> villagers = new List<string>();
            List<String> wolves = new List<string>();
            List<String> seers = new List<string>();
            for (int i = 0; i < roles.Count; i++)
            {
                txtRoleList.Text += roster[i] + " - " + roles[i] + Environment.NewLine;

                //Add roles into their list for PMing purposes. Warning: Brutal hack where we assume the spelling/capatilization of the roles
                if (roles[i] == "villager")
                {
                    villagers.Add(roster[i]);
                }
                else if (roles[i] == "wolf")
                {
                    wolves.Add(roster[i]);
                }
                else if (roles[i] == "seer")
                {
                    seers.Add(roster[i]);
                }
            }
            for (int i = 0; i < villagers.Count; i += 8)
            {
                _forum.SendPM(null, villagers.GetRange(i, Math.Min(8, villagers.Count - i)), txtGameName.Text + " Role PM", @"*************************************************
You are a villager! You win by eliminating all the wolves.

The game thread is here: " + txtGameTitle.Text + @"

Good luck!
*************************************************", true);
                System.Threading.Thread.Sleep(60000);
            }
            for (int i = 0; i < wolves.Count; i += 8)
            {
                _forum.SendPM(null, wolves.GetRange(i, Math.Min(8, wolves.Count - i)), txtGameName.Text + " Role PM", @"*************************************************
You are a wolf! You win by achieving even numbers with the village

The wolf team is:
" + String.Join(Environment.NewLine, wolves) + @"

The game thread is here: " + txtGameTitle.Text + @"

Good luck!
*************************************************", true);
                System.Threading.Thread.Sleep(60000);
            }
            for (int i = 0; i < seers.Count; i += 8)
            {
                String peektext = "";
                if (boxSeerPeek.Text == "n0 True Peek") peektext = "You receive an n0 peek chosen from the playerlist!";
                else if (boxSeerPeek.Text == "n0 Villager Peek") peektext = "You receive a random n0 villager peek!";
                else if (boxSeerPeek.Text == "No n0 Peek") peektext = "You don't receive an n0 peek!";

                _forum.SendPM(null, seers.GetRange(i, Math.Min(8, seers.Count - i)), txtGameName.Text + " Role PM", @"*************************************************
You are the Seer! " + peektext + @"

The Playerlist is:
" + String.Join(Environment.NewLine, roster) + @"

The game thread is here: " + txtGameTitle.Text + @"

Good luck!
*************************************************", true);
                System.Threading.Thread.Sleep(60000);
            }
        }

        private void txtPlayerList_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtPassword_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnMakeOP_Click(object sender, EventArgs e)
        {
            String peektext = "";
            String majlynchtext = "";

            if (boxSeerPeek.Text == "n0 True Peek") peektext = "You receive an n0 peek chosen from the playerlist!";
            else if (boxSeerPeek.Text == "n0 Villager Peek") peektext = "You receive a random n0 villager peek!";
            else if (boxSeerPeek.Text == "No n0 Peek") peektext = "You don't receive an n0 peek!";

            if (boxMajLynch.Text == "Majority Lynch") majlynchtext = "If at any time during the day any player has a majority the possible votes on him (e.g., with nine players alive, five or more is a majority), the day ends immediately and that player is lynched. Any player who believes a player has reached majority should post '[B]night[/B]', and all other posting should cease while the vote count is confirmed.";
            else majlynchtext = "";

            txtOP.Text = @"[b][u]POG Werewolf Rules[/u][/b]
[indent]For more details, please see [url=http://forumserver.twoplustwo.com/showpost.php?p=32574213&postcount=5]Werewolf Rules And Etiquette[/url][/indent][list][*][b][color=green]As long as the game is ongoing, you may talk about the game only inside the game thread[/color][/b][*][b][color=green]You may post in the game thread only when it is [u]DAY[/u][/color][/b][*][b][color=green]You may post only when you are alive[/color][/b][*][b][color=green]If you have a role which allows you to communicate outside the game thread, then you may do so only when allowed by your role, and only with individuals specified by your role[/color][/b][*][b][color=green]You may not edit or delete posts in the game thread for any reason at any time[/color][/b][*][b][color=green]You may not post screenshots of, or directly quote your role PM, seer peeks, or any other game-related communication that originated outside of the game thread[/color][/b][*][b][color=green]You are expected to participate[/color][/b][*][b][color=green]You are expected to be familiar with the rules, and you are expected to abide by them even if you think they are incorrect[/color][/b][*][b][color=green]You are expected to behave civilly[/color][/b][/list][indent]

Werewolf is a game about lying and catching people lying. It's an adversarial game and arguments between players are normal and expected. Passion and Intensity are fine but excessively personal attacks or insults are not. Even in a werewolf game you must respect the forum rules about civility. [u]Failure to do so may cause you to be infracted or temp banned, and repeated problems may get you perma-banned.[/u]

Werewolf is also a community and team-based game. While there are many styles and strategies and reasons for playing and you may choose your own, you are expected to be respectful of the time and energy others put in as players and as moderators. [u]You are expected to play to win. Intentionally sabotaging your team, or choosing strategies with the sole purpose of trolling other players in the game is not allowed.[/u][/indent]
[b][U]The Setup[/U][/b][indent]

Villager PM:

[quote]" + @"*************************************************
You are a villager! You win by eliminating all the wolves.

The game thread is here: " + txtGameTitle.Text + @"

Good luck!
*************************************************" + @"
[/quote]

Wolf PM:
[quote]" + @"*************************************************
You are a wolf! You win by achieving even numbers with the village

The wolf team is:
WOLVES

The game thread is here: " + txtGameTitle.Text + @"

Good luck!
*************************************************" + @"
[/quote]

Seer PM:
[quote]" + @"*************************************************
You are the Seer! " + peektext + @"

The Playerlist is:
" + String.Join(Environment.NewLine, roster) + @"

The game thread is here: " + txtGameTitle.Text + @"

Good luck!
*************************************************" + @"
[/quote]

[U]Voting[/U]

Each day, each player is allowed (and encouraged) to vote to lynch a player. To do so, type the name of the player in [B]bold[/B]; That is: [b]zurvan[/b]

You may remove your vote by posting '[B]unvote[/B]'. You may change your vote simply by voting the new player; you do not need to unvote.

At the end of the scheduled game day, the player with the most votes is lynched, and his role revealed in the thread. If two or more players are tied for the most votes, one of the tied players is chosen randomly by the moderator.

" + majlynchtext + @"
[/indent]
[b][U]Day and Night Times[/U][/b]
[indent]All times give are server time, which is also Eastern Standard Time.

I will open the thread every morning by posting '[B]It is day.[/B]' You may not post in the thread until I have made the morning post. Day will usually start at [color=red]" + boxSODTime.Text + @"[/color]

Day lasts until [b][color=red]" + boxEODTime.Text + @"[/color][/b]. Posting at [b][color=red]" + boxEODTime.Text + @"[/color][/b] is acceptable, and votes with that time stamp will be counted. Posting after that, even just a minute after, is not acceptable, even if I have not yet declared it night by posting '[B]It is night.[/B]' Do not post in the thread at any time, for any reason, between the end of the day and the opening of the thread the next morning.

Weekends are different: There is not 'night' Saturday night -- the thread remains open from Saturday morning until Sunday evening. There is no scheduled Saturday night lynch.

If any player reaches majority and is lynched before 2 pm, the moderator may, at his discretion, reopen the thread for a second game day that same day. He will do so only if it seems clear that this will not unduly disadvantage either side.[/indent]
[b][U]Wolf Chat[/U][/b]
[indent]Wolves may communicate with each other by any means they desire during [I]wolf chat[/I]. 

[b][color=red]Wolf chat is: " + boxWolfChat.Text + @"[/color][/b][/indent]
[b][U]The Roster[/U][/b]
[indent][color=red][b]" + String.Join(Environment.NewLine, roster)  + @"[/b][/color][/indent]
[b][U]Questions and Confirmation[/U][/b]
[indent][color=red][b]Any questions regarding the rules should be submitted to the moderator by PM ONLY[/b][/color][/indent]
[U][b]Must Lynch[/b][/u]
[indent][color=red]" + boxMustLynch.Text + @"
[/color][/indent]

This post was made by automod(TM)

[b]IT IS NIGHT DO NOT POST[/b]";
        }

        private void removeRoles_Click(object sender, EventArgs e)
        {
            Button clickedButton = (Button)sender;
            int rowIndex = tblRoles.GetRow(clickedButton);
            System.Console.WriteLine(tblRoles.RowCount);
            System.Console.WriteLine(tblRoles.RowStyles.Count);
            System.Console.WriteLine(rowIndex);
            tblRoles.RowStyles.RemoveAt(rowIndex - 1);
            for (int columnIndex = 0; columnIndex < tblRoles.ColumnCount; columnIndex++)
            {
                var control = tblRoles.GetControlFromPosition(columnIndex, rowIndex);
                tblRoles.Controls.Remove(control);
            }

            for (int i = rowIndex + 1; i <= tblRoles.RowCount; i++)
            {
                for (int columnIndex = 0; columnIndex < tblRoles.ColumnCount - 1; columnIndex++)
                {
                    var control = tblRoles.GetControlFromPosition(columnIndex, i);
                    tblRoles.SetRow(control, i - 1);
                    tblRoles.Name = tblRoles.Name.Substring(tblRoles.Name.Length - 1) + (i - 1);
                }
            }
            tblRoles.RowCount--;
        }

        private void btnAddRole_Click(object sender, EventArgs e)
        {
            tblRoles.RowCount += 1;
            tblRoles.RowStyles.Add(new RowStyle(System.Windows.Forms.SizeType.AutoSize));
            Button removeRoles = new Button();
            removeRoles.Text = "Remove Role";
            removeRoles.Name = "btnRemoveRole";
            removeRoles.Height = 23;
            removeRoles.Width = 85;
            removeRoles.Click += new EventHandler(this.removeRoles_Click);
            tblRoles.Controls.Add(removeRoles, 0, tblRoles.RowCount);

            ComboBox team = new ComboBox();
            string[] teamlist = new string[]{
        	    "Wolves",
        	    "Village",
        	    "Neutral"
        	};
            team.Items.AddRange(teamlist);
            team.Name = "boxTeam";
            tblRoles.Controls.Add(team, 1, tblRoles.RowCount);

            ComboBox role = new ComboBox();
            string[] rolelist = new string[]{
        	    "Angel",
                "Roleblocker",
                "Seer",
                "Vanilla",
                "Vigilante"
        	};
            role.Items.AddRange(rolelist);
            role.Name = "boxRole";
            tblRoles.Controls.Add(role, 2, tblRoles.RowCount);

            ComboBox subrole = new ComboBox();
            string[] subrolelist = new string[]{
        	    "Even",
                "Odd",
                "Full",
                "1x",
                "n0",
                "n1",
                "n2",
                "n3"
        	};
            subrole.Items.AddRange(subrolelist);
            subrole.Width = 50;
            subrole.Name = "boxSubRole";
            tblRoles.Controls.Add(subrole, 3, tblRoles.RowCount);

            TextBox extraflavor = new TextBox();
            extraflavor.Multiline = true;
            extraflavor.Width = 206;
            extraflavor.Height = 81;
            extraflavor.ScrollBars = ScrollBars.Both;
            extraflavor.Name = "txtExtraFlavor";
            tblRoles.Controls.Add(extraflavor, 4, tblRoles.RowCount);

            TextBox wincon = new TextBox();
            wincon.Multiline = true;
            wincon.Width = 206;
            wincon.Height = 81;
            wincon.ScrollBars = ScrollBars.Both;
            wincon.Name = "txtWinCon";
            tblRoles.Controls.Add(wincon, 5, tblRoles.RowCount);

            TextBox fullpm = new TextBox();
            fullpm.Multiline = true;
            fullpm.Enabled = false;
            fullpm.ReadOnly = true;
            fullpm.Width = 206;
            fullpm.Height = 81;
            fullpm.ScrollBars = ScrollBars.Both;
            fullpm.Name = "txtFullPM";
            tblRoles.Controls.Add(fullpm, 6, tblRoles.RowCount);

            TextBox count = new TextBox();
            fullpm.Name = "txtCount";
            tblRoles.Controls.Add(count, 7, tblRoles.RowCount);
        }
    }
}
