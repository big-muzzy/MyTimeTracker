using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.IO;
namespace MyTimeTracker
{
  public class ReportWizardPage3Data : INotifyPropertyChanged
  {
    private string templatePath;
    public string TemplatePath
    {
      get { return templatePath; }
      set
      {
        templatePath = value;
        Notify("TemplatePath");
      }
    }

    private FileInfo template;
    public FileInfo Template
    {
      get { return template; }
      set
      {
        template = value;
        Notify("Template");
      }
    }

    private ReportInfo _info;
    public ReportInfo Info
    {
      get { return _info; }
      set
      {
        _info = value;
        Notify("Info");
      }
    }

    public ReportWizardPage3Data()
    {
      _info = new ReportInfo();
    }

    #region INotifyPropertyChanged

    public void Notify(string info)
    {
      if (PropertyChanged != null)
      {
        PropertyChanged(this, new PropertyChangedEventArgs(info));
      }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    #endregion INotifyPropertyChanged

  }
}
