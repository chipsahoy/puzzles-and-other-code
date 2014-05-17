using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using POG.Werewolf;
using POG.Forum;
using POG.Utils;

namespace WerewolfTest
{
    
    [TestFixture]
    public class TestNameCorrection
    {
        ElectionInfo _count;

        [TestFixtureSetUp]
        public void Setup()
        {
            String url = "forumserver.twoplustwo.com";
            String vbVersion = "3.8.7";
            String lobby = "59/puzzles-other-games/";
            POG.Forum.Language _language = Language.English;
            Action<Action> invoker = (x) => x();
            String dbPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\pog\";
			System.IO.Directory.CreateDirectory(dbPath);
			String dbName = String.Format("{0}posts.{1}.sqlite", dbPath, url);
			PogSqlite _db = new PogSqlite();
			_db.Connect(dbName);
			
			var _forum = new VBulletinForum(invoker, url, vbVersion, Language.English, lobby, "", "");
            var reader = _forum.Reader();
            _count = new ElectionInfo(invoker, reader, _db, _forum.ForumURL, url, _forum.PostsPerPage, _language, "3.8.7");
        }
        [TestFixtureTearDown]
        public void Teardown()
        {
        }
        [Test]
        public void TestInitials()
        {
            String input = "Chips";
            List<ElectionInfo.Alias> choices = new List<ElectionInfo.Alias>();
            choices.Add(new ElectionInfo.Alias("Chips Ahoy", "Chips Ahoy"));
            choices.Add(new ElectionInfo.Alias("Top Tier Tom", "Top Tier Tom"));
            choices.Add(new ElectionInfo.Alias("stuckinarutt", "stuckinarutt"));
            String answer = _count.ParseInputToChoice(input, choices);
            Assert.AreEqual("Chips Ahoy", answer);
            input = "ttt";
            answer = _count.ParseInputToChoice(input, choices);
            Assert.AreEqual("Top Tier Tom", answer);
            input = "rrr";
            answer = _count.ParseInputToChoice(input, choices);
            Assert.AreEqual("", answer);
        }
        [Test]
        public void TestSmallInput()
        {
            String input = ".";
            List<ElectionInfo.Alias> choices = new List<ElectionInfo.Alias>();
            choices.Add(new ElectionInfo.Alias("Chips Ahoy", "Chips Ahoy"));
            String answer = _count.ParseInputToChoice(input, choices);
            Assert.AreEqual("", answer);
        }
    }
}
