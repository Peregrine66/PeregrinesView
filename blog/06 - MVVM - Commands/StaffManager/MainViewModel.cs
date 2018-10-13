using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using Peregrine.Library;
using StaffManager.Model;
using StaffManager.ServiceContracts;
using StaffManager.ViewModel;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Peregrine.WPF.ViewModel.Command;

namespace StaffManager
{
    public class MainViewModel : ObservableObject
    {
        private readonly IStaffManagerDataService _dataService;

        public MainViewModel() : this(null)
        {
        }

        [PreferredConstructor]
        public MainViewModel(IStaffManagerDataService dataService)
        {
            _dataService = dataService;

            LoadDataCommand = new perRelayCommandAsync(OnLoadData);

            AddPersonCommand = new RelayCommand(OnAddPerson);

            DeletePersonCommand = new RelayCommand(OnDeletePerson, () => SelectedPersonVm != null)
                .ObservesInternalProperty(this, nameof(SelectedPersonVm));

            ListSelectedPeopleCommand = new RelayCommand(OnListSelectedPeople, ()=>_personVmList.Any())
                .ObservesCollection(_personVmList);

            _departmentVmList.CollectionChanged += (s, e) => RaisePropertyChanged(nameof(DepartmentVmsForCombo));
        }

        public ICommand LoadDataCommand { get; }

        private async Task OnLoadData()
        {
            var getDepartmentsTask = _dataService.GetDepartmentsAsync();
            var getPeopleTask = _dataService.GetPeopleAsync();

            await Task.WhenAll(getPeopleTask, getDepartmentsTask).ConfigureAwait(true);

            var departments = await getDepartmentsTask.ConfigureAwait(true);
            _departmentVmList.Clear();
            _departmentVmList.AddRange(departments.Select(d => new DepartmentViewModel {Model = d}));

            var people = await getPeopleTask.ConfigureAwait(true);
            _personVmList.Clear();
            _personVmList.AddRange(people.Select(p => new PersonViewModel {Model = p}));

            foreach (var personVm in _personVmList)
            {
                var departmentVm = _departmentVmList.FirstOrDefault(d => d.Model.Id == personVm.Model.DepartmentId);
                departmentVm?.AddPerson(personVm);
            }

            _personVmList.Sort();
            SelectedPersonVm = _personVmList.FirstOrDefault();
        }

        public ICommand AddPersonCommand { get; }

        private void OnAddPerson()
        {
            var newPersonVm = new PersonViewModel { Model = new Person() };
            _personVmList.Add(newPersonVm);
            SelectedPersonVm = newPersonVm;
        }

        public ICommand DeletePersonCommand { get; }

        private void OnDeletePerson()
        {
            var personVmToRemove = SelectedPersonVm;
            SelectedPersonVm = null;
            _personVmList.Remove(personVmToRemove);
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

        private readonly perObservableCollection<PersonViewModel> _personVmList = new perObservableCollection<PersonViewModel>();

        public IEnumerable<PersonViewModel> PeopleVms => _personVmList;

        private readonly perObservableCollection<DepartmentViewModel> _departmentVmList = new perObservableCollection<DepartmentViewModel>();

        public IEnumerable<DepartmentViewModel> DepartmentVms => _departmentVmList;

        public IReadOnlyCollection<perValueDisplayPair<DepartmentViewModel>> DepartmentVmsForCombo => DepartmentVms.CreateSortedValuePairList(dvm => dvm.Model.Description);

        private PersonViewModel _selectedPersonVm;

        public PersonViewModel SelectedPersonVm
        {
            get => _selectedPersonVm;
            set => Set(nameof(SelectedPersonVm), ref _selectedPersonVm, value);
        }
    }
}