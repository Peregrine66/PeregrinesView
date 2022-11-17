using System;

namespace Peregrine.Library
{
    /// <summary>
    /// Convert between Rgb and Hsl colour values
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

            if (Math.Abs(delta) < 0.001f)
            {
                return new perHsla(0f, 0f, luminosity);
            }

            float hue;
            var saturation = luminosity < 0.5
                ? delta / (max + min)
                : delta / (510 - max - min);

            if (red == max)
            {
                hue = (green - blue) / delta;
            }
            else if (green == max)
            {
                hue = 2f + (blue - red) / delta;
            }
            else
            {
                hue = 4f + (red - green) / delta;
            }

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

            // zero saturation => some shade of grey
            if (Math.Abs(saturation) < 0.001f)
            {
                red = Convert.ToByte(luminosity * 255);
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

        private static byte HueToRgb(float t1, float t2, float hue)
        {
            while (hue >= 360)
            {
                hue -= 360;
            }

            while (hue < 0)
            {
                hue += 360;
            }

            float x;
            if (hue < 60)
            {
                x = t1 + (t2 - t1) * hue / 60;
            }
            else if (hue < 180)
            {
                x = t2;
            }
            else if (hue < 240)
            {
                x = t1 + (t2 - t1) * (240 - hue) / 60;
            }
            else
            {
                x = t1;
            }

            return Convert.ToByte(x * 255);
        }
    }
}