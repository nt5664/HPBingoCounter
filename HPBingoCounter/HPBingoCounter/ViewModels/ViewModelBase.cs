using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;

namespace HPBingoCounter.ViewModels
{
    internal abstract class ViewModelBase : INotifyPropertyChanged, IDisposable
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected bool SetValue<T>(ref T? oldVal, T? newVal, [CallerMemberName] string? propName = null, params string[] addProps)
        {
            if (EqualityComparer<T>.Default.Equals(oldVal, newVal))
                return false;

            oldVal = newVal;
            RaisePropertyChanged(propName, addProps);
            return true;
        }

        protected void RaisePropertyChanged(string? propName,  params string[] addProps)
        {
            if (string.IsNullOrWhiteSpace(propName))
                return;

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
            if (addProps is null)
                return;

            foreach (var prop in addProps)
            {
                if (string.IsNullOrWhiteSpace(prop))
                    continue;

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
            }
        }

        public virtual void Dispose()
        { }

        protected static void SafeInvoke(Action action, Action? cleanupAction = null)
        {
            try
            {
                action?.Invoke();
            }
            catch (Exception ex)
            {
                ShowError(ex);
                cleanupAction?.Invoke();
            }
        }

        protected static async void SafeInvokeAsync(Func<Task> action, Action? cleanupAction = null)
        {
            try
            {
                await action();
            }
            catch (Exception ex)
            {
                ShowError(ex);
                cleanupAction?.Invoke();
            }
        }

        private static void ShowError(Exception ex)
        {
            StringBuilder sb = new StringBuilder($"Error!{Environment.NewLine}");
            sb.AppendLine(ex.Message);
            sb.Append("Inner exception: ");
            sb.AppendLine(ex.InnerException?.Message);

            MessageBox.Show(sb.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
