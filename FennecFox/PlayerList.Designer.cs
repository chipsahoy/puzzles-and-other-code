namespace POG.FennecFox
{
    partial class PlayerList
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PlayerList));
            this.btnSubmitPlayers = new System.Windows.Forms.Button();
            this.txtNewPlayers = new System.Windows.Forms.TextBox();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.Player = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Removed = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Joined = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Team = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Role = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PM = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // btnSubmitPlayers
            // 
            this.btnSubmitPlayers.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSubmitPlayers.Location = new System.Drawing.Point(756, 12);
            this.btnSubmitPlayers.Name = "btnSubmitPlayers";
            this.btnSubmitPlayers.Size = new System.Drawing.Size(75, 23);
            this.btnSubmitPlayers.TabIndex = 0;
            this.btnSubmitPlayers.Text = "OK";
            this.btnSubmitPlayers.UseVisualStyleBackColor = true;
            this.btnSubmitPlayers.Click += new System.EventHandler(this.btnSubmitPlayers_Click);
            // 
            // txtNewPlayers
            // 
            this.txtNewPlayers.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.txtNewPlayers.Location = new System.Drawing.Point(12, 12);
            this.txtNewPlayers.Multiline = true;
            this.txtNewPlayers.Name = "txtNewPlayers";
            this.txtNewPlayers.Size = new System.Drawing.Size(110, 416);
            this.txtNewPlayers.TabIndex = 1;
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Player,
            this.Removed,
            this.Joined,
            this.Team,
            this.Role,
            this.PM});
            this.dataGridView1.Location = new System.Drawing.Point(139, 12);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(611, 301);
            this.dataGridView1.TabIndex = 2;
            // 
            // Player
            // 
            this.Player.HeaderText = "Player";
            this.Player.Name = "Player";
            // 
            // Removed
            // 
            this.Removed.HeaderText = "Removed";
            this.Removed.Name = "Removed";
            this.Removed.Width = 60;
            // 
            // Joined
            // 
            this.Joined.HeaderText = "Joined";
            this.Joined.Name = "Joined";
            this.Joined.Width = 60;
            // 
            // Team
            // 
            this.Team.HeaderText = "Team";
            this.Team.Name = "Team";
            // 
            // Role
            // 
            this.Role.HeaderText = "Role";
            this.Role.Name = "Role";
            // 
            // PM
            // 
            this.PM.HeaderText = "PM";
            this.PM.Name = "PM";
            this.PM.Width = 150;
            // 
            // PlayerList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(843, 440);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.txtNewPlayers);
            this.Controls.Add(this.btnSubmitPlayers);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "PlayerList";
            this.Text = "PlayerList";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnSubmitPlayers;
        private System.Windows.Forms.TextBox txtNewPlayers;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Player;
        private System.Windows.Forms.DataGridViewTextBoxColumn Removed;
        private System.Windows.Forms.DataGridViewTextBoxColumn Joined;
        private System.Windows.Forms.DataGridViewTextBoxColumn Team;
        private System.Windows.Forms.DataGridViewTextBoxColumn Role;
        private System.Windows.Forms.DataGridViewTextBoxColumn PM;
    }
}