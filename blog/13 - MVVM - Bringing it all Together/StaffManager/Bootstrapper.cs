using Peregrine.WPF.View;
using Peregrine.WPF.ViewModel.IoC;
using StaffManager.ServiceImplementations;
using StaffManager.ServiceMocks;
using StaffManager.ViewModel;
using StaffManager.ViewModel.ServiceContracts;
using Peregrine.WPF.ViewModel.Helpers;
using StaffManager.Controls;

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

            perIoC.RegisterImplementation<IStaffManagerNavigationService, StaffManagerNavigationService>();

            perIoC.RegisterType<MainViewModel>();
            perIoC.RegisterType<AllDepartmentsViewModel>();
            perIoC.RegisterType<smEditDialogViewModel>();
        }
    }
}
