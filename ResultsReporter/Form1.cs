using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using POG.Forum;
using POG.Werewolf;
using POG.Utils;
using Newtonsoft.Json;

namespace ResultsReporter
{
    public partial class Form1 : Form
    {
        #region fields
        private POG.Werewolf.IPogDb _db;
        System.Timers.Timer _timer = new System.Timers.Timer();
        System.Timers.Timer _timerPM = new System.Timers.Timer();
        DateTimeOffset _EarliestPMTime = DateTimeOffset.Now;
        DateTimeOffset _maxTime = DateTimeOffset.MaxValue;
        List<String> _validNames = new List<string>();
        List<String> _canPM = new List<string>();
        List<String> _PMSent = new List<string>();
        ElectionInfo _count;

        private POG.Werewolf.AutoComplete _autoComplete;
        VBulletinForum _forum;
        Action<Action> _synchronousInvoker;
        StateMachineHost _host = new StateMachineHost("turbo");
        #endregion
        public Form1()
        {
            InitializeComponent();
            String host = "forumserver.twoplustwo.com";
            _synchronousInvoker = a => Invoke(a);
            _forum = new VBulletinForum(_synchronousInvoker, host, "3.8.7", "59/puzzles-other-games/");
            _forum.LoginEvent += new EventHandler<LoginEventArgs>(_forum_LoginEvent);

            string username = PogSettings.Read("TatianaModName");
            string password = PogSettings.Read("TatianaModPassword");

            String dbPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "/pog/";
            System.IO.Directory.CreateDirectory(dbPath);
            String dbName = String.Format("{0}posts.{1}.sqlite", dbPath, host);
            _db = new PogSqlite();
            _db.Connect(dbName);

            Action<Action> invoker = a => a();
            _autoComplete = new AutoComplete(_forum, invoker, _db);
            _forum.Login(username, password);
        }
        void ReadPost(Post post, String type)
        {
            var html = new HtmlAgilityPack.HtmlDocument();
            html.LoadHtml(post.Content);
            HtmlAgilityPack.HtmlNode root = html.DocumentNode;
            Console.WriteLine("title: {0}", post.Title);
            switch (type)
            {
                case "OP":
                    {
                        var nodes = root.SelectNodes("/div/u/following-sibling::text()");
                        if (nodes.Count < 9) return;
                        for (int i = 0; i < 9; i++)
                        {
                            String player = nodes[i].InnerText.Trim();
                            Console.WriteLine("Player: {0}", player);
                        }
                    }
                    break;

                case "Lynch":
                    {
                    }
                    break;

                case "Day":
                    {
                    }
                    break;

                case "End":
                    {
                    }
                    break;

                case "VoteCount":
                    {
                    }
                    break;


            }
        }
        void ReadTurbo(String url)
        {
            Int32 _threadId = Misc.TidFromURL(url);
            ThreadReader t = _forum.Reader();
            Action<Action> invoker = a => a();
            _count = new ElectionInfo(invoker, t, _db, _forum.ForumURL,
                url,
                _forum.PostsPerPage, Language.English);
            _count.CheckThread(() =>
            {
                Console.WriteLine("{0} posts", _count.LastPost);
                Post p = _db.GetPost(_threadId, 1);
                ReadPost(p, "OP");
                Console.WriteLine("{0} writes... {1}", p.Poster.Name, p.Title);
                var posts = _db.GetPosts(_threadId, p.Poster.Name);
                Console.WriteLine("{0} has {1} posts.", p.Poster.Name, posts.Count());
                foreach (Post post in posts)
                {
                    switch (post.Title)
                    {
                        case "Mod: Lynch result":
                            {
                                ReadPost(post, "Lynch");
                            }
                            break;

                        case "Mod: It is day!":
                            {
                                ReadPost(post, "Day");
                            }
                            break;

                        case "Mod: Game Over":
                            {
                                ReadPost(post, "End");
                            }
                            break;

                        case "Vote Count":
                            {
                                ReadPost(post, "VoteCount");
                            }
                            break;

                        default:
                            {
                            }
                            break;


                    }
                }
            });
        }
        private void button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            string url = @"http://forumserver.twoplustwo.com/59/puzzles-other-games/s-l-1426567/";
            ReadTurbo(url);
            button1.Enabled = true;
        }
        void _forum_LoginEvent(object sender, LoginEventArgs e)
        {
            switch (e.LoginEventType)
            {
                case LoginEventType.LoginSuccess:
                    {
                        button1.Enabled = true;
                    }
                    break;

                case LoginEventType.LoginFailure:
                    {
                        MessageBox.Show("Login Failure as " + e.Username);
                    }
                    break;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var game = new WerewolfGameInstance();
            game.Moderator = "Chips Ahoy";
            game.RandedRoles.value.test = "This is a SECRET bit of data.";
            String json = JsonConvert.SerializeObject(game, Formatting.Indented);
            Console.WriteLine("serialize no password: {0}", json);
            game = JsonConvert.DeserializeObject<WerewolfGameInstance>(json);
            bool ok = game.RandedRoles.SetPassword(true, "password1234");
            json = JsonConvert.SerializeObject(game, Formatting.Indented);
            Console.WriteLine("serialize with password: {0}", json);
            game = JsonConvert.DeserializeObject<WerewolfGameInstance>(json);
            Console.WriteLine("Deserialize encrypted: {0}", game.RandedRoles.value != null);
            ok = game.RandedRoles.SetPassword(true, "wrongpassword");
            Console.WriteLine("after wrong password: {0}", game.RandedRoles.value != null);
            ok = game.RandedRoles.SetPassword(true, "password1234");
            Console.WriteLine("after right password: {0}", game.RandedRoles.value != null);
            ok = game.RandedRoles.SetPassword(false);
            json = JsonConvert.SerializeObject(game, Formatting.Indented);
            Console.WriteLine("Password off: {0}", json);
            
        }
    }
    class GameState
    {
        public GameState AddPlayer(Player p)
        {
            GameState rc = null;
            return rc;
        }
        public GameState SubPlayer(Player p, String sub)
        {
            GameState rc = null;
            return rc;
        }
    }
}
