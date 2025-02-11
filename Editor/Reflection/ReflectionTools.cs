using System;
using System.Linq;
using System.Reflection;
using UnityEditor;

namespace Codeabuse
{
    public static class ReflectionTools
    {
        [InitializeOnLoadMethod]
        [MenuItem("Tools/Clear SerializedProperty Methods Cache")]
        static void ClearCache()
        {
            PropertyCache<MethodInfo>.Clear();
        }
        
        
        public static bool FindTargetMethodAndParameters(
                string methodName, 
                SerializedProperty property, 
                Type propertyDeclaringType, 
                out CachedMethod buttonTarget)
        {
            if (!PropertyCache<MethodInfo>.TryGetValue(property, out var targetMethod))
            {
                if (!property.GetFieldInfoAndStaticType(out _, out var propertyType))
                {
                    buttonTarget = default;
                    return false;
                }
                
                var targetMethods = propertyDeclaringType
                       .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                       .Where(m => m.Name == methodName)
                       .Where(m =>
                        {
                            var parameters = m.GetParameters();
                            return parameters.Length == 0 ||
                                   parameters.Length == 1 && parameters[0].ParameterType == propertyType;
                        })
                       .ToArray();

                targetMethod =
                        targetMethods.FirstOrDefault(m => m.GetParameters()
                               .FirstOrDefault(p => p.ParameterType == propertyType) is { })
                        ??
                        targetMethods.FirstOrDefault();
                
                PropertyCache<MethodInfo>.Set(property, targetMethod);
            }
            
            if (targetMethod is null)
            {
                buttonTarget = default;
                return false;
            }

            var paramsCount = targetMethod.GetParameters().Length;

            object[] GetParameters() => 
                    paramsCount == 1 ? 
                            new[] { property.GetValueBoxed() } : 
                            Array.Empty<object>();

            var target = property.GetPropertyOwnerObject();
            buttonTarget = new CachedMethod(target, targetMethod, GetParameters);
            return true;
        }
    }
}