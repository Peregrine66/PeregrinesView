using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;

// ReSharper disable InvalidXmlDocComment

namespace Peregrine.Library
{
    internal enum perTaskStatus
    {
        [Description("Completed OK")]
        CompletedOk,
        [Description("Timed Out")]
        TimedOut,
        Error,
        Cancelled
    }

    // ================================================================================================

    /// <summary>
    /// Wrapper around a task to provide timeout / cancellation / error trapping
    /// </summary>
    public static class perTaskHelper
    {
        /// <summary>
        /// run a Task with no timeout
        /// </summary>
        /// <param name="theTask" >
        /// The task to run
        /// </param>
        public static async Task<perAsyncActionResponse> ExecuteActionAsync(this Task theTask)
        {
            try
            {
                await theTask.ConfigureAwait(false);
            }
            catch
            {
                // trap any exception here, theTask will have Faulted status.
            }

            if (theTask.IsCanceled)
            {
                return new perAsyncActionResponse(perTaskStatus.Cancelled);
            }

            return theTask.IsFaulted
                ? new perAsyncActionResponse(perTaskStatus.Error, theTask.Exception?.GetText(), theTask.Exception)
                : new perAsyncActionResponse(perTaskStatus.CompletedOk);
        }

        /// <summary>
        /// run a task with the specified timeout
        /// </summary>
        public static Task<perAsyncActionResponse> ExecuteActionAsync(this Task theTask, TimeSpan timeout)
        {
            return theTask.ExecuteActionAsync(timeout, new CancellationTokenSource());
        }

        /// <summary>
        /// run a task with the specified timeout and cancellation token
        /// </summary>
        /// <remarks>
        /// Parameter is CancellationTokenSource rather than CancellationToken as we want to trigger it in order to cancel
        /// theTask (assuming it's using the same CancellationTokenSource or its token) in the event of a timeout.
        /// </remarks>
        public static async Task<perAsyncActionResponse> ExecuteActionAsync(this Task theTask, TimeSpan timeout, CancellationTokenSource cancellationTokenSource)
        {
            perAsyncActionResponse result;

            // if no timeout then just use the simpler version of this method
            if (timeout.IsForever())
            {
                result = await ExecuteActionAsync(theTask).ConfigureAwait(false);
            }
            else
            {
                // this will kill the timeout task, if the external cancellation token source is cancelled
                using (var timeoutTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationTokenSource.Token))
                {
                    var timeoutTask = Task.Delay(timeout, timeoutTokenSource.Token);

                    var completedTask = Task.CompletedTask;

                    try
                    {
                        completedTask = await Task.WhenAny(theTask, timeoutTask).ConfigureAwait(false);
                    }
                    catch
                    {
                        // trap any exception here, theTask will have Faulted status.
                    }

                    if (cancellationTokenSource.IsCancellationRequested || theTask.IsCanceled)
                    {
                        result = new perAsyncActionResponse(perTaskStatus.Cancelled);
                    }
                    else if (completedTask == timeoutTask)
                    {
                        result = new perAsyncActionResponse(perTaskStatus.TimedOut);

                        // signal theTask to cancel if possible
                        cancellationTokenSource.Cancel();
                    }
                    else if (theTask.IsFaulted)
                    {
                        result = new perAsyncActionResponse(perTaskStatus.Error, theTask.Exception?.GetText(), theTask.Exception);
                    }
                    else
                    {
                        result = new perAsyncActionResponse(perTaskStatus.CompletedOk);
                    }

                    // kill the timeoutTask if it's not already cancelled
                    timeoutTokenSource.Cancel();
                }
            }

            return result;
        }

        /// <summary>
        /// run a Task<T> with no timeout and return the result
        /// </summary>
        public static async Task<perAsyncFunctionResponse<T>> EvaluateFunctionAsync<T>(this Task<T> theTask)
        {
            var response = await theTask.ExecuteActionAsync().ConfigureAwait(false);

            var result = response.CloneAsFunctionResponse<T>();

            if (response.IsCompletedOk)
            {
                result.Data = await theTask.ConfigureAwait(false);
            }

            return result;
        }

        /// <summary>
        /// run a Task<T> with specified timeout and return the result
        /// </summary>
        public static Task<perAsyncFunctionResponse<T>> EvaluateFunctionAsync<T>(this Task<T> theTask, TimeSpan timeout)
        {
            return theTask.EvaluateFunctionAsync(timeout, new CancellationTokenSource());
        }

        /// <summary>
        /// run a Task<T>  with the specified timeout / cancellation token and return the result
        /// </summary>
        public static async Task<perAsyncFunctionResponse<T>> EvaluateFunctionAsync<T>(this Task<T> theTask, TimeSpan timeout, CancellationTokenSource cancellationTokenSource)
        {
            var response = await theTask.ExecuteActionAsync(timeout, cancellationTokenSource).ConfigureAwait(false);

            var result = response.CloneAsFunctionResponse<T>();

            // the task has already completed if status is CompletedOk, but using await once more is better than using theTask.Result
            if (result.IsCompletedOk)
            {
                result.Data = await theTask.ConfigureAwait(false);
            }

            return result;
        }
    }

    // ================================================================================================

    /// <summary>
    /// the return type for ExecuteActionAsync() methods
    /// </summary>
    public class perAsyncActionResponse
    {
        internal perAsyncActionResponse(perTaskStatus status, string errorMessage = null, AggregateException exception = null)
        {
            Status = status;
            ErrorMessage = errorMessage;
            Exception = exception;
        }

        internal perTaskStatus Status { get; }
        public string ErrorMessage { get;  }
        public AggregateException Exception { get; }

        public override string ToString()
        {
            return $"{Status}\r\n\r\n{ErrorMessage}".Trim();
        }

        public bool IsCompletedOk => Status == perTaskStatus.CompletedOk;
        public bool IsTimedOut => Status == perTaskStatus.TimedOut;
        public bool IsCancelled => Status == perTaskStatus.Cancelled;
        public bool IsError => Status == perTaskStatus.Error;

        public string StatusDescription => Status.Description();

        public perAsyncFunctionResponse<T> CloneAsFunctionResponse<T>() => new perAsyncFunctionResponse<T>(Status, ErrorMessage, Exception);
    }

    // ================================================================================================

    /// <summary>
    /// the return type for EvaluateFunctionAsync() methods
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class perAsyncFunctionResponse<T> : perAsyncActionResponse
    {
        internal perAsyncFunctionResponse(perTaskStatus status, string errorMessage= null, AggregateException exception = null) : base(status, errorMessage, exception)
        {
        }

        public T Data { get; internal set; }

        public perAsyncActionResponse CloneAsActionResponse() => new perAsyncActionResponse(Status, ErrorMessage, Exception);
    }
}