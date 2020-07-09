using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.IO;

namespace MyTimeTracker
{
  public class ReportInfo : INotifyPropertyChanged
  {
    private string name;
    public string Name
    {
      get { return name; }
      set
      {
        name = value;
        Notify("Name");
      }
    }

    private string description;
    public string Description
    {
      get { return description; }
      set
      {
        description = value;
        Notify("Description");
      }
    }

    private string extention;
    public string Extention
    {
      get { return extention; }
      set
      {
        extention = value;
        Notify("Extention");
      }
    }

    private string defaultFileName;
    public string DefaultFileName
    {
      get { return defaultFileName; }
      set
      {
        defaultFileName = value;
        Notify("DefaultFileName");
      }
    }

    public void ReadFromFile(FileInfo file)
    {
      //read template;
      var template = File.ReadAllText(file.FullName);

      //get Project Template (project)
      Name = ReportGenerator.ExtractBlock(@"<#ReportName>", @"</#ReportName>", ref template);
      Description = ReportGenerator.ExtractBlock(@"<#ReportDescription>", @"</#ReportDescription>", ref template);
      Extention = ReportGenerator.ExtractBlock(@"<#ReportExtention>", @"</#ReportExtention>", ref template);
      DefaultFileName = ReportGenerator.ExtractBlock(@"<#ReportDefaultFileName>", @"</#ReportDefaultFileName>", ref template);
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
