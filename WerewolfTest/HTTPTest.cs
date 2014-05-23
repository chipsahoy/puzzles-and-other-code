using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using POG.Forum;

namespace WerewolfTest
{
    [TestFixture]
    public class HTTPTest
    {
        ConnectionSettings _settings;
        [TestFixtureSetUp]
        public void Setup()
        {
            _settings = new ConnectionSettings("http://forumserver.twoplustwo.com");
            
        }
        [TestFixtureTearDown]
        public void Teardown()
        {
        }
        [Test]
        public void TestUnicode()
        {
            ConnectionSettings settings = _settings.Clone();
            settings.Url = "http://forumserver.twoplustwo.com/showpost.php?p=43345708&postcount=854";
            string rc = HtmlHelper.GetUrlResponseString(settings);
        }
    }
 
}
