using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using POG.Forum;
using System.Diagnostics;

namespace POG.FennecFox
{
    public partial class PlayerList : Form
    {
        private Werewolf.VoteCount _voteCount;
        Werewolf.AutoComplete _autoComplete;

        public IEnumerable<String> Players
        {
            get
            {
                String players = "";// txtNewPlayers.Text;
                List<String> rawList = players.Split(
                    new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(p => p.Trim())
                    .Distinct().ToList();
                return rawList;
            }
        }

        public PlayerList()
        {
            InitializeComponent();
            acMenu.Opening += new EventHandler<CancelEventArgs>(acMenu_Opening);
            acMenu.Closed += new EventHandler(acMenu_Closed);
        }

        void acMenu_Closed(object sender, EventArgs e)
        {
            grdRoster.DisableArrowNavigationMode = false;
        }

        void acMenu_Opening(object sender, CancelEventArgs e)
        {
            grdRoster.DisableArrowNavigationMode = true;
        }

        public PlayerList(Werewolf.VoteCount voteCount, Werewolf.AutoComplete autoComplete) : this()
        {
            _voteCount = voteCount;
            _autoComplete = autoComplete;
            _autoComplete.CompletionList += new EventHandler<NameCompletionEventArgs>(_autoComplete_CompletionList);
            CreateGridColumns();
            SetupGrid();
        }


        void _autoComplete_CompletionList(object sender, NameCompletionEventArgs e)
        {
            if (grdRoster.CurrentCell.ColumnIndex == (Int32)CounterColumn.Player)
            {
                TextBox txt = grdRoster.EditingControl as TextBox;
                if (txt != null)
                {
                    if (e.Fragment == _autoCompleteFragment)
                    {
                        List<String> names = new List<string>();
                        foreach (Poster p in e.Names)
                        {
                            names.Add(p.Name);
                        }
                        Trace.TraceInformation("Fragment: {0}", e.Fragment);
                        acMenu.SetAutocompleteItems(names);
                    }
                }
            }
        }


        private void btnGetPosterList_Click(object sender, EventArgs e)
        {

        }

        private void AddToRoster(String name)
        {
            name = name.Trim();
            foreach (CensusEntry e in _voteCount.Census)
            {
                if (e.Name == name)
                {
                    return;
                }
            }
            CensusEntry ce = new CensusEntry();
            ce.Name = name;
            ce.Alive = "Alive";
            _voteCount.Census.Add(ce);
        }

        private void MoveLeft(DataGridViewRow row)
        {
            String name = row.Cells[(Int32)CounterColumn.Player].Value as String;
            grdRoster.Rows.Remove(row);
        }
        private void btnOneLeft_Click(object sender, EventArgs e)
        {
            DataGridViewCell current =  grdRoster.CurrentCell;
            if (current != null)
            {
                MoveLeft(current.OwningRow);
            }

            DataGridViewSelectedRowCollection rows = grdRoster.SelectedRows;
            foreach (DataGridViewRow row in rows)
            {
                MoveLeft(row);
            }
        }

        private enum CounterColumn
        {
            Player = 0,
            Alive,
            PostNumber,
            Replacement,
        };
        
        private void SetupGrid()
        {
            if (_voteCount == null)
            {
                return;
            }
            BindingSource bs = new BindingSource();
            bs.DataSource = _voteCount.Census;
            grdRoster.DataSource = null;

            grdRoster.DataSource = bs;
            grdRoster.Sort(grdRoster.Columns[(Int32)CounterColumn.Player], ListSortDirection.Ascending);
        }

        private void CreateGridColumns()
        {
            grdRoster.AutoGenerateColumns = false;
            grdRoster.EditMode = DataGridViewEditMode.EditOnEnter;
            DataGridViewComboBoxColumn colCB;
            DataGridViewTextBoxColumn col;

            col = new DataGridViewTextBoxColumn();
            col.DataPropertyName = "Name";
            col.HeaderText = "Player";
            col.ReadOnly = false;
            col.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            col.Resizable = DataGridViewTriState.False;
            grdRoster.Columns.Insert((Int32)CounterColumn.Player, col);

            colCB = new DataGridViewComboBoxColumn();
            colCB.DataPropertyName = "Alive";
            colCB.HeaderText = "Alive?";
            colCB.DisplayStyleForCurrentCellOnly = true;
            colCB.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            colCB.Resizable = DataGridViewTriState.False;
            colCB.DataSource = new String[] { "Alive", "Dead", "Sub Out" };
            colCB.DefaultCellStyle.NullValue = "Alive";
            grdRoster.Columns.Insert((Int32)CounterColumn.Alive, colCB);

            col = new DataGridViewTextBoxColumn();
            col.DataPropertyName = "EndPostNumber";
            col.HeaderText = "Post #";
            col.ReadOnly = true;
            col.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            col.Resizable = DataGridViewTriState.False;
            grdRoster.Columns.Insert((Int32)CounterColumn.PostNumber, col);

            col = new DataGridViewTextBoxColumn();
            col.DataPropertyName = "Replacement";
            col.HeaderText = "Replaced by";
            col.ReadOnly = true;
            col.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            col.Resizable = DataGridViewTriState.False;
            grdRoster.Columns.Insert((Int32)CounterColumn.Replacement, col);

            grdRoster.CellValidating += new DataGridViewCellValidatingEventHandler(grdRoster_CellValidating);
            grdRoster.EditingControlShowing += new DataGridViewEditingControlShowingEventHandler(grdRoster_EditingControlShowing);
            
        }
        TextBox _editControl;
        void grdRoster_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            switch (grdRoster.CurrentCell.ColumnIndex)
            {
                case (Int32)CounterColumn.Player:
                    {
                        TextBox txt = e.Control as TextBox;
                        if (txt != null)
                        {
                            txt.TextChanged -= txt_TextChanged;
                            txt.TextChanged += new EventHandler(txt_TextChanged);
                            acMenu.SetAutocompleteMenu(txt, acMenu);
                            _editControl = txt;
                        }
                    }
                    break;
            }
        }
        String _autoCompleteFragment = String.Empty;

        void txt_TextChanged(object sender, EventArgs e)
        {
            TextBox txt = sender as TextBox;
            if (txt != null)
            {
                Int32 fragLength = 3;
                String name = txt.Text;
                if (name.Length >= fragLength)
                {
                    if (name != _autoCompleteFragment)
                    {
                        _autoCompleteFragment = name;
                        Trace.TraceInformation("text is '{0}'", txt.Text);
                        _autoComplete.SetNameFragment(name);
                    }
                }
            }
        }

        void grdRoster_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            e.Cancel = false;
            DataGridViewRow r = grdRoster.Rows[e.RowIndex];
            object oValue = e.FormattedValue;
            switch (e.ColumnIndex)
            {
                case (Int32)CounterColumn.Player:
                    {
                        if ((oValue != null) && (oValue as String != String.Empty))
                        {
                            r.Cells[(Int32)CounterColumn.Alive].ReadOnly = false;
                            String name = oValue as String;
                            if (_voteCount.GetPlayerId(name) < 0)
                            {
                                // never heard of this guy.
                                r.Cells[e.ColumnIndex].ErrorText = "Unknown name";
                            }
                            else
                            {
                                r.Cells[e.ColumnIndex].ErrorText = null;
                            }
                        }
                        else
                        {
                            r.Cells[(Int32)CounterColumn.Alive].ReadOnly = true;
                        }
                    }
                    break;

                case (Int32)CounterColumn.Alive:
                    {
                        String status = oValue as String;
                        if (status == "Alive")
                        {
                            r.Cells[(Int32)CounterColumn.PostNumber].ReadOnly = true;
                            r.Cells[(Int32)CounterColumn.PostNumber].Value = String.Empty;
                            r.Cells[(Int32)CounterColumn.Replacement].ReadOnly = true;
                            r.Cells[(Int32)CounterColumn.Replacement].Value = String.Empty;
                        }
                        else
                        {
                            r.Cells[(Int32)CounterColumn.PostNumber].ReadOnly = false;
                            if (status != "Dead")
                            {
                                r.Cells[(Int32)CounterColumn.Replacement].ReadOnly = false;
                            }
                        }
                    }
                    break;

                case (Int32)CounterColumn.PostNumber:
                    {
                    }
                    break;

                case (Int32)CounterColumn.Replacement:
                    {
                        if (oValue != null)
                        {
                            String name = oValue as String;
                            AddToRoster(name);
                        }
                    }
                    break;
            }
        }

        private void btnPaste_Click(object sender, EventArgs e)
        {
            object o = Clipboard.GetData(DataFormats.Text);
            if (o != null)
            {
                String clip = o as String;
                String[] lines = clip.Split(new String[] { "\r\n", "\r" }, StringSplitOptions.RemoveEmptyEntries);
                foreach (String line in lines)
                {
                    String name = line.Trim();
                    AddToRoster(name);
                }
            }
        }

        private void btnCopyLive_Click(object sender, EventArgs e)
        {
            List<String> live = new List<string>();
            foreach (CensusEntry ce in _voteCount.Census)
            {
                if (ce.Name == null)
                {
                    continue;
                }
                String name = ce.Name.Trim();
                if (name != String.Empty)
                {
                    live.Add(name);
                }
            }
            if (live.Count > 0)
            {
                String players = String.Join("\r\n", live) + "\r\n";
                Clipboard.SetData(DataFormats.StringFormat, players.ToString());
            }
        }

        private void grdRoster_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            acMenu.TargetControlWrapper = null;
            acMenu.SetAutocompleteMenu(_editControl, null);
        }

    }
}
