using System.Windows;

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

        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                confirm.RaiseEvent(new RoutedEventArgs(System.Windows.Controls.Primitives.ButtonBase.ClickEvent));
            }
        }
    }
}
