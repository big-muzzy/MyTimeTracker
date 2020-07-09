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
using System.IO;

namespace MyTimeTracker
{
  /// <summary>
  /// Interaction logic for ImportDialog.xaml
  /// </summary>
  public partial class ImportDialog : Window
  {

    public static readonly DependencyProperty ItemProperty = DependencyProperty.Register("Result", typeof(ImportSettings), typeof(ImportDialog));

    public ImportSettings Result
    {
      get { return (ImportSettings)GetValue(ItemProperty); }
      set { SetValue(ItemProperty, value); }
    }

    public ImportDialog()
    {
      InitializeComponent();
      Result = new ImportSettings();
    }

    private void fileSelect_Click(object sender, RoutedEventArgs e)
    {
      var fileDialog = new System.Windows.Forms.OpenFileDialog();
      if (System.IO.Directory.Exists(Result.Path))
        fileDialog.InitialDirectory = Result.Path;
      fileDialog.Multiselect = false;
      fileDialog.Title = "Выберите файл для импорта";
      if (fileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
      {
        Result.FileName = fileDialog.FileName;
      }
    }

    public ImportSettings GetImportSettings(AppSettings settings)
    {
      Result.Path = settings.DefaultExportPath;
      Result.MergeMode = settings.DefaultMergeMode;

      if (this.ShowDialog() == true)
      {
        return Result;
      }
      
      return null;
    }

    private void Ok_Click(object sender, RoutedEventArgs e)
    {
      if (Result.FileName == string.Empty) return;
      if (!File.Exists(Result.FileName)) return;
      if (MergeModes.GetMergeModes().Count(p => p.Key == Result.MergeMode) == 0) return;

      this.DialogResult = true;

    }
  }
}
