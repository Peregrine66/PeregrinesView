using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace Peregrine.Library
{
    public class perObservableCollection<T> : ObservableCollection<T>
    {
        public perObservableCollection()
        {
        }

        public perObservableCollection(IEnumerable<T> initialItems) : base(initialItems)
        {
        }

        public perObservableCollection(List<T> initialItems) : base(initialItems)
        {
        }

        public void Sort()
        {
            var internalList = Items as List<T>;

            if (internalList == null)
                return;

            internalList.Sort();
            RefreshObservers();
        }

        public void Sort(Comparison<T> comparison)
        {
            var internalList = Items as List<T>;

            if (internalList == null)
                return;

            internalList.Sort(comparison);
            RefreshObservers();
        }

        public void Sort(Comparer<T> comparer)
        {
            var internalList = Items as List<T>;

            if (internalList == null)
                return;

            internalList.Sort(comparer);
            RefreshObservers();
        }

        public void Sort(int index, int count, Comparer<T> comparer)
        {
            var internalList = Items as List<T>;

            if (internalList == null)
                return;

            internalList.Sort(index, count, comparer);
            RefreshObservers();
        }

        public void AddRange(IEnumerable<T> collection, bool sortAfterAdd = false)
        {
            var internalList = Items as List<T>;

            if (internalList == null)
                return;

            internalList.AddRange(collection);

            if (sortAfterAdd)
                Sort();
            else
                RefreshObservers();
        }

        public void AddRange(IEnumerable<T> collection, Comparison<T> comparison)
        {
            var internalList = Items as List<T>;

            if (internalList == null)
                return;

            internalList.AddRange(collection);
            Sort(comparison);
        }

        public void AddRange(IEnumerable<T> collection, Comparer<T> comparer)
        {
            var internalList = Items as List<T>;

            if (internalList == null)
                return;

            internalList.AddRange(collection);
            Sort(comparer);
        }

        protected void RefreshObservers()
        {
            OnPropertyChanged(new PropertyChangedEventArgs(nameof(Count)));
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }
    }
}
