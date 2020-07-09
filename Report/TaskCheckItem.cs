using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Collections.ObjectModel;


namespace MyTimeTracker
{
  public class TaskCheckItem : INotifyPropertyChanged
  {
    public Guid ID
    {
      get { return _id; }
      set
      {
        _id = value;
        Notify("ID");
      }
    }
    private Guid _id;

    public bool IsChecked
    {
      get { return isChecked; }
      set
      {
        isChecked = value;
        Notify("IsChecked");
      }
    }
    private bool isChecked;

    public string Name
    {
      get { return _name; }
      set
      {
        _name = value;
        Notify("Name");
      }
    }
    private string _name;


    public TaskCheckItem(Guid id, string name)
    {
      _id = id;
      _name = name;
    }

    public override string ToString()
    {
      return Name;
    }


    #region INotifyPropertyChanged

    private void Notify(string info)
    {
      if (PropertyChanged != null)
      {
        PropertyChanged(this, new PropertyChangedEventArgs(info));
      }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    #endregion
  }
}
