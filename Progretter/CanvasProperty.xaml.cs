using System.Windows;

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

        public delegate void EditSizeHandler(double size);

        public event EditSizeHandler ESEvent;
        private double slider_size = 1;


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

        private void StrokeEditingMode_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
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

        public void RecEditingMode(byte A, byte R, byte G, byte B, string noweditmode)
        {
            colorpicker.Color.A = A;
            colorpicker.Color.RGB_R = R;
            colorpicker.Color.RGB_G = G;
            colorpicker.Color.RGB_B = B;
            switch (noweditmode)
            {
                case "Ink":
                    StrokeEditingMode.SelectedIndex = 0;
                    break;

                case "EraseByPoint":
                    StrokeEditingMode.SelectedIndex = 1;
                    break;

                case "EraseByStroke":
                    StrokeEditingMode.SelectedIndex = 2;
                    break;

                case "GestureOnly":
                    StrokeEditingMode.SelectedIndex = 3;
                    break;

                case "InkAndGesture":
                    StrokeEditingMode.SelectedIndex = 4;
                    break;

                case "Select":
                    StrokeEditingMode.SelectedIndex = 5;
                    break;
            }
        }

        private void StrokeSize_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            slider_size = StrokeSize.Value;
            var handler = ESEvent;
            if (null != handler)
                ESEvent(slider_size);
        }
    }
}
