using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using Peregrine.WPF.ViewModel;
using Peregrine.WPF.ViewModel.Command;
using Peregrine.WPF.ViewModel.Helpers;
using StaffManager.Model;
using StaffManager.ViewModel.Messages;
using StaffManager.ViewModel.ServiceContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace StaffManager.ViewModel
{
    public class DepartmentViewModel : smViewModelBase<Department>, IComparable<DepartmentViewModel>
    {
        private readonly HashSet<PersonViewModel> _peopleVmSet = new HashSet<PersonViewModel>();
        private readonly IStaffManagerDataService _dataService;
        private readonly IStaffManagerNavigationService _navigationService;

        // parameter-less constructor for Xaml designer
        public DepartmentViewModel()
            : this(null, null)
        {
            Model = new MockDepartment();
        }

        [PreferredConstructor]
        public DepartmentViewModel(IStaffManagerNavigationService navigationService, IStaffManagerDataService dataService)
        {
            _dataService = dataService;
            _navigationService = navigationService;

            // creates a expansion toggle button in lazy loading mode
            SetLazyLoadingMode();

            ViewPersonCommand = new perRelayCommand(OnViewPerson, () => SelectedPerson != null)
                .ObservesInternalProperty(this, nameof(SelectedPerson));

            AddNewPersonCommand = new perRelayCommand(OnAddNewPerson);
        }

        public override void OnModelSet()
        {
            Caption = Model.Description;
        }

        protected override IEnumerable<perTreeViewItemViewModelBase> LazyLoadChildren => PeopleVms;

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

        protected override Task<perTreeViewItemViewModelBase[]> LazyLoadFetchChildren()
        {
            var people = _dataService.GetPeopleForDepartment(Model.Id);

            var peopleVms = people.Select(smViewModelFactory.GetViewModelForModel).ToList();

            foreach (var personVm in peopleVms.Cast<PersonViewModel>())
                _peopleVmSet.Add(personVm);

            return Task.FromResult(peopleVms.Cast<perTreeViewItemViewModelBase>().ToArray());
        }

        public void AddPerson(PersonViewModel person)
        {
            if (_peopleVmSet.Add(person))
                RefreshChildren();

            SetChildPropertiesFromParent(person);
        }

        public void RemovePerson(PersonViewModel person)
        {
            if (_peopleVmSet.Remove(person))
                RefreshChildren();

            ReCalculateNodeCheckState();
        }

        private PersonViewModel _selectedPerson;

        public PersonViewModel SelectedPerson
        {
            get => _selectedPerson;
            set => Set(nameof(SelectedPerson), ref _selectedPerson, value);
        }

        public ICommand ViewPersonCommand { get; }

        private void OnViewPerson()
        {
            perMessageService.SendMessage(new SelectItemMessage(SelectedPerson));
        }

        public int CompareTo(DepartmentViewModel other)
        {
            return Model.CompareTo(other.Model);
        }

        public override string ToString()
        {
            return Model?.Description ?? "[Null]";
        }

        public ICommand AddNewPersonCommand { get; }

        private void OnAddNewPerson()
        {
            var person = new Person
            {
                DepartmentId = Model.Id
            };

            if (_navigationService.EditModel(person))
            {
                _dataService.SavePerson(person);

                var personVm = smViewModelFactory.GetViewModelForModel(person);
            }
        }
    }
}
