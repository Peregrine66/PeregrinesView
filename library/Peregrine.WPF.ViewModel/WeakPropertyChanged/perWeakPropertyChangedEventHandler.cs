using Peregrine.Library.Collections;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Peregrine.WPF.ViewModel.WeakPropertyChanged
{
    /// <summary>
    /// Manage PropertyChanged handlers using weak references for many INotifyPropertyChanged sources 
    /// </summary>
    public static class perWeakPropertyChangedEventHandler
    {
        private static readonly perWeakWeakDictionary<INotifyPropertyChanged, perWeakPropertyChangedProperties> PropertiesForSource
            = new perWeakWeakDictionary<INotifyPropertyChanged, perWeakPropertyChangedProperties>();

        /// <summary>
        /// Register a new handler to an INotifyPropertyChanged property.
        /// </summary>
        /// <remarks>
        ///  The three parameter handler action prevents the formation of a closure
        /// or other hard reference to the listener object.
        /// </remarks>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="propertyName"></param>
        /// <param name="listener"></param>
        /// <param name="handler"></param>
        public static void Register<T>(INotifyPropertyChanged source, string propertyName, T listener, Action<T, object, PropertyChangedEventArgs> handler) where T : class
        {
            if (!PropertiesForSource.ContainsKey(source))
                PropertiesForSource[source] = new perWeakPropertyChangedProperties(source);

            PropertiesForSource[source].AddListener(propertyName, listener, handler);
        }

        /// <summary>
        /// Stop listening to an INotifyPropertyChanged source object.
        /// </summary>
        /// <param name="source"></param>
        public static void UnRegisterSource(INotifyPropertyChanged source)
        {
            if (PropertiesForSource.ContainsKey(source))
                PropertiesForSource[source].CleanupEverything();

            PropertiesForSource.Remove(source);
        }

        /// <summary>
        /// Stop listening to a specific property on an INotifyPropertyChanged source object.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="propertyName"></param>
        public static void UnRegisterSourceProperty(INotifyPropertyChanged source, string propertyName)
        {
            if (PropertiesForSource.ContainsKey(source))
                PropertiesForSource[source].CleanupForProperty(propertyName);
        }

        /// <summary>
        /// Sets the minimum time between automatic checks for disposed
        /// listener or source objects.
        /// </summary>
        public static TimeSpan AutoCleanupInterval
        {
            get => PropertiesForSource.AutoCleanupInterval;
            set => PropertiesForSource.AutoCleanupInterval = value;
        }

        /// <summary>
        /// Manage a collection of property changed handlers for a single INotifyPropertyChanged source object
        /// </summary>
        private class perWeakPropertyChangedProperties
        {
            private readonly Dictionary<string, perLinkedList<IperWeakEventHandler>> _listenerHandlersForProperty 
                = new Dictionary<string, perLinkedList<IperWeakEventHandler>>();
            private readonly WeakReference<INotifyPropertyChanged> _weakSource;

            public perWeakPropertyChangedProperties(INotifyPropertyChanged source)
            {
                _weakSource = new WeakReference<INotifyPropertyChanged>(source);
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

            /// <summary>
            /// Add a new handler to the specified property.
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="propertyName"></param>
            /// <param name="listener"></param>
            /// <param name="handler"></param>
            public void AddListener<T>(string propertyName, T listener, Action<T, object, PropertyChangedEventArgs> handler) where T : class
            {
                if (!_listenerHandlersForProperty.ContainsKey(propertyName))
                    _listenerHandlersForProperty[propertyName] = new perLinkedList<IperWeakEventHandler>();

                _listenerHandlersForProperty[propertyName].AddNodeAtTail(new perWeakPropertyChangedListenerHandler<T>(listener, handler));
            }

            /// <summary>
            /// Remove all handlers for the managed object
            /// </summary>
            /// <param name="unRegisterSource"></param>
            public void CleanupEverything(bool unRegisterSource = false)
            {
                if (_weakSource.TryGetTarget(out var source))
                {
                    source.PropertyChanged -= PropertyChangedHandler;

                    if (unRegisterSource)
                        UnRegisterSource(source);
                }

                _listenerHandlersForProperty.Clear();
            }

            /// <summary>
            /// Remove all handlers for the specified property on the managed object
            /// </summary>
            /// <param name="propertyName"></param>
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

        /// <summary>
        /// A single listener / handler pair.
        /// </summary>
        /// <typeparam name="TListener"></typeparam>
        private class perWeakPropertyChangedListenerHandler<TListener> : IperWeakEventHandler<PropertyChangedEventArgs>
        {
            private readonly Action<TListener, object, PropertyChangedEventArgs> _handler;

            public perWeakPropertyChangedListenerHandler(object listener, Action<TListener, object, PropertyChangedEventArgs> handler)
            {
                ListenerReference = new WeakReference(listener);
                _handler = handler;
            }

            public WeakReference ListenerReference { get; }

            /// <summary>
            /// Trigger the handler when a property changed event is detected.
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="args"></param>
            /// <returns></returns>
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
