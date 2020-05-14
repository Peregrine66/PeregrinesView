using GalaSoft.MvvmLight;
using Peregrine.Library;
using Peregrine.WPF.ViewModel.Command;
using Peregrine.WPF.ViewModel.DialogService;
using StaffManager.Model;
using StaffManager.ViewModel;
using StaffManager.ViewModel.ServiceContracts;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace StaffManager
{
    public class MainViewModel : ObservableObject
    {
        private readonly IStaffManagerDataService _dataService;
        private readonly IperDialogService _dialogService;

        public MainViewModel(IStaffManagerDataService dataService, IperDialogService dialogService)
        {
            _dataService = dataService;
            _dialogService = dialogService;

            LoadDataCommand = new perRelayCommandAsync(OnLoadData);

            AddPersonCommand = new perRelayCommand(OnAddPerson);

            DeletePersonCommand = new perRelayCommand(OnDeletePerson, () => SelectedPersonVm != null)
                .ObservesInternalProperty(this, nameof(SelectedPersonVm));

            ListSelectedPeopleCommand = new perRelayCommandAsync(OnListSelectedPeople, ()=>_personVmList.Any())
                .ObservesCollection(_personVmList);

            _departmentVmList.CollectionChanged += (s, e) => RaisePropertyChanged(nameof(DepartmentVmsForCombo));
        }

        public ICommand LoadDataCommand { get; }

        private async Task OnLoadData()
        {
            await _dataService.InitialiseAsync().ConfigureAwait(false);

            var departments = _dataService.GetAllDepartments();
            var people = _dataService.GetAllPeople();

            _departmentVmList.Clear();
            _departmentVmList.AddRange(departments.Select(d => new DepartmentViewModel {Model = d}));

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

        private async Task OnListSelectedPeople()
        {
            var selectedpeople = PeopleVms.Where(p => p.IsSelected).ToList();

            var message = selectedpeople.Any()
                ? "The following people are selected\r\n    " + string.Join("\r\n    ", selectedpeople.Select(p => p.DisplayName))
                : "No people are selected";

            await _dialogService.ShowMessageAsync(this, message).ConfigureAwait(false);
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