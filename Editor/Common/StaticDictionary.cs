using System.Collections.Generic;

namespace Codeabuse
{
    public class StaticDictionary<TKey, TValue>
    {
        private static readonly Dictionary<TKey, TValue> _data = new();

        public static bool TryGetValue(TKey key, out TValue value)
        {
            return _data.TryGetValue(key, out value);
        }

        public static void Set(TKey key, TValue value)
        {
            _data[key] = value;
        }

        public static void Clear()
        {
            _data.Clear();
        }
    }
}