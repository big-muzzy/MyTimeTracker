using System;
using System.Windows;
using System.Windows.Navigation;
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace MyTimeTracker
{
  public partial class ReportWizardDialogBox : NavigationWindow
  {
    private ReportSettings reportSettings;
    public ReportSettings ReportSettings
    {
      get { return this.reportSettings; }
    }

    public ReportWizardInputs Inputs { get; set; }

    public ReportWizardDialogBox(ObservableCollection<KeyValuePair<Guid, string>> checkList, string templatePath)
    {
      InitializeComponent();
      reportSettings = new ReportSettings();

      Inputs = new ReportWizardInputs(checkList, templatePath);

      // Launch the wizard
      ReportWizardLauncher wizardLauncher = new ReportWizardLauncher(Inputs);
      wizardLauncher.WizardReturn += new WizardReturnEventHandler(wizardLauncher_WizardReturn);
      this.Navigate(wizardLauncher);
    }

    void wizardLauncher_WizardReturn(object sender, WizardReturnEventArgs e)
    {
      // Handle wizard return
      this.reportSettings = e.Data as ReportSettings;
      if (this.DialogResult == null)
      {
        this.DialogResult = (e.Result == WizardResult.Finished);
      }
    }
  }
}