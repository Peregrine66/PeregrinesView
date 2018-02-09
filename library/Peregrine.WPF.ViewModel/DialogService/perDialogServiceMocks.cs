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
