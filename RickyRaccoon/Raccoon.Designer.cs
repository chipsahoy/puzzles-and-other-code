namespace RickyRaccoon
{
    partial class Raccoon
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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.btnPasteList = new System.Windows.Forms.Button();
            this.btnCheckNames = new System.Windows.Forms.Button();
            this.btnDoIt = new System.Windows.Forms.Button();
            this.txtGameTitle = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.cmbRoleSet = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtPlayerCount = new System.Windows.Forms.TextBox();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.label2, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.label3, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.txtPlayerCount, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.txtGameTitle, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.cmbRoleSet, 1, 3);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 6;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(624, 442);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // btnPasteList
            // 
            this.btnPasteList.Location = new System.Drawing.Point(3, 3);
            this.btnPasteList.Name = "btnPasteList";
            this.btnPasteList.Size = new System.Drawing.Size(75, 23);
            this.btnPasteList.TabIndex = 0;
            this.btnPasteList.Text = "Paste List";
            this.btnPasteList.UseVisualStyleBackColor = true;
            this.btnPasteList.Click += new System.EventHandler(this.btnPasteList_Click);
            // 
            // btnCheckNames
            // 
            this.btnCheckNames.Enabled = false;
            this.btnCheckNames.Location = new System.Drawing.Point(182, 3);
            this.btnCheckNames.Name = "btnCheckNames";
            this.btnCheckNames.Size = new System.Drawing.Size(100, 23);
            this.btnCheckNames.TabIndex = 1;
            this.btnCheckNames.Text = "Check Names";
            this.btnCheckNames.UseVisualStyleBackColor = true;
            this.btnCheckNames.Click += new System.EventHandler(this.btnCheckNames_Click);
            // 
            // btnDoIt
            // 
            this.btnDoIt.Enabled = false;
            this.btnDoIt.Location = new System.Drawing.Point(361, 3);
            this.btnDoIt.Name = "btnDoIt";
            this.btnDoIt.Size = new System.Drawing.Size(75, 23);
            this.btnDoIt.TabIndex = 4;
            this.btnDoIt.Text = "DO IT!";
            this.btnDoIt.UseVisualStyleBackColor = true;
            this.btnDoIt.Click += new System.EventHandler(this.btnDoIt_Click);
            // 
            // txtGameTitle
            // 
            this.txtGameTitle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtGameTitle.Location = new System.Drawing.Point(78, 64);
            this.txtGameTitle.Name = "txtGameTitle";
            this.txtGameTitle.Size = new System.Drawing.Size(543, 20);
            this.txtGameTitle.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(11, 61);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(61, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Game Title:";
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(21, 87);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(51, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Role Set:";
            // 
            // cmbRoleSet
            // 
            this.cmbRoleSet.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cmbRoleSet.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbRoleSet.Enabled = false;
            this.cmbRoleSet.FormattingEnabled = true;
            this.cmbRoleSet.Location = new System.Drawing.Point(78, 90);
            this.cmbRoleSet.Name = "cmbRoleSet";
            this.cmbRoleSet.Size = new System.Drawing.Size(543, 21);
            this.cmbRoleSet.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 35);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(69, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Player count:";
            // 
            // txtPlayerCount
            // 
            this.txtPlayerCount.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtPlayerCount.Location = new System.Drawing.Point(78, 38);
            this.txtPlayerCount.Name = "txtPlayerCount";
            this.txtPlayerCount.ReadOnly = true;
            this.txtPlayerCount.Size = new System.Drawing.Size(543, 20);
            this.txtPlayerCount.TabIndex = 8;
            this.txtPlayerCount.TabStop = false;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 3;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 34F));
            this.tableLayoutPanel2.Controls.Add(this.btnPasteList, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.btnCheckNames, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.btnDoIt, 2, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(78, 3);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(543, 29);
            this.tableLayoutPanel2.TabIndex = 9;
            // 
            // Raccoon
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(624, 442);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "Raccoon";
            this.Text = "Ricky Raccoon\'s Randomizer of Doom";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TextBox txtGameTitle;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cmbRoleSet;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtPlayerCount;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Button btnPasteList;
        private System.Windows.Forms.Button btnCheckNames;
        private System.Windows.Forms.Button btnDoIt;
    }
}

