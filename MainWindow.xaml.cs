using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Xml.Serialization;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows.Threading;
using System.ComponentModel;
using System.Windows.Interop;
using System.Runtime.InteropServices;

namespace MyTimeTracker
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window, INotifyPropertyChanged, IDisposable
  {

#if DEBUG
    private const string SaveFilePath = @"MyTime_test.xml";
#else
        private const string SaveFilePath = @"MyTime.xml";
#endif


#if DEBUG
    private const string SettingsFile = @"mttSettings_debug.xml";
#else
        private const string SettingsFile = @"mttSettings.xml";
#endif

    public const bool MinimizeToTray = true;

    public static readonly DependencyProperty ProjectsListProperty = DependencyProperty.Register(
        "ProjectsList", typeof(ObservableCollection<Task>), typeof(MainWindow),
        new PropertyMetadata(new PropertyChangedCallback(ProjectsListChanged)));

    public static void ProjectsListChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      var control = (d as MainWindow);
      if (control != null)
        control.OnItemsSourceChanged((ObservableCollection<Task>)e.OldValue, (ObservableCollection<Task>)e.NewValue);
    }

    private Task ActiveTask { get; set; }

    private Task SelectedTask { get; set; }

    private DispatcherTimer MainTimer;


    public DateTime TotalDudationToday
    {
      get
      {
        DateTime result = new DateTime();
        foreach (var item in ProjectsList)
        {
          result = result.AddTicks(item.TotalDurationToday.Ticks);
        }
        return result;

      }
    }

    public AppSettings MttSettings { get; set; }

    private void OnItemsSourceChanged(ObservableCollection<Task> oldValue, ObservableCollection<Task> newValue)
    {
      if (oldValue != null)
      {
        oldValue.CollectionChanged -= UpdateProjectGridsSource;
        oldValue.CollectionChanged -= SetParent;
        newValue.CollectionChanged -= SetSelectedTask;
        foreach (var item in oldValue)
        {
          item.SubTasks.CollectionChanged -= UpdateProjectGridsSource;
          item.SubTasks.CollectionChanged -= SetParent;
          item.SubTasks.CollectionChanged -= SetSelectedTask;
        }
      }

      if (newValue != null)
      {
        newValue.CollectionChanged += UpdateProjectGridsSource;
        newValue.CollectionChanged += SetParent;
        newValue.CollectionChanged += SetSelectedTask;
        foreach (var item in newValue)
        {
          item.SubTasks.CollectionChanged += UpdateProjectGridsSource;
          item.SubTasks.CollectionChanged += SetParent;
          item.SubTasks.CollectionChanged += SetSelectedTask;
        }
        UpdateTasksParents(newValue);
      }
    }

    private void SetSelectedTask(object sender, NotifyCollectionChangedEventArgs e)
    {
      if (SelectedTask != null)
        ProjectGrid.SelectedItem = SelectedTask;
    }

    private void UpdateTasksParents(ObservableCollection<Task> collection)
    {
      foreach (var item in collection)
      {
        item.Parent = Task.GetParent(item, ProjectsList);
        if (item.HasChild) UpdateTasksParents(item.SubTasks);
      }
    }

    public ObservableCollection<Task> ProjectsList
    {
      get { return (ObservableCollection<Task>)GetValue(ProjectsListProperty); }
      set { SetValue(ProjectsListProperty, value); }
    }

    static System.Threading.EventWaitHandle _waitHandle = new System.Threading.AutoResetEvent(false);

    #region Notify Icon

    private System.Windows.Forms.NotifyIcon ni;

    private System.Drawing.Icon CurrIcon { get; set; }

    #endregion Notify Icon

    public MainWindow()
    {

      ProjectsList = new ObservableCollection<Task>();

      MainTimer = new DispatcherTimer();
      MainTimer.Interval = new TimeSpan(0, 0, 1);
      MainTimer.Tick += MainTimer_Tick;

      #region Create Tasks;
      //var task1 = new Task("Проект 1");
      //var task2 = new Task("Проект 2");
      //var task3 = new Task("Проект 3");
      //var task4 = new Task("Проект 4");
      //var task11 = new Task("Задача 11");
      //var task12 = new Task("Задача 12");
      //var task13 = new Task("Задача 13");
      //var task14 = new Task("Задача 14");
      //var task21 = new Task("Задача 21");
      //var task22 = new Task("Задача 22");
      //var task23 = new Task("Задача 23");
      //var task31 = new Task("Задача 31");
      //var task32 = new Task("Задача 32");
      //var task41 = new Task("Задача 41");
      //task1.SubTasks.Add(task11);
      //task1.SubTasks.Add(task12);
      //task1.SubTasks.Add(task13);
      //task1.SubTasks.Add(task14);
      //task2.SubTasks.Add(task21);
      //task2.SubTasks.Add(task22);
      //task2.SubTasks.Add(task23);
      //task3.SubTasks.Add(task31);
      //task3.SubTasks.Add(task32);
      //task4.SubTasks.Add(task41);
      //ProjectsList.Add(task1);
      //ProjectsList.Add(task2);
      //ProjectsList.Add(task3);
      //ProjectsList.Add(task4);

      #endregion Create Tasks;

      InitializeComponent();

      #region Tray icon

#if DEBUG
      CurrIcon = Properties.Resources.clock_Work;
#else
      CurrIcon = Properties.Resources.clock;
#endif

      Icon = Imaging.CreateBitmapSourceFromHIcon(CurrIcon.Handle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
      ni = new System.Windows.Forms.NotifyIcon();
      ni.Icon = CurrIcon;
      ni.Visible = true;
      ni.DoubleClick += delegate(object sender, EventArgs args)
      {
        this.Show();
        this.WindowState = WindowState.Normal;
      };

      #endregion Tray icon

      mFileOpen_Click(this, null);
      LoadAppSettings();
    }

    private void LoadAppSettings()
    {
      //todo: Load project's data from xml file;
      string path = string.Format("{0}\\MyTimeTraker", Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
      if (!Directory.Exists(path)) Directory.CreateDirectory(path);
      string filename = string.Format("{0}\\MyTimeTraker\\{1}",
          Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), SettingsFile);

      try
      {
        XmlSerializer ser = new XmlSerializer(typeof(AppSettings));
        FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read);
        MttSettings = (ser.Deserialize(fs) as AppSettings);
        fs.Close();
      }
      catch
      {
        MttSettings = new AppSettings();
        MttSettings.MinimizeOnTimeStart = false;
        MttSettings.MinimizeToTray = false;
        MttSettings.ReportOutputPath = "";
        MttSettings.ReportTemplatePath = "";
        MttSettings.ShowBaloonOnTimeStart = false;
        SaveAppSettings();
      }
    }

    private void SaveAppSettings()
    {
      string path = string.Format("{0}\\MyTimeTraker", Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
      if (!Directory.Exists(path)) Directory.CreateDirectory(path);
      string filename = string.Format("{0}\\{1}", path, SettingsFile);

      FileStream fs = new FileStream(filename, FileMode.Create, FileAccess.Write);
      XmlSerializer ser = new XmlSerializer(MttSettings.GetType());
      ser.Serialize(fs, MttSettings);
      fs.Close();
    }

    protected override void OnStateChanged(EventArgs e)
    {
      if (MttSettings.MinimizeToTray)
        if (WindowState == WindowState.Minimized)
          this.Hide();

      base.OnStateChanged(e);
    }

    void MainTimer_Tick(object sender, EventArgs e)
    {
      if (ActiveTask != null) ActiveTask.UpdateDuration();
      Notify("TotalDudationToday");
    }

    void SetParent(object sender, NotifyCollectionChangedEventArgs e)
    {
      if (e.Action == NotifyCollectionChangedAction.Add)
      {
        foreach (Task item in e.NewItems)
        {
          item.Parent = Task.GetParent(item, ProjectsList);
        }
      }
    }

    void UpdateProjectGridsSource(object sender, NotifyCollectionChangedEventArgs e)
    {
      ProjectGrid.ItemsSource = Task.GetAllTasks(ProjectsList);
    }

    private void mFileExit_Click(object sender, RoutedEventArgs e)
    {
      Close();
    }

    private void mFileSave_Click(object sender, RoutedEventArgs e)
    {
      string path = string.Format("{0}\\MyTimeTraker", Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
      if (!Directory.Exists(path)) Directory.CreateDirectory(path);
      string filename = string.Format("{0}\\{1}", path, SaveFilePath);

      FileStream fs = new FileStream(filename, FileMode.Create, FileAccess.Write);
      XmlSerializer ser = new XmlSerializer(ProjectsList.GetType());
      ser.Serialize(fs, ProjectsList);
      fs.Close();
    }

    private void mFileOpen_Click(object sender, RoutedEventArgs e)
    {
      string path = string.Format("{0}\\MyTimeTraker", Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
      if (!Directory.Exists(path)) Directory.CreateDirectory(path);
      string filename = string.Format("{0}\\MyTimeTraker\\{1}",
          Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), SaveFilePath);

      try
      {
        XmlSerializer ser = new XmlSerializer(ProjectsList.GetType());
        FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read);
        ProjectsList = (ser.Deserialize(fs) as ObservableCollection<Task>);
        fs.Close();

        if (ProjectsList != null) ProjectGrid.ItemsSource = Task.GetAllTasks(ProjectsList);
      }
      catch
      {
        MessageBox.Show(string.Format("Не удалось прочитать файл сохранения по адресу: \n\r{0}", filename),
            "Файл не найден", MessageBoxButton.OK, MessageBoxImage.Error);
      }
    }

    private void ProjectGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
      var grid = (sender as DataGrid);
      if (grid != null)
      {
        var task = (grid.SelectedItem as Task);
        if (task != null) if (task.HasChild) task.IsExpanded = !task.IsExpanded;
      }
    }

    private void Window_Closed(object sender, EventArgs e)
    {
      if (ActiveTask != null) ActiveTask.CloseActivePeriod();
      ni.Visible = false;
      ni.Dispose();
      mFileSave_Click(this, null);
    }

    #region Button click events

    private void mProjectAdd_Click(object sender, RoutedEventArgs e)
    {
      var task = new Task("Новый проект");
      task.SubTasks.CollectionChanged += UpdateProjectGridsSource;
      task.SubTasks.CollectionChanged += SetParent;
      task.SubTasks.CollectionChanged += SetSelectedTask;
      SelectedTask = task;
      ProjectsList.Add(task);

      ProjectGrid.ScrollIntoView(task);
      CurrentTaskDetails.TaskName.Focus();
      CurrentTaskDetails.TaskName.SelectAll();
    }

    private void mTaskAdd_Click(object sender, RoutedEventArgs e)
    {
      var task = (ProjectGrid.SelectedItem as Task);
      if (task != null)
      {
        ObservableCollection<Task> collection = null;
        if (task.Parent == null) collection = task.SubTasks;
        else collection = task.Parent.SubTasks;
        Task newtask = new Task("Новая задача");
        SelectedTask = newtask;
        collection.Add(newtask);

        ProjectGrid.ScrollIntoView(newtask);
        CurrentTaskDetails.TaskName.Focus();
        CurrentTaskDetails.TaskName.SelectAll();
      }
      else MessageBox.Show("Для добавления задачи выберите проект", "Добавление задачи", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    private void mTaskRemove_Click(object sender, RoutedEventArgs e)
    {
      var task = (ProjectGrid.SelectedItem as Task);
      if (task != null)
      {
        ObservableCollection<Task> collection = null;
        if (task.Parent == null) collection = ProjectsList;
        else collection = task.Parent.SubTasks;

        collection.Remove(task);
      }
      else MessageBox.Show("Выберите объект для удаления", "Удаление задачи", MessageBoxButton.OK, MessageBoxImage.Information);

    }

    private void tbStartStop_Click(object sender, RoutedEventArgs e)
    {
      var task = (ProjectGrid.SelectedItem as Task);
      if (ActiveTask != null)
      {
        if (ActiveTask.HasOpenedPeriod)
        {
          ActiveTask.CloseActivePeriod();
          StopTimer();
          mFileSave_Click(sender, e);
        }
        else
        {
          if (task != null)
          {
            ActiveTask = task;
            task.OpenNewPeriod();
            StartTimer();
          }
        }
      }
      else
      {
        if (task != null)
        {
          ActiveTask = task;
          task.OpenNewPeriod();
          StartTimer();
        }
      }
    }

    private void StartTimer()
    {
      MainTimer.Start();
      tbStartStop.Content = "Стоп";
      if (MttSettings.MinimizeOnTimeStart)
        this.WindowState = System.Windows.WindowState.Minimized;
      if (MttSettings.ShowBaloonOnTimeStart)
        ni.ShowBalloonTip(3, "MyTimeTracker",
            string.Format("Отсчет времени запущен. \r\nПроект: {0}", ActiveTask.Name),
            System.Windows.Forms.ToolTipIcon.Info);
      slStatus.Content = "Ведется отсчёт";
    }

    private void StopTimer()
    {
      MainTimer.Stop();
      tbStartStop.Content = "Старт";
      if (MttSettings.ShowBaloonOnTimeStart)
        ni.ShowBalloonTip(3, "MyTimeTracker",
          string.Format("Отсчет времени остановлен для задачи: {0}", ActiveTask.Name),
          System.Windows.Forms.ToolTipIcon.Info);
      slStatus.Content = "Отсчёт не ведется";
    }

    private void mHelpAbout_Click(object sender, RoutedEventArgs e)
    {
      AboutWindow about = new AboutWindow();
      SetDialogWnd(about);
      about.ShowDialog();
      ClearDialogWnd();
    }

    private void mSettingsSet_Click(object sender, RoutedEventArgs e)
    {
      var settingsWnd = new SettingsDialog();
      SetDialogWnd(settingsWnd);
      AppSettings settings = settingsWnd.GetAppSettings(MttSettings);
      if (settings != null)
      {
        MttSettings = settings;
        SaveAppSettings();
      }
      ClearDialogWnd();
    }



    private void mMakeReport_Click(object sender, RoutedEventArgs e)
    {
      string reportTemplatePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
      if (Directory.Exists(MttSettings.ReportTemplatePath))
        reportTemplatePath = MttSettings.ReportTemplatePath;
      string reportOutputPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
      if (Directory.Exists(MttSettings.ReportOutputPath))
        reportOutputPath = MttSettings.ReportOutputPath;


      ReportWizardDialogBox reportWindow = new ReportWizardDialogBox(
        new ObservableCollection<KeyValuePair<Guid, string>>(
          ProjectsList.Select<Task, KeyValuePair<Guid, string>>(t => new KeyValuePair<Guid, string>(t.ID, t.Name))),
          reportTemplatePath
        );


      SetDialogWnd(reportWindow);
      bool dialogResult = (bool)reportWindow.ShowDialog();

      if (!dialogResult)
      {
        ClearDialogWnd();
        return;
      }

      if (reportWindow.ReportSettings != null)
      {
        System.Windows.Forms.SaveFileDialog saveDialog = new System.Windows.Forms.SaveFileDialog();
        if (reportWindow.ReportSettings.Info != null)
        {
          if (reportWindow.ReportSettings.Info.Extention != string.Empty)
          {
            try
            {
              saveDialog.Filter = reportWindow.ReportSettings.Info.Extention;
            }
            catch
            {
              saveDialog.DefaultExt = reportWindow.ReportSettings.Info.Extention;
            }
          }
          else
          {
            saveDialog.Filter = "Все файлы|*.*";
          }

          if (reportWindow.ReportSettings.Info.DefaultFileName != string.Empty)
          {
            saveDialog.FileName = reportWindow.ReportSettings.Info.DefaultFileName;
          }
        }
        
        saveDialog.RestoreDirectory = true;
        saveDialog.InitialDirectory = reportOutputPath;
        if (saveDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
        {
          try
          {
            var report = ReportGenerator.GenerateReport(reportWindow.ReportSettings, 
              new ObservableCollection<Task>(ProjectsList.Where(t => reportWindow.ReportSettings.Tasks.Contains(t.ID))));
            System.IO.File.WriteAllText(saveDialog.FileName, report);
          }
          catch
          {
            MessageBox.Show("Ошибка при создании отчета", "Создание отчета", MessageBoxButton.OK, MessageBoxImage.Error);
          }
        }
      }

      ClearDialogWnd();
    }

    private void tbExport_Click(object sender, RoutedEventArgs e)
    {
      var exportWnd = new ExportDialog();
      SetDialogWnd(exportWnd);
      ExportSettings exportSettings = exportWnd.GetExportSettings(ProjectsList, MttSettings);

      if (exportSettings != null)
      {

        ObservableCollection<Task> exportList = new ObservableCollection<Task>();
        foreach (var item in exportSettings.Projects)
          if (item.IsChecked) exportList.Add((Task)ProjectsList.Single(p => p.ID == item.Id));

        SaveProjectsList(exportList, exportSettings.ExportPath, exportSettings.FileName);

        if (exportSettings.RemoveAfterExport)
          foreach (var item in exportList) ProjectsList.Remove(item);
      }

      ClearDialogWnd();
    }

    private void SaveProjectsList(ObservableCollection<Task> list, string path, string fileName)
    {
      if (!Directory.Exists(path)) Directory.CreateDirectory(path);
      string fullfilename = string.Format("{0}\\{1}", path, fileName);

      FileStream fs = new FileStream(fullfilename, FileMode.Create, FileAccess.Write);
      XmlSerializer ser = new XmlSerializer(list.GetType());
      ser.Serialize(fs, list);
      fs.Close();
    }

    private void tbImport_Click(object sender, RoutedEventArgs e)
    {
      var importWnd = new ImportDialog();
      SetDialogWnd(importWnd);
      var importSettings = importWnd.GetImportSettings(MttSettings);
      if (importSettings != null) ImportProjects(importSettings);
      ClearDialogWnd();
    }

    private void ImportProjects(ImportSettings importSettings)
    {
      ObservableCollection<Task> import = OpenProjectsFile(importSettings.FileName);

      if (import == null) return;

      foreach (var item in import)
      {
        if (importSettings.MergeMode == MergeEnum.Never)
        {
          AddProjectToMainList(item);
        }
        if (importSettings.MergeMode == MergeEnum.Always)
        {
          if (ProjectsList.Count(p => p.Name == item.Name) > 0)
          {
            var parent = ProjectsList.First(p => p.Name == item.Name);
            foreach (var child in item.SubTasks)
            {
              parent.SubTasks.Add(child);
            }
          }
          else
          {
            AddProjectToMainList(item);
          }
        }
        if (importSettings.MergeMode == MergeEnum.Ask)
        {
          if (ProjectsList.Count(p => p.Name == item.Name) > 0)
          {
            if (MessageBox.Show(
              string.Format("При импорте обнаружены проекты с одинаковыми именами: {0}{1}{2}Объеденить проект с существующим?",
                Environment.NewLine, item.Name, Environment.NewLine),
                "Совпадение имён проектов", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
              var parent = ProjectsList.First(p => p.Name == item.Name);
              foreach (var child in item.SubTasks)
              {
                parent.SubTasks.Add(child);
              }
            }
            else
            {
              AddProjectToMainList(item);
            }
          }
          else
          {
            AddProjectToMainList(item);
          }
        }
      }
    }

    private void AddProjectToMainList(Task item)
    {
      foreach (var child in item.SubTasks)
      {
        child.Parent = item;
      }
      item.SubTasks.CollectionChanged += UpdateProjectGridsSource;
      item.SubTasks.CollectionChanged += SetParent;
      item.SubTasks.CollectionChanged += SetSelectedTask;
      ProjectsList.Add(item);
    }

    private ObservableCollection<Task> OpenProjectsFile(string p)
    {
      if (!System.IO.File.Exists(p)) return null;
      ObservableCollection<Task> result = new ObservableCollection<Task>();
      FileStream fs = null;

      try
      {
        XmlSerializer ser = new XmlSerializer(result.GetType());
        fs = new FileStream(p, FileMode.Open, FileAccess.Read);
        result = (ser.Deserialize(fs) as ObservableCollection<Task>);
      }
      catch
      {
        MessageBox.Show(string.Format("Не удалось прочитать файл сохранения по адресу: \n\r{0}", p),
            "Файл не найден", MessageBoxButton.OK, MessageBoxImage.Error);
        result = null;
      }
      finally
      {
        if (fs != null) fs.Close();
      }

      return result;
    }

    #endregion #Button click events

    #region NotifyPropertyChanged

    private void Notify(string property)
    {
      if (PropertyChanged != null)
      {
        PropertyChanged(this, new PropertyChangedEventArgs(property));
      }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    #endregion NotifyPropertyChanged

    #region window messages hook

    protected override void OnSourceInitialized(EventArgs e)
    {
      base.OnSourceInitialized(e);
      HwndSource source = HwndSource.FromHwnd(new WindowInteropHelper(this).Handle);
      source.AddHook(new HwndSourceHook(WndProc));
    }

    private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
    {
      System.Windows.Forms.Message m = System.Windows.Forms.Message.Create(hwnd, msg, wParam, lParam);
      if (m.Msg == NativeMethods.WM_COPYDATA)
      {
        // Get the COPYDATASTRUCT struct from lParam.
        NativeMethods.COPYDATASTRUCT cds = (NativeMethods.COPYDATASTRUCT)m.GetLParam(typeof(NativeMethods.COPYDATASTRUCT));

        // If the size matches
        if (cds.cbData == Marshal.SizeOf(typeof(NativeMethods.MyStruct)))
        {
          // Marshal the data from the unmanaged memory block to a 
          // MyStruct managed struct.
          NativeMethods.MyStruct myStruct = (NativeMethods.MyStruct)Marshal.PtrToStructure(cds.lpData,
              typeof(NativeMethods.MyStruct));

          // Display the MyStruct data members.
          if (myStruct.Message == "ShowMyTimeTrackerMainWindow")
          {
            this.Show();
            this.WindowState = System.Windows.WindowState.Normal;
          }
        }
      }
      return IntPtr.Zero;
    }

    #endregion window messages hook

    #region IDisposable members

    protected virtual void Dispose(bool disposing)
    {
      if (disposing)
      {
        if (ni != null)
        {
          //hide and dispose ni
          ni.Visible = false;
          ni.Dispose();
          ni = null;
        }
      }
      else
      {
        //throw new NotImplementedException("MainWindow does not have any unmanaged resourses. I hope so...");
      }
    }

    public void Dispose()
    {
      Dispose(true);
      GC.SuppressFinalize(this);
    }

    ~MainWindow()
    {
      Dispose(false);
    }

    #endregion IDisposable

    #region Drag and Drop Rows

    /// <summary>
    /// DraggedItem Dependency Property
    /// </summary>
    public static readonly DependencyProperty DraggedItemProperty =
        DependencyProperty.Register("DraggedItem", typeof(Task), typeof(MainWindow));

    public Task DraggedItem
    {
      get { return (Task)GetValue(DraggedItemProperty); }
      set { SetValue(DraggedItemProperty, value); }
    }

    /// <summary>
    /// Keeps in mind whether
    /// </summary>
    public bool IsDragging { get; set; }

    /// <summary>
    /// Initiates a drag action if the grid is not in edit mode.
    /// </summary>
    private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
      var row = UIHelpers.TryFindFromPoint<DataGridRow>((UIElement)sender, e.GetPosition(ProjectGrid));
      if (row == null) return;

      //set flag that indicates we're capturing mouse movements
      IsDragging = true;
      DraggedItem = (Task)row.Item;
    }


    /// <summary>
    /// Completes a drag/drop operation.
    /// </summary>
    private void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      if (!IsDragging || DraggedItem == null)
      {
        return;
      }

      //make sure the row under the grid is being selected
      Point position = e.GetPosition(ProjectGrid);
      var row = UIHelpers.TryFindFromPoint<DataGridRow>(ProjectGrid, position);
      if (row != null)
      {
        //get the target item
        Task targetItem = (Task)row.Item;
        if (targetItem.HasParent) targetItem = targetItem.Parent;

        if (targetItem != null && !ReferenceEquals(DraggedItem, targetItem) && !ReferenceEquals(DraggedItem.Parent, targetItem))
        {

          if (MessageBox.Show(
                string.Format("Перенести {2}'{0}' {2}к проекту: {2}'{1}'?", DraggedItem, targetItem, Environment.NewLine),
                "Перенос проекта (задачи)",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question,
                MessageBoxResult.Yes) == MessageBoxResult.No)
          {
            ResetDragDrop();
            return;
          }

          while (DraggedItem.HasChild)
            MoveTask(DraggedItem.SubTasks[0], targetItem);

          MoveTask(DraggedItem, targetItem);

        }
      }

      //reset
      ResetDragDrop();
    }

    private void MoveTask(Task item, Task target)
    {
      if (item.HasParent) item.Parent.SubTasks.Remove(item);
      else ProjectsList.Remove(item);

      item.Parent = target;
      target.SubTasks.Add(item);
    }


    /// <summary>
    /// Closes the popup and resets the
    /// grid to read-enabled mode.
    /// </summary>
    private void ResetDragDrop()
    {
      IsDragging = false;
      DraggedItem = null;
      DragPopup.IsOpen = false;
    }

    private bool OutOfGrid { get; set; }

    /// <summary>
    /// Updates the popup's position in case of a drag/drop operation.
    /// </summary>
    private void OnMouseMove(object sender, MouseEventArgs e)
    {
      if (!IsDragging || e.LeftButton != MouseButtonState.Pressed) return;

      //display the popup if it hasn't been opened yet
      if (!DragPopup.IsOpen && !OutOfGrid)
      {
        //make sure the popup is visible
        DragPopup.IsOpen = true;
      }

      double rowheight = DragPopup.ActualHeight;
      //var r = UIHelpers.GetFirstVisualChild<DataGridRow>(ProjectGrid);
      //if (r != null) rowheight = r.ActualHeight;
      Size popupSize = new Size(DragPopup.ActualWidth, rowheight);
      DragPopup.PlacementRectangle = new Rect(e.GetPosition(ProjectGrid), popupSize);

      //make sure the row under the grid is being selected
      Point position = e.GetPosition(ProjectGrid);
      var row = UIHelpers.TryFindFromPoint<DataGridRow>(ProjectGrid, position);

      if (row != null)
      {
        OutOfGrid = false;
        ProjectGrid.SelectedItem = row.Item;
      }
      else { OutOfGrid = true; }

      if (OutOfGrid) { DragPopup.IsOpen = false; }
      else { DragPopup.IsOpen = true; }
    }

    #endregion


    #region Focus window

    private Window dialogWnd = null;

    private void SetDialogWnd(Window wnd)
    {
      dialogWnd = wnd;
      this.Effect = new System.Windows.Media.Effects.BlurEffect();
    }

    private void ClearDialogWnd()
    {
      dialogWnd = null;
      this.Effect = null;
    }

    private void Window_Activated(object sender, EventArgs e)
    {
      if (dialogWnd != null) dialogWnd.Activate();
    }

    #endregion Focus window

  }
}
