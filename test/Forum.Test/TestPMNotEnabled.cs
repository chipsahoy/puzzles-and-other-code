using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using POG.Forum;

namespace Forum.Test.NoPM
{
    [SetUpFixture]
    class SetupTwoPlusTwo
    {
        static VBulletinForum _forum;
        public SetupTwoPlusTwo()
        {
        }

        [SetUp]
        public void Setup()
        {
            Action<Action> synchronousInvoker = a => a();
            _forum = new POG.Forum.VBulletinForum(synchronousInvoker,
                "forumserver.twoplustwo.com", "3.8.7", "59/puzzles-other-games/");
            _forum.LoginEvent += new EventHandler<LoginEventArgs>(_forum_LoginEvent);
            _forum.Login("ICantSendPMs", "fzUH7O4fI0kU26Fmsq4c");
            for (int i = 0; i < 100; i++)
            {
                if (LoggedIn != null)
                {
                    break;
                }
                System.Threading.Thread.Sleep(100);
            }
        }
        void _forum_LoginEvent(object sender, LoginEventArgs e)
        {
            switch (e.LoginEventType)
            {
                case LoginEventType.LoginFailure:
                    {
                        LoggedIn = false;
                    }
                    break;

                case LoginEventType.LoginSuccess:
                    {
                        LoggedIn = true;
                    }
                    break;
            }
        }
        public static bool? LoggedIn
        {
            get;
            private set;
        }
        public static VBulletinForum Forum
        {
            get
            {
                return _forum;
            }
        }
    }
    [TestFixture]
    public class TestPMNotEnabled
    {
        [Test]
        public void Test00Login()
        {
            Assert.NotNull(SetupTwoPlusTwo.LoggedIn);
            Assert.AreEqual(SetupTwoPlusTwo.LoggedIn.Value, true);
        }
        [Test]
        public void Test01PMFail()
        {
            PrivateMessage pm = new PrivateMessage(
                new List<String> { "Fennec Fox" },
                null,
                "Test PM ",
                "This is a test.");
            bool rc = SetupTwoPlusTwo.Forum.SendPM(pm,
                (PrivateMessage pmSent, PrivateMessageError err, String errDetails, object value, object cookie) =>
                {
                    Assert.AreEqual(PrivateMessageError.PMNotAllowed, err);
                }
            );
            Assert.IsFalse(rc);
        }
    }
}
