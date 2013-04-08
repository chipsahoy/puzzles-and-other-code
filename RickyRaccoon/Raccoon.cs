using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace RickyRaccoon
{
    public partial class Raccoon : Form
    {
        List<String> roster = new List<string>();
        public Raccoon()
        {
            InitializeComponent();
        }

        private void btnPasteList_Click(object sender, EventArgs e)
        {
            roster.Clear();
            btnCheckNames.Enabled = false;
            object o = Clipboard.GetData(DataFormats.Text);
            if (o != null)
            {
                String clip = o as String;
                String[] lines = clip.Split(new String[] { "\r\n", "\r" }, StringSplitOptions.RemoveEmptyEntries);
                foreach (String line in lines)
                {
                    String name = line.Trim();
                    roster.Add(name);
                }
                txtPlayerCount.Text = roster.Count.ToString();
                btnCheckNames.Enabled = true;
            }

        }

        private void btnCheckNames_Click(object sender, EventArgs e)
        {

        }

        private void btnDoIt_Click(object sender, EventArgs e)
        {

        }
    }
}
