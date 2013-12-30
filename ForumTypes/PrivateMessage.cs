using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace POG.Forum
{
	public delegate void PMReadMessageResult(Int32 id, PrivateMessage pm, object cookie);
	public delegate void PMReadPageResult(PMFolderPage page, String errMessage, object cookie);
	public delegate void PMResult(PrivateMessage pm, PrivateMessageError err, String errMessage, object value, object cookie);
	[ComVisible(true)]
	[Guid("86863B24-FB86-4577-ABF4-A17D4B6109EB")]
	public enum PrivateMessageError
	{
		PMSuccess = 0,
		PMUnknownError = 1,
		PMTooSoon = 2,
		PMUnknownRecepient = 3,
		PMNotLoggedIn = 4,
		PMNotAllowed = 5,
		PMTooManyRecepients = 6,
		PMSenderFull = 7,
		PMRecepientFull = 8,
		PMHttpError = 9,
		PMRecepientNotAllowed = 10,
		PMTooLongDidntSend = 11,
		PMNoRecepient = 12,
		PMNoTitle = 13,
		PMNoBody = 14,
	}
	[ComVisible(true)]
	[Guid("26D3E7F9-E146-4E18-8086-8F81C4D70250")]
	public class PrivateMessage
	{
		List<String> to = new List<string>();
		List<String> bcc = new List<string>();
		public PrivateMessage(String from, IEnumerable<String> sTo, String title, String content, DateTime ts, Int32 id)
		{
			// pm we received
			Id = id;
			From = from;
			if (title == null)
			{
				title = String.Empty;
			}
			Title = title;
			if (content == null)
			{
				content = String.Empty;
			}
			Content = content;
			TimeStamp = ts;
		}
		public PrivateMessage(IEnumerable<String> sTo, IEnumerable<String> sBcc, String title, String content, DateTime? ts = null, Int32? id = null)
		{
			// pm we send / sent
			From = "Me";
			if (sTo == null)
			{
				sTo = new List<String>();
			}

			to = new List<string>(sTo);
			if (sBcc == null)
			{
				sBcc = new List<String>();
			}
			bcc = new List<string>(sBcc);
			if (title == null)
			{
				title = String.Empty;
			}
			Title = title;
			if (content == null)
			{
				content = String.Empty;
			}
			Content = content;
			TimeStamp = ts;
			if (id != null)
			{
				Id = id.Value;
			}
		}
		public IEnumerable<String> To
		{
			get
			{
				return to;
			}
		}
		public IEnumerable<String> BCC
		{
			get
			{
				return bcc;
			}
		}
		public String Title
		{
			get;
			private set;
		}
		public String Content
		{
			get;
			private set;
		}
		public DateTime? TimeStamp
		{
			get;
			private set;
		}

		public String From { get; private set; }

		public int Id { get; private set; }
	}
	[ComVisible(true)]
	[Guid("9268C65E-3F4A-4F20-8682-5F422C7B84E6")]
	public class PMHeader
	{
		public PMHeader(Int32 id, DateTimeOffset timestamp, String sender, Int32 senderId, String title, String firstLine, Boolean unread)
		{
			Id = id;
			Unread = unread;
			Timestamp = timestamp;
			Sender = sender;
			SenderId = senderId;
			Title = title;
			FirstLine = firstLine;
		}
		public Int32 Id
		{
			get;
			private set;
		}
		public Boolean Unread
		{
			get;
			private set;
		}
		public DateTimeOffset Timestamp
		{
			get;
			private set;
		}
		public String Sender
		{
			get;
			private set;
		}
		public Int32 SenderId
		{
			get;
			private set;
		}
		public String Title
		{
			get;
			private set;
		}
		public String FirstLine
		{
			get;
			private set;
		}
	}
	[ComVisible(true)]
	[Guid("F1237A44-2B20-4F28-B130-69DFD420A706")]
	public class PMFolderPage
	{
		List<PMHeader> _messages;
		public PMFolderPage(Int32 capacity, Int32 totalMessages, String folderName, Int32 folderIndex, Int32 count, Int32 unreadCount, Int32 page,
			IEnumerable<PMHeader> messages)
		{
			Capacity = capacity;
			TotalMessages = totalMessages;
			FolderIndex = folderIndex;
			Name = folderName;
			Count = count;
			UnreadCount = unreadCount;
			Page = page;
			_messages = messages.ToList();
		}
		public Int32 Capacity
		{
			get;
			private set;
		}
		public Int32 TotalMessages
		{
			get;
			private set;
		}
		public Int32 FolderIndex
		{
			get;
			private set;
		}
		public String Name
		{
			get;
			private set;
		}
		public Int32 Count
		{
			get;
			private set;
		}
		public Int32 UnreadCount
		{
			get;
			private set;
		}
		public Int32 Page
		{
			get;
			private set;
		}
		public Int32 MessagesThisPage
		{
			get
			{
				Int32 rc = _messages.Count;
				return rc;
			}
		}
		public PMHeader this[Int32 ix]
		{
			get
			{
				return _messages[ix];
			}
		}
	}
}
