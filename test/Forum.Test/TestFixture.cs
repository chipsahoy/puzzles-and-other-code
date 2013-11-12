using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using POG.Forum;

namespace Forum.Test
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
            _forum.Login("Fennec Fox", "qFLBU2HDxKLYjYeSmv5W");
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
    public class TestFixture1
    {
        [Test]
        public void Test00Login()
        {
            Assert.NotNull(SetupTwoPlusTwo.LoggedIn);
            Assert.AreEqual(SetupTwoPlusTwo.LoggedIn.Value, true);
        }
        [Test]
        public void Test00ReadInbox()
        {
            bool rc = SetupTwoPlusTwo.Forum.CheckPMs(0, 1, null, (page, errMessage, cookie) =>
                {
                    Console.WriteLine("{0} {1} {2}", page.TotalMessages, page.UnreadCount, page.Name);
                    for (int i = 0; i < page.MessagesThisPage; i++)
                    {
                        PMHeader header = page[i];
                        Console.WriteLine("{0}: {1}", header.Sender, header.FirstLine);
                        SetupTwoPlusTwo.Forum.ReadPM(header.Id, null, (id, pm, cookie2) =>
                            {
                            }
                        );
                    }
                }
            );
        }
        [Test]
        public void Test01PMInvalidTo()
        {
            PrivateMessage pm = new PrivateMessage(
                new List<String> { "Chipped Ahoy" },
                null,
                "Test PM",
                "This is a test.");
            bool rc = SetupTwoPlusTwo.Forum.SendPM(pm,
                (PrivateMessage pmSent, PrivateMessageError err, String errDetails, object value, object cookie) => 
                {
                    Assert.AreEqual(PrivateMessageError.PMUnknownRecepient, err);
                    Assert.AreEqual("Chipped Ahoy", value as String);
                }
            );
            Assert.IsFalse(rc);
        }
        //[Test]
        //public void Test02PMOnIgnore()
        //{
        //    PrivateMessage pm = new PrivateMessage(
        //        new List<String> { "magicbobonerdfan" },
        //        null,
        //        "Test PM",
        //        "This is a test.");
        //    bool rc = SetupTwoPlusTwo.Forum.SendPM(pm,
        //        (PrivateMessage pmSent, PrivateMessageError err, String errDetails, object cookie) =>
        //        {
        //            Assert.AreEqual(PrivateMessageError.PMRecepientFullorIgnore, err);
        //        }
        //    );
        //    Assert.IsFalse(rc);
        //}
        [Test]
        public void Test03PMNewUser()
        {
            String to = "ICantSendPMs";
            PrivateMessage pm = new PrivateMessage(
                new List<String> { to },
                null,
                "Test PM",
                "This is a test.");
            bool rc = SetupTwoPlusTwo.Forum.SendPM(pm,
                (PrivateMessage pmSent, PrivateMessageError err, String errDetails, object value, object cookie) =>
                {
                    Assert.AreEqual(PrivateMessageError.PMRecepientNotAllowed, err);
                    Assert.AreEqual(to, value as String);
                }
            );
            Assert.IsFalse(rc);
        }
        [Test]
        public void Test04NoTitle()
        {
            PrivateMessage pm = new PrivateMessage(
                new List<String> { "Fennec Fox" },
                null,
                null,
                "This is a test.");
            bool rc = SetupTwoPlusTwo.Forum.SendPM(pm,
                (PrivateMessage pmSent, PrivateMessageError err, String errDetails, object value, object cookie) =>
                {
                    Assert.AreEqual(PrivateMessageError.PMNoTitle, err);
                }
            );
            Assert.IsFalse(rc);
        }
        [Test]
        public void Test05NoBody()
        {
            PrivateMessage pm = new PrivateMessage(
                new List<String> { "Fennec Fox" },
                null,
                "Test PM",
                null);
            bool rc = SetupTwoPlusTwo.Forum.SendPM(pm,
                (PrivateMessage pmSent, PrivateMessageError err, String errDetails, object value, object cookie) =>
                {
                    Assert.AreEqual(PrivateMessageError.PMNoBody, err);
                }
            );
            Assert.IsFalse(rc);
        }
        [Test]
        public void Test06TooLong()
        {
            PrivateMessage pm = new PrivateMessage(
                new List<String> { "Fennec Fox" },
                null,
                "Test PM",
                new String('x', 10001));
            bool rc = SetupTwoPlusTwo.Forum.SendPM(pm,
                (PrivateMessage pmSent, PrivateMessageError err, String errDetails, object value, object cookie) =>
                {
                    Assert.AreEqual(PrivateMessageError.PMTooLongDidntSend, err);
                    Assert.IsInstanceOf<Int32>(value);
                }
            );
            Assert.IsFalse(rc);
        }
        [Test]
        public void Test07NoRecepient()
        {
            PrivateMessage pm = new PrivateMessage(
                null,
                null,
                "Test PM",
                "This is a test.");
            bool rc = SetupTwoPlusTwo.Forum.SendPM(pm,
                (PrivateMessage pmSent, PrivateMessageError err, String errDetails, object value, object cookie) =>
                {
                    Assert.AreEqual(PrivateMessageError.PMNoRecepient, err);
                }
            );
            Assert.IsFalse(rc);
        }
        [Test]
        public void Test08TooManyRecepients()
        {
            PrivateMessage pm = new PrivateMessage(
                new List<String> { "hydrox1908", "Andy Schleck", "Anders Hejlsberg" },
                null,
                "Test PM",
                "This is a test.");
            bool rc = SetupTwoPlusTwo.Forum.SendPM(pm,
                (PrivateMessage pmSent, PrivateMessageError err, String errDetails, object value, object cookie) =>
                {
                    Assert.AreEqual(PrivateMessageError.PMTooManyRecepients, err);
                    Assert.IsInstanceOf<Int32>(value);
                }
            );
            Assert.IsFalse(rc);
        }
        //[Test]
        //public void Test99PMSuccess()
        //{
        //    PrivateMessage pm = new PrivateMessage(
        //        new List<String> { "Fennec Fox" },
        //        null,
        //        "Test PM ",
        //        "This is a test.");
        //    bool rc = SetupTwoPlusTwo.Forum.SendPM(pm,
        //        (PrivateMessage pmSent, PrivateMessageError err, String errDetails, object value, object cookie) =>
        //        {
        //            Assert.AreEqual(PrivateMessageError.PMSuccess, err);
        //        }
        //    );
        //    Assert.IsTrue(rc);
        //    //System.Threading.Thread.Sleep(30000);
        //}
    }
}
