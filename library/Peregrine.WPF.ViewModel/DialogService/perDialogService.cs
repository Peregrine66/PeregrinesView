using System.Threading.Tasks;
using Peregrine.WPF.ViewModel.DialogService.Enums;

namespace Peregrine.WPF.ViewModel.DialogService
{
    public interface IperDialogService
    {
        Task ShowMessageAsync(object viewModel, string body, perDialogIcon dialogIcon = perDialogIcon.Information, string title = "");
        Task<perDialogButton> ShowDialogAsync(object viewModel, perDialogButton buttons, string body, perDialogIcon dialogIcon = perDialogIcon.Information, string title = "" );
        Task<perDialogButton> ShowContentDialogAsync(object viewModel, perDialogButton buttons, perDialogIcon dialogIcon = perDialogIcon.Information, string title = "", string tag = "");
    }
}