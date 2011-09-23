namespace FennecFox
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormVoteCounter));
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.GoButtonAgain = new System.Windows.Forms.Button();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.listVotes = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.WebBrowserPage = new System.Windows.Forms.WebBrowser();
            this.btnIgnore = new System.Windows.Forms.Button();
            this.btnUnignore = new System.Windows.Forms.Button();
            this.textPostsPerPage = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtLastPost = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtFirstPost = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.URLTextBox = new System.Windows.Forms.TextBox();
            this.statusBrowser = new System.Windows.Forms.StatusStrip();
            this.statusText = new System.Windows.Forms.ToolStripStatusLabel();
            this.tabVotes = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.tabPage4.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.statusBrowser.SuspendLayout();
            this.tabVotes.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabPage4
            // 
            this.tabPage4.BackColor = System.Drawing.Color.Gainsboro;
            this.tabPage4.Controls.Add(this.tableLayoutPanel1);
            this.tabPage4.Controls.Add(this.statusBrowser);
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4.Size = new System.Drawing.Size(643, 587);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "Get Post Ids";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 11;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.GoButtonAgain, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.splitContainer1, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.btnIgnore, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.btnUnignore, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.textPostsPerPage, 9, 1);
            this.tableLayoutPanel1.Controls.Add(this.label1, 4, 1);
            this.tableLayoutPanel1.Controls.Add(this.txtLastPost, 7, 1);
            this.tableLayoutPanel1.Controls.Add(this.label3, 8, 1);
            this.tableLayoutPanel1.Controls.Add(this.txtFirstPost, 5, 1);
            this.tableLayoutPanel1.Controls.Add(this.label2, 6, 1);
            this.tableLayoutPanel1.Controls.Add(this.button1, 3, 1);
            this.tableLayoutPanel1.Controls.Add(this.label4, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.URLTextBox, 2, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(637, 559);
            this.tableLayoutPanel1.TabIndex = 17;
            // 
            // GoButtonAgain
            // 
            this.GoButtonAgain.Location = new System.Drawing.Point(3, 29);
            this.GoButtonAgain.Name = "GoButtonAgain";
            this.GoButtonAgain.Size = new System.Drawing.Size(56, 20);
            this.GoButtonAgain.TabIndex = 3;
            this.GoButtonAgain.Text = "Get Em";
            this.GoButtonAgain.UseVisualStyleBackColor = true;
            this.GoButtonAgain.Click += new System.EventHandler(this.GoButtonAgain_Click);
            // 
            // splitContainer1
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.splitContainer1, 11);
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.Location = new System.Drawing.Point(3, 75);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.listVotes);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.WebBrowserPage);
            this.splitContainer1.Size = new System.Drawing.Size(631, 481);
            this.splitContainer1.SplitterDistance = 272;
            this.splitContainer1.SplitterWidth = 6;
            this.splitContainer1.TabIndex = 16;
            // 
            // listVotes
            // 
            this.listVotes.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3});
            this.listVotes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listVotes.FullRowSelect = true;
            this.listVotes.HideSelection = false;
            this.listVotes.Location = new System.Drawing.Point(0, 0);
            this.listVotes.Name = "listVotes";
            this.listVotes.Size = new System.Drawing.Size(272, 481);
            this.listVotes.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.listVotes.TabIndex = 14;
            this.listVotes.UseCompatibleStateImageBehavior = false;
            this.listVotes.View = System.Windows.Forms.View.Details;
            this.listVotes.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.listVotes_ColumnClick);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Tag = "String";
            this.columnHeader1.Text = "Poster";
            this.columnHeader1.Width = 140;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Tag = "Numeric";
            this.columnHeader2.Text = "#";
            this.columnHeader2.Width = 50;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Tag = "String";
            this.columnHeader3.Text = "Bolded";
            this.columnHeader3.Width = 236;
            // 
            // WebBrowserPage
            // 
            this.WebBrowserPage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.WebBrowserPage.Location = new System.Drawing.Point(0, 0);
            this.WebBrowserPage.MinimumSize = new System.Drawing.Size(20, 20);
            this.WebBrowserPage.Name = "WebBrowserPage";
            this.WebBrowserPage.ScriptErrorsSuppressed = true;
            this.WebBrowserPage.Size = new System.Drawing.Size(353, 481);
            this.WebBrowserPage.TabIndex = 0;
            this.WebBrowserPage.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.WebBrowserPage_DocumentCompleted);
            // 
            // btnIgnore
            // 
            this.btnIgnore.Location = new System.Drawing.Point(65, 29);
            this.btnIgnore.Name = "btnIgnore";
            this.btnIgnore.Size = new System.Drawing.Size(47, 20);
            this.btnIgnore.TabIndex = 10;
            this.btnIgnore.Text = "Hide";
            this.btnIgnore.UseVisualStyleBackColor = true;
            this.btnIgnore.Click += new System.EventHandler(this.btnIgnore_Click);
            // 
            // btnUnignore
            // 
            this.btnUnignore.Location = new System.Drawing.Point(118, 29);
            this.btnUnignore.Name = "btnUnignore";
            this.btnUnignore.Size = new System.Drawing.Size(58, 20);
            this.btnUnignore.TabIndex = 15;
            this.btnUnignore.Text = "Unhide";
            this.btnUnignore.UseVisualStyleBackColor = true;
            this.btnUnignore.Click += new System.EventHandler(this.btnUnignore_Click);
            // 
            // textPostsPerPage
            // 
            this.textPostsPerPage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.textPostsPerPage.Location = new System.Drawing.Point(593, 29);
            this.textPostsPerPage.Name = "textPostsPerPage";
            this.textPostsPerPage.Size = new System.Drawing.Size(41, 20);
            this.textPostsPerPage.TabIndex = 6;
            this.textPostsPerPage.Text = "100";
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(263, 32);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(50, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "First Post";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtLastPost
            // 
            this.txtLastPost.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtLastPost.Location = new System.Drawing.Point(452, 29);
            this.txtLastPost.Name = "txtLastPost";
            this.txtLastPost.ReadOnly = true;
            this.txtLastPost.Size = new System.Drawing.Size(66, 20);
            this.txtLastPost.TabIndex = 5;
            this.txtLastPost.Text = "0";
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(524, 32);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(63, 13);
            this.label3.TabIndex = 9;
            this.label3.Text = "Posts/Page";
            // 
            // txtFirstPost
            // 
            this.txtFirstPost.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtFirstPost.Location = new System.Drawing.Point(319, 29);
            this.txtFirstPost.Name = "txtFirstPost";
            this.txtFirstPost.Size = new System.Drawing.Size(70, 20);
            this.txtFirstPost.TabIndex = 4;
            this.txtFirstPost.Text = "1";
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(395, 32);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(51, 13);
            this.label2.TabIndex = 8;
            this.label2.Text = "Last Post";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(182, 29);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 20);
            this.button1.TabIndex = 16;
            this.button1.Text = "Clear";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.label4.AutoSize = true;
            this.tableLayoutPanel1.SetColumnSpan(this.label4, 2);
            this.label4.Location = new System.Drawing.Point(3, 6);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(109, 13);
            this.label4.TabIndex = 17;
            this.label4.Text = "Main Thread URL";
            // 
            // URLTextBox
            // 
            this.URLTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.SetColumnSpan(this.URLTextBox, 9);
            this.URLTextBox.Location = new System.Drawing.Point(118, 3);
            this.URLTextBox.Name = "URLTextBox";
            this.URLTextBox.Size = new System.Drawing.Size(516, 20);
            this.URLTextBox.TabIndex = 1;
            this.URLTextBox.Text = "\r\n";
            // 
            // statusBrowser
            // 
            this.statusBrowser.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusText});
            this.statusBrowser.Location = new System.Drawing.Point(3, 562);
            this.statusBrowser.Name = "statusBrowser";
            this.statusBrowser.Size = new System.Drawing.Size(637, 22);
            this.statusBrowser.TabIndex = 13;
            this.statusBrowser.Text = "statusStrip1";
            // 
            // statusText
            // 
            this.statusText.Name = "statusText";
            this.statusText.Size = new System.Drawing.Size(38, 17);
            this.statusText.Text = "status";
            // 
            // tabVotes
            // 
            this.tabVotes.Controls.Add(this.tabPage4);
            this.tabVotes.Controls.Add(this.tabPage1);
            this.tabVotes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabVotes.Location = new System.Drawing.Point(0, 0);
            this.tabVotes.Name = "tabVotes";
            this.tabVotes.SelectedIndex = 0;
            this.tabVotes.Size = new System.Drawing.Size(651, 613);
            this.tabVotes.TabIndex = 4;
            // 
            // tabPage1
            // 
            this.tabPage1.BackColor = System.Drawing.Color.Gainsboro;
            this.tabPage1.Controls.Add(this.textBox1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(643, 587);
            this.tabPage1.TabIndex = 4;
            this.tabPage1.Text = "Help";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(8, 6);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.Size = new System.Drawing.Size(264, 374);
            this.textBox1.TabIndex = 0;
            this.textBox1.Text = resources.GetString("textBox1.Text");
            // 
            // FormVoteCounter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(651, 613);
            this.Controls.Add(this.tabVotes);
            this.MinimumSize = new System.Drawing.Size(667, 651);
            this.Name = "FormVoteCounter";
            this.Text = "Fennec Fox Vote Counter";
            this.tabPage4.ResumeLayout(false);
            this.tabPage4.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.statusBrowser.ResumeLayout(false);
            this.statusBrowser.PerformLayout();
            this.tabVotes.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.Button btnUnignore;
        private System.Windows.Forms.ListView listVotes;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.StatusStrip statusBrowser;
        private System.Windows.Forms.ToolStripStatusLabel statusText;
        private System.Windows.Forms.TextBox textPostsPerPage;
        private System.Windows.Forms.TextBox txtLastPost;
        private System.Windows.Forms.TextBox txtFirstPost;
        private System.Windows.Forms.WebBrowser WebBrowserPage;
        private System.Windows.Forms.TextBox URLTextBox;
        private System.Windows.Forms.Button btnIgnore;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button GoButtonAgain;
        private System.Windows.Forms.TabControl tabVotes;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TextBox textBox1;


    }
}

