using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace MyTimeTracker
{
  public class ReportWizardInputs
  {
    public ObservableCollection<KeyValuePair<Guid, String>> Tasks { get; set; }
    public string DefaultTemplatePath { get; set; }

    public ReportWizardInputs()
    {
      Tasks = new ObservableCollection<KeyValuePair<Guid, string>>();
      DefaultTemplatePath = string.Empty;
    }

    public ReportWizardInputs(ObservableCollection<KeyValuePair<Guid, String>> tasks, string templatePath)
    {
      Tasks = tasks;
      DefaultTemplatePath = templatePath;
    }

  }
}
