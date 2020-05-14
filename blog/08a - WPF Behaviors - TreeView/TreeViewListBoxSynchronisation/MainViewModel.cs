using Peregrine.Library;
using Peregrine.WPF.ViewModel;
using Peregrine.WPF.ViewModel.Command;
using System.Collections.Generic;
using System.Windows.Input;

namespace TreeViewListBoxSynchronisation
{
    public class MainViewModel : perViewModelBase
    {
        public MainViewModel()
        {
            LoadedCommand = new perRelayCommand(OnLoaded);
            InitialiseSelectedItemCommand = new perRelayCommand(OnInitialiseSelectedItem);
        }

        public ICommand LoadedCommand { get; }

        private void OnLoaded()
        {
            var rootItems = ItemVmFactory.CreateItemVms("Item ", 0);
            _rootItemVms.Clear();
            _rootItemVms.AddRange(rootItems);
        }

        private readonly perObservableCollection<ItemVm> _rootItemVms = new perObservableCollection<ItemVm>();
        public IEnumerable<ItemVm> RootItemVms => _rootItemVms;

        public IEnumerable<ItemVm> AllItemVms => ItemVmFactory.AllItemVms;

        private ItemVm _selectedItem;

        public ItemVm SelectedItem
        {
            get => _selectedItem;
            set => Set(nameof(SelectedItem), ref _selectedItem, value);
        }

        public ICommand InitialiseSelectedItemCommand { get; }

        private void OnInitialiseSelectedItem()
        {
            if (SelectedItem != null)
                SelectedItem.IsExpanded = !SelectedItem.IsExpanded;
        }
    }
}