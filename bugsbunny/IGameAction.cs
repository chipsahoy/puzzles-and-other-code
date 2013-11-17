using System;
using System.Runtime.InteropServices;
using System.ComponentModel;
namespace POG.Automation
{
    [ComVisible(true)]
    [Guid("126B1B41-50A0-48DE-A741-72A3BDBBBE7C")]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface ICorralEvent
    {
        CorralAction Action {
            [Description("Parsed from post/pm: What the poster is trying to do")]
            get;
        }
        string Actor
        {
            [Description("Name of the poster who posted or pm'd")]
            get;
        }
        string Content {
            [Description("Body of post/pm")]
            get;
        }
        int Id {
            [Description("Id of post/pm. Useful for [quote=xxx]")]
            get;
        }
        string Parameter {
            [Description("Parsed from post/pm: optional extra info")]
            get;
        }
        int PostNumber {
            [Description("Number of post.")]
            get;
        }
        string Target {
            [Description("Parsed from post/pm: Who the poster is targeting")]
            get;
        }
        string TimeString {
            [Description("Timestamp of post/pm in UTC")]
            get;
        }
        string Title {
            [Description("Title of post/pm")]
            get;
        }
    }
}
