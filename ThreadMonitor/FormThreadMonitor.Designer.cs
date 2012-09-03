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
            this.statusText = new System.Windows.Forms.StatusStrip();
            this.tsStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.txtUsername = new System.Windows.Forms.TextBox();
            this.btnLogin = new System.Windows.Forms.Button();
            this.btnLogout = new System.Windows.Forms.Button();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.tlThreadMonitor.SuspendLayout();
            this.statusText.SuspendLayout();
            this.SuspendLayout();
            // 
            // tlThreadMonitor
            // 
            this.tlThreadMonitor.ColumnCount = 2;
            this.tlThreadMonitor.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlThreadMonitor.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlThreadMonitor.Controls.Add(this.statusText, 1, 2);
            this.tlThreadMonitor.Controls.Add(this.txtUsername, 1, 0);
            this.tlThreadMonitor.Controls.Add(this.btnLogin, 0, 0);
            this.tlThreadMonitor.Controls.Add(this.btnLogout, 0, 1);
            this.tlThreadMonitor.Controls.Add(this.txtPassword, 1, 1);
            this.tlThreadMonitor.Controls.Add(this.button1, 0, 2);
            this.tlThreadMonitor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlThreadMonitor.Location = new System.Drawing.Point(0, 0);
            this.tlThreadMonitor.Name = "tlThreadMonitor";
            this.tlThreadMonitor.RowCount = 3;
            this.tlThreadMonitor.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlThreadMonitor.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlThreadMonitor.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlThreadMonitor.Size = new System.Drawing.Size(216, 81);
            this.tlThreadMonitor.TabIndex = 0;
            // 
            // statusText
            // 
            this.statusText.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsStatus});
            this.statusText.Location = new System.Drawing.Point(81, 65);
            this.statusText.Name = "statusText";
            this.statusText.Size = new System.Drawing.Size(312, 22);
            this.statusText.TabIndex = 2;
            this.statusText.Text = "statusStrip1";
            // 
            // tsStatus
            // 
            this.tsStatus.Name = "tsStatus";
            this.tsStatus.Size = new System.Drawing.Size(28, 17);
            this.tsStatus.Text = "Info";
            // 
            // txtUsername
            // 
            this.txtUsername.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtUsername.Location = new System.Drawing.Point(84, 3);
            this.txtUsername.Name = "txtUsername";
            this.txtUsername.Size = new System.Drawing.Size(306, 20);
            this.txtUsername.TabIndex = 1;
            // 
            // btnLogin
            // 
            this.btnLogin.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnLogin.Location = new System.Drawing.Point(3, 3);
            this.btnLogin.Name = "btnLogin";
            this.btnLogin.Size = new System.Drawing.Size(75, 23);
            this.btnLogin.TabIndex = 3;
            this.btnLogin.Text = "login";
            this.btnLogin.UseVisualStyleBackColor = true;
            this.btnLogin.Click += new System.EventHandler(this.btnLogin_Click);
            // 
            // btnLogout
            // 
            this.btnLogout.Enabled = false;
            this.btnLogout.Location = new System.Drawing.Point(3, 32);
            this.btnLogout.Name = "btnLogout";
            this.btnLogout.Size = new System.Drawing.Size(75, 23);
            this.btnLogout.TabIndex = 4;
            this.btnLogout.Text = "logout";
            this.btnLogout.UseVisualStyleBackColor = true;
            this.btnLogout.Click += new System.EventHandler(this.btnLogout_Click);
            // 
            // txtPassword
            // 
            this.txtPassword.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtPassword.Location = new System.Drawing.Point(84, 32);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.Size = new System.Drawing.Size(306, 20);
            this.txtPassword.TabIndex = 2;
            // 
            // button1
            // 
            this.button1.Enabled = false;
            this.button1.Location = new System.Drawing.Point(3, 61);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 5;
            this.button1.Text = "Go!";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // frmThreadMonitor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(216, 81);
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
        private System.Windows.Forms.Button btnLogin;
        private System.Windows.Forms.Button btnLogout;
        private System.Windows.Forms.StatusStrip statusText;
        private System.Windows.Forms.ToolStripStatusLabel tsStatus;
        private System.Windows.Forms.TextBox txtUsername;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.Button button1;

    }
}

