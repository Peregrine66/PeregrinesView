using Peregrine.Library;
using Peregrine.WPF.ViewModel;
using Peregrine.WPF.ViewModel.Command;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace TaskThrottleDemo
{
    public class MainViewModel : perViewModelBase
    {
        public MainViewModel()
        {
            RunTasksCommand = new perRelayCommandAsync(OnRunTasks);
        }

        public perRelayCommandAsync RunTasksCommand { get; }

        private async Task OnRunTasks()
        {
            var urls = new[]
            {
                "http://www.google.com",
                "http://www.microsoft.com",
                "http://www.stackexchange.com",
                "http://www.tripadvisor.com",
                "http://www.bbc.co.uk",
                "http://www.github.com",
                "http://www.fivethirtyeight.com",
                "http://www.youtube.com",
                "http://www.notavalidurl.xyz"
            };

            Responses.Clear();

            var progress = new Progress<string>();
            progress.ProgressChanged += (s, e) => ProgressDisplay += "\r\n" + e;

            ProgressDisplay = $"{DateTime.Now:HH:mm:ss.fff} - Downloads started";

            var responsesDict = await urls
                .ForEachAsyncThrottled(url => GetWebContent(url, progress), 3)
                .ConfigureAwait(true);

            ProgressDisplay += $"\r\n{DateTime.Now:HH:mm:ss.fff} - Downloads completed";

            var sortedKeys = responsesDict.Keys.OrderBy(x => x).ToList();

            foreach (var key in sortedKeys)
            {
                var output = responsesDict[key].Replace("\r", " ").Replace("\n", " ").Trim().Left(100);
                Responses.Add(new Tuple<string, string>(key, output));
            }
        }

        private string _progressDisplay;

        public string ProgressDisplay
        {
            get => _progressDisplay;
            set => Set(nameof(ProgressDisplay), ref _progressDisplay, value);
        }

        public ObservableCollection<Tuple<string, string>> Responses { get; } = new ObservableCollection<Tuple<string, string>>();

        private static async Task<string> GetWebContent(string url, IProgress<string> progress)
        {
            string result;
            progress.Report($"{DateTime.Now:HH:mm:ss.fff} - Downloading {url}");
            var downloadResult = await perIOAsync
                .ReadAllBytesFromUrlAsync(url)
                .EvaluateFunctionAsync(TimeSpan.FromSeconds(2));

            if (downloadResult.IsCompletedOk)
            {
                result = downloadResult.Data.ToUtf8String();
            }
            else if (downloadResult.IsCancelled)
            {
                result = "Cancelled";
            }
            else if (downloadResult.IsTimedOut)
            {
                result = "Timed Out";
            }
            else
            {
                result = downloadResult.ErrorMessage + "\r\n\r\n" + downloadResult.Exception.GetText();
            }

            progress.Report($"{DateTime.Now:HH:mm:ss.fff} - Completed {url}");

            return result;
        }
    }
}
