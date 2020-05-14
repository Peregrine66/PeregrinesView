using System;
using System.Threading;
using System.Threading.Tasks;

namespace TaskHelperDemo
{
public static class DemoWorker
{
    // Simulate a long running task
    // Repeat stepCount times, pausing by stepInterval each loop, and providing feedback to the caller via progress
    public static async Task<string> DoWork(string workerId, int stepCount, TimeSpan stepInterval, IProgress<int> progress, CancellationToken token)
    {
        var step = 1;

        while (step <= stepCount && !token.IsCancellationRequested)
        {
            await Task.Delay(stepInterval, token).ConfigureAwait(false);

            token.ThrowIfCancellationRequested();

            progress.Report(step++);

            // test what happens when an exception is thrown inside the task
            if (!token.IsCancellationRequested && step > 8)
                throw new ApplicationException("Worker " + workerId + ": Bang!!!");
        }

        return token.IsCancellationRequested
            ? string.Empty
            : "Result from worker " + workerId;
    }
}
}