namespace MaryMeerkat
{
    partial class Meerkat
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadGameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadGameToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.loadGameToolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutMaryMeerkatToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sendPMToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.lockThreadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 24);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(681, 507);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.dataGridView1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(673, 481);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Players";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(3, 3);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(667, 475);
            this.dataGridView1.TabIndex = 0;
            // 
            // tabPage2
            // 
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(673, 481);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Teams";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(681, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadGameToolStripMenuItem,
            this.loadGameToolStripMenuItem1,
            this.loadGameToolStripMenuItem2});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // loadGameToolStripMenuItem
            // 
            this.loadGameToolStripMenuItem.Name = "loadGameToolStripMenuItem";
            this.loadGameToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.loadGameToolStripMenuItem.Text = "New";
            this.loadGameToolStripMenuItem.Click += new System.EventHandler(this.loadGameToolStripMenuItem_Click);
            // 
            // loadGameToolStripMenuItem1
            // 
            this.loadGameToolStripMenuItem1.Name = "loadGameToolStripMenuItem1";
            this.loadGameToolStripMenuItem1.Size = new System.Drawing.Size(152, 22);
            this.loadGameToolStripMenuItem1.Text = "Save";
            // 
            // loadGameToolStripMenuItem2
            // 
            this.loadGameToolStripMenuItem2.Name = "loadGameToolStripMenuItem2";
            this.loadGameToolStripMenuItem2.Size = new System.Drawing.Size(152, 22);
            this.loadGameToolStripMenuItem2.Text = "Load";
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutMaryMeerkatToolStripMenuItem,
            this.sendPMToolStripMenuItem,
            this.lockThreadToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(59, 20);
            this.helpToolStripMenuItem.Text = "Actions";
            // 
            // aboutMaryMeerkatToolStripMenuItem
            // 
            this.aboutMaryMeerkatToolStripMenuItem.Name = "aboutMaryMeerkatToolStripMenuItem";
            this.aboutMaryMeerkatToolStripMenuItem.Size = new System.Drawing.Size(211, 22);
            this.aboutMaryMeerkatToolStripMenuItem.Text = "Paste Player List in Thread";
            // 
            // sendPMToolStripMenuItem
            // 
            this.sendPMToolStripMenuItem.Name = "sendPMToolStripMenuItem";
            this.sendPMToolStripMenuItem.Size = new System.Drawing.Size(211, 22);
            this.sendPMToolStripMenuItem.Text = "Send PM";
            // 
            // lockThreadToolStripMenuItem
            // 
            this.lockThreadToolStripMenuItem.Name = "lockThreadToolStripMenuItem";
            this.lockThreadToolStripMenuItem.Size = new System.Drawing.Size(211, 22);
            this.lockThreadToolStripMenuItem.Text = "Lock Thread";
            // 
            // Meerkat
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(681, 531);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.menuStrip1);
            this.Name = "Meerkat";
            this.Text = "Mary Meerkat, Modder";
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadGameToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutMaryMeerkatToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadGameToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem loadGameToolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem sendPMToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem lockThreadToolStripMenuItem;
    }
}

