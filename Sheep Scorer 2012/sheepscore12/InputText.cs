using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace sheepscore12
{
    public partial class InputText : Form
    {
        public InputText()
        {
            InitializeComponent();
        }

        //ok
        private void button1_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        //cancel
        private void button2_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void InputText_Load(object sender, EventArgs e)
        {

        }

        private void InputText_Shown(object sender, EventArgs e)
        {
            textBox1.Focus();
            this.MaximumSize = this.Size;
            this.MinimumSize = this.Size;
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                this.DialogResult = DialogResult.OK;
            }
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {

        }
    }
}
