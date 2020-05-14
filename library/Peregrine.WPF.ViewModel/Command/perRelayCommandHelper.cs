using Peregrine.WPF.ViewModel.WeakPropertyChanged;
using System;
using System.Collections.Specialized;
using System.ComponentModel;

namespace Peregrine.WPF.ViewModel.Command
{
    public static class perRelayCommandHelper
    {
        // use this when the command is owned by the source object - i.e. we aren't concerned about their relative lifespans
        public static perCommandBase ObservesInternalProperty(this perCommandBase command, INotifyPropertyChanged source, string propertyName)
        {
            source.PropertyChanged += (s, e) =>
            {
                if (string.Equals(e.PropertyName, propertyName, StringComparison.InvariantCultureIgnoreCase))
                    command.RaiseCanExecuteChanged();
            };

            return command;
        }

        // use this when the command is owned by an object (ViewModel) other than the source object
        // i.e. we need to take account of their relative lifespans - in particular if the source object is live for the whole application runtime
        public static perCommandBase ObservesExternalProperty(this perCommandBase command, INotifyPropertyChanged source, string propertyName)
        {
            perWeakPropertyChangedEventHandler.Register(source, propertyName, command, (listener, sender, args) => listener.RaiseCanExecuteChanged());

            return command;
        }

        public static perCommandBase ObservesCollection(this perCommandBase command, INotifyCollectionChanged collection)
        {
            collection.CollectionChanged += (s, e) => command.RaiseCanExecuteChanged();

            return command;
        }
    }
}