using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

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

        private void RaisePropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ICommand LoadDataCommand { get; }

        private void OnLoadData()
        {
            var people = new List<Person>
            {
                new Person {Id = 1, Department = "I.T.", FirstName = "Alan", LastName = "Jones", IsManager = false},
                new Person {Id = 2, Department = "I.T.", FirstName = "Joseph", LastName = "Preston", IsManager = true},
                new Person {Id = 3, Department = "IT", FirstName = "Stella", LastName = "Mcbride", IsManager = false},
                new Person {Id = 4, Department = "IT", FirstName = "Branden", LastName = "Owens", IsManager = false},
                new Person {Id = 5, Department = "I.T.", FirstName = "Leonard", LastName = "Marquez", IsManager = false},
                new Person {Id = 6, Department = "I.T.", FirstName = "Colin", LastName = "Brady", IsManager = false},
                new Person {Id = 7, Department = "Accounts", FirstName = "Callum", LastName = "Roberts", IsManager = true},
                new Person {Id = 8, Department = "Accounts", FirstName = "Jillian", LastName = "Scott", IsManager = false},
                new Person {Id = 9, Department = "Accounts", FirstName = "Calvin", LastName = "Moran", IsManager = false},
                new Person {Id = 10, Department = "Sales", FirstName = "Harlan", LastName = "Reid", IsManager = false},
                new Person {Id = 11, Department = "Sales", FirstName = "Felix", LastName = "Schroeder", IsManager = false},
                new Person {Id = 12, Department = "Sales", FirstName = "Joseph", LastName = "Smith", IsManager = true},
                new Person {Id = 13, Department = "Sales", FirstName = "Jasmine", LastName = "Emerson", IsManager = false},
                new Person {Id = 14, Department = "Sales", FirstName = "Lucas", LastName = "Edwards", IsManager = false},
                new Person {Id = 15, Department = "Sales", FirstName = "David", LastName = "Baxter", IsManager = false},
                new Person {Id = 16, Department = "Logistics", FirstName = "Kane", LastName = "Foreman", IsManager = false},
                new Person {Id = 17, Department = "Logistics", FirstName = "Laurel", LastName = "Curtis", IsManager = false},
                new Person {Id = 18, Department = "Logistics", FirstName = "Lucy", LastName = "Tanner", IsManager = true},
                new Person {Id = 19, Department = "Logistics", FirstName = "Christian", LastName = "Pittman", IsManager = false},
                new Person {Id = 20, Department = "Logistics", FirstName = "Patricia", LastName = "Wilkinson", IsManager = false}
            };

            people.Sort();
            People = people;
            SelectedPerson = people.FirstOrDefault();
        }

        public ICommand AddPersonCommand { get; }

        private void OnAddPerson()
        {
            var people = People;
            var newPerson = new Person();
            people.Add(newPerson);
            People = null;
            People = people;
            SelectedPerson = newPerson;
        }

        public ICommand DeletePersonCommand { get; }

        private void OnDeletePerson()
        {
            var people = People;
            var personToDelete = SelectedPerson;
            people.Remove(personToDelete);
            People = null;
            People = people;
            SelectedPerson = null;
        }

        public ICommand ListSelectedPeopleCommand { get; }

        private void OnListSelectedPeople()
        {
            var selectedpeople = People.Where(p => p.IsSelected).ToList();

            var message = selectedpeople.Any()
                ? "The following people are selected\r\n    " + string.Join("\r\n    ", selectedpeople.Select(p=>p.DisplayName)) 
                : "No people are selected";

            MessageBox.Show(message);
        }

        private List<Person> _people;

        public List<Person> People
        {
            get => _people;
            set
            {
                _people = value;
                RaisePropertyChanged("People");
            }
        }

        private Person _selectedPerson;

        public Person SelectedPerson
        {
            get => _selectedPerson;
            set
            {
                _selectedPerson = value;
                RaisePropertyChanged("SelectedPerson");
            }
        }
    }
}