using Peregrine.WPF.ViewModel.DialogService.Enums;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Peregrine.WPF.ViewModel.DialogService
{
    public interface IperDialogService
    {
        Task ShowMessageAsync(object viewModel, string body, perDialogIcon dialogIcon = perDialogIcon.Information, string title = "");
        Task<perDialogButton> ShowDialogAsync(object viewModel, perDialogButton buttons, string body, perDialogIcon dialogIcon = perDialogIcon.Information, string title = "" );
        Task<perDialogButton> ShowContentDialogAsync(object viewModel, perDialogButton buttons, perDialogIcon dialogIcon = perDialogIcon.Information, string title = "", string tag = "");
        Task<string> OpenFileDialogAsync(object viewModel, string title = "Open File ...", string initialFolder = "");
        Task<IReadOnlyCollection<string>> OpenFilesDialogAsync(object viewModel, string title = "Open File(s) ...", string initialFolder = "");
        Task<string> SaveFileAsDialogAsync(object viewModel, string title = "Save File As ...", string initialFolder = "", string defaultExt = "", string filter = "");
        Task<string> SelectFolderDialogAsync(object viewModel, string title = "Select Folder ...", string initialFolder = "");
    }
}