using Peregrine.Library;
using System;
using System.Windows.Data;
using System.Windows.Media;

namespace Peregrine.WPF.View.Converters
{
    public class perRgbToSolidColorBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return !(value is perRgba rgba)
                       ? Brushes.Fuchsia
                       : new SolidColorBrush(Color.FromArgb(rgba.Alpha, rgba.Red, rgba.Green, rgba.Blue));
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (!(value is SolidColorBrush brush))
            {
                return perColors.Chartreuse;
            }

            var color = brush.Color;
            return new perRgba(color.R, color.G, color.B, color.A);
        }
    }
}