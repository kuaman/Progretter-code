﻿using AutoUpdaterDotNET;
using Microsoft.Toolkit.Uwp.Notifications;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

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
            InitialConfig();
            var timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(750) };
            timer.Tick += Timer_Tick;
            timer.Start();
            Uri BaseUri = new Uri("https://raw.githubusercontent.com/kuaman/Progretter-code/master/Progretter/version.xml");
            AutoUpdater.AppCastURL = BaseUri.AbsoluteUri;
            AutoUpdater.CheckForUpdateEvent += AutoUpdaterOnCheckForUpdateEvent;
        }

        private void InitialConfig()
        {
            AppDomain.CurrentDomain.SetData("APP_CONFIG_FILE", Pgpath + @"\app.config");
            AppConfig.Change(Pgpath + @"\app.config");

            if (!File.Exists(Pgpath + @"\app.config"))
            {
                AppDomain.CurrentDomain.SetData("APP_CONFIG_FILE", Pgpath + @"\app.config");
                AppConfig.Change(Pgpath + @"\app.config");
                Config.Set("ScheduleStartUpImport", "false");
                Config.Set("ScheduleStartUpPath", "");
                Config.Set("ScheduleCloseSave", "false");
                Config.Set("TextTheme", "0");
                Config.Set("TextDragAllExtension", "false");
                Config.Set("CalculatorDeleteLog", "false");
                Config.Set("CalculatorLog", "");
                Config.Set("CanvasStrokeSlider", "");
                Config.Set("CanvasEraseMode", "");
                Config.Set("CanvasEraseSlider", "");
                Config.Set("CanvasAutoLoad", "false");
                Config.Set("CanvasAutoSave", "false");
                Config.Set("CanvasLastPath", "");
                Process.Start(Process.GetCurrentProcess().MainModule.FileName);
                Application.Current.Shutdown();
            }

            DirectoryInfo di = new(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Progretter\AutoSave");

            if (!di.Exists)
            {
                di.Create();
            }
        }
        public readonly string Pgpath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Progretter";
        public readonly string ASpath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Progretter\AutoSave";

        public bool NoUpdate_Noti = false;

        // 현재 시간 표시
        private void Timer_Tick(object sender, EventArgs e)
        {
            Timenow.Content = DateTime.Now.ToString();
        }

        private void tabcontrol_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (tabcontrol.SelectedIndex != 4)
            {
                foreach (Window openForm in Application.Current.Windows)
                {
                    if (openForm.Title == "그림판 속성") // 열린 창의 이름 검사
                    {
                        openForm.WindowState = WindowState.Minimized;
                        return;
                    }
                }
            }
            else
            {
                foreach (Window openForm in Application.Current.Windows)
                {
                    if (openForm.Title == "그림판 속성") // 열린 창의 이름 검사
                    {
                        if (openForm.WindowState == WindowState.Minimized)
                        {  // 창을 최소화시켜 하단에 내려놓았는지 검사
                            openForm.WindowState = WindowState.Normal;
                        }
                        openForm.Activate();
                        return;
                    }
                }
            }
        }
        #endregion

        #region Window_Loaded
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (Config.Get("ScheduleStartUpImport") == "true")
            {
                Schedule.ItemsSource = Schedules.ImportExcel(Config.Get("ScheduleStartUpPath")).DefaultView;
            }

            foreach (var item in Config.Get("CalculatorLog").ToString().Split(new char[] { ',' }))
            {
                if (!string.IsNullOrEmpty(item))
                    Cal_log.Items.Add(item);
            }

            switch (Config.Get("TextTheme"))
            {
                case "1":
                    Text.Background = Brushes.Black;
                    Text.Foreground = Brushes.White;
                    break;

                case "2":
                    Text.Background = Brushes.ForestGreen;
                    Text.Foreground = Brushes.White;
                    break;

                default:
                    break;
            }

            if (Config.Get("CanvasAutoLoad") == "true")
            {
                if (Config.Get("CanvasLastPath") != "")
                {
                    ImageLoad(1);
                }
            }

            for (int i = 0; i <= 3; i++)
            {
                alarm_Day.Items.Add(i);
            }
            for (int i = 0; i <= 24; i++)
            {
                alarm_Hour.Items.Add(i);
            }
            for (int i = 0; i <= 60; i++)
            {
                alarm_Minutes.Items.Add(i);
                alarm_Second.Items.Add(i);
            }
            alarm_Day.SelectedIndex = 0;
            alarm_Hour.SelectedIndex = 0;
            alarm_Minutes.SelectedIndex = 0;
            alarm_Second.SelectedIndex = 0;


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

            AutoUpdater.Start("https://raw.githubusercontent.com/kuaman/Progretter-code/master/Progretter/version.xml"); //XML RAW URL
        }
        #endregion

        #region Window_Closing
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (Config.Get("CanvasAutoSave") == "true")
            {
                if (Config.Get("CanvasLastPath") == "") // 빈 상태로 그릴 때
                {
                    if (isCanvasmod || inkCanvas.Strokes.Count > 0) //+그릴떄 활성화
                    {
                        RenderTargetBitmap bitmap = ConverterBitmapImage(inkCanvas);
                        ImageSave(bitmap, 2);
                    }
                }
                else
                {
                    if (inkCanvas.Strokes.Count > 0)
                    {
                        if (!isCanvasmod)
                        {
                            RenderTargetBitmap bitmap = ConverterBitmapImage(inkCanvas);
                            ImageSave(bitmap, 2);
                        }
                        else
                        {
                            if (MessageBox.Show("그림판 변경사항을 이전 파일에 저장하시겠습니까?", "그림판 자동 저장", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                            {
                                RenderTargetBitmap bitmap = ConverterBitmapImage(inkCanvas);
                                ImageSave(bitmap, 1);
                            }
                        }
                    }
                }
            }
        }
        #endregion

        #region Window_Closed
        private void Window_Closed(object sender, EventArgs e)
        {
            if (Config.Get("ScheduleCloseSave") == "true")
            {
                if (Schedule.ItemsSource != null)
                {
                    string path = ASpath + @"\" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
                    Schedules.ExportExcel((DataView)Schedule.ItemsSource, path);
                }
            }
            if (Config.Get("CalculatorDeleteLog") == "false")
            {
                Config.Set("CalculatorLog", string.Empty);
                foreach (var item in Cal_log.Items)
                {
                    Config.Add("CalculatorLog", (string)item);
                }
            }
            else
            {
                Config.Set("CalculatorLog", string.Empty);
            }
            Config.Set("CanvasStrokeSlider", "");
            Config.Set("CanvasEraseMode", "");
            Config.Set("CanvasEraseSlider", "");
        }
        #endregion

        #region 메뉴
        private void Menu_Setting_General(object sender, RoutedEventArgs e)
        {
            Settings settings = new Settings();
            settings.Owner = this;
            settings.Show();
        }
        private void Menu_Help_PCInfo(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo("msinfo32.exe"));
        }
        private void Menu_Help_ProductInfo(object sender, RoutedEventArgs e)
        {
            Info info = new Info();
            info.Owner = this;
            info.Show();
        }
        private void Menu_Help_Update(object sender, RoutedEventArgs e)
        {
            NoUpdate_Noti = true;
            AutoUpdater.CheckForUpdateEvent += AutoUpdaterOnCheckForUpdateEvent;
        }
        #endregion

        #region 알람
        private void Alarm_RemoveAll(object sender, RoutedEventArgs e)
        {
            ToastNotificationManagerCompat.History.Clear();
        }
        private void Alarm_Add(object sender, RoutedEventArgs e)
        {
            int returnVal;
            int day = 0;
            int hour = 0;
            int min = 0;
            int sec = 0;
            if (alarm_Day.Text != null)
            {
                bool bl = int.TryParse(alarm_Day.Text.ToString(), out returnVal);
                if (bl)
                {
                    day = int.Parse(alarm_Day.Text.ToString());
                }
            }
            if (alarm_Hour.Text != null)
            {
                bool bl = int.TryParse(alarm_Hour.Text.ToString(), out returnVal);
                if (bl)
                {
                    hour = int.Parse(alarm_Hour.Text.ToString());
                }
            }
            if (alarm_Minutes.Text != null)
            {
                bool bl = int.TryParse(alarm_Minutes.Text.ToString(), out returnVal);
                if (bl)
                {
                    min = int.Parse(alarm_Minutes.Text.ToString());
                }
            }
            if (alarm_Second.Text != null)
            {
                bool bl = int.TryParse(alarm_Second.Text.ToString(), out returnVal);
                if (bl)
                {
                    sec = int.Parse(alarm_Second.Text.ToString());
                }
            }
            if (Alarm.IsChecked == true && Todo.IsChecked == false)
            {
                Notification(Alarm_Title.Text, Alarm_Contents.Text, 9100, day, hour, min, sec);
            }
            else if (Alarm.IsChecked == false && Todo.IsChecked == true)
            {
                Notification(Alarm_Title.Text, Alarm_Contents.Text, 9000, day, hour, min, sec);
            }
            else
            {
                MessageBox.Show("오류가 발생했습니다. 알람 종류를 선택하여 주십시오.");
            }
        }

        private void Alarm_Checked(object sender, RoutedEventArgs e)
        {
            Todo.IsChecked = false;
        }

        private void Todo_Checked(object sender, RoutedEventArgs e)
        {
            Alarm.IsChecked = false;
        }
        #endregion

        #region 시간표
        private void ImportExcel(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Excel File(*.xlsx)|*.xlsx";
            if (openFileDialog.ShowDialog() == true)
            {
                Schedule.ItemsSource = Schedules.ImportExcel(openFileDialog.FileName).DefaultView;
            }
        }

        private void ExportToExcel(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Excel file (*.xlsx)|*.xlsx";
            if (saveFileDialog.ShowDialog() == true)
            {
                Schedules.ExportExcel((DataView)Schedule.ItemsSource, saveFileDialog.FileName);
            }
        }

        private void Schedule_Row_Add_Btn_Click(object sender, RoutedEventArgs e)
        {
            DataTable dt = new DataTable();
            if (Schedule.ItemsSource != null)
                dt = ((DataView)Schedule.ItemsSource).ToTable();
            dt.Rows.Add();
            Schedule.ItemsSource = dt.DefaultView;
        }

        private void Schedule_Row_Del_Btn_Click(object sender, RoutedEventArgs e)
        {
            if (Schedule.ItemsSource != null)
            {
                DataTable dt = new DataTable();
                dt = ((DataView)Schedule.ItemsSource).ToTable();
                if (dt.Rows.Count > 0)
                    dt.Rows.RemoveAt(dt.Rows.Count - 1);
                Schedule.ItemsSource = dt.DefaultView;
            }
        }

        public string columnindex;

        private void Schedule_Column_Add_Btn_Click(object sender, RoutedEventArgs e)
        {
            ScheduleColumn scheduleColumn = new ScheduleColumn((Schedule.Columns.Count + 1).ToString());
            scheduleColumn.Owner = this;
            scheduleColumn.ShowDialog();
            if (scheduleColumn.DialogResult == true)
            {
                DataTable dt = new DataTable();
                if (Schedule.ItemsSource != null)
                    dt = ((DataView)Schedule.ItemsSource).ToTable();
                dt.Columns.Add(ScheduleColumn.columnindex);
                Schedule.ItemsSource = dt.DefaultView;
            }
        }

        private void Schedule_Column_Del_Btn_Click(object sender, RoutedEventArgs e)
        {
            if (Schedule.ItemsSource != null)
            {
                DataTable dt = new DataTable();
                dt = ((DataView)Schedule.ItemsSource).ToTable();
                if (dt.Columns.Count > 1)
                {
                    dt.Columns.RemoveAt(dt.Columns.Count - 1);
                    Schedule.ItemsSource = dt.DefaultView;
                }
                else
                {
                    Schedule.ItemsSource = null;
                }
            }
        }
        private void Schedule_GotFocus(object sender, RoutedEventArgs e)
        {
            Schedule_Cell_Row_Column();
        }
        private void Schedule_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Schedule_Cell_Row_Column();
        }
        private void Schedule_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            Schedule_Cell_Row_Column();
        }

        private void Schedule_Cell_Row_Column()
        {
            if (Schedule.SelectedIndex > -1 && Schedule.CurrentColumn != null)
                Schedule_Row_Column.Content = "행 : " + Schedule.SelectedIndex.ToString() + ", " + "열 : " + Schedule.CurrentColumn.DisplayIndex.ToString();
            else
                Schedule_Row_Column.Content = "행 : " + ", " + "열 : ";
        }
        #endregion

        #region 메모장
        private void text_load_Btn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Text file (*.txt)|*.txt";
            if (openFileDialog.ShowDialog() == true)
            {
                Text.Text = File.ReadAllText(openFileDialog.FileName);
            }
        }
        private void text_save_Btn_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Text file (*.txt)|*.txt";
            if (saveFileDialog.ShowDialog() == true)
            {
                File.WriteAllText(saveFileDialog.FileName, Text.Text);
            }
        }
        private void text_family_combo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Text.FontFamily = new FontFamily(text_family_combo.SelectedItem.ToString());
        }

        private void text_size_combo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int returnVal;
            if (text_size_combo.SelectedItem != null)
            {
                bool bl = int.TryParse(text_size_combo.SelectedItem.ToString(), out returnVal);
                if (bl)
                {
                    Text.FontSize = double.Parse(text_size_combo.SelectedItem.ToString());
                }
            }
        }

        private void text_size_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                int returnVal;
                bool bl = int.TryParse(text_size_combo.Text, out returnVal);
                if (bl)
                {
                    if (Convert.ToInt32(text_size_combo.Text) <= 500)
                    {
                        Text.FontSize = double.Parse(text_size_combo.Text);
                    }
                    else // 500 초과
                    {
                        MessageBox.Show("글자 크기는 500을 넘을 수 없습니다.");
                        text_size_combo.Text = Text.FontSize.ToString();
                    }
                }
                else
                {
                    MessageBox.Show("글자 크기에는 숫자만 들어갈 수 있습니다.");
                    text_size_combo.Text = Text.FontSize.ToString();
                }
            }
        }
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

        private void text_theme_Btn_Click(object sender, RoutedEventArgs e)
        {
            switch (Config.Get("TextTheme"))
            {
                case "0":
                    Text.Background = Brushes.Black;
                    Text.Foreground = Brushes.White;
                    Config.Set("TextTheme", "1");
                    break;

                case "1":
                    Text.Background = Brushes.ForestGreen;
                    Text.Foreground = Brushes.White;
                    Config.Set("TextTheme", "2");
                    break;

                case "2":
                    Text.Background = Brushes.White;
                    Text.Foreground = Brushes.Black;
                    Config.Set("TextTheme", "0");
                    break;
            }
        }

        private void text_delete_Btn_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("텍스트를 모두 삭제하시겠습니까? 복구할 수 없습니다!", "텍스트 모두 삭제", MessageBoxButton.YesNo);
            switch (result)
            {
                case MessageBoxResult.Yes:
                    Text.Text = null;
                    break;
                case MessageBoxResult.No:
                    break;
            }
        }
        private void TextBox_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                foreach (var file in files)
                {
                    var ext = Path.GetExtension(file);
                    if (ext.Equals(".txt", StringComparison.CurrentCultureIgnoreCase) || (Config.Get("TextDragAllExtension") == "true"))
                    {
                        if (files != null && files.Length > 0)
                        {
                            Text.Text = File.ReadAllText(files[0]);
                        }
                    }
                    else    // .txt 확장자가 아니면서 옵션이 활성화되지 않았을 경우
                    {
                        MessageBox.Show("파일의 확장자가 .txt가 아닙니다. \n계속 로드하려면 설정에서 \"모든 확장자 사용\"을 활성화해주세요.", "확장자 주의!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }
                }
            }
        }

        private void TextBox_PreviewDragOver(object sender, DragEventArgs e)
        {
            e.Handled = true;
        }
        #endregion

        #region 계산기
        decimal Result_Value;
        string Operator_Performed = string.Empty;
        bool reset20 = false;
        bool PerformedOp;

        private void Cal_num(object sender, RoutedEventArgs e)
        {
            if (txtResult.Text == "0" || PerformedOp)
            {
                txtResult.Clear();
            }
            PerformedOp = false;
            Button button = (Button)sender;
            txtResult.Text += button.Content;
            reset20 = false;
            txtResult.Focus();
            if (num1 != string.Empty)
            {
                num2 = txtResult.Text;
            }
        }

        private void Cal_dot(object sender, RoutedEventArgs e)
        {
            // point
            Button button = (Button)sender;
            if (!txtResult.Text.Contains("."))
            {
                txtResult.Text += button.Content;
            }
            reset20 = false;
        }

        private string num1 = string.Empty;

        private void Cal_op(object sender, RoutedEventArgs e)
        {
            // +, -, *, / operators
            num1 = txtResult.Text;
            Button button = (Button)sender;
            if (!PerformedOp)
            {
                if (Result_Value != 0)
                {
                    Cal_equal_btn.RaiseEvent(new RoutedEventArgs(System.Windows.Controls.Primitives.ButtonBase.ClickEvent));
                    Operator_Performed = button.Content.ToString();
                    Label_log.Content = Result_Value + " " + Operator_Performed;
                    PerformedOp = true;
                }
                else
                {
                    Operator_Performed = button.Content.ToString();
                    Result_Value = decimal.Parse(txtResult.Text);
                    Label_log.Content = Result_Value + " " + Operator_Performed;
                    PerformedOp = true;
                }
            }
            reset20 = false;
        }

        private void Cal_CE(object sender, RoutedEventArgs e)
        {
            //CLEAR ENTRY BUTTON
            txtResult.Text = "0";
            reset20 = false;
        }

        private void Cal_C(object sender, RoutedEventArgs e)
        {
            //CLEAR BUTTON
            txtResult.Text = "0";
            Result_Value = 0;
            Operator_Performed = " ";
            Label_log.Content = " ";
            PerformedOp = false;
            reset20 = false;
        }

        private string num2;

        private void Cal_equal(object sender, RoutedEventArgs e)
        {
            // EQUALS BUTTON
            if (!PerformedOp)
            {
                switch (Operator_Performed)
                {
                    case "+":
                        txtResult.Text = (Result_Value + decimal.Parse(txtResult.Text)).ToString();
                        if (!reset20)
                            Cal_log_Add(Label_log.Content.ToString(), num2, txtResult.Text);
                        break;

                    case "-":
                        txtResult.Text = (Result_Value - decimal.Parse(txtResult.Text)).ToString();
                        if (!reset20)
                            Cal_log_Add(Label_log.Content.ToString(), num2, txtResult.Text);
                        break;

                    case "×":
                        txtResult.Text = (Result_Value * decimal.Parse(txtResult.Text)).ToString();
                        if (!reset20)
                            Cal_log_Add(Label_log.Content.ToString(), num2, txtResult.Text);
                        break;

                    case "÷":
                        try
                        {
                            txtResult.Text = (Result_Value / decimal.Parse(txtResult.Text)).ToString();
                            if (!reset20)
                                Cal_log_Add(Label_log.Content.ToString(), num2, txtResult.Text);
                        }
                        catch (Exception m)
                        {
                            MessageBox.Show("0으로 나눴습니다." + m.Message);
                        }
                        break;

                    default:
                        break;

                }
                Result_Value = decimal.Parse(txtResult.Text);
                Label_log.Content = " ";
                PerformedOp = false;
                Operator_Performed = " ";
                reset20 = false;
            }
            else
            {
                MessageBox.Show("연산자 뒤에 = 이 올 수 없습니다.");
            }
        }

        private void Cal_plusminus(object sender, RoutedEventArgs e)
        {
            double v = double.Parse(txtResult.Text);
            txtResult.Text = (-v).ToString();
            reset20 = false;
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
                case Key.F5:
                    Cal_equal_btn.RaiseEvent(new RoutedEventArgs(System.Windows.Controls.Primitives.ButtonBase.ClickEvent));
                    break;

                case Key.Enter:
                    Cal_equal_btn.RaiseEvent(new RoutedEventArgs(System.Windows.Controls.Primitives.ButtonBase.ClickEvent));
                    Keyboard.Focus(Label_log);
                    break;

                case Key.F6:
                    CalC.RaiseEvent(new RoutedEventArgs(System.Windows.Controls.Primitives.ButtonBase.ClickEvent));
                    break;

                case Key.Back:
                    CalBack.RaiseEvent(new RoutedEventArgs(System.Windows.Controls.Primitives.ButtonBase.ClickEvent));
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
            reset20 = false;
        }

        private void txtResult_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtResult.Text.Length > 20)
            {
                MessageBox.Show("[ERROR] 계산값이 20자리를 초과하였습니다.");
                reset20 = true;
                Result_Value = 0;
                txtResult.Text = "0";
                Operator_Performed = " ";
                Label_log.Content = " ";
                PerformedOp = false;
            }
        }
        private void Cal_log_Add(string label_log, string num2, string result)
        {
            Cal_log.Items.Add(label_log + " " + num2 + " " + "=" + " " + result);
        }
        private void Cal_log_remove_Click(object sender, RoutedEventArgs e)
        {
            Cal_log.Items.Clear();
            Config.Set("CalculatorLog", string.Empty);
        }

        private void Cal_log_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (Cal_log.SelectedIndex != -1)
            {
                string loadlog = Cal_log.Items.GetItemAt(Cal_log.SelectedIndex).ToString();
                string loadresult = loadlog.Substring(loadlog.IndexOf("=") + 2);
                txtResult.Text = loadresult;
            }
        }

        private void Cal_log_export_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Text file (*.txt)|*.txt";
            if (saveFileDialog.ShowDialog() == true)
            {
                using (var sw = new StreamWriter(saveFileDialog.FileName, false))
                    foreach (var item in Cal_log.Items)
                        sw.Write(item.ToString() + Environment.NewLine);
            }
        }
        #endregion

        #region 그림판
        private void Canvas_load_btn_Click(object sender, RoutedEventArgs e)
        {
            ImageLoad(0);
        }

        private void ImageLoad(int mode) // mode 0 = 일반 로드, mode 1 = 자동 로드
        {
            if (mode == 0)
            {
                inkCanvas.Strokes.Clear();
                OpenFileDialog openDialog = new OpenFileDialog();
                openDialog.Filter = "Image Files(*.PNG; *.JPG; *.GIF; *.BMP;)| *.PNG; *.JPG; *.GIF; *.BMP";
                if (openDialog.ShowDialog() == true)
                {
                    if (File.Exists(openDialog.FileName))
                    {
                        BitmapImage bitmapImage = new BitmapImage();
                        bitmapImage.BeginInit();
                        bitmapImage.CacheOption = BitmapCacheOption.None;
                        bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                        bitmapImage.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                        bitmapImage.UriSource = new Uri(openDialog.FileName, UriKind.RelativeOrAbsolute);
                        // InkCanvas의 배경으로 지정
                        inkCanvas.Background = new ImageBrush(bitmapImage);
                        bitmapImage.EndInit();
                    }
                }
                Config.Set("CanvasLastPath", openDialog.FileName);
            }
            else // mode 1
            {
                if (File.Exists(Config.Get("CanvasLastPath")))
                {
                    BitmapImage bitmapImage = new BitmapImage();
                    bitmapImage.BeginInit();
                    bitmapImage.CacheOption = BitmapCacheOption.None;
                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImage.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                    bitmapImage.UriSource = new Uri(Config.Get("CanvasLastPath"), UriKind.RelativeOrAbsolute);
                    // InkCanvas의 배경으로 지정
                    inkCanvas.Background = new ImageBrush(bitmapImage);
                    bitmapImage.EndInit();
                }
                else
                {
                    MessageBox.Show("그림판 자동 로드에 실패하였습니다.", "파일 존재 오류");
                    Config.Set("CanvasLastPath", "");
                }
            }
            isCanvasmod = true;
        }

        private byte[] Pixels = new byte[4];

        private bool isCanvasmod = false;

        // 이미지 캡쳐
        private void Canvas_save_btn_Click(object sender, RoutedEventArgs e)
        {
            RenderTargetBitmap bitmap = ConverterBitmapImage(inkCanvas);
            ImageSave(bitmap, 0);
        }

        // 해당 객체를 이미지로 변환
        private static RenderTargetBitmap ConverterBitmapImage(FrameworkElement element)
        {
            DrawingVisual drawingVisual = new DrawingVisual();
            DrawingContext drawingContext = drawingVisual.RenderOpen();

            // 해당 객체의 그래픽요소로 사각형의 그림을 그립니다.
            drawingContext.DrawRectangle(new VisualBrush(element), null,
                new Rect(new Point(0, 0), new Point(element.ActualWidth, element.ActualHeight)));

            drawingContext.Close();

            // 비트맵으로 변환합니다.
            RenderTargetBitmap target = new RenderTargetBitmap((int)element.ActualWidth, (int)element.ActualHeight, 96, 96, PixelFormats.Pbgra32);
            target.Render(drawingVisual);
            return target;
        }

        // 해당 이미지 저장
        private static void ImageSave(BitmapSource source, int mode) //mode 0 = 일반 저장, mode 1 = 파일에 자동 저장, mode 2 = 자동 저장 파일 생성
        {
            if (source != null)
            {
                if (mode == 0)
                {
                    SaveFileDialog saveDialog = new SaveFileDialog();

                    // 이미지 포맷들
                    saveDialog.Filter = "PNG|*.png|JPG|*.jpg|GIF|*.gif|BMP|*.bmp";
                    saveDialog.AddExtension = true;

                    if (saveDialog.ShowDialog() == true)
                    {
                        BitmapEncoder encoder = null;
                        // 파일 생성
                        FileStream stream = new FileStream(saveDialog.FileName, FileMode.Create, FileAccess.Write);

                        // 파일 포맷
                        string upper = saveDialog.SafeFileName.ToUpper();
                        char[] format = upper.ToCharArray(saveDialog.SafeFileName.Length - 3, 3);
                        upper = new string(format);

                        // 해당 포맷에 맞게 인코더 생성
                        switch (upper.ToString())
                        {
                            case "PNG":
                                encoder = new PngBitmapEncoder();
                                break;

                            case "JPG":
                                encoder = new JpegBitmapEncoder();
                                break;

                            case "GIF":
                                encoder = new GifBitmapEncoder();
                                break;

                            case "BMP":
                                encoder = new BmpBitmapEncoder();
                                break;
                        }

                        // 인코더 프레임에 이미지 추가
                        encoder.Frames.Add(BitmapFrame.Create(source));
                        // 파일에 저장
                        encoder.Save(stream);

                        stream.Close();

                        Config.Set("CanvasLastPath", saveDialog.FileName);
                    }
                }
                else if (mode == 1)
                {
                    BitmapEncoder encoder = null;
                    // 파일 생성
                    if (File.Exists(Config.Get("CanvasLastPath")))
                    {
                        FileStream stream = new FileStream(Config.Get("CanvasLastPath"), FileMode.Create, FileAccess.Write);

                        string upper = Config.Get("CanvasLastPath").ToUpper();
                        char[] format = upper.ToCharArray(Config.Get("CanvasLastPath").Length - 3, 3);
                        upper = new string(format);

                        switch (upper.ToString())
                        {
                            case "PNG":
                                encoder = new PngBitmapEncoder();
                                break;

                            case "JPG":
                                encoder = new JpegBitmapEncoder();
                                break;

                            case "GIF":
                                encoder = new GifBitmapEncoder();
                                break;

                            case "BMP":
                                encoder = new BmpBitmapEncoder();
                                break;

                            default:
                                MessageBox.Show("'파일에 자동 저장'을 사용하려면 파일 확장자가 .PNG, .JPG, .GIF, .BMP 중 하나여야 합니다. \n문서\\Progretter\\AutoSave폴더에 자동 저장됩니다.", "확장자 오류");
                                ImageSave(source, 2);
                                break;
                        }

                        // 인코더 프레임에 이미지 추가
                        encoder.Frames.Add(BitmapFrame.Create(source));
                        // 파일에 저장
                        encoder.Save(stream);

                        stream.Close();
                    }
                    else
                    {
                        MessageBox.Show("'파일에 자동 저장'을 사용하려면 파일이 존재해야 합니다. \n문서\\Progretter\\AutoSave폴더에 자동 저장됩니다.", "파일 존재 오류");
                        Config.Set("CanvasLastPath", "");
                        ImageSave(source, 2);
                    }
                }
                else // mode == 2
                {
                    DirectoryInfo di = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Progretter\AutoSave");

                    if (!di.Exists)   //If New Folder not exits  
                    {
                        di.Create();             //create Folder
                    }

                    string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/Progretter/AutoSave/" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".jpg"; // AutoSave/가 맞음 (AutoSave\ X)
                                                                                                                                                                                 // 파일 생성
                    FileStream stream = new FileStream(path, FileMode.Create, FileAccess.Write);

                    BitmapEncoder encoder = new JpegBitmapEncoder();

                    // 인코더 프레임에 이미지 추가
                    encoder.Frames.Add(BitmapFrame.Create(source));
                    // 파일에 저장
                    encoder.Save(stream);

                    stream.Close();

                    Config.Set("CanvasLastPath", path);
                }
            }
        }

        // 픽셀 값 얻어오기
        public Color GetPixelColor(Point CurrentPoint)
        {
            BitmapSource CurrentSource = colorsImage.Source as BitmapSource;

            // 비트맵 내의 좌표 값 계산
            CurrentPoint.X *= CurrentSource.PixelWidth / colorsImage.ActualWidth;
            CurrentPoint.Y *= CurrentSource.PixelHeight / colorsImage.ActualHeight;

            if (CurrentSource.Format == PixelFormats.Bgra32 || CurrentSource.Format == PixelFormats.Bgr32)
            {
                // 32bit stride = (width * bpp + 7) /8
                int Stride = (CurrentSource.PixelWidth * CurrentSource.Format.BitsPerPixel + 7) / 8;
                // 한 픽셀 복사
                CurrentSource.CopyPixels(new Int32Rect((int)CurrentPoint.X, (int)CurrentPoint.Y, 1, 1), Pixels, Stride, 0);

                // 컬러로 변환 후 리턴
                return Color.FromArgb(Pixels[3], Pixels[2], Pixels[1], Pixels[0]);
            }
            else
            {
                MessageBox.Show("지원되지 않는 포맷형식");
            }

            return Color.FromArgb(Pixels[3], Pixels[2], Pixels[1], Pixels[0]);
        }

        private void Canvas_brush_property_Click(object sender, RoutedEventArgs e)
        {
            foreach (Window openForm in Application.Current.Windows)
            {
                if (openForm.Title == "그림판 속성") // 열린 창의 이름 검사
                {
                    if (openForm.WindowState == WindowState.Minimized)
                    {  // 창을 최소화시켜 하단에 내려놓았는지 검사
                        openForm.WindowState = WindowState.Normal;
                    }
                    openForm.Activate();
                    return;
                }
            }
            CanvasProperty canvasProperty = new CanvasProperty();
            canvasProperty.CPEvent += ColorChange;
            canvasProperty.EMEvent += StrokeEditingModeChange;
            canvasProperty.SSEvent += StrokeSizeChange;
            canvasProperty.ESEvent += EraseSizeChange;
            canvasProperty.RecEditingMode(inkCanvas.EditingMode.ToString());
            canvasProperty.colorpicker_ColorChange(inkCanvas.DefaultDrawingAttributes.Color.A, inkCanvas.DefaultDrawingAttributes.Color.R, inkCanvas.DefaultDrawingAttributes.Color.G, inkCanvas.DefaultDrawingAttributes.Color.B);
            canvasProperty.Show();
        }

        private void ColorChange(byte A, byte R, byte G, byte B)
        {
            inkCanvas.DefaultDrawingAttributes.Color = Color.FromArgb(A, R, G, B);
        }

        private void StrokeEditingModeChange(int mode)
        {
            switch (mode)
            {
                case 0:
                    inkCanvas.EditingMode = InkCanvasEditingMode.Ink;
                    break;

                case 2:
                    inkCanvas.EditingMode = InkCanvasEditingMode.GestureOnly;
                    break;

                case 3:
                    inkCanvas.EditingMode = InkCanvasEditingMode.Select;
                    break;

                default:
                    break;
            }
        }

        private void StrokeSizeChange(double size)
        {
            inkCanvas.DefaultDrawingAttributes.Width = size;
            inkCanvas.DefaultDrawingAttributes.Height = size;
        }

        private void EraseSizeChange(int mode, double size)
        {
            switch (mode)
            {
                case 0:
                    inkCanvas.EditingMode = InkCanvasEditingMode.None;
                    inkCanvas.EraserShape = new RectangleStylusShape(size, size); // size Default value = 8
                    inkCanvas.EditingMode = InkCanvasEditingMode.EraseByPoint;
                    break;

                case 1:
                    inkCanvas.EditingMode = InkCanvasEditingMode.None;
                    inkCanvas.EraserShape = new EllipseStylusShape(size, size);
                    inkCanvas.EditingMode = InkCanvasEditingMode.EraseByPoint;
                    break;

                case 2:
                    inkCanvas.EditingMode = InkCanvasEditingMode.None;
                    inkCanvas.EraserShape = new RectangleStylusShape(8, 8);
                    inkCanvas.EditingMode = InkCanvasEditingMode.EraseByStroke;
                    break;

                default:
                    break;
            }
        }

        // 잉크 색상 변경
        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Point point = e.GetPosition(sender as Image);
            inkCanvas.DefaultDrawingAttributes.Color = GetPixelColor(point);
        }

        private void Canvas_clear_btn_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("그림판을 비우시겠습니까?", "그림판 비우기", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                inkCanvas.Strokes.Clear();
                inkCanvas.Background = Brushes.White; //배경도 지우기
                isCanvasmod = false;
                Config.Set("CanvasLastPath", "");
            }
        }
        #endregion

        #region Function
        private void App_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.F11:
                    WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
                    break;

                case Key.Up:
                    if (tabcontrol.SelectedIndex != 0)
                        tabcontrol.SelectedIndex -= 1;
                    break;
                case Key.Down:
                    if (tabcontrol.SelectedIndex != 4)
                        tabcontrol.SelectedIndex += 1;
                    break;
            }
        }

        private void AutoUpdaterOnCheckForUpdateEvent(UpdateInfoEventArgs args)
        {
            if (args.Error == null)
            {
                if (args.IsUpdateAvailable)
                {
                    Notification("온라인 클래스 도우미 업데이트", "업데이트가 있습니다.", 9900, 3, 0, 0, 0); // Update Check
                }
                else
                {
                    if (NoUpdate_Noti)
                    {
                        Notification("온라인 클래스 도우미 업데이트", "현재 업데이트가 없습니다.", 9000, 0, 3, 0, 0);
                        NoUpdate_Noti = false;
                    }
                }
            }
        }

        public void Notification(string title, string contents, int notitag, int day, int hour, int min, int sec)
        {
            TimeSpan time = new TimeSpan(day, hour, min, sec);
            // Requires Microsoft.Toolkit.Uwp.Notifications NuGet package version 7.0 or greater
            if (notitag == 9100)
            {
                var builder = new ToastContentBuilder()
                    .AddArgument("action", "NotiTag")
                    .AddArgument("NotiTag", notitag)
                    .AddText(title)
                    .AddText(contents)
                    .SetToastScenario(ToastScenario.Alarm)
                    .AddButton(new ToastButton()
                        .SetContent("중지")
                        .AddArgument("dismiss", "true"))
                    .AddButton(new ToastButton()
                        .SetContent("다시 알림")
                        .AddArgument("again", "true"));
                builder.Schedule(DateTimeOffset.Now.LocalDateTime.Add(time));
            }
            else
            {
                new ToastContentBuilder()
                .AddArgument("action", "NotiTag")
                .AddArgument("NotiTag", notitag)
                .AddText(title)
                .AddText(contents)
                .Show(toast =>
                {
                    toast.ExpirationTime = DateTime.Now.Add(time);
                });// Not seeing the Show() method? Make sure you have version 7.0, and if you're using .NET 6 (or later), then your TFM must be net6.0-windows10.0.17763.0 or greater
            }
        }
        #endregion
    }
}