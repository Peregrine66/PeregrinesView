using System;

namespace Peregrine.WPF.ViewModel.DialogService.Enums
{
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
}