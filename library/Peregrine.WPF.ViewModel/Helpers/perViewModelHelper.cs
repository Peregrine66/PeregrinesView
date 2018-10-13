using GalaSoft.MvvmLight;

namespace Peregrine.WPF.ViewModel.Helpers
{
    public static class perViewModelHelper
    {
        public static bool IsInDesignMode => ViewModelBase.IsInDesignModeStatic;
    }
}
