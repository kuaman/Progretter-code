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

        private void colorpicker_ColorChanged(object sender, RoutedEventArgs e)
        {
            Alpha = colorpicker.SelectedColor.A;
            Red = colorpicker.SelectedColor.R;
            Green = colorpicker.SelectedColor.G;
            Blue = colorpicker.SelectedColor.B;

            CPEvent(Alpha, Red, Green, Blue);
        }

        private int load = 0;

        private void StrokeEditingMode_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (load == 1)
            {
                editmode = StrokeEditingMode.SelectedIndex;

                EMEvent(editmode);
            }
            else
            {
                load = 1;
            }
        }

        public void RecEditingMode(string noweditmode)
        {
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
    }
}
