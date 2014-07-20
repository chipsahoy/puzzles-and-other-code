namespace sheepscore12
{
    partial class Form1
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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("Node0");
            System.Windows.Forms.TreeNode treeNode2 = new System.Windows.Forms.TreeNode("Node1");
            System.Windows.Forms.TreeNode treeNode3 = new System.Windows.Forms.TreeNode("Node2");
            System.Windows.Forms.TreeNode treeNode4 = new System.Windows.Forms.TreeNode("Node4");
            System.Windows.Forms.TreeNode treeNode5 = new System.Windows.Forms.TreeNode("Node5");
            System.Windows.Forms.TreeNode treeNode6 = new System.Windows.Forms.TreeNode("Node3", new System.Windows.Forms.TreeNode[] {
            treeNode4,
            treeNode5});
            this.numericUpDown_question = new System.Windows.Forms.NumericUpDown();
            this.label_question = new System.Windows.Forms.Label();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newSheepToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadSheepToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveSheepToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sheepToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editQuestionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addRemovePlayersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.scoringToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sheepToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.peehsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.scoringMethod1ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.scoringMethod2ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.heepToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.kangarooToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.manualToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.outputToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyAnswersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyScoresUpToThisQuestionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyPlayerListToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.hepToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.hepToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.RCM_group = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.RCM_group_correct = new System.Windows.Forms.ToolStripMenuItem();
            this.RCM_group_bonus = new System.Windows.Forms.ToolStripMenuItem();
            this.RCM_answer = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.RCM_answer_use_as_group_name = new System.Windows.Forms.ToolStripMenuItem();
            this.RCM_answer_bonus = new System.Windows.Forms.ToolStripMenuItem();
            this.label1 = new System.Windows.Forms.Label();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.RCM_move_to_new_group = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_question)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.RCM_group.SuspendLayout();
            this.RCM_answer.SuspendLayout();
            this.SuspendLayout();
            // 
            // numericUpDown_question
            // 
            this.numericUpDown_question.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.numericUpDown_question.Location = new System.Drawing.Point(49, 31);
            this.numericUpDown_question.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDown_question.Name = "numericUpDown_question";
            this.numericUpDown_question.Size = new System.Drawing.Size(49, 24);
            this.numericUpDown_question.TabIndex = 1;
            this.numericUpDown_question.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDown_question.ValueChanged += new System.EventHandler(this.numericUpDown_question_ValueChanged);
            // 
            // label_question
            // 
            this.label_question.AutoSize = true;
            this.label_question.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_question.Location = new System.Drawing.Point(104, 35);
            this.label_question.Name = "label_question";
            this.label_question.Size = new System.Drawing.Size(45, 16);
            this.label_question.TabIndex = 3;
            this.label_question.Text = "label1";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.sheepToolStripMenuItem,
            this.outputToolStripMenuItem,
            this.hepToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(400, 28);
            this.menuStrip1.TabIndex = 5;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newSheepToolStripMenuItem,
            this.loadSheepToolStripMenuItem,
            this.saveSheepToolStripMenuItem,
            this.toolStripMenuItem1,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(44, 24);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // newSheepToolStripMenuItem
            // 
            this.newSheepToolStripMenuItem.Name = "newSheepToolStripMenuItem";
            this.newSheepToolStripMenuItem.Size = new System.Drawing.Size(168, 24);
            this.newSheepToolStripMenuItem.Text = "New Reveal";
            this.newSheepToolStripMenuItem.Click += new System.EventHandler(this.newSheepToolStripMenuItem_Click);
            // 
            // loadSheepToolStripMenuItem
            // 
            this.loadSheepToolStripMenuItem.Name = "loadSheepToolStripMenuItem";
            this.loadSheepToolStripMenuItem.Size = new System.Drawing.Size(168, 24);
            this.loadSheepToolStripMenuItem.Text = "Load Reveal...";
            this.loadSheepToolStripMenuItem.Click += new System.EventHandler(this.loadSheepToolStripMenuItem_Click);
            // 
            // saveSheepToolStripMenuItem
            // 
            this.saveSheepToolStripMenuItem.Name = "saveSheepToolStripMenuItem";
            this.saveSheepToolStripMenuItem.Size = new System.Drawing.Size(168, 24);
            this.saveSheepToolStripMenuItem.Text = "Save Reveal...";
            this.saveSheepToolStripMenuItem.Click += new System.EventHandler(this.saveSheepToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(165, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(168, 24);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // sheepToolStripMenuItem
            // 
            this.sheepToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.editQuestionsToolStripMenuItem,
            this.addRemovePlayersToolStripMenuItem,
            this.scoringToolStripMenuItem});
            this.sheepToolStripMenuItem.Name = "sheepToolStripMenuItem";
            this.sheepToolStripMenuItem.Size = new System.Drawing.Size(62, 24);
            this.sheepToolStripMenuItem.Text = "Sheep";
            // 
            // editQuestionsToolStripMenuItem
            // 
            this.editQuestionsToolStripMenuItem.Name = "editQuestionsToolStripMenuItem";
            this.editQuestionsToolStripMenuItem.Size = new System.Drawing.Size(182, 24);
            this.editQuestionsToolStripMenuItem.Text = "Edit Questions...";
            this.editQuestionsToolStripMenuItem.Click += new System.EventHandler(this.editQuestionsToolStripMenuItem_Click);
            // 
            // addRemovePlayersToolStripMenuItem
            // 
            this.addRemovePlayersToolStripMenuItem.Name = "addRemovePlayersToolStripMenuItem";
            this.addRemovePlayersToolStripMenuItem.Size = new System.Drawing.Size(182, 24);
            this.addRemovePlayersToolStripMenuItem.Text = "Edit Entries...";
            this.addRemovePlayersToolStripMenuItem.Click += new System.EventHandler(this.addRemovePlayersToolStripMenuItem_Click);
            // 
            // scoringToolStripMenuItem
            // 
            this.scoringToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.sheepToolStripMenuItem1,
            this.peehsToolStripMenuItem,
            this.heepToolStripMenuItem,
            this.kangarooToolStripMenuItem,
            this.manualToolStripMenuItem});
            this.scoringToolStripMenuItem.Name = "scoringToolStripMenuItem";
            this.scoringToolStripMenuItem.Size = new System.Drawing.Size(182, 24);
            this.scoringToolStripMenuItem.Text = "Scoring";
            // 
            // sheepToolStripMenuItem1
            // 
            this.sheepToolStripMenuItem1.Name = "sheepToolStripMenuItem1";
            this.sheepToolStripMenuItem1.Size = new System.Drawing.Size(143, 24);
            this.sheepToolStripMenuItem1.Text = "Sheep";
            this.sheepToolStripMenuItem1.Click += new System.EventHandler(this.sheepToolStripMenuItem1_Click);
            // 
            // peehsToolStripMenuItem
            // 
            this.peehsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.scoringMethod1ToolStripMenuItem,
            this.scoringMethod2ToolStripMenuItem});
            this.peehsToolStripMenuItem.Name = "peehsToolStripMenuItem";
            this.peehsToolStripMenuItem.Size = new System.Drawing.Size(143, 24);
            this.peehsToolStripMenuItem.Text = "Peehs";
            // 
            // scoringMethod1ToolStripMenuItem
            // 
            this.scoringMethod1ToolStripMenuItem.Name = "scoringMethod1ToolStripMenuItem";
            this.scoringMethod1ToolStripMenuItem.Size = new System.Drawing.Size(196, 24);
            this.scoringMethod1ToolStripMenuItem.Text = "Scoring method 1";
            this.scoringMethod1ToolStripMenuItem.Click += new System.EventHandler(this.scoringMethod1ToolStripMenuItem_Click);
            // 
            // scoringMethod2ToolStripMenuItem
            // 
            this.scoringMethod2ToolStripMenuItem.Name = "scoringMethod2ToolStripMenuItem";
            this.scoringMethod2ToolStripMenuItem.Size = new System.Drawing.Size(196, 24);
            this.scoringMethod2ToolStripMenuItem.Text = "Scoring method 2";
            this.scoringMethod2ToolStripMenuItem.Click += new System.EventHandler(this.scoringMethod2ToolStripMenuItem_Click);
            // 
            // heepToolStripMenuItem
            // 
            this.heepToolStripMenuItem.Name = "heepToolStripMenuItem";
            this.heepToolStripMenuItem.Size = new System.Drawing.Size(143, 24);
            this.heepToolStripMenuItem.Text = "Heep";
            this.heepToolStripMenuItem.Click += new System.EventHandler(this.heepToolStripMenuItem_Click);
            // 
            // kangarooToolStripMenuItem
            // 
            this.kangarooToolStripMenuItem.Name = "kangarooToolStripMenuItem";
            this.kangarooToolStripMenuItem.Size = new System.Drawing.Size(143, 24);
            this.kangarooToolStripMenuItem.Text = "Kangaroo";
            this.kangarooToolStripMenuItem.Click += new System.EventHandler(this.kangarooToolStripMenuItem_Click_1);
            // 
            // manualToolStripMenuItem
            // 
            this.manualToolStripMenuItem.Name = "manualToolStripMenuItem";
            this.manualToolStripMenuItem.Size = new System.Drawing.Size(143, 24);
            this.manualToolStripMenuItem.Text = "Manual";
            this.manualToolStripMenuItem.Click += new System.EventHandler(this.manualToolStripMenuItem_Click);
            // 
            // outputToolStripMenuItem
            // 
            this.outputToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.copyAnswersToolStripMenuItem,
            this.copyScoresUpToThisQuestionToolStripMenuItem,
            this.copyPlayerListToolStripMenuItem});
            this.outputToolStripMenuItem.Name = "outputToolStripMenuItem";
            this.outputToolStripMenuItem.Size = new System.Drawing.Size(67, 24);
            this.outputToolStripMenuItem.Text = "Output";
            // 
            // copyAnswersToolStripMenuItem
            // 
            this.copyAnswersToolStripMenuItem.Name = "copyAnswersToolStripMenuItem";
            this.copyAnswersToolStripMenuItem.Size = new System.Drawing.Size(284, 24);
            this.copyAnswersToolStripMenuItem.Text = "Copy answers for this question";
            this.copyAnswersToolStripMenuItem.Click += new System.EventHandler(this.copyAnswersToolStripMenuItem_Click);
            // 
            // copyScoresUpToThisQuestionToolStripMenuItem
            // 
            this.copyScoresUpToThisQuestionToolStripMenuItem.Name = "copyScoresUpToThisQuestionToolStripMenuItem";
            this.copyScoresUpToThisQuestionToolStripMenuItem.Size = new System.Drawing.Size(284, 24);
            this.copyScoresUpToThisQuestionToolStripMenuItem.Text = "Copy scores up to this question";
            this.copyScoresUpToThisQuestionToolStripMenuItem.Click += new System.EventHandler(this.copyScoresUpToThisQuestionToolStripMenuItem_Click);
            // 
            // copyPlayerListToolStripMenuItem
            // 
            this.copyPlayerListToolStripMenuItem.Name = "copyPlayerListToolStripMenuItem";
            this.copyPlayerListToolStripMenuItem.Size = new System.Drawing.Size(284, 24);
            this.copyPlayerListToolStripMenuItem.Text = "Copy player list";
            this.copyPlayerListToolStripMenuItem.Click += new System.EventHandler(this.copyPlayerListToolStripMenuItem_Click);
            // 
            // hepToolStripMenuItem
            // 
            this.hepToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.hepToolStripMenuItem1,
            this.aboutToolStripMenuItem});
            this.hepToolStripMenuItem.Name = "hepToolStripMenuItem";
            this.hepToolStripMenuItem.Size = new System.Drawing.Size(53, 24);
            this.hepToolStripMenuItem.Text = "Help";
            // 
            // hepToolStripMenuItem1
            // 
            this.hepToolStripMenuItem1.Name = "hepToolStripMenuItem1";
            this.hepToolStripMenuItem1.Size = new System.Drawing.Size(128, 24);
            this.hepToolStripMenuItem1.Text = "Help...";
            this.hepToolStripMenuItem1.Click += new System.EventHandler(this.hepToolStripMenuItem1_Click);
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(128, 24);
            this.aboutToolStripMenuItem.Text = "About...";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // treeView1
            // 
            this.treeView1.AllowDrop = true;
            this.treeView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.treeView1.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.treeView1.FullRowSelect = true;
            this.treeView1.Location = new System.Drawing.Point(12, 61);
            this.treeView1.Name = "treeView1";
            treeNode1.Name = "Node0";
            treeNode1.Text = "Node0";
            treeNode2.Name = "Node1";
            treeNode2.Text = "Node1";
            treeNode3.Name = "Node2";
            treeNode3.Text = "Node2";
            treeNode4.Name = "Node4";
            treeNode4.Text = "Node4";
            treeNode5.Name = "Node5";
            treeNode5.Text = "Node5";
            treeNode6.Name = "Node3";
            treeNode6.Text = "Node3";
            this.treeView1.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode1,
            treeNode2,
            treeNode3,
            treeNode6});
            this.treeView1.Size = new System.Drawing.Size(376, 522);
            this.treeView1.TabIndex = 7;
            this.treeView1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.treeView1_MouseUp);
            this.treeView1.DragDrop += new System.Windows.Forms.DragEventHandler(this.treeView1_DragDrop);
            this.treeView1.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.treeView1_ItemDrag);
            this.treeView1.DragOver += new System.Windows.Forms.DragEventHandler(this.treeView1_DragOver);
            // 
            // RCM_group
            // 
            this.RCM_group.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.RCM_group_correct,
            this.RCM_group_bonus});
            this.RCM_group.Name = "RCM_group";
            this.RCM_group.Size = new System.Drawing.Size(153, 48);
            // 
            // RCM_group_correct
            // 
            this.RCM_group_correct.Name = "RCM_group_correct";
            this.RCM_group_correct.Size = new System.Drawing.Size(152, 22);
            this.RCM_group_correct.Text = "Mark Correct";
            this.RCM_group_correct.Click += new System.EventHandler(this.RCM_group_correct_Click);
            // 
            // RCM_group_bonus
            // 
            this.RCM_group_bonus.Name = "RCM_group_bonus";
            this.RCM_group_bonus.Size = new System.Drawing.Size(152, 22);
            this.RCM_group_bonus.Text = "Group Bonus...";
            this.RCM_group_bonus.Click += new System.EventHandler(this.RCM_group_bonus_Click);
            // 
            // RCM_answer
            // 
            this.RCM_answer.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.RCM_answer_use_as_group_name,
            this.RCM_move_to_new_group,
            this.RCM_answer_bonus});
            this.RCM_answer.Name = "RCM_answer";
            this.RCM_answer.Size = new System.Drawing.Size(182, 70);
            // 
            // RCM_answer_use_as_group_name
            // 
            this.RCM_answer_use_as_group_name.Name = "RCM_answer_use_as_group_name";
            this.RCM_answer_use_as_group_name.Size = new System.Drawing.Size(181, 22);
            this.RCM_answer_use_as_group_name.Text = "Use as Group Name";
            this.RCM_answer_use_as_group_name.Click += new System.EventHandler(this.RCM_answer_use_as_group_name_Click);
            // 
            // RCM_answer_bonus
            // 
            this.RCM_answer_bonus.Name = "RCM_answer_bonus";
            this.RCM_answer_bonus.Size = new System.Drawing.Size(181, 22);
            this.RCM_answer_bonus.Text = "Player Bonus...";
            this.RCM_answer_bonus.Click += new System.EventHandler(this.RCM_answer_bonus_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(12, 35);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(31, 16);
            this.label1.TabIndex = 8;
            this.label1.Text = "Q. #";
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // RCM_move_to_new_group
            // 
            this.RCM_move_to_new_group.Name = "RCM_move_to_new_group";
            this.RCM_move_to_new_group.Size = new System.Drawing.Size(181, 22);
            this.RCM_move_to_new_group.Text = "Move to New Group";
            this.RCM_move_to_new_group.Click += new System.EventHandler(this.RCM_move_to_new_group_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(400, 595);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.treeView1);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.label_question);
            this.Controls.Add(this.numericUpDown_question);
            this.MainMenuStrip = this.menuStrip1;
            this.MinimumSize = new System.Drawing.Size(300, 500);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Sheep Score 2012";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_question)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.RCM_group.ResumeLayout(false);
            this.RCM_answer.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.NumericUpDown numericUpDown_question;
        private System.Windows.Forms.Label label_question;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadSheepToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveSheepToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sheepToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editQuestionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addRemovePlayersToolStripMenuItem;
        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.ContextMenuStrip RCM_group;
        private System.Windows.Forms.ToolStripMenuItem RCM_group_correct;
        private System.Windows.Forms.ContextMenuStrip RCM_answer;
        private System.Windows.Forms.ToolStripMenuItem RCM_group_bonus;
        private System.Windows.Forms.ToolStripMenuItem RCM_answer_use_as_group_name;
        private System.Windows.Forms.ToolStripMenuItem RCM_answer_bonus;
        private System.Windows.Forms.ToolStripMenuItem outputToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyAnswersToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyScoresUpToThisQuestionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyPlayerListToolStripMenuItem;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ToolStripMenuItem scoringToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sheepToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem peehsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem scoringMethod1ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem scoringMethod2ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem heepToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem kangarooToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem manualToolStripMenuItem;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.ToolStripMenuItem hepToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem hepToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newSheepToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem RCM_move_to_new_group;
    }
}

