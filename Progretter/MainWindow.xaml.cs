﻿using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using System.Data;

namespace Progretter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region 초기설정
        public MainWindow()
        {
            InitializeComponent();
            text_size.Text = Text.FontSize.ToString();
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        // 현재 시간 표시
        private void Timer_Tick(object sender, EventArgs e)
        {
            Timenow.Content = DateTime.Now.ToString();

            if (DateTime.Now.Second == 0)
            {
/*                switch(DateTime.Now.Hour) //현재 속한 교시 + 그에 맞는 알림
                {
                    case period && rest5min:
                        break;
                }*/
                Notification("시간 변경 알림", "지금은 수학시간 5분 전 입니다.");
            }
        }

        private void Notification(string NotiTitle, string NotiContents)
        {
            // Requires Microsoft.Toolkit.Uwp.Notifications NuGet package version 7.0 or greater
            new ToastContentBuilder()
                .AddArgument("action", "viewConversation")
                .AddArgument("conversationId", 9813)
                .AddText($"{NotiTitle}")
                .AddText($"{NotiContents}")
                .Show(); // Not seeing the Show() method? Make sure you have version 7.0, and if you're using .NET 5, your TFM must be net5.0-windows10.0.17763.0 or greater
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (Config.Get("ScheduleIsCheckBox") == "true")
            {

            }

            System.Windows.Markup.XmlLanguage cond = System.Windows.Markup.XmlLanguage.GetLanguage(System.Globalization.CultureInfo.CurrentUICulture.Name);
            List<string> listFont = new List<string>();
            foreach (FontFamily font in Fonts.SystemFontFamilies)
            {
                if (font.FamilyNames.ContainsKey(cond))
                    listFont.Add(font.FamilyNames[cond]);
                else
                    listFont.Add(font.ToString());
            }

            listFont.Sort();

            text_family_combo.ItemsSource = listFont;
            text_family_combo.SelectedIndex = 6;

            List<string> listSize = new List<string>() { "8", "9", "10", "11", "12", "14", "16", "18", "20", "24", "28", "32", "48", "54", "72", "88", "96", "128", "144", "240", "288", "324", "480", "500" };
            text_size_combo.ItemsSource = listSize;
            text_size_combo.SelectedIndex = 4;


            // DataTable 생성
            DataTable dataTable = new DataTable();

            // 컬럼 생성

            dataTable.Columns.Add("Period", typeof(string));
            dataTable.Columns.Add("Monday", typeof(string));
            dataTable.Columns.Add("Tuesday", typeof(string));
            dataTable.Columns.Add("Wednesday", typeof(string));
            dataTable.Columns.Add("Thursday", typeof(string));
            dataTable.Columns.Add("Friday", typeof(string));

            //토요일 일요일
/*            dataTable.Columns.Add("Saturday", typeof(string));
            dataTable.Columns.Add("Sunday", typeof(string));*/
//xmal
/*          < DataGridTextColumn Header = "토요일" Binding = "{Binding Path=Saturday}" />
            < DataGridTextColumn Header = "일요일" Binding = "{Binding Path=Sunday}" />*/

            // 데이터 생성
            // dataTable.Rows.Add(new string[] { "1교시", "월요일", "화요일", "수요일", "목요일", "금요일"/*, "토요일", "일요일"*/ });
            dataTable.Rows.Add(new string[] { "1교시", "수학", "도덕" });
            dataTable.Rows.Add(new string[] { "2교시", "영어", "역사" });
            dataTable.Rows.Add(new string[] { "3교시", "국어", "수학" });
            dataTable.Rows.Add(new string[] { "4교시", "사회", "영어" });
            dataTable.Rows.Add(new string[] { "5교시", "과학", "국어" });
            dataTable.Rows.Add(new string[] { "6교시", "역사", "체육" });
            dataTable.Rows.Add(new string[] { "7교시", "기술", "미술" });

            // DataTable의 Default View를 바인딩하기
            Schedule.ItemsSource = dataTable.DefaultView;
        }
        #endregion

        #region 설정
        private void chk_ScheduleCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            Config.Set("ScheduleIsCheckBox", "true");
        }

        private void chk_ScheduleCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            Config.Set("ScheduleIsCheckBox", "false");
        }
        #endregion

        #region 시간표
        #endregion

        #region 메모장
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
        private void text_family_combo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Text.FontFamily = new FontFamily(text_family_combo.SelectedItem.ToString());
        }

        private string textsize;

        private void text_size_combo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int returnVal;
            if (textsize != "key")
            {
                bool bl = int.TryParse(text_size_combo.SelectedItem.ToString(), out returnVal);
                if (bl)
                {
                    Text.FontSize = double.Parse(text_size_combo.SelectedItem.ToString());
                    text_size.Text = text_size_combo.SelectedItem.ToString();
                }
            }
            textsize = "combo";
        }

        private void text_size_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                int returnVal;
                bool bl = int.TryParse(text_size.Text, out returnVal);
                if (bl)
                {
                    if (Convert.ToInt32(text_size.Text) <= 500)
                    {
                        Text.FontSize = double.Parse(text_size.Text);
                        textsize = "key";
                        text_size_combo.Text = null;
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
        #endregion

        #region 계산기
        double Result_Value;
        string Operator_Performed = " ";
        bool PerformedOp;
        string callog;

        private void Cal_num(object sender, RoutedEventArgs e)
        {
            // numbers button
            if (txtResult.Text == "0" || PerformedOp)
            {
                txtResult.Clear();
            }
            PerformedOp = false;
            Button button = (Button)sender;
            txtResult.Text += button.Content;
        }

        private void Cal_dot(object sender, RoutedEventArgs e)
        {
            // point
            Button button = (Button)sender;
            if (!txtResult.Text.Contains("."))
            {
                txtResult.Text += button.Content;
            }
        }
        private void Cal_op(object sender, RoutedEventArgs e)
        {
            // +, -, *, / operators
            callog += txtResult.Text + " ";
            Button button = (Button)sender;
            if (!PerformedOp && Convert.ToBoolean(txtResult.Text.Contains(".")))
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
            else if (!PerformedOp && !Convert.ToBoolean(txtResult.Text.Contains(".")))
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
            else
            {
                callog += "0";
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
        }

        private void Cal_equal(object sender, RoutedEventArgs e)
        {
            // EQUALS BUTTON
            callog += " " + txtResult.Text;
            switch (Operator_Performed)
            {
                case "+":
                    txtResult.Text = (Result_Value + double.Parse(txtResult.Text)).ToString();
                    callog += " = " + txtResult.Text.ToString();
                    Cal_log.Items.Add(callog);
                    break;

                case "-":
                    txtResult.Text = (Result_Value - double.Parse(txtResult.Text)).ToString();
                    callog += " = " + txtResult.Text.ToString();
                    Cal_log.Items.Add(callog);
                    break;

                case "×":
                    txtResult.Text = (Result_Value * double.Parse(txtResult.Text)).ToString();
                    callog += " = " + txtResult.Text.ToString();
                    Cal_log.Items.Add(callog);
                    break;

                case "÷":
                    txtResult.Text = (Result_Value / double.Parse(txtResult.Text)).ToString();
                    callog += " = " + txtResult.Text.ToString();
                    Cal_log.Items.Add(callog);
                    break;

                default:
                    break;

            }
            Result_Value = double.Parse(txtResult.Text);
            Label_log.Content = " ";
            Operator_Performed = " ";
            Label_log.Content = " ";
            PerformedOp = false;
            callog = null;
        }

        private void Cal_plusminus(object sender, RoutedEventArgs e)
        {
            double v = double.Parse(txtResult.Text);
            txtResult.Text = (-v).ToString();
        }

        private void Calculator_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.D0:
                case Key.NumPad0:
                    Cal0.RaiseEvent(new RoutedEventArgs(System.Windows.Controls.Primitives.ButtonBase.ClickEvent));
                    break;

                case Key.D1:
                case Key.NumPad1:
                    Cal1.RaiseEvent(new RoutedEventArgs(System.Windows.Controls.Primitives.ButtonBase.ClickEvent));
                    break;

                case Key.D2:
                case Key.NumPad2:
                    Cal2.RaiseEvent(new RoutedEventArgs(System.Windows.Controls.Primitives.ButtonBase.ClickEvent));
                    break;

                case Key.D3:
                case Key.NumPad3:
                    Cal3.RaiseEvent(new RoutedEventArgs(System.Windows.Controls.Primitives.ButtonBase.ClickEvent));
                    break;

                case Key.D4:
                case Key.NumPad4:
                    Cal4.RaiseEvent(new RoutedEventArgs(System.Windows.Controls.Primitives.ButtonBase.ClickEvent));
                    break;

                case Key.D5:
                case Key.NumPad5:
                    Cal5.RaiseEvent(new RoutedEventArgs(System.Windows.Controls.Primitives.ButtonBase.ClickEvent));
                    break;

                case Key.D6:
                case Key.NumPad6:
                    Cal6.RaiseEvent(new RoutedEventArgs(System.Windows.Controls.Primitives.ButtonBase.ClickEvent));
                    break;

                case Key.D7:
                case Key.NumPad7:
                    Cal7.RaiseEvent(new RoutedEventArgs(System.Windows.Controls.Primitives.ButtonBase.ClickEvent));
                    break;

                case Key.D8:
                case Key.NumPad8:
                    Cal8.RaiseEvent(new RoutedEventArgs(System.Windows.Controls.Primitives.ButtonBase.ClickEvent));
                    break;

                case Key.D9:
                case Key.NumPad9:
                    Cal9.RaiseEvent(new RoutedEventArgs(System.Windows.Controls.Primitives.ButtonBase.ClickEvent));
                    break;

                case Key.OemPeriod:
                    CalPer.RaiseEvent(new RoutedEventArgs(System.Windows.Controls.Primitives.ButtonBase.ClickEvent));
                    break;

                case Key.Add:
                case Key.F1:
                    CalPlus.RaiseEvent(new RoutedEventArgs(System.Windows.Controls.Primitives.ButtonBase.ClickEvent));
                    break;

                case Key.Subtract:
                case Key.F2:
                    CalSub.RaiseEvent(new RoutedEventArgs(System.Windows.Controls.Primitives.ButtonBase.ClickEvent));
                    break;

                case Key.Multiply:
                case Key.F3:
                    CalMul.RaiseEvent(new RoutedEventArgs(System.Windows.Controls.Primitives.ButtonBase.ClickEvent));
                    break;

                case Key.Divide:
                case Key.F4:
                    CalDiv.RaiseEvent(new RoutedEventArgs(System.Windows.Controls.Primitives.ButtonBase.ClickEvent));
                    break;

                case Key.OemPlus:
                case Key.Enter:
                case Key.F5:
                    Cal_equal_btn.RaiseEvent(new RoutedEventArgs(System.Windows.Controls.Primitives.ButtonBase.ClickEvent));
                    break;

                case Key.F6:
                    CalC.RaiseEvent(new RoutedEventArgs(System.Windows.Controls.Primitives.ButtonBase.ClickEvent));
                    break;
            }
        }

        private void Cal_back(object sender, RoutedEventArgs e)
        {
            txtResult.Text = txtResult.Text.Remove(txtResult.Text.Length - 1);
            if (txtResult.Text.Length == 0)
            {
                txtResult.Text = "0";
            }
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
            }
        }
        private void Cal_log_remove_Click(object sender, RoutedEventArgs e)
        {
            Cal_log.Items.Clear();
        }

        private void Cal_log_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            string loadlog = Cal_log.Items.GetItemAt(Cal_log.SelectedIndex).ToString();
            string loadresult = loadlog.Substring(loadlog.IndexOf("=") + 2);
            txtResult.Text = loadresult;
        }
        #endregion

        #region 테트리스
        private void Tetris_btn_Click(object sender, RoutedEventArgs e)
        {
            Tetris tetris = new Tetris();
            tetris.Show();
        }
        #endregion
    }
}