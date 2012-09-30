namespace POG.FennecFox
{
    partial class KillSub
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
            this.dtExitTime = new System.Windows.Forms.DateTimePicker();
            this.dtExitDate = new System.Windows.Forms.DateTimePicker();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.chkSub = new System.Windows.Forms.CheckBox();
            this.txtReplacement = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // dtExitTime
            // 
            this.dtExitTime.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtExitTime.Location = new System.Drawing.Point(127, 41);
            this.dtExitTime.Name = "dtExitTime";
            this.dtExitTime.ShowUpDown = true;
            this.dtExitTime.Size = new System.Drawing.Size(101, 20);
            this.dtExitTime.TabIndex = 12;
            // 
            // dtExitDate
            // 
            this.dtExitDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtExitDate.Location = new System.Drawing.Point(127, 12);
            this.dtExitDate.Name = "dtExitDate";
            this.dtExitDate.Size = new System.Drawing.Size(101, 20);
            this.dtExitDate.TabIndex = 11;
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(153, 155);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 10;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(57, 155);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 9;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            // 
            // chkSub
            // 
            this.chkSub.AutoSize = true;
            this.chkSub.Location = new System.Drawing.Point(12, 15);
            this.chkSub.Name = "chkSub";
            this.chkSub.Size = new System.Drawing.Size(90, 17);
            this.chkSub.TabIndex = 13;
            this.chkSub.Text = "Replaced By:";
            this.chkSub.UseVisualStyleBackColor = true;
            // 
            // txtReplacement
            // 
            this.txtReplacement.Location = new System.Drawing.Point(12, 41);
            this.txtReplacement.Name = "txtReplacement";
            this.txtReplacement.Size = new System.Drawing.Size(100, 20);
            this.txtReplacement.TabIndex = 14;
            // 
            // KillSub
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this.txtReplacement);
            this.Controls.Add(this.chkSub);
            this.Controls.Add(this.dtExitTime);
            this.Controls.Add(this.dtExitDate);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Name = "KillSub";
            this.Text = "KillSub";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DateTimePicker dtExitTime;
        private System.Windows.Forms.DateTimePicker dtExitDate;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.CheckBox chkSub;
        private System.Windows.Forms.TextBox txtReplacement;
    }
}