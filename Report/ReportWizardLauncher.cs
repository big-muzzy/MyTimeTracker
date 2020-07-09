using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace MyTimeTracker
{
  public class ReportWizardLauncher : PageFunction<WizardResult>
  {
    public event WizardReturnEventHandler WizardReturn;
    public ReportWizardInputs Inputs { get; set; }

    private ReportWizardPage1 wizardPage1;
    private ReportWizardPage2 wizardPage2;
    private ReportWizardPage3 wizardPage3;

    public ReportWizardLauncher(ReportWizardInputs inputs)
    {
      //reportSettings = new ReportSettings();
      Inputs = inputs;
    }

    protected override void Start()
    {
      base.Start();

      // So we remember the WizardCompleted event registration
      this.KeepAlive = true;

      // Launch the wizard

      wizardPage3 = new ReportWizardPage3(Inputs, null);
      wizardPage3.Return += new ReturnEventHandler<WizardResult>(wizardPage_Return);

      wizardPage2 = new ReportWizardPage2(Inputs, wizardPage3);
      wizardPage2.Return += new ReturnEventHandler<WizardResult>(wizardPage_Return);

      wizardPage1 = new ReportWizardPage1(Inputs, wizardPage2);
      wizardPage1.Return += new ReturnEventHandler<WizardResult>(wizardPage_Return);

      this.NavigationService.Navigate(wizardPage1);
    }

    public void wizardPage_Return(object sender, ReturnEventArgs<WizardResult> e)
    {
      // Notify client that wizard has completed
      // NOTE: We need this custom event because the Return event cannot be
      // registered by window code - if WizardDialogBox registers an event handler with
      // the WizardLauncher's Return event, the event is not raised.
      if (this.WizardReturn != null)
      {
        this.WizardReturn(this, new WizardReturnEventArgs(e.Result, GetReportSettings()));
      }
      OnReturn(null);
    }

    #region Wizard Output

    public DateTime StartDate
    {
      get
      {
        if (wizardPage1.PageData.AllTime) return DateTime.MinValue;
        if (wizardPage1.PageData.FreePeriod) return wizardPage1.PageData.StartDate;
        if (wizardPage1.PageData.SelectedPeriod)
        {
          if (wizardPage1.PageData.Periods == wizardPage1.PageData.tpvDay)
          {
            return DateTime.Today;
          }
          if (wizardPage1.PageData.Periods == wizardPage1.PageData.tpvWeek)
          {
            DateTime start = DateTime.Today;
            while (start.DayOfWeek != DayOfWeek.Monday)
            {
              start = start.Subtract(new TimeSpan(1, 0, 0, 0));
            }
            return start;
          }
          if (wizardPage1.PageData.Periods == wizardPage1.PageData.tpvMonth)
          {
            return new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
          }
        }
        throw new ArgumentOutOfRangeException("ReportWizardLauncher.StartDate",
          string.Format("AllTime: {1}{0}FreePeriod: {2}{0}SelectedPeriod: {3}{0}Periods: {4}", 
            Environment.NewLine,
            wizardPage1.PageData.AllTime,
            wizardPage1.PageData.FreePeriod,
            wizardPage1.PageData.SelectedPeriod,
            wizardPage1.PageData.Periods));
      }
    }

    public DateTime EndDate
    {
      get
      {
        if (wizardPage1.PageData.FreePeriod)
        {
          return wizardPage1.PageData.EndDate;
        }
        return DateTime.Now;
      }
    }


    public IEnumerable<Guid> Tasks
    {
      get
      {
        if (wizardPage2.PageData.SelectedProjects)
        {
          return wizardPage2.PageData.Projects.Where(p => p.IsChecked).Select<TaskCheckItem, Guid>(t => t.ID);
        }
        if (wizardPage2.PageData.AllProjects)
        {
          return wizardPage2.PageData.Projects.Select<TaskCheckItem, Guid>(t => t.ID);
        }
        return null;
      }
    }

    public FileInfo TemplateFileName
    {
      get
      {
        return wizardPage3.PageData.Template;
      }
    }

    public ReportInfo Info
    {
      get { return wizardPage3.PageData.Info; }
    }

    public ReportSettings GetReportSettings()
    {
      return new ReportSettings(StartDate, EndDate, Tasks, TemplateFileName, Info);
    }

    #endregion Wizard Output



  }
}
