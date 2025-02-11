using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Codeabuse
{
    public static class ReflectionExtensions
    {
        private const BindingFlags flags = BindingFlags.Instance | 
                                           BindingFlags.Public | 
                                           BindingFlags.NonPublic | 
                                           BindingFlags.FlattenHierarchy;
        
        private static readonly Dictionary<CallbackDescriptor, MethodInfo> methods_cached = new();
        
        public static MethodInfo GetNonPublicMethod(this Type targetType, string methodName, Type[] argumentTypes)
        {
            var descriptor = new CallbackDescriptor { Type = targetType, MethodName = methodName, ArgTypes = argumentTypes};
            if (methods_cached.TryGetValue(descriptor, out var callback))
                return callback;
            callback = targetType.GetMethod(
                    methodName, 
                    flags, 
                    null, 
                    argumentTypes, 
                    Array.Empty<ParameterModifier>());
            if (callback is not null)
                methods_cached[descriptor] = callback;
            return callback;
        }

        public static Type GetCollectionElementType(this Type containerType)
        {
            if (containerType.IsArray)
                return containerType.GetElementType();
            if (containerType.IsGenericType)
            {
                foreach (var genericArgument in containerType.GetGenericArguments())
                {
                    if (typeof(IEnumerable<>).MakeGenericType(genericArgument) is { } ienum)
                        return ienum;
                }
            }

            return null;
        }

        private record CallbackDescriptor
        {
            public Type Type;
            public string MethodName;
            public Type[] ArgTypes;
        }

        public static bool MatchGenericInterfaceImplementation(this Type baseInterfaceType, Type implementingType)
        {
            if (!baseInterfaceType.IsInterface)
                return false;
            
            var genericInterfaces = implementingType
                   .GetInterfaces()
                   .Where(i => i.GetGenericArguments().Length > 0);

            foreach (var genericInterface in genericInterfaces)
            {
                foreach (var genericArgument in genericInterface.GetGenericArguments())
                {
                    var sampleInterfaceType = baseInterfaceType.MakeGenericType(genericArgument);
                    if (sampleInterfaceType == genericInterface)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}