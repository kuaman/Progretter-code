using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows.Threading;

namespace Progretter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // 초기설정
        public MainWindow()
        {
            InitializeComponent();
            text_size.Text = Text.FontSize.ToString();
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Tick += Timer_Tick;
            timer.Start();
            // Requires Microsoft.Toolkit.Uwp.Notifications NuGet package version 7.0 or greater
/*            new ToastContentBuilder()
                .AddArgument("action", "viewConversation")
                .AddArgument("conversationId", 9813)
                .AddText("Andrew sent you a picture")
                .AddText("Check this out, The Enchantments in Washington!")
                .Show(); // Not seeing the Show() method? Make sure you have version 7.0, and if you're using .NET 5, your TFM must be net5.0-windows10.0.17763.0 or greater*/
        }

        private double[] fontSizeArray = new double[]
        {
              8.0d,   9.0d,  10.0d,  10.5d,  11.0d,  12.0d,  14.0d,  16.0d,  18.0d,  20.0d,   24.0d,  28.0d,
             32.0d,  36.0d,  40.0d,  44.0d,  48.0d,  54.0d,  60.0d,  66.0d,  72.0d,  80.0d,  88.0d,  96.0d,
            128.0d, 136.0d, 144.0d,  160.0d, 192.0d, 256.0d, 288.0d, 352.0d, 480.0d, 500.0d
        };

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (FontFamily fontFamily in Fonts.SystemFontFamilies)
            {
                text_family_combo.Items.Add(fontFamily.Source);
            }

            text_family_combo.SelectedIndex = 24;
        }

        // 현재 시간 표시
        private void Timer_Tick(object sender, EventArgs e)
        {
            Timenow.Content = DateTime.Now.ToString();
        }


        // 메모장 함수
        private void bold_Btn_Checked(object sender, RoutedEventArgs e)
        {
            Text.FontWeight = FontWeights.Bold;
        }
        private void bold_Btn_Unchecked(object sender, RoutedEventArgs e)
        {
            Text.FontWeight = FontWeights.Normal;
        }
        private void Italic_Btn_Checked(object sender, RoutedEventArgs e)
        {
            Text.FontStyle = FontStyles.Italic;
        }
        private void Italic_Btn_Unchecked(object sender, RoutedEventArgs e)
        {
            Text.FontStyle = FontStyles.Normal;
        }
        private void underline_Btn_Checked(object sender, RoutedEventArgs e)
        {
            if (strikethrough_Btn.IsChecked == true)
            {
                strikethrough_Btn.IsChecked = false;
            }
            Text.TextDecorations = TextDecorations.Underline;
        }
        private void underline_Btn_Unchecked(object sender, RoutedEventArgs e)
        {
            if (strikethrough_Btn.IsChecked == true)
            {
                Text.TextDecorations = TextDecorations.Strikethrough;
            }
            else
            {
                Text.TextDecorations = null;
            }
        }
        private void strikethrough_Btn_Checked(object sender, RoutedEventArgs e)
        {
            if (underline_Btn.IsChecked == true)
            {
                underline_Btn.IsChecked = false;
            }
            Text.TextDecorations = TextDecorations.Strikethrough;
        }
        private void strikethrough_Btn_Unchecked(object sender, RoutedEventArgs e)
        {
            if (underline_Btn.IsChecked == true)
            {
                Text.TextDecorations = TextDecorations.Underline;
            }
            else
            {
                Text.TextDecorations = null;
            }
        }
        private void text_size_combo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Console.WriteLine(text_size_combo.Text);
            Console.WriteLine("1");
        }

        private void text_size_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                int returnVal = 0;
                bool bl = int.TryParse(text_size.Text, out returnVal);
                if (bl == true)
                {
                    if (Convert.ToInt32(text_size.Text) <= 500)
                    {
                        Text.FontSize = double.Parse(text_size.Text);
                    }
                    else // 500 초과
                    {
                        MessageBox.Show("글자 크기는 500을 넘을 수 없습니다.");
                        text_size.Text = null;
                    }
                }
                else
                {
                    MessageBox.Show("글자 크기에는 숫자만 들어갈 수 있습니다.");
                    text_size.Text = null;
                }
            }
        }


        // 계산기 코드
        double Result_Value;
        string Operator_Performed = " ";
        bool PerformedOp;
        string callog;
        string plusminus;

        private void Cal_num(object sender, RoutedEventArgs e)
        {
            // numbers button
            if (txtResult.Text == "0" || PerformedOp)
            {
                txtResult.Clear();
            }
            else
            {
                callog += "0";
            }
            PerformedOp = false;
            Button button = (Button)sender;
            txtResult.Text += button.Content;
            callog += button.Content.ToString();
        }

        private void Cal_dot(object sender, RoutedEventArgs e)
        {
            // point
            Button button = (Button)sender;
            if (!txtResult.Text.Contains("."))
            {
                txtResult.Text += button.Content;
                callog += button.Content.ToString();
            }
            PerformedOp = true;
        }
        private void Cal_op(object sender, RoutedEventArgs e)
        {
            // +, -, *, / operators
            Button button = (Button)sender;
            if (!PerformedOp)
            {
                if (Result_Value != 0)
                {
                    Cal_equal_btn.RaiseEvent(new RoutedEventArgs(System.Windows.Controls.Primitives.ButtonBase.ClickEvent));
                    Operator_Performed = button.Content.ToString();
                    Label_log.Content = Result_Value + " " + Operator_Performed;
                    PerformedOp = true;
                    callog += button.Content.ToString();
                }
                else
                {
                    Operator_Performed = button.Content.ToString();
                    Result_Value = double.Parse(txtResult.Text);
                    Label_log.Content = Result_Value + " " + Operator_Performed;
                    PerformedOp = true;
                    callog += button.Content.ToString();
                }
            }
        }

        private void Cal_CE(object sender, RoutedEventArgs e)
        {
            //CLEAR ENTRY BUTTON
            txtResult.Text = "0";
        }

        private void Cal_C(object sender, RoutedEventArgs e)
        {
            //CLEAR BUTTON
            txtResult.Text = "0";
            Result_Value = 0;
            Operator_Performed = " ";
            Label_log.Content = " ";
            PerformedOp = false;
            callog = null;
            plusminus = null;
        }

        private void Cal_equal(object sender, RoutedEventArgs e)
        {
            // EQUALS BUTTON
            switch (Operator_Performed)
            {
                case "+":
                    txtResult.Text = (Result_Value + double.Parse(txtResult.Text)).ToString();
                    break;

                case "-":
                    txtResult.Text = (Result_Value - double.Parse(txtResult.Text)).ToString();
                    break;

                case "×":
                    txtResult.Text = (Result_Value * double.Parse(txtResult.Text)).ToString();
                    break;

                case "÷":
                    txtResult.Text = (Result_Value / double.Parse(txtResult.Text)).ToString();
                    break;

                default:
                    break;

            }
            Result_Value = double.Parse(txtResult.Text);
            Label_log.Content = " ";
            callog += " = " + Result_Value.ToString();
            Cal_log.Items.Add(callog);
            callog = Result_Value.ToString();
            Operator_Performed = " ";
            Label_log.Content = " ";
            PerformedOp = false;
            callog = null;
            plusminus = null;
        }

        private void Cal_plusminus(object sender, RoutedEventArgs e) // 곱하기 나누기 뒤에 음수오는 경우 추가
        {
            switch (plusminus)
            {
                case null:
                    if (txtResult.Text != "0")
                    {
                        plusminus = "minus";
                        Result_Value = double.Parse(txtResult.Text);
                        txtResult.Text = Result_Value.ToString().Insert(0, "-");
                    }
                    break;

                case "plus":
                    if (txtResult.Text != "0")
                    {
                        plusminus = "minus";
                        Result_Value = double.Parse(txtResult.Text);
                        txtResult.Text = Result_Value.ToString().Insert(0, "-");
                    }
                    break;

                case "minus":
                    if (txtResult.Text != "0")
                    {
                        plusminus = "plus";
                        Result_Value = double.Parse(txtResult.Text);
                        txtResult.Text = Result_Value.ToString().Remove(0, 1);
                    }
                    break;
            }
        }

        private void Cal_back(object sender, RoutedEventArgs e)
        {
            txtResult.Text = Result_Value.ToString().Remove(Convert.ToInt32(Result_Value.ToString().Length) - 1); // 로그에도 적용
        }

        private void txtResult_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtResult.Text.Length > 20)
            {
                MessageBox.Show("[ERROR] 계산값이 20자리를 초과하였습니다.");
                txtResult.Text = "0";
                Result_Value = 0;
                Operator_Performed = " ";
                Label_log.Content = " ";
                PerformedOp = false;
                Cal_log.Items.Add(callog);
                callog = null;
                plusminus = null;
            }
        }
        private void Cal_log_remove_Click(object sender, RoutedEventArgs e)
        {
            Cal_log.Items.Clear();
        }

        // 테트리스 코드
        private void Tetris_btn_Click(object sender, RoutedEventArgs e)
        {
            Tetris tetris = new Tetris();
            tetris.Show();
        }
    }
}
