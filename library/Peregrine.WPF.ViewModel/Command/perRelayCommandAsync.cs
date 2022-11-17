using System;
using System.Threading;
using System.Threading.Tasks;

namespace Peregrine.WPF.ViewModel.Command
{
    /// <summary>
    /// Async implementation of ICommand
    /// </summary>
    public class perRelayCommandAsync : perRelayCommandAsyncBase
    {
        private readonly Func<Task> _execute;
        private readonly Func<CancellationToken, Task> _cancellableExecute;
        private readonly Func<bool> _canExecute;

        public perRelayCommandAsync(Func<Task> execute) : this(execute, () => true)
        {
        }

        public perRelayCommandAsync(Func<CancellationToken, Task> cancellableExecute) : this(cancellableExecute, () => true)
        {
        }

        public perRelayCommandAsync(Func<Task> execute, Func<bool> canExecute)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public perRelayCommandAsync(Func<CancellationToken, Task> cancellableExecute, Func<bool> canExecute)
        {
            _cancellableExecute = cancellableExecute ?? throw new ArgumentNullException(nameof(cancellableExecute));
            _canExecute = canExecute;
        }

        public override bool CanExecute(object parameter) => !IsExecuting && _canExecute();

        protected override Task DoExecuteInternal(object parameter)
        {
            if (_execute == null)
            {
                throw new InvalidOperationException("Command must defined with a non-cancellable execute parameter for use with no timeout");
            }

            return _execute();
        }

        protected override Task DoExecuteInternal(object parameter, CancellationToken cancellationToken)
        {
            if (_cancellableExecute == null)
            {
                throw new InvalidOperationException("Command must defined with a cancellable execute parameter for use with a timeout");
            }

            return _cancellableExecute(cancellationToken);
        }
    }
}