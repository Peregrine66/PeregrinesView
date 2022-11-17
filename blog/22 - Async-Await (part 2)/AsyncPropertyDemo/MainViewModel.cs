using Peregrine.WPF.ViewModel.Async;
using Peregrine.WPF.ViewModel.Command;
using System.Collections.Generic;
using System.Windows.Input;

namespace AsyncPropertyDemo
{
    public class MainViewModel
    {
        public MainViewModel()
        {
            var repository = new DataItemRepository();

            DataItemsCollection = new perAsyncProperty<IReadOnlyCollection<DataItemViewModel>>(() => repository.LoadData());

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
