using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight.Ioc;
using Peregrine.WPF.ViewModel;
using Peregrine.WPF.ViewModel.Command;
using Peregrine.WPF.ViewModel.DialogService;

namespace FileSystemDialogsDemo
{
    public class MainViewModel: perViewModelBase
    {
        private readonly IperDialogService _dialogService;

        // for Xaml designer only
        public MainViewModel()
        {
        }

        [PreferredConstructor]
        public MainViewModel(IperDialogService dialogService)
        {
            _dialogService = dialogService;

            OpenFileCommand = new perRelayCommandAsync(OnOpenFile);
            OpenFilesCommand = new perRelayCommandAsync(OnOpenFiles);
            SelectFolderCommand = new perRelayCommandAsync(OnSelectFolder);
            SaveAsCommand = new perRelayCommandAsync(OnSaveAs);
        }

        public ICommand OpenFileCommand { get; }

        private async Task OnOpenFile()
        {
            OpenFileResponse = await _dialogService.OpenFileDialogAsync(this).ConfigureAwait(false);
        }

        private string _openFileResponse;

        public string OpenFileResponse
        {
            get => _openFileResponse;
            set => Set(nameof(OpenFileResponse), ref _openFileResponse, value);
        }

        public ICommand OpenFilesCommand { get; }

        private async Task OnOpenFiles()
        {
            var response = await _dialogService.OpenFilesDialogAsync(this).ConfigureAwait(false);
            OpenFilesResponse = string.Join("\r\n", response);
        }

        private string _openFilesResponse;

        public string OpenFilesResponse
        {
            get => _openFilesResponse;
            set => Set(nameof(OpenFilesResponse), ref _openFilesResponse, value);
        }

        public ICommand SelectFolderCommand { get; }

        private async Task OnSelectFolder()
        {
            SelectFolderResponse = await _dialogService.SelectFolderDialogAsync(this).ConfigureAwait(false);
        }

        private string _selectFolderResponse;

        public string SelectFolderResponse
        {
            get => _selectFolderResponse;
            set => Set(nameof(SelectFolderResponse), ref _selectFolderResponse, value);
        }

        public ICommand SaveAsCommand { get; }

        private async Task OnSaveAs()
        {
            SaveAsResponse = await _dialogService.SaveFileAsDialogAsync(this).ConfigureAwait(false);
        }

        private string _saveAsResponse;

        public string SaveAsResponse
        {
            get => _saveAsResponse;
            set => Set(nameof(SaveAsResponse), ref _saveAsResponse, value);
        }
    }
}
