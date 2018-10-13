using System;
using System.Collections.Generic;
using System.Linq;

namespace Peregrine.Library.Collections
{
    // A generic dictionary, which allows both its keys and values to be garbage collected if there are no other references
    // to them other than from the dictionary itself.
    //
    // Based on Nick Guerrera: Presenting WeakDictionary[TKey, TValue]
    // https://blogs.msdn.microsoft.com/nicholg/2006/06/04/presenting-weakdictionarytkey-tvalue/
    //
    // If either the key or value of a particular entry in the dictionary has been collected, then both the key and value become effectively
    // unreachable. However, left-over perWeakReference objects for the key and value will physically remain in the dictionary until
    // RemoveCollectedEntries is called. This will lead to a discrepancy between the Count property and the number of iterations required
    // to visit all of the elements of the dictionary using its enumerator or those of the Keys and Values collections. Similarly,
    // CopyTo will copy fewer than Count elements in this situation.
    //
    // AutoCleanupInterval is the minimum time between automated checks for dead entries, done as part standard dictionary operations

    public sealed class perWeakWeakDictionary<TKey, TValue> : perBaseDictionary<TKey, TValue>
        where TKey : class
        where TValue : class
    {
        private DateTime _earliestCleanup = DateTime.MinValue;
        private readonly Dictionary<object, perWeakReference<TValue>> _dictionary;
        private readonly perWeakKeyComparer<TKey> _comparer;

        public perWeakWeakDictionary()
            : this(0, null)
        {
        }

        public perWeakWeakDictionary(int capacity)
            : this(capacity, null)
        {
        }

        public perWeakWeakDictionary(IEqualityComparer<TKey> comparer)
            : this(0, comparer)
        {
        }

        public perWeakWeakDictionary(int capacity, IEqualityComparer<TKey> comparer)
        {
            _comparer = new perWeakKeyComparer<TKey>(comparer);
            _dictionary = new Dictionary<object, perWeakReference<TValue>>(capacity, _comparer);
        }

        // Minimum time between automatically calling RemoveCollectedEntries() during dictionary operations
        public TimeSpan AutoCleanupInterval = TimeSpan.FromMinutes(1);

        // WARNING: The count returned here may include entries for which either the key or value objects have already been garbage collected. 
        // If you really need the exact number of live entries in the dictionary, call RemoveCollectedEntries() prior to using Count.
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

            var weakKey = new perWeakKeyReference<TKey>(key, _comparer);
            _dictionary[weakKey] = perWeakReference<TValue>.Create(value);
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

            if (_dictionary.TryGetValue(key, out var weakValue))
            {
                value = weakValue.Target;
                return weakValue.IsAlive;
            }

            value = null;
            return false;
        }

        protected override void SetValue(TKey key, TValue value)
        {
            CheckForCleanup();

            if (key == null)
                throw new ArgumentNullException(nameof(key));

            var weakKey = new perWeakKeyReference<TKey>(key, _comparer);
            _dictionary[weakKey] = perWeakReference<TValue>.Create(value);
        }

        public override void Clear() => _dictionary.Clear();

        public override IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            var liveItems = _dictionary
                .Select(pair => new {K = pair.Key as perWeakKeyReference<TKey>, V = pair.Value})
                .Where(x => x.K.IsAlive && x.V.IsAlive)
                .Select(x => new KeyValuePair<TKey, TValue>(x.K.Target, x.V.Target))
                .ToList();

            return liveItems.GetEnumerator();
        }

        private void CheckForCleanup()
        {
            var now = DateTime.Now;

            if (now <= _earliestCleanup)
                return;

            RemoveCollectedEntries();

            _earliestCleanup = now + AutoCleanupInterval;
        }

        // Removes the left-over weak references for entries in the dictionary whose key or value has already been reclaimed by the garbage
        // collector. This will reduce the dictionary's Count by the number of dead key-value pairs that were eliminated.
        public void RemoveCollectedEntries()
        {
            var keysToRemove = _dictionary
                .Select(pair => new {K = pair.Key as perWeakKeyReference<TKey>, V = pair.Value})
                .Where(x => !x.K.IsAlive || !x.V.IsAlive)
                .Select(x => x.K)
                .ToList();

            foreach (var key in keysToRemove)
                _dictionary.Remove(key);
        }
    }
}