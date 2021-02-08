using System;
using System.Collections;
using System.Collections.Generic;

namespace HashTable
{
    public class HashTable<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>
    {
        private const int MAX_BUCKET_LENGTH = 5;

        private HashTableNode<TKey, TValue>[][] _values;
        private int _capacity;
        private int _length;

        public HashTable() : this(20)
        {
        }

        public HashTable(int initialCapacity)
        {
            _capacity = initialCapacity;
            _values = new HashTableNode<TKey, TValue>[initialCapacity][];
        }

        public int Length => _length;

        public void Add(TKey key, TValue value)
        {
            Add(key, value, true);
        }

        private void Add(TKey key, TValue value, bool add)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            if (value == null)
                throw new ArgumentNullException(nameof(value));

            var list = GetSetBucket(key);
            ref var node = ref GetNode(key, out _, list);
            if (node.Initialized)
            {
                if (add)
                    throw new ArgumentException($"Key already exists {key}");
                node.Value = value;
            }
            else
            {
                ref var lastNode = ref list[MAX_BUCKET_LENGTH - 1];
                while (lastNode.Initialized)
                {
                    ReHash(_capacity * 2);
                    list = GetSetBucket(key);
                    lastNode = ref list[MAX_BUCKET_LENGTH - 1];
                }

                int position;
                for (position = 0; position < list.Length; position++)
                {
                    ref var n = ref list[position];
                    if (!n.Initialized)
                        break;
                }

                list[position] = new HashTableNode<TKey, TValue>(key, value);

                _length++;
            }
        }

        private void ReHash(int newCapacity)
        {
            var newValues = new HashTableNode<TKey, TValue>[newCapacity][];

            foreach (var bucket in _values)
            {
                if (bucket == null)
                    continue;

                var index = 0;
                for(var i = 0; i < bucket.Length; i++)
                {
                    ref var item = ref bucket[i];
                    if (!item.Initialized)
                        break;

                    var hash = Math.Abs(item.Key.GetHashCode() % newCapacity);
                    HashTableNode<TKey, TValue>[] list = newValues[hash];
                    if (list == null)
                        list = newValues[hash] = new HashTableNode<TKey, TValue>[MAX_BUCKET_LENGTH];

                    ref var lastNode = ref list[MAX_BUCKET_LENGTH -1];
                    if (lastNode.Initialized)
                    {
                        ReHash(newCapacity * 2);
                        return;
                    }
                    else
                    {
                        int position;
                        for (position = 0; position < list.Length; position++)
                        {
                            ref var n = ref list[position];
                            if (!n.Initialized)
                                break;
                        }
                        list[position] = item;
                    }
                    index++;
                }
            }

            _values = newValues;
            _capacity = newCapacity;
        }

        public TValue Get(TKey key)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            ref var value = ref GetNode(key, out _);
            if (!value.Initialized)
                return default(TValue);
            return value.Value;
        }

        public bool Remove(TKey key)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            int hash;
            int position;

            var list = GetBucket(key, out hash);
            if (list == null)
                return false;

            ref var value = ref GetNode(key, out position, list);
            if (!value.Initialized)
                return false;

            list[position] = HashTableNode<TKey, TValue>.Default;

            int i;
            for (i = position + 1; i < list.Length; i++)
            {
                if (!list[i].Initialized)
                    break;
                list[i - 1] = list[i];
                list[i] = HashTableNode<TKey, TValue>.Default;
            }

            _length--;
            return true;
        }

        public void Clear()
        {
            _values = new HashTableNode<TKey, TValue>[_capacity][];
            _length = 0;
        }

        public bool ContainsKey(TKey key)
        {
            ref var node = ref GetNode(key, out _);
            return node.Initialized;
        }

        public TValue this[TKey key]
        {
            get
            {
                return Get(key);
            }
            set
            {
                Add(key, value, false);
            }
        }

        private ref HashTableNode<TKey, TValue> GetNode(TKey key, out int position, HashTableNode<TKey, TValue>[] possibleValues)
        {
            position = -1;
            if (possibleValues == null)
                return ref HashTableNode<TKey, TValue>.Default;

            for (int i = 0; i < possibleValues.Length; i++)
            {
                ref var value = ref possibleValues[i];
                if (!value.Initialized)
                    break;

                if (value.Key.Equals(key))
                {
                    position = i;
                    return ref value;
                }
            }

            return ref HashTableNode<TKey, TValue>.Default;
        }
        
        private ref HashTableNode<TKey, TValue> GetNode(TKey key, out int position)
        {
            var list = GetBucket(key, out _);
            return ref GetNode(key, out position, list);
        }

        private HashTableNode<TKey, TValue>[] GetBucket(TKey key, out int hash)
        {
            hash = Math.Abs(key.GetHashCode() % _capacity);
            return _values[hash];
        }

        private HashTableNode<TKey, TValue>[] GetSetBucket(TKey key)
        {
            var list = GetBucket(key, out var hash);
            if (list == null)
                list = _values[hash] = new HashTableNode<TKey, TValue>[MAX_BUCKET_LENGTH];
            return list;
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            foreach (var list in _values)
            {
                if (list == null) continue;
                foreach (var item in list)
                {
                    if (!item.Initialized) break;
                    yield return new KeyValuePair<TKey, TValue>(item.Key, item.Value);
                }
            }

            yield break;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
