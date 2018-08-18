using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Peregrine.WPF.ViewModel.Command;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using ViewOpenCloseDemo.ServiceContracts;

namespace ViewOpenCloseDemo
{
    public class MainViewModel: ObservableObject
    {
        private readonly INavigationService _navigationService;

        public MainViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;

            AllDataItems = Enumerable.Range(1, 20).Select(i => new DataItem(i, "Data Item " + i)).ToList();

            ShowDataItemCommand = new RelayCommand(OnShowDataItem, ()=>SelectedDataItem != null)
                .ObservesInternalProperty(this, nameof(SelectedDataItem));
        }

        public ICollection<DataItem> AllDataItems { get; }

        private DataItem _selectedDataItem;

        public DataItem SelectedDataItem
        {
            get { return _selectedDataItem; }
            set { Set(nameof(SelectedDataItem), ref _selectedDataItem, value); }
        }

        public ICommand ShowDataItemCommand { get; }

        private void OnShowDataItem()
        {
            _navigationService.ShowDataItem(SelectedDataItem);
        }
    }
}
