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
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.btnUnignore = new System.Windows.Forms.Button();
            this.listVotes = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.statusBrowser = new System.Windows.Forms.StatusStrip();
            this.statusText = new System.Windows.Forms.ToolStripStatusLabel();
            this.textPostsPerPage = new System.Windows.Forms.TextBox();
            this.txtLastPost = new System.Windows.Forms.TextBox();
            this.txtFirstPost = new System.Windows.Forms.TextBox();
            this.WebBrowserPage = new System.Windows.Forms.WebBrowser();
            this.URLTextBox = new System.Windows.Forms.TextBox();
            this.btnIgnore = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.GoButtonAgain = new System.Windows.Forms.Button();
            this.tabVotes = new System.Windows.Forms.TabControl();
            this.tabPage4.SuspendLayout();
            this.statusBrowser.SuspendLayout();
            this.tabVotes.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabPage4
            // 
            this.tabPage4.BackColor = System.Drawing.Color.Gainsboro;
            this.tabPage4.Controls.Add(this.btnUnignore);
            this.tabPage4.Controls.Add(this.listVotes);
            this.tabPage4.Controls.Add(this.statusBrowser);
            this.tabPage4.Controls.Add(this.textPostsPerPage);
            this.tabPage4.Controls.Add(this.txtLastPost);
            this.tabPage4.Controls.Add(this.txtFirstPost);
            this.tabPage4.Controls.Add(this.WebBrowserPage);
            this.tabPage4.Controls.Add(this.URLTextBox);
            this.tabPage4.Controls.Add(this.btnIgnore);
            this.tabPage4.Controls.Add(this.label3);
            this.tabPage4.Controls.Add(this.label2);
            this.tabPage4.Controls.Add(this.label1);
            this.tabPage4.Controls.Add(this.GoButtonAgain);
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4.Size = new System.Drawing.Size(765, 587);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "Get Post Ids";
            // 
            // btnUnignore
            // 
            this.btnUnignore.Location = new System.Drawing.Point(131, 40);
            this.btnUnignore.Name = "btnUnignore";
            this.btnUnignore.Size = new System.Drawing.Size(58, 20);
            this.btnUnignore.TabIndex = 15;
            this.btnUnignore.Text = "Unhide";
            this.btnUnignore.UseVisualStyleBackColor = true;
            this.btnUnignore.Click += new System.EventHandler(this.btnUnignore_Click);
            // 
            // listVotes
            // 
            this.listVotes.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3});
            this.listVotes.FullRowSelect = true;
            this.listVotes.HideSelection = false;
            this.listVotes.Location = new System.Drawing.Point(18, 77);
            this.listVotes.Name = "listVotes";
            this.listVotes.Size = new System.Drawing.Size(430, 480);
            this.listVotes.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.listVotes.TabIndex = 14;
            this.listVotes.UseCompatibleStateImageBehavior = false;
            this.listVotes.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Poster";
            this.columnHeader1.Width = 140;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "#";
            this.columnHeader2.Width = 50;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Bolded";
            this.columnHeader3.Width = 236;
            // 
            // statusBrowser
            // 
            this.statusBrowser.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusText});
            this.statusBrowser.Location = new System.Drawing.Point(3, 562);
            this.statusBrowser.Name = "statusBrowser";
            this.statusBrowser.Size = new System.Drawing.Size(759, 22);
            this.statusBrowser.TabIndex = 13;
            this.statusBrowser.Text = "statusStrip1";
            // 
            // statusText
            // 
            this.statusText.Name = "statusText";
            this.statusText.Size = new System.Drawing.Size(38, 17);
            this.statusText.Text = "status";
            // 
            // textPostsPerPage
            // 
            this.textPostsPerPage.Location = new System.Drawing.Point(568, 40);
            this.textPostsPerPage.Name = "textPostsPerPage";
            this.textPostsPerPage.Size = new System.Drawing.Size(83, 20);
            this.textPostsPerPage.TabIndex = 6;
            this.textPostsPerPage.Text = "100";
            // 
            // txtLastPost
            // 
            this.txtLastPost.Location = new System.Drawing.Point(401, 40);
            this.txtLastPost.Name = "txtLastPost";
            this.txtLastPost.ReadOnly = true;
            this.txtLastPost.Size = new System.Drawing.Size(83, 20);
            this.txtLastPost.TabIndex = 5;
            this.txtLastPost.Text = "0";
            // 
            // txtFirstPost
            // 
            this.txtFirstPost.Location = new System.Drawing.Point(255, 40);
            this.txtFirstPost.Name = "txtFirstPost";
            this.txtFirstPost.Size = new System.Drawing.Size(83, 20);
            this.txtFirstPost.TabIndex = 4;
            this.txtFirstPost.Text = "1";
            // 
            // WebBrowserPage
            // 
            this.WebBrowserPage.Location = new System.Drawing.Point(483, 77);
            this.WebBrowserPage.MinimumSize = new System.Drawing.Size(20, 20);
            this.WebBrowserPage.Name = "WebBrowserPage";
            this.WebBrowserPage.ScriptErrorsSuppressed = true;
            this.WebBrowserPage.Size = new System.Drawing.Size(274, 480);
            this.WebBrowserPage.TabIndex = 0;
            this.WebBrowserPage.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.WebBrowserPage_DocumentCompleted);
            // 
            // URLTextBox
            // 
            this.URLTextBox.Location = new System.Drawing.Point(16, 6);
            this.URLTextBox.Name = "URLTextBox";
            this.URLTextBox.Size = new System.Drawing.Size(548, 20);
            this.URLTextBox.TabIndex = 1;
            this.URLTextBox.Text = "http://forumserver.twoplustwo.com/59/puzzles-other-games/newb-free-turbo-1086996/" +
    "";
            // 
            // btnIgnore
            // 
            this.btnIgnore.Location = new System.Drawing.Point(78, 40);
            this.btnIgnore.Name = "btnIgnore";
            this.btnIgnore.Size = new System.Drawing.Size(47, 20);
            this.btnIgnore.TabIndex = 10;
            this.btnIgnore.Text = "Hide";
            this.btnIgnore.UseVisualStyleBackColor = true;
            this.btnIgnore.Click += new System.EventHandler(this.btnIgnore_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(501, 47);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(63, 13);
            this.label3.TabIndex = 9;
            this.label3.Text = "Posts/Page";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(344, 47);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(51, 13);
            this.label2.TabIndex = 8;
            this.label2.Text = "Last Post";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(198, 47);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(50, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "First Post";
            // 
            // GoButtonAgain
            // 
            this.GoButtonAgain.Location = new System.Drawing.Point(16, 40);
            this.GoButtonAgain.Name = "GoButtonAgain";
            this.GoButtonAgain.Size = new System.Drawing.Size(56, 20);
            this.GoButtonAgain.TabIndex = 3;
            this.GoButtonAgain.Text = "Get Em";
            this.GoButtonAgain.UseVisualStyleBackColor = true;
            this.GoButtonAgain.Click += new System.EventHandler(this.GoButtonAgain_Click);
            // 
            // tabVotes
            // 
            this.tabVotes.Controls.Add(this.tabPage4);
            this.tabVotes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabVotes.Location = new System.Drawing.Point(0, 0);
            this.tabVotes.Name = "tabVotes";
            this.tabVotes.SelectedIndex = 0;
            this.tabVotes.Size = new System.Drawing.Size(773, 613);
            this.tabVotes.TabIndex = 4;
            // 
            // FormVoteCounter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(773, 613);
            this.Controls.Add(this.tabVotes);
            this.Name = "FormVoteCounter";
            this.Text = "Fennec Fox Vote Counter";
            this.tabPage4.ResumeLayout(false);
            this.tabPage4.PerformLayout();
            this.statusBrowser.ResumeLayout(false);
            this.statusBrowser.PerformLayout();
            this.tabVotes.ResumeLayout(false);
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


    }
}

