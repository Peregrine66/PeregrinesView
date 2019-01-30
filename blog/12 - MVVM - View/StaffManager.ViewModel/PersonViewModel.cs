using System;
using StaffManager.Model;

namespace StaffManager.ViewModel
{
    public class PersonViewModel : smViewModelBase<Person>, IComparable<PersonViewModel>
    {
        public PersonViewModel()
        {
            AddModelPropertyDependency(nameof(Model.FirstName), nameof(DisplayName));
            AddModelPropertyDependency(nameof(Model.LastName), nameof(DisplayName));
        }

        public string DisplayName => Model.FirstName + " " + Model.LastName;

        private bool _isSelected;

        public bool IsSelected
        {
            get => _isSelected;
            set => Set(nameof(IsSelected), ref _isSelected, value);
        }

        private DepartmentViewModel _departmentVm;

        public DepartmentViewModel DepartmentVm
        {
            get => _departmentVm;
            set
            {
                var oldDepartmentVm = DepartmentVm;

                if (Set(nameof(DepartmentVm), ref _departmentVm, value))
                {
                    // stop a potential circular loop
                    if (oldDepartmentVm != null && oldDepartmentVm.HasPersonVm(this))
                        oldDepartmentVm.RemovePerson(this);

                    Model.DepartmentId = value?.Model?.Id ?? 0;

                    // stop a potential circular loop
                    if (!DepartmentVm.HasPersonVm(this))
                        DepartmentVm.AddPerson(this);
                }
            }
        }

        // sort any PersonViewModel without a Department last in the list
        private int SortingDepartmentId => DepartmentVm?.Model?.Id ?? int.MaxValue;

        public int CompareTo(PersonViewModel other)
        {
            var result = SortingDepartmentId.CompareTo(other.SortingDepartmentId);
            if (result == 0)
                result = other.Model.IsManager.CompareTo(Model.IsManager); // sort true before false
            if (result == 0)
                result = string.Compare(Model.LastName, other.Model.LastName, StringComparison.InvariantCultureIgnoreCase);
            return result;
        }
    }
}