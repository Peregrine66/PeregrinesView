using System;
using System.Threading.Tasks;
using System.Threading;

// ReSharper disable InvalidXmlDocComment

namespace Peregrine.Library
{
    /// <summary>
    /// The status code returned by RunTaskxxxAsync() & GetTaskResultxxxAsync() methods
    /// </summary>
    public enum perTaskStatus
    {
        CompletedOk,
        TimedOut,
        Error,
        Cancelled
    }

    // ================================================================================================

    /// <summary>
    /// Wrapper around an async operation to provide timeout / cancellation / error trapping
    /// </summary>
    public static class perTaskHelper
    {
        /// <summary>
        /// run a Task with no timeout
        /// </summary>
        /// <param name="theTask" >
        /// The task to run
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public static Task<perTaskResult> RunTaskAsync(this Task theTask)
        {
            return theTask.RunTaskWithTimeoutAsync(perTimeSpanHelper.Forever);
        }

        /// <summary>
        /// run a task with the specified timeout
        /// </summary>
        public static Task<perTaskResult> RunTaskWithTimeoutAsync(this Task theTask, TimeSpan timeout)
        {
            return theTask.RunTaskWithTimeoutAsync(timeout, new CancellationTokenSource());
        }

        /// <summary>
        /// run a task with the specified CancellationTokenSource
        /// </summary>
        public static Task<perTaskResult> RunTaskAsync(this Task theTask, CancellationTokenSource tokenSource)
        {
            return theTask.RunTaskWithTimeoutAsync(perTimeSpanHelper.Forever, tokenSource);
        }

        /// <summary>
        /// run a task with the specified timeout and cancellation token
        /// </summary>
        /// <remarks>
        /// tokenSource Parameter is CancellationTokenSource rather than CancellationToken as we want to trigger it in order to cancel
        /// theTask (assuming it's using the same CancellationTokenSource or its token) in the event of a timeout.
        /// </remarks>
        public static async Task<perTaskResult> RunTaskWithTimeoutAsync(this Task theTask, TimeSpan timeout, CancellationTokenSource tokenSource)
        {
            var result = new perTaskResult();

            // this will kill the timeout task, if the external cancellation token source is cancelled
            using (var timeoutTokenSource = CancellationTokenSource.CreateLinkedTokenSource(tokenSource.Token))
            {
                var timeoutTask = Task.Delay(timeout, timeoutTokenSource.Token);

                var completedTask = await Task.WhenAny(theTask, timeoutTask).ConfigureAwait(false);

                if (tokenSource.IsCancellationRequested)
                {
                    result.Status = perTaskStatus.Cancelled;
                }
                else if (completedTask == timeoutTask)
                {
                    result.Status = perTaskStatus.TimedOut;

                    // signal theTask to cancel if possible
                    tokenSource.Cancel();
                }
                else if (theTask.IsFaulted)
                {
                    result.Status = perTaskStatus.Error;
                    result.Exception = theTask.Exception;
                    result.ErrorMessage = theTask.Exception?.GetText();
                }
                else
                {
                    result.Status = perTaskStatus.CompletedOk;
                }

                // kill the timeoutTask if it's not already cancelled
                timeoutTokenSource.Cancel();
            }

            return result;
        }

        /// <summary>
        /// run a Task T with no timeout and return the result
        /// </summary>
        public static Task<perTaskResult<T>> GetTaskResultAsync<T>(this Task<T> theTask)
        {
            return theTask.GetTaskResultWithTimeoutAsync(perTimeSpanHelper.Forever);
        }

        /// <summary>
        /// run a Task T  with specified timeout and return the result
        /// </summary>
        public static Task<perTaskResult<T>> GetTaskResultWithTimeoutAsync<T>(this Task<T> theTask, TimeSpan timeout)
        {
            return theTask.GetTaskResultWithTimeoutAsync(timeout, new CancellationTokenSource());
        }

        /// <summary>
        /// run a Task T with specified CancellationTokenSource and return the result
        /// </summary>
        public static Task<perTaskResult<T>> GetTaskResultAsync<T>(this Task<T> theTask, CancellationTokenSource tokenSource)
        {
            return theTask.GetTaskResultWithTimeoutAsync(perTimeSpanHelper.Forever, tokenSource);
        }

        /// <summary>
        /// run a Task<T> T with the specified timeout / cancellation token and return the result
        /// </summary>
        public static async Task<perTaskResult<T>> GetTaskResultWithTimeoutAsync<T>(this Task<T> theTask, TimeSpan timeout, CancellationTokenSource tokenSource)
        {
            var taskResult = await theTask.RunTaskWithTimeoutAsync(timeout, tokenSource).ConfigureAwait(false);
            var result = new perTaskResult<T>
            {
                Status = taskResult.Status,
                Exception = taskResult.Exception,
                ErrorMessage = taskResult.ErrorMessage
            };

            // the task has already completed if status is CompletedOk, but using await once more is better than using theTask.Result
            if (result.Status == perTaskStatus.CompletedOk)
                result.Data = await theTask.ConfigureAwait(false);

            return result;
        }
    }

    // ================================================================================================

    /// <summary>
    /// the return type for RunTaskxxxAsync() methods
    /// </summary>
    public class perTaskResult
    {
        public perTaskStatus Status { get; internal set; }
        public string ErrorMessage { get; internal set; }
        public AggregateException Exception { get; internal set; }

        public override string ToString()
        {
            return $"{Status}\r\n\r\n{ErrorMessage}".Trim();
        }
    }

    // ================================================================================================

    /// <inheritdoc/>
    /// <summary>
    /// the return type for GetTaskResultxxxAsync() methods
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class perTaskResult<T> : perTaskResult
    {
        public T Data { get; set; }
    }
}
