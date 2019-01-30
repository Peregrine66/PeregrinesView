using System.Windows;

namespace Peregrine.WPF.View.Controls
{
    public class perDialogBase : perViewBase
    {
        static perDialogBase()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(perDialogBase), new FrameworkPropertyMetadata(typeof(perDialogBase)));
        }

        public perDialogBase()
        {
            MouseLeftButtonDown += (s, e) => DragMove();
        }
    }
}