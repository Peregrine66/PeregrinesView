using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Threading;
using Peregrine.WPF.ViewModel.Helpers;
using Peregrine.Library;

namespace Peregrine.WPF.ViewModel
{
    public class perTreeViewItemViewModelBase : perViewModelBase
    {
        // a dummy item used in lazy loading mode, ensuring that each node has at least one child so that the expand button is shown
        private static perTreeViewItemViewModelBase LoadingDataItem { get; }

        static perTreeViewItemViewModelBase()
        {
            LoadingDataItem = new perTreeViewItemViewModelBase { Caption = "Loading Data ..." };
        }

        private readonly perObservableCollection<perTreeViewItemViewModelBase> _childrenList = new perObservableCollection<perTreeViewItemViewModelBase>();

        public perTreeViewItemViewModelBase(bool addLoadingDataItem = false)
        {
            if (addLoadingDataItem)
                _childrenList.Add(LoadingDataItem);
        }

        private string _caption;

        public string Caption
        {
            get { return _caption; }
            set { Set(nameof(Caption), ref _caption, value); }
        }

        public void ClearChildren()
        {
            _childrenList.Clear();
        }

        public void AddChild(perTreeViewItemViewModelBase child)
        {
            if (_childrenList.Any() && _childrenList.First() == LoadingDataItem)
                ClearChildren();

            _childrenList.Add(child);
            SetChildPropertiesFromParent(child);
        }

        protected void SetChildPropertiesFromParent(perTreeViewItemViewModelBase child)
        {
            child.Parent = this;

            if (IsChecked.GetValueOrDefault())
                child.IsChecked = true;
        }

        public void AddChildren(IEnumerable<perTreeViewItemViewModelBase> children)
        {
            foreach (var child in children)
                AddChild(child);
        }

        protected perTreeViewItemViewModelBase Parent { get; private set; }

        private bool? _isChecked = false;

        public bool? IsChecked
        {
            get { return _isChecked; }
            set
            {
                if (Set(nameof(IsChecked), ref _isChecked, value))
                {
                    foreach (var child in Children)
                        if (child.IsEnabled)
                            child.SetIsCheckedIncludingChildren(value);

                    SetParentIsChecked();
                }
            }
        }

        private bool _isExpanded;

        public bool IsExpanded
        {
            get { return _isExpanded; }
            set
            {
                if (!Set(nameof(IsExpanded), ref _isExpanded, value) || IsInitialised || IsInitialising)
                    return;

                var unused = InitialiseAsync();
            }
        }

        private bool _isEnabled = true;

        public bool IsEnabled
        {
            get { return _isEnabled; }
            set { Set(nameof(IsEnabled), ref _isEnabled, value); }
        }

        public bool IsInitialising { get; private set; }
        public bool IsInitialised { get; private set; }

        public async Task InitialiseAsync()
        {
            if (IsInitialised || IsInitialising)
                return;

            IsInitialising = true;
            await InitialiseChildrenAsync().ConfigureAwait(false);
            foreach (var child in InitialisedChildren)
                SetChildPropertiesFromParent(child);
            IsInitialised = true;
            RaisePropertyChanged(nameof(Children));
        }

        protected virtual Task InitialiseChildrenAsync()
        {
            return Task.CompletedTask;
        }

        public IEnumerable<perTreeViewItemViewModelBase> Children => IsInitialised
            ? InitialisedChildren
            : _childrenList;

        // override this as required in descendent classes
        // e.g. if Children is a union of multiple child item collections which are populated in InitialiseChildrenAsync()
        protected virtual IEnumerable<perTreeViewItemViewModelBase> InitialisedChildren => _childrenList;

        private bool _isSelected;

        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                // ensure that all ancestor items are expanded, so this item will be visible
                if (value)
                {
                    var parent = Parent;
                    while (parent != null)
                    {
                        parent.IsExpanded = true;
                        parent = parent.Parent;
                    }
                }

                if (_isSelected == value)
                    return;

                // use DispatcherPriority.ContextIdle so that we wait for any children of newly expanded items to be fully created in the
                // parent TreeView, before setting IsSelected for this item (which will scroll it into view - see perTreeViewItemHelper)
                perDispatcherHelper.CheckBeginInvokeOnUI(() => Set(nameof(IsSelected), ref _isSelected, value), DispatcherPriority.ContextIdle);

                // note that by rule, a TreeView can only have one selected item, but this is handled automatically by 
                // the control - we aren't required to manually unselect the previously selected item.
            }
        }

        private void SetIsCheckedIncludingChildren(bool? value)
        {
            _isChecked = value;
            RaisePropertyChanged(nameof(IsChecked));

            foreach (var child in Children)
                if (child.IsEnabled)
                    child.SetIsCheckedIncludingChildren(value);
        }

        private void SetIsCheckedThisItemOnly(bool? value)
        {
            _isChecked = value;
            RaisePropertyChanged(nameof(IsChecked));
        }

        private void SetParentIsChecked()
        {
            var parent = Parent;

            while (parent != null)
            {
                var hasIndeterminateChild = parent.Children.Any(c => c.IsEnabled && !c.IsChecked.HasValue);

                if (hasIndeterminateChild)
                    parent.SetIsCheckedThisItemOnly(null);
                else
                {
                    var hasSelectedChild = parent.Children.Any(c => c.IsEnabled && c.IsChecked.GetValueOrDefault());
                    var hasUnselectedChild = parent.Children.Any(c => c.IsEnabled && !c.IsChecked.GetValueOrDefault());

                    if (hasUnselectedChild && hasSelectedChild)
                        parent.SetIsCheckedThisItemOnly(null);
                    else
                        parent.SetIsCheckedThisItemOnly(hasSelectedChild);
                }

                parent = parent.Parent;
            }
        }

        public override string ToString()
        {
            return Caption;
        }
    }
}