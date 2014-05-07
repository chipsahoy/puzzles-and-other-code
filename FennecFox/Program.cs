using System;
using System.Windows.Forms;
using CommandLine.Text;
using CommandLine;

namespace POG.FennecFox
{
    class Options
    {
        [Option('h', "host", DefaultValue="forumserver.twoplustwo.com", HelpText = "base url of forum")]
        public String Host { get; set; }
    }
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            var options = new Options();
            String forum = "forumserver.twoplustwo.com";
            if (CommandLine.Parser.Default.ParseArguments(args, options))
            {
                forum = options.Host.ToLowerInvariant();
            }
            Form f = new FoxParent(forum);
            if (f != null)
            {
                Application.Run(f);
            }
        }
    }
}
