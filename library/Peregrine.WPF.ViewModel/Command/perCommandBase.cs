using System;
using System.Windows.Input;

namespace Peregrine.WPF.ViewModel.Command
{
    /// <summary>
    /// Base class for commands, that adds RaiseCanExecuteChanged() to ICommand
    /// </summary>
    public abstract class perCommandBase : ICommand
    {
        // ICommand members
        public abstract bool CanExecute(object parameter);
        public abstract void Execute(object parameter);
        public event EventHandler CanExecuteChanged;

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}