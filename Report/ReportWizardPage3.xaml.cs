using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Collections.Generic;
using System.IO;

namespace MyTimeTracker
{
  /// <summary>
  /// Interaction logic for ReportWizardPage3.xaml
  /// </summary>
  public partial class ReportWizardPage3
  {
    public ReportWizardInputs Inputs { get; set; }
    public ReportWizardPage3Data PageData { get; set; }
    private Page NextPage { get; set; }

    public ReportWizardPage3(ReportWizardInputs inputs, Page nextPage)
    {
      Inputs = inputs;
      PageData = new ReportWizardPage3Data();
      PageData.TemplatePath = Inputs.DefaultTemplatePath;
      NextPage = nextPage;

      InitializeComponent();
      SetTemplates();
    }

    void backButton_Click(object sender, RoutedEventArgs e)
    {
      // Go to previous wizard page
      this.NavigationService.GoBack();
    }

    void cancelButton_Click(object sender, RoutedEventArgs e)
    {
      // Cancel the wizard and don't return any data
      OnReturn(new ReturnEventArgs<WizardResult>(WizardResult.Canceled));
    }

    void finishButton_Click(object sender, RoutedEventArgs e)
    {
      // Finish the wizard and return bound data to calling page
      OnReturn(new ReturnEventArgs<WizardResult>(WizardResult.Finished));
    }

    private List<System.IO.FileInfo> ReadTemplatesFromDirectory(string p)
    {
      if (System.IO.Directory.Exists(p))
      {
        var result = new List<System.IO.FileInfo>();
        System.IO.DirectoryInfo info = new System.IO.DirectoryInfo(p);
        System.IO.FileInfo[] files = info.GetFiles();

        foreach (var file in files)
        {
          if (file.Extension == ".trs") result.Add(file);
        }

        return result;
      }
      return null;
    }

    private void SetTemplates()
    {
      cbTemplateFileName.Items.Clear();
      PageData.Template = null;
      var templates = ReadTemplatesFromDirectory(PageData.TemplatePath);
      if (templates != null)
        foreach (var t in templates)
        {
          cbTemplateFileName.Items.Add(t);
          if (PageData.Template == null) PageData.Template = t;        
        }
    }

    private void btnSelectPath_Click(object sender, RoutedEventArgs e)
    {
      var folderDialog = new System.Windows.Forms.FolderBrowserDialog();
      if (System.IO.Directory.Exists(tbTemplatePath.Text))
        folderDialog.SelectedPath = tbTemplatePath.Text;
      folderDialog.Description = "Выберите папку с шаблонами";
      folderDialog.ShowNewFolderButton = false;
      if (folderDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
      {
        PageData.TemplatePath = folderDialog.SelectedPath;
        SetTemplates();
      }
    }

    private void cbTemplateFileName_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      if (cbTemplateFileName.SelectedItem != null)
      {
        PageData.Info.ReadFromFile(PageData.Template);
      }
    }

    

  }
}
