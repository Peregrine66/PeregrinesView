using System;

namespace Peregrine.WPF.ViewModel.Command
{
    /// <summary>
    /// ICommand implementation, that takes a typed parameter
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class perRelayCommand<T> : perRelayCommandBase
    {
        private readonly Action<T> _execute;
        private readonly Func<T, bool> _canExecute;

        public perRelayCommand(Action<T> execute) : this(execute, _ => true)
        {
        }

        public perRelayCommand(Action<T> execute, Func<T, bool> canExecute)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public override bool CanExecute(object parameter) => _canExecute.Invoke((T) parameter);

        protected override void ExecuteInternal(object parameter)
        {
            _execute.Invoke((T) parameter);
        }
    }
}