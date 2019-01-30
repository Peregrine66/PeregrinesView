using Peregrine.Library.Collections;
using System;
using System.Windows.Threading;
using System.Threading.Tasks;

namespace Peregrine.WPF.ViewModel.Helpers
{
    /// <summary>
    /// Provides a mechanism for invoking actions on the UI dispatcher
    /// </summary>
    /// <remarks>
    /// Based on Galasoft MvvmLight DispatcherHelper, but allowing a DispatcherPriority value to be applied.
    /// </remarks>
    public static class perDispatcherHelper
    {
        private static Dispatcher UIDispatcher { get; set; }

        public static void Initialise()
        {
            if (UIDispatcher != null && UIDispatcher.Thread.IsAlive)
                return;

            UIDispatcher = Dispatcher.CurrentDispatcher;
        }

        private static void CheckDispatcher()
        {
            if (UIDispatcher != null)
                return;

            const string errorMessage = "The Dispatcher Helper is not initialized.\r\n\r\n" +
                                        "Call perDispatcherHelper.Initialize() in the static App constructor.";

            throw new InvalidOperationException(errorMessage);
        }

        /// <summary>
        /// Invoke the action on the captured UI dispatcher, using the default priority
        /// </summary>
        /// <param name="action"></param>
        public static void CheckBeginInvokeOnUI(Action action)
        {
            CheckBeginInvokeOnUI(action, DispatcherPriority.Normal);
        }

        /// <summary>
        /// Invoke the action on the captured UI dispatcher, using the specified priority
        /// </summary>
        /// <param name="action"></param>
        /// <param name="priority"></param>
        public static void CheckBeginInvokeOnUI(Action action, DispatcherPriority priority)
        {
            var unused = RunAsync(action, priority);
        }

        /// <summary>
        /// Invoke the action on the captured UI dispatcher, using the default priority
        /// and return an awaitable object
        /// </summary>
        /// <param name="action"></param>
        public static DispatcherOperation RunAsync(Action action)
        {
            return RunAsync(action, DispatcherPriority.Normal);
        }

        /// <summary>
        /// Invoke the action on the captured UI dispatcher, using the specified priority
        /// and return an awaitable object
        /// </summary>
        /// <param name="action"></param>
        /// <param name="priority"></param>
        public static DispatcherOperation RunAsync(Action action, DispatcherPriority priority)
        {
            CheckDispatcher();
            return UIDispatcher.BeginInvoke(action, priority);
        }

        // max heap keeps the highest sorting item at the top of the heap, without any requirement to exactly sort the remaining items
        private static readonly perMaxHeap<perDispatcherItemPair> PriorityQueue = new perMaxHeap<perDispatcherItemPair>();

        /// <summary>
        /// Add a new operation to the queue, with the default priority 
        /// </summary>
        /// <remarks>
        /// Operations are executed in DispatcherPriority order (highest value executes first)
        /// </remarks>
        /// <param name="action"></param>
        public static void AddToQueue(Action action)
        {
            AddToQueue(action, DispatcherPriority.Normal);
        }

        /// <summary>
        /// Add a new operation to the queue, with the specified priority 
        /// </summary>
        /// <remarks>
        /// Operations are executed in DispatcherPriority order (highest value executes first)
        /// </remarks>
        /// <param name="action"></param>
        /// <param name="dispatcherPriority"></param>
        public static void AddToQueue(Action action, DispatcherPriority dispatcherPriority)
        {
            PriorityQueue.Add(new perDispatcherItemPair(action, dispatcherPriority));
        }

        // stops the queue being processed more than once at a time
        private static bool IsProcessingQueue { get; set; }

        /// <summary>
        /// Execute each operation from the operations queue in order.
        /// </summary>
        /// <remarks>
        /// An operation may result in more items being added to the queue, which will be
        /// executed in the appropriate order.
        /// </remarks>
        /// <returns></returns>
        public static async Task ProcessQueueAsync()
        {
            if (IsProcessingQueue)
                return;

            IsProcessingQueue = true;

            try
            {
                while (PriorityQueue.Any())
                {
                    // remove brings the next highest item to the top of the heap
                    var pair = PriorityQueue.Remove();
                    await RunAsync(pair.Action, pair.DispatcherPriority);
                }
            }
            finally
            {
                IsProcessingQueue = false;
            }
        }

        /// <summary>
        /// Clear any outstanding operations from the queue
        /// </summary>
        public static void ResetQueue()
        {
            PriorityQueue.Reset();
        }

        // ================================================================================

        /// <summary>
        /// Internal class representing a queued operation.
        /// </summary>
        private class perDispatcherItemPair : IComparable<perDispatcherItemPair>
        {
            public perDispatcherItemPair(Action action, DispatcherPriority dispatcherPriority)
            {
                Action = action;
                DispatcherPriority = dispatcherPriority;

                // calculate ItemIndex so that items added earlier sort higher in the heap
                // no need to worry about Ticks overflowing, ~100 billion days ought to be sufficient
                ItemIndex = long.MaxValue - DateTime.UtcNow.Ticks;
            }

            public Action Action { get; }

            public DispatcherPriority DispatcherPriority { get; }

            // keep items with the same DispatcherPriority in the order they were added to the queue
            private long ItemIndex { get; }

            public int CompareTo(perDispatcherItemPair other)
            {
                var result = ((int)DispatcherPriority).CompareTo((int)other.DispatcherPriority);

                if (result == 0)
                    result = ItemIndex.CompareTo(other.ItemIndex);

                return result;
            }
        }
    }
}
