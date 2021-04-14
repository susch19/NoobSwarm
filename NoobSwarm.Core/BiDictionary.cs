using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace NoobSwarm
{
    public class BiDictionary<T, TU> : IEnumerable<KeyValuePair<T, TU>>
        where T : notnull
        where TU : notnull
    {
        private readonly Dictionary<T, TU> dict1;
        private readonly Dictionary<TU, T> dict2;

        public BiDictionary() : this(null, null) {}
        public BiDictionary(IEqualityComparer<T>? tComparer, IEqualityComparer<TU>? tuComparer) : this(0, tComparer, tuComparer) { }

        public BiDictionary(int capacity, IEqualityComparer<T>? tComparer = null, IEqualityComparer<TU>? tuComparer = null)
        {
            switch (capacity)
            {
                case < 0:
                    throw new ArgumentOutOfRangeException(nameof(capacity));
                case > 0:
                    dict1 = new (capacity, tComparer);
                    dict2 = new (capacity, tuComparer);
                    break;
                default:
                    dict1 = new (tComparer);
                    dict2 = new (tuComparer);
                    break;
            }
        }

        private static KeyValuePair<TB, TA> SwitchPairs<TA, TB>(KeyValuePair<TA, TB> pair) =>
            new KeyValuePair<TB, TA>(pair.Value, pair.Key);
        private static IEnumerable<KeyValuePair<TB, TA>> SwitchPairs<TA, TB>(IEnumerable<KeyValuePair<TA, TB>> collection)
        {
            return collection.Select(SwitchPairs);
        }

        public BiDictionary(IEnumerable<KeyValuePair<T, TU>> collection) : this(collection, null, null){}
        public BiDictionary(IEnumerable<KeyValuePair<T, TU>> collection, IEqualityComparer<T>? tComparer, IEqualityComparer<TU>? tuComparer)
        {
            var keyValuePairs = collection as KeyValuePair<T, TU>[] ?? collection.ToArray();
            dict1 = new (keyValuePairs, tComparer);
            dict2 = new (SwitchPairs(keyValuePairs), tuComparer);
        }

        public Dictionary<T, TU>.Enumerator GetEnumerator()
        {
            return dict1.GetEnumerator();
        }
        IEnumerator<KeyValuePair<T, TU>> IEnumerable<KeyValuePair<T,TU>>.GetEnumerator()
        {
            return GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(KeyValuePair<T, TU> item) => Add(item.Key, item.Value);

        public void Clear()
        {
            dict1.Clear();
            dict2.Clear();
        }

        public bool Contains(KeyValuePair<T, TU> item) => dict1.Contains(item);
        
        public bool Remove(KeyValuePair<T, TU> item)
        {
            dict1.Remove(item.Key);
            return dict2.Remove(item.Value);
        }

        public int Count => dict1.Count;
        public bool IsReadOnly => false;
        public void Add(T key, TU value)
        {
            dict1.Add(key, value);
            dict2.Add(value, key);
        }
        public void Add(TU key, T value) => Add(value, key);

        public bool ContainsKey(T key) => dict1.ContainsKey(key);

        public bool TryGetValue(T key, [MaybeNullWhen(false)] out TU value) => dict1.TryGetValue(key, out value);

        public IReadOnlyDictionary<T, TU> LeftToRight => dict1; 
        public IReadOnlyDictionary<TU, T> RightToLeft => dict2;
    }
}