using System.Threading.Tasks;

namespace Peregrine.WPF.ViewModel
{
    public interface IAllowClose
    {
        Task<bool> AllowCloseAsync();
    }
}
