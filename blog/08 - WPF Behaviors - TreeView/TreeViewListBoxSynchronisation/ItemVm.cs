using System;
using System.Threading.Tasks;
using Peregrine.WPF.ViewModel;

namespace TreeViewListBoxSynchronisation
{
    public class ItemVm : perTreeViewItemViewModelBase, IComparable<ItemVm>
    {
        public ItemVm(ItemModel model)
        {
            Model = model;
            Caption = model.Caption;

            SetLazyLoadingMode();
        }

        public ItemModel Model { get; }

        protected override Task LazyLoadFetchChildren()
        {
            var children = ItemVmFactory.CreateItemVms( Model.Caption, Model.Level + 1);
            AddChildren(children);

            return Task.CompletedTask;
        }

        public int CompareTo(ItemVm other)
        {
            return string.Compare(Caption, other.Caption, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
