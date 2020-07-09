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

namespace MyTimeTracker
{
    /// <summary>
    /// Логика взаимодействия для ReportSettingsDialog.xaml
    /// </summary>
    public partial class ReportSettingsDialog : Window
    {
        //public ObservableCollection<ListBoxItem> Items;

        public ReportSettingsDialog()
        {
            InitializeComponent();
        }

        public ReportSettings GetReportSettings(ObservableCollection<Task> projects, string templatePath)
        {
            var projectsListItems = new ObservableCollection<CheckedListItem>();
            foreach (var item in projects)
            {
                projectsListItems.Add(new CheckedListItem(item.ID, item.Name));
            }

            projectsList.ItemsSource = projectsListItems;

            SetTemplates(templatePath);

            if (this.ShowDialog() == true)
            {
                ReportSettings result = new ReportSettings();

                if (rbFullTime.IsChecked == true)
                {
                    result.StartDate = DateTime.MinValue;
                    result.EndDate = DateTime.Now;
                }
                else if (rbFixedPeriod.IsChecked == true)
                {
                    if (cbFixedPeriod.SelectedItem == cbiDay)
                    {
                        result.StartDate = DateTime.Today;
                        result.EndDate = DateTime.Now;
                    }
                    else if (cbFixedPeriod.SelectedItem == cbiWeek)
                    {
                        DateTime start = DateTime.Today;
                        while (start.DayOfWeek != DayOfWeek.Monday)
                        {
                            start = start.Subtract(new TimeSpan(1, 0, 0, 0));
                        }
                        result.StartDate = start;
                        result.EndDate = DateTime.Now;
                    }
                    else if (cbFixedPeriod.SelectedItem == cbiMonth)
                    {
                        result.StartDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                        result.EndDate = DateTime.Now;
                    }
                    else
                    {
                        result.StartDate = DateTime.Today;
                        result.EndDate = DateTime.Now;
                    }
                }
                else if (rbCustomPeriod.IsChecked == true)
                {
                    result.StartDate = (DateTime)dpStart.SelectedDate;
                    result.EndDate = (DateTime)dpEnd.SelectedDate;
                }
                else
                {
                    result.StartDate = DateTime.MinValue;
                    result.EndDate = DateTime.Now;
                }

                if (rbAllProjects.IsChecked == true)
                {
                    result.Items = projects;
                }
                else if (rbCustomProjects.IsChecked == true)
                {
                    //ObservableCollection<Task> proj = new ObservableCollection<Task>();
                    foreach (var item in projectsList.Items)
                    {
                        CheckedListItem checkedItem = (item as CheckedListItem);
                        if (checkedItem.IsChecked)
                            result.Items.Add(projects.First(t => t.ID == checkedItem.Id));
                    }
                }

                result.TemplateFileName = (cbTemplateFileName.SelectedValue as System.IO.FileInfo).FullName;

                return result;
            }

            return null;
        }

        private void SetTemplates(string templatePath)
        {
            cbTemplateFileName.Items.Clear();
            tbTemplatePath.Text = templatePath;
            var templates = ReadTemplatesFromDirectory(tbTemplatePath.Text);
            if (templates != null)
                foreach (var t in templates)
                {
                    cbTemplateFileName.Items.Add(t);
                }
            if (cbTemplateFileName.Items.Count > 0) cbTemplateFileName.SelectedIndex = 0;
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

        private void btnSelectPath_Click(object sender, RoutedEventArgs e)
        {
            var folderDialog = new System.Windows.Forms.FolderBrowserDialog();
            if (System.IO.Directory.Exists(tbTemplatePath.Text))
                folderDialog.SelectedPath = tbTemplatePath.Text;
            folderDialog.Description = "Выберите папку с шаблонами";
            folderDialog.ShowNewFolderButton = false;
            if (folderDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                SetTemplates(folderDialog.SelectedPath);
            }
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            bool templateError = false;
            bool projectError = false;
            bool periodError = false;
            if (rbCustomPeriod.IsChecked == true)
            {
                if (dpStart.SelectedDate >= dpEnd.SelectedDate)
                {
                    periodError = true;
                    MessageBox.Show("Неверно выбран приод для формирования отчета", "Неверно выбран приод", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            if (cbTemplateFileName.SelectedItem == null)
            {
                templateError = true;
                MessageBox.Show("Не выбран шаблон для формирования отчета", "Не выбран шаблон", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            if (rbCustomProjects.IsChecked == true)
            {
                if (projectsList.Items.Count == 0)
                {
                    projectError = true;
                }
                else
                {
                    int i = 0;
                    bool found = false;
                    while (!found && i < projectsList.Items.Count)
                    {
                        found = found || (projectsList.Items[i] as CheckedListItem).IsChecked;
                        i++;
                    }
                    projectError = projectError || !found;
                }
                if (projectError)
                    MessageBox.Show("Не выбраны проекты для формирования отчета", "Не выбраны проекты", MessageBoxButton.OK, MessageBoxImage.Information);
            }

            if (!templateError && !projectError && !periodError) DialogResult = true;
        }
    }
}
