using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Apache.NMS;
using Apache.NMS.Stomp;
using Apache.NMS.Util;

namespace POG.Database
{
	public class CheckMyStats
	{
		protected static TimeSpan receiveTimeout = TimeSpan.FromSeconds(10);
		IConnection connection;
		ISession session;
		IMessageConsumer consumer;
		IMessageProducer producer;

		public void PublishPost(String threadURL, String posterName, Int32 postNumber, DateTime postTime, String postLink, String html)
		{
			// Send a message
			ITextMessage request = session.CreateTextMessage(html);
			request.NMSCorrelationID = "abc";
			request.Properties["URL"] = threadURL;
			request.Properties["Poster"] = posterName;
			request.Properties["Number"] = postNumber.ToString();
			request.Properties["Time"] = postTime.ToUniversalTime().ToString();
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
				producer.Send(request);
			}
			catch (RequestTimedOutException e)
			{
				Console.WriteLine("Timeout sending post " + postNumber.ToString() + " " + e.ToString());
			}

		}
		public CheckMyStats()
		{
			Uri connecturi = new Uri("failover:tcp://173.230.149.24:61612");
			Console.WriteLine("About to connect to " + connecturi);
			// NOTE: ensure the nmsprovider-activemq.config file exists in the executable folder.
			IConnectionFactory factory = new Apache.NMS.Stomp.ConnectionFactory(connecturi);
			connection = factory.CreateConnection();
			session = connection.CreateSession();
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
			IDestination destination = SessionUtil.GetDestination(session, "queue://FOO.BAR");
			Console.WriteLine("Using destination: " + destination);
			//consumer = session.CreateConsumer(destination);
			producer = session.CreateProducer(destination);

			// Start the connection so that messages will be processed.
			connection.Start();
			producer.DeliveryMode = MsgDeliveryMode.Persistent;
			producer.RequestTimeout = receiveTimeout;

			//consumer.Listener += new MessageListener(OnMessage);

		}
		protected static void OnMessage(IMessage receivedMsg)
		{
			ITextMessage message;
			message = receivedMsg as ITextMessage;
			if (message == null)
			{
				Console.WriteLine("No message received!");
			}
			else
			{
				Console.WriteLine("Received message with ID:   " + message.NMSMessageId);
				Console.WriteLine("Received post #{0} by {1}", message.Properties["Number"].ToString(), message.Properties["Poster"].ToString());
			}
		}
	}
}
