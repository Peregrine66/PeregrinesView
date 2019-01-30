using GalaSoft.MvvmLight;
using Peregrine.WPF.ViewModel.Command;
using System;
using System.Threading.Tasks;

namespace BusySpinnerDemo
{

    public class MainViewModel : ViewModelBase
    {
        public MainViewModel()
        {
            var progress = new Progress<int>(i => TaskProgress = i);
            DoSomethingAsyncCommand = new perRelayCommandAsync(() => OnDoSomethingAsync(progress));
        }

        public perRelayCommandAsync DoSomethingAsyncCommand { get; }

        private async Task OnDoSomethingAsync(IProgress<int> progress)
        {
            for (var i = 0; i < 10; i++)
            {
                progress.Report(i * 10);
                await Task.Delay(TimeSpan.FromSeconds(0.5)).ConfigureAwait(false);
            }
        }

        private int _taskProgress;

        public int TaskProgress
        {
            get => _taskProgress;
            set => Set(nameof(TaskProgress), ref _taskProgress, value);
        }
    }
}
