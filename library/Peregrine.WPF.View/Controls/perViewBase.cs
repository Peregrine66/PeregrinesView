using System.Windows;

namespace Peregrine.WPF.View.Controls
{
    public class perViewBase : Window
    {
        static perViewBase()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(perViewBase), new FrameworkPropertyMetadata(typeof(perViewBase)));
        }

        protected perViewBase()
        {
        }
    }
}
