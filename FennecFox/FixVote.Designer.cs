namespace POG.FennecFox
{
    partial class FixVote
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnApply = new System.Windows.Forms.Button();
            this.radioUnignore = new System.Windows.Forms.RadioButton();
            this.radioOverride = new System.Windows.Forms.RadioButton();
            this.cmbValidVotes = new System.Windows.Forms.ComboBox();
            this.radioAlias = new System.Windows.Forms.RadioButton();
            this.radioIgnore = new System.Windows.Forms.RadioButton();
            this.radioNoChange = new System.Windows.Forms.RadioButton();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtBolded = new System.Windows.Forms.TextBox();
            this.txtVotee = new System.Windows.Forms.TextBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnApply);
            this.groupBox1.Controls.Add(this.radioUnignore);
            this.groupBox1.Controls.Add(this.radioOverride);
            this.groupBox1.Controls.Add(this.cmbValidVotes);
            this.groupBox1.Controls.Add(this.radioAlias);
            this.groupBox1.Controls.Add(this.radioIgnore);
            this.groupBox1.Controls.Add(this.radioNoChange);
            this.groupBox1.Location = new System.Drawing.Point(27, 71);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(261, 165);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Action:";
            // 
            // btnApply
            // 
            this.btnApply.Location = new System.Drawing.Point(180, 132);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(75, 23);
            this.btnApply.TabIndex = 7;
            this.btnApply.Text = "Apply";
            this.btnApply.UseVisualStyleBackColor = true;
            this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
            // 
            // radioUnignore
            // 
            this.radioUnignore.AutoSize = true;
            this.radioUnignore.Location = new System.Drawing.Point(6, 65);
            this.radioUnignore.Name = "radioUnignore";
            this.radioUnignore.Size = new System.Drawing.Size(186, 17);
            this.radioUnignore.TabIndex = 6;
            this.radioUnignore.TabStop = true;
            this.radioUnignore.Text = "Restore a previously ignored vote.";
            this.radioUnignore.UseVisualStyleBackColor = true;
            // 
            // radioOverride
            // 
            this.radioOverride.AutoSize = true;
            this.radioOverride.Location = new System.Drawing.Point(6, 111);
            this.radioOverride.Name = "radioOverride";
            this.radioOverride.Size = new System.Drawing.Size(251, 17);
            this.radioOverride.TabIndex = 3;
            this.radioOverride.TabStop = true;
            this.radioOverride.Text = "A special case. Set the vote to the name below.";
            this.radioOverride.UseVisualStyleBackColor = true;
            this.radioOverride.Visible = false;
            // 
            // cmbValidVotes
            // 
            this.cmbValidVotes.FormattingEnabled = true;
            this.cmbValidVotes.Location = new System.Drawing.Point(6, 134);
            this.cmbValidVotes.Name = "cmbValidVotes";
            this.cmbValidVotes.Size = new System.Drawing.Size(147, 21);
            this.cmbValidVotes.TabIndex = 5;
            // 
            // radioAlias
            // 
            this.radioAlias.AutoSize = true;
            this.radioAlias.Location = new System.Drawing.Point(6, 88);
            this.radioAlias.Name = "radioAlias";
            this.radioAlias.Size = new System.Drawing.Size(190, 17);
            this.radioAlias.TabIndex = 2;
            this.radioAlias.TabStop = true;
            this.radioAlias.Text = "This is an alias for the name below.";
            this.radioAlias.UseVisualStyleBackColor = true;
            // 
            // radioIgnore
            // 
            this.radioIgnore.AutoSize = true;
            this.radioIgnore.Location = new System.Drawing.Point(6, 42);
            this.radioIgnore.Name = "radioIgnore";
            this.radioIgnore.Size = new System.Drawing.Size(153, 17);
            this.radioIgnore.TabIndex = 1;
            this.radioIgnore.TabStop = true;
            this.radioIgnore.Text = "This is not a vote. Ignore it.";
            this.radioIgnore.UseVisualStyleBackColor = true;
            // 
            // radioNoChange
            // 
            this.radioNoChange.AutoSize = true;
            this.radioNoChange.Location = new System.Drawing.Point(6, 19);
            this.radioNoChange.Name = "radioNoChange";
            this.radioNoChange.Size = new System.Drawing.Size(124, 17);
            this.radioNoChange.TabIndex = 0;
            this.radioNoChange.TabStop = true;
            this.radioNoChange.Text = "Leave the vote as is.";
            this.radioNoChange.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(108, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(42, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "bolded:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(43, 45);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(107, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "current interpretation:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // txtBolded
            // 
            this.txtBolded.Location = new System.Drawing.Point(156, 19);
            this.txtBolded.Name = "txtBolded";
            this.txtBolded.ReadOnly = true;
            this.txtBolded.Size = new System.Drawing.Size(132, 20);
            this.txtBolded.TabIndex = 3;
            // 
            // txtVotee
            // 
            this.txtVotee.Location = new System.Drawing.Point(156, 45);
            this.txtVotee.Name = "txtVotee";
            this.txtVotee.ReadOnly = true;
            this.txtVotee.Size = new System.Drawing.Size(132, 20);
            this.txtVotee.TabIndex = 4;
            // 
            // btnOK
            // 
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(207, 242);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 6;
            this.btnOK.Text = "Close";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // FixVote
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(314, 277);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.txtVotee);
            this.Controls.Add(this.txtBolded);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.groupBox1);
            this.Name = "FixVote";
            this.Text = "Fix Vote";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton radioOverride;
        private System.Windows.Forms.ComboBox cmbValidVotes;
        private System.Windows.Forms.RadioButton radioAlias;
        private System.Windows.Forms.RadioButton radioIgnore;
        private System.Windows.Forms.RadioButton radioNoChange;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtBolded;
        private System.Windows.Forms.TextBox txtVotee;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.RadioButton radioUnignore;
    }
}