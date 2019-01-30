using GalaSoft.MvvmLight.Ioc;
using System;

namespace Peregrine.WPF.ViewModel.IoC
{
    /// <summary>
    /// A common facade over the designated IoC Container - provides a single point of reference
    /// </summary>
    public static class perIoC
    {
        /// <summary>
        /// Register a type with the IoC container.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static void RegisterType<T>() where T: class
        {
            SimpleIoc.Default.Register<T>();
        }

        /// <summary>
        /// Register an type that implements the specified input (base class or interface) with the IoC Container.
        /// </summary>
        /// <typeparam name="TInput"></typeparam>
        /// <typeparam name="TOutput"></typeparam>
        public static void RegisterImplementation<TInput, TOutput>() where TInput : class where TOutput : class, TInput 
        {
            SimpleIoc.Default.Register<TInput, TOutput>();
        }

        /// <summary>
        /// Get the default (singleton) instance of a type from the IoC container.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GetInstance<T>()
        {
            return (T)GetInstance(typeof(T), string.Empty);
        }

        /// <summary>
        /// Get the instance of a type from the IoC container with the specified instanceId.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instanceId"></param>
        /// <returns></returns>
        public static T GetInstance<T>(string instanceId)
        {
            return (T)GetInstance(typeof(T), instanceId);
        }

        /// <summary>
        /// Get the default (singleton) instance of a type from the IoC container.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object GetInstance(Type type)
        {
            return GetInstance(type, string.Empty);
        }

        /// <summary>
        ///  Get the instance of a type from the IoC container with the specified instanceId. 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="instanceId"></param>
        /// <returns></returns>
        public static object GetInstance(Type type, string instanceId)
        {
            return SimpleIoc.Default.GetInstance(type, instanceId);
        }

        /// <summary>
        /// Remove a single instance of a type from the IoC container.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        public static void RemoveInstance<T>(T item) where T: class
        {
            SimpleIoc.Default.Unregister(item);
        }

        /// <summary>
        /// Remove all instances of the specified type from the IoC container.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static void UnRegisterType<T>() where T: class
        {
            SimpleIoc.Default.Unregister<T>();
        }
    }
}
