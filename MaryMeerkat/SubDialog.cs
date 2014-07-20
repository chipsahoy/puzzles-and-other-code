using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MaryMeerkat
{
    public partial class SubDialog : Form
    {
        public SubDialog()
        {
            InitializeComponent();
        }

        public void SetQuestion(string question)
        {
            lblQuestion.Text = question;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }
    }
}
