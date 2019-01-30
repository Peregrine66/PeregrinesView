using Peregrine.Library;
using StaffManager.Model;
using System;
using System.Collections.Generic;

namespace StaffManager.ViewModel
{
    public class PersonViewModel : smViewModelBase<Person>, IComparable<PersonViewModel>
    {
        public PersonViewModel() 
        {
            AddModelPropertyDependency(nameof(Model.FirstName), nameof(DisplayName));
            AddModelPropertyDependency(nameof(Model.LastName), nameof(DisplayName));

            AllDepartmentVmPairs = smViewModelFactory.GetAllDepartmentViewModels()
                .CreateSortedValuePairList(d => d.Model.Description);
        }

        public override void OnModelSet()
        {
            Caption = DisplayName;
            var departmentVmFromModel = smViewModelFactory.GetDepartmentViewModel(Model.DepartmentId);
            DepartmentVm = departmentVmFromModel;
        }

        public string DisplayName => Model.FirstName + " " + Model.LastName;

        private DepartmentViewModel _departmentVm;

        public DepartmentViewModel DepartmentVm
        {
            get => _departmentVm;
            set
            {
                var oldDepartmentVm = _departmentVm;

                if (Set(nameof(DepartmentVm), ref _departmentVm, value))
                {
                    Model.DepartmentId = value?.Model?.Id ?? 0;
                    IsDirty = true;

                    // don't add this item to the department's collection of people when editing
                    if (IsEditing)
                        return;

                    // stop a potential circular loop
                    if (oldDepartmentVm != null && oldDepartmentVm.HasPersonVm(this))
                        oldDepartmentVm.RemovePerson(this);

                    // stop a potential circular loop
                    if (_departmentVm != null && !_departmentVm.HasPersonVm(this))
                        _departmentVm.AddPerson(this);
                }
                else
                {
                    // Sort the department's people after reassigning the department following an edit
                    _departmentVm?.RefreshChildren();
                }
            }
        }

        public IEnumerable<perValueDisplayPair<DepartmentViewModel>> AllDepartmentVmPairs { get; }

        public int CompareTo(PersonViewModel other)
        {
            return Model.CompareTo(other.Model);
        }

        public override string ToString()
        {
            return $"{DisplayName} [{DepartmentVm}]";
        }

        protected override string ValidateProperty(string propertyName)
        {
            var result = base.ValidateProperty(propertyName);

            if (nameof(DepartmentVm).Equals(propertyName))
            {
                if (DepartmentVm == null)
                {
                    result = "Department is required";
                }
            }

            return result;
        }

    }
}