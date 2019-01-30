using Peregrine.WPF.ViewModel.IoC;
using StaffManager.Model;
using System.Collections.Generic;
using System.Linq;

namespace StaffManager.ViewModel
{
    /// <summary>
    /// Provides a mapping between a Model class and the corresponding Wrapper ViewModel
    /// </summary>
    public static class smViewModelFactory
    {
        static smViewModelFactory()
        {
            perIoC.RegisterImplementation<smViewModelBase<Person>, PersonViewModel>();
            perIoC.RegisterImplementation<smViewModelBase<Department>, DepartmentViewModel>();
        }

        /// <summary>
        /// Gets the Wrapper ViewModel for this Model instance.
        /// </summary>
        /// <remarks>
        /// Use the IoC container as ViewModel instances may require injected services.
        /// The call to perIoC.GetInstance() uses Model.Id as the instanceId, so that we only ever make one VM instance for each Model object.
        /// </remarks>
        /// <typeparam name="TModel">The type of the model</typeparam>
        /// <param name="model">The model instance</param>
        /// <returns>The VM instance</returns>
        public static smViewModelBase<TModel> GetViewModelForModel<TModel>(TModel model)
            where TModel : smModelBase
        {
            var result = GetViewModelForModel(model, false);

            // when we create a new department Vm, add it to the cache
            if (model is Department && !DepartmentVmCache.ContainsKey(model.Id))
            {
                DepartmentVmCache[model.Id] = result as DepartmentViewModel;
            }

            return result;
        }

        /// <summary>
        /// Gets the Wrapper ViewModel for this Model instance.
        /// The ViewModel has IsEditing set to true, prior to assigning the model.
        /// </summary>
        /// <remarks>
        /// Use the IoC container as ViewModel instances may require injected services.
        /// The call to perIoC.GetInstance() uses Model.Id as the instanceId, so that we only ever make one VM instance for each Model object.
        /// </remarks>
        /// <typeparam name="TModel">The type of the model</typeparam>
        /// <param name="model">The model instance</param>
        /// <returns>The VM instance</returns>
        public static smViewModelBase<TModel> GetEditingViewModelForModel<TModel>(TModel model)
            where TModel : smModelBase
        {
            return GetViewModelForModel(model, true);
        }

        private static smViewModelBase<TModel> GetViewModelForModel<TModel>(TModel model, bool isEditing) where TModel : smModelBase
        {
            var genericViewModelType = typeof(smViewModelBase<>).MakeGenericType(model.GetType());
            var vm = perIoC.GetInstance(genericViewModelType, model.Id.ToString());

            if (!(vm is smViewModelBase<TModel> result))
                return null;

            result.IsEditing = isEditing;
            result.Model = model;

            return result;
        }

        public static void RemoveInstance(smViewModelBase item)
        {
            perIoC.RemoveInstance(item);
        }

        /// <summary>
        /// Since department Vms are widely used throughout this demo application, build them
        /// all at startup and cache the instances
        /// </summary>
        private static readonly IDictionary<int, DepartmentViewModel> DepartmentVmCache = new Dictionary<int, DepartmentViewModel>();

        public static void BuildAllDepartmentVms(IEnumerable<Department> departments)
        {
            DepartmentVmCache.Clear();

            foreach (var department in departments)
                DepartmentVmCache[department.Id] = GetViewModelForModel(department) as DepartmentViewModel;
        }

        public static IReadOnlyCollection<DepartmentViewModel> GetAllDepartmentViewModels()
        {
            var result = DepartmentVmCache.Values.ToList();
            result.Sort();
            return result.AsReadOnly();
        }

        public static DepartmentViewModel GetDepartmentViewModel(int departmentId)
        {
            return DepartmentVmCache.ContainsKey(departmentId) ? DepartmentVmCache[departmentId] : null;
        }
    }
}
