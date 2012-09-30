namespace POG.FennecFox
{
    partial class DayEditor
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
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.udDay = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.udStartPost = new System.Windows.Forms.NumericUpDown();
            this.dtEodDate = new System.Windows.Forms.DateTimePicker();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.dtEodTime = new System.Windows.Forms.DateTimePicker();
            this.txtStartPostTime = new System.Windows.Forms.TextBox();
            this.txtStartPostPoster = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.udDay)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.udStartPost)).BeginInit();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(57, 199);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 0;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(153, 199);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // udDay
            // 
            this.udDay.Location = new System.Drawing.Point(116, 24);
            this.udDay.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.udDay.Name = "udDay";
            this.udDay.Size = new System.Drawing.Size(50, 20);
            this.udDay.TabIndex = 2;
            this.udDay.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.udDay.ValueChanged += new System.EventHandler(this.udDay_ValueChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(84, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(26, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Day";
            // 
            // udStartPost
            // 
            this.udStartPost.Location = new System.Drawing.Point(116, 50);
            this.udStartPost.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.udStartPost.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.udStartPost.Name = "udStartPost";
            this.udStartPost.Size = new System.Drawing.Size(101, 20);
            this.udStartPost.TabIndex = 4;
            this.udStartPost.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // dtEodDate
            // 
            this.dtEodDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtEodDate.Location = new System.Drawing.Point(116, 128);
            this.dtEodDate.Name = "dtEodDate";
            this.dtEodDate.Size = new System.Drawing.Size(101, 20);
            this.dtEodDate.TabIndex = 5;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(80, 145);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(30, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "EOD";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(57, 52);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Start Post";
            // 
            // dtEodTime
            // 
            this.dtEodTime.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtEodTime.Location = new System.Drawing.Point(116, 154);
            this.dtEodTime.Name = "dtEodTime";
            this.dtEodTime.ShowUpDown = true;
            this.dtEodTime.Size = new System.Drawing.Size(101, 20);
            this.dtEodTime.TabIndex = 8;
            // 
            // txtStartPostTime
            // 
            this.txtStartPostTime.Location = new System.Drawing.Point(60, 102);
            this.txtStartPostTime.Name = "txtStartPostTime";
            this.txtStartPostTime.ReadOnly = true;
            this.txtStartPostTime.Size = new System.Drawing.Size(157, 20);
            this.txtStartPostTime.TabIndex = 9;
            // 
            // txtStartPostPoster
            // 
            this.txtStartPostPoster.Location = new System.Drawing.Point(116, 76);
            this.txtStartPostPoster.Name = "txtStartPostPoster";
            this.txtStartPostPoster.ReadOnly = true;
            this.txtStartPostPoster.Size = new System.Drawing.Size(101, 20);
            this.txtStartPostPoster.TabIndex = 10;
            // 
            // DayEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this.txtStartPostPoster);
            this.Controls.Add(this.txtStartPostTime);
            this.Controls.Add(this.dtEodTime);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.dtEodDate);
            this.Controls.Add(this.udStartPost);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.udDay);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "DayEditor";
            this.Text = "DayEditor";
            ((System.ComponentModel.ISupportInitialize)(this.udDay)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.udStartPost)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.NumericUpDown udDay;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown udStartPost;
        private System.Windows.Forms.DateTimePicker dtEodDate;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.DateTimePicker dtEodTime;
        private System.Windows.Forms.TextBox txtStartPostTime;
        private System.Windows.Forms.TextBox txtStartPostPoster;
    }
}