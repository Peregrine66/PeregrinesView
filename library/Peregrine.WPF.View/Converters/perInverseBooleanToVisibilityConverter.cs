using System.Windows;

namespace Peregrine.WPF.View.Converters
{
    public class perInverseBooleanToVisibilityConverter : perGenericBooleanConverter<Visibility>
    {
        public perInverseBooleanToVisibilityConverter()
        {
            TrueValue = Visibility.Collapsed;
            FalseValue = Visibility.Visible;
        }
    }
}
