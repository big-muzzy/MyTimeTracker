using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace MyTimeTracker
{
  public class ExportSettings: INotifyPropertyChanged
  {
    public ObservableCollection<CheckedListItem> Projects { get; set; }

    private bool removeAfterExport;
    public bool RemoveAfterExport
    {
      get { return removeAfterExport; }
      set
      {
        removeAfterExport = value;
        Notify("RemoveAfterExport");
      }
    }

    private string fileName;
    public string FileName
    {
      get { return fileName; }
      set
      {
        fileName = value;
        Notify("FileName");
      }
    }

    private string exportPath;
    public string ExportPath
    {
      get { return exportPath; }
      set
      {
        exportPath = value;
        Notify("ExportPath");
      }
    }

    public ExportSettings()
    {
      Projects = new ObservableCollection<CheckedListItem>();
    }

    public event PropertyChangedEventHandler PropertyChanged;

    private void Notify(string info)
    {
      if (PropertyChanged != null)
      {
        PropertyChanged(this, new PropertyChangedEventArgs(info));
      }
    }

  }
}
