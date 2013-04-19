using System;
using System.Windows.Forms;

namespace POG.FennecFox
{
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
            String forum = "forumserver.twoplustwo.com";
            if (args.Length > 0)
            {
                forum = args[0].ToLowerInvariant();
            }
            Form f = new FoxParent(forum);
            if (f != null)
            {
                Application.Run(f);
            }
        }
    }
}
