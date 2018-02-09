using GalaSoft.MvvmLight.Ioc;
using StaffManager.ServiceContracts;
using StaffManager.ServiceImplementations;

namespace StaffManager
{
    public static class Bootstrapper
    {
        public static void Run()
        {
            SimpleIoc.Default.Register<IStaffManagerDataService, StaffManagerDataService>();
            SimpleIoc.Default.Register<MainViewModel>();
        }
    }
}
