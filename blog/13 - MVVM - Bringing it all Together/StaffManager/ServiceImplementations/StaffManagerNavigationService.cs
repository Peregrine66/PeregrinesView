using System;
using Peregrine.WPF.ViewModel.IoC;
using StaffManager.Controls;
using StaffManager.Model;
using StaffManager.ViewModel;
using StaffManager.ViewModel.ServiceContracts;
using System.Windows;

namespace StaffManager.ServiceImplementations
{
    /// <inheritdoc />
    public class StaffManagerNavigationService : IStaffManagerNavigationService
    {
        /// <inheritdoc />
        public bool EditModel(dynamic model)
        {
            if (!(model is smModelBase))
                throw new ArgumentException("EditModel() called on a non-smModelBase item");

            if (!(smViewModelFactory.GetEditingViewModelForModel(model) is smViewModelBase editingViewModel))
                throw new InvalidOperationException($"No ViewModel type is defined for Model type ({model.GetType()})");

            var isNewModel = (model as smModelBase).Id == 0;

            editingViewModel.OperationDescription = isNewModel ? "Add New" : "Edit";
            editingViewModel.Clean(); // resets the IsDirty flag.

            var editDialogViewModel = perIoC.GetInstance<smEditDialogViewModel>();
            editDialogViewModel.EditingViewModel = editingViewModel;

            var editDialog = new smEditDialog
                                 {
                                     DataContext = editDialogViewModel,
                                     WindowStartupLocation = WindowStartupLocation.CenterOwner,
                                     Owner = Application.Current.MainWindow
                                 };

            var result = editDialog.ShowDialog().GetValueOrDefault();

            perIoC.RemoveInstance(editingViewModel);
            perIoC.RemoveInstance(editDialogViewModel);

            return result;
        }
    }
}
