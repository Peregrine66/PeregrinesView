using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Peregrine.Library;

namespace Peregrine.WPF.ViewModel.Command
{
    /// <summary>
    /// Async implementation of ICommand
    /// </summary>
    public class perRelayCommandAsync : perCommandBase, INotifyPropertyChanged
    {
        private readonly Func<Task> _execute;
        private readonly Func<bool> _canExecute;

        public perRelayCommandAsync(Func<Task> execute) : this(execute, () => true)
        {
        }

        public perRelayCommandAsync(Func<Task> execute, Func<bool> canExecute)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        private bool _isExecuting;

        /// <summary>
        /// Is the command currently executing
        /// </summary>
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

        public override bool CanExecute(object parameter) => !IsExecuting && _canExecute();

        public override async void Execute(object parameter)
        {
            if (!CanExecute(parameter))
            {
                return;
            }

            IsExecuting = true;
            try
            {
                var response = await _execute()
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