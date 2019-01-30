using Peregrine.WPF.ViewModel;

namespace StaffManager.ViewModel.Messages
{
    public class SelectItemMessage
    {
        public SelectItemMessage(perTreeViewItemViewModelBase selectedItem)
        {
            SelectedItem = selectedItem;
        }

        public perTreeViewItemViewModelBase SelectedItem { get; }
    }
}
