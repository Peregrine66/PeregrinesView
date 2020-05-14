using GalaSoft.MvvmLight;
using Peregrine.WPF.ViewModel.Command;
using System.Windows.Input;

namespace ViewOpenCloseDemo
{
    public class DataItemViewModel: ObservableObject
    {
        public DataItemViewModel(DataItem model)
        {
            Model = model;
            CloseViewCommand = new perRelayCommand(() => ViewClosed = true);
        }

        public DataItem Model { get; }

        private bool? _viewClosed;

        public bool? ViewClosed
        {
            get => _viewClosed;
            set => Set(nameof(ViewClosed), ref _viewClosed, value);
        }

        public ICommand CloseViewCommand { get; }
    }
}
