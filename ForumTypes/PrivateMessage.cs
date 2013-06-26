using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace POG.Forum
{
    public delegate void PMResult(PrivateMessage pm, PrivateMessageError err, String errMessage, object value, object cookie);
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
    public class PrivateMessage
    {
        List<String> to;
        List<String> bcc;
        public PrivateMessage(IEnumerable<String> sTo, IEnumerable<String> sBcc, String title, String content, DateTime? ts = null)
        {
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
    }
}
