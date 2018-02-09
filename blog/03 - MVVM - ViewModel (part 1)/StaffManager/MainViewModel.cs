using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using StaffManager.Model;
using StaffManager.ViewModel;
using Peregrine.Library;

namespace StaffManager
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public MainViewModel()
        {
            LoadDataCommand = new Command(OnLoadData);
            AddPersonCommand = new Command(OnAddPerson);
            DeletePersonCommand = new Command(OnDeletePerson);
            ListSelectedPeopleCommand = new Command(OnListSelectedPeople);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ICommand LoadDataCommand { get; }

        private void OnLoadData()
        {
            var departments = new List<Department>
            {
                new Department {Id = 1, Description = "I.T."},
                new Department {Id = 2, Description = "Accounts"},
                new Department {Id = 3, Description = "Sales"},
                new Department {Id = 4, Description = "Logistics"}
            };

            DepartmentVms = departments.Select(d => new DepartmentViewModel {Model = d}).ToList();

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

            PeopleVms = people.Select(p => new PersonViewModel {Model = p}).ToList();

            foreach (var personVm in PeopleVms)
            {
                var departmentVm = DepartmentVms.FirstOrDefault(d => d.Model.Id == personVm.Model.DepartmentId);
                departmentVm?.AddPerson(personVm);
            }

            PeopleVms.Sort();
            SelectedPersonVm = PeopleVms.FirstOrDefault();
        }

        public ICommand AddPersonCommand { get; }

        private void OnAddPerson()
        {
            var people = PeopleVms;
            var newPersonVm = new PersonViewModel {Model = new Person()};
            people.Add(newPersonVm);
            PeopleVms = null;
            PeopleVms = people;
            SelectedPersonVm = newPersonVm;
        }

        public ICommand DeletePersonCommand { get; }

        private void OnDeletePerson()
        {
            var peopleVms = PeopleVms;
            var personVmToDelete = SelectedPersonVm;
            peopleVms.Remove(personVmToDelete);
            PeopleVms = null;
            PeopleVms = peopleVms;
            SelectedPersonVm = null;
        }

        public ICommand ListSelectedPeopleCommand { get; }

        private void OnListSelectedPeople()
        {
            var selectedpeople = PeopleVms.Where(p => p.IsSelected).ToList();

            var message = selectedpeople.Any()
                ? "The following people are selected\r\n    " + string.Join("\r\n    ", selectedpeople.Select(p => p.DisplayName))
                : "No people are selected";

            MessageBox.Show(message);
        }

        private List<PersonViewModel> _peopleVms;

        public List<PersonViewModel> PeopleVms
        {
            get { return _peopleVms; }
            set
            {
                _peopleVms = value;
                RaisePropertyChanged(nameof(PeopleVms));
            }
        }

        private List<DepartmentViewModel> _departmentVms;

        public List<DepartmentViewModel> DepartmentVms
        {
            get { return _departmentVms; }
            set
            {
                _departmentVms = value;
                RaisePropertyChanged(nameof(DepartmentVms));
                RaisePropertyChanged(nameof(DepartmentVmsForCombo));
            }
        }

        public IReadOnlyCollection<perValueDisplayPair<DepartmentViewModel>> DepartmentVmsForCombo => DepartmentVms.CreateSortedValuePairList(dvm => dvm.Model.Description);

        private PersonViewModel _selectedPersonVm;

        public PersonViewModel SelectedPersonVm
        {
            get { return _selectedPersonVm; }
            set
            {
                _selectedPersonVm = value;
                RaisePropertyChanged(nameof(SelectedPersonVm));
            }
        }
    }
}