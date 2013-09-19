namespace RickyRaccoon
{
    partial class PMs
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
            this.txtPMs = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // txtPMs
            // 
            this.txtPMs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtPMs.Location = new System.Drawing.Point(0, 0);
            this.txtPMs.Multiline = true;
            this.txtPMs.Name = "txtPMs";
            this.txtPMs.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtPMs.Size = new System.Drawing.Size(566, 477);
            this.txtPMs.TabIndex = 0;
            // 
            // PMs
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(566, 477);
            this.Controls.Add(this.txtPMs);
            this.Name = "PMs";
            this.Text = "PMs";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        internal System.Windows.Forms.TextBox txtPMs;



    }
}