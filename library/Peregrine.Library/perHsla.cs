namespace Peregrine.Library
{
    /// <summary>
    /// Data class for colour values in Hue / Saturation / Luminosity / Alpha format
    /// </summary>
    /// <remarks>
    /// Hue: 0 .. 360
    /// Saturation: 0.0 .. 1.0
    /// Luminosity: 0.0 .. 1.0
    /// Alpha: 0 .. 255
    /// </remarks>
    public class perHsla
    {
        public perHsla(float hue, float saturation, float luminosity) : this(hue, saturation, luminosity, 255)
        {
        }

        public perHsla(float hue, float saturation, float luminosity, byte alpha)
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
}