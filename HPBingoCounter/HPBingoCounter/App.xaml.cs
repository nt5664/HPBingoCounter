using System.Text;
using System.Windows;
using System.Windows.Threading;

namespace HPBingoCounter
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            DispatcherUnhandledException += OnUnhandledException;
        }

        private void OnUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Fatal error occurred!");
            sb.AppendLine(e.Exception.Message);
            sb.Append("Inner exception: ");
            sb.AppendLine(e.Exception.InnerException?.Message);
            sb.Append($"{Environment.NewLine}The application will close...");

            MessageBox.Show(sb.ToString(), "Fatal error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            DispatcherUnhandledException -= OnUnhandledException;

            base.OnExit(e);
        }
    }

}
