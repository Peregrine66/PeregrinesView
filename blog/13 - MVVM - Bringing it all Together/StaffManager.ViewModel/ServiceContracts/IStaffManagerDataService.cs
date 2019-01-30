using System.Collections.ObjectModel;
using System.Threading.Tasks;
using StaffManager.Model;

namespace StaffManager.ViewModel.ServiceContracts
{
    public interface IStaffManagerDataService
    {
        Task InitialiseAsync();

        ReadOnlyCollection<Person> GetAllPeople();
        ReadOnlyCollection<Person> GetAllPeopleMatchingSearch(string searchText);
        ReadOnlyCollection<Person> GetPeopleForDepartment(int departmentId);
        ReadOnlyCollection<Department> GetAllDepartments();

        void SavePerson(Person person);
        void SaveDepartment(Department department);
    }
}
