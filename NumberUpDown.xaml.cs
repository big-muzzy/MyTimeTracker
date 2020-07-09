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
    /// Логика взаимодействия для NumberUpDown.xaml
    /// </summary>
    public partial class NumberUpDown : UserControl
    {
        private int _numValue = 0;
        public int NumValue
        {
            get { return _numValue; }
            set
            {
                _numValue = value;
                txtNum.Text = value.ToString();
            }
        }

        public int MaxValue { get; set; }
        public int MinValue { get; set; }

        public NumberUpDown()
        {
            InitializeComponent();
        }

        private void cmdUp_Click(object sender, RoutedEventArgs e)
        {
            if (NumValue < MaxValue) { NumValue++; }
            else { NumValue = MinValue; }
        }

        private void cmdDown_Click(object sender, RoutedEventArgs e)
        {
            if (NumValue > MinValue) { NumValue--; }
            else { NumValue = MaxValue; }
        }

        private void txtNum_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!int.TryParse(txtNum.Text, out _numValue))
                txtNum.Text = _numValue.ToString();
        }
    }
}
