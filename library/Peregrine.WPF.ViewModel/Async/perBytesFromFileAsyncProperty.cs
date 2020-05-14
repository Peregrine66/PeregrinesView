using Peregrine.Library;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Peregrine.WPF.ViewModel.Async
{
    /// <inheritdoc />
    public class perBytesFromFileAsyncProperty : perAsyncProperty<byte[]>
    {
        public perBytesFromFileAsyncProperty(string filePath) : base(() => FetchData(filePath))
        {
        }

        private static Task<byte[]> FetchData(string filePath)
        {
            Debug.WriteLine("loading file - " + filePath);
            return perIOAsync.ReadAllBytesFromFileRawAsync(filePath);
        }
    }
}