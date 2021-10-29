using Microsoft.Toolkit.Uwp.Notifications;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
/*using Excel = Microsoft.Office.Interop.Excel;*/
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
                /*switch (DateTime.Now.Hour) //현재 속한 교시 + 그에 맞는 알림
                {
                    case period && rest5min:
                        break;
                }
                Notification("시간 변경 알림", "지금은 수학시간 5분 전 입니다.");*/
            }
        }

        private void Notification(string Title, string Contents)
        {
            // Requires Microsoft.Toolkit.Uwp.Notifications NuGet package version 7.0 or greater
            new ToastContentBuilder()
                .AddArgument("action", "viewConversation")
                .AddArgument("conversationId", 9813)
                .AddText($"{Title}")
                .AddText($"{Contents}")
                .Show(); // Not seeing the Show() method? Make sure you have version 7.0, and if you're using .NET 5, your TFM must be net5.0-windows10.0.17763.0 or greater
        }
        #endregion

        #region Window_Loaded

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Setting_Schedule_StartUp_Label.Content = Config.Get("ScheduleStartUpPath");
            if (Config.Get("ScheduleStartUpImport") == "true")
            {
                Schedule.ItemsSource = Schedules.ImportExcel(Config.Get("ScheduleStartUpPath")).DefaultView;
            }

            if (Config.Get("ScheduleCloseSave") == "true")
            {
                Setting_Schedule_Close_Save_CheckBox.IsChecked = true;
            }

            foreach (var item in Config.Get("CalculatorLog").ToString().Split(new char[] { ',' }))
            {
                if (!string.IsNullOrEmpty(item))
                    Cal_log.Items.Add(item);
            }

            if (Config.Get("ScheduleIsCheckBox") == "true")
            {

            }

            if (Config.Get("TextTheme") == "1")
            {
                Text.Background = Brushes.Black;
                Text.Foreground = Brushes.White;
            }

            if (Config.Get("CalculatorDeleteLog") == "true")
            {
                Setting_Cal_DeleteLog_CheckBox.IsChecked = true;
            }

            if (Config.Get("CanvasAutoLoad") == "true")
            {
                Setting_Canvas_Autoload_Checkbox.IsChecked = true;
            }

            if (Config.Get("CanvasAutoSave") == "true")
            {
                Setting_Canvas_Autosave_Checkbox.IsChecked = true;
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
        }
        #endregion

        #region Window_Closed
        private void Window_Closed(object sender, EventArgs e)
        {
            if(Config.Get("ScheduleCloseSave") == "true")
            {
                if(Schedule.ItemsSource != null)
                {
                    string path = @"AutoSave\" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
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

            if (Config.Get("CanvasAutoSave") == "true")
            {
                if(Config.Get("CanvasLastAdress") != "")
                {
                    RenderTargetBitmap bitmap = ConverterBitmapImage(inkCanvas);
                    ImageSave(bitmap, 1);
                }
                else
                {
                    RenderTargetBitmap bitmap = ConverterBitmapImage(inkCanvas);
                    ImageSave(bitmap, 2);
                }
            }
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

        private void Setting_Schedule_StartUp_Btn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Excel File(*.xlsx)|*.xlsx|Excel 97-2003 File(*.xls)|*.xls|Csv File(*.csv)|*.csv";
            if (openFileDialog.ShowDialog() == true)
            {
                Setting_Schedule_StartUp_Label.Content = openFileDialog.FileName;
                Config.Set("ScheduleStartUpImport", "true");
                Config.Set("ScheduleStartUpPath", Setting_Schedule_StartUp_Label.Content.ToString());
            }
        }

        private void Setting_Schedule_StartUp_Reset_Btn_Click(object sender, RoutedEventArgs e)
        {
            Setting_Schedule_StartUp_Label.Content = null;
            Config.Set("ScheduleStartUpImport", "false");
            Config.Set("ScheduleStartUpPath", "");
        }

        private void Setting_Schedule_Close_Save_CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            Config.Set("ScheduleCloseSave", "true");
        }

        private void Setting_Schedule_Close_Save_CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            Config.Set("ScheduleCloseSave", "false");
        }

        private void Setting_Cal_DeleteLog_CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            Config.Set("CalculatorDeleteLog", "true");
        }

        private void Setting_Cal_DeleteLog_CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            Config.Set("CalculatorDeleteLog", "false");
        }
        private void Setting_Canvas_Autosave_Checkbox_Checked(object sender, RoutedEventArgs e)
        {
            Config.Set("CanvasAutoSave", "true");
        }

        private void Setting_Canvas_Autosave_Checkbox_Unchecked(object sender, RoutedEventArgs e)
        {
            Config.Set("CanvasAutoSave", "false");
        }

        private void Setting_Canvas_Autoload_Checkbox_Checked(object sender, RoutedEventArgs e)
        {
            Config.Set("CanvasAutoLoad", "true");
        }

        private void Setting_Canvas_Autoload_Checkbox_Unchecked(object sender, RoutedEventArgs e)
        {
            Config.Set("CanvasAutoLoad", "false");
        }
        private void Setting_Alarm_Chk_Checked(object sender, RoutedEventArgs e)
        {
            Setting_Alarm_Grid.Visibility = Visibility.Visible;
        }

        private void Setting_Alarm_Chk_Unchecked(object sender, RoutedEventArgs e)
        {
            Setting_Alarm_Grid.Visibility = Visibility.Collapsed;
        }
        #endregion

        #region 시간표
        private void Schedule_CellColor_Combo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void ImportExcel(object sender, RoutedEventArgs e)
        {
        OpenFileDialog openFileDialog = new OpenFileDialog();
        openFileDialog.Filter = "Excel File(*.xlsx)|*.xlsx|Excel 97-2003 File(*.xls)|*.xls|Csv File(*.csv)|*.csv";
            if (openFileDialog.ShowDialog() == true)
            {
                Schedule.ItemsSource = Schedules.ImportExcel(openFileDialog.FileName).DefaultView;
            }

                // 여기는 Interop 엑셀 있어야만 가능한 코드 (원래 코드 잘 작동함)
                /*                var path = openFileDialog.FileName;

                                Excel.Application xlApp;
                                Excel.Workbook xlWorkBook;
                                Excel.Worksheet xlWorkSheet;
                                Excel.Range range;

                                string str;
                                int rCnt = 0;
                                int cCnt = 0;
                                string sCellData = "";
                                double dCellData;

                                xlApp = new Excel.Application();

                                try
                                {
                                    xlWorkBook = xlApp.Workbooks.Open(path, 0, true, 5, "", "", true, Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);
                                    xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);

                                    range = xlWorkSheet.UsedRange;

                                    DataTable dt = new DataTable();

                                    // 첫 행을 제목으로
                                    for (cCnt = 1; cCnt <= range.Columns.Count; cCnt++)
                                    {
                                        str = (string)(range.Cells[1, cCnt] as Excel.Range).Value2;
                                        dt.Columns.Add(str, typeof(string));
                                    }

                                    for (rCnt = 2; rCnt <= range.Rows.Count; rCnt++)
                                    {
                                        string sData = "";
                                        for (cCnt = 1; cCnt <= range.Columns.Count; cCnt++)
                                        {
                                            try
                                            {
                                                sCellData = (string)(range.Cells[rCnt, cCnt] as Excel.Range).Value2;
                                                sData += sCellData + "|";
                                            }
                                            catch (Exception ex)
                                            {
                                                dCellData = (double)(range.Cells[rCnt, cCnt] as Excel.Range).Value2;
                                                sData += dCellData.ToString() + "|";
                                            }
                                        }
                                        sData = sData.Remove(sData.Length - 1, 1);
                                        dt.Rows.Add(sData.Split('|'));
                                    }

                                    Schedule.ItemsSource = dt.DefaultView;

                                    xlWorkBook.Close(true, null, null);
                                    xlApp.Quit();

                                    releaseObject(xlWorkSheet);
                                    releaseObject(xlWorkBook);
                                    releaseObject(xlApp);
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show("파일 열기 실패! : " + ex.Message);
                                    return;
                                }*/
        }


/*        private void releaseObject(object obj) // Interop
        {
            try
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
                obj = null;
            }
            catch (Exception ex)
            {
                obj = null;
                MessageBox.Show("Unable to release the Object " + ex.ToString());
            }
            finally
            {
                GC.Collect();
            }
        }*/


        private void ExportToExcel(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Excel file (*.xlsx)|*.xlsx|Csv File(*.csv)|*.csv";
            if (saveFileDialog.ShowDialog() == true)
            {
                Schedules.ExportExcel((DataView)Schedule.ItemsSource, saveFileDialog.FileName);

                // 여기는 Interop 엑셀 있어야만 가능한 코드 (원래 코드 잘 작동함)
                /*                string path = saveFileDialog.FileName;
                                try
                                {
                                    var excelApp = new Excel.Application();
                                    excelApp.Workbooks.Add();
                                    Excel._Worksheet workSheet = (Excel._Worksheet)excelApp.ActiveSheet;
                                    for (var i = 0; i < dt.Columns.Count; i++)
                                    {
                                        workSheet.Cells[1, i + 1] = dt.Columns[i].ColumnName;
                                    }
                                    for (var i = 0; i < dt.Rows.Count; i++)
                                    {
                                        for (var j = 0; j < dt.Columns.Count; j++)
                                        {
                                            workSheet.Cells[i + 2, j + 1] = dt.Rows[i][j];
                                        }
                                    }
                                    if (File.Exists(path))
                                    {
                                        File.Delete(path);
                                    }
                                    workSheet.SaveAs2(path);
                                    excelApp.Quit();
                                }
                                catch (Exception ex)
                                {

                                }*/
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
            DataTable dt = new DataTable();
            if (Schedule.ItemsSource != null)
                dt = ((DataView)Schedule.ItemsSource).ToTable();
            dt.Rows.RemoveAt(dt.Rows.Count - 1);
            Schedule.ItemsSource = dt.DefaultView;
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
            DataTable dt = new DataTable();
            if (Schedule.ItemsSource != null)
                dt = ((DataView)Schedule.ItemsSource).ToTable();
            dt.Columns.RemoveAt(dt.Columns.Count - 1);
            Schedule.ItemsSource = dt.DefaultView;
        }

        private void Schedule_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e) // 테스트 용 실사 불가능
        {
            if (Schedule.SelectedIndex != -1)
            {
                row.Content = Schedule.SelectedIndex.ToString();
            }
            else
            {
                if (Convert.ToInt32(row.Content) - 1 < 0)
                {
                    row.Content = 0;
                }
                else
                {
                    row.Content = (Convert.ToInt32(row.Content) - 1).ToString();
                }
            }
            if (Schedule.CurrentColumn != null)
            {
                column.Content = Schedule.CurrentColumn.DisplayIndex.ToString();
            }
            else
            {

            }
            if (Convert.ToInt32(column.Content) - 1 < 0)
            {
                column.Content = 0;
            }
            else
            {
                column.Content = (Convert.ToInt32(column.Content) - 1).ToString();
            }
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
            if (Config.Get("TextTheme") == "0")
            {
                Text.Background = Brushes.Black;
                Text.Foreground = Brushes.White;
                Config.Set("TextTheme", "1");
            }
            else
            {
                Text.Background = Brushes.White;
                Text.Foreground = Brushes.Black;
                Config.Set("TextTheme", "0");
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
        #endregion

        #region 계산기
        decimal Result_Value;
        string Operator_Performed = " ";
        bool reset20 = false;
        bool PerformedOp;

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
            reset20 = false;
            txtResult.Focus();
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

        private string num1;

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
                num2 = txtResult.Text;
                switch (Operator_Performed)
                {
                    case "+":
                        txtResult.Text = (Result_Value + decimal.Parse(txtResult.Text)).ToString();
                        if (!reset20)
                            Cal_log_Add(num1, Operator_Performed, num2, txtResult.Text);
                        break;

                    case "-":
                        txtResult.Text = (Result_Value - decimal.Parse(txtResult.Text)).ToString();
                        if (!reset20)
                            Cal_log_Add(num1, Operator_Performed, num2, txtResult.Text);
                        break;

                    case "×":
                        txtResult.Text = (Result_Value * decimal.Parse(txtResult.Text)).ToString();
                        if (!reset20)
                            Cal_log_Add(num1, Operator_Performed, num2, txtResult.Text);
                        break;

                    case "÷":
                        try
                        {
                            txtResult.Text = (Result_Value / decimal.Parse(txtResult.Text)).ToString();
                            if (!reset20)
                                Cal_log_Add(num1, Operator_Performed, num2, txtResult.Text);
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
        private void Cal_log_Add(string num1, string op, string num2, string result)
        {
            Cal_log.Items.Add(num1 + " " + op + " " + num2 + " " + "=" + " " + result);
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
            inkCanvas.Strokes.Clear();
            OpenFileDialog openDialog = new OpenFileDialog();
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
            Config.Set("CanvasLastAdress", openDialog.FileName);
        }

        private byte[] Pixels = new byte[4];

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
            RenderTargetBitmap target =
                new RenderTargetBitmap((int)element.ActualWidth, (int)element.ActualHeight,
                96, 96, PixelFormats.Pbgra32);

            target.Render(drawingVisual);
            return target;
        }

        // 해당 이미지 저장
        private static void ImageSave(BitmapSource source, int mode) //mode 0 = 일반 저장, mode 1 = 파일에 자동 저장, mode 2 = 자동 저장 파일 생성
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
                }
            }
            else if (mode == 1)
            {
                BitmapEncoder encoder = null;
                // 파일 생성
                FileStream stream = new FileStream(Config.Get("CanvasLastAdress"), FileMode.Create, FileAccess.Write);

                string upper = Config.Get("CanvasLastAdress").ToUpper();
                char[] format = upper.ToCharArray(Config.Get("CanvasLastAdress").Length - 3, 3);
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
                }

                // 인코더 프레임에 이미지 추가
                encoder.Frames.Add(BitmapFrame.Create(source));
                // 파일에 저장
                encoder.Save(stream);

                stream.Close();
            }
            else // mode == 2
            {
                // 파일 생성
                FileStream stream = new FileStream(@"AutoSave\" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".jpg", FileMode.Create, FileAccess.Write);

                BitmapEncoder encoder = new JpegBitmapEncoder();

                // 인코더 프레임에 이미지 추가
                encoder.Frames.Add(BitmapFrame.Create(source));
                // 파일에 저장
                encoder.Save(stream);

                stream.Close();
            }
        }

        // 픽셀 값 얻어오기
        private Color GetPixelColor(Point CurrentPoint)
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
            CanvasProperty canvasProperty = new CanvasProperty();
            canvasProperty.CPEvent += ColorChange;
            canvasProperty.EMEvent += StrokeEditingModeChange;
            canvasProperty.ESEvent += StrokeSizeChange;
            canvasProperty.RecEditingMode(inkCanvas.DefaultDrawingAttributes.Color.A, inkCanvas.DefaultDrawingAttributes.Color.R, inkCanvas.DefaultDrawingAttributes.Color.G, inkCanvas.DefaultDrawingAttributes.Color.B, inkCanvas.EditingMode.ToString());
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

                case 1:
                    inkCanvas.EditingMode = InkCanvasEditingMode.EraseByPoint;
                    break;

                case 2:
                    inkCanvas.EditingMode = InkCanvasEditingMode.EraseByStroke;
                    break;

                case 3:
                    inkCanvas.EditingMode = InkCanvasEditingMode.GestureOnly;
                    break;

                case 4:
                    inkCanvas.EditingMode = InkCanvasEditingMode.InkAndGesture;
                    break;

                case 5:
                    inkCanvas.EditingMode = InkCanvasEditingMode.Select;
                    break;
            }
        }

        private void StrokeSizeChange(double size)
        {
            inkCanvas.DefaultDrawingAttributes.Width = size;
            inkCanvas.DefaultDrawingAttributes.Height = size;
        }

        // 잉크 색상 변경
        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Point point = e.GetPosition(sender as Image);
            inkCanvas.DefaultDrawingAttributes.Color = GetPixelColor(point);
            StrokeSizeChange(11);
        }

        private void Canvas_clear_btn_Click(object sender, RoutedEventArgs e)
        {
            inkCanvas.Strokes.Clear();
            inkCanvas.Background = Brushes.White; //배경도 지우기
            Config.Set("CanvasLastAdress", "");
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