using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Codeabuse.EditorTools
{
    public static class ObjectEditorExtensions
    {
        [InitializeOnLoadMethod]
        private static void Init()
        {
            CompilationPipeline.compilationFinished -= OnScriptsRecompiled;
            CompilationPipeline.compilationFinished += OnScriptsRecompiled;
        }

        private static void OnScriptsRecompiled(object obj)
        {
            TypeAssociatedCache<MethodInfo[]>.Clear();
        }
        
        public static IEnumerable<ContextFunction> GetContextFunctions(this Object target)
        {
            var targetType = target.GetType();
            if (!TypeAssociatedCache<MethodInfo[]>.TryGetValue(targetType, out var methods))
            {
                methods = targetType
                       .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                       .Where(method => method.GetCustomAttribute<ContextMenu>() is { } &&
                                        method.GetParameters().Length == 0)
                       .ToArray();
                
                TypeAssociatedCache<MethodInfo[]>.Set(targetType, methods);
            }

            return methods.Select(method => new ContextFunction(target, method, method.Name));
        }

        public static bool IsSceneObject(this GameObject gameObject)
        {
            return !string.IsNullOrEmpty(gameObject.scene.name);
        }

        public static bool IsSceneObject(this Component component) => component.gameObject.IsSceneObject();
    }
}