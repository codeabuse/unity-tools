using UnityEngine;

namespace Codeabuse
{
    /// <summary>
    /// Allows to create a Dropdown to pick a value from the serialized Array/List of the same type
    /// as the field with that attribute. Field type should provide ToString() method to correctly present its value
    /// in the Dropdown control.
    /// </summary>
    public class SelectFromAttribute : PropertyAttribute
    {
        public string CollectionPropertyName { get; }
        
        public SelectFromAttribute(string collectionPropertyName)
        {
            CollectionPropertyName = collectionPropertyName;
        }
    }
}