using Peregrine.Library;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Peregrine.WPF.ViewModel.Async
{
    /// <inheritdoc />
    public class perBytesFromUrlAsyncProperty : perAsyncProperty<byte[]>
    {
        private readonly string _url;

        public perBytesFromUrlAsyncProperty(string url)
        {
            _url = url;
        }

        protected override Task<byte[]> FetchValue()
        {
            Debug.WriteLine("Downloading ... " + _url);

            return perIOAsync.ReadAllBytesFromUrlAsync(_url);
        }
    }
}