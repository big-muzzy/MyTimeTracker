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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MyTimeTracker
{
  /// <summary>
  /// Логика взаимодействия для TaskDetails.xaml
  /// </summary>
  public partial class TaskDetails : UserControl
  {
    public static readonly DependencyProperty ItemProperty = DependencyProperty.Register(
    "Item", typeof(Task), typeof(TaskDetails), new PropertyMetadata(new PropertyChangedCallback(ItemChanged)));

    public static void ItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      //todo: something
    }

    public Task Item
    {
      get { return (Task)GetValue(ItemProperty); }
      set { SetValue(ItemProperty, value); }
    }

    public TaskDetails()
    {
      InitializeComponent();
    }

    private FrameworkElement GetParent(FrameworkElement element)
    {
      //MessageBox.Show(string.Format("{0}: {1}", element.GetType().ToString(), element.Name));
      if (element.Parent == null) return element;
      else return GetParent((FrameworkElement)element.Parent);
    }

    private void DeletePeriod_Click(object sender, RoutedEventArgs e)
    {
      //MessageBox.Show(string.Format("delete {0}", (sender as Button).Parent.GetType().ToString()));
      var value = (PeriodsGrid.SelectedItem as Period);

      if (ReferenceEquals(Item.ActivePeriod, value) && Item.HasOpenedPeriod)
      {
        MessageBox.Show("Остановите таймер для удаления выбранного периода", "Нельзя удалять активный период", MessageBoxButton.OK, MessageBoxImage.Error);
        return;
      }

      if (MessageBox.Show(
        string.Format("Удалить период с {0} по {1} (всего: {2})",
          value.Start.ToString("dd.MM.yyyy HH:mm:ss"),
          value.Finish.ToString("dd.MM.yyyy HH:mm:ss"),
          value.Duration.ToLongTimeString()),
        "Удаление париода работы", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes) == MessageBoxResult.Yes)
      {

        Item.Periods.Remove(value);
      }
    }

    private void EditPeriod_Click(object sender, RoutedEventArgs e)
    {
      var parent = GetParent(this);
      parent.Effect = new System.Windows.Media.Effects.BlurEffect();
      Period value = new EditPeriodWindow().GetPeriod((PeriodsGrid.SelectedItem as Period));
      if (value != null)
      {
        Period period = (PeriodsGrid.SelectedItem as Period);
        if (period != null)
        {
          period.Start = value.Start;
          period.Finish = value.Finish;
          period.UpdateDuration();
          Item.UpdateDuration();
        }
      }
      parent.Effect = null;
    }

  }
}
