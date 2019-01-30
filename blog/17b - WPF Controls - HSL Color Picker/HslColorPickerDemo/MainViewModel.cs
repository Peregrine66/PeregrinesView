using GalaSoft.MvvmLight;
using Peregrine.Library;

namespace HslColorPickerDemo
{
    public class MainViewModel : ViewModelBase
    {
        private perRgba _boundColor = new perRgba(210, 105, 30);

        public perRgba BoundColor
        {
            get => _boundColor;
            set => Set(nameof(BoundColor), ref _boundColor, value);
        }
    }
}
