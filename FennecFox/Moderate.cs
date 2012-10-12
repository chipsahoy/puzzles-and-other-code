using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace POG.FennecFox
{
    public partial class Moderate : Form
    {
        public Moderate()
        {
            InitializeComponent();
        }
        public Moderate(Boolean autoPost, Boolean autoLock)
            : this()
        {
            chkLockThread.Checked = autoLock;
            chkAutoPost.Checked = autoPost;
        }
        public void GetResults(out Boolean autoPost, out Boolean autoLock)
        {
            autoPost = chkAutoPost.Checked;
            autoLock = chkLockThread.Checked;
        }
    }
}
