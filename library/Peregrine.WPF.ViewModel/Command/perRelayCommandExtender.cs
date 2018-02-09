using GalaSoft.MvvmLight.Command;
using Peregrine.WPF.ViewModel.WeakPropertyChanged;
using System;
using System.Collections.Specialized;
using System.ComponentModel;

namespace Peregrine.WPF.ViewModel.Command
{
    public static class lcRelayCommandExtender
    {
        // This code is duplicated for the three command types as there is no common base class that has RaiseCanExecuteChanged() 

        // use this when the command is owned by the source object - i.e. we aren't concerned about their relative lifespans
        public static RelayCommand ObservesInternalProperty(this RelayCommand command, INotifyPropertyChanged source, string propertyName)
        {
            source.PropertyChanged += (s, e) =>
            {
                if (string.Equals(e.PropertyName, propertyName, StringComparison.InvariantCultureIgnoreCase))
                    command.RaiseCanExecuteChanged();
            };

            return command;
        }

        // use this when the command owned by an object (ViewModel) other than the source object
        // i.e. we need to take account of their relative lifespans - in particular if the source object is live for the whole application runtime
        public static RelayCommand ObservesExternalProperty(this RelayCommand command, INotifyPropertyChanged source, string propertyName)
        {
            perWeakPropertyChangedEventHandler.Register(source, propertyName, command,
                (listener, sender, args) => listener.RaiseCanExecuteChanged());

            return command;
        }

        public static RelayCommand<T> ObservesInternalProperty<T>(this RelayCommand<T> command, INotifyPropertyChanged source, string propertyName)
        {
            source.PropertyChanged += (s, e) =>
            {
                if (string.Equals(e.PropertyName, propertyName, StringComparison.InvariantCultureIgnoreCase))
                    command.RaiseCanExecuteChanged();
            };

            return command;
        }


        public static RelayCommand<T> ObservesExternalProperty<T>(this RelayCommand<T> command, INotifyPropertyChanged source, string propertyName)
        {
            perWeakPropertyChangedEventHandler.Register(source, propertyName, command,
                (listener, sender, args) => listener.RaiseCanExecuteChanged());

            return command;
        }

        public static perRelayCommandAsync ObservesInternalProperty(this perRelayCommandAsync command, INotifyPropertyChanged source, string propertyName)
        {
            source.PropertyChanged += (s, e) =>
            {
                if (string.Equals(e.PropertyName, propertyName, StringComparison.InvariantCultureIgnoreCase))
                    command.RaiseCanExecuteChanged();
            };

            return command;
        }

        public static perRelayCommandAsync ObservesExternalProperty(this perRelayCommandAsync command, INotifyPropertyChanged source, string propertyName)
        {
            perWeakPropertyChangedEventHandler.Register(source, propertyName, command,
                (listener, sender, args) => listener.RaiseCanExecuteChanged());

            return command;
        }

        public static RelayCommand ObservesCollection(this RelayCommand command, INotifyCollectionChanged collection)
        {
            collection.CollectionChanged += (s, e) => command.RaiseCanExecuteChanged();
            return command;
        }

        public static RelayCommand<T> ObservesCollection<T>(this RelayCommand<T> command, INotifyCollectionChanged collection)
        {
            collection.CollectionChanged += (s, e) => command.RaiseCanExecuteChanged();
            return command;
        }

        public static perRelayCommandAsync ObservesCollection(this perRelayCommandAsync command, INotifyCollectionChanged collection)
        {
            collection.CollectionChanged += (s, e) => command.RaiseCanExecuteChanged();
            return command;
        }
    }    
}