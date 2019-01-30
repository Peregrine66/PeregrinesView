using Peregrine.WPF.View;
using Peregrine.WPF.ViewModel.Helpers;
using Peregrine.WPF.ViewModel.IoC;
using StaffManager.ServiceImplementations;
using StaffManager.ServiceMocks;
using StaffManager.ViewModel.ServiceContracts;

namespace StaffManager
{
    public static class Bootstrapper
    {
        public static void Run()
        {
            perWpfViewBootstrapper.Run();

            if (perViewModelHelper.IsInDesignMode)
                perIoC.RegisterImplementation<IStaffManagerDataService, StaffManagerDataServiceMock>();
            else
                perIoC.RegisterImplementation<IStaffManagerDataService, StaffManagerDataService>();

            perIoC.RegisterType<MainViewModel>();
        }
    }
}
