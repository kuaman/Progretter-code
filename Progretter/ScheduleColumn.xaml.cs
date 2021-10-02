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

namespace Progretter
{
    /// <summary>
    /// ScheduleColumn.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ScheduleColumn : Window
    {
        public static string columnindex;

        public ScheduleColumn(string column)
        {
            InitializeComponent();
            TextBox1.Text = column + "열";
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            columnindex = TextBox1.Text;
            this.DialogResult = true;
            this.Close();
        }
    }
}
