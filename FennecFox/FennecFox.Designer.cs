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
            this.addressBar = new System.Windows.Forms.TextBox();
            this.goButton = new System.Windows.Forms.Button();
            this.webBrowser1 = new System.Windows.Forms.WebBrowser();
            this.htmlSource = new System.Windows.Forms.TextBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.postGobutton = new System.Windows.Forms.Button();
            this.postArea = new System.Windows.Forms.TextBox();
            this.WebBrowserPost = new System.Windows.Forms.WebBrowser();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.btnClear = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.textPostsPerPage = new System.Windows.Forms.TextBox();
            this.txtLastPost = new System.Windows.Forms.TextBox();
            this.txtFirstPost = new System.Windows.Forms.TextBox();
            this.WebBrowserPage = new System.Windows.Forms.WebBrowser();
            this.GoButtonAgain = new System.Windows.Forms.Button();
            this.AnswerTextBox = new System.Windows.Forms.TextBox();
            this.URLTextBox = new System.Windows.Forms.TextBox();
            this.udPostNumber = new System.Windows.Forms.NumericUpDown();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.tabPage4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.udPostNumber)).BeginInit();
            this.SuspendLayout();
            // 
            // addressBar
            // 
            this.addressBar.Location = new System.Drawing.Point(30, 19);
            this.addressBar.Name = "addressBar";
            this.addressBar.Size = new System.Drawing.Size(644, 20);
            this.addressBar.TabIndex = 0;
            this.addressBar.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // goButton
            // 
            this.goButton.Location = new System.Drawing.Point(680, 16);
            this.goButton.Name = "goButton";
            this.goButton.Size = new System.Drawing.Size(62, 23);
            this.goButton.TabIndex = 1;
            this.goButton.Text = "Go";
            this.goButton.UseVisualStyleBackColor = true;
            this.goButton.Click += new System.EventHandler(this.goButton_Click);
            // 
            // webBrowser1
            // 
            this.webBrowser1.Location = new System.Drawing.Point(30, 68);
            this.webBrowser1.MinimumSize = new System.Drawing.Size(20, 20);
            this.webBrowser1.Name = "webBrowser1";
            this.webBrowser1.Size = new System.Drawing.Size(700, 294);
            this.webBrowser1.TabIndex = 2;
            this.webBrowser1.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.webBrowser1_DocumentCompleted);
            // 
            // htmlSource
            // 
            this.htmlSource.Location = new System.Drawing.Point(6, 0);
            this.htmlSource.Multiline = true;
            this.htmlSource.Name = "htmlSource";
            this.htmlSource.Size = new System.Drawing.Size(736, 386);
            this.htmlSource.TabIndex = 3;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.tabPage4);
            this.tabControl1.Location = new System.Drawing.Point(5, 2);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(756, 414);
            this.tabControl1.TabIndex = 4;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.webBrowser1);
            this.tabPage1.Controls.Add(this.goButton);
            this.tabPage1.Controls.Add(this.addressBar);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(748, 388);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "tabPage1";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.htmlSource);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(748, 388);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "tabPage2";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.udPostNumber);
            this.tabPage3.Controls.Add(this.postGobutton);
            this.tabPage3.Controls.Add(this.postArea);
            this.tabPage3.Controls.Add(this.WebBrowserPost);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(748, 388);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Get Post";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // postGobutton
            // 
            this.postGobutton.Location = new System.Drawing.Point(132, 6);
            this.postGobutton.Name = "postGobutton";
            this.postGobutton.Size = new System.Drawing.Size(64, 20);
            this.postGobutton.TabIndex = 3;
            this.postGobutton.Text = "Go";
            this.postGobutton.UseVisualStyleBackColor = true;
            this.postGobutton.Click += new System.EventHandler(this.postGobutton_Click);
            // 
            // postArea
            // 
            this.postArea.Location = new System.Drawing.Point(3, 32);
            this.postArea.Multiline = true;
            this.postArea.Name = "postArea";
            this.postArea.Size = new System.Drawing.Size(228, 341);
            this.postArea.TabIndex = 2;
            // 
            // WebBrowserPost
            // 
            this.WebBrowserPost.Location = new System.Drawing.Point(274, 23);
            this.WebBrowserPost.MinimumSize = new System.Drawing.Size(20, 20);
            this.WebBrowserPost.Name = "WebBrowserPost";
            this.WebBrowserPost.Size = new System.Drawing.Size(452, 350);
            this.WebBrowserPost.TabIndex = 1;
            this.WebBrowserPost.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.WebBrowserPost_DocumentCompleted);
            // 
            // tabPage4
            // 
            this.tabPage4.BackColor = System.Drawing.Color.Gainsboro;
            this.tabPage4.Controls.Add(this.btnClear);
            this.tabPage4.Controls.Add(this.label3);
            this.tabPage4.Controls.Add(this.label2);
            this.tabPage4.Controls.Add(this.label1);
            this.tabPage4.Controls.Add(this.textPostsPerPage);
            this.tabPage4.Controls.Add(this.txtLastPost);
            this.tabPage4.Controls.Add(this.txtFirstPost);
            this.tabPage4.Controls.Add(this.WebBrowserPage);
            this.tabPage4.Controls.Add(this.GoButtonAgain);
            this.tabPage4.Controls.Add(this.AnswerTextBox);
            this.tabPage4.Controls.Add(this.URLTextBox);
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4.Size = new System.Drawing.Size(748, 388);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "Get Post Ids";
            // 
            // btnClear
            // 
            this.btnClear.Location = new System.Drawing.Point(492, 49);
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
            this.label3.Location = new System.Drawing.Point(489, 24);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(63, 13);
            this.label3.TabIndex = 9;
            this.label3.Text = "Posts/Page";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(310, 53);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(51, 13);
            this.label2.TabIndex = 8;
            this.label2.Text = "Last Post";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(310, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(50, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "First Post";
            // 
            // textPostsPerPage
            // 
            this.textPostsPerPage.Location = new System.Drawing.Point(558, 21);
            this.textPostsPerPage.Name = "textPostsPerPage";
            this.textPostsPerPage.Size = new System.Drawing.Size(83, 20);
            this.textPostsPerPage.TabIndex = 6;
            this.textPostsPerPage.Text = "50";
            // 
            // txtLastPost
            // 
            this.txtLastPost.Location = new System.Drawing.Point(367, 46);
            this.txtLastPost.Name = "txtLastPost";
            this.txtLastPost.Size = new System.Drawing.Size(83, 20);
            this.txtLastPost.TabIndex = 5;
            this.txtLastPost.Text = "50";
            // 
            // txtFirstPost
            // 
            this.txtFirstPost.Location = new System.Drawing.Point(367, 21);
            this.txtFirstPost.Name = "txtFirstPost";
            this.txtFirstPost.Size = new System.Drawing.Size(83, 20);
            this.txtFirstPost.TabIndex = 4;
            this.txtFirstPost.Text = "1";
            // 
            // WebBrowserPage
            // 
            this.WebBrowserPage.Location = new System.Drawing.Point(255, 81);
            this.WebBrowserPage.MinimumSize = new System.Drawing.Size(20, 20);
            this.WebBrowserPage.Name = "WebBrowserPage";
            this.WebBrowserPage.Size = new System.Drawing.Size(473, 271);
            this.WebBrowserPage.TabIndex = 0;
            this.WebBrowserPage.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.WebBrowserPage_DocumentCompleted);
            // 
            // GoButtonAgain
            // 
            this.GoButtonAgain.Location = new System.Drawing.Point(229, 17);
            this.GoButtonAgain.Name = "GoButtonAgain";
            this.GoButtonAgain.Size = new System.Drawing.Size(47, 20);
            this.GoButtonAgain.TabIndex = 3;
            this.GoButtonAgain.Text = "Go";
            this.GoButtonAgain.UseVisualStyleBackColor = true;
            this.GoButtonAgain.Click += new System.EventHandler(this.GoButtonAgain_Click);
            // 
            // AnswerTextBox
            // 
            this.AnswerTextBox.Location = new System.Drawing.Point(16, 47);
            this.AnswerTextBox.Multiline = true;
            this.AnswerTextBox.Name = "AnswerTextBox";
            this.AnswerTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.AnswerTextBox.Size = new System.Drawing.Size(233, 305);
            this.AnswerTextBox.TabIndex = 2;
            // 
            // URLTextBox
            // 
            this.URLTextBox.Location = new System.Drawing.Point(16, 17);
            this.URLTextBox.Name = "URLTextBox";
            this.URLTextBox.Size = new System.Drawing.Size(207, 20);
            this.URLTextBox.TabIndex = 1;
            // 
            // udPostNumber
            // 
            this.udPostNumber.Location = new System.Drawing.Point(6, 6);
            this.udPostNumber.Name = "udPostNumber";
            this.udPostNumber.Size = new System.Drawing.Size(120, 20);
            this.udPostNumber.TabIndex = 4;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(773, 415);
            this.Controls.Add(this.tabControl1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            this.tabPage4.ResumeLayout(false);
            this.tabPage4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.udPostNumber)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox addressBar;
        private System.Windows.Forms.Button goButton;
        private System.Windows.Forms.WebBrowser webBrowser1;
        private System.Windows.Forms.TextBox htmlSource;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.Button postGobutton;
        private System.Windows.Forms.TextBox postArea;
        private System.Windows.Forms.WebBrowser WebBrowserPost;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.TextBox URLTextBox;
        private System.Windows.Forms.WebBrowser WebBrowserPage;
        private System.Windows.Forms.Button GoButtonAgain;
        private System.Windows.Forms.TextBox AnswerTextBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textPostsPerPage;
        private System.Windows.Forms.TextBox txtLastPost;
        private System.Windows.Forms.TextBox txtFirstPost;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.NumericUpDown udPostNumber;
    }
}

