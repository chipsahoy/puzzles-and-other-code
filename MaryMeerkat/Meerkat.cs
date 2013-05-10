using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Newtonsoft.Json;
using System.IO;
using POG.Forum;
using POG.Werewolf;
using POG.FennecFox;

namespace MaryMeerkat
{
    public partial class Meerkat : Form
    {
       
       private VBulletinForum _forum;
       private Action<Action> _synchronousInvoker;
       private Boolean _loggedIn = false;
       Boolean _inLoginDialog = false;
       private RolePMSet gamepms;

        public Meerkat()
        {
            InitializeComponent();
        }

        private void Meerkat_Load(object sender, EventArgs e)
        {
            ShowLogin();
        }

        private void ShowLogin()
        {
            if (!_inLoginDialog)
            {
                _inLoginDialog = true;
                _synchronousInvoker = a => Invoke(a);
                String host = "forumserver.twoplustwo.com";

                _forum = new VBulletinForum(_synchronousInvoker, host, "3.8.7", "59/puzzles-other-games/");
                _forum.LoginEvent += new EventHandler<LoginEventArgs>(_forum_LoginEvent);

                LoginDialog dlg = new LoginDialog(_forum);
                DialogResult dr = dlg.ShowDialog();
                if (!_loggedIn)
                {
                    Application.Exit();
                }
                else
                {
                    Console.WriteLine("Logged in to forum");
                }
                _inLoginDialog = false;
            }
        }

        private void _forum_LoginEvent(object sender, POG.Forum.LoginEventArgs e)
        {
            switch (e.LoginEventType)
            {
                case POG.Forum.LoginEventType.LoginFailure:
                    {
                        ShowLogin();
                    }
                    break;

                case POG.Forum.LoginEventType.LoginSuccess:
                    {
                        _loggedIn = true;
                    }
                    break;

                case POG.Forum.LoginEventType.LogoutSuccess:
                    {
                        _loggedIn = false;
                    }
                    break;
            }
        }

        private void loadGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Show the dialog and get result.
            DialogResult result = openFileDialog.ShowDialog();
            if (result == DialogResult.OK) // Test result.
            {
                string file = openFileDialog.FileName;
                try
                {
                    RolePMSet temp = JsonConvert.DeserializeObject<RolePMSet>(File.ReadAllText(file));
                    resetGameList(temp);
                }
                catch (JsonSerializationException error)
                {
                    Console.WriteLine(error);
                    MessageBox.Show("This isn't valid JSON!");
                    return;
                }
            }
        }

        private void resetGameList(RolePMSet pms)
        {
            dataPlayers.Rows.Clear();
            dataTeams.Rows.Clear();
            gamepms = pms;
            for (int i = 0; i < gamepms.Teams.Count(); i++)
            {
                dataTeams.Rows.Add(gamepms.Teams[i].Name, gamepms.Teams[i].Color, 0, 0, gamepms.Teams[i].WinCon);
            }
            for (int i = 0; i < gamepms.Teams.Count; i++)
            {
                for (int j = 0; j < gamepms.Teams[i].Members.Count; j++)
                {
                    for (int k = 0; k < gamepms.Teams[i].Members[j].Players.Count; k++)
                    {
                        Console.WriteLine(gamepms.Teams[i].Members[j].Players[k].Alive.ToString());
                        dataPlayers.Rows.Add(false,gamepms.Teams[i].Members[j].Players[k].Name, gamepms.Teams[i], gamepms.Teams[i].Members[j], gamepms.Teams[i].Members[j].Color, gamepms.Teams[i].Members[j].Players[k].Alive.ToString());
                    }
                }
            }
            txtGameURL.Text = gamepms.GameURL;
            lblStatus.Text = "Game Loaded";
        }

        private void loadGameToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            String roleset = JsonConvert.SerializeObject(gamepms, Formatting.Indented, new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
            // Show the dialog and get result.
            saveFileDialog.FileName = gamepms.Name;
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

        private void dataPlayers_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            /* not relevant right now, maybe later?
             */
        }

        private void menuPlayerList_Click(object sender, EventArgs e)
        {
            bool posted = false;
            if (txtGameURL.Text != "")
            {
               posted = _forum.MakePost(POG.Utils.Misc.TidFromURL(txtGameURL.Text), "Player List", String.Format("[b]Playerlist ([color=red]{0}[/color]):{1}{2}[/b]", gamepms.GetAliveRoster().Count, Environment.NewLine, String.Join(Environment.NewLine, gamepms.GetAliveRoster().Select(player => player.Name))), 0, false);
            }
            if (!posted)
                MessageBox.Show("Post FAIL. Try checking the URL or waiting 25 seconds?");
            else
                lblStatus.Text = "Player List Posted";
        }

        private void lockThreadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool locked = _forum.LockThread(POG.Utils.Misc.TidFromURL(txtGameURL.Text), true);
            if (!locked)
                MessageBox.Show("Lock FAIL. Try checking the URL or if you have lock access to this thread?");
            else
                lblStatus.Text = "Thread Locked";
        }

        private void btnSelectAll_Click(object sender, EventArgs e)
        {
            bool allselected = true;
            for (int i = 0; i < dataPlayers.Rows.Count; i++)
            {
                if((bool)dataPlayers.Rows[i].Cells["boxSelect"].Value == false)
                    allselected = false;
            }
            for (int i = 0; i < dataPlayers.Rows.Count; i++)
            {
                dataPlayers.Rows[i].Cells["boxSelect"].Value = !allselected;
            }
        }

        private void btnSubOut_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < dataPlayers.Rows.Count; i++)
            {
                if ((bool)dataPlayers.Rows[i].Cells["boxSelect"].Value == false)
                {
                    continue;
                }
                SubDialog dialog = new SubDialog();
                if (dialog.ShowDialog(this) == DialogResult.OK)
                {
                    string subin = dialog.txtSub.Text;
                    Console.WriteLine(subin);
                    RolePM role = (RolePM)dataPlayers.Rows[i].Cells["txtRole"].Value;
                    Team team = (Team)dataPlayers.Rows[i].Cells["boxTeam"].Value;
                    string playername = dataPlayers.Rows[i].Cells["txtPlayerName"].Value.ToString();
                    bool posted = _forum.MakePost(POG.Utils.Misc.TidFromURL(txtGameURL.Text), "Sub Post", String.Format("[b][color=red]{0} is subbing in for {1}![/color][/b]", subin, playername), 0, false);
                    bool pmsent = _forum.SendPM(new List<string>(new string[] { subin }), null, "Sub Role PM", String.Format("You are subbing in for {0}. Your role is: [quote]{1}[/quote]", playername, role.FullPM(txtGameURL.Text, gamepms, team, null)), false);
                    for (int j = 0; j < role.Players.Count; j++)
                    {
                        if (role.Players[j].Name == playername)
                        {
                            role.Players[j].Name = subin;
                            lblStatus.Text = "Subbed In";
                            dataPlayers.Rows[i].Cells["txtPlayerName"].Value = subin;
                        }
                    }
                }
                dialog.Dispose();
            }
        }

        private void btnDeathReveal_Click(object sender, EventArgs e)
        {
            string deathpost = "";
            for (int i = 0; i < dataPlayers.Rows.Count; i++)
            {
                if ((bool)dataPlayers.Rows[i].Cells["boxSelect"].Value == false)
                {
                    continue;
                }
                if (MessageBox.Show(String.Format("Are you sure you want to post the death reveal for {0}??? You can't go back after this!", dataPlayers.Rows[i].Cells["txtPlayerName"].Value.ToString()), "Continue?", MessageBoxButtons.YesNo) != DialogResult.Yes)
                {
                    return;
                }
                RolePM role = (RolePM)dataPlayers.Rows[i].Cells["txtRole"].Value;
                Team team = (Team)dataPlayers.Rows[i].Cells["boxTeam"].Value;
                string playername = dataPlayers.Rows[i].Cells["txtPlayerName"].Value.ToString();
                string deathreason = "";
                if (dataPlayers.Rows[i].Cells["txtDeathReason"].Value != null)
                    deathreason = dataPlayers.Rows[i].Cells["txtDeathReason"].Value.ToString();
                deathpost += String.Format("[b][color={0}]{1}[/color][/b] is dead! {2} {3} [quote]{4}[/quote]{5}", team.Color, playername, Environment.NewLine, deathreason, role.EditedPM(txtGameURL.Text, team), Environment.NewLine);
                dataPlayers.Rows[i].Cells["boxAlive"].Value = "False";
            }
            if (MessageBox.Show(String.Format("Are you sure you want to post this death reveal??? {0} You can't go back after this!", deathpost), "Continue?", MessageBoxButtons.YesNo) != DialogResult.Yes)
            {
                return;
            }
            bool posted = _forum.MakePost(POG.Utils.Misc.TidFromURL(txtGameURL.Text), "Death Post", deathpost, 0, false);
            if (!posted)
                MessageBox.Show("Post FAIL. Try checking the URL or waiting 25 seconds?");
            else
            {                
                lblStatus.Text = "Death Reveal Posted";
            }
        }

        private void btnPeek_Click(object sender, EventArgs e)
        {
            string peekpm = "";
            for (int i = 0; i < dataPlayers.Rows.Count; i++)
            {
                if ((bool)dataPlayers.Rows[i].Cells["boxSelect"].Value == false)
                {
                    continue;
                }
                
                RolePM role = (RolePM)dataPlayers.Rows[i].Cells["txtRole"].Value;
                Team team = (Team)dataPlayers.Rows[i].Cells["boxTeam"].Value;
                string playername = dataPlayers.Rows[i].Cells["txtPlayerName"].Value.ToString();
                if (MessageBox.Show(String.Format("Are you sure you want to pm a peek of {0}???", playername), "Continue?", MessageBoxButtons.YesNo) != DialogResult.Yes)
                {
                    return;
                }
                peekpm += String.Format("[b][color={0}]{1}[/color][/b] is your peek!{2}[quote]{3}[/quote]", team.Color, playername, Environment.NewLine, role.EditedPM(txtGameURL.Text, team));
                dataPlayers.Rows[i].Cells["boxAlive"].Value = "False";
            }
            string peeker = "";
            SubDialog dialog = new SubDialog();
            dialog.SetQuestion("Who is doing the peeking?");
            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                dialog.Dispose();
                peeker = dialog.txtSub.Text;
            }
            else
            {
                dialog.Dispose();
                return;
            }            
            if (MessageBox.Show(String.Format("Are you sure you want to pm this peek to {0}??? {1}", peeker, peekpm), "Continue?", MessageBoxButtons.YesNo) != DialogResult.Yes)
            {
                return;
            }
            bool pmsent = _forum.SendPM(new List<string>(new string[] { peeker }), null, "Peek Result", peekpm, false);
            if (!pmsent)
                MessageBox.Show("PM Sent FAIL. Try checking the name or waiting 30 seconds?");
            else
            {
                lblStatus.Text = "Peek PM Sent";
            }
        }

        private void dataPlayers_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            for (int i = 0; i < dataPlayers.Rows.Count; i++)
            {
                RolePM role = (RolePM)dataPlayers.Rows[i].Cells["txtRole"].Value;
                Team team = (Team)dataPlayers.Rows[i].Cells["boxTeam"].Value;
                string playername = dataPlayers.Rows[i].Cells["txtPlayerName"].Value.ToString();
                for (int j = 0; j < role.Players.Count; j++)
                {
                    if (role.Players[j].Name == playername)
                    {
                        role.Players[j].Alive = (bool)dataPlayers.Rows[i].Cells["boxAlive"].Value;
                    }
                }
            }
        }
    }
}
