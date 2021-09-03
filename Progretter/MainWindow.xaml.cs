using Microsoft.Toolkit.Uwp.Notifications;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Windows;
using Excel = Microsoft.Office.Interop.Excel;
using System.Windows.Controls;
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
            foreach (var item in Config.Get("CalculatorLog").ToString().Split(new char[] { ',' }))
            {
                if (!string.IsNullOrEmpty(item))
                    Cal_log.Items.Add(item);
            }

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
        }
        #endregion

        #region Window_Closed
        private void Window_Closed(object sender, EventArgs e)
        {
            Config.Set("CalculatorLog", string.Empty);
            foreach (var item in Cal_log.Items)
            {
                Config.Add("CalculatorLog", (string)item);
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
        #endregion

        #region 시간표
        private void Schedule_import_Click(object sender, RoutedEventArgs e)
        {
            ExportToExcelAndCsv();
        }

        private void ExportToExcelAndCsv()
        {
            Schedule.SelectAllCells();
            Schedule.ClipboardCopyMode = DataGridClipboardCopyMode.IncludeHeader;
            ApplicationCommands.Copy.Execute(null, Schedule);
            String resultat = (string)Clipboard.GetData(DataFormats.CommaSeparatedValue);
            String result = (string)Clipboard.GetData(DataFormats.Text);
            Schedule.UnselectAllCells();
            System.IO.StreamWriter file1 = new System.IO.StreamWriter(@"D:\test.xls");
            file1.WriteLine(result.Replace(',', ' '));
            file1.Close();

            MessageBox.Show(" Exporting DataGrid data to Excel file created.xls");
        }
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
        #endregion

        #region 그림판
        private void Canvas_load_btn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openDialog = new OpenFileDialog();
            if (openDialog.ShowDialog() == true)
            {
                if (File.Exists(openDialog.FileName))
                {
                    BitmapImage bitmapImage = new BitmapImage(new Uri(openDialog.FileName, UriKind.RelativeOrAbsolute));
                    // InkCanvas의 배경으로 지정
                    inkCanvas.Background = new ImageBrush(bitmapImage);
                }
            }
        }

        private byte[] Pixels = new byte[4];

        // 이미지 캡쳐
        private void Canvas_save_btn_Click(object sender, RoutedEventArgs e)
        {
            RenderTargetBitmap bitmap = ConverterBitmapImage(inkCanvas);
            ImageSave(bitmap);
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
        private static void ImageSave(BitmapSource source)
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
            canvasProperty.Show();
        }


        /*        // 지우기 모드
                private void btn_Erase(object sender, RoutedEventArgs e)
                {
                    inkCanvas.EditingMode = InkCanvasEditingMode.EraseByPoint;
                }

                // 잉크 모드
                private void btn_Pen(object sender, RoutedEventArgs e)
                {
                    inkCanvas.EditingMode = InkCanvasEditingMode.Ink;
                }*/

        // 잉크 색상 변경
        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Point point = e.GetPosition(sender as Image);

            inkCanvas.DefaultDrawingAttributes.Color = GetPixelColor(point);
        }

        private void Canvas_clear_btn_Click(object sender, RoutedEventArgs e)
        {
            inkCanvas.Strokes.Clear();
            inkCanvas.Background = Brushes.White; //배경도 지우기
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