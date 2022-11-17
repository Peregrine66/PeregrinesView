using System.Windows;
using System.Windows.Media;

namespace Peregrine.WPF.View.Helpers
{
    public static class perVisualTreeHelper
    {
        public static T FindLogicalParent<T>(this DependencyObject child) where T : DependencyObject
        {
            while (true)
            {
                var parent = LogicalTreeHelper.GetParent(child);

                if (parent == null)
                {
                    return null;
                }

                if (parent is T p)
                {
                    return p;
                }

                child = parent;
            }
        }

        public static T FindVisualParent<T>(this DependencyObject child) where T : DependencyObject
        {
            while (true)
            {
                var parent = VisualTreeHelper.GetParent(child);

                if (parent == null)
                {
                    return null;
                }

                if (parent is T p)
                {
                    return p;
                }

                child = parent;
            }
        }
    }
}