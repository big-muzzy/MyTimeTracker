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
  /// Логика взаимодействия для TaskCell.xaml
  /// </summary>
  public partial class TaskCell : UserControl
  {

    public static readonly DependencyProperty ItemProperty = DependencyProperty.Register(
        "Item", typeof(Task), typeof(TaskCell), new PropertyMetadata(new PropertyChangedCallback(ItemChanged)));

    public static void ItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      //todo: something
    }

    public Task Item
    {
      get { return (Task)GetValue(ItemProperty); }
      set { SetValue(ItemProperty, value); }
    }

    public TaskCell()
    {
      InitializeComponent();
    }
  }
}
