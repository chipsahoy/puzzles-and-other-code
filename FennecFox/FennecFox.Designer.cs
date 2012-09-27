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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.btnUnignore = new System.Windows.Forms.Button();
            this.btnIgnore = new System.Windows.Forms.Button();
            this.label18 = new System.Windows.Forms.Label();
            this.grdVotes = new System.Windows.Forms.DataGridView();
            this.txtCountDown = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtLastPost = new System.Windows.Forms.TextBox();
            this.txtEndPost = new System.Windows.Forms.TextBox();
            this.udStartPost = new System.Windows.Forms.TextBox();
            this.dtStartTime = new System.Windows.Forms.TextBox();
            this.dtEndTime = new System.Windows.Forms.TextBox();
            this.btnGetPosts = new System.Windows.Forms.Button();
            this.btnEditDay = new System.Windows.Forms.Button();
            this.btnRoster = new System.Windows.Forms.Button();
            this.udDay = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.btnPostIt = new System.Windows.Forms.Button();
            this.btnCopyIt = new System.Windows.Forms.Button();
            this.lblDaysToEOD = new System.Windows.Forms.Label();
            this.contextMenuStrip1.SuspendLayout();
            this.statusBrowser.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdVotes)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.udDay)).BeginInit();
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
            this.statusBrowser.Location = new System.Drawing.Point(0, 388);
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
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 9;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.label18, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.txtEndPost, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.udStartPost, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.dtStartTime, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.dtEndTime, 2, 2);
            this.tableLayoutPanel1.Controls.Add(this.btnEditDay, 3, 1);
            this.tableLayoutPanel1.Controls.Add(this.btnRoster, 3, 2);
            this.tableLayoutPanel1.Controls.Add(this.label3, 4, 1);
            this.tableLayoutPanel1.Controls.Add(this.label2, 0, 5);
            this.tableLayoutPanel1.Controls.Add(this.txtLastPost, 1, 5);
            this.tableLayoutPanel1.Controls.Add(this.btnIgnore, 2, 5);
            this.tableLayoutPanel1.Controls.Add(this.btnUnignore, 3, 5);
            this.tableLayoutPanel1.Controls.Add(this.udDay, 4, 2);
            this.tableLayoutPanel1.Controls.Add(this.txtCountDown, 8, 5);
            this.tableLayoutPanel1.Controls.Add(this.btnGetPosts, 7, 1);
            this.tableLayoutPanel1.Controls.Add(this.btnPostIt, 8, 1);
            this.tableLayoutPanel1.Controls.Add(this.btnCopyIt, 8, 2);
            this.tableLayoutPanel1.Controls.Add(this.grdVotes, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.lblDaysToEOD, 7, 5);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 6;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 1F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(654, 388);
            this.tableLayoutPanel1.TabIndex = 18;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(51, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "Start";
            this.label1.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // btnUnignore
            // 
            this.btnUnignore.Location = new System.Drawing.Point(299, 361);
            this.btnUnignore.Name = "btnUnignore";
            this.btnUnignore.Size = new System.Drawing.Size(65, 23);
            this.btnUnignore.TabIndex = 15;
            this.btnUnignore.Text = "Unhide";
            this.btnUnignore.UseVisualStyleBackColor = true;
            this.btnUnignore.Click += new System.EventHandler(this.btnUnignore_Click);
            // 
            // btnIgnore
            // 
            this.btnIgnore.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnIgnore.Location = new System.Drawing.Point(228, 361);
            this.btnIgnore.Name = "btnIgnore";
            this.btnIgnore.Size = new System.Drawing.Size(65, 23);
            this.btnIgnore.TabIndex = 10;
            this.btnIgnore.Text = "Hide";
            this.btnIgnore.UseVisualStyleBackColor = true;
            this.btnIgnore.Click += new System.EventHandler(this.btnIgnore_Click);
            // 
            // label18
            // 
            this.label18.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(3, 37);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(51, 13);
            this.label18.TabIndex = 21;
            this.label18.Text = "End";
            this.label18.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // grdVotes
            // 
            this.grdVotes.AllowUserToAddRows = false;
            this.grdVotes.AllowUserToDeleteRows = false;
            this.grdVotes.AllowUserToOrderColumns = true;
            this.grdVotes.AllowUserToResizeRows = false;
            this.grdVotes.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.grdVotes.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.tableLayoutPanel1.SetColumnSpan(this.grdVotes, 9);
            this.grdVotes.ContextMenuStrip = this.contextMenuStrip1;
            this.grdVotes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grdVotes.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.grdVotes.Location = new System.Drawing.Point(3, 61);
            this.grdVotes.MultiSelect = false;
            this.grdVotes.Name = "grdVotes";
            this.grdVotes.RowHeadersVisible = false;
            this.grdVotes.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.grdVotes.ShowEditingIcon = false;
            this.grdVotes.Size = new System.Drawing.Size(648, 293);
            this.grdVotes.TabIndex = 19;
            // 
            // txtCountDown
            // 
            this.txtCountDown.Location = new System.Drawing.Point(574, 361);
            this.txtCountDown.Name = "txtCountDown";
            this.txtCountDown.ReadOnly = true;
            this.txtCountDown.Size = new System.Drawing.Size(65, 20);
            this.txtCountDown.TabIndex = 27;
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 366);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(51, 13);
            this.label2.TabIndex = 8;
            this.label2.Text = "Last Post";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtLastPost
            // 
            this.txtLastPost.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtLastPost.Location = new System.Drawing.Point(60, 363);
            this.txtLastPost.Name = "txtLastPost";
            this.txtLastPost.ReadOnly = true;
            this.txtLastPost.Size = new System.Drawing.Size(107, 20);
            this.txtLastPost.TabIndex = 3;
            this.txtLastPost.TabStop = false;
            this.txtLastPost.Text = "0";
            // 
            // txtEndPost
            // 
            this.txtEndPost.Location = new System.Drawing.Point(60, 32);
            this.txtEndPost.Name = "txtEndPost";
            this.txtEndPost.ReadOnly = true;
            this.txtEndPost.Size = new System.Drawing.Size(100, 20);
            this.txtEndPost.TabIndex = 32;
            // 
            // udStartPost
            // 
            this.udStartPost.Location = new System.Drawing.Point(60, 3);
            this.udStartPost.Name = "udStartPost";
            this.udStartPost.ReadOnly = true;
            this.udStartPost.Size = new System.Drawing.Size(100, 20);
            this.udStartPost.TabIndex = 34;
            // 
            // dtStartTime
            // 
            this.dtStartTime.Location = new System.Drawing.Point(173, 3);
            this.dtStartTime.Name = "dtStartTime";
            this.dtStartTime.ReadOnly = true;
            this.dtStartTime.Size = new System.Drawing.Size(120, 20);
            this.dtStartTime.TabIndex = 35;
            // 
            // dtEndTime
            // 
            this.dtEndTime.Location = new System.Drawing.Point(173, 32);
            this.dtEndTime.Name = "dtEndTime";
            this.dtEndTime.ReadOnly = true;
            this.dtEndTime.Size = new System.Drawing.Size(120, 20);
            this.dtEndTime.TabIndex = 36;
            // 
            // btnGetPosts
            // 
            this.btnGetPosts.Location = new System.Drawing.Point(503, 2);
            this.btnGetPosts.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnGetPosts.Name = "btnGetPosts";
            this.btnGetPosts.Size = new System.Drawing.Size(65, 23);
            this.btnGetPosts.TabIndex = 33;
            this.btnGetPosts.Text = "Get Posts";
            this.btnGetPosts.UseVisualStyleBackColor = true;
            this.btnGetPosts.Click += new System.EventHandler(this.btnGetPosts_Click);
            // 
            // btnEditDay
            // 
            this.btnEditDay.Location = new System.Drawing.Point(299, 3);
            this.btnEditDay.Name = "btnEditDay";
            this.btnEditDay.Size = new System.Drawing.Size(65, 23);
            this.btnEditDay.TabIndex = 37;
            this.btnEditDay.Text = "Day...";
            this.btnEditDay.UseVisualStyleBackColor = true;
            this.btnEditDay.Click += new System.EventHandler(this.btnEditDay_Click);
            // 
            // btnRoster
            // 
            this.btnRoster.Location = new System.Drawing.Point(299, 32);
            this.btnRoster.Name = "btnRoster";
            this.btnRoster.Size = new System.Drawing.Size(65, 23);
            this.btnRoster.TabIndex = 38;
            this.btnRoster.Text = "Players...";
            this.btnRoster.UseVisualStyleBackColor = true;
            this.btnRoster.Click += new System.EventHandler(this.btnRoster_Click);
            // 
            // udDay
            // 
            this.udDay.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.udDay.Location = new System.Drawing.Point(370, 33);
            this.udDay.Maximum = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.udDay.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.udDay.Name = "udDay";
            this.udDay.Size = new System.Drawing.Size(37, 20);
            this.udDay.TabIndex = 39;
            this.udDay.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.udDay.ValueChanged += new System.EventHandler(this.udDay_ValueChanged);
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(370, 16);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(29, 13);
            this.label3.TabIndex = 40;
            this.label3.Text = "Day:";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // btnPostIt
            // 
            this.btnPostIt.Location = new System.Drawing.Point(574, 2);
            this.btnPostIt.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnPostIt.Name = "btnPostIt";
            this.btnPostIt.Size = new System.Drawing.Size(65, 23);
            this.btnPostIt.TabIndex = 41;
            this.btnPostIt.Text = "Post It";
            this.btnPostIt.UseVisualStyleBackColor = true;
            this.btnPostIt.Click += new System.EventHandler(this.btnPostIt_Click);
            // 
            // btnCopyIt
            // 
            this.btnCopyIt.Location = new System.Drawing.Point(574, 31);
            this.btnCopyIt.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnCopyIt.Name = "btnCopyIt";
            this.btnCopyIt.Size = new System.Drawing.Size(65, 23);
            this.btnCopyIt.TabIndex = 42;
            this.btnCopyIt.Text = "Copy It";
            this.btnCopyIt.UseVisualStyleBackColor = true;
            this.btnCopyIt.Click += new System.EventHandler(this.btnCopyIt_Click);
            // 
            // lblDaysToEOD
            // 
            this.lblDaysToEOD.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lblDaysToEOD.AutoSize = true;
            this.lblDaysToEOD.Location = new System.Drawing.Point(518, 366);
            this.lblDaysToEOD.Name = "lblDaysToEOD";
            this.lblDaysToEOD.Size = new System.Drawing.Size(50, 13);
            this.lblDaysToEOD.TabIndex = 43;
            this.lblDaysToEOD.Text = "days and";
            this.lblDaysToEOD.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lblDaysToEOD.Visible = false;
            // 
            // FormVoteCounter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(654, 410);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.statusBrowser);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FormVoteCounter";
            this.Text = "Fennec Fox Vote Counter";
            this.contextMenuStrip1.ResumeLayout(false);
            this.statusBrowser.ResumeLayout(false);
            this.statusBrowser.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdVotes)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.udDay)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem mnuHide;
        private System.Windows.Forms.ToolStripMenuItem mnuUnhide;
        private System.Windows.Forms.StatusStrip statusBrowser;
        private System.Windows.Forms.ToolStripStatusLabel statusText;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnUnignore;
        private System.Windows.Forms.Button btnIgnore;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.DataGridView grdVotes;
        private System.Windows.Forms.TextBox txtCountDown;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtLastPost;
        private System.Windows.Forms.TextBox txtEndPost;
        private System.Windows.Forms.TextBox udStartPost;
        private System.Windows.Forms.TextBox dtStartTime;
        private System.Windows.Forms.TextBox dtEndTime;
        private System.Windows.Forms.Button btnGetPosts;
        private System.Windows.Forms.Button btnEditDay;
        private System.Windows.Forms.Button btnRoster;
        private System.Windows.Forms.NumericUpDown udDay;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnPostIt;
        private System.Windows.Forms.Button btnCopyIt;
        private System.Windows.Forms.Label lblDaysToEOD;


    }
}

