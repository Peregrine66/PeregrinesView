using Peregrine.WPF.ViewModel;
using Peregrine.WPF.ViewModel.Command;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace DataBindingDemo
{
    public class MainViewModel : perViewModelBase
    {
        public MainViewModel()
        {
            SetReadOnlyPropCommand = new perRelayCommand(OnSetReadOnlyProp);
            AddDataItemCommand = new perRelayCommand(OnAddDataItem);
            DeleteDataItemCommand = new perRelayCommand<DataItemViewModel>(OnDeleteDataItem);

            for (var i = 1; i <= 10; i++)
            {
                OnAddDataItem();
            }
        }

        public string ViewTitle => "WPF Data-Binding Demo";

        private string _readOnlyProp;

        public string ReadOnlyProp
        {
            get => _readOnlyProp;
            private set => Set(nameof(ReadOnlyProp), ref _readOnlyProp, value);
        }

        public ICommand SetReadOnlyPropCommand { get; } 

        private void OnSetReadOnlyProp()
        {
            ReadOnlyProp = "This get-only property was set from the ViewModel.";
        }

        private string _textToDuplicate;

        public string TextToDuplicate
        {
            get => _textToDuplicate;
            set => Set(nameof(TextToDuplicate), ref _textToDuplicate, value);
        }

        public ObservableCollection<DataItemViewModel> DataItems { get; } = new ObservableCollection<DataItemViewModel>();

        private int _nextItemId = 1;

        public ICommand AddDataItemCommand { get; }

        private void OnAddDataItem()
        {
            var item = new DataItemViewModel(_nextItemId++);
            DataItems.Add(item);
        }

        public ICommand DeleteDataItemCommand { get; }

        private void OnDeleteDataItem(DataItemViewModel item)
        {
            DataItems.Remove(item);
        }
    }
}
