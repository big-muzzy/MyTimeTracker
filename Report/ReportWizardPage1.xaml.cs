using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace MyTimeTracker
{
  public partial class ReportWizardPage1
  {
    public ReportWizardInputs Inputs { get; set; }
    public ReportWizardPage1Data PageData { get; set; }
    private Page NextPage { get; set; }


    public ReportWizardPage1(ReportWizardInputs inputs, Page nextPage)
    {

      // Bind wizard state to UI
      Inputs = inputs;
      if (PageData == null) PageData = new ReportWizardPage1Data();
      NextPage = nextPage;

      InitializeComponent();
      PageData.Periods = 0;
    }


    void nextButton_Click(object sender, RoutedEventArgs e)
    {
      if (this.NavigationService.CanGoForward)
      {
        this.NavigationService.GoForward();
      }
      else
      {
        this.NavigationService.Navigate(NextPage);
      }
    }

    void cancelButton_Click(object sender, RoutedEventArgs e)
    {
      // Cancel the wizard and don't return any data
      OnReturn(new ReturnEventArgs<WizardResult>(WizardResult.Canceled));
    }

    public void wizardPage_Return(object sender, ReturnEventArgs<WizardResult> e)
    {
      // If returning, wizard was completed (finished or canceled),
      // so continue returning to calling page
      OnReturn(e);
    }

    private void dpStart_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
    {
      if (dpEnd != null)
        if (dpStart.SelectedDate > dpEnd.SelectedDate) dpStart.SelectedDate = dpEnd.SelectedDate;
    }

    private void dpEnd_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
    {
      if (dpStart != null)
        if (dpEnd.SelectedDate < dpStart.SelectedDate) dpEnd.SelectedDate = dpStart.SelectedDate;
    }
  }
}