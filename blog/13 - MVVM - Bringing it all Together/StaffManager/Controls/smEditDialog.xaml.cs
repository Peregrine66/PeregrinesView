using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Peregrine.WPF.ViewModel.Command;
using Peregrine.WPF.ViewModel.DialogService;
using Peregrine.WPF.ViewModel.DialogService.Enums;
using StaffManager.ViewModel;
using System.Threading.Tasks;
using System.Windows.Input;

namespace StaffManager.Controls
{
    public partial class smEditDialog
    {
        public smEditDialog()
        {
            InitializeComponent();
        }
    }

    public class smEditDialogViewModel : ViewModelBase
    {
        private readonly IperDialogService _dialogService;

        public smEditDialogViewModel(IperDialogService dialogService)
        {
            _dialogService = dialogService;

            CancelCommand = new perRelayCommandAsync(OnCancelAsync);
        }

        private smViewModelBase _editingViewModel;

        /// <summary>
        /// The wrapper ViewModel for the Model item being edited.
        /// </summary>
        public smViewModelBase EditingViewModel
        {
            get => _editingViewModel;
            set
            {
                if (Set(nameof(EditingViewModel), ref _editingViewModel, value) && value != null)
                {
                    SaveCommand = new RelayCommand(() => ViewClosed = true, () => EditingViewModel.IsDirty && EditingViewModel.IsValid)
                        .ObservesExternalProperty(EditingViewModel, nameof(smViewModelBase.IsDirty))
                        .ObservesExternalProperty(EditingViewModel, nameof(smViewModelBase.IsValid));
                }
            }
        }

        public ICommand SaveCommand { get; private set; }

        public ICommand CancelCommand { get; }

        private async Task OnCancelAsync()
        {
            if (EditingViewModel.IsDirty)
            {
                var cancelDialogResult = await _dialogService.ShowDialogAsync(this, perDialogButton.YesNo, "Abandon changes", perDialogIcon.Question).ConfigureAwait(false);
                if (cancelDialogResult == perDialogButton.Yes)
                    ViewClosed = false;
            }
            else
                ViewClosed = false;
        }

        private bool? _viewClosed;

        public bool? ViewClosed
        {
            get => _viewClosed;
            set => Set(nameof(ViewClosed), ref _viewClosed, value);
        }

    }
}
