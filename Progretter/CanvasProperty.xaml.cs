﻿using System.Windows;
using System.Windows.Controls;

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

        private void colorpicker_ColorChanged(object sender, RoutedEventArgs e)
        {
            Alpha = colorpicker.SelectedColor.A;
            Red = colorpicker.SelectedColor.R;
            Green = colorpicker.SelectedColor.G;
            Blue = colorpicker.SelectedColor.B;

            CPEvent(Alpha, Red, Green, Blue);
        }

        private void StrokeEditingMode_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
/*            EMEvent(StrokeEditingMode.SelectedIndex);*/
        }
    }
}
