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
        private static perTreeViewItemViewModelBase LazyLoadingChildIndicator { get; }

        private bool _inLazyLoadingMode;

        static perTreeViewItemViewModelBase()
        {
            LazyLoadingChildIndicator = new perTreeViewItemViewModelBase { Caption = "Loading Data ..." };
        }

        private readonly perObservableCollection<perTreeViewItemViewModelBase> _childrenList = new perObservableCollection<perTreeViewItemViewModelBase>();

        // LazyLoadingChildIndicator ensures a visble expansion toggle button in lazy loading mode
        protected void SetLazyLoadingMode()
        {
            _childrenList.Clear();
            _childrenList.Add(LazyLoadingChildIndicator);

            LazyLoadCompleted = false;
            LazyLoadExecuting = false;
            _inLazyLoadingMode = true;

            IsExpanded = false;
        }

        private string _caption;

        public string Caption
        {
            get => _caption;
            set => Set(nameof(Caption), ref _caption, value);
        }

        public void ClearChildren()
        {
            _childrenList.Clear();
        }

        public void AddChild(perTreeViewItemViewModelBase child)
        {
            if (_childrenList.Any() && _childrenList.First() == LazyLoadingChildIndicator)
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
            get => _isChecked;
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
            get => _isExpanded;
            set
            {
                if (!Set(nameof(IsExpanded), ref _isExpanded, value) || LazyLoadExecuting || LazyLoadCompleted || !_inLazyLoadingMode)
                    return;

                var unused = DoLazyLoadAsync();
            }
        }

        private bool _isEnabled = true;

        public bool IsEnabled
        {
            get => _isEnabled;
            set => Set(nameof(IsEnabled), ref _isEnabled, value);
        }

        public bool LazyLoadCompleted { get; private set; }
        public bool LazyLoadExecuting { get; private set; }

        public async Task DoLazyLoadAsync()
        {
            if (LazyLoadExecuting || LazyLoadCompleted)
                return;

            LazyLoadExecuting = true;
            await LazyLoadFetchChildren().ConfigureAwait(false);
            foreach (var child in LazyLoadChildren)
                SetChildPropertiesFromParent(child);
            LazyLoadCompleted = true;
            RaisePropertyChanged(nameof(Children));
        }

        protected virtual Task LazyLoadFetchChildren()
        {
            return Task.CompletedTask;
        }

        public IEnumerable<perTreeViewItemViewModelBase> Children => LazyLoadCompleted
            ? LazyLoadChildren
            : _childrenList;

        // override this as required in descendent classes
        // e.g. if Children is formed from a union of multiple internal child item collections (of different types) which are populated in LazyLoadFetchChildren()
        protected virtual IEnumerable<perTreeViewItemViewModelBase> LazyLoadChildren => _childrenList;

        private bool _isSelected;

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                // build a priority queue of dispatcher operations

                // All operations relating to tree item expansion are added with priority = DispatcherPriority.ContextIdle, so that they are
                // sorted before any operations relating to selection (which have priority = DispatcherPriority.ApplicationIdle).
                // This ensures that the visual container for all items are created before any selection operation is carried out.
                // First expand all ancestors of the selected item - those closest to the root first
                // Expanding a node will scroll as many of its children as possible into view - see perTreeViewItemHelper, but these scrolling
                // operations will be added to the queue after all of the parent expansions.
                if (value)
                {
                    var ancestorsToExpand = new Stack<perTreeViewItemViewModelBase>();

                    var parent = Parent;
                    while (parent != null)
                    {
                        if (!parent.IsExpanded)
                            ancestorsToExpand.Push(parent);

                        parent = parent.Parent;
                    }

                    while (ancestorsToExpand.Any())
                    {
                        var parentToExpand = ancestorsToExpand.Pop();
                        perDispatcherHelper.AddToQueue(() => parentToExpand.IsExpanded = true, DispatcherPriority.ContextIdle);
                    }
                }

                if (_isSelected == value)
                    return;

                // Set the item's selected state - use DispatcherPriority.ApplicationIdle so this operation is executed after all
                // expansion operations, no matter when they were added to the queue.
                // Selecting a node will also scroll it into view - see perTreeViewItemHelper
                perDispatcherHelper.AddToQueue(() => Set(nameof(IsSelected), ref _isSelected, value), DispatcherPriority.ApplicationIdle);

                // note that by rule, a TreeView can only have one selected item, but this is handled automatically by 
                // the control - we aren't required to manually unselect the previously selected item.

                // execute all of the queued operations in descending DipatecherPriority order (expansion before selection)
                var unused = perDispatcherHelper.ProcessQueueAsync();
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

        public int ChildCount => Children.Count() + Children.Sum(c => c.ChildCount);
    }
}