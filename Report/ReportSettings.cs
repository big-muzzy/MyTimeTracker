using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;

namespace MyTimeTracker
{
  public class ReportSettings : INotifyPropertyChanged
  {
    private DateTime _startDate;
    public DateTime StartDate
    {
      get { return _startDate; }
      set
      {
        _startDate = value;
        Notify("StartDate");
      }
    }

    private DateTime _endDate;
    public DateTime EndDate
    {
      get { return _endDate; }
      set
      {
        _endDate = value;
        Notify("EndDate");
      }
    }

    public IEnumerable<Guid> Tasks { get; set; }
    public FileInfo TemplateFileName { get; set; }

    private ReportInfo _info;
    public ReportInfo Info
    {
      get { return _info; }
      set {
        _info = value;
        Notify("Info");
      }
    }

    public ReportSettings()
    {
      Tasks = new List<Guid>();
      
    }

    public ReportSettings(DateTime startDate, DateTime endDate, IEnumerable<Guid> tasks, FileInfo template, ReportInfo info)
    {
      _startDate = startDate;
      _endDate = endDate;
      Tasks = tasks;
      TemplateFileName = template;
      _info = info;
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
