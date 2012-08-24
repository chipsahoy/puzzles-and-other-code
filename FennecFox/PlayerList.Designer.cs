namespace POG.FennecFox
{
    partial class PlayerList
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PlayerList));
            this.btnSubmitPlayers = new System.Windows.Forms.Button();
            this.txtNewPlayers = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // btnSubmitPlayers
            // 
            this.btnSubmitPlayers.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSubmitPlayers.Location = new System.Drawing.Point(197, 12);
            this.btnSubmitPlayers.Name = "btnSubmitPlayers";
            this.btnSubmitPlayers.Size = new System.Drawing.Size(75, 23);
            this.btnSubmitPlayers.TabIndex = 0;
            this.btnSubmitPlayers.Text = "OK";
            this.btnSubmitPlayers.UseVisualStyleBackColor = true;
            this.btnSubmitPlayers.Click += new System.EventHandler(this.btnSubmitPlayers_Click);
            // 
            // txtNewPlayers
            // 
            this.txtNewPlayers.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtNewPlayers.Location = new System.Drawing.Point(12, 12);
            this.txtNewPlayers.Multiline = true;
            this.txtNewPlayers.Name = "txtNewPlayers";
            this.txtNewPlayers.Size = new System.Drawing.Size(176, 238);
            this.txtNewPlayers.TabIndex = 1;
            // 
            // PlayerList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this.txtNewPlayers);
            this.Controls.Add(this.btnSubmitPlayers);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "PlayerList";
            this.Text = "PlayerList";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnSubmitPlayers;
        private System.Windows.Forms.TextBox txtNewPlayers;
    }
}