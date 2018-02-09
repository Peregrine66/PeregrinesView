using StaffManager.Model;
using System.Collections.Generic;
using System.Linq;

namespace StaffManager.ViewModel
{
    public class DepartmentViewModel: smViewModelBase<Department>
    {
        private readonly HashSet<PersonViewModel> _peopleVmSet = new HashSet<PersonViewModel>();

        public IEnumerable<PersonViewModel> PeopleVms
        {
            get
            {
                var result = _peopleVmSet.ToList();
                result.Sort();
                return result;
            }
        }

        public bool HasPersonVm(PersonViewModel personVm)
        {
            return _peopleVmSet.Contains(personVm);
        }

        public void AddPerson(PersonViewModel personVm)
        {
            if (_peopleVmSet.Add(personVm))
                RaisePropertyChanged(nameof(PeopleVms));

            // stop a potential circular loop
            if (personVm.DepartmentVm != this)
                personVm.DepartmentVm = this;
        }

        public bool RemovePerson(PersonViewModel personVm)
        {
            var result = _peopleVmSet.Remove(personVm);

            if (result)
                RaisePropertyChanged(nameof(PeopleVms));

            // stop a potential circular loop
            if (personVm.DepartmentVm == this)
                personVm.DepartmentVm = null;

            return result;
        }
    }
}
