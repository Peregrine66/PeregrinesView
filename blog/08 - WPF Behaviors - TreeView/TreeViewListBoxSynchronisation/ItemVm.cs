using System;
using System.Threading.Tasks;
using Peregrine.WPF.ViewModel;

namespace TreeViewListBoxSynchronisation
{
    public class ItemVm : perTreeViewItemViewModelBase, IComparable<ItemVm>
    {
        public ItemVm(ItemModel model): base(true)
        {
            Model = model;
            Caption = model.Caption;
        }

        public ItemModel Model { get; }

        protected override Task InitialiseChildrenAsync()
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
