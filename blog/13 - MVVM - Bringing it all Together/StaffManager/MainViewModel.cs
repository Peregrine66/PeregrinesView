using Peregrine.Library;
using Peregrine.WPF.Model;
using Peregrine.WPF.ViewModel;
using Peregrine.WPF.ViewModel.Command;
using Peregrine.WPF.ViewModel.Helpers;
using Peregrine.WPF.ViewModel.IoC;
using StaffManager.Model;
using StaffManager.ViewModel;
using StaffManager.ViewModel.Messages;
using StaffManager.ViewModel.ServiceContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;

namespace StaffManager
{
    public class MainViewModel : perObservableObject
    {
        private readonly IStaffManagerDataService _dataService;

        private readonly IStaffManagerNavigationService _navigationService;

        public MainViewModel(IStaffManagerDataService dataService, IStaffManagerNavigationService navigationService)
        {
            _dataService = dataService;
            _navigationService = navigationService;

            LoadDataCommand = new perRelayCommandAsync(RefreshDataAsync);
            perMessageService.RegisterMessageHandler<SelectItemMessage>(this, msg => SelectedItem = msg.SelectedItem);

            SelectPreviousSearchCommand = new perRelayCommand(OnSelectPreviousSearch, ()=>_peopleMatchingSearchCollection.Count > 1)
                .ObservesCollection(_peopleMatchingSearchCollection);

            SelectNextSearchCommand = new perRelayCommand(OnSelectNextSearch, ()=>_peopleMatchingSearchCollection.Count > 1)
                .ObservesCollection(_peopleMatchingSearchCollection);

            EditSelectedItemCommand = new perRelayCommand(OnEditSelectedItem, ()=>SelectedItem is smViewModelBase)
                .ObservesInternalProperty(this, nameof(SelectedItem));
        }

        public ICommand LoadDataCommand { get; }

        private async Task RefreshDataAsync()
        {
            await _dataService.InitialiseAsync().ConfigureAwait(false);
            var departments = _dataService.GetAllDepartments();
            smViewModelFactory.BuildAllDepartmentVms(departments);

            var rootTreeItem = perIoC.GetInstance<AllDepartmentsViewModel>();
            _rootTreeItemsList.Clear();
            _rootTreeItemsList.Add(rootTreeItem);

            // this will trigger lazy loading for the department items
            rootTreeItem.IsExpanded = true;

            SelectedItem = rootTreeItem;
        }

        private readonly perObservableCollection<AllDepartmentsViewModel> _rootTreeItemsList = new perObservableCollection<AllDepartmentsViewModel>();

        public IEnumerable<perTreeViewItemViewModelBase> RootTreeItems => _rootTreeItemsList;

        private perTreeViewItemViewModelBase _selectedItem;

        public perTreeViewItemViewModelBase SelectedItem
        {
            get => _selectedItem;
            set
            {
                Set(nameof(SelectedItem), ref _selectedItem, value);
                RaisePropertyChanged(nameof(ShowEditButton));

                value?.TriggerLazyLoading();
            }
        }

        public bool ShowEditButton => SelectedItem is smViewModelBase;

        public ICommand EditSelectedItemCommand { get; }

        /// <summary>
        /// Generic editor for any ViewModel (smViewModelBase) / Model (smModelBase) types
        /// </summary>
        private void OnEditSelectedItem()
        {
            if (!(SelectedItem is smViewModelBase selectedViewModel))
                throw new InvalidOperationException("OnEditSelectedItem() can only be called for items that descend from smViewModelBase");

            if (!(selectedViewModel.GetModel() is smModelBase selectedModel))
                throw new InvalidOperationException("No Model is set on the selected ViewModel");

            var modelToEdit = selectedModel.CloneExcludingId();

            // ensure that we get a different ViewModel instance
            modelToEdit.Id = selectedModel.Id * -1;

            if (_navigationService.EditModel(modelToEdit))
            {
                modelToEdit.AssignToExcludingId(selectedModel);
                selectedViewModel.OnModelSet();

                // update the UI in the case that the edit has caused this item to move within the TreeView
                Action action = () => SelectedItem = null;
                perDispatcherHelper.AddToQueue(action, DispatcherPriority.ApplicationIdle);
                action = () => SelectedItem = selectedViewModel;
                perDispatcherHelper.AddToQueue(action, DispatcherPriority.ApplicationIdle);
                var unused = perDispatcherHelper.ProcessQueueAsync();
            }
        }

        private string _searchText = string.Empty;

        public string SearchText
        {
            get => _searchText;
            set
            {
                if (Set(nameof(SearchText), ref _searchText, value))
                    GetPeopleMatchingSearch();
            }
        }

        private void GetPeopleMatchingSearch()
        {
            // Do the search in Model items rather than ViewModels, as not all possible PersonVm items 
            // are guaranteed to have been created yet.
            var peopleMatchingSearch = _dataService.GetAllPeopleMatchingSearch(SearchText);
            _peopleMatchingSearchCollection.Clear();
            _peopleMatchingSearchCollection.AddRange(peopleMatchingSearch);
            RaisePropertyChanged(nameof(NoSearchMatch));

            if (_peopleMatchingSearchCollection.Any())
                SearchIndex = 0;
        }

        private readonly perObservableCollection<Person> _peopleMatchingSearchCollection = new perObservableCollection<Person>();

        public bool NoSearchMatch => !string.IsNullOrWhiteSpace(SearchText) && !_peopleMatchingSearchCollection.Any();

        private int _searchIndex;

        private int SearchIndex
        {
            get => _searchIndex;
            set
            {
                _searchIndex = value;

                var searchPerson = _peopleMatchingSearchCollection[_searchIndex];

                // first ensure that the corresponding DepartmentVm is expanded (which creates all the child PersonVms)
                var departmentVm = smViewModelFactory.GetDepartmentViewModel(searchPerson.DepartmentId);
                Action action = () => departmentVm.IsExpanded = true;
                perDispatcherHelper.AddToQueue(action, DispatcherPriority.ContextIdle);

                // then select the corresponding PersonVm item
                var searchPersonVm = smViewModelFactory.GetViewModelForModel(searchPerson);
                action = () => SelectedItem = searchPersonVm;
                perDispatcherHelper.AddToQueue(action, DispatcherPriority.ApplicationIdle);

                var unused = perDispatcherHelper.ProcessQueueAsync();
            }
        }

        public ICommand SelectPreviousSearchCommand { get; }

        private void OnSelectPreviousSearch()
        {
            SearchIndex = (SearchIndex - 1 + _peopleMatchingSearchCollection.Count) % _peopleMatchingSearchCollection.Count;
        }

        public ICommand SelectNextSearchCommand { get; }

        private void OnSelectNextSearch()
        {
            SearchIndex = (SearchIndex + 1) % _peopleMatchingSearchCollection.Count;
        }
    }
}