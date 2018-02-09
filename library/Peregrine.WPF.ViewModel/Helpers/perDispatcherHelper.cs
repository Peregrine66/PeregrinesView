using System;
using System.Windows.Threading;

namespace Peregrine.WPF.ViewModel.Helpers
{
    /// <summary>
    /// Based on Galasoft MvvmLight DispatcherHelper, but allowing a DispatecherPriority value to be applied.
    /// </summary>
    public static class perDispatcherHelper
    {
        public static Dispatcher UIDispatcher { get; private set; }

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

        public static void CheckBeginInvokeOnUI(Action action, DispatcherPriority priority = DispatcherPriority.Normal)
        {
            var unused = RunAsync(action, priority);
        }

        public static DispatcherOperation RunAsync(Action action, DispatcherPriority priority = DispatcherPriority.Normal)
        {
            CheckDispatcher();
            return UIDispatcher.BeginInvoke(action, priority);
        }
    }
}
