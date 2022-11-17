﻿using System;

namespace Peregrine.WPF.ViewModel.Command
{
    /// <summary>
    /// Simple ICommand implementation
    /// </summary>
    public class perRelayCommand : perRelayCommandBase
    {
        private readonly Action _execute;
        private readonly Func<bool> _canExecute;

        public perRelayCommand(Action execute) : this(execute, () => true)
        {
        }

        public perRelayCommand(Action execute, Func<bool> canExecute)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public override bool CanExecute(object parameter) => _canExecute.Invoke();

        protected override void ExecuteInternal(object parameter)
        {
            _execute.Invoke();
        }
    }
}