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
    public partial class OP : Form
    {
        public bool makeOP = false;
        public bool cancelRand = false;
        public OP()
        {
            InitializeComponent();
        }

        public TextBox getOPTextBox()
        {
            return textOP;
        }

        private void btnMakeOP_Click(object sender, EventArgs e)
        {
            makeOP = true;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            cancelRand = true;
            this.Close();
        }

        private void btnDontMakeOP_Click(object sender, EventArgs e)
        {
            makeOP = false;
            this.Close();
        }

        private void btnCopyOP_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(textOP.Text);
        }        
    }
}
