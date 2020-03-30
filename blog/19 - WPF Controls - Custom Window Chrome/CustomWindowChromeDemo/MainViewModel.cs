using Peregrine.Library;
using Peregrine.WPF.ViewModel;
using Peregrine.WPF.ViewModel.Command;
using Peregrine.WPF.ViewModel.DialogService;
using Peregrine.WPF.ViewModel.DialogService.Enums;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace CustomWindowChromeDemo
{
    public class MainViewModel : perViewModelBase, IAllowClose
    {
        private readonly IperDialogService _dialogService;

        public MainViewModel(IperDialogService dialogService)
        {
            _dialogService = dialogService;
            AllResizeModes = perEnumHelper.MakeValueDisplayPairs<ResizeMode>();
            ShowMessageDialogCommand = new perRelayCommandAsync(OnShowMessageDialogAsync);
        }

        private bool _canClose = true;

        public bool CanClose
        {
            get => _canClose;
            set => Set(nameof(CanClose), ref _canClose, value);
        }

        public IEnumerable<perValueDisplayPair<ResizeMode>> AllResizeModes { get; }

        private ResizeMode _selectedResizeMode = ResizeMode.NoResize;

        public ResizeMode SelectedResizeMode
        {
            get => _selectedResizeMode;
            set => Set(nameof(SelectedResizeMode), ref _selectedResizeMode, value);
        }

        public ICommand ShowMessageDialogCommand { get; }

        private Task OnShowMessageDialogAsync()
        {
            return _dialogService.ShowMessageAsync(this,
                "A message dialog displayed using the<lb><b>custom</b> window format",
                perDialogIcon.Asterisk,
                "Here's a message ...");
        }

        public async Task<bool> AllowCloseAsync()
        {
            // usually you would add some logic relating to ViewModel properties here

            var dialogResult = await _dialogService.ShowDialogAsync(
                this,
                perDialogButton.YesNo,
                "Do you want to close the demo application?",
                perDialogIcon.Question,
                "Close").ConfigureAwait(false);

            return dialogResult == perDialogButton.Yes;
        }
    }
}
