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
using System.Text.RegularExpressions;
using POG.Automation;
using System.Media;

namespace MaryMeerkat
{
    public partial class Meerkat : Form
    {
        private CorralData _corral = new CorralData();
        Random _random = new Random();
        String _filename;
        PogCorral _forum = new PogCorral();

        public Meerkat()
        {
            InitializeComponent();
            _filename = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "\\corral.json";
        }

        private void Meerkat_Load(object sender, EventArgs e)
        {
            //ShowLogin();
            lbAlerts.Items.Add("FOUR");
            _forum.OnLoginResult += new PogCorral.LoginResultDelegate(_forum_OnLoginResult);
            _forum.OnPlayerActions += new PogCorral.PlayerActionsDelegate(_forum_OnPlayerActions);
            _forum.Login("http://forumserver.twoplustwo.com/59/puzzles-other-games/return-pog-corral-1390183/", "TimeLady", "dJURkAGdTygyhq");

            loadGameToolStripMenuItem_Click(null, EventArgs.Empty);
        }

        void _forum_OnPlayerActions(CorralEvents actions)
        {
            List<String> lines = new List<string>();
            foreach (CorralEvent action in actions)
            {
                String msg = "";
                switch (action.Action)
                {
                    case CorralAction.Arm:
                        msg = String.Format(" ARMS with {0}", action.Parameter);
                        break;
                    case CorralAction.Shoot:
                        msg = String.Format(" SHOOTS {0}", action.Target);
                        break;
                    case CorralAction.Steal:
                        msg = String.Format(" STEALS {0} FROM {1}", action.Parameter, action.Target);
                        break;
                    case CorralAction.Toss:
                        msg = String.Format(" TOSSES {0}", action.Parameter);
                        break;
                    case CorralAction.Trade:
                        msg = String.Format(" TRADES with {0}", action.Target);
                        break;
                    case CorralAction.UnArm:
                        msg = String.Format(" UNARMS");
                        break;
                    case CorralAction.NoAction:
                        if (action.PostNumber != 0) continue;
                        break;
                }
                String final = String.Format("{0:HH\\:mm} {1} {2} ({3})", action.TimeStamp, action.Actor, msg, action.Content);
                final = final.Replace('\r', ' ');
                final = final.Replace('\n', ' ');
                lines.Add(final);
            }
            if (lines.Count > 0)
            {
                lbAlerts.Invoke((Action)(() =>
                    {
                        foreach (var line in lines)
                        {
                            lbAlerts.Items.Insert(0, line);
                        }
                        FlashWindow.Flash(this);
                        String filename = Application.StartupPath + @"\\redalert.wav";
                        if (File.Exists(filename))
                        {
                            SoundPlayer simpleSound = new SoundPlayer(filename);
                            simpleSound.Play();
                        }
                        else
                        {
                            System.Media.SystemSounds.Exclamation.Play();
                        }
                    }));
            }

        }

        void _forum_OnLoginResult(string username, bool success)
        {
            if (success)
            {
                btnStartPoll.Enabled = true;
                MessageBox.Show("Logged in as " + username);
            }
            else
            {
                MessageBox.Show("Login failed. No forum tools available.");
            }
        }

        private void ShowLogin()
        {
        }




        private void SaveGame()
        {
            if ((_corral.Players.Count == 0) || (_corral.Guns.Count == 0) || (_corral.Suits.Count == 0))
            {
                MessageBox.Show("Not saving since no game is loaded.");
                return;
            }
            String roleset = JsonConvert.SerializeObject(_corral, Formatting.Indented, new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
            // Show the dialog and get result.
            using (Stream s = File.Open(_filename, FileMode.Create))
            using (StreamWriter sw = new StreamWriter(s))
            {
                sw.Write(roleset);
            }
        }

        private void saveGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveGame();
        }

        private void loadGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Show the dialog and get result.
            //DialogResult result = openFileDialog.ShowDialog();
            //if (result == DialogResult.OK) // Test result.
            {
                //string file = openFileDialog.FileName;
                try
                {
                    CorralData temp = JsonConvert.DeserializeObject<CorralData>(File.ReadAllText(_filename));
                    _corral = temp;
                    txtGameURL.Text = _corral.Url;
                    InitActionsTab();
                    lblStatus.Text = "Game Loaded";
                }
                catch (JsonSerializationException error)
                {
                    Console.WriteLine(error);
                    MessageBox.Show("This isn't valid JSON!");
                    return;
                }
            }
        }




        private void menuPlayerList_Click(object sender, EventArgs e)
        {
        }

        private void lockThreadToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }



        private void btnSelectAll_Click(object sender, EventArgs e)
        {
        }

        private void btnSubOut_Click(object sender, EventArgs e)
        {
        }

        private void btnDeathReveal_Click(object sender, EventArgs e)
        {
        }

        private void btnResend_Click(object sender, EventArgs e)
        {
        }


        private void btnPeek_Click(object sender, EventArgs e)
        {
        }



        private void dataPlayers_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
        }




        private void Meerkat_FormClosing(object sender, FormClosingEventArgs e)
        {
        }

        private void tabPage2_Click(object sender, EventArgs e)
        {
        }
        void InitActionsTab()
        {
            cmbActor.Items.Clear();
            cmbTarget.Items.Clear();
            var livePlayers = from p in _corral.Players where p.Dead == false orderby p.Name.ToLower() select p.Name;
            cmbActor.Items.AddRange(livePlayers.ToArray());
            cmbTarget.Items.AddRange(livePlayers.ToArray());
            cmbActor.SelectedIndex = 0;
            cmbTarget.SelectedIndex = 0;
        }
        void reload()
        {
            cmbActor_SelectedIndexChanged(null, EventArgs.Empty);
            cmbTarget_SelectedIndexChanged(null, EventArgs.Empty);
            SaveGame();
        }
        private void cmbActor_SelectedIndexChanged(object sender, EventArgs e)
        {
            String suit = "[no suit]";
            String armedWith = "[not armed]";
            String[] gun12 = new String[] { "[no gun]", "[no gun]", "[no gun]", "[no gun]", "[no gun]", "[no gun]" };
            Boolean tookShot = false;
            Boolean disco = false;
            Boolean silenced = false;
            Boolean doubled = false;
            String pm = "Your items:\r\n";
            String id = "NA";
            String team = "[no team]";

            if (cmbActor.SelectedIndex >= 0)
            {
                var player = (from p in _corral.Players where p.Name == cmbActor.Text select p).FirstOrDefault();
                if (player != null)
                {
                    id = player.Id.ToString();
                    tookShot = player.HasAttacked;
                    disco = player.Discombobulated;
                    silenced = player.Silenced;
                    team = player.Team;
                    if ((player.ArmedGun != null) && (player.ArmedGun.Value != 0))
                    {
                        var gun = (from g in _corral.Guns where g.Id == player.ArmedGun select g).FirstOrDefault();
                        if (gun != null)
                        {
                            if (gun.Owner != player.Id)
                            {
                                armedWith = "*** ";
                            }
                            else
                            {
                                armedWith = "";
                            }
                            armedWith += String.Format("{0}({1})", _corral.TypeToString(gun.Type), gun.Id);
                        }
                        else
                        {
                            armedWith = String.Format("*** Unknown gun {0}", player.ArmedGun);
                        }                        
                    }
                    var armor = (from s in _corral.Suits where s.Owner == player.Id select s).FirstOrDefault();
                    if (armor != null)
                    {
                        suit = String.Format("{0}({1})", _corral.TypeToString(armor.Type), armor.Id);
                        pm += _corral.TypeToString(armor.Type) + "\r\n";
                        doubled = armor.Doubled;
                    }
                    var guns = from g in _corral.Guns where g.Owner == player.Id select g;
                    if (guns != null)
                    {
                        int ix = 0;
                        foreach (var g in guns)
                        {
                            gun12[ix] = String.Format("{0}({1})", _corral.TypeToString(g.Type), g.Id);
                            pm += _corral.TypeToString(g.Type) + "\r\n";
                            ix++;
                        }
                        if (ix > 2)
                        {
                            gun12[1] = "*** " + gun12[1];
                        }
                    }
                }
            }
            txtActorArmed.Text = armedWith;
            txtActorSuit.Text = suit;
            txtActorGun1.Text = gun12[0];
            txtActorGun2.Text = gun12[1];
            chkActorShot.Checked = tookShot;
            chkActorDisco.Checked = disco;
            chkActorSilenced.Checked = silenced;
            chkActorDoubled.Checked = doubled;
            txtActorId.Text = id;
            txtActorTeam.Text = team;
        }
        Player GetActor()
        {
            if (cmbActor.SelectedIndex >= 0)
            {
                var player = (from p in _corral.Players where p.Name == cmbActor.Text select p).FirstOrDefault();
                return player;
            }
            return null;
        }
        Player GetTarget()
        {
            if (cmbTarget.SelectedIndex >= 0)
            {
                var player = (from p in _corral.Players where p.Name == cmbTarget.Text select p).FirstOrDefault();
                return player;
            }
            return null;
        }
        private void cmbTarget_SelectedIndexChanged(object sender, EventArgs e)
        {
            String suit = "[no suit]";
            String armedWith = "[not armed]";
            String[] gun12 = new String[] { "[no gun]", "[no gun]", "[no gun]" };
            Boolean tookShot = false;
            Boolean disco = false;
            Boolean silenced = false;
            Boolean doubled = false;
            String id = "NA";
            String team = "[no team]";

            var player = GetTarget();
            if (player != null)
            {
                team = player.Team;
                tookShot = player.HasAttacked;
                disco = player.Discombobulated;
                silenced = player.Silenced;
                id = player.Id.ToString();
                if ((player.ArmedGun != null) && (player.ArmedGun.Value != 0))
                {
                    var gun = (from g in _corral.Guns where g.Id == player.ArmedGun select g).FirstOrDefault();
                    if (gun != null)
                    {
                        if (gun.Owner != player.Id)
                        {
                            armedWith = "*** ";
                        }
                        else
                        {
                            armedWith = "";
                        }
                        armedWith += String.Format("{0}({1})", _corral.TypeToString(gun.Type), gun.Id);
                    }
                    else
                    {
                        armedWith = String.Format("*** Unknown gun {0}", player.ArmedGun);
                    }
                }
                var armor = (from s in _corral.Suits where s.Owner == player.Id select s).FirstOrDefault();
                if (armor != null)
                {
                    suit = String.Format("{0}({1})", _corral.TypeToString(armor.Type), armor.Id);
                    doubled = armor.Doubled;
                }
                var guns = from g in _corral.Guns where g.Owner == player.Id select g;
                if (guns != null)
                {
                    int ix = 0;
                    foreach (var g in guns)
                    {
                        gun12[ix] = String.Format("{0}({1})", _corral.TypeToString(g.Type), g.Id);
                        ix++;
                    }
                    if (ix > 2)
                    {
                        gun12[1] = "*** " + gun12[1];
                    }
                }
            }
            txtTargetArmed.Text = armedWith;
            txtTargetSuit.Text = suit;
            txtTargetGun1.Text = gun12[0];
            txtTargetGun2.Text = gun12[1];
            chkTargetDoubled.Checked = doubled;
            txtTeam.Text = team;

            chkTargetShot.Checked = tookShot;
            chkTargetDisco.Checked = disco;
            chkTargetSilenced.Checked = silenced;
            txtTargetId.Text = id;
        }
        void SaveChanges(String msg)
        {
            txtResults.Text = msg;
            reload();
            
        }
        T RandomItemFromList<T>(IEnumerable<T> seq)
        {
            List<T> list = seq.ToList();
            Int32 ix = _random.Next(list.Count);
            return list[ix];
        }
        private void btnRandSuit_Click(object sender, EventArgs e)
        {
            txtResults.Text = "rand suit failed";
            var player = GetActor();
            if (player != null)
            {
                var pilesuits = (from s in _corral.Suits where s.Owner == 0 select s);
                var newsuit = RandomItemFromList(pilesuits);

                var oldarmor = (from s in _corral.Suits where s.Owner == player.Id select s).FirstOrDefault();
                Int32 oldSuit = 0;
                if (oldarmor != null)
                {
                    oldSuit = oldarmor.Id;
                    oldarmor.Owner = 0;
                }
                String msg = String.Format("{0} gets suit {1} from the pile to replace {2}", player.Name, newsuit.Id, oldSuit);
                newsuit.Owner = player.Id;
                SaveChanges(msg);
            }
            reload();
        }

        private void btnStealSuit_Click(object sender, EventArgs e)
        {
            txtResults.Text = "Steal failed";
            var player = GetActor();
            var target = GetTarget();
            if ((player != null) && (target != null))
            {

                var newarmor = (from s in _corral.Suits where s.Owner == target.Id select s).FirstOrDefault();

                var pilesuits = (from s in _corral.Suits where s.Owner == 0 select s);
                var pilesuit = RandomItemFromList(pilesuits);

                var oldarmor = (from s in _corral.Suits where s.Owner == player.Id select s).FirstOrDefault();
                Int32 oldSuit = 0;
                if (oldarmor != null)
                {
                    oldSuit = oldarmor.Id;
                    oldarmor.Owner = 0;
                }

                String msg = String.Format("{0} steals suit {1} to replace {2}. {3} gets {4} from the pile", player.Name, newarmor.Id, 
                    oldSuit, target.Name, pilesuit.Id);

                newarmor.Owner = player.Id;
                pilesuit.Owner = target.Id;
                
                SaveChanges(msg);
            }
            reload();

        }

        private void btnStealGun_Click(object sender, EventArgs e)
        {
            txtResults.Text = "Steal failed";
            var player = GetActor();
            var target = GetTarget();
            if ((player != null) && (target != null))
            {
                var pileguns = (from s in _corral.Guns where s.Owner == 0 select s);
                var pilegun = RandomItemFromList(pileguns);

                Gun targetGun = null;
                if ((target.ArmedGun != null) && (target.ArmedGun > 0))
                {
                    Gun armedGun = (from g in _corral.Guns where g.Id == target.ArmedGun.Value select g).FirstOrDefault();
                    if (armedGun.Owner == target.Id)
                    {
                        targetGun = armedGun;
                    }
                }
                if(targetGun == null)
                {
                    var guns = from g in _corral.Guns where g.Owner == target.Id select g;
                    Gun randGun = RandomItemFromList(guns);
                    targetGun = randGun;
                }
                Gun saveGun = null;
                if ((player.ArmedGun != null) && (player.ArmedGun > 0))
                {
                    Gun armedGun = (from g in _corral.Guns where g.Id == player.ArmedGun.Value select g).FirstOrDefault();
                    if (armedGun.Owner == player.Id)
                    {
                        saveGun = armedGun;
                    }
                }
                if (saveGun == null)
                {
                    var guns = from g in _corral.Guns where g.Owner == player.Id select g;
                    Gun randGun = RandomItemFromList(guns);
                    saveGun = randGun;
                }
                var dropGuns = from g in _corral.Guns where (g.Owner == player.Id) && (g != saveGun) select g;

                String msg = String.Format("{0} steals gun {1} from {2} who gets {3} from pile. {0} returns to pile: ", player.Name, targetGun.Id,
                    target.Name, pilegun.Id);
                foreach (var d in dropGuns)
                {
                    msg += d.Id.ToString() + " ";
                    d.Owner = 0;
                }
                pilegun.Owner = target.Id;
                targetGun.Owner = player.Id;

                SaveChanges(msg);
            }
            reload();

        }

        private void btnRandGun_Click(object sender, EventArgs e)
        {
            var player = GetActor();
            if (player != null)
            {
                var pileguns = (from s in _corral.Guns where s.Owner == 0 select s);
                var pilegun = RandomItemFromList(pileguns);

                Gun saveGun = null;
                if ((player.ArmedGun != null) && (player.ArmedGun > 0))
                {
                    Gun armedGun = (from g in _corral.Guns where g.Id == player.ArmedGun.Value select g).FirstOrDefault();
                    if (armedGun.Owner == player.Id)
                    {
                        saveGun = armedGun;
                    }
                }
                if (saveGun == null)
                {
                    var guns = from g in _corral.Guns where g.Owner == player.Id select g;
                    Gun randGun = RandomItemFromList(guns);
                    saveGun = randGun;
                }
                var dropGuns = from g in _corral.Guns where (g.Owner == player.Id) && (g != saveGun) select g;

                String msg = String.Format("{0} rands gun {1} from pile. {0} returns to pile: ", player.Name, pilegun.Id);
                foreach (var d in dropGuns)
                {
                    msg += d.Id.ToString() + " ";
                    d.Owner = 0;
                }

                pilegun.Owner = player.Id;
                SaveChanges(msg);
            }
            reload();

        }

        private void btnSwap_Click(object sender, EventArgs e)
        {
            txtResults.Text = "swap failed";
            var player = GetActor();
            var target = GetTarget();
            if ((player != null) && (target != null))
            {
                if ((player.Team != "Wolf") || (target.Team != "Wolf"))
                {
                    txtResults.Text = "only wolves can swap.";
                    return;
                }
                Int32 oldid = 0;
                Int32 newid = 0;
                var oldarmor = (from s in _corral.Suits where s.Owner == player.Id select s).FirstOrDefault();
                var newarmor = (from s in _corral.Suits where s.Owner == target.Id select s).FirstOrDefault();
                if (oldarmor == null)
                {
                    txtResults.Text = player.Name + " is naked. Swapping not allowed.";
                    return;
                }                
                if (newarmor == null)
                {
                    txtResults.Text = target.Name + " is naked. Swapping not allowed.";
                    return;
                }
                oldarmor.Owner = target.Id;
                oldid = oldarmor.Id;
                newarmor.Owner = player.Id;
                newid = newarmor.Id;
                
                String msg = String.Format("{0} swapped suit {1} for suit {2} from {3}.", player.Name, oldid, newid, target.Name);
                SaveChanges(msg);
            }
            reload();

        }

        private void btnShowPile_Click(object sender, EventArgs e)
        {
            var gunCounts = _corral.Guns.Where(a => a.Owner == 0).GroupBy(g => g.Type).Select(x => new { x.Key, Count = x.Count() });
            String msg= "[b]Guns in the Pile[/b]\r\n";
            foreach (var gc in gunCounts)
            {
                msg += _corral.TypeToString(gc.Key) + ": " + gc.Count.ToString() + "\r\n";
            }
            var suitCounts = _corral.Suits.Where(a => (a.Owner == 0) && (a.Doubled == false)).GroupBy(g => g.Type).Select(x => new { x.Key, Count = x.Count() });
            msg += "\r\n[b]Suits in the Pile[/b]\r\n";
            foreach (var gc in suitCounts)
            {
                msg += _corral.TypeToString(gc.Key) + ": " + gc.Count.ToString() + "\r\n";
            }
            var doubledCounts = _corral.Suits.Where(a => (a.Owner == 0) && (a.Doubled == true)).GroupBy(g => g.Type).Select(x => new { x.Key, Count = x.Count() });
            foreach (var gc in doubledCounts)
            {
                msg += "DOUBLED " + _corral.TypeToString(gc.Key) + ": " + gc.Count.ToString() + "\r\n";
            }

            txtResults.Text = msg;
        }

        private void btnShowItems_Click(object sender, EventArgs e)
        {
            String msg = "";
            var wolves = from p in _corral.Players where (p.Dead == false) orderby p.Name select p;
            foreach (var wolf in wolves)
            {
                String line = wolf.Name + "\t";
                var suit = (from s in _corral.Suits where s.Owner == wolf.Id select s).FirstOrDefault();
                String description = "no suit";
                if (suit != null)
                {
                    description = _corral.TypeToString(suit.Type);
                    if (suit.Doubled) description += "(dbl)";
                }
                line += description + "\t" + wolf.ArmedGun.Value + "\t";
                var guns = from g in _corral.Guns where g.Owner == wolf.Id select g;
                foreach (var g in guns)
                {
                    String gd = String.Format("{0}({1})\t", _corral.TypeToString(g.Type), g.Id);
                    line += gd;
                }
                line += "Shot:" + wolf.HasAttacked.ToString() + "\r\n";
                msg += line;
            }
            txtResults.Text = msg;
        }

        private void btnClearShots_Click(object sender, EventArgs e)
        {
            var players = _corral.Players.Where(p => p.Dead == false);
            foreach (var player in players)
            {
                player.HasAttacked = false;
                player.Silenced = false;
                player.Discombobulated = false;
                
            }
            reload();

        }

        private void btnArmGun1_Click(object sender, EventArgs e)
        {
            var player = GetActor();
            Match m = Regex.Match(txtActorGun1.Text, @"\((\d+)\)");
            if(m.Success)
            {
                player.ArmedGun = int.Parse(m.Groups[1].Value);
            }
            reload();
        }

        private void btnArmGun2_Click(object sender, EventArgs e)
        {
            var player = GetActor();
            Match m = Regex.Match(txtActorGun2.Text, @"\((\d+)\)");
            if (m.Success)
            {
                player.ArmedGun = int.Parse(m.Groups[1].Value);
            }
            reload();
        }

        private void btnKillTarget_Click(object sender, EventArgs e)
        {
            var player = GetTarget();
            if (player != null)
            {
                player.Dead = true;
                InitActionsTab();
                String msg = player.Name + " is dead now.";
                SaveChanges(msg);
            }
        }
        private void chkActorDoubled_CheckedChanged(object sender, EventArgs e)
        {
            var player = GetActor();
            if (player != null)
            {
                var suit = (from s in _corral.Suits where s.Owner == player.Id select s).FirstOrDefault();
                if (suit != null)
                {
                    suit.Doubled = chkActorDoubled.Checked;
                }
                reload();
            }
        }

        private void chkTargetDoubled_CheckedChanged(object sender, EventArgs e)
        {
            var target = GetTarget();
            if (target != null)
            {
                var suit = (from s in _corral.Suits where s.Owner == target.Id select s).FirstOrDefault();
                if (suit != null)
                {
                    suit.Doubled = chkTargetDoubled.Checked;
                }
                reload();
            }

        }

        private void chkActorShot_CheckedChanged(object sender, EventArgs e)
        {
            var player = GetActor();
            if (player != null)
            {
                player.HasAttacked = chkActorShot.Checked;
            }
            reload();
        }
        private void chkTargetShot_CheckedChanged(object sender, EventArgs e)
        {
            var player = GetTarget();
            if (player != null)
            {
                player.HasAttacked = chkTargetShot.Checked;
            }
            reload();

        }



        private void chkDisco_CheckedChanged(object sender, EventArgs e)
        {
            var player = GetActor();
            if (player != null)
            {
                player.Discombobulated = chkActorDisco.Checked;
            }
            reload();
        }
        private void chkTargetDisco_CheckedChanged(object sender, EventArgs e)
        {
            var player = GetTarget();
            if (player != null)
            {
                player.Discombobulated = chkTargetDisco.Checked;
            }
            reload();
        }


        private void btnWolfSuits_Click(object sender, EventArgs e)
        {
            String msg= "";
            var wolves = from p in _corral.Players where (p.Dead == false) && (p.Team == "Wolf") orderby p.Name select p; 
            foreach(var wolf in wolves)
            {
                var suit = (from s in _corral.Suits where s.Owner == wolf.Id select s).FirstOrDefault();
                String description = "no suit";
                if(suit != null)
                {
                    if (suit.Doubled)
                    {
                        description = "DOUBLED ";
                    }
                    else
                    {
                        description = "";
                    }
                    description += _corral.TypeToString(suit.Type);
                    msg+= String.Format("{0}: {1}\r\n", wolf.Name, description);
                }
                
            }
            txtResults.Text = msg;
        }

        private void txtResults_TextChanged(object sender, EventArgs e)
        {
            txtResults.SelectAll();
        }

        private void txtGunNumber_TextChanged(object sender, EventArgs e)
        {
            String type = "[no gun]";
            String owner = "";
            String ownerName = "[no owner]";
            Int32 id = -2;
            Boolean enableChangeOwner = false;
            if (Int32.TryParse(txtGunNumber.Text, out id))
            {
                if ((id <= _corral.Guns.Count) && (id >= 1))
                {
                    enableChangeOwner = true;
                    Gun gun = (from g in _corral.Guns where g.Id == id select g).First();
                    type = _corral.TypeToString(gun.Type);
                    Int32 ownerId = gun.Owner;
                    owner = ownerId.ToString();
                    switch (ownerId)
                    {
                        case -1:
                            ownerName = "destroyed";
                            break;
                        case 0:
                            ownerName = "pile";
                            break;
                        default:
                            ownerName = (from p in _corral.Players where p.Id == ownerId select p.Name).First();
                            break;
                    }
                }
            }
            btnChangeGunOwner.Enabled = enableChangeOwner;
            txtGunOwner.Text = owner;
            txtGunType.Text = type;
            txtGunOwnerName.Text = ownerName;
        }

        private void btnChangeGunOwner_Click(object sender, EventArgs e)
        {
            Int32 newOwner = -2;
            if (!Int32.TryParse(txtGunOwner.Text, out newOwner))
            {
                return;
            }
            Int32 id = -2;
            if (Int32.TryParse(txtGunNumber.Text, out id))
            {
                if ((id <= _corral.Guns.Count) && (id >= 1))
                {
                    Gun gun = (from g in _corral.Guns where g.Id == id select g).First();
                    Int32 oldOwner = gun.Owner;
                    gun.Owner = newOwner;
                    String msg = String.Format("Gun {0} changed owner from {1} to {2}", id, oldOwner, newOwner);
                    SaveChanges(msg);
                    txtGunNumber_TextChanged(null, EventArgs.Empty);
                }
            }

        }

        private void txtSuitNumber_TextChanged(object sender, EventArgs e)
        {
            String type = "[no suit]";
            String owner = "";
            String ownerName = "[no owner]";
            Int32 id = -2;
            Boolean enableChangeOwner = false;
            if (Int32.TryParse(txtSuitNumber.Text, out id))
            {
                if ((id <= _corral.Suits.Count) && (id >= 1))
                {
                    enableChangeOwner = true;
                    Suit suit = (from s in _corral.Suits where s.Id == id select s).First();
                    if (suit.Doubled) 
                        type = "DBL:"; 
                    else 
                        type = "";
                    type += _corral.TypeToString(suit.Type);
                    Int32 ownerId = suit.Owner;
                    owner = ownerId.ToString();
                    switch (ownerId)
                    {
                        case -1:
                            ownerName = "destroyed";
                            break;
                        case 0:
                            ownerName = "pile";
                            break;
                        default:
                            ownerName = (from p in _corral.Players where p.Id == ownerId select p.Name).First();
                            break;
                    }
                }
            }
            btnChangeSuitOwner.Enabled = enableChangeOwner;
            txtSuitOwner.Text = owner;
            txtSuitType.Text = type;
            txtSuitOwnerName.Text = ownerName;

        }

        private void btnChangeSuitOwner_Click(object sender, EventArgs e)
        {
            Int32 newOwner = -2;
            if (!Int32.TryParse(txtSuitOwner.Text, out newOwner))
            {
                return;
            }
            Int32 id = -2;
            if (Int32.TryParse(txtSuitNumber.Text, out id))
            {
                if ((id <= _corral.Suits.Count) && (id >= 1))
                {
                    Suit suit = (from g in _corral.Suits where g.Id == id select g).First();
                    Int32 oldOwner = suit.Owner;
                    suit.Owner = newOwner;
                    String msg = String.Format("Suit {0} changed owner from {1} to {2}", id, oldOwner, newOwner);
                    SaveChanges(msg);
                    txtSuitNumber_TextChanged(null, EventArgs.Empty);
                }
            }


        }

        private void chkActorSilenced_CheckedChanged(object sender, EventArgs e)
        {
            var player = GetActor();
            if (player != null)
            {
                player.Silenced = chkActorSilenced.Checked;
                reload();
            }
        }

        private void chkTargetSilenced_CheckedChanged(object sender, EventArgs e)
        {
            var player = GetTarget();
            if (player != null)
            {
                player.Silenced = chkTargetSilenced.Checked;
                reload();
            }
        }
        Boolean SendPM(string to, string title, string content)
        {
            if (_forum != null)
            {
                _forum.SendPM(to, title, content);
                return true;
            }
            return false;
        }
        private void btnPMTarget_Click(object sender, EventArgs e)
        {
            String msg = "[no pm sent.]";
            btnPMTarget.Enabled = false;
            var player = GetTarget();
            if (player != null)
            {
                String pm = MakeStatusPM(player);
                msg = "pm sent to " + player.Name + "\r\n" + pm;
                SendPM(player.Name, "Pog Corral: your items", pm);
            }
            txtResults.Text = msg;
            btnPMTarget.Enabled = true;
        }
        String  MakeStatusPM(Player player)
        {
            String pm = "Player: " + player.Name + "\r\n\r\nSuit: ";
            var armor = (from s in _corral.Suits where s.Owner == player.Id select s).FirstOrDefault();
            if (armor != null)
            {
                if(armor.Doubled)
                {
                    pm += "DOUBLED ";
                }
                pm += _corral.TypeToString(armor.Type) + "\r\n";
            }
            pm+= "\r\n";
            var guns = from g in _corral.Guns where g.Owner == player.Id select g;
            if (guns != null)
            {
                int ix = 0;
                foreach (var g in guns)
                {
                    pm += "Gun: " + _corral.TypeToString(g.Type);
                    if (g.Id == player.ArmedGun)
                    {
                        pm += "(ARMED)";
                    }
                    pm += "\r\n";
                    ix++;
                }
            }
            return pm;
        }

        private void btnPMAll_Click(object sender, EventArgs e)
        {
            txtResults.Text = "Sending pms...\r\n";
            btnPMAll.Enabled = false;
            var players = from p in _corral.Players where (p.Dead == false) orderby p.Name.ToLower() select p;
            foreach(var player in players)
            {
                String pm = MakeStatusPM(player);
                txtResults.Text += "pm sent to " + player.Name + "\r\n";
                SendPM(player.Name, "Pog Corral: your items", pm);
            }
            lblStatus.Text = "Done sending pms to all";
            btnPMAll.Enabled = true;
        }

        private void btnCopyResult_Click(object sender, EventArgs e)
        {
            Clipboard.SetData(DataFormats.StringFormat, txtResults.Text);
            lblStatus.Text = "copied results to clipboard.";
        }

        private void btnStartPoll_Click(object sender, EventArgs e)
        {
            lbAlerts.Items.Clear();
            btnStopPoll.Enabled = true;
            btnStartPoll.Enabled = false;
            Int32 startPost = 1;
            Int32.TryParse(txtStartPost.Text, out startPost);
            _forum.StartPolling(startPost, null, null);
        }

        private void btnStopPoll_Click(object sender, EventArgs e)
        {
            btnStartPoll.Enabled = true;
            btnStopPoll.Enabled = false;
        }

        private void btnCanShoot_Click(object sender, EventArgs e)
        {
            String list = "Shots Remaining:\r\n";
            var players = from p in _corral.Players
                          where (p.Dead == false) &&
                              (p.Discombobulated == false) && (p.HasAttacked == false) && (p.Silenced == false)
                          orderby p.Name.ToLower()
                          select p.Name;
            foreach (var player in players)
            {
                list += player + "\r\n";
            }
            txtResults.Text = list;
        }

        private void btnDiscoList_Click(object sender, EventArgs e)
        {
            String list = "Discombobulated:\r\n";
            var players = from p in _corral.Players
                          where (p.Dead == false) &&
                              (p.Discombobulated == true)
                          orderby p.Name.ToLower()
                          select p.Name;
            foreach (var player in players)
            {
                list += player + "\r\n";
            }
            txtResults.Text = list;

        }

        private void btnShowTarget_Click(object sender, EventArgs e)
        {
            String msg = "[no info]";
            var player = GetTarget();
            if (player != null)
            {
                msg = MakeStatusPM(player);
            }
            txtResults.Text = msg;
        }

        private void btnRandomPlayer_Click(object sender, EventArgs e)
        {
            var livePlayers = from p in _corral.Players where p.Dead == false select p.Name;
            var player = RandomItemFromList(livePlayers);
            txtRandomPlayer.Text = player;
        }

        private void btnExceptions_Click(object sender, EventArgs e)
        {
            // look for players without 2 guns & 1 suit.
            String msg = "";
            var players = from p in _corral.Players where (p.Dead == false) select p;
            foreach (var player in players)
            {
                var suits = from s in _corral.Suits where (s.Owner == player.Id) select s;
                var guns = from g in _corral.Guns where (g.Owner == player.Id) select g;
                Int32 sc = suits.Count();
                Int32 gc = guns.Count();
                if ((sc != 1) || (gc != 2))
                {
                    msg += String.Format("{0} has {1} guns and {2} suits.\r\n", player.Name, gc, sc);
                }
            }
            if (msg == "")
            {
                msg = "everybody has 2 guns and 1 suit.";
            }
            txtResults.Text = msg;
        }

        private void btnUnArm_Click(object sender, EventArgs e)
        {
            Player p = GetActor();
            if (p != null)
            {
                p.ArmedGun = 0;
                reload();
            }
        }




    }
}
