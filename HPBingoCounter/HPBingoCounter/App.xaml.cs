using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Threading;
using HPBingoCounter.Core;
using HPBingoCounter.ViewModels;

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

        public static Version AppVersion => new(2, 2);

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

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            MainWindow window = new MainWindow
            {
                 DataContext = new MainViewModel(new HPBingoService())
            };

            MainWindow = window;
            MainWindow.Show();

            CheckNewVersion();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            DispatcherUnhandledException -= OnUnhandledException;

            base.OnExit(e);
        }

        private static async void CheckNewVersion()
        {
            try
            {
                bool updateAvailable = await GitHubInterop.IsNewerVersionAvailableAsync(AppVersion);
                if (updateAvailable)
                {
                    if (MessageBox.Show("A newer version of the app is available. Would you like to download it?", "Update Available", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        Process.Start(new ProcessStartInfo(GitHubInterop.RELEASES_URL) { UseShellExecute = true });
                    }
                }
            }
            catch (Exception ex)
            {
                string message = $"Something went wrong; could not check for updates.{Environment.NewLine}Details: {ex.Message}";
                MessageBox.Show(message, "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }

}
