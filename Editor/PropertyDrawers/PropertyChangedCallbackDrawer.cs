using System;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Codeabuse
{
    [CustomPropertyDrawer(typeof(PropertyChangedCallbackAttribute))]
    public class PropertyChangedCallbackDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var propertyField = new PropertyField(property);
            
            if (!FindCallbackTarget(property,
                        out var callback,
                        out var callbackTarget,
                        out var argsTypes))
            {
                return propertyField;
            }
            
            if (callback is null)
                return propertyField;
            
            propertyField.RegisterCallback<SerializedPropertyChangeEvent>(OnPropertyChanged);
            void OnPropertyChanged(SerializedPropertyChangeEvent evt)
            {
                if (Application.isPlaying)
                    return;
                
                var args = argsTypes.Length == 0
                    ? Array.Empty<object>()
                    : new [] { evt.changedProperty.GetValueBoxed() };
                
                
                if (((PropertyChangedCallbackAttribute)attribute).UpdateHierarchy)
                {
                    Undo.RegisterFullObjectHierarchyUndo(property.serializedObject.targetObject, "Hierarchy changed");
                }
                callback.Invoke(callbackTarget, args);
                evt.changedProperty.serializedObject.Update();
            }
        
            return propertyField;
        }

        /*
         * The workflow iwthout caching the reflection stuff is highly inefficient in GUI loop.
         * If one is considered to be really used, its better to get method infos mapped by property has as key
         * using SerializedPropertyTools
         */
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (Application.isPlaying)
                return;

            var propertyMmodified = EditorGUI.PropertyField(position, property, label);
            if (!propertyMmodified)
                return;

            if (!FindCallbackTarget(property,
                        out var callback,
                        out var callbackTarget,
                        out var argsTypes))
            {
                return;
            }
            
            if (callback is null)
                return;
            
            var args = argsTypes.Length == 0
                ? Array.Empty<object>()
                : new [] { property.GetValueBoxed() };

            if (((PropertyChangedCallbackAttribute)attribute).UpdateHierarchy)
            {
                Undo.RegisterFullObjectHierarchyUndo(property.serializedObject.targetObject, "Hierarchy changed");
            }
            
            callback.Invoke(callbackTarget, args);
            property.serializedObject.Update();
        }

        private bool FindCallbackTarget(SerializedProperty property, 
                out MethodInfo callback, 
                out object callbackTarget, 
                out Type[] argsTypes)
        {
            callback = default;
            callbackTarget = null;
            argsTypes = default;
            if (attribute is not PropertyChangedCallbackAttribute propertyChangedAttr)
                return false;

            callbackTarget = property.GetPropertyOwnerObject();
            var argumentType = propertyChangedAttr.ArgumentType;
            var methodName = propertyChangedAttr.MethodName;
            argsTypes = argumentType is null ? Type.EmptyTypes : new[] { argumentType };
            var callbackTargetType = callbackTarget.GetType();
            callback = callbackTargetType.GetNonPublicMethod(methodName, argsTypes);
            return true;
        }
    }
}