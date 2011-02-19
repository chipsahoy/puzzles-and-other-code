namespace FennecFox
{
    partial class Form1
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
            this.statusBrowser = new System.Windows.Forms.StatusStrip();
            this.statusText = new System.Windows.Forms.ToolStripStatusLabel();
            this.BrowserPost = new System.Windows.Forms.WebBrowser();
            this.textPostsPerPage = new System.Windows.Forms.TextBox();
            this.txtLastPost = new System.Windows.Forms.TextBox();
            this.txtFirstPost = new System.Windows.Forms.TextBox();
            this.WebBrowserPage = new System.Windows.Forms.WebBrowser();
            this.URLTextBox = new System.Windows.Forms.TextBox();
            this.btnClear = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.GoButtonAgain = new System.Windows.Forms.Button();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.btnSearch = new System.Windows.Forms.Button();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.txtCountPosts = new System.Windows.Forms.TextBox();
            this.udPostNumber = new System.Windows.Forms.NumericUpDown();
            this.postArea = new System.Windows.Forms.TextBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.txtPostNumber = new System.Windows.Forms.TextBox();
            this.tabPage4.SuspendLayout();
            this.statusBrowser.SuspendLayout();
            this.tabPage3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.udPostNumber)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabPage4
            // 
            this.tabPage4.BackColor = System.Drawing.Color.Gainsboro;
            this.tabPage4.Controls.Add(this.statusBrowser);
            this.tabPage4.Controls.Add(this.BrowserPost);
            this.tabPage4.Controls.Add(this.textPostsPerPage);
            this.tabPage4.Controls.Add(this.txtLastPost);
            this.tabPage4.Controls.Add(this.txtFirstPost);
            this.tabPage4.Controls.Add(this.WebBrowserPage);
            this.tabPage4.Controls.Add(this.URLTextBox);
            this.tabPage4.Controls.Add(this.btnClear);
            this.tabPage4.Controls.Add(this.label3);
            this.tabPage4.Controls.Add(this.label2);
            this.tabPage4.Controls.Add(this.label1);
            this.tabPage4.Controls.Add(this.GoButtonAgain);
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4.Size = new System.Drawing.Size(748, 388);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "Get Post Ids";
            // 
            // statusBrowser
            // 
            this.statusBrowser.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusText});
            this.statusBrowser.Location = new System.Drawing.Point(3, 363);
            this.statusBrowser.Name = "statusBrowser";
            this.statusBrowser.Size = new System.Drawing.Size(742, 22);
            this.statusBrowser.TabIndex = 13;
            this.statusBrowser.Text = "statusStrip1";
            // 
            // statusText
            // 
            this.statusText.Name = "statusText";
            this.statusText.Size = new System.Drawing.Size(38, 17);
            this.statusText.Text = "status";
            // 
            // BrowserPost
            // 
            this.BrowserPost.Location = new System.Drawing.Point(382, 72);
            this.BrowserPost.MinimumSize = new System.Drawing.Size(20, 20);
            this.BrowserPost.Name = "BrowserPost";
            this.BrowserPost.Size = new System.Drawing.Size(349, 271);
            this.BrowserPost.TabIndex = 11;
            this.BrowserPost.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.BrowserPost_DocumentCompleted);
            // 
            // textPostsPerPage
            // 
            this.textPostsPerPage.Location = new System.Drawing.Point(530, 21);
            this.textPostsPerPage.Name = "textPostsPerPage";
            this.textPostsPerPage.Size = new System.Drawing.Size(83, 20);
            this.textPostsPerPage.TabIndex = 6;
            this.textPostsPerPage.Text = "50";
            // 
            // txtLastPost
            // 
            this.txtLastPost.Location = new System.Drawing.Point(365, 46);
            this.txtLastPost.Name = "txtLastPost";
            this.txtLastPost.ReadOnly = true;
            this.txtLastPost.Size = new System.Drawing.Size(83, 20);
            this.txtLastPost.TabIndex = 5;
            this.txtLastPost.Text = "0";
            // 
            // txtFirstPost
            // 
            this.txtFirstPost.Location = new System.Drawing.Point(365, 21);
            this.txtFirstPost.Name = "txtFirstPost";
            this.txtFirstPost.Size = new System.Drawing.Size(83, 20);
            this.txtFirstPost.TabIndex = 4;
            this.txtFirstPost.Text = "1";
            // 
            // WebBrowserPage
            // 
            this.WebBrowserPage.Location = new System.Drawing.Point(16, 72);
            this.WebBrowserPage.MinimumSize = new System.Drawing.Size(20, 20);
            this.WebBrowserPage.Name = "WebBrowserPage";
            this.WebBrowserPage.Size = new System.Drawing.Size(342, 271);
            this.WebBrowserPage.TabIndex = 0;
            this.WebBrowserPage.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.WebBrowserPage_DocumentCompleted);
            // 
            // URLTextBox
            // 
            this.URLTextBox.Location = new System.Drawing.Point(16, 17);
            this.URLTextBox.Name = "URLTextBox";
            this.URLTextBox.Size = new System.Drawing.Size(288, 20);
            this.URLTextBox.TabIndex = 1;
            // 
            // btnClear
            // 
            this.btnClear.Location = new System.Drawing.Point(110, 43);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(47, 20);
            this.btnClear.TabIndex = 10;
            this.btnClear.Text = "Clear";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(463, 24);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(63, 13);
            this.label3.TabIndex = 9;
            this.label3.Text = "Posts/Page";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(308, 53);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(51, 13);
            this.label2.TabIndex = 8;
            this.label2.Text = "Last Post";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(308, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(50, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "First Post";
            // 
            // GoButtonAgain
            // 
            this.GoButtonAgain.Location = new System.Drawing.Point(16, 43);
            this.GoButtonAgain.Name = "GoButtonAgain";
            this.GoButtonAgain.Size = new System.Drawing.Size(56, 20);
            this.GoButtonAgain.TabIndex = 3;
            this.GoButtonAgain.Text = "Get Em";
            this.GoButtonAgain.UseVisualStyleBackColor = true;
            this.GoButtonAgain.Click += new System.EventHandler(this.GoButtonAgain_Click);
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.txtPostNumber);
            this.tabPage3.Controls.Add(this.btnSearch);
            this.tabPage3.Controls.Add(this.txtSearch);
            this.tabPage3.Controls.Add(this.txtCountPosts);
            this.tabPage3.Controls.Add(this.udPostNumber);
            this.tabPage3.Controls.Add(this.postArea);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(748, 388);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Get Post";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // btnSearch
            // 
            this.btnSearch.Location = new System.Drawing.Point(422, 4);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(75, 23);
            this.btnSearch.TabIndex = 7;
            this.btnSearch.Text = "Search";
            this.btnSearch.UseVisualStyleBackColor = true;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // txtSearch
            // 
            this.txtSearch.Location = new System.Drawing.Point(234, 5);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(182, 20);
            this.txtSearch.TabIndex = 6;
            // 
            // txtCountPosts
            // 
            this.txtCountPosts.Location = new System.Drawing.Point(82, 6);
            this.txtCountPosts.Name = "txtCountPosts";
            this.txtCountPosts.ReadOnly = true;
            this.txtCountPosts.Size = new System.Drawing.Size(70, 20);
            this.txtCountPosts.TabIndex = 5;
            // 
            // udPostNumber
            // 
            this.udPostNumber.Location = new System.Drawing.Point(6, 6);
            this.udPostNumber.Name = "udPostNumber";
            this.udPostNumber.Size = new System.Drawing.Size(70, 20);
            this.udPostNumber.TabIndex = 4;
            this.udPostNumber.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.udPostNumber.ValueChanged += new System.EventHandler(this.udPostNumber_ValueChanged);
            // 
            // postArea
            // 
            this.postArea.Location = new System.Drawing.Point(3, 31);
            this.postArea.Multiline = true;
            this.postArea.Name = "postArea";
            this.postArea.Size = new System.Drawing.Size(524, 342);
            this.postArea.TabIndex = 2;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.tabPage4);
            this.tabControl1.Location = new System.Drawing.Point(5, 2);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(756, 414);
            this.tabControl1.TabIndex = 4;
            // 
            // txtPostNumber
            // 
            this.txtPostNumber.Location = new System.Drawing.Point(158, 5);
            this.txtPostNumber.Name = "txtPostNumber";
            this.txtPostNumber.ReadOnly = true;
            this.txtPostNumber.Size = new System.Drawing.Size(70, 20);
            this.txtPostNumber.TabIndex = 8;
            this.txtPostNumber.Text = "1";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(773, 415);
            this.Controls.Add(this.tabControl1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.tabPage4.ResumeLayout(false);
            this.tabPage4.PerformLayout();
            this.statusBrowser.ResumeLayout(false);
            this.statusBrowser.PerformLayout();
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.udPostNumber)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.WebBrowser BrowserPost;
        private System.Windows.Forms.TextBox textPostsPerPage;
        private System.Windows.Forms.TextBox txtLastPost;
        private System.Windows.Forms.TextBox txtFirstPost;
        private System.Windows.Forms.WebBrowser WebBrowserPage;
        private System.Windows.Forms.TextBox URLTextBox;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button GoButtonAgain;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.NumericUpDown udPostNumber;
        private System.Windows.Forms.TextBox postArea;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.StatusStrip statusBrowser;
        private System.Windows.Forms.ToolStripStatusLabel statusText;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.TextBox txtCountPosts;
        private System.Windows.Forms.TextBox txtPostNumber;

    }
}

