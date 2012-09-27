using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace POG.FennecFox
{
    internal class NoArrowNavigateDataGridView : DataGridView
    {
        public bool DisableArrowNavigationMode { get; set; }

        public NoArrowNavigateDataGridView()
        {
            DisableArrowNavigationMode = false;
        }

        protected override bool ProcessDialogKey(Keys keyData)
        {
            if (DisableArrowNavigationMode)
            {
                if (EditingControl != null)
                {
                    if (keyData == Keys.Enter)
                    {
                        return false;
                    }
                }
            }
            bool rc = base.ProcessDialogKey(keyData);
            return rc;
        }

        protected override bool ProcessDataGridViewKey(KeyEventArgs e)
        {
            if (DisableArrowNavigationMode)
            {
                if (EditingControl != null)
                {
                    if (e.KeyCode == Keys.Down || e.KeyCode == Keys.Up || e.KeyCode == Keys.Enter)
                    {
                        return false;
                    }
                }
            }
            bool rc = base.ProcessDataGridViewKey(e);
            return rc;
        }
    }
}
