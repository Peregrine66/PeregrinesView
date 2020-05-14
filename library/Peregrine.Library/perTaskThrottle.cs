using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Peregrine.Library
{
    public static class perTaskThrottle
    {
        /// <summary>
        /// Run multiple tasks in parallel - up to concurrentTasks tasks may run at any one time
        /// </summary>
        /// <typeparam name="TInput"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="sourceItems"></param>
        /// <param name="func"></param>
        /// <param name="concurrentTasks"></param>
        /// <returns></returns>
        public static Task<IDictionary<TInput, TResult>> ForEachAsyncThrottled<TInput, TResult>(
            this IEnumerable<TInput> sourceItems,
            Func<TInput, Task<TResult>> func,
            int concurrentTasks = 1)
        {
            return ForEachAsyncThrottled(sourceItems, func, CancellationToken.None, concurrentTasks);
        }

        /// <summary>
        /// Run multiple tasks in parallel - up to concurrentTasks tasks may run at any one time
        /// </summary>
        /// <typeparam name="TInput"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="sourceItems"></param>
        /// <param name="func"></param>
        /// <param name="token"></param>
        /// <param name="concurrentTasks"></param>
        /// <returns></returns>
        public static async Task<IDictionary<TInput, TResult>> ForEachAsyncThrottled<TInput, TResult>(
            this IEnumerable<TInput> sourceItems,
            Func<TInput, Task<TResult>> func,
            CancellationToken token,
            int concurrentTasks = 1)
        {
            var result = new ConcurrentDictionary<TInput, TResult>();

            var tasksList = new List<Task>();
            using (var semaphoreSlim = new SemaphoreSlim(concurrentTasks))
            {
                foreach (var item in sourceItems)
                {
                    token.ThrowIfCancellationRequested();

                    // if there are already concurrentTasks tasks executing, pause until one has completed ( semaphoreSlim.Release() )
                    await semaphoreSlim.WaitAsync(perTimeSpanHelper.Forever, token).ConfigureAwait(false);

                    token.ThrowIfCancellationRequested();

                    Action<Task<TResult>> okContinuation = async task =>
                    {
                        // the task has already completed if status is CompletedOk, but using await once more is safer than using task.Result
                        var taskResult = await task;
                        result[item] = taskResult;
                    };

                    // ReSharper disable once AccessToDisposedClosure
                    Action<Task> allContinuation = task => semaphoreSlim.Release();

                    tasksList.Add(func.Invoke(item)
                        .ContinueWith(okContinuation, TaskContinuationOptions.OnlyOnRanToCompletion)
                        .ContinueWith(allContinuation, token));

                    token.ThrowIfCancellationRequested();
                }

                if (!token.IsCancellationRequested)
                {
                    await Task.WhenAll(tasksList).ConfigureAwait(false);
                }
            }

            return result;
        }
    }
}