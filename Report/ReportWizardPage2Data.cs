using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace MyTimeTracker
{
  public class ReportWizardPage2Data: INotifyPropertyChanged
  {
    private bool allProjects = true;
    public bool AllProjects
    {
      get { return allProjects; }
      set
      {
        allProjects = value;
        Notify("AllProjects");
      }
    }

    private bool selectedProjects = false;
    public bool SelectedProjects
    {
      get { return selectedProjects; }
      set
      {
        selectedProjects = value;
        Notify("SelectedProjects");
      }
    }

    private ObservableCollection<TaskCheckItem> projects;
    public ObservableCollection<TaskCheckItem> Projects
    {
      get { return projects; }
      set
      {
        projects = value;
        Notify("Projects");
      }
    }

    public override string ToString()
    {
      return string.Format("AllProjects: {0}{1)SelectedProjects: {2}", allProjects.ToString(), Environment.NewLine, selectedProjects.ToString());
    }

    public ReportWizardPage2Data()
    {
      projects = new ObservableCollection<TaskCheckItem>();
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
