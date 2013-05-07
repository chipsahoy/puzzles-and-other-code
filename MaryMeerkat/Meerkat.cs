using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Newtonsoft.Json;
using System.IO;

namespace MaryMeerkat
{
    public partial class Meerkat : Form
    {
        public Meerkat()
        {
            InitializeComponent();
        }

        private void loadGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Show the dialog and get result.
            OpenFileDialog openFileDialog = new OpenFileDialog();
            DialogResult result =  openFileDialog.ShowDialog();
            if (result == DialogResult.OK) // Test result.
            {
                string file = openFileDialog.FileName;
                try
                {
                    RickyRaccoon.RolePMSet temp = JsonConvert.DeserializeObject<RickyRaccoon.RolePMSet>(File.ReadAllText(file));
                    resetGameList(temp);
                }
                catch (JsonSerializationException error)
                {
                    Console.WriteLine(error);
                    MessageBox.Show("This isn't valid JSON!");
                    return;
                }
            }
        }

        private void resetGameList(RickyRaccoon.RolePMSet pms)
        {
            //todo
        }
    }
}
