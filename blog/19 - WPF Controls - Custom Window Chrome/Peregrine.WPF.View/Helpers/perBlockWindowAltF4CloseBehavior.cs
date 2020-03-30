using System.Windows;
using System.Windows.Input;

namespace Peregrine.WPF.View.Helpers
{
    /// <inheritdoc/>
    /// <summary>
    /// Prevent Alt-F4 from closing a window
    /// </summary>
    public class perBlockWindowAltF4CloseBehavior : perBehaviorForStyle<Window, perBlockWindowAltF4CloseBehavior>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.KeyDown += OnKeyDown;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.KeyDown -= OnKeyDown;
            base.OnDetaching();
        }

        private static void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.System && e.SystemKey == Key.F4)
                e.Handled = true;
        }
    }
}
