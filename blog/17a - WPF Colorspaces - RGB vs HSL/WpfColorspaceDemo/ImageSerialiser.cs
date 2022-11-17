using System;
using System.Globalization;

namespace WpfColorspaceDemo
{
    public static class ImageSerialiser
    {
        /// <summary>
        /// The field separator to use between the components of the serialized string.
        /// Must not be used by Base64 encoding.
        /// </summary>
        private const char FieldSeparator = '*';

        /// <summary>
        /// Deserialises a Base64 string representing an image and its size data into a
        /// raw data array
        /// </summary>
        public static RawImage DeserialiseRawImage(string base64EncodedData)
        {
            var elements = base64EncodedData.Split(FieldSeparator);
            var width = int.Parse(elements[0], CultureInfo.InvariantCulture);
            var height = int.Parse(elements[1], CultureInfo.InvariantCulture);
            var bytes = Convert.FromBase64String(elements[2]);
            var temperatureData = ConvertTo2DUshortArray(bytes, width, height);
            var result = new RawImage(temperatureData);
            return result;
        }

#pragma warning disable 1584 // syntax error in comment

        /// <summary>
        /// Converts a one dimensional <see cref="byte[]"/> into a two dimensional 
        /// <see cref="ushort[,]"/> array given a width and height
        /// </summary>
        private static ushort[,] ConvertTo2DUshortArray(byte[] data, int width, int height)
        {
            var result = new ushort[width, height];
            Buffer.BlockCopy(data, 0, result, 0, width * height * sizeof(ushort));
            return result;
        }
    }
}
