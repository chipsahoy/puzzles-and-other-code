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
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Form f = new FoxParent();
            if (f != null)
            {
                Application.Run(f);
            }
        }
    }
}
