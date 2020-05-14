using Peregrine.WPF.ViewModel.Async;

namespace AsyncPropertyDemo
{
    public class DataItemViewModel
    {
        public DataItemViewModel(string imageFilePath, string caption)
        {
            ImageBytes = new perBytesFromFileAsyncProperty(imageFilePath);
            Caption = caption;
        }

        public perBytesFromFileAsyncProperty ImageBytes { get; }

        public string Caption { get; }
    }
}
