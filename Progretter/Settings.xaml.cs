using Microsoft.Win32;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;

namespace Progretter
{
    /// <summary>
    /// Menu.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Settings : Window
    {
        public Settings()
        {
            InitializeComponent();
            Assembly assembly = Assembly.GetEntryAssembly();
            Setting_Version_Label.Content = $"현재 버전 : {assembly.GetName().Version}";
            Setting_Schedule_StartUp_Label.Content = Config.Get("ScheduleStartUpPath");

            if (Config.Get("ScheduleCloseSave") == "true")
            {
                Setting_Schedule_Close_Save_CheckBox.IsChecked = true;
            }
            if (Config.Get("TextDragAllExtension") == "true")
            {
                Setting_Note_DragAllExtension_CheckBox.IsChecked = true;
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
        }


        #region 설정
        private void Setting_Schedule_StartUp_Btn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Excel File(*.xlsx)|*.xlsx";
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

        private void Setting_Note_DragAllExtension_CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            Config.Set("TextDragAllExtension", "true");
        }

        private void Setting_Note_DragAllExtension_CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            Config.Set("TextDragAllExtension", "false");
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

        private void Setting_Info_Btn_Click(object sender, RoutedEventArgs e)
        {
            Info info = new Info();
            info.Owner = this;
            info.ShowDialog();
        }
        private void Setting_AS_Folder_Btn_Click(object sender, RoutedEventArgs e)
        {
            DirectoryInfo di = new DirectoryInfo(((MainWindow)Application.Current.MainWindow).ASpath);

            if (!di.Exists)   //If New Folder not exists
            {
                di.Create();             //create Folder
            }
            ProcessStartInfo startInfo = new ProcessStartInfo(((MainWindow)Application.Current.MainWindow).ASpath)
            {
                UseShellExecute = true
            };
            Process.Start(startInfo);
        }

        private void Setting_Reset_Btn_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("모든 설정을 초기화 하시겠습니까?", "설정 초기화", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
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
        }
        #endregion

        private void Tetris_btn_Click(object sender, RoutedEventArgs e)
        {
            Tetris tetris = new Tetris();
            tetris.Show();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
