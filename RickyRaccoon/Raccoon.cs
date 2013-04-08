using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace RickyRaccoon
{
    public partial class Raccoon : Form
    {

        List<String> roster = new List<string>();
        List<String> roles = new List<string>();

        public Raccoon()
        {
            InitializeComponent();
        }

        private void btnPasteRoles_Click(object sender, EventArgs e)
        {
            roles.Clear();
            object o = Clipboard.GetData(DataFormats.Text);
            if (o != null)
            {
                String clip = o as String;
                String[] lines = clip.Split(new String[] { "\r\n", "\r" }, StringSplitOptions.RemoveEmptyEntries);
                foreach (String line in lines)
                {
                    String name = line.Trim();
                    if(Regex.IsMatch(name, @"^[\s]*$")) continue;
                    roles.Add(name);
                }
                txtRoleCount.Text = roles.Count.ToString();
                txtRoleList.Text = String.Join(Environment.NewLine, roles);
            }
            if (roles.Count > 0 && roster.Count > 0 && roster.Count == roles.Count)
            {
                btnDoIt.Enabled = true;
            }
            else btnDoIt.Enabled = false;
        }

        private void btnPasteList_Click(object sender, EventArgs e)
        {
            roster.Clear();
            object o = Clipboard.GetData(DataFormats.Text);
            if (o != null)
            {
                String clip = o as String;
                String[] lines = clip.Split(new String[] { "\r\n", "\r" }, StringSplitOptions.RemoveEmptyEntries);
                foreach (String line in lines)
                {
                    String name = line.Trim();
                    if (Regex.IsMatch(name, @"^[\s]*$")) continue;
                    roster.Add(name);
                }
                txtPlayerCount.Text = roster.Count.ToString();
                txtPlayerList.Text = String.Join(Environment.NewLine, roster);
                if (roles.Count > 0 && roster.Count > 0 && roster.Count == roles.Count)
                {
                    btnDoIt.Enabled = true;
                }
                else btnDoIt.Enabled = false;
            }
        }


        private void cmbRoleSet_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void txtPlayerCount_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnDoIt_Click(object sender, EventArgs e)
        {
            if (roster.Count == 0)
            {
                MessageBox.Show("Please enter in your playerlist!");
                return;
            }
            else if (roles.Count == 0)
            {
                MessageBox.Show("Please enter in your roles!");
                return;
            }
            else if (roster.Count != roles.Count)
            {
                MessageBox.Show("There needs to be the same amount of players as roles!");
                return;
            }
            Random rng = new Random();
            int n = roles.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                String value = roles[k];
                roles[k] = roles[n];
                roles[n] = value;
            }
            txtPlayerList.Text = "";
            for(int i=0; i<roles.Count; i++)
            {
                txtPlayerList.Text += roster[i] + " - " + roles[i] + Environment.NewLine;
            }            
            String password = txtPassword.Text;
            btnDoIt.Enabled = false;
        }

        private void txtPlayerList_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtPassword_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
