using Peregrine.WPF.ViewModel.DialogService.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Peregrine.WPF.ViewModel.DialogService
{
    /// <summary>
    /// For unit testing usages of perDialogService 
    /// Select the version with the desired result value for the interface implementation in the IoC container.
    /// </summary>
    public class perMockDialogReturnOk : IperDialogService
    {
        public Task ShowMessageAsync(object viewModel, string body, perDialogIcon dialogIcon = perDialogIcon.Information, string title = "")
        {
            return Task.CompletedTask;
        }

        public Task<perDialogButton> ShowDialogAsync(object viewModel, perDialogButton buttons, string body, perDialogIcon dialogIcon = perDialogIcon.Information, string title = "")
        {
            return Task.FromResult(ReturnValue);
        }

        public Task<perDialogButton> ShowContentDialogAsync(object viewModel, perDialogButton buttons, perDialogIcon dialogIcon = perDialogIcon.Information, string title = "", string tag = "")
        {
            return Task.FromResult(ReturnValue);
        }

        protected virtual perDialogButton ReturnValue => perDialogButton.Ok;

        public async Task<string> OpenFileDialogAsync(object viewModel, string title = "", string initialFolder = "")
        {
            return Path.Combine(await SelectFolderDialogAsync(viewModel).ConfigureAwait(false), "test.txt");
        }

        public async Task<IReadOnlyCollection<string>> OpenFilesDialogAsync(object viewModel, string title = "", string initialFolder = "")
        {
            var folder = await SelectFolderDialogAsync(viewModel).ConfigureAwait(false);

            var result = new List<string>
            {
                Path.Combine(folder, "Test1.txt"),
                Path.Combine(folder, "Test2.txt"),
                Path.Combine(folder, "Test3.txt")
            };

            return result.AsReadOnly();
        }

        public Task<string> SaveFileAsDialogAsync(object viewModel, string title = "", string initialFolder = "", string defaultExt = "", string filter = "")
        {
            return OpenFileDialogAsync(viewModel);
        }

        public Task<string> SelectFolderDialogAsync(object viewModel, string title = "", string initialFolder = "")
        {
            var folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Unit Tests", "Test");
            return Task.FromResult(folder);
        }

    }

    public class perMockDialogReturnYes : perMockDialogReturnOk
    {
        protected override perDialogButton ReturnValue => perDialogButton.Yes;
    }

    public class perMockDialogReturnNo : perMockDialogReturnOk
    {
        protected override perDialogButton ReturnValue => perDialogButton.No;
    }
    
    public class perMockDialogReturnCancel : perMockDialogReturnOk
    {
        protected override perDialogButton ReturnValue => perDialogButton.Cancel;
    }

    public class perMockDialogReturnRetry : perMockDialogReturnOk
    {
        protected override perDialogButton ReturnValue => perDialogButton.Retry;
    }

    public class perMockDialogReturnIgnore : perMockDialogReturnOk
    {
        protected override perDialogButton ReturnValue => perDialogButton.Ignore;
    }

    public class perMockDialogReturnAbort : perMockDialogReturnOk
    {
        protected override perDialogButton ReturnValue => perDialogButton.Abort;
    }
}
