using GalaSoft.MvvmLight.Command;
using Peregrine.WPF.ViewModel;
using Peregrine.WPF.ViewModel.Command;
using Peregrine.WPF.ViewModel.Helpers;
using StaffManager.ViewModel.Messages;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Linq;
using GalaSoft.MvvmLight.Ioc;
using StaffManager.Model;
using StaffManager.ViewModel.ServiceContracts;

namespace StaffManager.ViewModel
{
    public class AllDepartmentsViewModel : perTreeViewItemViewModelBase
    {
        private readonly IStaffManagerNavigationService _navigationService;
        private readonly IStaffManagerDataService _dataService;

        public AllDepartmentsViewModel() : this(null, null)
        {
        }

        [PreferredConstructor]
        public AllDepartmentsViewModel(IStaffManagerNavigationService navigationService, IStaffManagerDataService dataService)
        {
            _navigationService = navigationService;
            _dataService = dataService;

            Caption = "All Departments";
            SetLazyLoadingMode();

            ViewDepartmentCommand = new perRelayCommand(OnViewDepartment, () => SelectedDepartment != null)
                .ObservesInternalProperty(this, nameof(SelectedDepartment));

            AddNewDepartmentCommand = new perRelayCommand(OnAddNewDepartment);
        }

        protected override Task<perTreeViewItemViewModelBase[]> LazyLoadFetchChildren()
        {
            var dvms = smViewModelFactory.GetAllDepartmentViewModels();
            return Task.FromResult(dvms.Cast<perTreeViewItemViewModelBase>().ToArray());
        }

        private DepartmentViewModel _selectedDepartment;

        public DepartmentViewModel SelectedDepartment
        {
            get => _selectedDepartment;
            set => Set(nameof(SelectedDepartment), ref _selectedDepartment, value);
        }

        public ICommand ViewDepartmentCommand { get; }

        private void OnViewDepartment()
        {
            perMessageService.SendMessage(new SelectItemMessage(SelectedDepartment));
        }

        public ICommand AddNewDepartmentCommand { get; }

        private void OnAddNewDepartment()
        {
            var department = new Department();

            if (_navigationService.EditModel(department))
            {
                _dataService.SaveDepartment(department);

                var departmentVm = smViewModelFactory.GetViewModelForModel(department);
                AddChild(departmentVm);
            }
        }
    }
}
