using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;
using Peregrine.WPF.ViewModel;

namespace Peregrine.WPF.View.Helpers
{
    public class perTreeViewHelper : Behavior<TreeView>
    {
        public object BoundSelectedItem
        {
            get { return GetValue(BoundSelectedItemProperty); }
            set { SetValue(BoundSelectedItemProperty, value); }
        }

        public static readonly DependencyProperty BoundSelectedItemProperty =
            DependencyProperty.Register("BoundSelectedItem",
                typeof(object),
                typeof(perTreeViewHelper),
                new FrameworkPropertyMetadata(null,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    OnBoundSelectedItemChanged));

        private static void OnBoundSelectedItemChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var item = args.NewValue as perTreeViewItemViewModelBase;

            if (item != null)
                item.IsSelected = true;
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.SelectedItemChanged += OnTreeViewSelectedItemChanged;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.SelectedItemChanged -= OnTreeViewSelectedItemChanged;
            base.OnDetaching();
        }

        private void OnTreeViewSelectedItemChanged(object obj, RoutedPropertyChangedEventArgs<object> args)
        {
            BoundSelectedItem = args.NewValue;
        }
    }
}