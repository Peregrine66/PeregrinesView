using GalaSoft.MvvmLight.Command;
using Peregrine.Library;
using System.Collections.Generic;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using System.IO;
using System.Linq;

namespace WpfColorspaceDemo
{
    public class MainViewModel: ViewModelBase
    {
        private const string RawDataFilesFolder = @".\RawDataFiles";

        public MainViewModel()
        {
            LoadedCommand = new RelayCommand(OnLoaded);
        }

        public ICommand LoadedCommand { get; }

        private void OnLoaded()
        {
            var rawDataFiles = PerIO.ListFiles(RawDataFilesFolder, "*.txt", false);
            _rawDataFileNames.Clear();
            _rawDataFileNames.AddRange(rawDataFiles);

            SelectedFile = _rawDataFileNames.FirstOrDefault();
        }

        private readonly perObservableCollection<FileInfo> _rawDataFileNames = new perObservableCollection<FileInfo>();

        public IEnumerable<FileInfo> RawDataFileNames => _rawDataFileNames;

        private FileInfo _selectedFile;

        public FileInfo SelectedFile
        {
            get => _selectedFile;
            set
            {
                Set(nameof(SelectedFile), ref _selectedFile, value);

                perIOAsync.ReadAsciiTextFromFileAsync(value.FullName)
                    .ContinueWith(
                        async t =>
                            {
                                var taskResult = await t;

                                if (taskResult.Status == perTaskStatus.CompletedOk)
                                {
                                    RawImage = ImageSerialiser.DeserialiseRawImage(taskResult.Data);
                                }
                            });
            }
        }

        private RawImage _rawImage;

        public RawImage RawImage
        {
            get => _rawImage;
            set => Set(nameof(RawImage), ref _rawImage, value);
        }
    }
}
