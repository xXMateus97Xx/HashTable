using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HashTable
{
    public class HashTable<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>
    {
        private const int MAX_BUCKET_LENGTH = 5;

        private HashTableNode<TKey, TValue>[][] _values;
        private int[] _stackPointers;
        private int _capacity;
        private int _length;

        public HashTable() : this(20)
        {
        }

        public HashTable(int initialCapacity)
        {
            _capacity = initialCapacity;
            _values = new HashTableNode<TKey, TValue>[initialCapacity][];
            _stackPointers = new int[initialCapacity];
        }

        public int Length { get { return _length; } }

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

            int hash;
            int position;
            var list = GetBucket(key, out hash, true);
            var node = GetNode(key, out position, list);
            if (node != null)
            {
                if (add)
                    throw new ArgumentException();
                list[position].Value = value;
            }
            else
            {
                while (_stackPointers[hash] == MAX_BUCKET_LENGTH)
                {
                    ReHash(_capacity * 2);
                    list = GetBucket(key, out hash, true);
                }
                position = _stackPointers[hash];
                list[position] = new HashTableNode<TKey, TValue>(key, value);
                _stackPointers[hash]++;
                _length++;
            }
        }

        private void ReHash(int newCapacity)
        {
            var newValues = new HashTableNode<TKey, TValue>[newCapacity][];
            var newPointers = new int[newCapacity];

            var bucketIndex = -1;
            foreach (var bucket in _values)
            {
                bucketIndex++;
                if (bucket == null)
                    continue;

                var oldStackPointer = _stackPointers[bucketIndex];
                var index = 0;
                foreach (var item in bucket)
                {
                    if (index >= oldStackPointer)
                        break;

                    var hash = Math.Abs(item.Key.GetHashCode() % newCapacity);
                    if (newValues[hash] == null)
                        newValues[hash] = new HashTableNode<TKey, TValue>[MAX_BUCKET_LENGTH];

                    var list = newValues[hash];
                    var stackPointer = newPointers[hash];
                    if (stackPointer == MAX_BUCKET_LENGTH)
                    {
                        ReHash(newCapacity * 2);
                        return;
                    }
                    else
                    {
                        newPointers[hash]++;
                        list[stackPointer] = item;
                    }
                    index++;
                }
            }

            _values = newValues;
            _stackPointers = newPointers;
            _capacity = newCapacity;
        }

        public TValue Get(TKey key)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            int position;
            var value = GetNode(key, out position);
            if (value == null)
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

            var value = GetNode(key, out position, list);
            if (value == null)
                return false;

            var lastValue = _stackPointers[hash];
            for (int i = position + 1; i <= lastValue; i++)
                list[i - 1] = list[i];

            list[lastValue] = null;

            _stackPointers[hash]--;
            _length--;
            return true;
        }

        public void Clear()
        {
            _values = new HashTableNode<TKey, TValue>[_capacity][];
            _stackPointers = new int[_capacity];
            _length = 0;
        }

        public bool ContainsKey(TKey key)
        {
            int position;
            var node = GetNode(key, out position);
            return node != null;
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

        private HashTableNode<TKey, TValue> GetNode(TKey key, out int position, HashTableNode<TKey, TValue>[] possibleValues = null)
        {
            int hash;
            if (possibleValues == null)
                possibleValues = GetBucket(key, out hash);

            position = -1;
            if (possibleValues == null)
                return null;

            for (int i = 0; i < possibleValues.Length && possibleValues[i] != null; i++)
            {
                var value = possibleValues[i];
                if (value.Key.Equals(key))
                {
                    position = i;
                    return value;
                }
            }

            return null;
        }

        private HashTableNode<TKey, TValue>[] GetBucket(TKey key, out int hash, bool create = false)
        {
            hash = Math.Abs(key.GetHashCode() % _capacity);
            var list = _values[hash];
            if (list == null && create)
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
                    if (item == null) continue;
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
