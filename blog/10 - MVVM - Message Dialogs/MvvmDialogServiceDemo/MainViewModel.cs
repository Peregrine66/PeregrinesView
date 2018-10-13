using GalaSoft.MvvmLight.Ioc;
using Peregrine.Library;
using Peregrine.WPF.ViewModel;
using Peregrine.WPF.ViewModel.Command;
using Peregrine.WPF.ViewModel.DialogService;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using Peregrine.WPF.ViewModel.DialogService.Enums;

namespace MvvmDialogServiceDemo
{
    public class MainViewModel : perViewModelBase
    {
        private readonly IperDialogService _dialogService;

        public const string RED_CIRCLE = "RedCircle";
        public const string GREEN_SQUARE = "GreenSquare";

        // for Xaml designer only
        public MainViewModel()
        {
        }

        [PreferredConstructor]
        public MainViewModel(IperDialogService dialogService)
        {
            _dialogService = dialogService;
            ShowDialogCommand = new perRelayCommandAsync(OnShowDialogAsync, () => SelectedDialogIndex > 0)
                .ObservesInternalProperty(this, nameof(SelectedDialogIndex));

            AllDialogTypes = new List<perValueDisplayPair<int>>
            {
                1.CreateValueDisplayPair("Simple Message Dialog"),
                2.CreateValueDisplayPair("Message Dialog with a Choice of Buttons"),
                3.CreateValueDisplayPair("Xaml content: Red Circle"),
                4.CreateValueDisplayPair("Xaml content: Green Square + Icon")
            };
        }

        public ICollection<perValueDisplayPair<int>> AllDialogTypes { get; }

        private int _selectedDialogIndex;

        public int SelectedDialogIndex
        {
            get => _selectedDialogIndex;
            set => Set(nameof(SelectedDialogIndex), ref _selectedDialogIndex, value);
        }

        public ICommand ShowDialogCommand { get; }

        private async Task OnShowDialogAsync()
        {
            switch (SelectedDialogIndex)
            {
                case 1:
                    await _dialogService.ShowMessageAsync(this, "Hello from the dialog service.", perDialogIcon.Information, "Mvvm Dialog Service").ConfigureAwait(false);
                    break;

                case 2:
                    var response = await _dialogService.ShowDialogAsync(this, perDialogButton.YesNo, "Do you want to continue?", perDialogIcon.Question, "Mvvm Dialog Service").ConfigureAwait(false);
                    await _dialogService.ShowMessageAsync(this, "You clicked on\r\n\r\n" + response, perDialogIcon.Information, "Mvvm Dialog Service").ConfigureAwait(false);
                    break;
                case 3:
                    await _dialogService.ShowContentDialogAsync(this, perDialogButton.Ok, perDialogIcon.None, "Xaml Content Dialog", RED_CIRCLE).ConfigureAwait(false);
                    break;
                case 4:
                    await _dialogService.ShowContentDialogAsync(this, perDialogButton.Ok, perDialogIcon.Information, "Xaml Content Dialog with Icon", GREEN_SQUARE).ConfigureAwait(false);
                    break;
            }
        }

        public string RedCircleDescription => "Red Circle";
        public string GreenSquareDescription => "Green Square";
    }
}