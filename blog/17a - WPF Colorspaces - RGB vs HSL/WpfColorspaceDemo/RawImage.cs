using System;
using System.Linq;

namespace WpfColorspaceDemo
{
    /// <summary>
    /// The intermediate image format - a 2 dimensional array of 16 bit temperature values
    /// </summary>
    public class RawImage
    {
        /// <summary>
        /// A private copy of the raw image data
        /// </summary>
        private readonly ushort[,] _temperatureData;

        /// <summary>
        /// Constructs a new <see cref="RawImage"/>
        /// </summary>
        public RawImage(ushort[,] temperatureData)
        {
            _temperatureData = (ushort[,]) temperatureData.Clone();

            Width = temperatureData.GetLength(0);
            Height = temperatureData.GetLength(1);
            MaxValue = temperatureData.Cast<ushort>().Max();
            MinValue = temperatureData.Cast<ushort>().Min();
        }

        /// <summary>
        /// Gets a temperature value for the given co-ordinates
        /// </summary>
        public ushort this[int x, int y]
        {
            get
            {
                if (x < 0 || x >= Width)
                    throw new ArgumentException($"RawImage - invalid x co-ordinate [{x}]");

                if (y < 0 || y >= Height)
                    throw new ArgumentException($"RawImage - invalid y co-ordinate [{y}]");

                return _temperatureData[x, y];
            }
        }

        /// <summary>
        /// Gets the Width of the Image
        /// </summary>
        public int Width { get; }

        /// <summary>
        /// Gets the Height of the Image
        /// </summary>
        public int Height { get; }

        /// <summary>
        /// Gets the maximum temperature value across the whole image
        /// </summary>
        public ushort MaxValue { get; }

        /// <summary>
        /// Gets the minimum temperature value across the whole image
        /// </summary>
        public ushort MinValue { get; }
    }
}