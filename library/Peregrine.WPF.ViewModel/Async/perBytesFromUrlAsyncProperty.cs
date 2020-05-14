using Peregrine.Library;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Peregrine.WPF.ViewModel.Async
{
    /// <inheritdoc />
public class perBytesFromUrlAsyncProperty : perAsyncProperty<byte[]>
{
    public perBytesFromUrlAsyncProperty(string url): base(() => FetchData(url))
    {
    }

    private static Task<byte[]> FetchData(string url)
    {
        Debug.WriteLine("downloading Url - " + url);
        return perIOAsync.ReadAllBytesFromUrlRawAsync(url);
    }
}
}