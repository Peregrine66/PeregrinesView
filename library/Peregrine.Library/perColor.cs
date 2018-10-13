using System;

namespace Peregrine.Library
{
    /// <summary>
    /// Data class for color values in Hue / Saturation / Luminosity / Alpha format
    /// </summary>
    /// <remarks>
    /// Hue: 0 .. 360
    /// Saturation: 0.0 .. 1.0
    /// Luminosity: 0.0 .. 1.0
    /// Alpha: 0 .. 255
    /// </remarks>
    public class perHsla
    {
        public perHsla(float hue, float saturation, float luminosity, byte alpha = 255)
        {
            Hue = hue;
            Saturation = saturation;
            Luminosity = luminosity;
            Alpha = alpha;
        }

        public float Hue { get; }
        public float Saturation { get; }
        public float Luminosity { get; }
        public byte Alpha { get; }

        public override string ToString()
        {
            return $"H:{Hue}, S:{Saturation}, L:{Luminosity} A:{Alpha}";
        }
    }

    /// <summary>
    /// Data class for color values in Red / Green / Blue / Alpha format
    /// </summary>
    /// <remarks>
    /// red / green / blue / alpha values are 0..255
    /// Use this so these routines can be used for both Winfoms and WPF which have different base color classes
    /// </remarks>
    public class perRgba
    {
        static perRgba()
        {
            Black = new perRgba(0, 0, 0);
        }

        public perRgba(byte red, byte green, byte blue, byte alpha = 255)
        {
            Red = red;
            Green = green;
            Blue = blue;
            Alpha = alpha;
        }

        public static perRgba Black { get; }

        public byte Red { get; }
        public byte Green { get; }
        public byte Blue { get; }
        public byte Alpha { get; }

        public override string ToString()
        {
            return $"R:{Red}, G:{Green}, B:{Blue} A:{Alpha} [#{Alpha:X2}{Red:X2}{Green:X2}{Blue:X2}]";
        }
    }

    /// <summary>
    /// Convert between Rgb and Hsl color values
    /// </summary>
    public static class perColorConverter
    {
        public static perHsla ToHsl(this perRgba rgba)
        {
            return RgbToHsl(rgba.Red, rgba.Green, rgba.Blue, rgba.Alpha);
        }

        public static perHsla RgbToHsl(byte red, byte green, byte blue, byte alpha = 255)
        {
            var min = Math.Min(Math.Min(red, green), blue);
            var max = Math.Max(Math.Max(red, green), blue);
            var delta = Convert.ToSingle(max - min);

            var luminosity = (max + min) / 510f;

            if (Math.Abs(delta) < 0.0001f)
                return new perHsla(0f, 0f, luminosity);

            float hue;
            var saturation = luminosity < 0.5
                ? delta / (max + min)
                : delta / (510 - max - min);

            if (red == max)
                hue = (green - blue) / delta;
            else if (green == max)
                hue = 2f + (blue - red) / delta;
            else
                hue = 4f + (red - green) / delta;

            return new perHsla(hue * 60f, saturation, luminosity, alpha);
        }

        public static perRgba ToRgb(this perHsla hsla)
        {
            return HslToRgb(hsla.Hue, hsla.Saturation, hsla.Luminosity, hsla.Alpha);
        }

        public static perRgba HslToRgb(float hue, float saturation, float luminosity)
        {
            return HslToRgb(hue, saturation, luminosity, 255);
        }

        public static perRgba HslToRgb(float hue, float saturation, float luminosity, byte alpha)
        {
            byte red, green, blue;

            // zero saturation => grey
            if (Math.Abs(saturation) < 0.0001f)
            {
                red = (byte)Math.Round(luminosity * 255);
                green = red;
                blue = red;
            }
            else
            {
                var t2 = luminosity < 0.5f
                    ? luminosity * (1.0f + saturation)
                    : luminosity + saturation - (luminosity * saturation);
                var t1 = (2 * luminosity) - t2;

                red = HueToRgb(t1, t2, hue + 120);
                green = HueToRgb(t1, t2, hue);
                blue = HueToRgb(t1, t2, hue - 120);
            }

            return new perRgba(red, green, blue, alpha);
        }

        private static byte HueToRgb(float t1, float t2, float h)
        {
            while (h >= 360)
            { h -= 360; }

            while (h < 0)
            { h += 360; }

            float x;
            if (h < 60)
                x = t1 + (t2 - t1) * h / 60;
            else if (h < 180)
                x = t2;
            else if (h < 240)
                x = t1 + (t2 - t1) * (240 - h) / 60;
            else
                x = t1;

            return Convert.ToByte(x * 255);
        }
    }
}