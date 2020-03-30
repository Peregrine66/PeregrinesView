using Peregrine.WPF.ViewModel;

namespace BlinkingBorderDemo
{
    public class MainViewModel: perViewModelBase
    {
        private bool _isBlinking;

        public bool IsBlinking
        {
            get => _isBlinking;
            set => Set(nameof(IsBlinking), ref _isBlinking, value);
        }
    }
}
