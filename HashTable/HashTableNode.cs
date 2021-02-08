namespace HashTable
{
    internal struct HashTableNode<TKey, TValue>
    {
        public static HashTableNode<TKey, TValue> Default = new HashTableNode<TKey, TValue>();
        
        public HashTableNode(TKey key, TValue value)
        {
            Key = key;
            Value = value;
            Initialized = true;
        }

        public readonly TKey Key;
        public TValue Value;
        public readonly bool Initialized;
    }
}
