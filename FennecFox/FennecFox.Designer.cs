namespace POG.FennecFox
{
    partial class FormVoteCounter
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormVoteCounter));
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.mnuHide = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuUnhide = new System.Windows.Forms.ToolStripMenuItem();
            this.statusBrowser = new System.Windows.Forms.StatusStrip();
            this.statusText = new System.Windows.Forms.ToolStripStatusLabel();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.txtVersion = new System.Windows.Forms.TextBox();
            this.btnNewPlayerList = new System.Windows.Forms.Button();
            this.btnReset = new System.Windows.Forms.Button();
            this.btnStartGame = new System.Windows.Forms.Button();
            this.URLTextBox = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.btnLogout = new System.Windows.Forms.Button();
            this.btnLogin = new System.Windows.Forms.Button();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.label15 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.txtUsername = new System.Windows.Forms.TextBox();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.btnUnignore = new System.Windows.Forms.Button();
            this.btnIgnore = new System.Windows.Forms.Button();
            this.label18 = new System.Windows.Forms.Label();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.grdVotes = new System.Windows.Forms.DataGridView();
            this.grdVotes2 = new System.Windows.Forms.DataGridView();
            this.txtPostTable = new System.Windows.Forms.TextBox();
            this.txtCountDown = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtLastPost = new System.Windows.Forms.TextBox();
            this.txtEndPost = new System.Windows.Forms.TextBox();
            this.btnGetPosts = new System.Windows.Forms.Button();
            this.tabVotes = new System.Windows.Forms.TabControl();
            this.udStartPost = new System.Windows.Forms.TextBox();
            this.dtStartTime = new System.Windows.Forms.TextBox();
            this.dtEndTime = new System.Windows.Forms.TextBox();
            this.btnEditDay = new System.Windows.Forms.Button();
            this.contextMenuStrip1.SuspendLayout();
            this.statusBrowser.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdVotes)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdVotes2)).BeginInit();
            this.tabVotes.SuspendLayout();
            this.SuspendLayout();
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuHide,
            this.mnuUnhide});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(155, 48);
            // 
            // mnuHide
            // 
            this.mnuHide.Name = "mnuHide";
            this.mnuHide.ShortcutKeyDisplayString = "";
            this.mnuHide.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.H)));
            this.mnuHide.Size = new System.Drawing.Size(154, 22);
            this.mnuHide.Text = "Hide";
            this.mnuHide.Click += new System.EventHandler(this.mnuHide_Click);
            // 
            // mnuUnhide
            // 
            this.mnuUnhide.Name = "mnuUnhide";
            this.mnuUnhide.ShortcutKeyDisplayString = "";
            this.mnuUnhide.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.U)));
            this.mnuUnhide.Size = new System.Drawing.Size(154, 22);
            this.mnuUnhide.Text = "Unhide";
            this.mnuUnhide.Click += new System.EventHandler(this.mnuUnhide_Click);
            // 
            // statusBrowser
            // 
            this.statusBrowser.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusText});
            this.statusBrowser.Location = new System.Drawing.Point(0, 644);
            this.statusBrowser.Name = "statusBrowser";
            this.statusBrowser.Size = new System.Drawing.Size(654, 22);
            this.statusBrowser.TabIndex = 14;
            this.statusBrowser.Text = "statusStrip1";
            // 
            // statusText
            // 
            this.statusText.Name = "statusText";
            this.statusText.Size = new System.Drawing.Size(38, 17);
            this.statusText.Text = "status";
            // 
            // tabPage2
            // 
            this.tabPage2.BackColor = System.Drawing.Color.Gainsboro;
            this.tabPage2.Controls.Add(this.txtVersion);
            this.tabPage2.Controls.Add(this.btnNewPlayerList);
            this.tabPage2.Controls.Add(this.btnReset);
            this.tabPage2.Controls.Add(this.btnStartGame);
            this.tabPage2.Controls.Add(this.URLTextBox);
            this.tabPage2.Controls.Add(this.label4);
            this.tabPage2.Controls.Add(this.btnLogout);
            this.tabPage2.Controls.Add(this.btnLogin);
            this.tabPage2.Controls.Add(this.tableLayoutPanel2);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(646, 618);
            this.tabPage2.TabIndex = 5;
            this.tabPage2.Text = "Choose Game";
            // 
            // txtVersion
            // 
            this.txtVersion.Location = new System.Drawing.Point(183, 114);
            this.txtVersion.Name = "txtVersion";
            this.txtVersion.ReadOnly = true;
            this.txtVersion.Size = new System.Drawing.Size(291, 20);
            this.txtVersion.TabIndex = 46;
            // 
            // btnNewPlayerList
            // 
            this.btnNewPlayerList.Location = new System.Drawing.Point(212, 219);
            this.btnNewPlayerList.Name = "btnNewPlayerList";
            this.btnNewPlayerList.Size = new System.Drawing.Size(156, 23);
            this.btnNewPlayerList.TabIndex = 45;
            this.btnNewPlayerList.Text = "New Player List...";
            this.btnNewPlayerList.UseVisualStyleBackColor = true;
            this.btnNewPlayerList.Click += new System.EventHandler(this.btnNewPlayerList_Click);
            // 
            // btnReset
            // 
            this.btnReset.Location = new System.Drawing.Point(293, 190);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(75, 23);
            this.btnReset.TabIndex = 44;
            this.btnReset.Text = "Reset All";
            this.btnReset.UseVisualStyleBackColor = true;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // btnStartGame
            // 
            this.btnStartGame.Enabled = false;
            this.btnStartGame.Location = new System.Drawing.Point(212, 190);
            this.btnStartGame.Name = "btnStartGame";
            this.btnStartGame.Size = new System.Drawing.Size(75, 23);
            this.btnStartGame.TabIndex = 43;
            this.btnStartGame.Text = "Start";
            this.btnStartGame.UseVisualStyleBackColor = true;
            this.btnStartGame.Click += new System.EventHandler(this.btnStartGame_Click);
            // 
            // URLTextBox
            // 
            this.URLTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.URLTextBox.Location = new System.Drawing.Point(21, 167);
            this.URLTextBox.Name = "URLTextBox";
            this.URLTextBox.Size = new System.Drawing.Size(599, 20);
            this.URLTextBox.TabIndex = 42;
            this.URLTextBox.TextChanged += new System.EventHandler(this.URLTextBox_TextChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(18, 151);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(92, 13);
            this.label4.TabIndex = 41;
            this.label4.Text = "Main Thread URL";
            // 
            // btnLogout
            // 
            this.btnLogout.Enabled = false;
            this.btnLogout.Location = new System.Drawing.Point(102, 111);
            this.btnLogout.Name = "btnLogout";
            this.btnLogout.Size = new System.Drawing.Size(75, 23);
            this.btnLogout.TabIndex = 4;
            this.btnLogout.Text = "Logout";
            this.btnLogout.UseVisualStyleBackColor = true;
            this.btnLogout.Click += new System.EventHandler(this.btnLogout_Click);
            // 
            // btnLogin
            // 
            this.btnLogin.Location = new System.Drawing.Point(21, 111);
            this.btnLogin.Name = "btnLogin";
            this.btnLogin.Size = new System.Drawing.Size(75, 23);
            this.btnLogin.TabIndex = 3;
            this.btnLogin.Text = "Login";
            this.btnLogin.UseVisualStyleBackColor = true;
            this.btnLogin.Click += new System.EventHandler(this.btnLogin_Click);
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 28.11388F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 71.88612F));
            this.tableLayoutPanel2.Controls.Add(this.label15, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.label16, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.txtPassword, 1, 1);
            this.tableLayoutPanel2.Controls.Add(this.txtUsername, 1, 0);
            this.tableLayoutPanel2.Location = new System.Drawing.Point(21, 18);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 4;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(281, 87);
            this.tableLayoutPanel2.TabIndex = 0;
            // 
            // label15
            // 
            this.label15.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(3, 6);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(73, 13);
            this.label15.TabIndex = 0;
            this.label15.Text = "Username";
            // 
            // label16
            // 
            this.label16.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(3, 32);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(73, 13);
            this.label16.TabIndex = 1;
            this.label16.Text = "Password";
            // 
            // txtPassword
            // 
            this.txtPassword.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtPassword.Location = new System.Drawing.Point(82, 29);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.Size = new System.Drawing.Size(196, 20);
            this.txtPassword.TabIndex = 2;
            // 
            // txtUsername
            // 
            this.txtUsername.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtUsername.Location = new System.Drawing.Point(82, 3);
            this.txtUsername.Name = "txtUsername";
            this.txtUsername.Size = new System.Drawing.Size(196, 20);
            this.txtUsername.TabIndex = 1;
            // 
            // tabPage4
            // 
            this.tabPage4.BackColor = System.Drawing.Color.Gainsboro;
            this.tabPage4.Controls.Add(this.tableLayoutPanel1);
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4.Size = new System.Drawing.Size(646, 618);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "Get Votes";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 8;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.btnUnignore, 1, 5);
            this.tableLayoutPanel1.Controls.Add(this.btnIgnore, 0, 5);
            this.tableLayoutPanel1.Controls.Add(this.label18, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.splitContainer1, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.txtCountDown, 6, 5);
            this.tableLayoutPanel1.Controls.Add(this.label2, 6, 2);
            this.tableLayoutPanel1.Controls.Add(this.txtLastPost, 7, 2);
            this.tableLayoutPanel1.Controls.Add(this.txtEndPost, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.btnGetPosts, 6, 1);
            this.tableLayoutPanel1.Controls.Add(this.udStartPost, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.dtStartTime, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.dtEndTime, 2, 2);
            this.tableLayoutPanel1.Controls.Add(this.btnEditDay, 4, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 6;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(640, 612);
            this.tableLayoutPanel1.TabIndex = 17;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(47, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "Start";
            // 
            // btnUnignore
            // 
            this.btnUnignore.Location = new System.Drawing.Point(56, 585);
            this.btnUnignore.Name = "btnUnignore";
            this.btnUnignore.Size = new System.Drawing.Size(58, 23);
            this.btnUnignore.TabIndex = 15;
            this.btnUnignore.Text = "Unhide";
            this.btnUnignore.UseVisualStyleBackColor = true;
            this.btnUnignore.Click += new System.EventHandler(this.btnUnignore_Click);
            // 
            // btnIgnore
            // 
            this.btnIgnore.Location = new System.Drawing.Point(3, 585);
            this.btnIgnore.Name = "btnIgnore";
            this.btnIgnore.Size = new System.Drawing.Size(47, 23);
            this.btnIgnore.TabIndex = 10;
            this.btnIgnore.Text = "Hide";
            this.btnIgnore.UseVisualStyleBackColor = true;
            this.btnIgnore.Click += new System.EventHandler(this.btnIgnore_Click);
            // 
            // label18
            // 
            this.label18.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(3, 35);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(47, 13);
            this.label18.TabIndex = 21;
            this.label18.Text = "End";
            // 
            // splitContainer1
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.splitContainer1, 12);
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(3, 58);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.grdVotes);
            this.splitContainer1.Panel1.Controls.Add(this.grdVotes2);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.txtPostTable);
            this.tableLayoutPanel1.SetRowSpan(this.splitContainer1, 2);
            this.splitContainer1.Size = new System.Drawing.Size(637, 521);
            this.splitContainer1.SplitterDistance = 254;
            this.splitContainer1.TabIndex = 22;
            // 
            // grdVotes
            // 
            this.grdVotes.AllowUserToAddRows = false;
            this.grdVotes.AllowUserToDeleteRows = false;
            this.grdVotes.AllowUserToOrderColumns = true;
            this.grdVotes.AllowUserToResizeRows = false;
            this.grdVotes.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.grdVotes.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.grdVotes.ContextMenuStrip = this.contextMenuStrip1;
            this.grdVotes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grdVotes.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.grdVotes.Location = new System.Drawing.Point(0, 0);
            this.grdVotes.MultiSelect = false;
            this.grdVotes.Name = "grdVotes";
            this.grdVotes.RowHeadersVisible = false;
            this.grdVotes.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.grdVotes.ShowEditingIcon = false;
            this.grdVotes.Size = new System.Drawing.Size(637, 254);
            this.grdVotes.TabIndex = 19;
            // 
            // grdVotes2
            // 
            this.grdVotes2.AllowUserToAddRows = false;
            this.grdVotes2.AllowUserToDeleteRows = false;
            this.grdVotes2.AllowUserToOrderColumns = true;
            this.grdVotes2.AllowUserToResizeRows = false;
            this.grdVotes2.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.grdVotes2.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.grdVotes2.ContextMenuStrip = this.contextMenuStrip1;
            this.grdVotes2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grdVotes2.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.grdVotes2.Location = new System.Drawing.Point(0, 0);
            this.grdVotes2.MultiSelect = false;
            this.grdVotes2.Name = "grdVotes2";
            this.grdVotes2.RowHeadersVisible = false;
            this.grdVotes2.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.grdVotes2.ShowEditingIcon = false;
            this.grdVotes2.Size = new System.Drawing.Size(637, 254);
            this.grdVotes2.TabIndex = 18;
            this.grdVotes2.Visible = false;
            // 
            // txtPostTable
            // 
            this.txtPostTable.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtPostTable.Location = new System.Drawing.Point(0, 0);
            this.txtPostTable.Multiline = true;
            this.txtPostTable.Name = "txtPostTable";
            this.txtPostTable.ReadOnly = true;
            this.txtPostTable.Size = new System.Drawing.Size(637, 263);
            this.txtPostTable.TabIndex = 19;
            this.txtPostTable.Click += new System.EventHandler(this.txtPostTable_Click);
            // 
            // txtCountDown
            // 
            this.txtCountDown.Location = new System.Drawing.Point(454, 585);
            this.txtCountDown.Name = "txtCountDown";
            this.txtCountDown.ReadOnly = true;
            this.txtCountDown.Size = new System.Drawing.Size(100, 20);
            this.txtCountDown.TabIndex = 27;
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(454, 35);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(100, 13);
            this.label2.TabIndex = 8;
            this.label2.Text = "Last Post";
            // 
            // txtLastPost
            // 
            this.txtLastPost.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtLastPost.Location = new System.Drawing.Point(560, 32);
            this.txtLastPost.Name = "txtLastPost";
            this.txtLastPost.ReadOnly = true;
            this.txtLastPost.Size = new System.Drawing.Size(80, 20);
            this.txtLastPost.TabIndex = 3;
            this.txtLastPost.TabStop = false;
            this.txtLastPost.Text = "0";
            // 
            // txtEndPost
            // 
            this.txtEndPost.Location = new System.Drawing.Point(56, 32);
            this.txtEndPost.Name = "txtEndPost";
            this.txtEndPost.ReadOnly = true;
            this.txtEndPost.Size = new System.Drawing.Size(100, 20);
            this.txtEndPost.TabIndex = 32;
            // 
            // btnGetPosts
            // 
            this.btnGetPosts.Location = new System.Drawing.Point(454, 2);
            this.btnGetPosts.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnGetPosts.Name = "btnGetPosts";
            this.btnGetPosts.Size = new System.Drawing.Size(64, 19);
            this.btnGetPosts.TabIndex = 33;
            this.btnGetPosts.Text = "Get Posts";
            this.btnGetPosts.UseVisualStyleBackColor = true;
            this.btnGetPosts.Click += new System.EventHandler(this.btnGetPosts_Click);
            // 
            // tabVotes
            // 
            this.tabVotes.Controls.Add(this.tabPage4);
            this.tabVotes.Controls.Add(this.tabPage2);
            this.tabVotes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabVotes.Location = new System.Drawing.Point(0, 0);
            this.tabVotes.Name = "tabVotes";
            this.tabVotes.SelectedIndex = 0;
            this.tabVotes.Size = new System.Drawing.Size(654, 644);
            this.tabVotes.TabIndex = 15;
            // 
            // udStartPost
            // 
            this.udStartPost.Location = new System.Drawing.Point(56, 3);
            this.udStartPost.Name = "udStartPost";
            this.udStartPost.ReadOnly = true;
            this.udStartPost.Size = new System.Drawing.Size(100, 20);
            this.udStartPost.TabIndex = 34;
            // 
            // dtStartTime
            // 
            this.dtStartTime.Location = new System.Drawing.Point(162, 3);
            this.dtStartTime.Name = "dtStartTime";
            this.dtStartTime.ReadOnly = true;
            this.dtStartTime.Size = new System.Drawing.Size(100, 20);
            this.dtStartTime.TabIndex = 35;
            // 
            // dtEndTime
            // 
            this.dtEndTime.Location = new System.Drawing.Point(162, 32);
            this.dtEndTime.Name = "dtEndTime";
            this.dtEndTime.ReadOnly = true;
            this.dtEndTime.Size = new System.Drawing.Size(100, 20);
            this.dtEndTime.TabIndex = 36;
            // 
            // btnEditDay
            // 
            this.btnEditDay.Location = new System.Drawing.Point(320, 3);
            this.btnEditDay.Name = "btnEditDay";
            this.btnEditDay.Size = new System.Drawing.Size(75, 23);
            this.btnEditDay.TabIndex = 37;
            this.btnEditDay.Text = "Edit Day...";
            this.btnEditDay.UseVisualStyleBackColor = true;
            this.btnEditDay.Click += new System.EventHandler(this.btnEditDay_Click);
            // 
            // FormVoteCounter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(654, 666);
            this.Controls.Add(this.tabVotes);
            this.Controls.Add(this.statusBrowser);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FormVoteCounter";
            this.Text = "Fennec Fox Vote Counter";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form_FormClosing);
            this.contextMenuStrip1.ResumeLayout(false);
            this.statusBrowser.ResumeLayout(false);
            this.statusBrowser.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.tabPage4.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grdVotes)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdVotes2)).EndInit();
            this.tabVotes.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem mnuHide;
        private System.Windows.Forms.ToolStripMenuItem mnuUnhide;
        private System.Windows.Forms.StatusStrip statusBrowser;
        private System.Windows.Forms.ToolStripStatusLabel statusText;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Button btnNewPlayerList;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.Button btnStartGame;
        private System.Windows.Forms.TextBox URLTextBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnLogout;
        private System.Windows.Forms.Button btnLogin;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.TextBox txtUsername;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnUnignore;
        private System.Windows.Forms.Button btnIgnore;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.DataGridView grdVotes;
        private System.Windows.Forms.DataGridView grdVotes2;
        private System.Windows.Forms.TextBox txtPostTable;
        private System.Windows.Forms.TextBox txtCountDown;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtLastPost;
        private System.Windows.Forms.TextBox txtEndPost;
        private System.Windows.Forms.Button btnGetPosts;
        private System.Windows.Forms.TabControl tabVotes;
        private System.Windows.Forms.TextBox txtVersion;
        private System.Windows.Forms.TextBox udStartPost;
        private System.Windows.Forms.TextBox dtStartTime;
        private System.Windows.Forms.TextBox dtEndTime;
        private System.Windows.Forms.Button btnEditDay;


    }
}

