using Peregrine.WPF.View;
using Peregrine.WPF.ViewModel.IoC;

namespace CustomWindowChromeDemo
{
 public class ViewModelLocator
    {
        public ViewModelLocator()
        {
            perWpfViewBootstrapper.Run();
            perIoC.RegisterType<MainViewModel>();
        }

        public MainViewModel MainViewModel => perIoC.GetInstance<MainViewModel>();
    }
}
