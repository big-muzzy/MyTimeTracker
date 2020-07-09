using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Collections.ObjectModel;
using System.Linq;

namespace MyTimeTracker
{
  /// <summary>
  /// Interaction logic for ReportWizardPage2.xaml
  /// </summary>
  public partial class ReportWizardPage2
  {
    public ReportWizardInputs Inputs { get; set; }
    public ReportWizardPage2Data PageData { get; set; }
    private Page NextPage { get; set; }


    public ReportWizardPage2(ReportWizardInputs inputs, Page nextPage)
    {

      // Bind wizard state to UI
      Inputs = inputs;
      PageData = new ReportWizardPage2Data();
      PageData.Projects = new ObservableCollection<TaskCheckItem>(Inputs.Tasks.Select(t => new TaskCheckItem(t.Key, t.Value)));

      NextPage = nextPage;

      InitializeComponent();

    }

    void backButton_Click(object sender, RoutedEventArgs e)
    {
      // Go to previous wizard page
      this.NavigationService.GoBack();
    }

    void nextButton_Click(object sender, RoutedEventArgs e)
    {
      // Go to next wizard page
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

  }
}
