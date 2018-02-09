using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace Peregrine.WPF.View.Helpers
{
    /// <inheritdoc />
    /// <summary>
    /// Hide the close button on a window.
    /// </summary>
    public class perHideWindowCloseButtonBehavior : perBehaviorForStyle<Window, perHideWindowCloseButtonBehavior>
    {
        private const int GWL_STYLE = -16;
        private const int WS_SYSMENU = 0x80000;
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.Loaded += OnLoaded;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.Loaded -= OnLoaded;
            base.OnDetaching();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            var hwnd = new WindowInteropHelper(AssociatedObject).Handle;
            SetWindowLong(hwnd, GWL_STYLE, GetWindowLong(hwnd, GWL_STYLE) & ~WS_SYSMENU);
        }
    }
}
