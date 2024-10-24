﻿using System.Windows.Input;

namespace HPBingoCounter.Commands
{
    internal class DelegateCommand : ICommand
    {
        public event EventHandler? CanExecuteChanged;

        private readonly Action<object?> _executeAction;
        private readonly Predicate<object?> _canExecuteAction;

        public DelegateCommand(Action<object?> executeAction, Predicate<object?>? canExecuteAction = null)
        {
            _executeAction = executeAction ?? throw new ArgumentNullException(nameof(executeAction));
            _canExecuteAction = canExecuteAction ?? DefaultCanExecute;
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        public bool CanExecute(object? parameter)
        {
            return _canExecuteAction(parameter);
        }

        public void Execute(object? parameter)
        {
            _executeAction(parameter);
        }

        private static bool DefaultCanExecute(object? parameter)
        {
            return true;
        }
    }
}
