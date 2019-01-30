using System;
using Peregrine.Library;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WpfColorspaceDemo
{
    public enum ColorizingMode
    {
        Rgb,
        Hsl
    }

    /// <summary>
    /// Conversion between raw image data and a displayable bitmap
    /// </summary>
    public static class ImageConverter
    {
        /// <summary>
        /// The Colors use for the lowest and highest temperatures
        /// </summary>
        private static readonly perRgba LowestTemperatureRgb = new perRgba(0, 0, 80); // dark blue
        private static readonly perRgba HighestTemperatureRgb = new perRgba(255, 255, 160); // bright yellow

        /// <summary>
        /// The RGB values for the lowest and highest temperatures
        /// </summary>
        private static readonly byte MinRed = LowestTemperatureRgb.Red;
        private static readonly byte MaxRed = HighestTemperatureRgb.Red;
        private static readonly byte MinGreen = LowestTemperatureRgb.Green;
        private static readonly byte MaxGreen = HighestTemperatureRgb.Green;
        private static readonly byte MinBlue = LowestTemperatureRgb.Blue;
        private static readonly byte MaxBlue = HighestTemperatureRgb.Blue;

        /// <summary>
        /// The equivalent HSL color values
        /// </summary>
        private static readonly perHsla LowestTemperatureHsl = LowestTemperatureRgb.ToHsl();
        private static readonly perHsla HighestTemperatureHsl = HighestTemperatureRgb.ToHsl();

        private static readonly float MinHue = LowestTemperatureHsl.Hue;
        private static readonly float MinLum = LowestTemperatureHsl.Luminosity;

        // force hue to go around the color circle from blue to yellow via magenta / red
        private static readonly float MaxHue = HighestTemperatureHsl.Hue + 360f;
        private static readonly float MaxLum = HighestTemperatureHsl.Luminosity;

        /// <summary>
        /// Convert from intermediate raw image to colorized bitmap for use in UI 
        /// </summary>
        public static BitmapSource RawImageToColorizedImage(RawImage rawImage, ColorizingMode colorizingMode)
        {
            if (rawImage == null)
            {
                return null;
            }

            var width = rawImage.Width;
            var height = rawImage.Height;

            var pf = PixelFormats.Rgb24;
            var rawStride = (width * pf.BitsPerPixel + 7) / 8;
            var pixelData = new byte[rawStride * height];

            var minTemp = rawImage.MinValue;
            var maxTemp = rawImage.MaxValue;

            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    var pixelRGB = colorizingMode == ColorizingMode.Rgb
                        ? CalculatePixelColorRgb(rawImage[x, y], minTemp, maxTemp)
                        : CalculatePixelColorHsl(rawImage[x, y], minTemp, maxTemp);
                    SetPixel(x, y, pixelRGB, pixelData, rawStride);
                }
            }

            var result = BitmapSource.Create(width, height, 96, 96, pf, null, pixelData, rawStride);

            return result;
        }

        /// <summary>
        /// Calculate the color for a pixel based on its temperature using interpolation across the RGB color space
        /// </summary>
        private static perRgba CalculatePixelColorRgb(ushort pixelTemperature, ushort minTemperature, ushort maxTemperature)
        {
            if (minTemperature == maxTemperature)
            {
                return new perRgba(0, 0, 0);
            }

            // how far between min and max is this pixel
            var temperaturePercentage = (pixelTemperature * 1.0f - minTemperature) / (maxTemperature * 1.0f - minTemperature);

            var pixelRed = Convert.ToByte(((MaxRed - MinRed) * temperaturePercentage) + MinRed);
            var pixelGreen = Convert.ToByte(((MaxGreen - MinGreen) * temperaturePercentage) + MinGreen);
            var pixelBlue = Convert.ToByte(((MaxBlue - MinBlue) * temperaturePercentage) + MinBlue);
            return new perRgba(pixelRed, pixelGreen, pixelBlue);
        }

        /// <summary>
        /// Calculate the color for a pixel based on its temperature using interpolation across the HSL color space
        /// </summary>
        private static perRgba CalculatePixelColorHsl(ushort pixelTemperature, ushort minTemperature, ushort maxTemperature)
        {
            if (minTemperature == maxTemperature)
            {
                return new perRgba(0, 0, 0);
            }

            // how far between min and max is this pixel
            var temperaturePercentage = (pixelTemperature * 1.0f - minTemperature) / (maxTemperature * 1.0f - minTemperature);

            var pixelHue = ((MaxHue - MinHue) * temperaturePercentage) + MinHue;
            const float pixelSaturation = 1.0f;
            var pixelLuminosity = ((MaxLum - MinLum) * temperaturePercentage) + MinLum;
            var pixelHsl = new perHsla(pixelHue, pixelSaturation, pixelLuminosity);
            return pixelHsl.ToRgb();
        }

        // set the value of an individual pixel within the buffer
        private static void SetPixel(int x, int y, perRgba pixelRGB, byte[] buffer, int rawStride)
        {
            var xIndex = x * 3;
            var yIndex = y * rawStride;
            buffer[xIndex + yIndex] = pixelRGB.Red;
            buffer[xIndex + yIndex + 1] = pixelRGB.Green;
            buffer[xIndex + yIndex + 2] = pixelRGB.Blue;
        }
    }
}
