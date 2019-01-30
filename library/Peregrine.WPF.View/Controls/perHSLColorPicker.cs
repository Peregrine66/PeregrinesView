using GalaSoft.MvvmLight.Command;
using Peregrine.Library;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Peregrine.WPF.View.Controls
{
    public class perHslColorPicker : Control
    {
        static perHslColorPicker()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(perHslColorPicker), new FrameworkPropertyMetadata(typeof(perHslColorPicker)));
        }

        public perHslColorPicker()
        {
            _colorButtonCommand = new RelayCommand<perHsla>(OnColorButton);
            CopyToClipboardCommand = new RelayCommand(() => Clipboard.SetText(SelectedColor.AsHex8));
        }

        public perRgba SelectedColor
        {
            get => (perRgba)GetValue(SelectedColorProperty);
            set => SetValue(SelectedColorProperty, value);
        }

        public static readonly DependencyProperty SelectedColorProperty = DependencyProperty.Register("SelectedColor",
            typeof(perRgba),
            typeof(perHslColorPicker),
            new FrameworkPropertyMetadata(perRgba.Black, OnColorChanged));

        private bool _settingColor;

        private static void OnColorChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            if (!(sender is perHslColorPicker colorPicker))
                return;

            colorPicker._settingColor = true;
            var color = colorPicker.SelectedColor;
            colorPicker.Alpha = color.Alpha;

            try
            {
                if (!colorPicker._settingHsl)
                {
                    var hsl = color.ToHsl();
                    colorPicker.Hue = hsl.Hue;
                    colorPicker.Saturation = hsl.Saturation;
                    colorPicker.Luminosity = hsl.Luminosity;
                }
            }
            finally
            {
                colorPicker._settingColor = false;
            }
        }

        public float Hue
        {
            get => (float)GetValue(HueProperty);
            set => SetValue(HueProperty, value);
        }

        public static readonly DependencyProperty HueProperty =
            DependencyProperty.Register("Hue", typeof(float), typeof(perHslColorPicker), new FrameworkPropertyMetadata(OnHslChanged));

        public float Saturation
        {
            get => (float)GetValue(SaturationProperty);
            set => SetValue(SaturationProperty, value);
        }

        public static readonly DependencyProperty SaturationProperty =
            DependencyProperty.Register("Saturation", typeof(float), typeof(perHslColorPicker), new FrameworkPropertyMetadata(OnHslChanged));

        public float Luminosity
        {
            get => (float)GetValue(LuminosityProperty);
            set => SetValue(LuminosityProperty, value);
        }

        public static readonly DependencyProperty LuminosityProperty =
            DependencyProperty.Register("Luminosity", typeof(float), typeof(perHslColorPicker), new FrameworkPropertyMetadata(OnHslChanged));

        private bool _settingHsl;

        private static void OnHslChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            if (!(sender is perHslColorPicker colorPicker) || colorPicker._settingColor)
                return;

            colorPicker._settingHsl = true;

            try
            {
                var rgba = perColorConverter.HslToRgb(colorPicker.Hue, colorPicker.Saturation, colorPicker.Luminosity);
                colorPicker.SelectedColor = rgba;
            }
            finally
            {
                colorPicker._settingHsl = false;
            }
        }

        public byte Alpha
        {
            get => (byte)GetValue(AlphaProperty);
            set => SetValue(AlphaProperty, value);
        }

        public static readonly DependencyProperty AlphaProperty =
            DependencyProperty.Register("Alpha", typeof(byte), typeof(perHslColorPicker), new FrameworkPropertyMetadata((byte)255, OnAlphaChanged));

        private static void OnAlphaChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            if (!(sender is perHslColorPicker colorPicker))
                return;

            var color = colorPicker.SelectedColor;

            if (args.Property == AlphaProperty)
                color = new perRgba(color.Red, color.Green, color.Blue, (byte)args.NewValue);

            colorPicker.SelectedColor = color;
        }

        public bool ShowAlphaChannel
        {
            get => (bool)GetValue(ShowAlphaChannelProperty);
            set => SetValue(ShowAlphaChannelProperty, value);
        }

        public static readonly DependencyProperty ShowAlphaChannelProperty =
            DependencyProperty.Register("ShowAlphaChannel", typeof(bool), typeof(perHslColorPicker), new PropertyMetadata(true));

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (!(GetTemplateChild("PART_ColorSelectorGrid") is Grid grid))
                return;

            grid.ColumnDefinitions.Add(new ColumnDefinition());

            // add buttons for 16 levels of grey 
            for (var greyIndex = 0; greyIndex <= 15; greyIndex++)
            {
                grid.RowDefinitions.Add(new RowDefinition());
                var button = CreateHslColorPickerButton(0f, 0f, greyIndex / 15.0f);
                Grid.SetColumn(button, 0);
                Grid.SetRow(button, greyIndex);
                grid.Children.Add(button);
            }

            // spacer between greys and colours
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(20) });

            // add buttons for a range of hue / saturation / luminosity values
            for (var h = 0; h <= 11; h++)
            {
                for (var s = 2; s <= 5; s++)
                {
                    grid.ColumnDefinitions.Add(new ColumnDefinition());

                    for (var l = 2; l <= 9; l++)
                    {
                        var hue = h * 15;
                        var saturation = s / 5f;
                        var luminosity = l / 10f;

                        var button = CreateHslColorPickerButton(hue, saturation, luminosity);
                        Grid.SetColumn(button, grid.ColumnDefinitions.Count - 1);
                        Grid.SetRow(button, l - 2);
                        grid.Children.Add(button);
                    }

                    for (var l = 2; l <= 9; l++)
                    {
                        var hue = h * 15 + 180;
                        var saturation = s / 5f;
                        var luminosity = l / 10f;

                        var button = CreateHslColorPickerButton(hue, saturation, luminosity);
                        Grid.SetColumn(button, grid.ColumnDefinitions.Count - 1);
                        Grid.SetRow(button, l + 6);
                        grid.Children.Add(button);
                    }
                }
            }
        }

        private Button CreateHslColorPickerButton(float h, float s, float l)
        {
            var hsl = new perHsla(h, s, l);
            var rgba = hsl.ToRgb();
            var buttonBrush = new SolidColorBrush(Color.FromArgb(rgba.Alpha, rgba.Red, rgba.Green, rgba.Blue));

            return new Button
            {
                Background = buttonBrush,
                Command = _colorButtonCommand,
                CommandParameter = hsl,
                ToolTip = $"H:{h:f1} S:{s:f3} L:{l:f3}\r\n" + rgba
            };
        }

        private readonly RelayCommand<perHsla> _colorButtonCommand;

        private void OnColorButton(perHsla hsla)
        {
            Hue = hsla.Hue;
            Saturation = hsla.Saturation;
            Luminosity = hsla.Luminosity;
        }

        public ICommand CopyToClipboardCommand { get; }
    }
}
