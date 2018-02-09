using GalaSoft.MvvmLight.Ioc;
using Peregrine.WPF.View;

namespace MvvmDialogServiceDemo
{
    public static class Bootstrapper
    {
        public static void Run()
        {
            perWpfViewBootstrapper.Run();
            SimpleIoc.Default.Register<MainViewModel>();
        }
    }
}
