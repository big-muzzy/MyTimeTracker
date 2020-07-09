using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.ComponentModel;

namespace MyTimeTracker
{
  [Serializable]
  public class AppSettings : INotifyPropertyChanged
  {
    //Шаблон нового проекта. 
    [XmlIgnore]
    private bool minimizeOnTimeStart;
    [XmlElement("MinimizeOnTimeStart")]
    public bool MinimizeOnTimeStart //сворачивать при запуске времени 
    {
      get { return minimizeOnTimeStart; }
      set
      {
        minimizeOnTimeStart = value;
        Notify("MinimizeOnTimeStart");
      }
    }

    //прятать в трей при запуске врмени 

    [XmlIgnore]
    private bool minimizeToTray;
    [XmlElement("MinimizeToTray")]
    public bool MinimizeToTray //прятать в трей при сворачивании :)
    {
      get { return minimizeToTray; }
      set
      {
        minimizeToTray = value;
        Notify("MinimizeToTray");
      }
    }

    [XmlIgnore]
    private bool showBaloonOnTimeStart;
    [XmlElement("ShowBaloonOnTimeStart")]
    public bool ShowBaloonOnTimeStart //показывать облачко при старте и стопе отсчета времени
    {
      get { return showBaloonOnTimeStart; }
      set
      {
        showBaloonOnTimeStart = value;
        Notify("ShowBaloonOnTimeStart");
      }
    }

    [XmlIgnore]
    private string reportTemplatePath;
    [XmlElement("ReportTemplatePath")]
    public string ReportTemplatePath //пути по умолчанию к шаблонам отчетов
    {
      get { return reportTemplatePath; }
      set
      {
        reportTemplatePath = value;
        Notify("ReportTemplatePath");
      }
    }

    [XmlIgnore]
    private string reportOutputPath;
    [XmlElement("ReportOutputPath")]
    public string ReportOutputPath //путь по умолчанию для выдачи отчета
    {
      get { return reportOutputPath; }
      set
      {
        reportOutputPath = value;
        Notify("ReportOutputPath");
      }
    }

    [XmlIgnore]
    private string defaultExportPath;
    [XmlElement("DefaultExportPath")]
    public string DefaultExportPath
    { get { return defaultExportPath; }
      set
      {
        defaultExportPath = value;
        Notify("DefaultExportPath");
      }
    }

    [XmlIgnore]
    private bool removeAfterExportDefaultValue;
    [XmlElement("RemoveAfterExportDefaultValue")]
    public bool RemoveAfterExportDefaultValue
    { get { return removeAfterExportDefaultValue; }
      set
      {
        removeAfterExportDefaultValue = value;
        Notify("RemoveAfterExportDefaultValue");
      }
    }

    [XmlIgnore]
    private MergeEnum defaultMergeMode;
    [XmlElement("DefaultMergeMode")]
    public MergeEnum DefaultMergeMode
    {
      get { return defaultMergeMode; }
      set
      {
        defaultMergeMode = value;
        Notify("DefaultMergeMode");
      }
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
