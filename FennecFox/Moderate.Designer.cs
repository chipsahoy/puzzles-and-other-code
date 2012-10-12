namespace POG.FennecFox
{
    partial class Moderate
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
            this.chkAutoPost = new System.Windows.Forms.CheckBox();
            this.chkLockThread = new System.Windows.Forms.CheckBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // chkAutoPost
            // 
            this.chkAutoPost.AutoSize = true;
            this.chkAutoPost.Location = new System.Drawing.Point(12, 12);
            this.chkAutoPost.Name = "chkAutoPost";
            this.chkAutoPost.Size = new System.Drawing.Size(161, 17);
            this.chkAutoPost.TabIndex = 0;
            this.chkAutoPost.Text = "Post vote counts periodically";
            this.chkAutoPost.UseVisualStyleBackColor = true;
            // 
            // chkLockThread
            // 
            this.chkLockThread.AutoSize = true;
            this.chkLockThread.Location = new System.Drawing.Point(12, 35);
            this.chkLockThread.Name = "chkLockThread";
            this.chkLockThread.Size = new System.Drawing.Size(121, 17);
            this.chkLockThread.TabIndex = 1;
            this.chkLockThread.Text = "Lock thread at night";
            this.chkLockThread.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(195, 12);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 2;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(195, 41);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // Moderate
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(282, 81);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.chkLockThread);
            this.Controls.Add(this.chkAutoPost);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Moderate";
            this.Text = "Moderate";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox chkAutoPost;
        private System.Windows.Forms.CheckBox chkLockThread;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
    }
}