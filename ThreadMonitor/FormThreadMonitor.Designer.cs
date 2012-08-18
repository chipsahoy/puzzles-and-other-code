namespace ThreadMonitor
{
    partial class frmThreadMonitor
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
            this.tlThreadMonitor = new System.Windows.Forms.TableLayoutPanel();
            this.txtAdd = new System.Windows.Forms.TextBox();
            this.lbThreads = new System.Windows.Forms.ListBox();
            this.statusText = new System.Windows.Forms.StatusStrip();
            this.tsStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.btnAdd = new System.Windows.Forms.Button();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.txtUsername = new System.Windows.Forms.TextBox();
            this.btnLogin = new System.Windows.Forms.Button();
            this.btnLogout = new System.Windows.Forms.Button();
            this.tlThreadMonitor.SuspendLayout();
            this.statusText.SuspendLayout();
            this.SuspendLayout();
            // 
            // tlThreadMonitor
            // 
            this.tlThreadMonitor.ColumnCount = 4;
            this.tlThreadMonitor.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlThreadMonitor.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlThreadMonitor.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlThreadMonitor.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlThreadMonitor.Controls.Add(this.txtAdd, 2, 1);
            this.tlThreadMonitor.Controls.Add(this.lbThreads, 1, 2);
            this.tlThreadMonitor.Controls.Add(this.statusText, 1, 3);
            this.tlThreadMonitor.Controls.Add(this.btnAdd, 1, 1);
            this.tlThreadMonitor.Controls.Add(this.txtPassword, 3, 0);
            this.tlThreadMonitor.Controls.Add(this.txtUsername, 2, 0);
            this.tlThreadMonitor.Controls.Add(this.btnLogin, 0, 0);
            this.tlThreadMonitor.Controls.Add(this.btnLogout, 1, 0);
            this.tlThreadMonitor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlThreadMonitor.Location = new System.Drawing.Point(0, 0);
            this.tlThreadMonitor.Name = "tlThreadMonitor";
            this.tlThreadMonitor.RowCount = 4;
            this.tlThreadMonitor.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlThreadMonitor.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlThreadMonitor.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlThreadMonitor.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlThreadMonitor.Size = new System.Drawing.Size(706, 262);
            this.tlThreadMonitor.TabIndex = 0;
            // 
            // txtAdd
            // 
            this.txtAdd.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tlThreadMonitor.SetColumnSpan(this.txtAdd, 2);
            this.txtAdd.Location = new System.Drawing.Point(165, 32);
            this.txtAdd.Name = "txtAdd";
            this.txtAdd.Size = new System.Drawing.Size(538, 20);
            this.txtAdd.TabIndex = 0;
            // 
            // lbThreads
            // 
            this.tlThreadMonitor.SetColumnSpan(this.lbThreads, 3);
            this.lbThreads.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbThreads.FormattingEnabled = true;
            this.lbThreads.Location = new System.Drawing.Point(84, 61);
            this.lbThreads.Name = "lbThreads";
            this.lbThreads.Size = new System.Drawing.Size(619, 178);
            this.lbThreads.TabIndex = 1;
            // 
            // statusText
            // 
            this.tlThreadMonitor.SetColumnSpan(this.statusText, 3);
            this.statusText.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsStatus});
            this.statusText.Location = new System.Drawing.Point(81, 242);
            this.statusText.Name = "statusText";
            this.statusText.Size = new System.Drawing.Size(625, 20);
            this.statusText.TabIndex = 2;
            this.statusText.Text = "statusStrip1";
            // 
            // tsStatus
            // 
            this.tsStatus.Name = "tsStatus";
            this.tsStatus.Size = new System.Drawing.Size(28, 15);
            this.tsStatus.Text = "Info";
            // 
            // btnAdd
            // 
            this.btnAdd.Location = new System.Drawing.Point(84, 32);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(75, 23);
            this.btnAdd.TabIndex = 3;
            this.btnAdd.Text = "Add";
            this.btnAdd.UseVisualStyleBackColor = true;
            // 
            // txtPassword
            // 
            this.txtPassword.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtPassword.Location = new System.Drawing.Point(437, 3);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.Size = new System.Drawing.Size(266, 20);
            this.txtPassword.TabIndex = 6;
            // 
            // txtUsername
            // 
            this.txtUsername.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtUsername.Location = new System.Drawing.Point(165, 3);
            this.txtUsername.Name = "txtUsername";
            this.txtUsername.Size = new System.Drawing.Size(266, 20);
            this.txtUsername.TabIndex = 5;
            // 
            // btnLogin
            // 
            this.btnLogin.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnLogin.Location = new System.Drawing.Point(3, 3);
            this.btnLogin.Name = "btnLogin";
            this.btnLogin.Size = new System.Drawing.Size(75, 23);
            this.btnLogin.TabIndex = 4;
            this.btnLogin.Text = "login";
            this.btnLogin.UseVisualStyleBackColor = true;
            this.btnLogin.Click += new System.EventHandler(this.btnLogin_Click);
            // 
            // btnLogout
            // 
            this.btnLogout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnLogout.Location = new System.Drawing.Point(84, 3);
            this.btnLogout.Name = "btnLogout";
            this.btnLogout.Size = new System.Drawing.Size(75, 23);
            this.btnLogout.TabIndex = 7;
            this.btnLogout.Text = "logout";
            this.btnLogout.UseVisualStyleBackColor = true;
            this.btnLogout.Click += new System.EventHandler(this.btnLogout_Click);
            // 
            // frmThreadMonitor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(706, 262);
            this.Controls.Add(this.tlThreadMonitor);
            this.Name = "frmThreadMonitor";
            this.Text = "Thread Monitor";
            this.tlThreadMonitor.ResumeLayout(false);
            this.tlThreadMonitor.PerformLayout();
            this.statusText.ResumeLayout(false);
            this.statusText.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tlThreadMonitor;
        private System.Windows.Forms.TextBox txtAdd;
        private System.Windows.Forms.ListBox lbThreads;
        private System.Windows.Forms.StatusStrip statusText;
        private System.Windows.Forms.ToolStripStatusLabel tsStatus;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnLogin;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.TextBox txtUsername;
        private System.Windows.Forms.Button btnLogout;

    }
}

