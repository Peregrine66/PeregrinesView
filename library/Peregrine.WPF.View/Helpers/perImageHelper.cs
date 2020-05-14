using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Peregrine.WPF.View.Helpers
{
    public static class perImageHelper
    {
        private static BitmapSource _transparentPixel;

        /// <summary>
        /// A bitmap that consists of a single transparent pixel
        /// </summary>
        public static BitmapSource TransparentPixel
        {
            get
            {
                if (_transparentPixel != null)
                    return _transparentPixel;

                _transparentPixel = BitmapSource.Create(
                    1,
                    1,
                    96,
                    96,
                    PixelFormats.Indexed1,
                    new BitmapPalette(new List<Color> { Colors.Transparent }),
                    new byte[] { 0, 0, 0, 0 },
                    1);

                _transparentPixel.Freeze();

                return _transparentPixel;
            }
        }
    }
}
