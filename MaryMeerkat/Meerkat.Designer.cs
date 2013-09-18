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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveGameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadGameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuPlayerList = new System.Windows.Forms.ToolStripMenuItem();
            this.sendPMToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.lockThreadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.txtGameURL = new System.Windows.Forms.TextBox();
            this.tab2 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            this.dataPlayers = new System.Windows.Forms.DataGridView();
            this.boxSelect = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.txtPlayerName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.boxTeam = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.txtRole = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.boxPlayerColor = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.boxAlive = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.txtDeathReason = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.btnSelectAll = new System.Windows.Forms.Button();
            this.btnDeathReveal = new System.Windows.Forms.Button();
            this.btnSubOut = new System.Windows.Forms.Button();
            this.btnPeek = new System.Windows.Forms.Button();
            this.btnResend = new System.Windows.Forms.Button();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.dataTeams = new System.Windows.Forms.DataGridView();
            this.txtTeam = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.boxColor = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.txtPlayerCount = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.txtAliveCount = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.txtWinCon = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.lblStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.menuStrip1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.tab2.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tableLayoutPanel4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataPlayers)).BeginInit();
            this.tableLayoutPanel3.SuspendLayout();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataTeams)).BeginInit();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(770, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveGameToolStripMenuItem,
            this.loadGameToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // saveGameToolStripMenuItem
            // 
            this.saveGameToolStripMenuItem.Name = "saveGameToolStripMenuItem";
            this.saveGameToolStripMenuItem.Size = new System.Drawing.Size(100, 22);
            this.saveGameToolStripMenuItem.Text = "Save";
            this.saveGameToolStripMenuItem.Click += new System.EventHandler(this.saveGameToolStripMenuItem_Click);
            // 
            // loadGameToolStripMenuItem
            // 
            this.loadGameToolStripMenuItem.Name = "loadGameToolStripMenuItem";
            this.loadGameToolStripMenuItem.Size = new System.Drawing.Size(100, 22);
            this.loadGameToolStripMenuItem.Text = "Load";
            this.loadGameToolStripMenuItem.Click += new System.EventHandler(this.loadGameToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuPlayerList,
            this.sendPMToolStripMenuItem,
            this.lockThreadToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(59, 20);
            this.helpToolStripMenuItem.Text = "Actions";
            // 
            // menuPlayerList
            // 
            this.menuPlayerList.Name = "menuPlayerList";
            this.menuPlayerList.Size = new System.Drawing.Size(211, 22);
            this.menuPlayerList.Text = "Paste Player List in Thread";
            this.menuPlayerList.Click += new System.EventHandler(this.menuPlayerList_Click);
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
            this.lockThreadToolStripMenuItem.Click += new System.EventHandler(this.lockThreadToolStripMenuItem_Click);
            // 
            // saveFileDialog
            // 
            this.saveFileDialog.DefaultExt = "json";
            this.saveFileDialog.Filter = "JSON|*.json*|All files|*.*";
            // 
            // openFileDialog
            // 
            this.openFileDialog.DefaultExt = "json";
            this.openFileDialog.Filter = "JSON|*.json*|All files|*.*";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.tab2, 0, 2);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 24);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(770, 495);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.txtGameURL, 1, 0);
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 2;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(500, 27);
            this.tableLayoutPanel2.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(57, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "GameURL";
            // 
            // txtGameURL
            // 
            this.txtGameURL.Location = new System.Drawing.Point(66, 3);
            this.txtGameURL.Name = "txtGameURL";
            this.txtGameURL.Size = new System.Drawing.Size(333, 20);
            this.txtGameURL.TabIndex = 1;
            // 
            // tab2
            // 
            this.tab2.Controls.Add(this.tabPage1);
            this.tab2.Controls.Add(this.tabPage2);
            this.tab2.Location = new System.Drawing.Point(3, 36);
            this.tab2.Name = "tab2";
            this.tab2.SelectedIndex = 0;
            this.tab2.Size = new System.Drawing.Size(767, 457);
            this.tab2.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.tableLayoutPanel4);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(759, 431);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Players";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel4
            // 
            this.tableLayoutPanel4.ColumnCount = 1;
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel4.Controls.Add(this.dataPlayers, 0, 1);
            this.tableLayoutPanel4.Controls.Add(this.tableLayoutPanel3, 0, 0);
            this.tableLayoutPanel4.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            this.tableLayoutPanel4.RowCount = 2;
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel4.Size = new System.Drawing.Size(763, 412);
            this.tableLayoutPanel4.TabIndex = 3;
            // 
            // dataPlayers
            // 
            this.dataPlayers.AllowUserToAddRows = false;
            this.dataPlayers.AllowUserToDeleteRows = false;
            this.dataPlayers.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataPlayers.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.boxSelect,
            this.txtPlayerName,
            this.boxTeam,
            this.txtRole,
            this.boxPlayerColor,
            this.boxAlive,
            this.txtDeathReason});
            this.dataPlayers.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataPlayers.Location = new System.Drawing.Point(3, 39);
            this.dataPlayers.Name = "dataPlayers";
            this.dataPlayers.Size = new System.Drawing.Size(757, 370);
            this.dataPlayers.TabIndex = 0;
            this.dataPlayers.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataPlayers_CellValueChanged);
            // 
            // boxSelect
            // 
            this.boxSelect.HeaderText = "";
            this.boxSelect.Name = "boxSelect";
            this.boxSelect.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.boxSelect.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.boxSelect.Width = 20;
            // 
            // txtPlayerName
            // 
            this.txtPlayerName.DataPropertyName = "Name";
            this.txtPlayerName.HeaderText = "Name";
            this.txtPlayerName.Name = "txtPlayerName";
            this.txtPlayerName.ReadOnly = true;
            // 
            // boxTeam
            // 
            this.boxTeam.HeaderText = "Team";
            this.boxTeam.Name = "boxTeam";
            this.boxTeam.ReadOnly = true;
            this.boxTeam.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.boxTeam.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // txtRole
            // 
            this.txtRole.HeaderText = "Role";
            this.txtRole.Name = "txtRole";
            this.txtRole.ReadOnly = true;
            // 
            // boxPlayerColor
            // 
            this.boxPlayerColor.HeaderText = "Color";
            this.boxPlayerColor.Items.AddRange(new object[] {
            "black",
            "green",
            "purple",
            "red",
            "blue",
            "yellow",
            "orange"});
            this.boxPlayerColor.Name = "boxPlayerColor";
            // 
            // boxAlive
            // 
            this.boxAlive.HeaderText = "Alive?";
            this.boxAlive.Items.AddRange(new object[] {
            "True",
            "False"});
            this.boxAlive.Name = "boxAlive";
            // 
            // txtDeathReason
            // 
            this.txtDeathReason.HeaderText = "Death Reason";
            this.txtDeathReason.Name = "txtDeathReason";
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 5;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel3.Controls.Add(this.btnSelectAll, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.btnDeathReveal, 1, 0);
            this.tableLayoutPanel3.Controls.Add(this.btnSubOut, 2, 0);
            this.tableLayoutPanel3.Controls.Add(this.btnPeek, 3, 0);
            this.tableLayoutPanel3.Controls.Add(this.btnResend, 4, 0);
            this.tableLayoutPanel3.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 1;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(500, 30);
            this.tableLayoutPanel3.TabIndex = 2;
            // 
            // btnSelectAll
            // 
            this.btnSelectAll.Location = new System.Drawing.Point(3, 3);
            this.btnSelectAll.Name = "btnSelectAll";
            this.btnSelectAll.Size = new System.Drawing.Size(74, 23);
            this.btnSelectAll.TabIndex = 0;
            this.btnSelectAll.Text = "Select All";
            this.btnSelectAll.UseVisualStyleBackColor = true;
            this.btnSelectAll.Click += new System.EventHandler(this.btnSelectAll_Click);
            // 
            // btnDeathReveal
            // 
            this.btnDeathReveal.Location = new System.Drawing.Point(83, 3);
            this.btnDeathReveal.Name = "btnDeathReveal";
            this.btnDeathReveal.Size = new System.Drawing.Size(113, 23);
            this.btnDeathReveal.TabIndex = 1;
            this.btnDeathReveal.Text = "Post Death Reveals";
            this.btnDeathReveal.UseVisualStyleBackColor = true;
            this.btnDeathReveal.Click += new System.EventHandler(this.btnDeathReveal_Click);
            // 
            // btnSubOut
            // 
            this.btnSubOut.Location = new System.Drawing.Point(202, 3);
            this.btnSubOut.Name = "btnSubOut";
            this.btnSubOut.Size = new System.Drawing.Size(71, 23);
            this.btnSubOut.TabIndex = 2;
            this.btnSubOut.Text = "Substitute";
            this.btnSubOut.UseVisualStyleBackColor = true;
            this.btnSubOut.Click += new System.EventHandler(this.btnSubOut_Click);
            // 
            // btnPeek
            // 
            this.btnPeek.Location = new System.Drawing.Point(279, 3);
            this.btnPeek.Name = "btnPeek";
            this.btnPeek.Size = new System.Drawing.Size(75, 23);
            this.btnPeek.TabIndex = 3;
            this.btnPeek.Text = "Peek Players";
            this.btnPeek.UseVisualStyleBackColor = true;
            this.btnPeek.Click += new System.EventHandler(this.btnPeek_Click);
            // 
            // btnResend
            // 
            this.btnResend.Location = new System.Drawing.Point(360, 3);
            this.btnResend.Name = "btnResend";
            this.btnResend.Size = new System.Drawing.Size(81, 23);
            this.btnResend.TabIndex = 4;
            this.btnResend.Text = "Resend Role PMs";
            this.btnResend.UseVisualStyleBackColor = true;
            this.btnResend.Click += new System.EventHandler(this.btnResend_Click);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.dataTeams);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(759, 431);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Teams";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // dataTeams
            // 
            this.dataTeams.AllowUserToAddRows = false;
            this.dataTeams.AllowUserToDeleteRows = false;
            this.dataTeams.AllowUserToOrderColumns = true;
            this.dataTeams.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataTeams.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.txtTeam,
            this.boxColor,
            this.txtPlayerCount,
            this.txtAliveCount,
            this.txtWinCon});
            this.dataTeams.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataTeams.Location = new System.Drawing.Point(3, 3);
            this.dataTeams.Name = "dataTeams";
            this.dataTeams.Size = new System.Drawing.Size(753, 425);
            this.dataTeams.TabIndex = 0;
            this.dataTeams.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataTeams_CellValueChanged);
            // 
            // txtTeam
            // 
            this.txtTeam.HeaderText = "Team Name";
            this.txtTeam.Name = "txtTeam";
            // 
            // boxColor
            // 
            this.boxColor.HeaderText = "Color";
            this.boxColor.Items.AddRange(new object[] {
            "black",
            "green",
            "purple",
            "red",
            "blue",
            "yellow",
            "orange"});
            this.boxColor.Name = "boxColor";
            // 
            // txtPlayerCount
            // 
            this.txtPlayerCount.HeaderText = "Player Count";
            this.txtPlayerCount.Name = "txtPlayerCount";
            this.txtPlayerCount.ReadOnly = true;
            // 
            // txtAliveCount
            // 
            this.txtAliveCount.HeaderText = "Alive Players";
            this.txtAliveCount.Name = "txtAliveCount";
            this.txtAliveCount.ReadOnly = true;
            // 
            // txtWinCon
            // 
            this.txtWinCon.HeaderText = "Win Condition";
            this.txtWinCon.Name = "txtWinCon";
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblStatus});
            this.statusStrip1.Location = new System.Drawing.Point(0, 497);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(770, 22);
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // lblStatus
            // 
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(0, 17);
            // 
            // Meerkat
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(770, 519);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.menuStrip1);
            this.Name = "Meerkat";
            this.Text = "Mary Meerkat, Modder";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Meerkat_FormClosing);
            this.Load += new System.EventHandler(this.Meerkat_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.tab2.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tableLayoutPanel4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataPlayers)).EndInit();
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataTeams)).EndInit();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem menuPlayerList;
        private System.Windows.Forms.ToolStripMenuItem saveGameToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadGameToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sendPMToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem lockThreadToolStripMenuItem;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TabControl tab2;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.DataGridView dataPlayers;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.DataGridView dataTeams;
        private System.Windows.Forms.DataGridViewTextBoxColumn txtTeam;
        private System.Windows.Forms.DataGridViewComboBoxColumn boxColor;
        private System.Windows.Forms.DataGridViewTextBoxColumn txtPlayerCount;
        private System.Windows.Forms.DataGridViewTextBoxColumn txtAliveCount;
        private System.Windows.Forms.DataGridViewTextBoxColumn txtWinCon;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtGameURL;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel lblStatus;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.Button btnSelectAll;
        private System.Windows.Forms.Button btnDeathReveal;
        private System.Windows.Forms.Button btnSubOut;
        private System.Windows.Forms.Button btnPeek;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
        private System.Windows.Forms.DataGridViewCheckBoxColumn boxSelect;
        private System.Windows.Forms.DataGridViewTextBoxColumn txtPlayerName;
        private System.Windows.Forms.DataGridViewTextBoxColumn boxTeam;
        private System.Windows.Forms.DataGridViewTextBoxColumn txtRole;
        private System.Windows.Forms.DataGridViewComboBoxColumn boxPlayerColor;
        private System.Windows.Forms.DataGridViewComboBoxColumn boxAlive;
        private System.Windows.Forms.DataGridViewTextBoxColumn txtDeathReason;
        private System.Windows.Forms.Button btnResend;
    }
}

