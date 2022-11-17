using Peregrine.WPF.ViewModel.Helpers;
using System;
using System.Windows.Input;

namespace Peregrine.WPF.ViewModel.Command
{
    public abstract class perRelayCommandBase : ICommand
    {
        protected perRelayCommandBase()
        {
            perDispatcherHelper.Initialise();
        }

        public void Execute(object parameter)
        {
            if (CanExecute(parameter))
            {
                ExecuteInternal(parameter);
            }
        }

        public abstract bool CanExecute(object parameter);

        protected abstract void ExecuteInternal(object parameter);

        public event EventHandler CanExecuteChanged;

        public void RaiseCanExecuteChanged()
        {
            perDispatcherHelper.InvokeOnUIContext(() => CanExecuteChanged?.Invoke(this, EventArgs.Empty));
        }
    }
}
