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
    public partial class PlayerList : Form
    {
        public IEnumerable<String> Players
        {
            get
            {
                String players = txtNewPlayers.Text;
                List<String> rawList = players.Split(
                    new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(p => p.Trim())
                    .Distinct().ToList();
                return rawList;
            }
        }

        public PlayerList()
        {
            InitializeComponent();
        }

        private void btnSubmitPlayers_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.OK;
            Close();
        }
    }
}
