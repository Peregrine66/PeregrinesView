using GalaSoft.MvvmLight.Ioc;

namespace Peregrine.WPF.ViewModel.Helpers
{
    public static class perIoC
    {
        public static void RegisterType<T>() where T: class
        {
            SimpleIoc.Default.Register<T>();
        }

        public static void RegisterInterfaceImplementation<TInterface, TImplementation>() where TInterface : class where TImplementation : class, TInterface 
        {
            SimpleIoc.Default.Register<TInterface, TImplementation>();
        }

        public static T GetInstance<T>(string instanceId = "")
        {
            return SimpleIoc.Default.GetInstance<T>(instanceId);
        }

        public static void RemoveInstance<T>(T item) where T: class
        {
            SimpleIoc.Default.Unregister(item);
        }
    }
}
