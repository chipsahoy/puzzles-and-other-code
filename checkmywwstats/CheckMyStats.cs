using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Apache.NMS;
using Apache.NMS.ActiveMQ;
using Apache.NMS.Util;

namespace POG.Database
{
	public class CheckMyStats
	{
		protected static TimeSpan receiveTimeout = TimeSpan.FromSeconds(30);
		IConnection _connection;
		ISession _session;
		IMessageConsumer _lobbyRequestsQueue;
        IMessageConsumer _threadRequestsQueue;
		IMessageProducer _postsQueue;
        IMessageProducer _lobbyQueue;
        //IDestination _readQueue;

        private Boolean DoLogin(String name)
        {
            ITextMessage request = _session.CreateTextMessage("login");
            //request.NMSReplyTo = _readQueue;
            request.Properties["Name"] = name;
            Console.WriteLine("Sending login for '{0}'", name);
            IDestination destination = SessionUtil.GetDestination(_session, "topic://fennecfox.listeners");
            using (IMessageProducer loginTopic = _session.CreateProducer(destination))
            {
                try
                {
                    loginTopic.Send(request);
                }
                catch (RequestTimedOutException)
                {
                    Console.WriteLine("*** Timeout sending login for '{0}'", name);
                    return false;
                }
            }
            return true;
        }
        private Boolean DoLogout(String name)
        {
            ITextMessage request = _session.CreateTextMessage("logout");
            //request.NMSReplyTo = _readQueue;
            request.Properties["Name"] = name;
            Console.WriteLine("Sending logout for '{0}'", name);
            IDestination destination = SessionUtil.GetDestination(_session, "topic://fennecfox.listeners");
            using (IMessageProducer loginTopic = _session.CreateProducer(destination))
            {
                try
                {
                    loginTopic.Send(request);
                }
                catch (RequestTimedOutException)
                {
                    Console.WriteLine("*** Timeout sending login for '{0}'", name);
                    return false;
                }
            }
            return true;
        }
        public static Int32 TidFromURL(String url)
        {
            int ixTidStart = url.LastIndexOf('-') + 1;
            string tid = url.Substring(ixTidStart, url.Length - (ixTidStart + 1));
            Int32 rc = 0;
            Int32.TryParse(tid, out rc);
            return rc;
        }
        public void PublishLobbyPage(String requestId, String url, String title, String threadIconText,
            String op, String lastPoster, DateTime lastPostTime, Int32 replies, Int32 views)
        {
            // Send a message
            ITextMessage request = _session.CreateTextMessage("lobby thread");
            //request.NMSReplyTo = _readQueue;
            request.NMSCorrelationID = requestId;
            request.Properties["Type"] = "lobbyThread";
            request.Properties["URL"] = url;
            request.Properties["Title"] = title;
            request.Properties["IconText"] = threadIconText;
            request.Properties["OP"] = op;
            request.Properties["LastPoster"] = lastPoster;
            request.Properties["LastPostTime"] = lastPostTime.ToString("yyyy-MM-dd HH:mm:ss");
            request.Properties["Replies"] = replies.ToString();
            request.Properties["Views"] = views.ToString();
            Int32 tid = TidFromURL(url);
            request.Properties["Thread"] = tid;
            Console.WriteLine("Sending Thread #{0} '{1}'", tid, title);
            try
            {
                _lobbyQueue.Send(request);
            }
            catch (RequestTimedOutException)
            {
                Console.WriteLine("Timeout sending tid '{0}'", tid);
            }
        }
        public void PublishPost(String threadURL, String ID, String posterName, Int32 postNumber, 
            DateTime postTime, String postLink, String html, String title, String edit)
		{
			// Send a message
			ITextMessage request = _session.CreateTextMessage(html);
            //request.NMSReplyTo = _readQueue;
			request.NMSCorrelationID = ID;
            request.Properties["Type"] = "post";
            request.Properties["URL"] = threadURL;
            request.Properties["Title"] = title;
            request.Properties["Edit"] = edit;
			request.Properties["Poster"] = posterName;
			request.Properties["Number"] = postNumber.ToString();
			request.Properties["Time"] = postTime.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss");
			request.Properties["Link"] = postLink;
			// Link: ?p=....&
			int ixPostStart = postLink.LastIndexOf("?p=") + 3;
			string sPost = postLink.Substring(ixPostStart);
			int ixPostLast = sPost.IndexOf('&');
			sPost = sPost.Substring(0, ixPostLast);
			request.Properties["PostId"] = sPost;
			// Thread: -.../
			int ixTidStart = threadURL.LastIndexOf('-') + 1;
			string tid = threadURL.Substring(ixTidStart, threadURL.Length - (ixTidStart + 1));
			request.Properties["Thread"] = tid;
			Console.WriteLine("Sending post #{0} by {1}", postNumber.ToString(), posterName);
			try
			{
				_postsQueue.Send(request);
			}
			catch (RequestTimedOutException e)
			{
				Console.WriteLine("Timeout sending post " + postNumber.ToString() + " " + e.ToString());
			}

		}
        public void PublishDoneReading(String threadURL, String ID, Int32 StartPost, Int32 EndPost, DateTime serverTime)
        {
            // Send a message
            ITextMessage request = _session.CreateTextMessage("Finished Reading");
            //request.NMSReplyTo = _readQueue;
            request.NMSCorrelationID = ID;
            request.Properties["Type"] = "End of set";
            request.Properties["URL"] = threadURL;
            request.Properties["StartPost"] = StartPost.ToString();
            request.Properties["EndPost"] = StartPost.ToString();
            request.Properties["Time"] = serverTime.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss");
            // Thread: -.../
            int ixTidStart = threadURL.LastIndexOf('-') + 1;
            string tid = threadURL.Substring(ixTidStart, threadURL.Length - (ixTidStart + 1));
            request.Properties["Thread"] = tid;
            try
            {
                _postsQueue.Send(request);
            }
            catch (RequestTimedOutException)
            {
                Console.WriteLine("Timeout sending Done reading msg. ");
            }

        }
        public CheckMyStats()
        {
        }
        public void Login(String name)
		{
			Uri connecturi = new Uri("failover:tcp://mq.checkmywwstats.com:61616");
			Console.WriteLine("About to connect to " + connecturi);
			// NOTE: ensure the nmsprovider-activemq.config file exists in the executable folder.
			IConnectionFactory factory = new Apache.NMS.ActiveMQ.ConnectionFactory(connecturi);
			_connection = factory.CreateConnection();
			_session = _connection.CreateSession();
			// Examples for getting a destination:
			//
			// Hard coded destinations:
			//    IDestination destination = session.GetQueue("FOO.BAR");
			//    Debug.Assert(destination is IQueue);
			//    IDestination destination = session.GetTopic("FOO.BAR");
			//    Debug.Assert(destination is ITopic);
			//
			// Embedded destination type in the name:
			//    IDestination destination = SessionUtil.GetDestination(session, "queue://FOO.BAR");
			//    Debug.Assert(destination is IQueue);
			//    IDestination destination = SessionUtil.GetDestination(session, "topic://FOO.BAR");
			//    Debug.Assert(destination is ITopic);
			//
			// Defaults to queue if type is not specified:
			//    IDestination destination = SessionUtil.GetDestination(session, "FOO.BAR");
			//    Debug.Assert(destination is IQueue);
			//
			// .NET 3.5 Supports Extension methods for a simplified syntax:
			//    IDestination destination = session.GetDestination("queue://FOO.BAR");
			//    Debug.Assert(destination is IQueue);
			//    IDestination destination = session.GetDestination("topic://FOO.BAR");
			//    Debug.Assert(destination is ITopic);
            //_readQueue = _session.CreateTemporaryQueue();
            DoLogin(name);
			IDestination destination = SessionUtil.GetDestination(_session, "queue://fennecfox.pleasereadlobby");
			Console.WriteLine("Using destination: " + destination);
			_lobbyRequestsQueue = _session.CreateConsumer(destination);

            destination = SessionUtil.GetDestination(_session, "queue://fennecfox.pleasereadthread");
            _threadRequestsQueue = _session.CreateConsumer(destination);

            destination = SessionUtil.GetDestination(_session, "queue://fennecfox.posts");
            _postsQueue = _session.CreateProducer(destination);

            destination = SessionUtil.GetDestination(_session, "queue://fennecfox.lobby");
            _lobbyQueue = _session.CreateProducer(destination);

			// Start the connection so that messages will be processed.
			_connection.Start();
			_postsQueue.DeliveryMode = MsgDeliveryMode.Persistent;
			_postsQueue.RequestTimeout = receiveTimeout;
            _lobbyQueue.DeliveryMode = MsgDeliveryMode.Persistent;
            _lobbyQueue.RequestTimeout = receiveTimeout;

			_lobbyRequestsQueue.Listener += new MessageListener(OnLobbyMessage);
            _threadRequestsQueue.Listener += new MessageListener(OnThreadMessage);

		}
        protected void OnThreadMessage(IMessage receivedMsg)
        {
            ITextMessage message;
            message = receivedMsg as ITextMessage;
            if (message == null)
            {
                Console.WriteLine("No message received!");
            }
            else
            {
                String url = (String)message.Properties["URL"];
                url = pogutils.Utils.NormalizeUrl(url);
                Int32 startPost = Convert.ToInt32(message.Properties["startPost"]);
                Int32 endPost = Int32.MaxValue;
                Object o = message.Properties["endPost"];
                if (o != null)
                {
                    endPost = Convert.ToInt32(o);
                }
                Console.WriteLine(receivedMsg.ToString());
                OnThreadReadEvent(new ThreadReadEventArgs(url, startPost, endPost));
            }
        }
		protected void OnLobbyMessage(IMessage receivedMsg)
		{
			ITextMessage message;
			message = receivedMsg as ITextMessage;
			if (message == null)
			{
				Console.WriteLine("No message received!");
			}
			else
			{
                Boolean recentFirst = Convert.ToBoolean((String)message.Properties["recentFirst"]);
                Int32 startPage = Convert.ToInt32(message.Properties["startPage"]);
                Int32 endPage = Convert.ToInt32(message.Properties["endPage"]);
                Console.WriteLine(receivedMsg.ToString());
                String url = "http://forumserver.twoplustwo.com/59/puzzles-other-games/";
                OnLobbyReadEvent(new LobbyReadEventArgs(url, startPage, endPage, recentFirst));
                //OnMessage(receivedMsg);
			}
		}

        public void Logout(string _username)
        {
            throw new NotImplementedException();
        }
        public event EventHandler<LobbyReadEventArgs> LobbyReadEvent;
        protected void OnLobbyReadEvent(LobbyReadEventArgs e)
        {
            var handler = LobbyReadEvent;
            if (handler != null)
            {
                handler(this, e);
            }
        }
        public event EventHandler<ThreadReadEventArgs> ThreadReadEvent;
        protected void OnThreadReadEvent(ThreadReadEventArgs e)
        {
            var handler = ThreadReadEvent;
            if (handler != null)
            {
                handler(this, e);
            }
        }
    }
}
