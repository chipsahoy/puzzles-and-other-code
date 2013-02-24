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
			this.btnDelete = new System.Windows.Forms.Button();
			this.btnPaste = new System.Windows.Forms.Button();
			this.btnCopyLive = new System.Windows.Forms.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.acMenu = new AutocompleteMenuNS.AutocompleteMenu();
			this.grdRoster = new POG.FennecFox.NoArrowNavigateDataGridView();
			this.btnKillSub = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.grdRoster)).BeginInit();
			this.SuspendLayout();
			// 
			// btnSubmitPlayers
			// 
			this.btnSubmitPlayers.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnSubmitPlayers.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnSubmitPlayers.Location = new System.Drawing.Point(448, 30);
			this.btnSubmitPlayers.Name = "btnSubmitPlayers";
			this.btnSubmitPlayers.Size = new System.Drawing.Size(75, 25);
			this.btnSubmitPlayers.TabIndex = 0;
			this.btnSubmitPlayers.Text = "Close";
			this.btnSubmitPlayers.UseVisualStyleBackColor = true;
			// 
			// btnDelete
			// 
			this.btnDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnDelete.Location = new System.Drawing.Point(448, 400);
			this.btnDelete.Name = "btnDelete";
			this.btnDelete.Size = new System.Drawing.Size(75, 50);
			this.btnDelete.TabIndex = 4;
			this.btnDelete.Text = "Erase Player";
			this.btnDelete.UseVisualStyleBackColor = true;
			this.btnDelete.Click += new System.EventHandler(this.btnErase_Click);
			// 
			// btnPaste
			// 
			this.btnPaste.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnPaste.Location = new System.Drawing.Point(448, 153);
			this.btnPaste.Name = "btnPaste";
			this.btnPaste.Size = new System.Drawing.Size(75, 50);
			this.btnPaste.TabIndex = 9;
			this.btnPaste.Text = "Paste a list";
			this.btnPaste.UseVisualStyleBackColor = true;
			this.btnPaste.Click += new System.EventHandler(this.btnPaste_Click);
			// 
			// btnCopyLive
			// 
			this.btnCopyLive.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnCopyLive.Location = new System.Drawing.Point(448, 97);
			this.btnCopyLive.Name = "btnCopyLive";
			this.btnCopyLive.Size = new System.Drawing.Size(75, 50);
			this.btnCopyLive.TabIndex = 10;
			this.btnCopyLive.Text = "Copy live players";
			this.btnCopyLive.UseVisualStyleBackColor = true;
			this.btnCopyLive.Click += new System.EventHandler(this.btnCopyLive_Click);
			// 
			// label2
			// 
			this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.label2.AutoSize = true;
			this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label2.Location = new System.Drawing.Point(150, 10);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(85, 16);
			this.label2.TabIndex = 12;
			this.label2.Text = "The Roster";
			// 
			// acMenu
			// 
			this.acMenu.AllowsTabKey = true;
			this.acMenu.AppearInterval = 100;
			this.acMenu.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
			this.acMenu.ImageList = null;
			this.acMenu.Items = new string[0];
			this.acMenu.MinFragmentLength = 3;
			this.acMenu.SearchPattern = ".";
			this.acMenu.TargetControlWrapper = null;
			// 
			// grdRoster
			// 
			this.grdRoster.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.grdRoster.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.grdRoster.DisableArrowNavigationMode = false;
			this.grdRoster.Location = new System.Drawing.Point(12, 30);
			this.grdRoster.Name = "grdRoster";
			this.grdRoster.Size = new System.Drawing.Size(430, 419);
			this.grdRoster.TabIndex = 2;
			this.grdRoster.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.grdRoster_CellEndEdit);
			this.grdRoster.RowValidating += new System.Windows.Forms.DataGridViewCellCancelEventHandler(this.grdRoster_RowValidating);
			// 
			// btnKillSub
			// 
			this.btnKillSub.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnKillSub.Location = new System.Drawing.Point(447, 344);
			this.btnKillSub.Name = "btnKillSub";
			this.btnKillSub.Size = new System.Drawing.Size(75, 50);
			this.btnKillSub.TabIndex = 13;
			this.btnKillSub.Text = "Kill / Sub";
			this.btnKillSub.UseVisualStyleBackColor = true;
			this.btnKillSub.Click += new System.EventHandler(this.btnKillSub_Click);
			// 
			// PlayerList
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 14F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(534, 476);
			this.ControlBox = false;
			this.Controls.Add(this.btnKillSub);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.btnCopyLive);
			this.Controls.Add(this.btnPaste);
			this.Controls.Add(this.btnDelete);
			this.Controls.Add(this.grdRoster);
			this.Controls.Add(this.btnSubmitPlayers);
			this.Font = new System.Drawing.Font("Times New Roman", 8.25F);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "PlayerList";
			this.Text = "Add / Remove Players";
			((System.ComponentModel.ISupportInitialize)(this.grdRoster)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnSubmitPlayers;
        private NoArrowNavigateDataGridView grdRoster;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnPaste;
        private System.Windows.Forms.Button btnCopyLive;
        private System.Windows.Forms.Label label2;
        private AutocompleteMenuNS.AutocompleteMenu acMenu;
        private System.Windows.Forms.Button btnKillSub;
    }
}