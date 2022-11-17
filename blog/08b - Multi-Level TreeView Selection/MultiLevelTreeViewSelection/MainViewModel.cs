using GalaSoft.MvvmLight;
using Peregrine.WPF.ViewModel;
using Peregrine.WPF.ViewModel.Command;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace MultiLevelTreeViewSelection
{
    public class MainViewModel: ViewModelBase
    {
        public const int ItemCount = 25;

        public MainViewModel()
        {
            var rootNode = new perTreeViewItemViewModelBase { Caption = "Root Node" };
            _rootItems.Add(rootNode);

            for (var i = 1; i <= ItemCount; i++)
            {
                var child1 = new perTreeViewItemViewModelBase { Caption = "Item " + i };
                rootNode.AddChild(child1);

                for (var j = 1; j <= ItemCount; j++)
                {
                    var child2 = new perTreeViewItemViewModelBase { Caption = child1.Caption + "." + j };
                    child1.AddChild(child2);

                    for (var k = 1; k <= ItemCount; k++)
                    {
                        var child3 = new perTreeViewItemViewModelBase { Caption = child2.Caption + "." + k };
                        child2.AddChild(child3);

                        for (var l = 1; l <= ItemCount; l++)
                        {
                            var child4 = new perTreeViewItemViewModelBase { Caption = child3.Caption + "." + l };
                            child3.AddChild(child4);
                        }
                    }
                }
            }

            SelectItemCommand = new perRelayCommand(OnSelectItem);
        }

        private readonly ObservableCollection<perTreeViewItemViewModelBase> _rootItems = new ObservableCollection<perTreeViewItemViewModelBase>();

        public IEnumerable<perTreeViewItemViewModelBase> RootItems => _rootItems;

        private int _index1;

        public int Index1
        {
            get => _index1;
            set
            {
                Set(nameof(Index1), ref _index1, value);

                if (value == 0)
                {
                    Index2 = 0;
                }
            }
        }

        private int _index2;

        public int Index2
        {
            get => _index2;
            set
            {
                Set(nameof(Index2), ref _index2, value);

                if (value == 0)
                {
                    Index3 = 0;
                }
            }
        }

        private int _index3;

        public int Index3
        {
            get => _index3;
            set
            {
                Set(nameof(Index3), ref _index3, value);

                if (value == 0)
                {
                    Index4 = 0;
                }
            }
        }

        private int _index4;

        public int Index4
        {
            get => _index4;
            set => Set(nameof(Index4), ref _index4, value);
        }

        public ICommand SelectItemCommand { get; }

        private void OnSelectItem()
        {
            var selectedItem = _rootItems.First();

            if (Index1 > 0)
            {
                // have to use Skip() / Take() here as Children is just IEnumerable<> - so can't be indexed
                selectedItem = selectedItem.Children.Skip(Index1 - 1).Take(1).FirstOrDefault();

                if (Index2 > 0 && selectedItem != null)
                {
                    selectedItem = selectedItem.Children.Skip(Index2 - 1).Take(1).FirstOrDefault();

                    if (Index3 > 0 && selectedItem != null)
                    {
                        selectedItem = selectedItem.Children.Skip(Index3 - 1).Take(1).FirstOrDefault();

                        if (Index4 > 0 && selectedItem != null)
                        {
                            selectedItem = selectedItem.Children.Skip(Index4 - 1).Take(1).FirstOrDefault();
                        }
                    }
                }
            }

            SelectedItem = selectedItem;
        }

        private perTreeViewItemViewModelBase _selectedItem;

        public perTreeViewItemViewModelBase SelectedItem
        {
            get => _selectedItem;
            set => Set(nameof(SelectedItem), ref _selectedItem, value);
        }
    }
}
