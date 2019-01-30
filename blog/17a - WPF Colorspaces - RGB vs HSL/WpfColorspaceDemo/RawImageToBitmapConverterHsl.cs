using System;
using System.Globalization;
using System.Windows.Data;

namespace WpfColorspaceDemo
{
    class RawImageToBitmapConverterHsl : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is RawImage rawImage
                       ? ImageConverter.RawImageToColorizedImage(rawImage, ColorizingMode.Hsl)
                       : null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
