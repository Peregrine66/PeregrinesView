using Peregrine.Library;
using Peregrine.WPF.ViewModel.Helpers;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Peregrine.WPF.ViewModel.Command
{
    public abstract class perRelayCommandAsyncBase : perRelayCommandBase, INotifyPropertyChanged
    {
        protected perRelayCommandAsyncBase()
        {
            perDispatcherHelper.Initialise();
        }

        // INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            perDispatcherHelper.InvokeOnUIContext(() => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)));
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
                RaiseCanExecuteChanged();
                RaisePropertyChanged();
            }
        }

        protected override async void ExecuteInternal(object parameter)
        {
            IsExecuting = true;

            try
            {
                perAsyncActionResponse response;

                // if no timeout then just use the simpler task handler
                if (ExecuteTimeOut.IsForever())
                {
                    response = await DoExecuteInternal(parameter)
                        .ExecuteActionAsync()
                        .ConfigureAwait(false);
                }
                else
                {
                    var cancellationTokenSource = new CancellationTokenSource();

                    response = await DoExecuteInternal(parameter, cancellationTokenSource.Token)
                        .ExecuteActionAsync(ExecuteTimeOut, cancellationTokenSource)
                        .ConfigureAwait(false);
                }

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

        protected abstract Task DoExecuteInternal(object parameter);

        protected abstract Task DoExecuteInternal(object parameter, CancellationToken cancellationToken);
    }
}
