using System;
using System.Collections.Generic;
using System.Linq;

namespace Peregrine.Library.Collections
{
    // A generic dictionary, with concrete keys but weak references to values - which allows them to be garbage collected if there are no other references
    // to them other than from the dictionary itself.
    //
    // Based on Nick Guerrera: Presenting WeakDictionary[TKey, TValue]
    // https://blogs.msdn.microsoft.com/nicholg/2006/06/04/presenting-weakdictionarytkey-tvalue/
    //
    // Left-over perWeakReference objects for the value will physically remain in the dictionary until RemoveCollectedEntries is called. 
    // This will lead to a discrepancy between the Count property and the number of iterations required to visit all of the elements of the 
    // dictionary using its enumerator or those of the Keys and Values collections. Similarly, CopyTo will copy fewer than Count elements in this situation.
    //
    // AutoCleanupInterval is the minimum time between automated checks for dead entries, done as part standard dictionary operations

    public sealed class perWeakDictionary<TKey, TValue> : perBaseDictionary<TKey, TValue>
        where TValue : class
    {
        private DateTime _nextCleanup = DateTime.MinValue;
        private readonly Dictionary<TKey, perWeakReference<TValue>> _dictionary;

        public perWeakDictionary()
            : this(0)
        {
        }

        public perWeakDictionary(int capacity)
        {
            _dictionary = new Dictionary<TKey, perWeakReference<TValue>>(capacity);
        }

        // Minimum time between automatically calling RemoveCollectedEntries() during dictionary operations
        public TimeSpan AutoCleanupInterval = TimeSpan.FromMinutes(1);

        // WARNING: The count returned here may include entries for which the value object has already been garbage collected. 
        // If you really need the number of live entries in the dictionary, call RemoveCollectedEntries prior to using Count.
        public override int Count
        {
            get
            {
                CheckForCleanup();
                return _dictionary.Count;
            }
        }

        public override void Add(TKey key, TValue value)
        {
            CheckForCleanup();

            if (key == null)
                throw new ArgumentNullException(nameof(key));

            var weakValue = perWeakReference<TValue>.Create(value);
            _dictionary[key] = weakValue;
        }

        public override bool ContainsKey(TKey key)
        {
            CheckForCleanup();
            return _dictionary.ContainsKey(key);
        }

        public override bool Remove(TKey key)
        {
            CheckForCleanup();
            return _dictionary.Remove(key);
        }

        public override bool TryGetValue(TKey key, out TValue value)
        {
            CheckForCleanup();
            perWeakReference<TValue> weakValue;
            if (_dictionary.TryGetValue(key, out weakValue) && weakValue.IsAlive)
            {
                value = weakValue.Target;
                return true;
            }

            value = null;
            return false;
        }

        protected override void SetValue(TKey key, TValue value)
        {
            CheckForCleanup();
            _dictionary[key] = perWeakReference<TValue>.Create(value);
        }

        public override void Clear() => _dictionary.Clear();

        public override IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            var liveItems = _dictionary
                .Where(pair => pair.Value.IsAlive)
                .Select(pair => new KeyValuePair<TKey, TValue>(pair.Key, pair.Value.Target))
                .ToList();

            return liveItems.GetEnumerator();
        }

        // Removes the left-over weak references for entries in the dictionary whose value has already been reclaimed by the garbage
        // collector. This will reduce the dictionary's Count by the number of dead key-value pairs that were eliminated.
        public void RemoveCollectedEntries()
        {
            var keysToRemove = _dictionary.Where(pair => !pair.Value.IsAlive).Select(pair => pair.Key).ToList();
            foreach (var key in keysToRemove)
                _dictionary.Remove(key);
        }

        private void CheckForCleanup()
        {
            var now = DateTime.Now;

            if (now <= _nextCleanup)
                return;

            RemoveCollectedEntries();
            _nextCleanup = now + AutoCleanupInterval;
        }
    }
}