using System;
using System.Runtime.InteropServices;
namespace POG.Forum
{
    [ComVisible(true)]
    [Guid("576CABF4-2566-4D4E-A326-100A5C273F74")]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface IPost
    {
       // System.Collections.Generic.IEnumerable<Bold> Bolded { get; }
        string Content { get; }
        PostEdit Edit { get; }
        Poster Poster { get; }
        int PostId { get; }
        string PostLink { get; }
        int PostNumber { get; }
        int ThreadId { get; }
      //  DateTimeOffset Time { get; }
        string Title { get; }
    }
}
