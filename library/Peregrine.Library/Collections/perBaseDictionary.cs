using System;
using System.Collections.Generic;
using System.Linq;

namespace Peregrine.Library.Collections
{
    // Represents a dictionary mapping keys to values.
    //
    // Provides the plumbing for the portions of IDictionary<TKey, TValue> which can reasonably be implemented without any
    // dependency on the underlying representation of the dictionary.
    //
    // based on Nick Guerrera: Presenting WeakDictionary[TKey, TValue]
    // https://blogs.msdn.microsoft.com/nicholg/2006/06/04/presenting-weakdictionarytkey-tvalue/

    public abstract class perBaseDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {
        private perKeyCollection _keys;
        private perValueCollection _values;

        public abstract int Count { get; }
        public abstract void Clear();
        public abstract void Add(TKey key, TValue value);
        public abstract bool ContainsKey(TKey key);
        public abstract bool Remove(TKey key);
        public abstract bool TryGetValue(TKey key, out TValue value);
        public abstract IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator();
        protected abstract void SetValue(TKey key, TValue value);

        public bool IsReadOnly => false;

        public ICollection<TKey> Keys => _keys ?? (_keys = new perKeyCollection(this));
        public ICollection<TValue> Values => _values ?? (_values = new perValueCollection(this));

        public TValue this[TKey key]
        {
            get
            {
                if (!TryGetValue(key, out var value))
                    throw new KeyNotFoundException();

                return value;
            }
            set => SetValue(key, value);
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            Add(item.Key, item.Value);
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return TryGetValue(item.Key, out var value)
                   && EqualityComparer<TValue>.Default.Equals(value, item.Value);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            Copy(this, array, arrayIndex);
        }

        public bool Remove(KeyValuePair<TKey, TValue> item) => Contains(item) && Remove(item.Key);

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();

        private static void Copy<T>(ICollection<T> source, T[] array, int arrayIndex)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));

            if (arrayIndex < 0 || arrayIndex > array.Length)
                throw new ArgumentOutOfRangeException(nameof(arrayIndex));

            if ((array.Length - arrayIndex) < source.Count)
                throw new ArgumentException("Destination array is not large enough. Check array.Length and arrayIndex.");

            foreach (var item in source)
                array[arrayIndex++] = item;
        }

        // ===============================================================================================

        private abstract class perDictionaryItemCollection<T> : ICollection<T>
        {
            protected readonly IDictionary<TKey, TValue> Dictionary;

            protected perDictionaryItemCollection(IDictionary<TKey, TValue> dictionary)
            {
                Dictionary = dictionary;
            }

            public int Count => Dictionary.Count;

            public bool IsReadOnly => true;

            public void CopyTo(T[] array, int arrayIndex)
            {
                Copy(this, array, arrayIndex);
            }

            public virtual bool Contains(T item)
            {
                return this.Any(element => EqualityComparer<T>.Default.Equals(element, item));
            }

            public IEnumerator<T> GetEnumerator()
            {
                return Dictionary.Select(GetItem).GetEnumerator();
            }

            protected abstract T GetItem(KeyValuePair<TKey, TValue> pair);

            public bool Remove(T item)
            {
                throw new NotSupportedException("Collection is read-only.");
            }

            public void Add(T item)
            {
                throw new NotSupportedException("Collection is read-only.");
            }

            public void Clear()
            {
                throw new NotSupportedException("Collection is read-only.");
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        private class perKeyCollection : perDictionaryItemCollection<TKey>
        {
            public perKeyCollection(IDictionary<TKey, TValue> dictionary)
                : base(dictionary)
            {
            }

            protected override TKey GetItem(KeyValuePair<TKey, TValue> pair)
            {
                return pair.Key;
            }

            public override bool Contains(TKey item)
            {
                return item != null
                       && Dictionary.ContainsKey(item);
            }
        }

        private class perValueCollection : perDictionaryItemCollection<TValue>
        {
            public perValueCollection(IDictionary<TKey, TValue> dictionary)
                : base(dictionary) { }

            protected override TValue GetItem(KeyValuePair<TKey, TValue> pair)
            {
                return pair.Value;
            }
        }
    }
}
