using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace sheepscore12
{

    using ShScoringMethod = ShGame.ShScoringMethod;
    using ShPlayer = ShGame.ShPlayer;
    using ShAnswer = ShGame.ShAnswer;
    using ShGroup = ShGame.ShGroup;
    using ShQuestion = ShGame.ShQuestion;
    
    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();
        }

        public class MiscConsts
        {
           public const string ListViewItem = "System.Windows.Forms.ListViewItem";
           public const string ListViewGroup = "System.Windows.Forms.ListViewGroup";
           public const int NewGroupTag = -1;
        }

        public static ShGame sg;

        public static int cur_q_index;
        public static ShScoringMethod curScoreMethod;

        editQuestions FormQuestions = new editQuestions();
        editAnswers FormAnswers = new editAnswers();
        bool sheep_modified;

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Text = Application.ProductName;
            SetScoringMethod(ShScoringMethod.Sheep);
            sg = new ShGame();
           
            redrawTreeView();
            SetTextForAllTreenodes();
            sheep_modified = false;
        }

        //Redraw all current groups/answers on the listview
        private void redrawTreeView()
        {

           
            treeView1.Nodes.Clear();

            //give instructions if no questions loaded
            if (sg == null || sg.Questions.Count == 0)
            {
                label_question.Text = "Click " + sheepToolStripMenuItem.Text + " > " +
                    editQuestionsToolStripMenuItem.Text + " to begin.";
                return;
            }

            //make sure we're on a valid question
            if (cur_q_index > sg.Questions.Count - 1)
                cur_q_index = sg.Questions.Count - 1;

            if (cur_q_index < 0)
                cur_q_index = 0;

            ShQuestion curQuestion = sg.Questions[cur_q_index];

            //make sure the updown and label is right
            numericUpDown_question.Minimum = 1;
            numericUpDown_question.Maximum = sg.Questions.Count;
            label_question.Text = curQuestion.Text;

            //give instructions if no players loaded
            if (sg.Players.Count == 0)
            {
    
                treeView1.Nodes.Add("Click " + sheepToolStripMenuItem.Text + " > " +
                    addRemovePlayersToolStripMenuItem.Text + " to add entries.");
                return;
            }

            TreeNode curGroup;
            TreeNode curItem;

            //loop through each group
            //text will be added later so don't bother with it in this function
            foreach (ShGroup grp in curQuestion.Groups)
            {
                //add group to listview.
                //use tag to keep track of group
                curGroup = new TreeNode("");
                curGroup.Tag = grp;
                treeView1.Nodes.Add(curGroup);

                //add each player's answer to listview.
                //use tag to keep track of answer 
                foreach (ShAnswer ans in grp.Answers)
                {
                    curItem = new TreeNode("");
                    curItem.Tag = ans;
                    curGroup.Nodes.Add(curItem);
                }
                //alternate colors each group
                //curGroup.Expand();

            }
            
            treeView1.TreeViewNodeSorter = new TreeNodeSorter();

            SetTextForAllTreenodes();

        }

        //change current question
        private void numericUpDown_question_ValueChanged(object sender, EventArgs e)
        {

            if (numericUpDown_question.Value > sg.Questions.Count)
            {
                numericUpDown_question.Value = numericUpDown_question.Maximum =
                    sg.Questions.Count;
            }
            cur_q_index = (int)numericUpDown_question.Value - 1;

            treeView1.BeginUpdate();
            redrawTreeView();
            treeView1.EndUpdate();

        }

        //exit program
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        //load edit questions window
        private void editQuestionsToolStripMenuItem_Click(object sender, EventArgs e)
        {

            FormQuestions.StartPosition = FormStartPosition.CenterParent;
            FormQuestions.ShowDialog();

            if (FormQuestions.DialogResult != DialogResult.OK)
                return;


            //modify questions and
            //resize answers, groupNames, and updown here
            string tempQstr = FormQuestions.textBox1.Text;
            tempQstr.Trim();
            string[] newQuestions = tempQstr.Split(new string[] { Environment.NewLine },
                StringSplitOptions.RemoveEmptyEntries);

            bool tempProceed = true;
            if (newQuestions.Length < sg.Questions.Count)
            {
                if (MessageBox.Show("You are reducing the number of questions."
                    + " This will delete some answers. Continue?",
                    "", MessageBoxButtons.OKCancel) != DialogResult.OK)
                {
                    tempProceed = false;
                }
            }
            if (tempProceed)
            {
                //if reducing questions, delete questions from the back
                if (newQuestions.Length < sg.Questions.Count)
                {
                    List<ShQuestion> questionsToDelete =
                        sg.Questions.GetRange(newQuestions.Length,
                        sg.Questions.Count - newQuestions.Length);
                    //remove answer references from player object
                    foreach (ShQuestion que in questionsToDelete)
                    {
                        sg.NiceDeleteQuestion(que);
                    }

                }
                //overwrite existing questions

                for (int iques = 0; iques < sg.Questions.Count; iques++)
                {
                    sg.Questions[iques].Text = newQuestions[iques];
                }

                //add new questions to the back if necessary
                if (newQuestions.Length > sg.Questions.Count)
                {
                    foreach (string newQtxt in newQuestions.Where(
                        (txt, i) => i >= sg.Questions.Count))
                    {
                        sg.NiceAddQuestion(newQtxt);
                    }

                }

            }

            sheep_modified = true;
            redrawTreeView();
        }

        //load edit players/answers window
        private void addRemovePlayersToolStripMenuItem_Click(object sender, EventArgs e)
        {

            FormAnswers.StartPosition = FormStartPosition.CenterParent;
            FormAnswers.ShowDialog();

            if (FormAnswers.DialogResult != DialogResult.OK)
                return;

            //get list of players that will be deleted later
            //by finding players whose positions are not in any of the ed_players original positions
            List<ShPlayer> playersToDelete = new List<ShPlayer>(
                    sg.Players.Where((ply, i) => FormAnswers.ed_players.All(
                        ep => ep.OriginalPosition != i)));

            //loop through each player from the editor
            foreach (editAnswers.EdPlayer ep in FormAnswers.ed_players)
            {
                //get list of answers as strings
                List<string> ansTxt = System.Text.RegularExpressions.Regex.Split(ep.Answers, Environment.NewLine).ToList();

                //if this player is new, add it
                if (ep.OriginalPosition == editAnswers.EdPlayer.NewPlayerOriginalPosition)
                {
                    ShPlayer newPlayer = sg.NiceAddPlayer(ep.Name, ansTxt.ToArray());
                    //guess groupings
                    foreach (ShAnswer ans in newPlayer.Answers)
                    {
                        sg.GuessGroup(ans);
                    }
                }
                else //this player is not new, all we have to do is update answers/names
                {
                    for (int iques = 0; iques < sg.Questions.Count; iques++)
                    {
                        //set some default text
                        string tempAnsTxt = "(blank)";
                        //check if we have text for this answer
                        if (iques < ansTxt.Count)
                            if (ansTxt[iques].Trim() != "")
                                tempAnsTxt = ansTxt[iques].Trim();
                        sg.Players[ep.OriginalPosition].Answers[iques].Text = tempAnsTxt;
                        sg.Players[ep.OriginalPosition].Name = ep.Name;
                    }
                }
            }

            //now delete all the players that no longer exist
            foreach (ShPlayer ply in playersToDelete)
            {
                sg.NiceDeletePlayer(ply);
            }

            sheep_modified = true;
            redrawTreeView();
        }

        //main drag/drop function
        private void treeView1_DragDrop(object sender, DragEventArgs e)
        {
            //stop sorting while dragging
            treeView1.TreeViewNodeSorter = null;

            Point cp = treeView1.PointToClient(new Point(e.X, e.Y));
            TreeNode destNode = treeView1.GetNodeAt(cp);

            //don't continue if not a valid node
            if (!e.Data.GetDataPresent(typeof(TreeNode)))
                return;

         //   treeView1.BeginUpdate();

            TreeNode movingNode = (TreeNode)e.Data.GetData(typeof(TreeNode));
            TreeNode prevParent, newParent;

            ShQuestion curQuestion = sg.Questions[cur_q_index];

            //different code depending on what type of thing we're dragging/dragging to

            if (movingNode.Tag.GetType() == typeof(ShAnswer)
                && destNode.Tag.GetType() == typeof(ShAnswer))
            {
                //moving an answer to another answer
                ShAnswer ansToMove = (ShAnswer)movingNode.Tag;
                ShAnswer destAnswer = (ShAnswer)destNode.Tag;
                ansToMove.ChangeGroup(destAnswer.Group);

                prevParent = movingNode.Parent;
                newParent = destNode.Parent;
                prevParent.Nodes.Remove(movingNode);
                newParent.Nodes.Add(movingNode);

            }
            else if (movingNode.Tag.GetType() == typeof(ShAnswer)
                && destNode.Tag.GetType() == typeof(ShGroup))
            {
                //moving an answer to another group
                ShAnswer ansToMove = (ShAnswer)movingNode.Tag;
                ShGroup destGroup = (ShGroup)destNode.Tag;
                ansToMove.ChangeGroup(destGroup);

                prevParent = movingNode.Parent;
                newParent = destNode;
                prevParent.Nodes.Remove(movingNode);
                newParent.Nodes.Add(movingNode);
            }
            else if (movingNode.Tag.GetType() == typeof(ShGroup)
                && destNode.Tag.GetType() == typeof(ShAnswer))
            {
                //moving a group to an answer
                ShGroup grpToMove = (ShGroup)movingNode.Tag;
                ShAnswer destAnswer = (ShAnswer)destNode.Tag;
                grpToMove.MergeToGroup(destAnswer.Group);

                prevParent = movingNode;
                newParent = destNode.Parent;

                List<TreeNode> ansNodes = new List<TreeNode>(prevParent.Nodes.Cast<TreeNode>());
                foreach (TreeNode nod in ansNodes)
                {
                    prevParent.Nodes.Remove(nod);
                    newParent.Nodes.Add(nod);
                }

            }
            else if (movingNode.Tag.GetType() == typeof(ShGroup)
                && destNode.Tag.GetType() == typeof(ShGroup))
            {
                //moving a group to a group
                ShGroup grpToMove = (ShGroup)movingNode.Tag;
                ShGroup destGroup = (ShGroup)destNode.Tag;
                grpToMove.MergeToGroup(destGroup);

                prevParent = movingNode;
                newParent = destNode;

                List<TreeNode> ansNodes = new List<TreeNode>(prevParent.Nodes.Cast<TreeNode>());
                foreach (TreeNode nod in ansNodes)
                {
                    prevParent.Nodes.Remove(nod);
                    newParent.Nodes.Add(nod);
                }
            }
            else
            { treeView1.EndUpdate(); return; }

            //if prevParent is empty, delete it
            if (prevParent.Nodes.Count == 0) prevParent.Remove();

            SetTextForAllTreenodes();
            sheep_modified = true;

            treeView1.EndUpdate();

        }

        //show appropriate cursor
        private void treeView1_DragOver(object sender, DragEventArgs e)
        {
            Point cp = treeView1.PointToClient(new Point(e.X, e.Y));
            TreeNode destNode = treeView1.GetNodeAt(cp);

           
            //show Move cursor if this is a valid Drop
            //otherwise show None cursor
            if (e.Data.GetDataPresent(typeof(TreeNode)))
            {
                
                TreeNode movingNode = (TreeNode)e.Data.GetData(typeof(TreeNode));

                //answers can be moved to another answer, group, or newgroup
                if (movingNode.Tag.GetType() == typeof(ShAnswer)
                    && (destNode.Tag.GetType() == typeof(ShAnswer) ||
                    destNode.Tag.GetType() == typeof(ShGroup)   ))
                {
                    e.Effect = DragDropEffects.Move;
                }
                else if (movingNode.Tag.GetType() == typeof(ShGroup)
                    && (destNode.Tag.GetType() == typeof(ShGroup)
                    || destNode.Tag.GetType() == typeof(ShAnswer)))
                {
                    //groups can be merged to another group 
                    e.Effect = DragDropEffects.Move;
                }
                else
                {
                    e.Effect = DragDropEffects.None;
                }

            }
            else
            {
                e.Effect = DragDropEffects.None;
            }


        }

        //starting a dragdrop
        private void treeView1_ItemDrag(object sender, ItemDragEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                treeView1.DoDragDrop(e.Item, DragDropEffects.Move);
            }
        }

        //returns text that should be displayed on this treenode
        private string TextForTreeNode(ShGroup grp)
        {
            if (grp.Answers.Count == 0)
                return grp.Text;

            string scoreString = "";

            try
            {
                if (curScoreMethod == ShScoringMethod.Manual)
                {
                    scoreString = "[" + grp.GroupBonus + "]";
                }
                else
                {
                    string bonus_text = "";
                    if (grp.GroupBonus > 0) bonus_text = " + " + grp.GroupBonus.ToString();
                    if (grp.GroupBonus < 0) bonus_text = " - " + (-grp.GroupBonus).ToString();
                    
                    scoreString = "[" + grp.Question.Scores(false)
                        [grp.Answers[0].Player].ToString()
                        + bonus_text + "]";
                }
            }
            catch
            {
                scoreString = "ERROR";
            }
            return grp.Text + " - " + (grp.Correct ? "" :
                ShGame.GetCorrectText(curScoreMethod, grp.Correct).ToUpper() + " - ") + scoreString;
        }

        private string TextForTreeNode(ShAnswer ans)
        {
            return ans.Text + " - " + ans.Player.Name + (ans.AnswerBonus==0?"":" (" +
                (ans.AnswerBonus>0?"+":"") + ans.AnswerBonus + ")");
            
        }
        
        //update text on all treenode items
        private void SetTextForAllTreenodes()
        {
            if (treeView1.Nodes.Count == 0) return;
            int i = 0;

            foreach (TreeNode grpNode in treeView1.Nodes)
            {
                if (grpNode.Tag == null) continue;

                if (grpNode.Tag.GetType() == typeof(ShGroup))
                {
                    ShGroup grp = (ShGroup)grpNode.Tag;
                    grpNode.Text = TextForTreeNode(grp);

                    foreach (TreeNode ansNode in grpNode.Nodes)
                    {
                        if (ansNode.Tag.GetType() == typeof(ShAnswer))
                        {
                            ansNode.Text=TextForTreeNode((ShAnswer)ansNode.Tag);
                            ansNode.ForeColor = treeView1.ForeColor;
                        }

                    }

                    if (i % 2 == 0)
                        grpNode.BackColor = Color.FromArgb(245, 245, 245);
                    else
                        grpNode.BackColor = Color.FromArgb(230, 230, 230);
                    

                    if (grp.Correct)
                        grpNode.ForeColor = Color.Blue;
                    else
                        grpNode.ForeColor = Color.DarkRed;

                }

                i++;
            }

        }
        
        public class TreeNodeSorter : System.Collections.IComparer //testey
        {
            public int Compare(object x, object y)
            {
                TreeNode tx = x as TreeNode;
                TreeNode ty = y as TreeNode;

                if (tx.Tag == null || ty.Tag == null)
                {
                    return 0;
                }

                if (tx.Tag.GetType() == typeof(ShGroup)
                    && ty.Tag.GetType() == typeof(ShGroup))
                {
                    ShGroup gx = (ShGroup)tx.Tag;
                    ShGroup gy = (ShGroup)ty.Tag;

                    return string.Compare(gx.Text, gy.Text);

                }
                else if (tx.Tag.GetType() == typeof(ShAnswer)
                    && ty.Tag.GetType() == typeof(ShAnswer))
                {
                    int temp = string.Compare(
                        ((ShAnswer)tx.Tag).Text,
                        ((ShAnswer)ty.Tag).Text,
                        true);

                    if (temp != 0) 
                        return temp;
                    else
                        return string.Compare(
                        ((ShAnswer)tx.Tag).Player.Name,
                        ((ShAnswer)ty.Tag).Player.Name,
                        true);
                }
                else
                    return 0;
            }
        }

        //show right-click menu
        //set Tag of RCM_group or RCM_answer to the clicked node
        private void treeView1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right 
                && treeView1.GetNodeAt(e.Location) != null
                && treeView1.GetNodeAt(e.Location).Tag != null)
                
            {
                TreeNode clicked_node = treeView1.GetNodeAt(e.Location);
                
                treeView1.SelectedNode = clicked_node;

                if (clicked_node.Tag.GetType() == typeof(ShGroup))
                {

                    ShGroup grp = (ShGroup)clicked_node.Tag;
                    RCM_group.Tag = clicked_node;
                    RCM_group_correct.Text = "Mark " +
                        ShGame.GetCorrectText(curScoreMethod, !grp.Correct);

                    RCM_group.Show(treeView1, e.Location);

                }
                else if (clicked_node.Tag.GetType() == typeof(ShAnswer))
                {
                    RCM_answer.Tag = clicked_node;
                    RCM_answer.Show(treeView1, e.Location);
                }

            }
        }

        //right click menu - mark group as (in)correct/(in)valid
        private void RCM_group_correct_Click(object sender, EventArgs e)
        {
            TreeNode clicked_node = RCM_group.Tag as TreeNode;
            if (clicked_node == null) return;
            ShGroup grp = clicked_node.Tag as ShGroup;
            if (grp == null) return;

            if (grp.Correct) grp.Correct = false;
            else grp.Correct = true;

            sheep_modified = true;
            SetTextForAllTreenodes();
        }

        //right click menu - set bonus score for a group
        private void RCM_group_bonus_Click(object sender, EventArgs e)
        {
            TreeNode clicked_node = RCM_group.Tag as TreeNode;
            if (clicked_node == null) return;
            ShGroup grp = clicked_node.Tag as ShGroup;
            if (grp == null) return;

            InputText IP = new InputText();
            IP.Text = "Bonus Score";
            IP.label1.Text = "Enter bonus score for " + grp.Text + ":";
            IP.textBox1.Text = grp.GroupBonus.ToString();
            IP.StartPosition = FormStartPosition.CenterParent;

            IP.ShowDialog();

            if (IP.DialogResult== DialogResult.OK)
            {
                if (IP.textBox1.Text.Trim() == "")
                    grp.GroupBonus = 0;

                try
                { grp.GroupBonus = Convert.ToInt32(IP.textBox1.Text); }
                catch { }

                sheep_modified = true;
                SetTextForAllTreenodes();
            }


        }

        //right click menu - set group name to this answer
        private void RCM_answer_use_as_group_name_Click(object sender, EventArgs e)
        {
            TreeNode clicked_node = RCM_answer.Tag as TreeNode;
            if (clicked_node == null) return;
            ShAnswer ans = clicked_node.Tag as ShAnswer;
            if (ans == null) return;

            ans.Group.Text = ans.Text;

            sheep_modified = true;
            SetTextForAllTreenodes();

        }

        //right click menu - set bonus score for this answer
        private void RCM_answer_bonus_Click(object sender, EventArgs e)
        {
            TreeNode clicked_node = RCM_answer.Tag as TreeNode;
            if (clicked_node == null) return;
            ShAnswer ans = clicked_node.Tag as ShAnswer;
            if (ans == null) return;

            InputText IP = new InputText();
            IP.Text = "Bonus Score";
            IP.label1.Text = "Enter bonus score for " + ans.Player.Name + ":";
            IP.textBox1.Text = ans.AnswerBonus.ToString();
            IP.StartPosition = FormStartPosition.CenterParent;

            IP.ShowDialog();

            if (IP.DialogResult == DialogResult.OK)
            {
                if (IP.textBox1.Text.Trim() == "")
                    ans.AnswerBonus = 0;

                try { ans.AnswerBonus = Convert.ToInt32(IP.textBox1.Text); }
                catch { }

                sheep_modified = true;
                SetTextForAllTreenodes();
            }

        }

        //right click menu - create a new group with this answer
        private void RCM_move_to_new_group_Click(object sender, EventArgs e)
        {
            TreeNode clicked_node = RCM_answer.Tag as TreeNode;
            if (clicked_node == null) return;
            ShAnswer ans = clicked_node.Tag as ShAnswer;
            if (ans == null) return;

            ShGroup newGroup = ans.Group.Question.StartNewGroup(ans.Text);
            ans.ChangeGroup(newGroup);

            TreeNode prevParent = clicked_node.Parent;
            TreeNode newParent = treeView1.Nodes.Add("b");
            newParent.Tag = newGroup;
            prevParent.Nodes.Remove(clicked_node);
            newParent.Nodes.Add(clicked_node);
            newParent.Expand();

            //if prevParent is empty, delete it
            if (prevParent.Nodes.Count == 0) prevParent.Remove();

            sheep_modified = true;
            SetTextForAllTreenodes();
        }

        //change scoring method and set checkmarks in menu
        private void SetScoringMethod(ShScoringMethod method)
        {
            curScoreMethod = method;
            if (sg != null) sg.Method = method;

            sheepToolStripMenuItem1.Checked=false;
            scoringMethod1ToolStripMenuItem.Checked=false;
            scoringMethod2ToolStripMenuItem.Checked=false;
            heepToolStripMenuItem.Checked=false;
            manualToolStripMenuItem.Checked=false;
            kangarooToolStripMenuItem.Checked=false;

            if (method == ShScoringMethod.Sheep)
                sheepToolStripMenuItem1.Checked = true;
            if (method == ShScoringMethod.Peehs1)
                scoringMethod1ToolStripMenuItem.Checked = true;
            if (method == ShScoringMethod.Peehs2)
                scoringMethod2ToolStripMenuItem.Checked = true;
            if (method == ShScoringMethod.Heep)
                heepToolStripMenuItem.Checked = true;
            if (method == ShScoringMethod.Kangaroo)
                kangarooToolStripMenuItem.Checked = true;
            if (method == ShScoringMethod.Manual)
                manualToolStripMenuItem.Checked = true;
            
            sheep_modified = true;
            SetTextForAllTreenodes();
        }

        //generate post for answers
        private void copyAnswersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (sg == null || sg.Players.Count == 0 || sg.Questions.Count == 0)
            {
                Clipboard.SetText("Either no questions or no players loaded.");
                return;
            }

            string txt = "[b]Question " + (cur_q_index + 1).ToString() + ": " +
                sg.Questions[cur_q_index].Text + "[/b]" + Environment.NewLine + Environment.NewLine;

            #region sheepscoreoutput
            List<ShGroup> validGroups = new List<ShGroup>(); //valid/correct groups
            List<ShGroup> validAces = new List<ShGroup>();   //valid/correct aces
            List<ShGroup> invalidGroups = new List<ShGroup>(); //invalid/incorrect groups

            if (curScoreMethod == ShScoringMethod.Sheep || curScoreMethod == ShScoringMethod.Heep
                || curScoreMethod == ShScoringMethod.Peehs1 || curScoreMethod == ShScoringMethod.Peehs2)
            {
                validGroups = (from g in sg.Questions[cur_q_index].Groups 
                               where (g.Answers.Count > 1) && (g.Correct)
                               orderby -g.Answers.Count select g).ToList();

                validAces = (from g in sg.Questions[cur_q_index].Groups
                             where (g.Answers.Count == 1) && (g.Correct) 
                             select g).ToList();

                invalidGroups = (from g in sg.Questions[cur_q_index].Groups
                                 where !g.Correct 
                                 select g).ToList();
            }
            else if (curScoreMethod == ShScoringMethod.Kangaroo)
            {
                validGroups = (from g in sg.Questions[cur_q_index].Groups 
                               where (g.Answers.Count > 1) && (!g.Correct)
                               orderby -g.Answers.Count select g).ToList();

                validAces = (from g in sg.Questions[cur_q_index].Groups
                             where (g.Answers.Count == 1) && (!g.Correct)
                             select g).ToList();

                invalidGroups = (from g in sg.Questions[cur_q_index].Groups
                                 where g.Correct
                                 select g).ToList();
            }
            else if (curScoreMethod == ShScoringMethod.Manual)
            {
                validGroups = (from g in sg.Questions[cur_q_index].Groups
                               where g.Correct
                               orderby -g.Answers.Count
                               select g).ToList();

                invalidGroups = (from g in sg.Questions[cur_q_index].Groups
                                 where !g.Correct
                                 select g).ToList();
            }

            foreach (ShGroup grp in validGroups)
            {

                txt += "[b]" + grp.Text + " - " + 
                    GetScoreOutputText(grp.GetScore(false), grp.GroupBonus,curScoreMethod) + 
                    "[/b]" + Environment.NewLine;

                foreach (ShAnswer ans in grp.Answers)
                    {
                        txt += ans.Player.Name + " " + 
                            GetBonusOutputText(ans.AnswerBonus, curScoreMethod)
                            + Environment.NewLine;
                    }
                    txt += Environment.NewLine;
            }

            if (validAces.Count >0)
            {
                txt += "[b]ACES:[/b]" + Environment.NewLine +Environment.NewLine;
            }

            foreach (ShGroup grp in validAces)
            {

                txt += "[b]" + grp.Text + "[/b] - " + grp.Answers[0].Player.Name + " "+
                        GetBonusOutputText(grp.GroupBonus + grp.Answers[0].AnswerBonus, curScoreMethod)
                        + Environment.NewLine;
            }

            if (invalidGroups.Count > 0)
            {
                txt += Environment.NewLine + "[b]" + ShGame.GetCorrectText(curScoreMethod,
                    (curScoreMethod == ShScoringMethod.Kangaroo ? true : false)).ToUpper() + "S[/b] - "
                    + invalidGroups[0].GetScore(false).ToString() + ":" + Environment.NewLine+Environment.NewLine;
            }
            foreach (ShGroup grp in invalidGroups)
            {
                foreach (ShAnswer ans in grp.Answers)
                {
                    txt += "[b]" + ans.Text + "[/b] - " + ans.Player.Name + " " +
                            GetBonusOutputText(grp.GroupBonus + grp.Answers[0].AnswerBonus, curScoreMethod)
                            + Environment.NewLine;
                }
            }

            #endregion

            Clipboard.SetText(txt);

        }

        //methods for getting score
        private string GetScoreOutputText(int score, int bonus, ShScoringMethod method)
        {
            if (method == ShScoringMethod.Manual) 
                return bonus.ToString();
            else 
                return score.ToString() + " " + GetBonusOutputText(bonus, method);
        }

        private string GetBonusOutputText(int bonus, ShScoringMethod method)
        {
            if (bonus == 0)
                return ""; 

            string bonuscolor = "Blue";

            if ((bonus < 0) ^ (method == ShScoringMethod.Peehs2 ||
                    method == ShScoringMethod.Peehs1))
            { bonuscolor = "Red"; }

            return "[COLOR=\"" + bonuscolor + "\"](" + (bonus > 0 ? "+" : "") + bonus + ")[/COLOR]";
        }

        //cgenerate post for score totals up to and including this question
        private void copyScoresUpToThisQuestionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (sg == null || sg.Players.Count == 0 || sg.Questions.Count == 0)
            {
                Clipboard.SetText("Either no questions or no players loaded.");
                return;
            }

            Dictionary<ShPlayer,int> curScores = 
                sg.Questions[cur_q_index].ScoreUpTo(true);

            foreach (ShPlayer plr in sg.Players)
            {
                if (!curScores.ContainsKey(plr))
                {
                   Clipboard.SetText("ERROR");
                   return;
                }
            }

            string txt = "[b]Scores after question " + (cur_q_index + 1).ToString() + ":[/b]"
                + Environment.NewLine + Environment.NewLine;

            if (ShGame.IsScoreDescending(curScoreMethod))
            {

                txt += string.Join(Environment.NewLine, (from p in sg.Players
                                                         orderby curScores[p] descending
                                                         select p.Name + " - " + curScores[p]).ToArray());
            }
            else
            {
                txt += string.Join(Environment.NewLine, (from p in sg.Players
                                                         orderby curScores[p] ascending
                                                         select p.Name + " - " + curScores[p]).ToArray());
            }

            Clipboard.SetText(txt);


        }

        //generate post for player list
        private void copyPlayerListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (sg == null || sg.Players.Count == 0 || sg.Questions.Count == 0)
            {
                Clipboard.SetText("Either no questions or no players loaded.");
                return;
            }

            string txt = "[b]" + sg.Players.Count.ToString() + " players:[/b]"
                + Environment.NewLine + Environment.NewLine +
                string.Join(Environment.NewLine, (from p in sg.Players
                                                  orderby p.Name
                                                  select p.Name).ToArray());

            Clipboard.SetText(txt);
        }

        //scoring type option menu items
        private void kangarooToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            SetScoringMethod(ShScoringMethod.Kangaroo);
        }

        private void sheepToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            SetScoringMethod(ShScoringMethod.Sheep);
        }

        private void scoringMethod1ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetScoringMethod(ShScoringMethod.Peehs1);
        }

        private void scoringMethod2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetScoringMethod(ShScoringMethod.Peehs2);
        }

        private void heepToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetScoringMethod(ShScoringMethod.Heep);
        }

        private void manualToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetScoringMethod(ShScoringMethod.Manual);
        }

        //save reveal file
        private void saveSheepToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowSaveSheepDialog();
        }

        private void ShowSaveSheepDialog()
        {
            saveFileDialog1.Title = "Save Sheep Scoring File";
            saveFileDialog1.Filter = "Sheep Score 2012 File|*.sheep12";
            saveFileDialog1.OverwritePrompt = true;

            if (saveFileDialog1.ShowDialog() != DialogResult.OK)
                return;

            try
            {
                XmlWriterSettings xws = new XmlWriterSettings();
                xws.Indent = true;
                using (XmlWriter xw = XmlWriter.Create(saveFileDialog1.FileName,xws))
                {
                    sg.WriteToXML(xw);
                }
            }
            catch
            {
                MessageBox.Show("Error writing to " + saveFileDialog1.FileName);
                return;
            }

            sheep_modified = false;
            MessageBox.Show("Successfully wrote " + saveFileDialog1.FileName);
        }

        private void ShowLoadSheepDialog()
        {
            openFileDialog1.Title = "Load Sheep Scoring File";
            openFileDialog1.Filter = "Sheep Score 2012 File|*.sheep12";
            openFileDialog1.CheckFileExists = true;

            if (openFileDialog1.ShowDialog() != DialogResult.OK)
                return;

            //temp game object - don't overwrite game in memory until we know load worked
            ShGame sg2 = new ShGame();

            try
            {
                XmlReaderSettings xrs = new XmlReaderSettings();
                using (XmlReader xr = XmlReader.Create(openFileDialog1.FileName))
                {
                    sg2.ReadFromXML(xr);
                }
            }
            catch
            {
                MessageBox.Show("Error reading from " + openFileDialog1.FileName);
                return;
            }

            sg = sg2;
            sheep_modified = false;

        }

        //load reveal file
        private void loadSheepToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CheckToSave();
            ShowLoadSheepDialog();
            redrawTreeView();
        }

        //about window
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {

            MessageBox.Show(Application.ProductName + " v" + Application.ProductVersion 
                + Environment.NewLine
                + "by DarkMagus" + Environment.NewLine
                + Environment.NewLine
                + "Visit twofive.ca/sheep for more info");
        }

        //show help
        private void hepToolStripMenuItem1_Click(object sender, EventArgs e)
        {

            try
            {
                System.Diagnostics.Process.Start("help.html");
             //   System.Diagnostics.Process.Start("http://twofive.ca/sheep");
            }
            catch
            {
                MessageBox.Show("Please visit twofive.ca/sheep for help");
            }
        }

        //closing the program - check if we want to save
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult d = CheckToSave();

            if (d == DialogResult.Cancel)
                e.Cancel=true;
        }

        //check if sheep is modified, ask user if he wants to save
        public DialogResult CheckToSave()
        {
            DialogResult d;
            if (sheep_modified)
            {
                d = MessageBox.Show("Do you want to save the current reveal?",
                    Application.ProductName, MessageBoxButtons.YesNoCancel);

                if (d == DialogResult.Yes)
                        ShowSaveSheepDialog();
             
            }
            else
            { d = DialogResult.Ignore; }

            return d;
        }

        //start new reveal
        private void newSheepToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult d = CheckToSave();

            if (d == DialogResult.Cancel)
                return;

            sg = new ShGame();
            redrawTreeView();
            sheep_modified = false;
        }


    }
}
