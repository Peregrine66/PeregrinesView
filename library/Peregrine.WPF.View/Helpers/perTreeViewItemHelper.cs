using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Peregrine.WPF.View.Helpers
{
    public static class perTreeViewItemHelper
    {
        public static bool GetBringSelectedItemIntoView(TreeViewItem treeViewItem)
        {
            return (bool)treeViewItem.GetValue(BringSelectedItemIntoViewProperty);
        }

        public static void SetBringSelectedItemIntoView(TreeViewItem treeViewItem, bool value)
        {
            treeViewItem.SetValue(BringSelectedItemIntoViewProperty, value);
        }

        public static readonly DependencyProperty BringSelectedItemIntoViewProperty =
            DependencyProperty.RegisterAttached(
                "BringSelectedItemIntoView",
                typeof(bool),
                typeof(perTreeViewItemHelper),
                new UIPropertyMetadata(false, BringSelectedItemIntoViewChanged));

        private static void BringSelectedItemIntoViewChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            if (!(args.NewValue is bool))
                return;

            var item = obj as TreeViewItem;

            if (item == null)
                return;

            if ((bool)args.NewValue)
                item.Selected += OnTreeViewItemSelected;
            else
                item.Selected -= OnTreeViewItemSelected;
        }

        private static void OnTreeViewItemSelected(object sender, RoutedEventArgs e)
        {
            var item = e.OriginalSource as TreeViewItem;
            item?.BringIntoView();

            // prevent this event bubbling up to any parent nodes
            e.Handled = true;
        }

        public static bool GetBringExpandedChildrenIntoView(TreeViewItem treeViewItem)
        {
            return (bool)treeViewItem.GetValue(BringExpandedChildrenIntoViewProperty);
        }

        public static void SetBringExpandedChildrenIntoView(TreeViewItem treeViewItem, bool value)
        {
            treeViewItem.SetValue(BringExpandedChildrenIntoViewProperty, value);
        }

        public static readonly DependencyProperty BringExpandedChildrenIntoViewProperty =
            DependencyProperty.RegisterAttached(
                "BringExpandedChildrenIntoView",
                typeof(bool),
                typeof(perTreeViewItemHelper),
                new UIPropertyMetadata(false, BringExpandedChildrenIntoViewChanged));

        private static void BringExpandedChildrenIntoViewChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            if (!(args.NewValue is bool))
                return;

            var item = obj as TreeViewItem;

            if (item == null)
                return;

            if ((bool)args.NewValue)
                item.Expanded += OnTreeViewItemExpanded;
            else
                item.Expanded -= OnTreeViewItemExpanded;
        }

        private static void OnTreeViewItemExpanded(object sender, RoutedEventArgs e)
        {
            var item = e.OriginalSource as TreeViewItem;

            if (item == null)
                return;

            // use DispatcherPriority.ContextIdle, so that we wait for all of the UI elements for any newly visible children to be created

            // first bring the last child into view
            Action action = () =>
            {
                var lastChild = item.ItemContainerGenerator.ContainerFromIndex(item.Items.Count - 1) as TreeViewItem;
                lastChild?.BringIntoView();
            };

            item.Dispatcher.BeginInvoke(action, DispatcherPriority.ContextIdle);

            // then bring the expanded item (back) into view
            action = () => { item.BringIntoView(); };
            item.Dispatcher.BeginInvoke(action, DispatcherPriority.ContextIdle);

            // prevent this event bubbling up to any parent nodes
            e.Handled = true;
        }
    }
}
