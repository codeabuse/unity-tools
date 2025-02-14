using UnityEditor;

namespace Codeabuse
{
    /// <summary>
    /// The generic dictionary of values associated with SerializedProperties.
    /// Properties are distinguished by their sanitized path (<b>".Array.data[xx]."</b> in path replaced with <b>"$Array$"</b>),
    /// which allows to handle array elements as having the same property address.
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    public class PropertyCache<TValue> : StaticDictionary<int, TValue>
    {
        public static bool TryGetValue(SerializedProperty key, out TValue value) => TryGetValue(key.GetPropertyPathHashcode(), out value);
        
        public static void Set(SerializedProperty key, TValue value) => Set(key.GetPropertyPathHashcode(), value);
    }
}