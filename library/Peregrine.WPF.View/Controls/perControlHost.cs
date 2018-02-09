using Peregrine.WPF.View.Helpers;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
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

            if (contentObject != null)
            {
                var properyDescriptor = DependencyPropertyDescriptor.FromProperty(Validation.HasErrorProperty, contentObject.GetType());
                properyDescriptor.AddValueChanged(contentObject, OnContentValidationHasErrorChanged);
            }
        }

        private static void OnContentValidationHasErrorChanged(object sender, EventArgs args)
        {
            var dObj = sender as DependencyObject;
            var controlHost = dObj?.FindLogicalParent<perControlHost>();

            if (controlHost == null)
                return;

            if (!(bool) dObj.GetValue(Validation.HasErrorProperty))
            {
                controlHost.TooltipContents = null;
                return;
            }

            var errors = dObj.GetValue(Validation.ErrorsProperty) as ReadOnlyObservableCollection<ValidationError>;

            if (errors == null)
            {
                controlHost.TooltipContents = null;
                return;
            }

            controlHost.TooltipContents = errors.First().ErrorContent.ToString();
        }

        public string TooltipContents
        {
            get { return (string)GetValue(TooltipContentsProperty); }
            set { SetValue(TooltipContentsProperty, value); }
        }

        public static readonly DependencyProperty TooltipContentsProperty =
            DependencyProperty.Register("TooltipContents", typeof(string), typeof(perControlHost), new PropertyMetadata(null));

        public string Caption
        {
            get { return (string)GetValue(CaptionProperty); }
            set { SetValue(CaptionProperty, value); }
        }

        public static readonly DependencyProperty CaptionProperty =
            DependencyProperty.Register("Caption", typeof(string), typeof(perControlHost), new PropertyMetadata(null));

        public perControlHostCaptionLocation CaptionLocation
        {
            get { return (perControlHostCaptionLocation)GetValue(CaptionLocationProperty); }
            set { SetValue(CaptionLocationProperty, value); }
        }

        public static readonly DependencyProperty CaptionLocationProperty =
            DependencyProperty.Register("CaptionLocation",
                typeof(perControlHostCaptionLocation),
                typeof(perControlHost),
                new PropertyMetadata(perControlHostCaptionLocation.AboveControl));

        public int CaptionWidth
        {
            get { return (int)GetValue(CaptionWidthProperty); }
            set { SetValue(CaptionWidthProperty, value); }
        }

        public static readonly DependencyProperty CaptionWidthProperty =
            DependencyProperty.Register("CaptionWidth",
                typeof(int),
                typeof(perControlHost),
                new PropertyMetadata(1));

        public FontWeight CaptionFontWeight
        {
            get { return (FontWeight)GetValue(CaptionFontWeightProperty); }
            set { SetValue(CaptionFontWeightProperty, value); }
        }

        public static readonly DependencyProperty CaptionFontWeightProperty =
            DependencyProperty.Register("CaptionFontWeight",
                typeof(FontWeight),
                typeof(perControlHost),
                new PropertyMetadata(FontWeights.Normal));

        public double CaptionFontSize
        {
            get { return (double)GetValue(CaptionFontSizeProperty); }
            set { SetValue(CaptionFontSizeProperty, value); }
        }

        public static readonly DependencyProperty CaptionFontSizeProperty =
            DependencyProperty.Register("CaptionFontSize",
                typeof(double),
                typeof(perControlHost),
                new PropertyMetadata(12d));

        public Brush CaptionForeground
        {
            get { return (Brush)GetValue(CaptionForegroundProperty); }
            set { SetValue(CaptionForegroundProperty, value); }
        }

        public static readonly DependencyProperty CaptionForegroundProperty =
            DependencyProperty.Register("CaptionForeground",
                typeof(Brush),
                typeof(perControlHost),
                new PropertyMetadata(Brushes.Black));

        public CornerRadius BorderCornerRadius
        {
            get { return (CornerRadius)GetValue(BorderCornerRadiusProperty); }
            set { SetValue(BorderCornerRadiusProperty, value); }
        }

        public static readonly DependencyProperty BorderCornerRadiusProperty =
            DependencyProperty.Register("BorderCornerRadius",
                typeof(CornerRadius),
                typeof(perControlHost),
                new PropertyMetadata(new CornerRadius(4)));

        public ControlTemplate ValidationErrorTemplate
        {
            get { return (ControlTemplate)GetValue(ValidationErrorTemplateProperty); }
            set { SetValue(ValidationErrorTemplateProperty, value); }
        }

        public static readonly DependencyProperty ValidationErrorTemplateProperty =
            DependencyProperty.Register("ValidationErrorTemplate",
                typeof(ControlTemplate),
                typeof(perControlHost),
                new PropertyMetadata(null, OnValidationErrorTemplateChanged));

        private static void OnValidationErrorTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var controlHost = d as perControlHost;
            var contentObject = controlHost?.Content as DependencyObject;
            contentObject?.SetValue(Validation.ErrorTemplateProperty, e.NewValue);
        }
    }
}
