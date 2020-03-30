namespace Peregrine.Library
{
    /// <summary>
    /// Data class for colour values in Red / Green / Blue / Alpha format
    /// </summary>
    /// <remarks>
    /// Red / Green / Blue / Alpha: 0..255
    /// Use this so these routines can be used for both Windows Forms and WPF which have different base colour classes
    /// </remarks>
    public class perRgba
    {
        public perRgba(byte red, byte green, byte blue) : this(red, green, blue, 255)
        {
        }

        public perRgba(byte red, byte green, byte blue, byte alpha)
        {
            Red = red;
            Green = green;
            Blue = blue;
            Alpha = alpha;
        }

        public byte Red { get; }
        public byte Green { get; }
        public byte Blue { get; }
        public byte Alpha { get; }

        public override string ToString()
        {
            return $"{AsRgba} [{AsHex8}]";
        }

        public string AsRgba => $"R:{Red}, G:{Green}, B:{Blue} A:{Alpha}";

        public string AsHex6 => $"{Red:X2}{Green:X2}{Blue:X2}";

        public string AsHex8 => $"{Alpha:X2}{Red:X2}{Green:X2}{Blue:X2}";
    }
}