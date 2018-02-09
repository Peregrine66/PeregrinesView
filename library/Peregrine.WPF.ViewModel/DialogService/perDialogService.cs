using System;
using System.Threading.Tasks;

namespace Peregrine.WPF.ViewModel.DialogService
{
    /// <summary>
    /// Clone of System.Windows.MessageBoxImage, so that ViewModel classes don't depend on any UI assembly
    /// </summary>
    public enum perDialogIcon
    {
        Asterisk,
        Error,
        Exclamation,
        Hand,
        Information,
        None,
        Question,
        Stop,
        Warning
    }

    /// <summary>
    /// Define the possible button / result values here, so that ViewModel classes don't depend on any UI assembly
    /// </summary>
    [Flags]
    public enum perDialogButton
    {
        None = 0,
        Ok = 1,
        Yes = 1 << 1,
        No = 1 << 2,
        Cancel = 1 << 3,
        Retry = 1 << 4,
        Ignore = 1 << 5,
        Abort = 1 << 6,
        Save = 1 << 7,
        YesNo = Yes | No,
        YesNoCancel = Yes | No | Cancel,
        OkCancel = Ok | Cancel,
        RetryCancel = Retry | Cancel,
        AbortRetryIgnore = Abort | Retry | Ignore,
        SaveCancel = Save | Cancel
    }

    public interface IperDialogService
    {
        Task ShowMessageAsync(object viewModel, string body, perDialogIcon dialogIcon = perDialogIcon.Information, string title = "");
        Task<perDialogButton> ShowDialogAsync(object viewModel, perDialogButton buttons, string body, perDialogIcon dialogIcon = perDialogIcon.Information, string title = "" );
        Task<perDialogButton> ShowContentDialogAsync(object viewModel, perDialogButton buttons, perDialogIcon dialogIcon = perDialogIcon.Information, string title = "", string tag = "");
    }
}