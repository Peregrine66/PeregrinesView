using GalaSoft.MvvmLight.Ioc;
using ViewOpenCloseDemo.ServiceContracts;
using ViewOpenCloseDemo.ServiceImplementations;

namespace ViewOpenCloseDemo
{
    public static class Bootstrapper
    {
        public static void Run()
        {
            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<INavigationService, NavigationService>();
        }
    }
}
