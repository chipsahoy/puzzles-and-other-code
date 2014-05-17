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
using POG.Werewolf;
using POG.Database;

namespace RickyRaccoon
{
    public partial class Raccoon : Form
    {

        List<String> roster = new List<string>();
        private VBulletinForum _forum;
        private Action<Action> _synchronousInvoker;
        RolePMSet rolepms = new RolePMSet("", new List<Team>());
        RolePMSet gamepms = new RolePMSet("", new List<Team>());
        bool testrun = true;
        bool saved = true;
        bool remote = false;
        Processing processing;
        Timer pmtimer = new Timer();
        Timer optimer = new Timer();
        Queue<PMToBeSent> pmstobesent;
        CheckMyStats _cms;
        Int32 signupthreadid;
        PMs pmform;
        OP opform;
        DataGridView pmData;

        public Raccoon()
        {
            InitializeComponent();
            string[] keys = new List<string>(rolepms.DefaultRoleSets.Keys).ToArray();
            boxRoleSetSelect.Items.AddRange(keys);
            boxRoleSetSelectLoad.Items.AddRange(keys);
            _cms = new CheckMyStats();
            _cms.RandReadEvent += new EventHandler<RandReadEventArgs>(_cms_NewRandEvent);
            _cms.RickyLogin("Ricky Raccoon");
            saved = true;
        }

        void _cms_NewRandEvent(object sender, RandReadEventArgs e)
        {
            if (txtGameName.InvokeRequired)
            {
                txtGameName.Invoke((MethodInvoker)(() => { _cms_NewRandEvent(sender, e); }));
                return;
            }
            Console.WriteLine("Got new rand event!");
            loadRoster(e.Playerlist.Replace(",", "\r\n"));
            txtGameName.Text = e.GameName;
            signupthreadid = e.SignUpThreadID;
            testrun = false;
            remote = true;
            makeGame(e.Username, e.Password);
        }

        private void makeGame(string username, string password)
        {
            String host = "forumserver.twoplustwo.com";
            _synchronousInvoker = a => Invoke(a);
            _forum = new VBulletinForum(_synchronousInvoker, host, "3.8.7", Language.English, "59/puzzles-other-games/");
            _forum.LoginEvent += new EventHandler<LoginEventArgs>(_forum_LoginEvent_mq);
            if ((username != String.Empty) && (password != String.Empty))
            {
                _forum.Login(username, password);
            }
        }

        private void boxRoleSetSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox roleset = (ComboBox)sender;
            gamepms = JsonConvert.DeserializeObject<RolePMSet>(rolepms.DefaultRoleSets[roleset.Text]);
            int playerCount = 0;
            txtRoleList.Text = "";
            for (int i = 0; i < gamepms.Teams.Count; i++)
            {
                for (int j = 0; j < gamepms.Teams[i].Members.Count; j++)
                {
                    string subrole = gamepms.Teams[i].Members[j].SubRole;
                    if (gamepms.Teams[i].Members[j].SubRole != "") subrole = gamepms.Teams[i].Members[j].SubRole + " ";
                    string team = gamepms.Teams[i].Name;
                    if (gamepms.Teams[i].Name != "") team = gamepms.Teams[i].Name + " ";
                    txtRoleList.Text += gamepms.Teams[i].Members[j].Count + "X " + team + subrole + gamepms.Teams[i].Members[j].Role + " with " + gamepms.Teams[i].Members[j].n0 + Environment.NewLine;
                    playerCount += gamepms.Teams[i].Members[j].Count;
                }
            }
            txtRoleCount.Text = Convert.ToString(playerCount);
            if (Convert.ToInt16(txtRoleCount.Text) > 0 && roster.Count > 0 && roster.Count == Convert.ToInt16(txtRoleCount.Text))
            {
                btnDoIt.Enabled = true;
                btnTest.Enabled = true;
            }
            else
            {
                btnDoIt.Enabled = false;
                btnTest.Enabled = false;
            }
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
                for (int i = 0; i < gamepms.Teams.Count; i++)
                {
                    for (int j = 0; j < gamepms.Teams[i].Members.Count; j++)
                    {
                        string subrole = gamepms.Teams[i].Members[j].SubRole;
                        if (gamepms.Teams[i].Members[j].SubRole != "") subrole = gamepms.Teams[i].Members[j].SubRole + " ";
                        string team = gamepms.Teams[i].Name;
                        if (gamepms.Teams[i].Name != "") team = gamepms.Teams[i].Name + " ";
                        txtRoleList.Text += gamepms.Teams[i].Members[j].Count + "X " + team + subrole + gamepms.Teams[i].Members[j].Role + " with " + gamepms.Teams[i].Members[j].n0 + Environment.NewLine;
                        playerCount += gamepms.Teams[i].Members[j].Count;
                    }
                }
                for (int i = 0; i < gamepms.Teams.Count; i++)
                {
                    Console.WriteLine(gamepms.Teams[i].Name + gamepms.Teams[i].Share);
                }
                txtRoleCount.Text = Convert.ToString(playerCount);
                if (Convert.ToInt16(txtRoleCount.Text) > 0 && roster.Count > 0 && roster.Count == Convert.ToInt16(txtRoleCount.Text))
                {
                    btnDoIt.Enabled = true;
                    btnTest.Enabled = true;
                }
                else
                {
                    btnDoIt.Enabled = false;
                    btnTest.Enabled = false;
                }
            }
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
                boxPMsinOP.Enabled = true;
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
                boxPMsinOP.Enabled = false;
            }
        }

        private void loadRoster(string playerlist)
        {
            roster.Clear();
            String[] lines = playerlist.Split(new String[] { "\r\n", "\r" }, StringSplitOptions.RemoveEmptyEntries);
                foreach (String line in lines)
                {
                    String name = line.Trim();
                    if (Regex.IsMatch(name, @"^[\s]*$")) continue;
                    Console.WriteLine(name);
                    roster.Add(name);
                }
                roster.Sort();
                txtPlayerCount.Text = roster.Count.ToString();
                txtPlayerList.Text = String.Join(Environment.NewLine, roster);
                int temp = 0;
                if (!Int32.TryParse(txtRoleCount.Text, out temp))
                {
                    btnDoIt.Enabled = false;
                    btnTest.Enabled = false;
                    return;
                }
                if (Convert.ToInt16(txtRoleCount.Text) > 0 && roster.Count > 0 && roster.Count == Convert.ToInt16(txtRoleCount.Text))
                {
                    btnDoIt.Enabled = true;
                    btnTest.Enabled = true;
                }
                else
                {
                    btnDoIt.Enabled = false;
                    btnTest.Enabled = false;
                }
        }

        private void btnPasteList_Click(object sender, EventArgs e)
        {         
            object o = Clipboard.GetData(DataFormats.Text);
            if (o != null)
            {
                String clip = o as String;
                loadRoster(clip);
            }                
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            testrun = true;
            makeOPAndRand();
        }

        private void btnDoIt_Click(object sender, EventArgs e)
        {
            testrun = false;
            makeOPAndRand();
        }

        private void makeOPAndRand()
        {
            btnDoIt.Enabled = false;
            btnTest.Enabled = false;
            remote = false;
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

            if (!testrun && MessageBox.Show("Are you sure you want to continue? You can't go back after this! (PMs will be sent and thread will be made even if you close the processing dialog)", "Continue?", MessageBoxButtons.YesNo) != DialogResult.Yes)
            {
                return;
            }

            processing = new Processing();
            processing.txtProgress.Text = "Logging in...";
            processing.barProgress.Maximum = 100;
            processing.barProgress.Minimum = 0;
            processing.barProgress.Value = 0;
            processing.Show();
            String username = txtUsername.Text;
            String password = txtPassword.Text;
            if (boxRicky.Checked)
            {
                username = "Ricky Raccoon";
                password = "DGmZSnD8";
            }
            makeGame(username, password);
        }

        private void ChangeProcessing(string text, int value)
        {
            if (!remote && processing != null && !processing.IsDisposed)
            {
                processing.txtProgress.Text = text;
                processing.barProgress.Value = value;
            }
        }

        private List<String> checkPMReceive()
        {
            List<String> errors = new List<string>();
            for (int i = 0; i < roster.Count; i++)
            {                
                if (!_forum.CanUserReceivePM(roster[i]))
                {
                    Console.WriteLine(roster[i]);
                    errors.Add(roster[i]);
                }
            }
            return errors;
        }

        private void _forum_LoginEvent_mq(object sender, POG.Forum.LoginEventArgs e)
        {
            switch (e.LoginEventType)
            {
                case POG.Forum.LoginEventType.LoginFailure:
                    if(remote)
                        _forum.MakePost(signupthreadid, txtGameName.Text, "There was an error logging in!", 0, false);
                    else
                        MessageBox.Show("There was an error logging in!");
                    return;

                case POG.Forum.LoginEventType.LoginSuccess:
                    if(!remote)
                        MessageBox.Show("Login Success! This may take a while...");
                    break;
            }
            ChangeProcessing("Checking preconditions...", 5);
            if (!remote && (boxMajLynch.Text == "" || boxSODTime.Text == "" || boxEODTime.Text == "" || boxWolfChat.Text == "" || roster.Count < 1 || boxMustLynch.Text == "" || txtGameName.Text == ""))
            {
                MessageBox.Show("Please fill in all boxes before submitting");
                return;
            }
            if (!remote && boxPMsinOP.Checked && MessageBox.Show("Are you sure you want to put the PMs in the OP? This is NOT recommended if this game is a Mish-Mash!", "Continue?", MessageBoxButtons.YesNo) != DialogResult.Yes)
            {
                return;
            }
            ChangeProcessing("Checking to see if users can recieve PMs...", 10);
            List<String> errors = checkPMReceive();
            if (errors.Count > 0)
            {
                if(remote)
                    _forum.MakePost(signupthreadid, txtGameName.Text, String.Format("{0} is spelled wrong, or can't receive PMs. Please correct and rerun the rand", String.Join(",", errors)), 0, false);
                else
                    MessageBox.Show(String.Format("{0} is spelled wrong, or can't receive PMs. Please correct and rerun the rand", String.Join(",", errors)));
                return;
            }
            ChangeProcessing("Making OP...", 20);
            if (!makeOP())
            {
                if (remote)
                   _forum.MakePost(signupthreadid, txtGameName.Text, "Fatal error making the OP :(", 0, false);
                else
                   MessageBox.Show("Failure Sending OP");
                return;
            }
        }

        private void finishSending()
        {
            int interval;
            if (boxMod.Checked)
                interval = 1;
            else
                interval = 40000;
            if (!testrun)
            {
                sendPMs(interval);
            }
            clearRand();
        }

        private void sendPMs(int interval)
        {
            pmtimer.Interval = interval;
            pmtimer.Tick += new EventHandler(pmtimer_Tick);
            pmtimer.Enabled = true;
            pmtimer.Start();
        }

        private void pmFormClosing(object sender, FormClosingEventArgs e)
        {
            DataGridView pmData = pmform.getDataPMs();
            if (pmform.savePMs)
            {
                if (!remote)
                {
                    String roleset = JsonConvert.SerializeObject(gamepms, Formatting.Indented, new JsonSerializerSettings()
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    });
                    // Show the dialog and get result.
                    saveFileDialog.FileName = gamepms.Name + DateTime.Now.ToString("MMMddyyyy hhmm") + "randed";

                    DialogResult result = saveFileDialog.ShowDialog();
                    if (result == DialogResult.OK) // Test result.
                    {
                        using (Stream s = File.Open(saveFileDialog.FileName, FileMode.Create))
                        using (StreamWriter sw = new StreamWriter(s))
                        {
                            sw.Write(roleset);
                        }
                    }
                }
            }
            for (int i = 0; i < pmData.Rows.Count; i++)
            {
                string recipients = (String)pmData.Rows[i].Cells[0].Value;
                char[] split = { ';' };
                pmstobesent.Enqueue(new PMToBeSent(new List<string>(recipients.Split(split)), (String)pmData.Rows[i].Cells[1].Value, (String)pmData.Rows[i].Cells[2].Value));
            }
            finishSending();
        }

        private void opFormClosing(object sender, FormClosingEventArgs e)
        {
            if (opform.cancelRand)
                return;
            else if (opform.makeOP && !testrun)
            {
                string optext = opform.getOPTextBox().Text;
                txtGameURL.Text = _forum.NewThread(59, txtGameName.Text, optext, 18, true).Item1;
                gamepms.GameURL = txtGameURL.Text;
            }
            gamepms.GameName = txtGameName.Text;
            ChangeProcessing("Randomizing Player List...", 30);
            randomizeRoster();
            ChangeProcessing("Assigning Players to Roles...", 30);
            assignRoles();
            ChangeProcessing("Making PMs...", 50);
            makePMs();
            ChangeProcessing("Sending PMs (Please be patient)...", 90);
        }

        private void makePMs()
        {
            if (!remote)
            {
                pmform = new PMs();
                pmData = pmform.getDataPMs();
                pmData.AllowUserToAddRows = false;
                pmform.FormClosing += pmFormClosing;
                pmform.Show();
            }
            pmstobesent = new Queue<PMToBeSent>();
            for (int i = 0; i < gamepms.Teams.Count; i++)
            {
                for (int j = 0; j < gamepms.Teams[i].Members.Count; j++)
                {
                    RolePM role = gamepms.Teams[i].Members[j];
                    if (role.n0 != "a random villager peek" && role.n0 != "a random peek across entire playerlist")
                    {
                        string pm = role.FullPM(txtGameURL.Text, gamepms, gamepms.Teams[i], new Player("", true), true);
                        for (int k = 0; k < gamepms.Teams[i].Members[j].Players.Count; k += 8)
                        {
                            if (!remote)
                            {
                                Console.WriteLine("not seer");
                                Console.WriteLine(String.Join(";", gamepms.Teams[i].Members[j].Players.GetRange(k, Math.Min(8, gamepms.Teams[i].Members[j].Players.Count - k)).Select(player => player.Name)) + pm);
                                pmData.Rows.Add(String.Join(";", gamepms.Teams[i].Members[j].Players.GetRange(k, Math.Min(8, gamepms.Teams[i].Members[j].Players.Count - k)).Select(player => player.Name)), txtGameName.Text + " Role PM", pm);
                            }
                            else 
                                pmstobesent.Enqueue(new PMToBeSent(gamepms.Teams[i].Members[j].Players.GetRange(k, Math.Min(8, gamepms.Teams[i].Members[j].Players.Count - k)).Select(player => player.Name).ToList(), txtGameName.Text + " Role PM", pm));
                       }
                    }
                    else
                    {
                        for (int k = 0; k < gamepms.Teams[i].Members[j].Players.Count; k++)
                        {
                            string pm = role.FullPM(txtGameURL.Text, gamepms, gamepms.Teams[i], gamepms.Teams[i].Members[j].Players[k], true);
                            if (!remote)
                            {
                                Console.WriteLine("seer");
                                Console.WriteLine(gamepms.Teams[i].Members[j].Players[k].Name + pm);
                                pmData.Rows.Add(gamepms.Teams[i].Members[j].Players[k].Name, txtGameName.Text + " Role PM", pm);
                            }
                            else 
                                pmstobesent.Enqueue(new PMToBeSent(new List<string>(new string[] { gamepms.Teams[i].Members[j].Players[k].Name }), txtGameName.Text + " Role PM", pm));
                        }
                    }
                }
            }
            if (remote)
                finishSending();
        }

        private void assignRoles()
        {
            int curplayer = 0;
            for (int i = 0; i < gamepms.Teams.Count; i++)
            {
                for (int j = 0; j < gamepms.Teams[i].Members.Count; j++)
                {
                    RolePM role = gamepms.Teams[i].Members[j];
                    for (int k = curplayer; k < curplayer + role.Count; k++)
                    {
                        role.Players.Add(new Player(roster[k], true));
                    }
                    curplayer += role.Count;
                }
            }
        }

        private void randomizeRoster()
        {
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
        }

        private void clearRand()
        {
            roster.Sort();
            for (int i = 0; i < gamepms.Teams.Count; i++)
            {
                for (int j = 0; j < gamepms.Teams[i].Members.Count; j++)
                {
                    gamepms.Teams[i].Members[j].Players.Clear();
                }
            }
        }

        void optimer_Tick(object sender, EventArgs e)
        {           
           optimer.Stop();
           _forum.MakePost(signupthreadid, "RANDING", "Randing STFU!!!", 0, false);    
        }

        void pmtimer_Tick(object sender, EventArgs e)
        {
            Console.WriteLine(pmstobesent.Count);
            if (pmstobesent.Count > 0)
            {
                PMToBeSent pm = pmstobesent.Dequeue();
                ChangeProcessing(String.Format("Sent PM. {0} left...", pmstobesent.Count), 50);
                if (!testrun)
                {
                    pm.SendPM(_forum);
                }
            }
            if(pmstobesent.Count == 0)
            {
                pmtimer.Stop();
                ChangeProcessing(String.Format("Sent All PMs!", pmstobesent.Count), 100);
                if (testrun)
                {
                    ChangeProcessing(String.Format("Test success! You can close the processing window at any time.", pmstobesent.Count), 100);
                }
            }
        }


        private bool makeTurboOP()
        {
            string pms = "";
            for (int i = 0; i < gamepms.Teams.Count; i++)
            {
                for (int j = 0; j < gamepms.Teams[i].Members.Count; j++)
                {
                    pms += "[quote]" + gamepms.Teams[i].Members[j].EditedPM(txtGameURL.Text, gamepms.Teams[i]) + "[/quote]" + Environment.NewLine + Environment.NewLine;
                }
            }
            string optext = String.Format(@"{0}
1st day is {1} minutes long, subsequent days are {2} minutes long. Nights are {3} minutes long, and NAs will be randed if not recieved by that time.
There is {4} after d1
Must Lynch rules are {5}
Wolf Chat is {6}

PMs: 
{7}

Playerlist: 
{8}

You will receive your PMs shortly.

This post was made by Ricky Raccoon. Forward all complaints/suggestions/bugs to Krayz or Chips Ahoy

[b]IT IS NIGHT, DO NOT POST[/b]", txtRoleList.Text, txtDay1Length.Text, txtDayLength.Text, txtNightLength.Text, boxMajLynch.Text, boxMustLynch.Text, boxWolfChat.Text, pms, String.Join(Environment.NewLine, roster));
            txtOP.Text = optext;
                opform = new OP();
                opform.getOPTextBox().Text = optext;
                opform.FormClosing += opFormClosing;
                opform.Show();
            return true;
        }

        private bool makeOP()
        {
            if (!boxTurbo.Checked && !boxLongGame.Checked)
            {
                MessageBox.Show("You must select either long game or a turbo!");
                return false;
            }
            if (boxTurbo.Checked)
            {
                return makeTurboOP();
            }
            string pms = "PMs:" + Environment.NewLine;
            string textrolelist = "";
            if (boxPMsinOP.Checked)
            {
                for (int i = 0; i < gamepms.Teams.Count; i++)
                {
                    for (int j = 0; j < gamepms.Teams[i].Members.Count; j++)
                    {
                        pms += "[quote]" + gamepms.Teams[i].Members[j].EditedPM(txtGameURL.Text, gamepms.Teams[i]) + "[/quote]" + Environment.NewLine + Environment.NewLine;
                    }
                }
                textrolelist = txtRoleList.Text;
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

            string optext = String.Format(@"[b][u]POG Werewolf Rules[/u][/b]
[indent]For more details, please see [url=http://forumserver.twoplustwo.com/showpost.php?p=32574213&postcount=5]Werewolf Rules And Etiquette[/url][/indent][list][*][b][color=green]As long as the game is ongoing, you may talk about the game only inside the game thread[/color][/b][*][b][color=green]You may post in the game thread only when it is [u]DAY[/u][/color][/b][*][b][color=green]You may post only when you are alive[/color][/b][*][b][color=green]If you have a role which allows you to communicate outside the game thread, then you may do so only when allowed by your role, and only with individuals specified by your role[/color][/b][*][b][color=green]You may not edit or delete posts in the game thread for any reason at any time[/color][/b][*][b][color=green]You may not post screenshots of, or directly quote your role PM, seer peeks, or any other game-related communication that originated outside of the game thread[/color][/b][*][b][color=green]You are expected to participate[/color][/b][*][b][color=green]You are expected to be familiar with the rules, and you are expected to abide by them even if you think they are incorrect[/color][/b][*][b][color=green]You are expected to behave civilly[/color][/b][/list][indent]

Werewolf is a game about lying and catching people lying. It's an adversarial game and arguments between players are normal and expected. Passion and Intensity are fine but excessively personal attacks or insults are not. Even in a werewolf game you must respect the forum rules about civility. [u]Failure to do so may cause you to be infracted or temp banned, and repeated problems may get you perma-banned.[/u]

Werewolf is also a community and team-based game. While there are many styles and strategies and reasons for playing and you may choose your own, you are expected to be respectful of the time and energy others put in as players and as moderators. [u]You are expected to play to win. Intentionally sabotaging your team, or choosing strategies with the sole purpose of trolling other players in the game is not allowed.[/u][/indent]
[b][U]The Setup[/U][/b][indent]
{0}{1}

[U]Voting[/U]

Each day, each player is allowed (and encouraged) to vote to lynch a player. To do so, type the name of the player in [B]bold[/B]; That is: [b]zurvan[/b]

You may remove your vote by posting '[B]unvote[/B]'. You may change your vote simply by voting the new player; you do not need to unvote.

At the end of the scheduled game day, the player with the most votes is lynched, and his role revealed in the thread. If two or more players are tied for the most votes, one of the tied players is chosen randomly by the moderator.

{1}
[/indent]
[b][U]Day and Night Times[/U][/b]
[indent]All times give are server time, which is also Eastern Standard Time.

I will open the thread every morning by posting '[B]It is day.[/B]' You may not post in the thread until I have made the morning post. Day will usually start at [color=red]{2}[/color]

Day lasts until [b][color=red]{3}[/color][/b]. Posting at [b][color=red]{4}[/color][/b] is acceptable, and votes with that time stamp will be counted. Posting after that, even just a minute after, is not acceptable, even if I have not yet declared it night by posting '[B]It is night.[/B]' Do not post in the thread at any time, for any reason, between the end of the day and the opening of the thread the next morning.

Night will fall on the following days: {5}

If this game has majority and any player reaches majority and is lynched before 2 pm, the moderator may, at his discretion, reopen the thread for a second game day that same day. He will do so only if it seems clear that this will not unduly disadvantage either side.[/indent]
[b][U]Wolf Chat[/U][/b]
[indent]Wolves may communicate with each other by any means they desire during [I]wolf chat[/I]. 

[b][color=red]Wolf chat is: {6}[/color][/b][/indent]
[b][U]The Roster[/U][/b]
[indent][color=red][b]{7}[/b][/color][/indent]
[b][U]Questions and Confirmation[/U][/b]
[indent][color=red][b]Any questions regarding the rules should be submitted to the moderator by PM ONLY[/b][/color][/indent]
[U][b]Must Lynch[/b][/u]
[indent][color=red]{8}[/color][/indent]

You will receive your PMs shortly.

This post was made by Ricky Raccoon. Forward all complaints/suggestions/bugs to Krayz or Chips Ahoy

[b]IT IS NIGHT DO NOT POST[/b]", textrolelist, pms, majlynchtext, boxSODTime.Text, boxEODTime.Text, boxEODTime.Text, lynchdays, boxWolfChat.Text, String.Join(Environment.NewLine, roster), boxMustLynch.Text);
            txtOP.Text = optext;
                opform = new OP();
                opform.getOPTextBox().Text = optext;
                opform.FormClosing += opFormClosing;
                opform.Show();
            return true;
        }

        private void txtGameTitle_TextChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < rolepms.Teams.Count; i++)
            {
                for (int j = 0; j < rolepms.Teams[i].Members.Count; j++)
                {
                    dataRoles.Rows[i].Cells["txtFullPM"].Value = rolepms.Teams[i].Members[j].EditedPM(txtGameURL.Text, rolepms.Teams[i]);
                }
            }
            rolepms.GameURL = txtGameURL.Text;
        }

        private void txtRoleSetName_TextChanged(object sender, EventArgs e)
        {
            rolepms.Name = txtRoleSetName.Text;
        }



        private void btnSaveRoleSet_Click(object sender, EventArgs e)
        {
            SaveRoleSet();
        }

        private void SaveRoleSet()
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
            saved = true;
        }

        private void resetRoleList(RolePMSet rolepmset)
        {
            dataRoles.Rows.Clear();
            dataTeams.Rows.Clear();
            rolepms = rolepmset;
            for (int i = 0; i < rolepms.Teams.Count(); i++)
            {
                dataTeams.Rows.Add(rolepms.Teams[i].Name, rolepms.Teams[i].WinCon, rolepms.Teams[i].Color, rolepms.Teams[i].Share, 0);
            }
            DataGridViewComboBoxColumn comboboxColumn = (DataGridViewComboBoxColumn)dataRoles.Columns[0];
            DataGridViewComboBoxColumn roleColumn = (DataGridViewComboBoxColumn)dataRoles.Columns[1];
            DataGridViewComboBoxColumn subRoleColumn = (DataGridViewComboBoxColumn)dataRoles.Columns[2];
            comboboxColumn.DataSource = rolepms.Teams;
            comboboxColumn.DisplayMember = "Name";
            comboboxColumn.ValueMember = "Self";
            for (int i = 0; i < rolepms.Teams.Count; i++)
            {
                for (int j = 0; j < rolepms.Teams[i].Members.Count; j++)
                {
                    if (!roleColumn.Items.Contains(rolepms.Teams[i].Members[j].Role) && rolepms.Teams[i].Members[j].Role.Length > 0) roleColumn.Items.Add(rolepms.Teams[i].Members[j].Role);
                    if (!subRoleColumn.Items.Contains(rolepms.Teams[i].Members[j].SubRole) && rolepms.Teams[i].Members[j].SubRole.Length > 0) subRoleColumn.Items.Add(rolepms.Teams[i].Members[j].SubRole);
                    dataRoles.Rows.Add(rolepms.Teams[i], rolepms.Teams[i].Members[j].Role, rolepms.Teams[i].Members[j].SubRole, rolepms.Teams[i].Members[j].Color, rolepms.Teams[i].Members[j].ExtraFlavor, rolepms.Teams[i].Members[j].Redacted, rolepms.Teams[i].Members[j].n0, "", rolepms.Teams[i].Members[j].Count);
                }
            }
            txtRoleSetName.Text = rolepms.Name;
            dataRoles_CellValueChanged(null, null);
        }

        private void boxRoleSetSelectLoad_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!saved)
            {
                if (MessageBox.Show("Do you want to save your progress before loading a new file?", "Save?", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    SaveRoleSet();
                }
            }
            ComboBox roleset = (ComboBox)sender;
            resetRoleList(JsonConvert.DeserializeObject<RolePMSet>(rolepms.DefaultRoleSets[roleset.Text]));
            saved = true;
        }

        private void btnLoadRoleSet_Click(object sender, EventArgs e)
        {
            if (!saved)
            {
                if (MessageBox.Show("Do you want to save your progress before loading a new file?", "Save?", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    SaveRoleSet();
                }
            }
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
                saved = true;
            }            
        }


        
        private void dataRoles_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            //for (int i = rolepms.Roles.Count; i < dataRoles.Rows.Count - 1; i++)
            //{
            //   rolepms.Roles.RemoveAt(i);
            //}
            saved = false;
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
                string color = "black";
                if (dataRoles.Rows[i].Cells["boxPlayerColor"].Value != null) color = dataRoles.Rows[i].Cells["boxPlayerColor"].Value.ToString();
                string n0 = "";
                if (dataRoles.Rows[i].Cells["boxn0"].Value != null) n0 = dataRoles.Rows[i].Cells["boxn0"].Value.ToString();
                int count = 0;
                if (dataRoles.Rows[i].Cells["txtCount"].Value != null) count = Convert.ToInt16(dataRoles.Rows[i].Cells["txtCount"].Value.ToString());
                string redacted = "";
                if (dataRoles.Rows[i].Cells["txtRedacted"].Value != null) redacted = dataRoles.Rows[i].Cells["txtRedacted"].Value.ToString();
                RolePM rolepm = new RolePM(role, subrole, color, extraflavor, n0, count, i, redacted);
                rolepms.setRolePM(rolepm, team, i);
                playerCount += count;
                dataRoles.Rows[i].Cells["txtFullPM"].Value = rolepm.EditedPM(txtGameURL.Text, team);
            }
            txtPlayers.Text = Convert.ToString(playerCount);
            for (int i = 0; i < dataTeams.Rows.Count; i++)
            {
                if (!dataTeams.Columns.Contains("colTeamName")) continue;
                if (dataTeams.Rows[i].Cells["colTeamName"].Value == null || dataTeams.Rows[i].Cells["colWinCon"].Value == null || dataTeams.Rows[i].Cells["boxColor"].Value == null)
                    continue;
                Team team = rolepms.Teams[i];
                int count = 0;
                for (int j = 0; j < team.Members.Count; j++)
                {
                    count += team.Members[j].Count;
                }
                dataTeams.Rows[i].Cells["txtTeamCount"].Value = count;
            }
        }

        private void dataRoles_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {            
            int rowIndex = e.Row.Index;
            Console.WriteLine(String.Format("Deleting row: {0}", rowIndex));
            rolepms.removeRolePM(rowIndex);
        }

        private void dataRoles_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            String columnName = this.dataRoles.Columns[this.dataRoles.CurrentCellAddress.X].Name;
            Console.WriteLine(columnName);
            if (columnName != "boxRole" && columnName != "boxSubRole") return;
            ComboBox cb = e.Control as ComboBox;
            if (cb != null)
            {
                cb.DropDownStyle = ComboBoxStyle.DropDown;
            }
        }

        private void dataRoles_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            String columnName = this.dataRoles.Columns[e.ColumnIndex].Name;
            Console.WriteLine(columnName);
            if (columnName != "boxRole" && columnName != "boxSubRole") return;
            DataGridViewComboBoxColumn col = this.dataRoles.Columns[e.ColumnIndex] as DataGridViewComboBoxColumn;
            if (!col.Items.Contains(e.FormattedValue) && e.FormattedValue.ToString().Length > 0)
            {
                col.Items.Add(e.FormattedValue);
            }
        }



        private void dataTeams_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            saved = false;
            List<Team> teams = rolepms.Teams;
            for (int i = 0; i < dataTeams.Rows.Count; i++)
            {

                if (!dataTeams.Columns.Contains("colTeamName")) continue;
                if (dataTeams.Rows[i].Cells["colTeamName"].Value == null || dataTeams.Rows[i].Cells["colWinCon"].Value == null || dataTeams.Rows[i].Cells["boxColor"].Value == null)
                    continue;
                string teamname = dataTeams.Rows[i].Cells["colTeamName"].Value.ToString();
                string wincon = dataTeams.Rows[i].Cells["colWinCon"].Value.ToString();
                string color = dataTeams.Rows[i].Cells["boxColor"].Value.ToString();
                bool share = Convert.ToBoolean(dataTeams.Rows[i].Cells["colShare"].Value);
                if (i < teams.Count)
                {
                    teams[i].Name = teamname;
                    teams[i].WinCon = wincon;
                    teams[i].Share = share;
                    teams[i].Color = color;
                }
                else
                {
                    Team team = new Team(teamname, wincon, share, color);
                    teams.Add(team);
                }
            }
            DataGridViewComboBoxColumn comboboxColumn = (DataGridViewComboBoxColumn)dataRoles.Columns[0];
            comboboxColumn.DataSource = rolepms.Teams;
            comboboxColumn.DisplayMember = "Name";
            comboboxColumn.ValueMember = "Self";
        }

        private void dataTeams_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            int rowIndex = e.Row.Index;
            Team team = rolepms.Teams[rowIndex];
            if (team.Members.Count > 0)
                e.Cancel = true;
            else
                rolepms.Teams.RemoveAt(rowIndex);
        }

        

        private void Raccoon_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!saved)
            {
                if (MessageBox.Show("Do you want to save your progress before exiting?", "Save?", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    SaveRoleSet();
                }
            }
        }
     }
}

