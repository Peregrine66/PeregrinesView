using System;
using System.Threading.Tasks;
using Peregrine.WPF.ViewModel;

namespace TreeViewListBoxSynchronisation
{
    using System.Linq;

    public class ItemVm : perTreeViewItemViewModelBase, IComparable<ItemVm>
    {
        public ItemVm(ItemModel model)
        {
            Model = model;
            Caption = model.Caption;

            SetLazyLoadingMode();
        }

        public ItemModel Model { get; }

        protected override Task<perTreeViewItemViewModelBase[]> LazyLoadFetchChildren()
        {
            var children = ItemVmFactory.CreateItemVms( Model.Caption, Model.Level + 1)
                .Cast<perTreeViewItemViewModelBase>()
                .ToArray();
           return Task.FromResult(children);
        }

        public int CompareTo(ItemVm other)
        {
            return string.Compare(Caption, other.Caption, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
