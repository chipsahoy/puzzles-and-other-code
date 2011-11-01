using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FennecFox
{
    /// <summary>
    /// http://www.codemeit.com/linq/c-array-delimited-tostring.html
    /// </summary>
    public static class ExtensionMethods
    {
        public static void Sort(this DataGridView grdVotes)
        {
            if (grdVotes.SortedColumn != null)
            {
                grdVotes.Sort(grdVotes.SortedColumn,
                              grdVotes.SortOrder == SortOrder.Descending
                                  ? ListSortDirection.Descending
                                  : ListSortDirection.Ascending);
            }
            else
            {
                grdVotes.Sort(grdVotes.Columns[0], ListSortDirection.Ascending);
            }
        }

        public static string ToString<T>(this IEnumerable<T> source, string separator)
        {
            if (source == null)
                throw new ArgumentException("Parameter source can not be null.");

            if (string.IsNullOrEmpty(separator))
                throw new ArgumentException("Parameter separator can not be null or empty.");

            string[] array = source.Where(n => n != null).Select(n => n.ToString()).ToArray();

            return string.Join(separator, array);
        }

        // for interface IEnumerable
        public static string ToString(this IEnumerable source, string separator)
        {
            if (source == null)
                throw new ArgumentException("Parameter source can not be null.");

            if (string.IsNullOrEmpty(separator))
                throw new ArgumentException("Parameter separator can not be null or empty.");

            string[] array = source.Cast<object>().Where(n => n != null).Select(n => n.ToString()).ToArray();

            return string.Join(separator, array);
        }
    }
}
