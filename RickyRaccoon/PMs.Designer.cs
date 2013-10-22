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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.button1 = new System.Windows.Forms.Button();
            this.dataPMs = new System.Windows.Forms.DataGridView();
            this.colRecipients = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colSubject = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colBody = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataPMs)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Controls.Add(this.button1, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.dataPMs, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 94.69388F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.306122F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(995, 490);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(3, 467);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 20);
            this.button1.TabIndex = 0;
            this.button1.Text = "Send PMs";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // dataPMs
            // 
            this.dataPMs.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dataPMs.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataPMs.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colRecipients,
            this.colSubject,
            this.colBody});
            this.dataPMs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataPMs.Location = new System.Drawing.Point(3, 3);
            this.dataPMs.Name = "dataPMs";
            this.dataPMs.Size = new System.Drawing.Size(989, 458);
            this.dataPMs.TabIndex = 1;
            this.dataPMs.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellContentClick);
            // 
            // colRecipients
            // 
            this.colRecipients.HeaderText = "Recipients";
            this.colRecipients.Name = "colRecipients";
            this.colRecipients.Width = 200;
            // 
            // colSubject
            // 
            this.colSubject.HeaderText = "Subject";
            this.colSubject.Name = "colSubject";
            this.colSubject.Width = 200;
            // 
            // colBody
            // 
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.colBody.DefaultCellStyle = dataGridViewCellStyle2;
            this.colBody.HeaderText = "Body";
            this.colBody.Name = "colBody";
            this.colBody.Width = 500;
            // 
            // PMs
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(995, 490);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "PMs";
            this.Text = "PMs";
            this.tableLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataPMs)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.DataGridView dataPMs;
        private System.Windows.Forms.DataGridViewTextBoxColumn colRecipients;
        private System.Windows.Forms.DataGridViewTextBoxColumn colSubject;
        private System.Windows.Forms.DataGridViewTextBoxColumn colBody;




    }
}