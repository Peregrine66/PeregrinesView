using System;
using System.Threading;
using System.Threading.Tasks;

namespace Peregrine.WPF.ViewModel.Command
{
    /// <summary>
    /// Async implementation of ICommand, that takes a typed parameter
    /// </summary>
    public class perRelayCommandAsync<T> : perRelayCommandAsyncBase
    {
        private readonly Func<T, Task> _execute;
        private readonly Func<T, CancellationToken, Task> _cancellableExecute;
        private readonly Func<T, bool> _canExecute;

        public perRelayCommandAsync(Func<T, Task> execute) : this(execute, _ => true)
        {
        }

        public perRelayCommandAsync(Func<T, CancellationToken, Task> cancellableExecute) : this(cancellableExecute, _ => true)
        {
        }

        public perRelayCommandAsync(Func<T, Task> execute, Func<T, bool> canExecute)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public perRelayCommandAsync(Func<T, CancellationToken, Task> cancellableExecute, Func<T, bool> canExecute)
        {
            _cancellableExecute = cancellableExecute ?? throw new ArgumentNullException(nameof(cancellableExecute));
            _canExecute = canExecute;
        }

        public override bool CanExecute(object parameter) => !IsExecuting && _canExecute((T) parameter);

        protected override Task DoExecuteInternal(object parameter)
        {
            if (_execute == null)
            {
                throw new InvalidOperationException("Command must defined with a non-cancellable execute parameter for use with no timeout");
            }

            return _execute((T) parameter);
        }

        protected override Task DoExecuteInternal(object parameter, CancellationToken cancellationToken)
        {
            if (_cancellableExecute == null)
            {
                throw new InvalidOperationException("Command must defined with a cancellable execute parameter for use with a timeout");
            }

            return _cancellableExecute((T) parameter, cancellationToken);
        }
    }
}