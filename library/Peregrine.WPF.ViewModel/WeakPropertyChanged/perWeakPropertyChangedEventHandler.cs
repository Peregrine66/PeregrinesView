using Peregrine.Library.Collections;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Peregrine.WPF.ViewModel.WeakPropertyChanged
{
    // manage PropertyChanged handlers using weak references for many INotifyPropertyChanged sources
    public static class perWeakPropertyChangedEventHandler
    {
        private static readonly perWeakWeakDictionary<INotifyPropertyChanged, perWeakPropertyChangedProperties> PropertiesForSource
            = new perWeakWeakDictionary<INotifyPropertyChanged, perWeakPropertyChangedProperties>();

        public static void Register<T>(INotifyPropertyChanged source, string propertyName, T listener, Action<T, object, PropertyChangedEventArgs> handler) where T : class
        {
            if (!PropertiesForSource.ContainsKey(source))
                PropertiesForSource[source] = new perWeakPropertyChangedProperties(source);

            PropertiesForSource[source].AddListener(propertyName, listener, handler);
        }

        public static void UnregisterSource(INotifyPropertyChanged source)
        {
            if (PropertiesForSource.ContainsKey(source))
                PropertiesForSource[source].CleanupEverything();

            PropertiesForSource.Remove(source);
        }

        public static void UnregisterSourceProperty(INotifyPropertyChanged source, string propertyName)
        {
            if (PropertiesForSource.ContainsKey(source))
                PropertiesForSource[source].CleanupForProperty(propertyName);
        }

        public static TimeSpan AutoCleanupInterval
        {
            get => PropertiesForSource.AutoCleanupInterval;
            set => PropertiesForSource.AutoCleanupInterval = value;
        }

        // manage a collection of property names being observed for a single INotifyPropertyChanged source
        private class perWeakPropertyChangedProperties
        {
            private readonly Dictionary<string, perLinkedList<IperWeakEventHandler>> _listenerHandlersForProperty = new Dictionary<string, perLinkedList<IperWeakEventHandler>>();
            private readonly WeakReference<INotifyPropertyChanged> _weakSouce;

            public perWeakPropertyChangedProperties(INotifyPropertyChanged source)
            {
                _weakSouce = new WeakReference<INotifyPropertyChanged>(source);
                source.PropertyChanged += PropertyChangedHandler;
            }

            private void PropertyChangedHandler(object sender, PropertyChangedEventArgs args)
            {
                if (!_listenerHandlersForProperty.ContainsKey(args.PropertyName))
                    return;

                var handlerList = _listenerHandlersForProperty[args.PropertyName];

                foreach (var node in handlerList)
                {
                    var handler = (IperWeakEventHandler<PropertyChangedEventArgs>)node.Data;

                    if (!handler.Invoke(sender, args))
                        node.MarkForDeletion();
                }

                // check if all listeners for this property have been garbage collected
                if (handlerList.IsEmpty)
                    CleanupForProperty(args.PropertyName);
            }

            public void AddListener<T>(string propertyName, T listener, Action<T, object, PropertyChangedEventArgs> handler) where T : class
            {
                if (!_listenerHandlersForProperty.ContainsKey(propertyName))
                    _listenerHandlersForProperty[propertyName] = new perLinkedList<IperWeakEventHandler>();

                _listenerHandlersForProperty[propertyName].AddNodeAtTail(new perWeakPropertyChangedListenerHandler<T>(listener, handler));
            }

            public void CleanupEverything(bool unregisterSource = false)
            {
                if (_weakSouce.TryGetTarget(out var source))
                {
                    source.PropertyChanged -= PropertyChangedHandler;

                    if (unregisterSource)
                        UnregisterSource(source);
                }

                _listenerHandlersForProperty.Clear();
            }

            public void CleanupForProperty(string propertyName)
            {
                _listenerHandlersForProperty.Remove(propertyName);

                // check if there are any other properties left in _listenerHandlersForProperty
                if (_listenerHandlersForProperty.Any())
                    CleanupEverything(true);
            }
        }

        public interface IperWeakEventHandler
        {
            WeakReference ListenerReference { get; }
        }

        public interface IperWeakEventHandler<in TArgs> : IperWeakEventHandler
        {
            bool Invoke(object sender, TArgs args);
        }

        // a single listener / handler pair
        private class perWeakPropertyChangedListenerHandler<TListener> : IperWeakEventHandler<PropertyChangedEventArgs>
        {
            private readonly Action<TListener, object, PropertyChangedEventArgs> _handler;

            public perWeakPropertyChangedListenerHandler(object listener, Action<TListener, object, PropertyChangedEventArgs> handler)
            {
                ListenerReference = new WeakReference(listener);
                _handler = handler;
            }

            public WeakReference ListenerReference { get; }

            public bool Invoke(object sender, PropertyChangedEventArgs args)
            {
                if (!ListenerReference.IsAlive)
                    return false;

                var listener = (TListener)ListenerReference.Target;

                if (listener == null)
                    return false;

                _handler(listener, sender, args);
                return true;
            }
        }
    }
}
