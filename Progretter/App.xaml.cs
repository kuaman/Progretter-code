using AutoUpdaterDotNET;
using Microsoft.Toolkit.Uwp.Notifications;
using System.Threading;
using System.Windows;

namespace Progretter
{
    /// <summary>
    /// App.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class App : Application
    {
        Mutex mutex;
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            ShutdownMode = ShutdownMode.OnMainWindowClose;

            string mutexName = "program";
            bool createNew;

            mutex = new Mutex(true, mutexName, out createNew);

            if (!createNew)
            {
                Shutdown();
            }

            // Listen to notification activation
            ToastNotificationManagerCompat.OnActivated += toastArgs =>
            {
                // Need to dispatch to UI thread if performing UI operations
                Application.Current.Dispatcher.Invoke(delegate
                {
                    string tag = toastArgs.Argument;
                    switch (tag.Substring(tag.LastIndexOf("9")))
                    {
                        case "9000": // Activation Toast Notificaton
                            foreach (Window openForm in Application.Current.Windows)
                            {
                                if (openForm.Title == "온라인 클래스 도우미") // 열린 창의 이름 검사
                                {
                                    if (openForm.WindowState == WindowState.Minimized)
                                    {  // 창을 최소화시켜 하단에 내려놓았는지 검사
                                        openForm.WindowState = WindowState.Normal;
                                    }
                                    openForm.Activate();
                                    return;
                                }
                            }
                            break;

                        case "9100": // User Custom Alarm
                            if (tag.StartsWith("again"))
                            {
                                ((MainWindow)Application.Current.MainWindow).Notification("다시 알림", "다시 알림을 설정한지 10분이 지났습니다.", 9100, 0, 0, 10, 0);
                            }
                            break;

                        case "9900": // Update Check
                            AutoUpdater.Start("https://raw.githubusercontent.com/kuaman/Progretter-code/master/Progretter/version.xml");
                            break;

                        default:
                            break;
                    }
                });
            };
        }
    }
}
