using System;
using UnityEngine;

namespace Codeabuse
{
    [AttributeUsage(AttributeTargets.Field)]
    public class WithInterfaceAttribute : PropertyAttribute
    {
        public Type InterfaceType { get; }
        public WithInterfaceAttribute(Type interfaceType)
        {
            if (!interfaceType.IsInterface)
                throw new ArgumentException($"{interfaceType.Name} is not an interface!");
            InterfaceType = interfaceType;
        }
    }
}