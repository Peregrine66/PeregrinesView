using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Peregrine.WPF.View.Controls
{
    public enum perControlHostCaptionLocation
    {
        ToLeftOfControl,
        AboveControl
    }

    public class perControlHost : ContentControl
    {
        static perControlHost()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(perControlHost),
                new FrameworkPropertyMetadata(typeof(perControlHost)));
        }

        public perControlHost()
        {
            var properyDescriptor = DependencyPropertyDescriptor.FromProperty(ContentProperty, typeof(perControlHost));
            properyDescriptor?.AddValueChanged(this, OnContentChanged);
        }

        private static void OnContentChanged(object sender, EventArgs args)
        {
            var controlHost = sender as perControlHost;
            var contentObject = controlHost?.Content as DependencyObject;
            contentObject?.SetValue(Validation.ErrorTemplateProperty, controlHost.ValidationErrorTemplate);
        }

        public string Caption
        {
            get => (string)GetValue(CaptionProperty);
            set => SetValue(CaptionProperty, value);
        }

        public static readonly DependencyProperty CaptionProperty =
            DependencyProperty.Register("Caption", typeof(string), typeof(perControlHost), new PropertyMetadata(null));

        public perControlHostCaptionLocation CaptionLocation
        {
            get => (perControlHostCaptionLocation)GetValue(CaptionLocationProperty);
            set => SetValue(CaptionLocationProperty, value);
        }

        public static readonly DependencyProperty CaptionLocationProperty =
            DependencyProperty.Register("CaptionLocation",
                typeof(perControlHostCaptionLocation),
                typeof(perControlHost),
                new PropertyMetadata(perControlHostCaptionLocation.AboveControl));

        public int CaptionWidth
        {
            get => (int)GetValue(CaptionWidthProperty);
            set => SetValue(CaptionWidthProperty, value);
        }

        public static readonly DependencyProperty CaptionWidthProperty =
            DependencyProperty.Register("CaptionWidth", typeof(int), typeof(perControlHost), new PropertyMetadata(1));

        public FontWeight CaptionFontWeight
        {
            get => (FontWeight)GetValue(CaptionFontWeightProperty);
            set => SetValue(CaptionFontWeightProperty, value);
        }

        public static readonly DependencyProperty CaptionFontWeightProperty =
            DependencyProperty.Register("CaptionFontWeight", typeof(FontWeight), typeof(perControlHost), new PropertyMetadata(FontWeights.Normal));

        public double CaptionFontSize
        {
            get => (double)GetValue(CaptionFontSizeProperty);
            set => SetValue(CaptionFontSizeProperty, value);
        }

        public static readonly DependencyProperty CaptionFontSizeProperty =
            DependencyProperty.Register("CaptionFontSize", typeof(double), typeof(perControlHost), new PropertyMetadata(12d));

        public Brush CaptionForeground
        {
            get => (Brush)GetValue(CaptionForegroundProperty);
            set => SetValue(CaptionForegroundProperty, value);
        }

        public static readonly DependencyProperty CaptionForegroundProperty =
            DependencyProperty.Register("CaptionForeground", typeof(Brush), typeof(perControlHost), new PropertyMetadata(Brushes.Black));

        public CornerRadius BorderCornerRadius
        {
            get => (CornerRadius)GetValue(BorderCornerRadiusProperty);
            set => SetValue(BorderCornerRadiusProperty, value);
        }

        public static readonly DependencyProperty BorderCornerRadiusProperty =
            DependencyProperty.Register("BorderCornerRadius", typeof(CornerRadius), typeof(perControlHost), new PropertyMetadata(new CornerRadius(4)));

        public ControlTemplate ValidationErrorTemplate
        {
            get => (ControlTemplate)GetValue(ValidationErrorTemplateProperty);
            set => SetValue(ValidationErrorTemplateProperty, value);
        }

        public static readonly DependencyProperty ValidationErrorTemplateProperty =
            DependencyProperty.Register("ValidationErrorTemplate", typeof(ControlTemplate), typeof(perControlHost), new PropertyMetadata(null, OnValidationErrorTemplateChanged));

        private static void OnValidationErrorTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var controlHost = d as perControlHost;
            var contentObject = controlHost?.Content as DependencyObject;
            contentObject?.SetValue(Validation.ErrorTemplateProperty, e.NewValue);
        }

        public bool HideContent
        {
            get => (bool)GetValue(HideContentProperty);
            set => SetValue(HideContentProperty, value);
        }

        public static readonly DependencyProperty HideContentProperty =
            DependencyProperty.Register("HideContent", typeof(bool), typeof(perControlHost), new PropertyMetadata(false));

        public string HiddenContentText
        {
            get => (string)GetValue(HiddenContentTextProperty);
            set => SetValue(HiddenContentTextProperty, value);
        }

        public static readonly DependencyProperty HiddenContentTextProperty =
            DependencyProperty.Register("HiddenContentText", typeof(string), typeof(perControlHost), new PropertyMetadata(null));

        public Brush HiddenContentTextBrush
        {
            get => (Brush)GetValue(HiddenContentTextBrushProperty);
            set => SetValue(HiddenContentTextBrushProperty, value);
        }

        public static readonly DependencyProperty HiddenContentTextBrushProperty =
            DependencyProperty.Register("HiddenContentTextBrush", typeof(Brush), typeof(perControlHost), new PropertyMetadata(Brushes.Black));

        public Brush ErrorTooltipForeground
        {
            get => (Brush)GetValue(ErrorTooltipForegroundProperty);
            set => SetValue(ErrorTooltipForegroundProperty, value);
        }

        public static readonly DependencyProperty ErrorTooltipForegroundProperty =
            DependencyProperty.Register("ErrorTooltipForeground", typeof(Brush), typeof(perControlHost), new PropertyMetadata(Brushes.Red));

        public Brush ErrorTooltipBackground
        {
            get => (Brush)GetValue(ErrorTooltipBackgroundProperty);
            set => SetValue(ErrorTooltipBackgroundProperty, value);
        }

        public static readonly DependencyProperty ErrorTooltipBackgroundProperty =
            DependencyProperty.Register("ErrorTooltipBackground", typeof(Brush), typeof(perControlHost), new PropertyMetadata(Brushes.LightYellow));
    }
}