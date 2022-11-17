using Peregrine.WPF.View;
using Peregrine.WPF.ViewModel.IoC;

namespace FileSystemDialogsDemo
{
    public static class Bootstrapper
    {
        public static void Run()
        {
            perWpfViewBootstrapper.Run();
            perIoC.RegisterType<MainViewModel>();
        }
    }
}
