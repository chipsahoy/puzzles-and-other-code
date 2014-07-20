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
    public partial class editQuestions : Form
    {
        public editQuestions()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.Title = "Load Questions from File";
            openFileDialog1.Filter = "Text File|*.txt";

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                System.IO.FileInfo finfo = new System.IO.FileInfo(openFileDialog1.FileName);

                if (finfo.Exists)
                {
                    textBox1.Text = System.IO.File.ReadAllText(openFileDialog1.FileName);
                }
                else
                {
                    MessageBox.Show("File not found:" + Environment.NewLine 
                        + openFileDialog1.FileName);
                }

            }

        }

        private void editQuestions_Load(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void editQuestions_Shown(object sender, EventArgs e)
        {
            if (Form1.sg != null)
            {
                textBox1.Text = string.Join(Environment.NewLine,
                    Form1.sg.Questions.Select(x => x.Text).ToArray());
            }
            textBox1.Select(0, 0);
        }


    }
}
