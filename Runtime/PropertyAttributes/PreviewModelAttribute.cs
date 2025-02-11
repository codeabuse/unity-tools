using System;
using UnityEngine;

namespace Codeabuse
{
    public class PreviewModelAttribute : PropertyAttribute
    {
        public string ModelPrefabKey { get; }
        public string AnimatorOverridePropertyPath { get; }

        public bool TryGetSelectPrefabFunction(object target, out Func<string, GameObject> selectPrefabFunction)
        {
            var targetType = target.GetType();
            var methods = targetType.GetMethods();
            selectPrefabFunction = null;
            foreach (var methodInfo in methods)
            {
                if (methodInfo.ReturnType != typeof(GameObject))
                    continue;
                var parameters = methodInfo.GetParameters();
                if (parameters.Length != 1 || parameters[0].ParameterType != typeof(string))
                    continue;
                selectPrefabFunction = 
                    Delegate.CreateDelegate(typeof(Func<string, GameObject>), methodInfo) as Func<string, GameObject>;
            }
            return selectPrefabFunction is not null;
        }
        
        public PreviewModelAttribute(string modelPrefabKey, string animatorOverridePropertyPath = null)
        {
            ModelPrefabKey = modelPrefabKey;
            AnimatorOverridePropertyPath = animatorOverridePropertyPath;
        }
    }
}