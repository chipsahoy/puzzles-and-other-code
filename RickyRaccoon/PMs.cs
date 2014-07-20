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
    public partial class PMs : Form
    {
        public bool savePMs = false;
        public PMs()
        {
            InitializeComponent();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        public DataGridView getDataPMs()
        {
            return dataPMs;
        }

        private void btnSavePMs_Click(object sender, EventArgs e)
        {
            savePMs = true;
            this.Close();
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
