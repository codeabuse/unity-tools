using UnityEngine;

namespace Codeabuse
{
    public class TypeSelectAttribute : PropertyAttribute
    {
        public string PropertyName { get; }
        public TypeSelectAttribute(string targetPropertyName)
        {
            PropertyName = targetPropertyName;
        }
    }
}