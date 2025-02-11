using System;
using UnityEngine;

namespace Codeabuse
{
    public class PropertyChangedCallbackAttribute : PropertyAttribute
    {
        public string MethodName { get; }
        public Type ArgumentType { get; set; }
        
        public bool UpdateHierarchy { get; set; }
        
        public PropertyChangedCallbackAttribute(string methodName)
        {
            MethodName = methodName;
        }
    }
}