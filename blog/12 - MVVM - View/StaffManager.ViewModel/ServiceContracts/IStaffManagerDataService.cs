using System.Collections.ObjectModel;
using System.Threading.Tasks;
using StaffManager.Model;

namespace StaffManager.ViewModel.ServiceContracts
{
    public interface IStaffManagerDataService
    {
        Task InitialiseAsync();

        ReadOnlyCollection<Person> GetAllPeople();
        ReadOnlyCollection<Department> GetAllDepartments();
    }
}
