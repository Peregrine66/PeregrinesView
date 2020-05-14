using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Threading;
using Peregrine.WPF.ViewModel.Helpers;
using Peregrine.Library;
using System;

namespace Peregrine.WPF.ViewModel
{
    /// <summary>
    /// A base class for items that can be displayed in a TreeView or other hierarchical display
    /// </summary>
    public class perTreeViewItemViewModelBase : perViewModelBase
    {
        // a dummy item used in lazy loading mode, ensuring that each node has at least one child so that the expand button is shown
        private static perTreeViewItemViewModelBase LazyLoadingChildIndicator { get; } 
            = new perTreeViewItemViewModelBase { Caption = "Loading Data ..." };

        private bool InLazyLoadingMode { get; set; }
        private bool LazyLoadTriggered { get; set; }
        private bool LazyLoadCompleted { get; set; }
        private bool RequiresLazyLoad => InLazyLoadingMode && !LazyLoadTriggered;

        // Has Children been overridden (e.g. to point at some private internal collection) 
        private bool LazyLoadChildrenOverridden => InLazyLoadingMode && !Equals(LazyLoadChildren, _childrenList);

        private readonly perObservableCollection<perTreeViewItemViewModelBase> _childrenList 
            = new perObservableCollection<perTreeViewItemViewModelBase>();

        /// <summary>
        /// LazyLoadingChildIndicator ensures a visible expansion toggle button in lazy loading mode 
        /// </summary>
        protected void SetLazyLoadingMode()
        {
            ClearChildren();
            _childrenList.Add(LazyLoadingChildIndicator);

            IsExpanded = false;
            InLazyLoadingMode = true;
            LazyLoadTriggered = false;
            LazyLoadCompleted = false;
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

        /// <summary>
        /// Add a new child item to this TreeView item
        /// </summary>
        /// <param name="child"></param>
        public void AddChild(perTreeViewItemViewModelBase child)
        {
            if (LazyLoadChildrenOverridden)
                throw new InvalidOperationException("Don't call AddChild for an item with LazyLoad mode set & LazyLoadChildren has been overridden");

            if (_childrenList.Any() && _childrenList.First() == LazyLoadingChildIndicator)
                _childrenList.Clear();

            _childrenList.Add(child);

            SetChildPropertiesFromParent(child);
        }

        protected void SetChildPropertiesFromParent(perTreeViewItemViewModelBase child)
        { 
            child.Parent = this;

            // if this node is checked then all new children added are set checked 
            if (IsChecked.GetValueOrDefault())
                child.SetIsCheckedIncludingChildren(true);

            ReCalculateNodeCheckState();
        }

        protected void ReCalculateNodeCheckState()
        {
            var item = this;

            while (item != null)
            {
                if (item.Children.Any() && !Equals(item.Children.FirstOrDefault(), LazyLoadingChildIndicator))
                {
                    var hasIndeterminateChild = item.Children.Any(c => c.IsEnabled && !c.IsChecked.HasValue);

                    if (hasIndeterminateChild)
                        item.SetIsCheckedThisItemOnly(null);
                    else
                    {
                        var hasSelectedChild = item.Children.Any(c => c.IsEnabled && c.IsChecked.GetValueOrDefault());
                        var hasUnselectedChild = item.Children.Any(c => c.IsEnabled && !c.IsChecked.GetValueOrDefault());

                        if (hasUnselectedChild && hasSelectedChild)
                            item.SetIsCheckedThisItemOnly(null);
                        else
                            item.SetIsCheckedThisItemOnly(hasSelectedChild);
                    }
                }

                item = item.Parent;
            }
        }

        private void SetIsCheckedIncludingChildren(bool? value)
        {
            if (IsEnabled)
            {
                _isChecked = value;
                RaisePropertyChanged(nameof(IsChecked));

                foreach (var child in Children)
                    if (child.IsEnabled)
                        child.SetIsCheckedIncludingChildren(value);
            }
        }

        private void SetIsCheckedThisItemOnly(bool? value)
        {
            _isChecked = value;
            RaisePropertyChanged(nameof(IsChecked));
        }

        /// <summary>
        /// Add multiple children to this TreeView item
        /// </summary>
        /// <param name="children"></param>
        public void AddChildren(IEnumerable<perTreeViewItemViewModelBase> children)
        {
            foreach (var child in children)
                AddChild(child);
        }

        /// <summary>
        /// Remove a child item from this TreeView item
        /// </summary>
        public void RemoveChild(perTreeViewItemViewModelBase child)
        {
            _childrenList.Remove(child);
            child.Parent = null;

            ReCalculateNodeCheckState();
        }

        public perTreeViewItemViewModelBase Parent { get; private set; }

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

                    Parent?.ReCalculateNodeCheckState();
                }
            }
        }

        private bool _isExpanded;

        public bool IsExpanded
        {
            get => _isExpanded;
            set
            {
                if (Set(nameof(IsExpanded), ref _isExpanded, value) && value && RequiresLazyLoad)
                    TriggerLazyLoading();
            }
        }

        private bool _isEnabled = true;

        public bool IsEnabled
        {
            get => _isEnabled;
            set => Set(nameof(IsEnabled), ref _isEnabled, value);
        }

        public void TriggerLazyLoading()
        {
            var unused = DoLazyLoadAsync();
        }

        private async Task DoLazyLoadAsync()
        {
            if (LazyLoadTriggered)
                return;

            LazyLoadTriggered = true;

            var lazyChildrenResult = await LazyLoadFetchChildren()
                .EvaluateFunctionAsync()
                .ConfigureAwait(false);

            LazyLoadCompleted = true;

            if (lazyChildrenResult.IsCompletedOk)
            {
                var lazyChildren = lazyChildrenResult.Data;

                foreach (var child in lazyChildren)
                    SetChildPropertiesFromParent(child);

                // If LazyLoadChildren has been overridden then just refresh the check state (using the new children) 
                // and update the check state (in case any of the new children is already set as checked)
                if (LazyLoadChildrenOverridden)
                    ReCalculateNodeCheckState();
                else
                    AddChildren(lazyChildren); // otherwise add the new children to the base collection.
            }

            RefreshChildren();
        }

        /// <summary>
        /// Get the children for this node, in Lazy-Loading Mode
        /// </summary>
        /// <returns></returns>
        protected virtual Task<perTreeViewItemViewModelBase[]> LazyLoadFetchChildren()
        {
            return Task.FromResult(new perTreeViewItemViewModelBase[0]);
        }

        /// <summary>
        /// Update the Children property
        /// </summary>
        public void RefreshChildren()
        {
            RaisePropertyChanged(nameof(Children));
        }

        /// <summary>
        /// In LazyLoading Mode, the Children property can be set to something other than
        /// the base _childrenList collection - e.g as the union ot two internal collections
        /// </summary>
        public IEnumerable<perTreeViewItemViewModelBase> Children => LazyLoadCompleted
                                                                    ? LazyLoadChildren
                                                                    : _childrenList;

        /// <summary>
        /// How are the children held when in lazy loading mode.
        /// </summary>
        /// <remarks>
        /// Override this as required in descendent classes - e.g. if Children is formed from a union
        /// of multiple internal child item collections (of different types) which are populated in LazyLoadFetchChildren()
        /// </remarks>
        protected virtual IEnumerable<perTreeViewItemViewModelBase> LazyLoadChildren => _childrenList;

        private bool _isSelected;

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                // if unselecting we don't care about anything else other than simply updating the property
                if (!value)
                {
                    Set(nameof(IsSelected), ref _isSelected, false);
                    return;
                }

                // Build a priority queue of operations
                //
                // All operations relating to tree item expansion are added with priority = DispatcherPriority.ContextIdle, so that they are
                // sorted before any operations relating to selection (which have priority = DispatcherPriority.ApplicationIdle).
                // This ensures that the visual container for all items are created before any selection operation is carried out.
                //
                // First expand all ancestors of the selected item - those closest to the root first
                //
                // Expanding a node will scroll as many of its children as possible into view - see perTreeViewItemHelper, but these scrolling
                // operations will be added to the queue after all of the parent expansions.
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

                // Set the item's selected state - use DispatcherPriority.ApplicationIdle so this operation is executed after all
                // expansion operations, no matter when they were added to the queue.
                //
                // Selecting a node will also scroll it into view - see perTreeViewItemHelper
                perDispatcherHelper.AddToQueue(() => Set(nameof(IsSelected), ref _isSelected, true), DispatcherPriority.ApplicationIdle);

                // note that by rule, a TreeView can only have one selected item, but this is handled automatically by 
                // the control - we aren't required to manually unselect the previously selected item.

                // execute all of the queued operations in descending DispatcherPriority order (expansion before selection)
                var unused = perDispatcherHelper.ProcessQueueAsync();
            }
        }

        public override string ToString()
        {
            return Caption;
        }

        /// <summary>
        /// What's the total number of child nodes beneath this one
        /// </summary>
        public int ChildCount => Children.Count() + Children.Sum(c => c.ChildCount);
    }
}