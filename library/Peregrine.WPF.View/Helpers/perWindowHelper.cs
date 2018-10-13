using System.Reflection;
using System.Windows;

namespace Peregrine.WPF.View.Helpers
{
    public static class perWindowHelper
    {
        public static readonly DependencyProperty CloseWindowProperty = DependencyProperty.RegisterAttached(
            "CloseWindow",
            typeof(bool?),
            typeof(perWindowHelper),
            new PropertyMetadata(null, OnCloseWindowChanged));

        private static void OnCloseWindowChanged(DependencyObject target, DependencyPropertyChangedEventArgs args)
        {
            if (!(target is Window view))
                return;

            if (view.IsModal())
                view.DialogResult = args.NewValue as bool?;
            else
                view.Close();
        }

        public static void SetCloseWindow(Window target, bool? value)
        {
            target.SetValue(CloseWindowProperty, value);
        }


        public static bool IsModal(this Window window)
        {
            var fieldInfo = typeof(Window).GetField("_showingAsDialog", BindingFlags.Instance | BindingFlags.NonPublic);
            return fieldInfo != null && (bool)fieldInfo.GetValue(window);
        }
    }
}
