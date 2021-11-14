using System.Windows;
using System;

namespace Progretter
{
    /// <summary>
    /// CanvasProperty.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class CanvasProperty : Window
    {
        public CanvasProperty()
        {
            InitializeComponent();
            if (Config.Get("CanvasStrokeSlider") != "")
                StrokeSize.Value = Convert.ToDouble(Config.Get("CanvasStrokeSlider"));
            if (Config.Get("CanvasEraseSlider") != "")
                EraserSize.Value = Convert.ToDouble(Config.Get("CanvasEraseSlider"));
            this.Left = ((MainWindow)Application.Current.MainWindow).Left + 984;
            this.Top = ((MainWindow)Application.Current.MainWindow).Top;
            ((MainWindow)Application.Current.MainWindow).colorsImage.MouseDown += new System.Windows.Input.MouseButtonEventHandler(ImageMouseDown);
        }

        public delegate void ColorPropertyDataHandler(byte A, byte R, byte G, byte B);

        public event ColorPropertyDataHandler CPEvent;
        private byte Alpha = 255;
        private byte Red = 0;
        private byte Green = 0;
        private byte Blue = 0;

        public delegate void EditModeHandler(int mode);

        public event EditModeHandler EMEvent;
        private int editmode = 0;

        public delegate void StrokeSizeHandler(double size);

        public event StrokeSizeHandler SSEvent;
        private double slider_size = 1;

        public delegate void EraserSizeHandler(int mode, double size);

        public event EraserSizeHandler ESEvent;


        private void ImageMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Point point = e.GetPosition(sender as System.Windows.Controls.Image);
            colorpicker_ColorChange(((MainWindow)Application.Current.MainWindow).GetPixelColor(point).A, ((MainWindow)Application.Current.MainWindow).GetPixelColor(point).R, ((MainWindow)Application.Current.MainWindow).GetPixelColor(point).G, ((MainWindow)Application.Current.MainWindow).GetPixelColor(point).B);
        }

        private void colorpicker_ColorChanged(object sender, RoutedEventArgs e)
        {
            Alpha = colorpicker.SelectedColor.A;
            Red = colorpicker.SelectedColor.R;
            Green = colorpicker.SelectedColor.G;
            Blue = colorpicker.SelectedColor.B;

            var handler = CPEvent;
            if (null != handler)
                CPEvent(Alpha, Red, Green, Blue);
        }

        public void colorpicker_ColorChange(byte A, byte R, byte G, byte B)
        {
            colorpicker.Color.A = A;
            colorpicker.Color.RGB_R = R;
            colorpicker.Color.RGB_G = G;
            colorpicker.Color.RGB_B = B;
        }

        private int load = 0;
        private int erasermode = -1;

        private void StrokeEditingMode_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (StrokeEditingMode.SelectedIndex == 1)
            {
                if (Config.Get("CanvasEraseMode") == "")
                {
                    EraserMode.SelectedIndex = erasermode;
                }
                else
                {
                    EraserMode.SelectedIndex = Convert.ToInt32(Config.Get("CanvasEraseMode"));
                }
                StrokeSizeLabel.Visibility = Visibility.Collapsed;
                StrokeSize.Visibility = Visibility.Collapsed;
                CanvasCP.Visibility = Visibility.Collapsed;
                CanvasEraser.Visibility = Visibility.Visible;
            }
            else if (CanvasCP != null && CanvasEraser != null)
            {
                erasermode = EraserMode.SelectedIndex;
                EraserMode.SelectedIndex = -1;
                if (StrokeEditingMode.SelectedIndex == 0)
                {
                    StrokeSizeLabel.Visibility = Visibility.Visible;
                    StrokeSize.Visibility = Visibility.Visible;
                    CanvasCP.Visibility = Visibility.Visible;
                }
                else
                {
                    StrokeSizeLabel.Visibility = Visibility.Collapsed;
                    StrokeSize.Visibility = Visibility.Collapsed;
                    CanvasCP.Visibility = Visibility.Collapsed;
                }
                CanvasEraser.Visibility = Visibility.Collapsed;
            }

            if (load == 1)
            {
                editmode = StrokeEditingMode.SelectedIndex;
                var handler = EMEvent;
                if (null != handler)
                    EMEvent(editmode);
            }
            else
            {
                load = 1;
            }
        }

        public void RecEditingMode(byte A, byte R, byte G, byte B, string noweditmode) // 중간에
        {
            colorpicker.Color.A = A;
            colorpicker.Color.RGB_R = R;
            colorpicker.Color.RGB_G = G;
            colorpicker.Color.RGB_B = B;
            switch (noweditmode)
            {
                case "Ink":
                    StrokeEditingMode.SelectedIndex = 0;
                    if (Config.Get("CanvasEraseMode") != "")
                        erasermode = Convert.ToInt32(Config.Get("CanvasEraseMode"));
                    break;

                case "EraseByPoint":
                    StrokeSizeLabel.Visibility = Visibility.Collapsed;
                    StrokeSize.Visibility = Visibility.Collapsed;
                    CanvasCP.Visibility = Visibility.Collapsed;
                    CanvasEraser.Visibility = Visibility.Visible;
                    StrokeEditingMode.SelectedIndex = 1;
                    if (Config.Get("CanvasEraseMode") == "0") // 사각형
                        EraserMode.SelectedIndex = 0;
                    else
                        EraserMode.SelectedIndex = 1;
                    break;

                case "EraseByStroke":
                    StrokeSizeLabel.Visibility = Visibility.Collapsed;
                    StrokeSize.Visibility = Visibility.Collapsed;
                    CanvasCP.Visibility = Visibility.Collapsed;
                    CanvasEraser.Visibility = Visibility.Visible;
                    StrokeEditingMode.SelectedIndex = 1;
                    EraserMode.SelectedIndex = 2;
                    EraserSize.Visibility = Visibility.Collapsed;
                    EraserLabel.Visibility = Visibility.Collapsed;
                    break;

                case "GestureOnly":
                    StrokeEditingMode.SelectedIndex = 2;
                    if (Config.Get("CanvasEraseMode") != "")
                        erasermode = Convert.ToInt32(Config.Get("CanvasEraseMode"));
                    break;

                case "Select":
                    StrokeEditingMode.SelectedIndex = 3;
                    if (Config.Get("CanvasEraseMode") != "")
                        erasermode = Convert.ToInt32(Config.Get("CanvasEraseMode"));
                    break;
            }
        }

        private void StrokeSize_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            slider_size = StrokeSize.Value;
            var handler = SSEvent;
            if (null != handler)
            {
                SSEvent(slider_size);
                Config.Set("CanvasStrokeSlider", slider_size.ToString());
            }
        }

        private void EraserMode_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (EraserMode.SelectedIndex >= 0)
            {
                if (EraserMode.SelectedIndex != 2)
                {
                    EraserSize.Visibility = Visibility.Visible;
                    EraserLabel.Visibility = Visibility.Visible;
                    Config.Set("CanvasEraseMode", EraserMode.SelectedIndex.ToString());
                }
                else
                {
                    EraserSize.Visibility = Visibility.Collapsed;
                    EraserLabel.Visibility = Visibility.Collapsed;
                }
            }
            var handler = ESEvent;
            if (null != handler && EraserMode.SelectedIndex >= 0 && EraserSize != null)
                ESEvent(EraserMode.SelectedIndex, EraserSize.Value);
        }

        private void EraserSize_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var handler = ESEvent;
            if (null != handler && EraserMode.SelectedIndex >= 0 && EraserSize != null)
            {
                ESEvent(EraserMode.SelectedIndex, EraserSize.Value);
                Config.Set("CanvasEraseSlider", EraserSize.Value.ToString());
            }
        }
    }
}
