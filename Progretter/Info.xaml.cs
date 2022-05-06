using System.Diagnostics;
using System.Windows;
using System.Windows.Navigation;

namespace Progretter
{
    /// <summary>
    /// Info.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Info : Window
    {
        public Info()
        {
            InitializeComponent();
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true });
        }

        // Display the underline on only the MouseEnter event.
        private void OnMouseEnter(object sender, System.EventArgs e)
        {
            (sender as System.Windows.Documents.Hyperlink).TextDecorations = TextDecorations.Underline;
        }

        // Remove the underline on the MouseLeave event.
        private void OnMouseLeave(object sender, System.EventArgs e)
        {
            (sender as System.Windows.Documents.Hyperlink).TextDecorations = null;
        }
    }
}
