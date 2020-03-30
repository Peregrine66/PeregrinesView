using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;

namespace Peregrine.WPF.View.Controls
{
    public class perBlinkingBorder : Border
    {
        static perBlinkingBorder()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(perBlinkingBorder), new FrameworkPropertyMetadata(typeof(perBlinkingBorder)));
        }

        public bool IsBlinking
        {
            get => (bool)GetValue(IsBlinkingProperty);
            set => SetValue(IsBlinkingProperty, value);
        }

        public static readonly DependencyProperty IsBlinkingProperty =
            DependencyProperty.Register("IsBlinking", typeof(bool), typeof(perBlinkingBorder), new PropertyMetadata(false, UpdateBorderBrush));

        private static void UpdateBorderBrush(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is perBlinkingBorder blinkingBorder))
                return;

            blinkingBorder.BorderBrush = blinkingBorder.IsBlinking
                ? blinkingBorder.BlinkingBorderBrush
                : blinkingBorder.DefaultBorderBrush;
        }

        public Brush DefaultBorderBrush
        {
            get => (Brush)GetValue(DefaultBorderBrushProperty);
            set => SetValue(DefaultBorderBrushProperty, value);
        }

        public static readonly DependencyProperty DefaultBorderBrushProperty =
            DependencyProperty.Register("DefaultBorderBrush", typeof(Brush), typeof(perBlinkingBorder), new PropertyMetadata(Brushes.Chartreuse, UpdateBorderBrush));

        public Brush BlinkingBorderBrush
        {
            get => (Brush)GetValue(BlinkingBorderBrushProperty);
            set => SetValue(BlinkingBorderBrushProperty, value);
        }

        public static readonly DependencyProperty BlinkingBorderBrushProperty =
            DependencyProperty.Register("BlinkingBorderBrush", typeof(Brush), typeof(perBlinkingBorder), new PropertyMetadata(Brushes.Fuchsia, UpdateBorderBrush));
    }
}
