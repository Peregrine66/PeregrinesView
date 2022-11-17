using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace Peregrine.WPF.View.Controls
{
    public enum CaptionPosition
    {
        None,
        ToLeftOfIcon,
        AboveIcon,
        ToRightOfIcon,
        BelowIcon
    }

    public enum IconSize
    {
        ExtraSmall,
        Small,
        Medium,
        Large,
        ExtraLarge,
        ExtraExtraLarge
    }

    public class perXamlIconHost : Control
    {
        static perXamlIconHost()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(perXamlIconHost), new FrameworkPropertyMetadata(typeof(perXamlIconHost)));
        }

        public FrameworkElement XamlIcon
        {
            get => (FrameworkElement) GetValue(XamlIconProperty);
            set => SetValue(XamlIconProperty, value);
        }

        public static readonly DependencyProperty XamlIconProperty =
            DependencyProperty.Register("XamlIcon", typeof(FrameworkElement), typeof(perXamlIconHost), new PropertyMetadata(null));

        public IconSize IconSize
        {
            get => (IconSize) GetValue(IconSizeProperty);
            set => SetValue(IconSizeProperty, value);
        }

        public static readonly DependencyProperty IconSizeProperty =
            DependencyProperty.Register("IconSize", typeof(IconSize), typeof(perXamlIconHost), new PropertyMetadata(IconSize.Medium));

        public string Caption
        {
            get => (string) GetValue(CaptionProperty);
            set => SetValue(CaptionProperty, value);
        }

        public static readonly DependencyProperty CaptionProperty =
            DependencyProperty.Register("Caption", typeof(string), typeof(perXamlIconHost), new PropertyMetadata(null));

        public CaptionPosition CaptionPosition
        {
            get => (CaptionPosition) GetValue(CaptionPositionProperty);
            set => SetValue(CaptionPositionProperty, value);
        }

        public static readonly DependencyProperty CaptionPositionProperty =
            DependencyProperty.Register("CaptionPosition", typeof(CaptionPosition), typeof(perXamlIconHost),
                new PropertyMetadata(CaptionPosition.ToRightOfIcon));

        public Brush StandardForeground
        {
            get => (Brush) GetValue(StandardForegroundProperty);
            set => SetValue(StandardForegroundProperty, value);
        }

        public static readonly DependencyProperty StandardForegroundProperty =
            DependencyProperty.Register("StandardForeground", typeof(Brush), typeof(perXamlIconHost), new PropertyMetadata(Brushes.Black));

        public Brush StandardHighlight
        {
            get => (Brush) GetValue(StandardHighlightProperty);
            set => SetValue(StandardHighlightProperty, value);
        }

        public static readonly DependencyProperty StandardHighlightProperty =
            DependencyProperty.Register("StandardHighlight", typeof(Brush), typeof(perXamlIconHost), new PropertyMetadata(Brushes.White));

        public Brush DisabledForeground
        {
            get => (Brush) GetValue(DisabledForegroundProperty);
            set => SetValue(DisabledForegroundProperty, value);
        }

        public static readonly DependencyProperty DisabledForegroundProperty =
            DependencyProperty.Register("DisabledForeground", typeof(Brush), typeof(perXamlIconHost), new PropertyMetadata(Brushes.Silver));

        public Brush DisabledHighlight
        {
            get => (Brush) GetValue(DisabledHighlightProperty);
            set => SetValue(DisabledHighlightProperty, value);
        }

        public static readonly DependencyProperty DisabledHighlightProperty =
            DependencyProperty.Register("DisabledHighlight", typeof(Brush), typeof(perXamlIconHost), new PropertyMetadata(Brushes.Gray));
    }

    // ==============================================================================================================================================

    public class perXamlIconSizeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            const int defaultSize = 40;

            if (!(value is IconSize))
            {
                return defaultSize;
            }

            var iconSizeValue = (IconSize) value;

            switch (iconSizeValue)
            {
                case IconSize.ExtraSmall:
                    return defaultSize / 2;
                case IconSize.Small:
                    return defaultSize * 3 / 4;
                case IconSize.Large:
                    return defaultSize * 3 / 2;
                case IconSize.ExtraLarge:
                    return defaultSize * 2;
                case IconSize.ExtraExtraLarge:
                    return defaultSize * 5 / 2;
                default:
                    return defaultSize;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}