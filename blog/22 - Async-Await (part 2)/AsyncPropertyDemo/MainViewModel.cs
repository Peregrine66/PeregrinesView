using Peregrine.WPF.ViewModel.Async;
using Peregrine.WPF.ViewModel.Command;
using System.Collections.Generic;
using System.Windows.Input;

namespace AsyncPropertyDemo
{
    public class MainViewModel
    {
        private readonly DataItemRepository _repository;

        public MainViewModel()
        {
            _repository = new DataItemRepository();

            DataItemsCollection = new perAsyncProperty<IReadOnlyCollection<DataItemViewModel>>(() => _repository.LoadData());

            ResetImagesCommand = new perRelayCommand(OnResetImages);
        }

        public perAsyncProperty<IReadOnlyCollection<DataItemViewModel>> DataItemsCollection { get; }

        public ICommand ResetImagesCommand { get; }

        private void OnResetImages()
        {
            if (DataItemsCollection.Value == null)
            {
                return;
            }

            foreach (var item in DataItemsCollection.Value)
            {
                item.ImageBytes.ResetValue();
            }
        }
    }
}
