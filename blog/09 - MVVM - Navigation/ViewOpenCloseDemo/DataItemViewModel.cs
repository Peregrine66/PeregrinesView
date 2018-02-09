using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System.Windows.Input;

namespace ViewOpenCloseDemo
{
    public class DataItemViewModel: ObservableObject
    {
        public DataItemViewModel(DataItem model)
        {
            Model = model;
            CloseViewCommand = new RelayCommand(() => ViewClosed = true);
        }

        public DataItem Model { get; }

        private bool? _viewClosed;

        public bool? ViewClosed
        {
            get { return _viewClosed; }
            set { Set(nameof(ViewClosed), ref _viewClosed, value); }
        }

        public ICommand CloseViewCommand { get; }
    }
}
