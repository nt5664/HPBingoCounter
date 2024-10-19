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
            MessageBox.Show(
                $"Fatal error occurred!{Environment.NewLine}{e.Exception.Message}{Environment.NewLine}The application will close.",
                "Fatal error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            DispatcherUnhandledException -= OnUnhandledException;

            base.OnExit(e);
        }
    }

}
