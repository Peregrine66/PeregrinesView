using Peregrine.Library;
using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Peregrine.WPF.ViewModel.Command
{
    /// <summary>
    /// Async implementation of ICommand, that takes a typed parameter
    /// </summary>
    public class perRelayCommandAsync<T> : perCommandBase, INotifyPropertyChanged
    {
        private readonly Func<T, Task> _execute;
        private readonly Func<T, bool> _canExecute;

        public perRelayCommandAsync(Func<T, Task> execute) : this(execute, _ => true)
        {
        }

        public perRelayCommandAsync(Func<T, Task> execute, Func<T, bool> canExecute)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        private bool _isExecuting;

        public bool IsExecuting
        {
            get => _isExecuting;
            set
            {
                _isExecuting = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsExecuting)));
                RaiseCanExecuteChanged();
            }
        }

        public override bool CanExecute(object parameter) => !_isExecuting && _canExecute((T)parameter);

        public override async void Execute(object parameter)
        {
            if (!CanExecute(parameter))
            {
                return;
            }

            IsExecuting = true;
            try
            {
                var response = await _execute((T)parameter)
                    .ExecuteActionWithTimeoutAsync(ExecuteTimeOut)
                    .ConfigureAwait(true);

                if (response.IsTimedOut)
                {
                    OnTimeOutAction?.Invoke(response);
                }
                else if (response.IsError)
                {
                    OnErrorAction?.Invoke(response);
                }
            }
            finally
            {
                IsExecuting = false;
            }
        }

        /// <summary>
        /// Timeout value for Execute invocation
        /// </summary>
        public TimeSpan ExecuteTimeOut { get; set; } = perTimeSpanHelper.Forever;

        /// <summary>
        /// Optional action to perform if Execute generates an error.
        /// </summary>
        public Action<perAsyncActionResponse> OnErrorAction { get; set; }

        /// <summary>
        /// Optional action to perform if Execute times out.
        /// </summary>
        public Action<perAsyncActionResponse> OnTimeOutAction { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
