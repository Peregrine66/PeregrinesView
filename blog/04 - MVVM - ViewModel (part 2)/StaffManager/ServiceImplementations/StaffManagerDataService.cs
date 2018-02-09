using System;
using StaffManager.Model;
using StaffManager.ServiceContracts;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace StaffManager.ServiceImplementations
{
    public class StaffManagerDataService: IStaffManagerDataService
    {
        public async Task<IReadOnlyCollection<Person>> GetPeopleAsync()
        {
            await Task.Delay(TimeSpan.FromSeconds(1)).ConfigureAwait(false);

            var people = new List<Person>
            {
                new Person {Id = 1, DepartmentId = 1, FirstName = "Alan", LastName = "Jones", IsManager = false},
                new Person {Id = 2, DepartmentId = 1, FirstName = "Joseph", LastName = "Preston", IsManager = true},
                new Person {Id = 3, DepartmentId = 1, FirstName = "Stella", LastName = "Mcbride", IsManager = false},
                new Person {Id = 4, DepartmentId = 1, FirstName = "Branden", LastName = "Owens", IsManager = false},
                new Person {Id = 5, DepartmentId = 1, FirstName = "Leonard", LastName = "Marquez", IsManager = false},
                new Person {Id = 6, DepartmentId = 1, FirstName = "Colin", LastName = "Brady", IsManager = false},
                new Person {Id = 7, DepartmentId = 2, FirstName = "Callum", LastName = "Roberts", IsManager = true},
                new Person {Id = 8, DepartmentId = 2, FirstName = "Jillian", LastName = "Scott", IsManager = false},
                new Person {Id = 9, DepartmentId = 2, FirstName = "Calvin", LastName = "Moran", IsManager = false},
                new Person {Id = 10, DepartmentId = 3, FirstName = "Harlan", LastName = "Reid", IsManager = false},
                new Person {Id = 11, DepartmentId = 3, FirstName = "Felix", LastName = "Schroeder", IsManager = false},
                new Person {Id = 12, DepartmentId = 3, FirstName = "Joseph", LastName = "Smith", IsManager = true},
                new Person {Id = 13, DepartmentId = 3, FirstName = "Jasmine", LastName = "Emerson", IsManager = false},
                new Person {Id = 14, DepartmentId = 3, FirstName = "Lucas", LastName = "Edwards", IsManager = false},
                new Person {Id = 15, DepartmentId = 3, FirstName = "David", LastName = "Baxter", IsManager = false},
                new Person {Id = 16, DepartmentId = 4, FirstName = "Kane", LastName = "Foreman", IsManager = false},
                new Person {Id = 17, DepartmentId = 4, FirstName = "Laurel", LastName = "Curtis", IsManager = false},
                new Person {Id = 18, DepartmentId = 4, FirstName = "Lucy", LastName = "Tanner", IsManager = true},
                new Person {Id = 19, DepartmentId = 4, FirstName = "Christian", LastName = "Pittman", IsManager = false},
                new Person {Id = 20, DepartmentId = 4, FirstName = "Patricia", LastName = "Wilkinson", IsManager = false}
            };

            return new ReadOnlyCollection<Person>(people);
        }

        public async Task<IReadOnlyCollection<Department>> GetDepartmentsAsync()
        {
            await Task.Delay(TimeSpan.FromSeconds(1)).ConfigureAwait(false);

            var departments = new List<Department>
            {
                new Department {Id = 1, Description = "I.T."},
                new Department {Id = 2, Description = "Accounts"},
                new Department {Id = 3, Description = "Sales"},
                new Department {Id = 4, Description = "Logistics"}
            };

            return new ReadOnlyCollection<Department>(departments);
        }
    }
}
