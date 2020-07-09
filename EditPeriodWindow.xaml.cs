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
    /// Логика взаимодействия для EditPeriodWindow.xaml
    /// </summary>
    public partial class EditPeriodWindow : Window
    {
        public EditPeriodWindow()
        {
            InitializeComponent();
        }

        public Period GetPeriod(Period old)
        {
            StartDate.SelectedDate = old.Start.Date;
            StartHour.NumValue = old.Start.Hour;
            StartMinute.NumValue = old.Start.Minute;
            StartSecond.NumValue = old.Start.Second;

            EndDate.SelectedDate = old.Finish.Date;
            EndHour.NumValue = old.Finish.Hour;
            EndMinute.NumValue = old.Finish.Minute;
            EndSecond.NumValue = old.Finish.Second;

            if (this.ShowDialog() == true)
            {
                DateTime start = new DateTime(
                        StartDate.SelectedDate.Value.Year,
                        StartDate.SelectedDate.Value.Month,
                        StartDate.SelectedDate.Value.Day,
                        StartHour.NumValue,
                        StartMinute.NumValue,
                        StartSecond.NumValue);
                DateTime end = new DateTime(
                        EndDate.SelectedDate.Value.Year,
                        EndDate.SelectedDate.Value.Month,
                        EndDate.SelectedDate.Value.Day,
                        EndHour.NumValue,
                        EndMinute.NumValue,
                        EndSecond.NumValue);
                if (start.Ticks > 0 && end.Ticks > 0)
                    if (end.Ticks > start.Ticks)
                        return new Period()
                        {
                            Start = start,
                            Finish = end
                        };
            }
            return null;
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void startNow_Click(object sender, RoutedEventArgs e)
        {
            StartDate.SelectedDate = DateTime.Today;
            StartHour.NumValue = DateTime.Now.Hour;
            StartMinute.NumValue = DateTime.Now.Minute;
            StartSecond.NumValue = DateTime.Now.Second;
        }

        private void endNow_Click(object sender, RoutedEventArgs e)
        {
            EndDate.SelectedDate = DateTime.Today;
            EndHour.NumValue = DateTime.Now.Hour;
            EndMinute.NumValue = DateTime.Now.Minute;
            EndSecond.NumValue = DateTime.Now.Second;
        }
    }
}
