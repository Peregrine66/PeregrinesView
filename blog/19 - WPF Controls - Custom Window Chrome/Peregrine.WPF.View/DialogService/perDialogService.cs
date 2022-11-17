using Ookii.Dialogs.Wpf;
using Peregrine.Library;
using Peregrine.WPF.ViewModel.DialogService;
using Peregrine.WPF.ViewModel.DialogService.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Peregrine.WPF.View.DialogService
{
    public class perDialogService : IperDialogService
    {
        public Task ShowMessageAsync(object viewModel, string body, perDialogIcon dialogIcon = perDialogIcon.Information, string title = "")
        {
            return ShowDialogAsync(viewModel, perDialogButton.Ok, body, dialogIcon, title);
        }

        public Task<perDialogButton> ShowDialogAsync(object viewModel, perDialogButton buttons, string body, perDialogIcon dialogIcon = perDialogIcon.Information, string title = "")
        {
            return _ShowDialogAsync(viewModel, body, title, buttons, dialogIcon);
        }

        public Task<perDialogButton> ShowContentDialogAsync(object viewModel, perDialogButton buttons, perDialogIcon dialogIcon = perDialogIcon.Information, string title = "", string tag = "")
        {
            var associatedControl = perDialogServiceRegistration.GetAssociatedControl(viewModel);
            if (associatedControl == null)
            {
                throw new ArgumentException("No control associated with this view model");
            }

            var dialogContent = perDialogServiceRegistration.GetDialogContentForHost(associatedControl, tag);
            if (dialogContent == null)
            {
                throw new ArgumentException("No dialog content defined for this control");
            }

            dialogContent.DataContext = viewModel;

            return _ShowDialogAsync(viewModel, dialogContent, title, buttons, dialogIcon);
        }

        private async Task<perDialogButton> _ShowDialogAsync(object viewModel, object content, string title, perDialogButton buttons, perDialogIcon dialogIcon)
        {
            var associatedControl = perDialogServiceRegistration.GetAssociatedControl(viewModel);
            if (associatedControl == null)
            {
                throw new ArgumentException("No control associated with this view model");
            }

            return await associatedControl.Dispatcher.InvokeAsync(() =>
            {
                var window = new perDialog
                {
                    Title = title,
                    DialogContent = content,
                    DataContext = viewModel,
                    DialogIcon = dialogIcon,
                    Owner = associatedControl as Window ?? Application.Current.MainWindow,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner
                };

                var buttonsList = new List<perValueDisplayPair<perDialogButton>>();

                if ((buttons & perDialogButton.Ok) > 0)
                {
                    buttonsList.Add(perDialogButton.Ok.CreateValueDisplayPair());
                }

                if ((buttons & perDialogButton.Yes) > 0)
                {
                    buttonsList.Add(perDialogButton.Yes.CreateValueDisplayPair());
                }

                if ((buttons & perDialogButton.No) > 0)
                {
                    buttonsList.Add(perDialogButton.No.CreateValueDisplayPair());
                }

                if ((buttons & perDialogButton.Cancel) > 0)
                {
                    buttonsList.Add(perDialogButton.Cancel.CreateValueDisplayPair());
                }

                if ((buttons & perDialogButton.Abort) > 0)
                {
                    buttonsList.Add(perDialogButton.Abort.CreateValueDisplayPair());
                }

                if ((buttons & perDialogButton.Retry) > 0)
                {
                    buttonsList.Add(perDialogButton.Retry.CreateValueDisplayPair());
                }

                if ((buttons & perDialogButton.Ignore) > 0)
                {
                    buttonsList.Add(perDialogButton.Ignore.CreateValueDisplayPair());
                }

                if ((buttons & perDialogButton.Save) > 0)
                {
                    buttonsList.Add(perDialogButton.Save.CreateValueDisplayPair());
                }

                window.Buttons = buttonsList.ToArray();

                window.ShowDialog();

                return window.SelectedButton;
            });
        }

        public async Task<string> OpenFileDialogAsync(object viewModel, string title = "Open File ...", string initialFolder = "")
        {
            var associatedControl = perDialogServiceRegistration.GetAssociatedControl(viewModel);

            if (associatedControl?.Dispatcher == null)
            {
                throw new ArgumentException("No control associated with this view model");
            }

            return await associatedControl.Dispatcher.InvokeAsync(() =>
            {
                var dialog = new VistaOpenFileDialog
                {
                    InitialDirectory = initialFolder,
                    CheckFileExists = true,
                    Multiselect = false,
                    ReadOnlyChecked = false,
                    Title = title
                };

                return dialog.ShowDialog().GetValueOrDefault()
                    ? dialog.FileName
                    : string.Empty;
            });
        }

        public async Task<IReadOnlyCollection<string>> OpenFilesDialogAsync(object viewModel, string title = "Open File(s) ...", string initialFolder = "")
        {
            var associatedControl = perDialogServiceRegistration.GetAssociatedControl(viewModel);

            if (associatedControl?.Dispatcher == null)
            {
                throw new ArgumentException("No control associated with this view model");
            }

            return await associatedControl.Dispatcher.InvokeAsync(() =>
            {
                var dialog = new VistaOpenFileDialog
                {
                    InitialDirectory = initialFolder,
                    CheckFileExists = true,
                    Multiselect = true,
                    ReadOnlyChecked = false,
                    Title = title
                };

                var result = dialog.ShowDialog().GetValueOrDefault()
                    ? dialog.FileNames.ToList()
                    : new List<string>();

                return result.AsReadOnly();
            });
        }

        public async Task<string> SaveFileAsDialogAsync(object viewModel, string title = "Save File As ...", string initialFolder = "", string defaultExt = "", string filter = "")
        {
            var associatedControl = perDialogServiceRegistration.GetAssociatedControl(viewModel);

            if (associatedControl?.Dispatcher == null)
            {
                throw new ArgumentException("No control associated with this view model");
            }

            return await associatedControl.Dispatcher.InvokeAsync(() =>
            {
                var dialog = new VistaSaveFileDialog
                {
                    InitialDirectory = initialFolder,
                    CheckFileExists = false,
                    AddExtension = true,
                    DefaultExt = defaultExt,
                    Filter = filter,
                    OverwritePrompt = true,
                    Title = title
                };

                return dialog.ShowDialog().GetValueOrDefault()
                    ? dialog.FileName
                    : string.Empty;
            });
        }

        public async Task<string> SelectFolderDialogAsync(object viewModel, string title = "Select Folder ...", string initialFolder = "")
        {
            var associatedControl = perDialogServiceRegistration.GetAssociatedControl(viewModel);

            if (associatedControl?.Dispatcher == null)
            {
                throw new ArgumentException("No control associated with this view model");
            }

            return await associatedControl.Dispatcher.InvokeAsync(() =>
            {
                var dialog = new VistaFolderBrowserDialog
                {
                    Description = title,
                    UseDescriptionForTitle = true,
                    SelectedPath = initialFolder
                };

                return dialog.ShowDialog().GetValueOrDefault()
                    ? dialog.SelectedPath
                    : string.Empty;
            });
        }
    }
}