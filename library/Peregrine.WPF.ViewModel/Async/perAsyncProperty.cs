using Peregrine.Library;
using System;
using System.Threading.Tasks;

namespace Peregrine.WPF.ViewModel.Async
{
    /// <summary>
    /// A read only property where the value is fetched once when required, in an async manner, and then cached for future use.
    /// </summary>
    /// <remarks>
    /// in Xaml, bind to perAsyncPropertyInstance.Value
    /// </remarks>
    /// <typeparam name="T"></typeparam>
    public class perAsyncProperty<T> : perViewModelBase where T : class
    {
        private bool _fetchingValue;

        private readonly Func<Task<T>> _fetchValue;

        public perAsyncProperty()
        {
        }

        public perAsyncProperty(Func<Task<T>> fetchValue)
        {
            _fetchValue = fetchValue;
        }

        protected virtual Task<T> FetchValue()
        {
            return _fetchValue.Invoke();
        }

        private T _value;

        /// <summary>
        /// The property's value - use this as the source of data binding.
        /// </summary>
        public T Value
        {
            get
            {
                if (_value != null || _fetchingValue)
                {
                    return _value;
                }

                _fetchingValue = true;

                // we can't use await inside a property getter, so use a continuation instead
                // if no timeout then just use the simpler task handler
                if (FetchValueTimeOut.IsForever())
                {
                    FetchValue()
                        .EvaluateFunctionAsync()
                        .ContinueWith(FetchValueContinuation);
                }
                else
                {
                    FetchValue()
                        .EvaluateFunctionAsync(FetchValueTimeOut)
                        .ContinueWith(FetchValueContinuation);
                }

                // Local function to refresh Value once the data fetch task has completed
                async void FetchValueContinuation(Task<perAsyncFunctionResponse<T>> task)
                {
                    var taskResult = await task.ConfigureAwait(false);

                    if (taskResult.IsCompletedOk)
                    {
                        Value = taskResult.Data;
                    }
                    else if (taskResult.IsTimedOut)
                    {
                        OnTimeOutAction?.Invoke(taskResult);
                    }
                    else if (taskResult.IsError)
                    {
                        OnErrorAction?.Invoke(taskResult);
                    }

                    _fetchingValue = false;
                }

                return _value;
            }
            private set => Set(nameof(Value), ref _value, value);
        }

        /// <summary>
        /// Timeout value for FetchValue invocation
        /// </summary>
        public TimeSpan FetchValueTimeOut { get; set; } = perTimeSpanHelper.Forever;

        /// <summary>
        /// Optional action to perform if FetchValue generates an error.
        /// </summary>
        public Action<perAsyncFunctionResponse<T>> OnErrorAction { get; set; }

        /// <summary>
        /// Optional action to perform if FetchValue times out.
        /// </summary>
        public Action<perAsyncFunctionResponse<T>> OnTimeOutAction { get; set; }

        /// <summary>
        /// Clear Value and force it to be re-fetched then next time it is read.
        /// </summary>
        public void ResetValue()
        {
            _fetchingValue = false;
            Value = null;
        }
    }
}