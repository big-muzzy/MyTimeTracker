using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;

namespace MyTimeTracker
{
  /// <summary>
  /// Interaction logic for ExportDialog.xaml
  /// </summary>
  public partial class ExportDialog : Window
  {

    public static readonly DependencyProperty ItemProperty = DependencyProperty.Register("Result", typeof(ExportSettings), typeof(ExportDialog));

    public ExportSettings Result
    {
      get { return (ExportSettings)GetValue(ItemProperty); }
      set { SetValue(ItemProperty, value); }
    }

    public ExportDialog()
    {
      InitializeComponent();
      Result = new ExportSettings();
    }

    public ExportSettings GetExportSettings(ObservableCollection<Task> projects, AppSettings settings)
    {
      var projectsListItems = new ObservableCollection<CheckedListItem>();
      foreach (var item in projects)
      {
        Result.Projects.Add(new CheckedListItem(item.ID, item.Name));
      }

      Result.ExportPath = settings.DefaultExportPath;
      Result.RemoveAfterExport = settings.RemoveAfterExportDefaultValue;

      if (this.ShowDialog() == true)
      {
        Result.FileName = GetExportFileName(Result.Projects);

        return Result;
      }

      return null;
    }

    private string GetExportFileName(IEnumerable<CheckedListItem> list)
    {
      int cnt = list.Count(p => p.IsChecked == true);

      if (cnt == 1)
      {
        return
          string.Format("{0}-{1}-{2}.xml",
            Regex.Replace(list.Single(p => p.IsChecked == true).Name, @"[~`!@#$%^&*()=+\[\]{};:/.,<>""/'\\/|]", "_"), 
            Regex.Replace(DateTime.Today.ToShortDateString(), @"\.", "_"),
            Regex.Replace(DateTime.Now.ToShortTimeString(), ":", "_"));
      }
      return 
        string.Format("{0}-Proj-{1}-{2}.xml", cnt,
            Regex.Replace(DateTime.Today.ToShortDateString(), @"\.", "_"),
            Regex.Replace(DateTime.Now.ToShortTimeString(), ":", "_"));
    }

    private void Ok_Click(object sender, RoutedEventArgs e)
    {
      bool found = false;

      foreach (CheckedListItem item in projectsList.Items)
      {
        if (item.IsChecked)
        {
          found = true;
          break;
        }
      }

      if (tbExportPath.Text == string.Empty)
      {
        MessageBox.Show("Не выбран каталог для сохранения", "Не выбран каталог", MessageBoxButton.OK, MessageBoxImage.Error);
        return;
      }

      if (!found)
      {
        MessageBox.Show("Не выбраны проекты для экспорта", "Не выбраны проекты", MessageBoxButton.OK, MessageBoxImage.Error);
        return;
      }

      DialogResult = true;
    }

    private void btnSelectPath_Click(object sender, RoutedEventArgs e)
    {
      var folderDialog = new System.Windows.Forms.FolderBrowserDialog();
      if (System.IO.Directory.Exists(Result.ExportPath))
        folderDialog.SelectedPath = Result.ExportPath;
      folderDialog.Description = "Выберите папку для экспорта";
      folderDialog.ShowNewFolderButton = false;
      if (folderDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
      {
        Result.ExportPath = folderDialog.SelectedPath;
      }
    }


  }
}
