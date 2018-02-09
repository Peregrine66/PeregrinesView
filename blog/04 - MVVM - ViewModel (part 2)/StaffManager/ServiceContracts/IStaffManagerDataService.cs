using StaffManager.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StaffManager.ServiceContracts
{
    public interface IStaffManagerDataService
    {
        Task<IReadOnlyCollection<Person>> GetPeopleAsync();
        Task<IReadOnlyCollection<Department>> GetDepartmentsAsync();
    }
}
