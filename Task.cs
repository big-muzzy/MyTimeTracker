using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace MyTimeTracker
{
  [Serializable]
  public class Task : INotifyPropertyChanged
  {
    private bool _isExpanded = true;

    [XmlElement("TaskName")]
    public String Name
    {
      get { return _name; }
      set
      {
        _name = value;
        Notify("Name");
      }
    }
    private string _name;

    [XmlIgnore]
    public Task Parent { get; set; }

    [XmlIgnore]
    public bool HasParent { get { return !(Parent == null); } }

    [XmlElement("Description")]
    public String Description
    {
      get { return _description; }
      set
      {
        _description = value;
        Notify("Description");
      }
    }
    private string _description;

    [XmlElement("Volume")]
    public int Volume
    {
      get { return _volume; }
      set
      {
        _volume = value;
        Notify("Volume");
      }
    }
    private int _volume;

    [XmlIgnore]
    public int ChildVolume
    {
      get
      {
        int result = 0;
        if (HasChild)
          result = TotalVolume - Volume;
        return result;
      }
    }

    [XmlIgnore]
    public int TotalVolume
    {
      get
      {
        int result = Volume;
        if (HasChild)
          foreach (var item in SubTasks)
            result += item.TotalVolume;
        return result;
      }
    }

    [XmlElement("Result")]
    public int Result
    {
      get { return _result; }
      set
      {
        _result = value;
        Notify("Result");
      }
    }
    private int _result;

    [XmlIgnore]
    public int ChildResult
    {
      get
      {
        int result = 0;
        if (HasChild)
          result = TotalResult - Result;
        return result;
      }
    }

    [XmlIgnore]
    public int TotalResult
    {
      get
      {
        int result = Result;
        if (HasChild)
          foreach (var item in SubTasks)
            result += item.TotalResult;
        return result;
      }
    }

    [XmlElement("TaskGUID")]
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

    [XmlArray("Periods"), XmlArrayItem("PeriodItem")]
    public ObservableCollection<Period> Periods
    {
      get { return _periods; }
      set
      {
        _periods = value;
        Notify("Periods");
      }
    }
    private ObservableCollection<Period> _periods;


    [XmlIgnore]
    public Period ActivePeriod
    {
      get { return _activePeriod; }
      set
      {
        _activePeriod = value;
        Notify("ActivePeriod");
      }
    }
    private Period _activePeriod;

    [XmlArray("TaskList"), XmlArrayItem("TaskItem")]
    public ObservableCollection<Task> SubTasks
    {
      get { return _subTasks; }
      set
      {
        _subTasks = value;
        Notify("SubTasks");
      }

    }
    private ObservableCollection<Task> _subTasks;

    [XmlElement("IsExpanded")]
    public bool IsExpanded
    {
      get
      {
        return _isExpanded;
      }
      set
      {
        _isExpanded = value;
        Notify("IsExpanded");
        OnIsVisibleChanged();
      }
    }

    private void OnIsVisibleChanged()
    {
      foreach (var subTask in SubTasks)
      {
        subTask.Notify("IsVisible");
      }
    }

    [XmlIgnore]
    public bool IsVisible
    {
      get
      {
        if (Parent == null) return true;
        else if (!Parent.IsVisible) return false;
        else if (Parent.IsExpanded) return true;
        else return false;
      }
    }

    public Task()
    {
      Periods = new ObservableCollection<Period>();
      SubTasks = new ObservableCollection<Task>();
      ID = Guid.NewGuid();
    }

    public Task(string name)
      : this()
    {
      Name = name;
      IsExpanded = true;
    }

    public void OpenNewPeriod()
    {
      if (ActivePeriod != null) if (ActivePeriod.State == States.Opened) ActivePeriod.Close();
      ActivePeriod = new Period(Periods.Count);
      Periods.Add(ActivePeriod);
      ActivePeriod.Open();
    }

    public void CloseActivePeriod()
    {
      if (ActivePeriod != null) if (ActivePeriod.State == States.Opened) ActivePeriod.Close();
    }

    public bool HasOpenedPeriod
    {
      get
      {
        if (ActivePeriod == null) return false;
        else return (ActivePeriod.State == States.Opened);
      }
    }

    [XmlIgnore]
    public DateTime TotalDuration
    {
      get
      {
        DateTime result = new DateTime();
        foreach (var item in Periods)
        {
          result = result.AddTicks(item.Duration.Ticks);
        }
        foreach (var item in SubTasks)
        {
          result = result.AddTicks(item.TotalDuration.Ticks);
        }
        return result;
      }
    }

    [XmlIgnore]
    public DateTime Duration
    {
      get
      {
        DateTime result = new DateTime();
        foreach (var item in Periods)
        {
          result = result.AddTicks(item.Duration.Ticks);
        }
        return result;
      }
    }

    [XmlIgnore]
    public DateTime ChildDuration
    {
      get
      {
        return new DateTime(TotalDuration.Ticks - Duration.Ticks);
      }
    }

    [XmlIgnore]
    public DateTime TotalDurationToday
    {
      get
      {
        DateTime result = new DateTime();
        foreach (var item in Periods)
        {
          result = result.AddTicks(item.DurationToday.Ticks);
        }
        foreach (var item in SubTasks)
        {
          result = result.AddTicks(item.TotalDurationToday.Ticks);
        }
        return result;
      }
    }

    [XmlIgnore]
    public DateTime DurationToday
    {
      get
      {
        DateTime result = new DateTime();
        foreach (var item in Periods)
        {
          result = result.AddTicks(item.DurationToday.Ticks);
        }
        return result;
      }
    }

    [XmlIgnore]
    public DateTime ChildDurationToday
    {
      get
      {
        return new DateTime(TotalDurationToday.Ticks - DurationToday.Ticks);
      }
    }


    public DateTime TotalSpan(DateTime start, DateTime end)
    {
      DateTime result = new DateTime();
      foreach (var item in Periods)
      {
        result = result.AddTicks(item.Span(start, end).Ticks);
      }
      foreach (var item in SubTasks)
      {
        result = result.AddTicks(item.TotalSpan(start, end).Ticks);
      }
      return result;
    }

    public DateTime Span(DateTime start, DateTime end)
    {
      DateTime result = new DateTime();
      foreach (var item in Periods)
      {
        result = result.AddTicks(item.Span(start, end).Ticks);
      }
      return result;
    }

    public DateTime ChildSpan(DateTime start, DateTime end)
    {
      return new DateTime(TotalSpan(start, end).Ticks - Span(start, end).Ticks);
    }

    public void UpdateDuration()
    {
      Notify("TotalDuration");
      Notify("Duration");
      Notify("ChildDuration");

      Notify("TotalDurationToday");
      Notify("DurationToday");
      Notify("ChildDurationToday");

      Notify("TaskEndedAt");

      foreach (var item in Periods)
        if (item.State == States.Opened) item.UpdateDuration();
      if (HasParent) Parent.UpdateDuration();
    }

    public override string ToString()
    {
      return string.Format("{0}\t{1}", Name, Duration.ToLongTimeString());
    }

    public bool HasChild { get { return (SubTasks.Count > 0); } }

    private DateTime GetStartTime(DateTime value)
    {
      var result = value;
      foreach (var item in Periods)
        if (result.Ticks > item.Start.Ticks) result = item.Start;
      if (HasChild)
        foreach (var child in SubTasks)
          result = child.GetStartTime(result);
      return result;
    }

    private DateTime GetEndTime(DateTime value)
    {
      var result = value;
      foreach (var item in Periods)
      {
        if (item.State == States.Opened) return DateTime.Now;
        if (result.Ticks < item.Finish.Ticks) result = item.Finish;
      }
      if (HasChild)
        foreach (var child in SubTasks)
        {
          result = child.GetEndTime(result);
        }
      return result;
    }

    [XmlIgnore]
    public DateTime TaskEndedAt
    {
      get
      {
        return GetEndTime(DateTime.MinValue);
      }
    }

    [XmlIgnore]
    public DateTime TaskStartedAt
    {
      get
      {
        return GetStartTime(DateTime.Now);
      }
    }


    public DateTime TaskEndedAtCut(DateTime cut)
    {

      var end = GetEndTime(DateTime.MinValue);
      if (end > cut) return cut;
      else return end;
    }

    public DateTime TaskStartedAtCut(DateTime cut)
    {
      var start = GetStartTime(DateTime.Now);
      if (start < cut) return cut;
      else return start;
    }


    [XmlIgnore]
    public int Level
    {
      get
      {
        if (Parent == null) return 0;
        return Parent.Level + 1;
      }
    }


    //#region selectable for edit

    //private bool _ischeckable;
    //public bool ischeckable
    //{
    //  get { return _ischeckable; }
    //  set
    //  {
    //    if (!hasparent) _ischeckable = value;
    //    notify("ischeckable");
    //  }
    //}

    //private bool _isChecked;
    //public bool IsChecked {
    //  get { return _isChecked; }
    //  set
    //  {
    //    _isChecked = value;
    //    Notify("IsChecked");
    //  }
    //}

    //#endregion Selectable for edit

    #region Static members

    public static IEnumerable<Task> GetAllTasks(ObservableCollection<Task> tasks)
    {
      foreach (var task in tasks)
      {
        yield return task;
        foreach (var subTask in Task.GetAllTasks(task.SubTasks))
        {
          yield return subTask;
        }
      }
    }

    public static Task GetParent(Task item, ObservableCollection<Task> collection)
    {
      if (collection.Contains(item)) return null;
      foreach (var i in collection)
        if (i.SubTasks.Contains(item)) return i;
      return null;
    }

    #endregion Static members


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
