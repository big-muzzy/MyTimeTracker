using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace MyTimeTracker
{
  public class ImportSettings : INotifyPropertyChanged
  {
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

    private MergeEnum mergeMode;
    public MergeEnum MergeMode
    {
      get { return mergeMode; }
      set
      {
        mergeMode = value;
        Notify("MergeMode");
      }
    }

    private string path;
    public string Path
    {
      get { return path; }
      set
      {
        path = value;
        Notify("Path");
      }
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
