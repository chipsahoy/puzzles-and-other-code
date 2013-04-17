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
using Newtonsoft.Json;
using System.IO;

namespace RickyRaccoon
{
    public partial class Raccoon : Form
    {

        List<String> roster = new List<string>();
        private TwoPlusTwoForum _forum;
        private Action<Action> _synchronousInvoker;
        RolePMSet rolepms = new RolePMSet("", new List<Team>());
        RolePMSet gamepms = new RolePMSet("", new List<Team>());


        public Raccoon()
        {
            InitializeComponent();
            string[] keys = new List<string>(rolepms.DefaultRoleSets.Keys).ToArray();
            boxRoleSetSelect.Items.AddRange(keys);
            boxRoleSetSelectLoad.Items.AddRange(keys);
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
                roster.Sort();
                txtPlayerCount.Text = roster.Count.ToString();
                txtPlayerList.Text = String.Join(Environment.NewLine, roster);
                int temp = 0;
                if (!Int32.TryParse(txtRoleCount.Text, out temp))
                {
                    btnDoIt.Enabled = false;
                    return;
                }
                if (Convert.ToInt16(txtRoleCount.Text) > 0 && roster.Count > 0 && roster.Count == Convert.ToInt16(txtRoleCount.Text))
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
            else if (Convert.ToInt16(txtRoleCount.Text) == 0)
            {
                MessageBox.Show("Please enter in your roles!");
                return;
            }
            else if (roster.Count != Convert.ToInt16(txtRoleCount.Text))
            {
                MessageBox.Show("There needs to be the same amount of players as roles!");
                return;
            }

            if (MessageBox.Show("Are you sure you want to continue? You can't go back after this!", "Continue?", MessageBoxButtons.YesNo) != DialogResult.Yes)
            {
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
            int n = Convert.ToInt16(txtRoleCount.Text);
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                String value = roster[k];
                roster[k] = roster[n];
                roster[n] = value;
            }
            int curplayer = 0;
            for (int i = 0; i < gamepms.Roles.Count; i++)
            {
                RolePM role = gamepms.Roles[i];
                for (int j = curplayer; j < curplayer + role.Count; j++)
                {
                    role.Players.Add(roster[j]);                    
                }
                curplayer += role.Count;
            }
            curplayer = 0;
            for (int i = 0; i < gamepms.Roles.Count; i++)
            {
                RolePM role = gamepms.Roles[i];
                string peek = "";
                string peektype = "";
                if (role.Role == "Seer" && boxSeerPeek.Text == "n0 Random Villager Peek")
                {
                    List<string> villagers = new List<string>();
                    for (int j = 0; j < gamepms.Roles.Count; j++)
                    {
                        if (gamepms.Roles[j].TeamRole.Name == "Villager")
                        {
                            for (int k = 0; k < gamepms.Roles[j].Players.Count; k++)
                            {
                                
                                villagers.Add(gamepms.Roles[j].Players[k]);
                            }
                        }
                    }
                    Random random = new Random();
                    int index = random.Next(villagers.Count);
                    peek = villagers[index];
                    peektype = "villager";
                }
                Console.WriteLine(peek);
                string pm = role.FullPM(txtGameURL.Text, gamepms, peek, peektype);
                for (int j = curplayer; j < curplayer + role.Count; j+=8)
                {
                    Console.WriteLine("PM: " + pm);
                    Console.WriteLine("currentplayer" + j + "-" + (Math.Min(8, curplayer + role.Count - j) + j));
                    _forum.SendPM(null, roster.GetRange(j, Math.Min(8, curplayer + role.Count - j) + j), txtGameName.Text + " Role PM", pm);
                    System.Threading.Thread.Sleep(30000);                    
                }
                curplayer += role.Count;
                gamepms.Roles[i].Players.Clear();
            }
            roster.Sort();
        }

        private void makeTurboOP()
        {
            txtOP.Text = String.Format(@"{0}
1st day is {1} minutes long, subsequent days are {2} minutes long. Nights are {3} minutes long, and NAs will be randed if not recieved by that time.
Seer got a {4}
There is {5} after d1
Must Lynch rules are {6}
Wolf Chat is {7}

You will receive your PMs shortly.

[b]IT IS NIGHT, DO NOT POST[/b]", txtRoleList.Text, txtDay1Length.Text, txtDayLength.Text, txtNightLength.Text, boxSeerPeek.Text, boxMajLynch.Text, boxMustLynch.Text, boxWolfChat.Text);
        }

        private void btnMakeOP_Click(object sender, EventArgs e)
        {
            if (boxTurbo.Checked)
            {
                makeTurboOP();
                return;
            }

            string pms = "";
            for (int i = 0; i < gamepms.Roles.Count; i++)
            {
                pms += "[quote]" + gamepms.Roles[i].EditedPM(txtGameURL.Text) + "[/quote]" + Environment.NewLine + Environment.NewLine;
            }
            string majlynchtext = "";
            string lynchdays = "";
            if (cboxSunday.Checked) lynchdays += "Sunday, ";
            if (cboxMonday.Checked) lynchdays += "Monday, ";
            if (cboxTuesday.Checked) lynchdays += "Tuesday, ";
            if (cboxWednesday.Checked) lynchdays += "Wednesday, ";
            if (cboxThursday.Checked) lynchdays += "Thursday, ";
            if (cboxFriday.Checked) lynchdays += "Friday, ";
            if (cboxSaturday.Checked) lynchdays += "Saturday, ";
            if (lynchdays.Length > 1)
                lynchdays = lynchdays.Substring(0, lynchdays.Length - 2);
            if (boxMajLynch.Text == "Majority Lynch") majlynchtext = "If at any time during the day any player has a majority the possible votes on him (e.g., with nine players alive, five or more is a majority), the day ends immediately and that player is lynched. Any player who believes a player has reached majority should post '[B]night[/B]', and all other posting should cease while the vote count is confirmed.";
            else majlynchtext = "";

            txtOP.Text = @"[b][u]POG Werewolf Rules[/u][/b]
[indent]For more details, please see [url=http://forumserver.twoplustwo.com/showpost.php?p=32574213&postcount=5]Werewolf Rules And Etiquette[/url][/indent][list][*][b][color=green]As long as the game is ongoing, you may talk about the game only inside the game thread[/color][/b][*][b][color=green]You may post in the game thread only when it is [u]DAY[/u][/color][/b][*][b][color=green]You may post only when you are alive[/color][/b][*][b][color=green]If you have a role which allows you to communicate outside the game thread, then you may do so only when allowed by your role, and only with individuals specified by your role[/color][/b][*][b][color=green]You may not edit or delete posts in the game thread for any reason at any time[/color][/b][*][b][color=green]You may not post screenshots of, or directly quote your role PM, seer peeks, or any other game-related communication that originated outside of the game thread[/color][/b][*][b][color=green]You are expected to participate[/color][/b][*][b][color=green]You are expected to be familiar with the rules, and you are expected to abide by them even if you think they are incorrect[/color][/b][*][b][color=green]You are expected to behave civilly[/color][/b][/list][indent]

Werewolf is a game about lying and catching people lying. It's an adversarial game and arguments between players are normal and expected. Passion and Intensity are fine but excessively personal attacks or insults are not. Even in a werewolf game you must respect the forum rules about civility. [u]Failure to do so may cause you to be infracted or temp banned, and repeated problems may get you perma-banned.[/u]

Werewolf is also a community and team-based game. While there are many styles and strategies and reasons for playing and you may choose your own, you are expected to be respectful of the time and energy others put in as players and as moderators. [u]You are expected to play to win. Intentionally sabotaging your team, or choosing strategies with the sole purpose of trolling other players in the game is not allowed.[/u][/indent]
[b][U]The Setup[/U][/b][indent]

PMs:
" + pms +
@"

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

Night will fall on the following days: " + lynchdays + @"

If this game has majority and any player reaches majority and is lynched before 2 pm, the moderator may, at his discretion, reopen the thread for a second game day that same day. He will do so only if it seems clear that this will not unduly disadvantage either side.[/indent]
[b][U]Wolf Chat[/U][/b]
[indent]Wolves may communicate with each other by any means they desire during [I]wolf chat[/I]. 

[b][color=red]Wolf chat is: " + boxWolfChat.Text + @"[/color][/b][/indent]
[b][U]The Roster[/U][/b]
[indent][color=red][b]" + String.Join(Environment.NewLine, roster) + @"[/b][/color][/indent]
[b][U]Questions and Confirmation[/U][/b]
[indent][color=red][b]Any questions regarding the rules should be submitted to the moderator by PM ONLY[/b][/color][/indent]
[U][b]Must Lynch[/b][/u]
[indent][color=red]" + boxMustLynch.Text + @"
[/color][/indent]

This post was made by automod(TM)

[b]IT IS NIGHT DO NOT POST[/b]";
        }

        private void txtGameTitle_TextChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < rolepms.Roles.Count; i++)
            {
                dataRoles.Rows[i].Cells["txtFullPM"].Value = rolepms.Roles[i].EditedPM(txtGameURL.Text);
            }
        }

        private void txtRoleSetName_TextChanged(object sender, EventArgs e)
        {
            rolepms.Name = txtRoleSetName.Text;
        }

        private void btnSaveRoleSet_Click(object sender, EventArgs e)
        {
            String roleset = JsonConvert.SerializeObject(rolepms, Formatting.Indented, new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
            // Show the dialog and get result.
            saveFileDialog.FileName = rolepms.Name;
            DialogResult result = saveFileDialog.ShowDialog();
            if (result == DialogResult.OK) // Test result.
            {
                using (Stream s = File.Open(saveFileDialog.FileName, FileMode.Create))
                using (StreamWriter sw = new StreamWriter(s))
                {
                    sw.Write(roleset);
                }
            }
            Console.WriteLine(result); // <-- For debugging use.
        }

        private void resetRoleList(RolePMSet rolepmset)
        {
            dataRoles.Rows.Clear();
            dataTeams.Rows.Clear();
            rolepms = rolepmset;
            for (int i = 0; i < rolepms.Teams.Count(); i++)
            {
                dataTeams.Rows.Add(rolepms.Teams[i].Name, rolepms.Teams[i].WinCon, rolepms.Teams[i].Hidden, rolepms.Teams[i].Share);
            }
            DataGridViewComboBoxColumn comboboxColumn = (DataGridViewComboBoxColumn)dataRoles.Columns[0];
            comboboxColumn.DataSource = rolepms.Teams;
            comboboxColumn.DisplayMember = "Name";
            comboboxColumn.ValueMember = "Self";
            for (int i = 0; i < rolepms.Roles.Count(); i++)
            {
                Team team = null;
                for (int j = 0; j < rolepms.Teams.Count(); j++)
                {
                    if (rolepms.Roles[i].TeamRole.Name == rolepms.Teams[j].Name)
                    {
                        team = rolepms.Teams[j];
                    }
                }
                dataRoles.Rows.Add(team, rolepms.Roles[i].Role, rolepms.Roles[i].SubRole, rolepms.Roles[i].ExtraFlavor, "", rolepms.Roles[i].Count);
            }
            txtRoleSetName.Text = rolepms.Name;
            dataRoles_CellValueChanged(null, null);
        }

        private void btnLoadRoleSet_Click(object sender, EventArgs e)
        {
            // Show the dialog and get result.
            DialogResult result = openFileDialog.ShowDialog();
            if (result == DialogResult.OK) // Test result.
            {
                string file = openFileDialog.FileName;
                try
                {
                    RolePMSet temp = JsonConvert.DeserializeObject<RolePMSet>(File.ReadAllText(file));
                    resetRoleList(temp);
                }
                catch (JsonSerializationException error)
                {
                    Console.WriteLine(error);
                    MessageBox.Show("This isn't valid JSON!");
                    return;
                }

            }
            Console.WriteLine(result); // <-- For debugging use.
        }

        private void boxRoleSetSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox roleset = (ComboBox)sender;
            gamepms = JsonConvert.DeserializeObject<RolePMSet>(rolepms.DefaultRoleSets[roleset.Text]);
            int playerCount = 0;
            txtRoleList.Text = "";
            for (int i = 0; i < gamepms.Roles.Count; i++)
            {
                string subrole = gamepms.Roles[i].SubRole;
                if (gamepms.Roles[i].SubRole != "") subrole = gamepms.Roles[i].SubRole + " ";
                string team = gamepms.Roles[i].TeamRole.Name;
                if (gamepms.Roles[i].TeamRole.Name != "") team = gamepms.Roles[i].TeamRole.Name + " ";
                txtRoleList.Text += gamepms.Roles[i].Count + "X " + team + subrole + gamepms.Roles[i].Role + Environment.NewLine;
                playerCount += gamepms.Roles[i].Count;
            }
            txtRoleCount.Text = Convert.ToString(playerCount);
            if (Convert.ToInt16(txtRoleCount.Text) > 0 && roster.Count > 0 && roster.Count == Convert.ToInt16(txtRoleCount.Text))
            {
                btnDoIt.Enabled = true;
            }
            else btnDoIt.Enabled = false;
        }

        private void boxRoleSetSelectLoad_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox roleset = (ComboBox)sender;
            resetRoleList(JsonConvert.DeserializeObject<RolePMSet>(rolepms.DefaultRoleSets[roleset.Text]));
        }

        private void btnLoadRoleSetGame_Click(object sender, EventArgs e)
        {
            DialogResult result = openFileDialog.ShowDialog();
            if (result == DialogResult.OK) // Test result.
            {
                string file = openFileDialog.FileName;
                List<String> rolestxt = new List<string>();
                try
                {
                    gamepms = JsonConvert.DeserializeObject<RolePMSet>(File.ReadAllText(file));
                }
                catch (JsonSerializationException error)
                {
                    Console.WriteLine(error);
                    MessageBox.Show("This isn't valid JSON!");
                    return;
                }
                int playerCount = 0;
                txtRoleList.Text = "";
                for (int i = 0; i < gamepms.Roles.Count; i++)
                {
                    string subrole = gamepms.Roles[i].SubRole;
                    if (gamepms.Roles[i].SubRole != "") subrole = gamepms.Roles[i].SubRole + " ";
                    string team = gamepms.Roles[i].TeamRole.Name;
                    if (gamepms.Roles[i].TeamRole.Name != "") team = gamepms.Roles[i].TeamRole.Name + " ";
                    txtRoleList.Text += gamepms.Roles[i].Count + "X " + team + subrole + gamepms.Roles[i].Role + Environment.NewLine;
                    playerCount += gamepms.Roles[i].Count;
                }
                for (int i = 0; i < gamepms.Teams.Count; i++)
                {
                    Console.WriteLine(gamepms.Teams[i].Name + gamepms.Teams[i].Share);
                }
                txtRoleCount.Text = Convert.ToString(playerCount);
                if (Convert.ToInt16(txtRoleCount.Text) > 0 && roster.Count > 0 && roster.Count == Convert.ToInt16(txtRoleCount.Text))
                {
                    btnDoIt.Enabled = true;
                }
                else btnDoIt.Enabled = false;
            }
        }

        private void dataTeams_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {

            List<Team> teams = rolepms.Teams;
            for (int i = 0; i < dataTeams.Rows.Count; i++)
            {

                if (!dataTeams.Columns.Contains("colTeamName")) continue;
                if (dataTeams.Rows[i].Cells["colTeamName"].Value == null || dataTeams.Rows[i].Cells["colWinCon"].Value == null)
                    continue;
                string teamname = dataTeams.Rows[i].Cells["colTeamName"].Value.ToString();
                string wincon = dataTeams.Rows[i].Cells["colWinCon"].Value.ToString();
                bool hidden = Convert.ToBoolean(dataTeams.Rows[i].Cells["colReveal"].Value);
                bool share = Convert.ToBoolean(dataTeams.Rows[i].Cells["colShare"].Value);
                if (i < teams.Count)
                {
                    teams[i].Name = teamname;
                    teams[i].WinCon = wincon;
                    teams[i].Hidden = hidden;
                    teams[i].Share = share;
                }
                else
                {
                    Team team = new Team(teamname, wincon, hidden, share);
                    teams.Add(team);
                }
            }
            DataGridViewComboBoxColumn comboboxColumn = (DataGridViewComboBoxColumn)dataRoles.Columns[0];
            comboboxColumn.DataSource = rolepms.Teams;
            comboboxColumn.DisplayMember = "Name";
            comboboxColumn.ValueMember = "Self";
        }

        private void dataRoles_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            //for (int i = rolepms.Roles.Count; i < dataRoles.Rows.Count - 1; i++)
            //{
            //   rolepms.Roles.RemoveAt(i);
            //}
            int playerCount = 0;
            for (int i = 0; i < dataRoles.Rows.Count; i++)
            {
                if (!dataRoles.Columns.Contains("boxTeam")) continue;
                if (dataRoles.Rows[i].Cells["boxTeam"].Value == null || dataRoles.Rows[i].Cells["boxRole"].Value == null || dataRoles.Rows[i].Cells["boxTeam"].Value.ToString() == "")
                    continue;
                Team team = (Team)dataRoles.Rows[i].Cells["boxTeam"].Value;
                string role = dataRoles.Rows[i].Cells["boxRole"].Value.ToString();
                string subrole = "";
                if (dataRoles.Rows[i].Cells["boxSubRole"].Value != null) subrole = dataRoles.Rows[i].Cells["boxSubRole"].Value.ToString();
                string extraflavor = "";
                if (dataRoles.Rows[i].Cells["txtExtraFlavor"].Value != null) extraflavor = dataRoles.Rows[i].Cells["txtExtraFlavor"].Value.ToString();
                int count = 0;
                if (dataRoles.Rows[i].Cells["txtCount"].Value != null) count = Convert.ToInt16(dataRoles.Rows[i].Cells["txtCount"].Value.ToString());
                RolePM rolepm = new RolePM(team, role, subrole, extraflavor, count);
                if (rolepms.Roles.Count > i)
                {
                    rolepms.Roles[i] = rolepm;
                }
                else
                {
                    rolepms.Roles.Add(rolepm);
                }
                playerCount += count;
                dataRoles.Rows[i].Cells["txtFullPM"].Value = rolepm.EditedPM(txtGameURL.Text);
            }
            txtPlayers.Text = Convert.ToString(playerCount);
        }

        private void dataRoles_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            int rowIndex = e.Row.Index;
            rolepms.Roles.RemoveAt(rowIndex);
        }

        private void dataTeams_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            int rowIndex = e.Row.Index;
            Team team = rolepms.Teams[rowIndex];
            for (int i = 0; i < rolepms.Roles.Count; i++)
            {
                if (rolepms.Roles[i].TeamRole.Equals(team))
                {
                    e.Cancel = true;
                    return;
                }
            }
            rolepms.Teams.RemoveAt(rowIndex);
        }

        private void boxTurbo_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox turbo = (CheckBox)sender;
            if (turbo.Checked)
            {
                txtDayLength.Enabled = true;
                txtDay1Length.Enabled = true;
                txtNightLength.Enabled = true;
                boxLongGame.Checked = false;
            }
            else
            {
                txtDayLength.Enabled = false;
                txtDay1Length.Enabled = false;
                txtNightLength.Enabled = false;
            }
        }

        private void boxLongGame_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox longgame = (CheckBox)sender;
            if (longgame.Checked)
            {
                boxEODTime.Enabled = true;
                boxSODTime.Enabled = true;
                cboxSunday.Enabled = true;
                cboxMonday.Enabled = true;
                cboxTuesday.Enabled = true;
                cboxWednesday.Enabled = true;
                cboxThursday.Enabled = true;
                cboxFriday.Enabled = true;
                cboxSaturday.Enabled = true;
                boxTurbo.Checked = false;
            }
            else
            {
                boxEODTime.Enabled = false;
                boxSODTime.Enabled = false;
                cboxSunday.Enabled = false;
                cboxMonday.Enabled = false;
                cboxTuesday.Enabled = false;
                cboxWednesday.Enabled = false;
                cboxThursday.Enabled = false;
                cboxFriday.Enabled = false;
                cboxSaturday.Enabled = false;
            }
        }
    }
}
