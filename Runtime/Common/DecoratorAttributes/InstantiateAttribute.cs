using System;
using UnityEngine;

namespace Codeabuse
{
    public class InstantiateAttribute : PropertyAttribute
    {
        public Type BaseType { get; }

        public InstantiateAttribute(Type baseType)
        {
            BaseType = baseType;
        }
    }
}