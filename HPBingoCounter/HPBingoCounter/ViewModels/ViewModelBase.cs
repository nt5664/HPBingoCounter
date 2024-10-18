using System.ComponentModel;
using System.Runtime.CompilerServices;

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
    }
}
