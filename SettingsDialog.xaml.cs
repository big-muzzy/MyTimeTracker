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

namespace MyTimeTracker
{
    /// <summary>
    /// Логика взаимодействия для SettingsDialog.xaml
    /// </summary>
    public partial class SettingsDialog : Window
    {
        public static readonly DependencyProperty ItemProperty = DependencyProperty.Register(
            "Item", typeof(AppSettings), typeof(SettingsDialog), 
            new PropertyMetadata(new PropertyChangedCallback(ItemChanged)));

        public static void ItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //todo: something
        }

        public AppSettings Item
        {
            get { return (AppSettings)GetValue(ItemProperty); }
            set { SetValue(ItemProperty, value); }
        }

        public SettingsDialog()
        {
            InitializeComponent();
        }

        public AppSettings GetAppSettings(AppSettings settings)
        {
            Item = settings;
            if (this.ShowDialog() == true)
            {
                return Item;
            }
            return null;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void TemplatePathSelect_Click(object sender, RoutedEventArgs e)
        {
            var folderDialog = new System.Windows.Forms.FolderBrowserDialog();
            if (System.IO.Directory.Exists(tbReportTemplatePath.Text))
                folderDialog.SelectedPath = tbReportTemplatePath.Text;
            folderDialog.Description = "Выберите папку с шаблонами";
            folderDialog.ShowNewFolderButton = false;
            if (folderDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Item.ReportTemplatePath = folderDialog.SelectedPath;
            }
        }

        private void ReportPathSelect_Click(object sender, RoutedEventArgs e)
        {
            var folderDialog = new System.Windows.Forms.FolderBrowserDialog();
            if (System.IO.Directory.Exists(tbReportPath.Text))
                folderDialog.SelectedPath = tbReportPath.Text;
            folderDialog.Description = "Выберите папку для сохранения отчетов";
            folderDialog.ShowNewFolderButton = true;
            if (folderDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Item.ReportOutputPath = folderDialog.SelectedPath;
            }
        }

        private void ExportPathSelect_Click(object sender, RoutedEventArgs e)
        {
          var folderDialog = new System.Windows.Forms.FolderBrowserDialog();
          if (System.IO.Directory.Exists(tbExportPath.Text))
            folderDialog.SelectedPath = tbExportPath.Text;
          folderDialog.Description = "Выберите папку для экспорта проектов";
          folderDialog.ShowNewFolderButton = true;
          if (folderDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
          {
            Item.DefaultExportPath = folderDialog.SelectedPath;
          }
        }
    }
}
