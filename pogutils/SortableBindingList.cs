#region Using directives

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

#endregion

namespace POG.Utils
{

  public class SortableBindingList<T> : BindingList<T> {

    #region Sorting

    private bool _isSorted;
    private ListSortDirection _sortDirection;
    private PropertyDescriptor _sortProperty;

    protected override bool SupportsSortingCore {
      get { return true; }
    }

    protected override void ApplySortCore(PropertyDescriptor property, ListSortDirection direction) {

      // Get list to sort
      List<T> items = this.Items as List<T>;

      // Apply and set the sort, if items to sort
      if( items != null ) {
        PropertyComparer<T> pc = new PropertyComparer<T>(property, direction);
        items.Sort(pc);
        _sortDirection = direction;
        _sortProperty = property;
        _isSorted = true;
      }
      else {
        _isSorted = false;
      }

      // Let bound controls know they should refresh their views
      this.OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
    }

    protected override bool IsSortedCore {
      get { return _isSorted; }
    }

    protected override void RemoveSortCore() {
      _isSorted = false;
    }
    protected override ListSortDirection SortDirectionCore
    {
        get { return _sortDirection; }
    }

    protected override PropertyDescriptor SortPropertyCore
    {
        get { return _sortProperty; }
    }

    #endregion

    #region Persistence

    // NOTE: BindingList<T> is not serializable but List<T> is

    public void Save(string filename) {
      BinaryFormatter formatter = new BinaryFormatter();
      using( FileStream stream = new FileStream(filename, FileMode.Create) ) {
        // Serialize data list items
        formatter.Serialize(stream, (List<T>)this.Items);
      }
    }

    public void Load(string filename) {

      this.ClearItems();

      if( File.Exists(filename) ) {
        BinaryFormatter formatter = new BinaryFormatter();
        using( FileStream stream = new FileStream(filename, FileMode.Open) ) {
          // Deserialize data list items
          ((List<T>)this.Items).AddRange((IEnumerable<T>)formatter.Deserialize(stream));
        }
      }

      // Let bound controls know they should refresh their views
      this.OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
    }

    #endregion
  }
}
