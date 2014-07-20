namespace RickyRaccoon
{
    partial class OP
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
            this.textOP = new System.Windows.Forms.TextBox();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.btnMakeOP = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnDontMakeOP = new System.Windows.Forms.Button();
            this.btnCopyOP = new System.Windows.Forms.Button();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.textOP, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1134, 501);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // textOP
            // 
            this.textOP.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textOP.Location = new System.Drawing.Point(3, 3);
            this.textOP.Multiline = true;
            this.textOP.Name = "textOP";
            this.textOP.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textOP.Size = new System.Drawing.Size(1128, 460);
            this.textOP.TabIndex = 0;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 4;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.Controls.Add(this.btnMakeOP, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.btnCancel, 3, 0);
            this.tableLayoutPanel2.Controls.Add(this.btnDontMakeOP, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.btnCopyOP, 2, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 469);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(1128, 29);
            this.tableLayoutPanel2.TabIndex = 1;
            // 
            // btnMakeOP
            // 
            this.btnMakeOP.Location = new System.Drawing.Point(3, 3);
            this.btnMakeOP.Name = "btnMakeOP";
            this.btnMakeOP.Size = new System.Drawing.Size(135, 23);
            this.btnMakeOP.TabIndex = 0;
            this.btnMakeOP.Text = "Make This OP and Rand";
            this.btnMakeOP.UseVisualStyleBackColor = true;
            this.btnMakeOP.Click += new System.EventHandler(this.btnMakeOP_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(470, 3);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(215, 23);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Cancel Rand (No PMs or OPs will be sent)";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnDontMakeOP
            // 
            this.btnDontMakeOP.Location = new System.Drawing.Point(144, 3);
            this.btnDontMakeOP.Name = "btnDontMakeOP";
            this.btnDontMakeOP.Size = new System.Drawing.Size(164, 23);
            this.btnDontMakeOP.TabIndex = 3;
            this.btnDontMakeOP.Text = "Don\'t Make OP, But Send PMs";
            this.btnDontMakeOP.UseVisualStyleBackColor = true;
            this.btnDontMakeOP.Click += new System.EventHandler(this.btnDontMakeOP_Click);
            // 
            // btnCopyOP
            // 
            this.btnCopyOP.Location = new System.Drawing.Point(314, 3);
            this.btnCopyOP.Name = "btnCopyOP";
            this.btnCopyOP.Size = new System.Drawing.Size(150, 23);
            this.btnCopyOP.TabIndex = 1;
            this.btnCopyOP.Text = "Copy This OP to Clipboard";
            this.btnCopyOP.UseVisualStyleBackColor = true;
            this.btnCopyOP.Click += new System.EventHandler(this.btnCopyOP_Click);
            // 
            // OP
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1134, 501);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "OP";
            this.Text = "Edit OP and Send";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TextBox textOP;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Button btnMakeOP;
        private System.Windows.Forms.Button btnDontMakeOP;
        private System.Windows.Forms.Button btnCopyOP;
        private System.Windows.Forms.Button btnCancel;
    }
}