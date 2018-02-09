using GalaSoft.MvvmLight.Ioc;
using Peregrine.WPF.View.DialogService;
using Peregrine.WPF.ViewModel.DialogService;

namespace Peregrine.WPF.View
{
    public static class perWpfViewBootstrapper
    {
        public static void Run()
        {
            SimpleIoc.Default.Register<IperDialogService, perDialogService>();
        }
    }
}
