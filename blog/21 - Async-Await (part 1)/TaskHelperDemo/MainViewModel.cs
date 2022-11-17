using Peregrine.Library;
using Peregrine.WPF.ViewModel;
using Peregrine.WPF.ViewModel.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace TaskHelperDemo
{
    public class MainViewModel : perViewModelBase
    {
        private CancellationTokenSource _cancellationTokenSource;

        public MainViewModel()
        {
            StartTasksCommand = new perRelayCommandAsync(OnStartTasks);
            CancelCommand = new perRelayCommand(() => _cancellationTokenSource?.Cancel(), ()=>StartTasksCommand.IsExecuting)
                .ObservesInternalProperty(StartTasksCommand, nameof(StartTasksCommand.IsExecuting));

            var allTimeouts = new List<perValueDisplayPair<TimeSpan>>
            {
                perTimeSpanHelper.Forever.CreateValueDisplayPair("None")
            };

            allTimeouts.AddRange(Enumerable.Range(1, 20)
                .Select(i => TimeSpan.FromSeconds(i).CreateValueDisplayPair($"{i} second{(i > 1 ? "s" : "")}")));

            AllTimeouts = allTimeouts;
            SelectedTimeout = allTimeouts.First().Value;
        }

        private readonly ObservableCollection<int> _progress1Collection = new ObservableCollection<int>();
        public IEnumerable<int> Progress1 => _progress1Collection;

        private string _globalStatus;

        public string GlobalStatus
        {
            get => _globalStatus;
            set => Set(nameof(GlobalStatus), ref _globalStatus, value);
        }

        private string _status1;

        public string Status1
        {
            get => _status1;
            set => Set(nameof(Status1), ref _status1, value);
        }

        private readonly ObservableCollection<int> _progress2Collection = new ObservableCollection<int>();
        public IEnumerable<int> Progress2 => _progress2Collection;

        private string _status2;

        public string Status2
        {
            get => _status2;
            set => Set(nameof(Status2), ref _status2, value);
        }

        private string _status3;

        private readonly ObservableCollection<int> _progress3Collection = new ObservableCollection<int>();
        public IEnumerable<int> Progress3 => _progress3Collection;

        public string Status3
        {
            get => _status3;
            set => Set(nameof(Status3), ref _status3, value);
        }

        public perRelayCommandAsync StartTasksCommand { get; }

        private async Task OnStartTasks()
        {
            using (_cancellationTokenSource = new CancellationTokenSource())
            {
                _progress1Collection.Clear();
                _progress2Collection.Clear();
                _progress3Collection.Clear();

                GlobalStatus = "Processing all Tasks ...";
                Status1 = "Processing ...";
                Status2 = "Processing ...";
                Status2 = "Processing ...";

                // Update the UI from within the task - no need to worry about dispatcher context, .Net handles that for us
                var progress1 = new Progress<int>(i => _progress1Collection.Add(i));

                // Run the work task
                // The continuation updates the UI, as soon as this task completes fully without an issue
                // Unwrap() removes the extra layer of task<> from the continuation
                var task1 = DemoWorker.DoWork("1", 10, TimeSpan.FromMilliseconds(400), progress1, _cancellationTokenSource.Token)
                    .EvaluateFunctionAsync()
                    .ContinueWith(t =>
                        {
                            Status1 = "Completed Ok.";
                            return t;
                        },
                        TaskContinuationOptions.OnlyOnRanToCompletion)
                    .Unwrap();

                var progress2 = new Progress<int>(i => _progress2Collection.Add(i));

                var task2 = DemoWorker.DoWork("2", 5, TimeSpan.FromMilliseconds(600), progress2, _cancellationTokenSource.Token)
                    .EvaluateFunctionAsync()
                    .ContinueWith(t =>
                        {
                            Status2 = "Completed Ok.";
                            return t;
                        },
                        TaskContinuationOptions.OnlyOnRanToCompletion)
                    .Unwrap();

                var progress3 = new Progress<int>(i => _progress3Collection.Add(i));

                var task3 = DemoWorker.DoWork("3", 6, TimeSpan.FromMilliseconds(200), progress3, _cancellationTokenSource.Token)
                    .EvaluateFunctionAsync()
                    .ContinueWith(t =>
                        {
                            Status3 = "Completed Ok.";
                            return t;
                        },
                        TaskContinuationOptions.OnlyOnRanToCompletion)
                    .Unwrap();

                // Wait for all three tasks to finish
                // ExecuteActionWithTimeoutAsync() handles shared time out, exceptions and cancellation
                var allTasksResponse = await Task.WhenAll(task1, task2, task3)
                    .ExecuteActionAsync(SelectedTimeout, _cancellationTokenSource)
                    .ConfigureAwait(false);

                GlobalStatus = "All tasks completed - " + allTasksResponse.StatusDescription;

                // Update the UI with the result of each task operation - a perAsyncFunctionResponse<string> instance. 
                // The tasks have already completed, but using await again here is better than task.Result
                var response1 = await task1.ConfigureAwait(false);
                var response2 = await task2.ConfigureAwait(false);
                var response3 = await task3.ConfigureAwait(false);

                Status1 = response1.IsCompletedOk
                    ? response1.Data
                    : response1.ToString();

                Status2 = response2.IsCompletedOk
                    ? response2.Data
                    : response2.ToString();

                Status3 = response3.IsCompletedOk
                    ? response3.Data
                    : response3.ToString();
            }

            _cancellationTokenSource = null;
        }

        public ICommand CancelCommand { get; }

        public IEnumerable<perValueDisplayPair<TimeSpan>> AllTimeouts { get; }

        private TimeSpan _selectedTimeout;

        public TimeSpan SelectedTimeout
        {
            get => _selectedTimeout;
            set => Set(nameof(SelectedTimeout), ref _selectedTimeout, value);
        }
    }
}