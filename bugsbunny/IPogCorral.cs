using System;
using System.Runtime.InteropServices;
namespace POG.Automation
{
    [ComVisible(true)]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    [Guid("0449A389-9A23-4DC5-AA08-6F3B8223F5A0")]
    public interface IPogCorral
    {
        void Login(string url, string username, string password);
        void MakePost(string title, string content);
        void SendPM(string to, string title, string content);
        void SendPMToGroup(string[] to, string title, string content);
        void StartPolling(Int32 startPost, object startTime, object stopTime);
        void Stop();
    }
}
