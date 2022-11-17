using Peregrine.Library;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Peregrine.WPF.ViewModel.Async
{
    /// <inheritdoc />
    public class perBytesFromFileAsyncProperty : perAsyncProperty<byte[]>
    {
        private readonly string _filePath;

        public perBytesFromFileAsyncProperty(string filePath)
        {
            _filePath = filePath;
        }

        protected override async Task<byte[]> FetchValue()
        {
            Debug.WriteLine("reading file - " + _filePath);

            var response = await perIOAsync.ReadAllBytesFromFileAsync(_filePath)
                .EvaluateFunctionAsync(FetchValueTimeOut)
                .ConfigureAwait(false);

            return response.IsCompletedOk
                ? response.Data
                : null;
        }
    }
}