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
        private Werewolf.ElectionInfo _voteCount;
        Werewolf.AutoComplete _autoComplete;
        List<String> _acNames = new List<string>();

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
            acMenu.Selected += new EventHandler<AutocompleteMenuNS.SelectedEventArgs>(acMenu_Selected);
        }

        void acMenu_Selected(object sender, AutocompleteMenuNS.SelectedEventArgs e)
        {
            acMenu.Close();
        }

        void acMenu_Closed(object sender, EventArgs e)
        {
            grdRoster.DisableArrowNavigationMode = false;
        }

        void acMenu_Opening(object sender, CancelEventArgs e)
        {
            grdRoster.DisableArrowNavigationMode = true;
        }

        public PlayerList(Werewolf.ElectionInfo voteCount, Werewolf.AutoComplete autoComplete) : this()
        {
            _voteCount = voteCount;
            _autoComplete = autoComplete;
            CreateGridColumns();
            SetupGrid();
        }




        private void btnGetPosterList_Click(object sender, EventArgs e)
        {

        }

        private void AddToRoster(String name)
        {
            name = name.Split('\t')[0];
            name = name.Trim();
            if (_autoComplete.GetPosterId(name,
                (poster, id) =>
                {
                    if (id > 0)
                    {
                        AddConfirmedPoster(poster);
                    }
                    else
                    {
                        String msg = String.Format("\"{0}\" is not a poster. Check the spelling.", poster);
                        MessageBox.Show(msg);
                    }
                }
                ) > 0)
            {
                AddConfirmedPoster(name);
            }
        }
        void AddConfirmedPoster(String name)
        {
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

        private void ErasePlayer(DataGridViewRow row)
        {
            String name = row.Cells[(Int32)CounterColumn.Player].Value as String;
            if (name != null)
            {
                grdRoster.Rows.Remove(row);
            }
        }


        private void btnErase_Click(object sender, EventArgs e)
        {
            DataGridViewCell current =  grdRoster.CurrentCell;
            if (current != null)
            {
                ErasePlayer(current.OwningRow);
            }

            DataGridViewSelectedRowCollection rows = grdRoster.SelectedRows;
            foreach (DataGridViewRow row in rows)
            {
                ErasePlayer(row);
            }
        }

        private void btnEraseAll_Click(object sender, EventArgs e)
        {
            while (grdRoster.RowCount > 1)
            {
                ErasePlayer(grdRoster.Rows[0]);
            }
        }


        private enum CounterColumn
        {
            Player = 0,
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
            DataGridViewTextBoxColumn col;

            col = new DataGridViewTextBoxColumn();
            col.DataPropertyName = "Name";
            col.HeaderText = "Player";
            col.ReadOnly = false;
            col.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            col.Resizable = DataGridViewTriState.False;
            grdRoster.Columns.Insert((Int32)CounterColumn.Player, col);

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
                            _acNames.Clear();
                            acMenu.SetAutocompleteItems(_acNames);
                            acMenu.SetAutocompleteMenu(txt, acMenu);
                            _editControl = txt;
                        }
                    }
                    break;
            }
        }

        void txt_TextChanged(object sender, EventArgs e)
        {
            TextBox txt = sender as TextBox;
            if (txt != null)
            {
                Int32 fragLength = 3;
                String name = txt.Text;
                if (name.Length >= fragLength)
                {
                    //Trace.TraceInformation("text is '{0}'", txt.Text);
                    _autoComplete.LookupNameFragment(name, 
                        (fragment, posters) =>
                        {
                            if (grdRoster.CurrentCell == null)
                            {
                                return;
                            }
                            if (grdRoster.CurrentCell.ColumnIndex == (Int32)CounterColumn.Player)
                            {
                                TextBox txtCurrent = grdRoster.EditingControl as TextBox;
                                if (txtCurrent != null)
                                {
                                    HashSet<String> oldNames = new HashSet<string>(_acNames);
                                    HashSet<String> addNames = new HashSet<string>();
                                    foreach (Poster p in posters)
                                    {
                                        addNames.Add(p.Name);
                                    }
                                    HashSet<String> deleteNames = new HashSet<string>(oldNames);
                                    deleteNames.ExceptWith(addNames);
                                    addNames.ExceptWith(oldNames);
                                    _acNames.AddRange(addNames);
                                    _acNames.RemoveAll((item) => 
                                    {
                                        Boolean rc = deleteNames.Contains(item);
                                        return rc; 
                                    });
                                    Trace.TraceInformation("Fragment: {0}", fragment);
                                    acMenu.SetAutocompleteItems(_acNames);
                                }
                            }
                        }
                    );
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
                live.Sort();
                String players = String.Join("\r\n", live) + "\r\n";
                Clipboard.SetData(DataFormats.StringFormat, players.ToString());
            }
        }

        private void grdRoster_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            acMenu.TargetControlWrapper = null;
            acMenu.SetAutocompleteMenu(_editControl, null);
        }

        private void grdRoster_RowValidating(object sender, DataGridViewCellCancelEventArgs e)
        {
            DataGridViewRow r = grdRoster.Rows[e.RowIndex];
            DataGridViewCell c = r.Cells[(Int32)CounterColumn.Player];
            object oValue = c.FormattedValue;
            if ((oValue != null) && (oValue as String != String.Empty) && (r.Index != -1) && (c.RowIndex != -1))
            {
                c.ReadOnly = false;
                String name = oValue as String;
                if (_voteCount.GetPlayerId(name) < 0)
                {
                    // never heard of this guy.
                    c.ErrorText = "Unknown name";
                }
                else
                {
                    c.ErrorText = null;
                    c.ReadOnly = true;
                }
            }
        }



    }
}
