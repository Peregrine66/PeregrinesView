﻿using GalaSoft.MvvmLight;
using Peregrine.Library;
using Peregrine.WPF.ViewModel.Command;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Input;

namespace WpfColorspaceDemo
{
    public class MainViewModel: ViewModelBase
    {
        private const string RawDataFilesFolder = @".\RawDataFiles";

        public MainViewModel()
        {
            LoadedCommand = new perRelayCommand(OnLoaded);
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

                // Load the selected file in an async "fire and forget" manner.
                // No need for await as the continuation will process the data once it is fully read.
                perIOAsync.ReadAsciiTextFromFileAsync(value.FullName, TimeSpan.FromSeconds(1))
                    .ContinueWith(
                        async t =>
                        {
                            var taskResult = await t.ConfigureAwait(false);

                            if (taskResult.IsCompletedOk)
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
