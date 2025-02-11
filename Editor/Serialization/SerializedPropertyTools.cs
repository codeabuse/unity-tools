using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace Codeabuse
{
    public static class SerializedPropertyTools
    {
        [InitializeOnLoadMethod]
        [MenuItem("Tools/Clear cached properties field info")]
        static void ClearCache()
        {
            PropertyCache<FieldInfo>.Clear();
        }

        private delegate FieldInfo FieldInfoAndStaticType(SerializedProperty aProperty, out Type aType);
        private static FieldInfoAndStaticType _getFieldInfoAndStaticType;
        private const string target_type_name = "ScriptAttributeUtility";
        private const string target_function_name = "GetFieldInfoAndStaticTypeFromProperty";

        private static FieldInfoAndStaticType CreateGetInfoAndStaticTypeDelegate()
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (type.Name == target_type_name)
                    {
                        var methodInfo = type.GetMethod(target_function_name, 
                            BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

                        if (methodInfo != null)
                            return (FieldInfoAndStaticType)Delegate.CreateDelegate(typeof(FieldInfoAndStaticType),
                                    methodInfo);
                        else
                            Debug.LogError($"Unable to find a method with name ");
                        break;
                    }
                }
            }

            return null;
        }

        public static int GetPropertyPathHashcode(this SerializedProperty property)
        {
            var propertyPathIndexNeutralized = property.propertyPath;
            int start = -1;
            while ((start = propertyPathIndexNeutralized.IndexOf(".Array.data", StringComparison.Ordinal)) != -1)
            {
                var end = propertyPathIndexNeutralized.IndexOf("]", StringComparison.Ordinal) +1;
                propertyPathIndexNeutralized = propertyPathIndexNeutralized.Replace(propertyPathIndexNeutralized[start..end], "$Array$");
            }
            return propertyPathIndexNeutralized.GetHashCode();
        }

        public static bool GetFieldInfoAndStaticType(this SerializedProperty property, 
                out FieldInfo fieldInfo,
                out Type type)
        {
            var key = property.GetPropertyPathHashcode();
            
            if (PropertyCache<FieldInfo>.TryGetValue(key, out fieldInfo))
            {
                type = fieldInfo.FieldType;
                return true;
            }
            
            _getFieldInfoAndStaticType ??= CreateGetInfoAndStaticTypeDelegate();
            
            if (_getFieldInfoAndStaticType is not null)
            {
                fieldInfo = _getFieldInfoAndStaticType(property, out type);
                PropertyCache<FieldInfo>.Set(key, fieldInfo);
                return true;
            }

            fieldInfo = null;
            type = null;
            Debug.LogError("GetFieldInfoAndStaticType::Reflection failed!");
            return false;
        }
        
        public static FieldInfo GetFieldInfoAndStaticType(this SerializedProperty property)
        {
            return property.GetFieldInfoAndStaticType(out var fieldInfo, out _) ? 
                    fieldInfo : 
                    null;
        }
 
        public static bool HasAttribute<T>(this SerializedProperty property, out T attribute) where T : Attribute
        {
            var info = property.GetFieldInfoAndStaticType();
            attribute = info?.GetCustomAttribute<T>();
            return attribute is { };
        }
        
        
        public static bool TryGetSerializedObjectFromProperty(
            string propertyPath, 
            SerializedProperty property, 
            out SerializedObject targetObject)
        {
            var otherObjectProperty = property.FindPropertyRelative(propertyPath);
            targetObject = otherObjectProperty?.propertyType is SerializedPropertyType.ObjectReference?
                new SerializedObject(otherObjectProperty.objectReferenceValue) : 
                null;
            return targetObject is not null;
        }

        public static bool TryGetStringListFromProperty(SerializedProperty property, List<string> result)
        {
            if (!property.IsArrayElementOfType(typeof(string)))
            {
                Debug.LogError($"Property '{property.propertyPath}' is not an array of string");
                return false;
            }
            
            for (var i = 0; i < property.arraySize; i++)
            {
                result.Add(property.GetArrayElementAtIndex(i).stringValue);
            }
            return true;
        }

        public static bool ValidatePropertyType(SerializedProperty property, SerializedPropertyType propertyType, Type fieldType = null)
        {
            if (property.propertyType != propertyType)
                return false;
            if (fieldType is null)
                return true;
            var propertyFieldInfo = property.GetFieldInfoAndStaticType();
            return propertyFieldInfo.FieldType.IsSubclassOf(fieldType);
        }

        public static bool IsOfType(this SerializedProperty property, Type type)
        {
            return PropertyTypeStringEquals(property.type, type);
        }

        public static bool IsArrayElementOfType(this SerializedProperty property, Type type)
        {
            return property.isArray && PropertyTypeStringEquals(property.arrayElementType, type);
        }

        public static bool GetContainingArray(this SerializedProperty property, out SerializedProperty arrayProperty)
        {
            var propertyPath = property.propertyPath;
            if (!propertyPath.Contains('['))
            {
                arrayProperty = null;
                return false;
            }
            var path = propertyPath.Remove(propertyPath.LastIndexOf(".Array", StringComparison.InvariantCulture));
            arrayProperty = property.serializedObject.FindProperty(path);
            return arrayProperty is { };
        }
        
        private static bool PropertyTypeStringEquals(string propertyType, Type type)
        {
            var sb = new StringBuilder(propertyType);
            var strippedName = SanitizeTypeName(sb).ToString();
            return strippedName.Equals(type.Name, StringComparison.InvariantCultureIgnoreCase);
        }

        private static StringBuilder SanitizeTypeName(StringBuilder sb)
        {
            sb.Replace("PPtr<$", "").Replace(">", "");
            return sb;
        }
        private static string SanitizeTypeName(string typeName)
        {
            return typeName.Replace("PPtr<$", "").Replace(">", "");
        }

        public static SerializedObject GetSerializedObjectFromPropertyPath(SerializedProperty property, string targetPropertyName)
        {
            var targetProperty = 
                property.FindPropertyRelative(targetPropertyName) ?? 
                property.FindPropertyNearby(targetPropertyName);
            var targetObject = targetProperty?.objectReferenceValue;
            return targetObject != null ? new SerializedObject(targetProperty.objectReferenceValue) : null;
        }

        public static SerializedProperty FindPropertyNearby(this SerializedProperty property, string nearbyPropertyPath)
        {
            var path = property.propertyPath;
            var absolutePath = path.Remove(path.LastIndexOf('.') + 1);
            absolutePath += nearbyPropertyPath;
            var result = property.serializedObject.FindProperty(absolutePath);
            return result;
        }

        public static object GetPropertyOwnerObject(this SerializedProperty property)
        {
            var depth = property.depth;
            if (depth == 0)
                return property.serializedObject.targetObject;
            var propertyPath = property.propertyPath;
            const string arrayPath = ".Array.data";
            var isArrayElement = propertyPath.EndsWith(arrayPath);
            if (isArrayElement)
            {
                propertyPath = propertyPath.Remove(propertyPath.LastIndexOf(arrayPath, StringComparison.InvariantCulture));
            }

            var hostObjectPropertyPath = propertyPath;
            var lastDotIndex = propertyPath.LastIndexOf('.');
            if (lastDotIndex != -1)
                hostObjectPropertyPath = propertyPath.Remove(lastDotIndex);
            var targetProperty = property.serializedObject.FindProperty(hostObjectPropertyPath);
            return targetProperty.isArray? GetPropertyOwnerObject(targetProperty) : targetProperty.GetValueBoxed();
        }

        public static object GetValueBoxed(this SerializedProperty property) => property.propertyType switch
        {
            SerializedPropertyType.Generic => property.GetGenericValue(),
            SerializedPropertyType.Integer => property.intValue,
            SerializedPropertyType.Boolean => property.boolValue,
            SerializedPropertyType.Float => property.floatValue,
            SerializedPropertyType.String => property.stringValue,
            SerializedPropertyType.Color => property.colorValue,
            SerializedPropertyType.ObjectReference => property.objectReferenceValue,
            SerializedPropertyType.LayerMask => property.intValue,
            SerializedPropertyType.Enum => Enum.GetValues(property.GetFieldInfoAndStaticType().FieldType).GetValue(property.enumValueIndex),
            SerializedPropertyType.Vector2 => property.vector2Value,
            SerializedPropertyType.Vector3 => property.vector3Value,
            SerializedPropertyType.Vector4 => property.vector4Value,
            SerializedPropertyType.Rect => property.rectValue,
            SerializedPropertyType.ArraySize => property.intValue,
            SerializedPropertyType.Character => (char)property.intValue,
            SerializedPropertyType.AnimationCurve => property.animationCurveValue,
            SerializedPropertyType.Bounds => property.boundsValue,
            SerializedPropertyType.Gradient => throw new NotSupportedException($"{SerializedPropertyType.Gradient} is not supported"),
            SerializedPropertyType.Quaternion => property.quaternionValue,
            SerializedPropertyType.ExposedReference => property.exposedReferenceValue,
            SerializedPropertyType.FixedBufferSize => property.fixedBufferSize,
            SerializedPropertyType.Vector2Int => property.vector2IntValue,
            SerializedPropertyType.Vector3Int => property.vector3IntValue,
            SerializedPropertyType.RectInt => property.rectIntValue,
            SerializedPropertyType.BoundsInt => property.boundsIntValue,
            SerializedPropertyType.ManagedReference => property.managedReferenceValue,
            SerializedPropertyType.Hash128 => property.hash128Value,
            _ => throw new ArgumentOutOfRangeException()
        };

        public static object GetGenericValue(this SerializedProperty property)
        {
            var pathSubstrings = property.propertyPath.Split('.');
            var target = property.serializedObject.targetObject;
            object result = target;
            var targetType = target.GetType();
            for (var index = 0; index < pathSubstrings.Length; index++)
            {
                var targetPropertyPath = pathSubstrings[index];
                if (targetPropertyPath == "Array" && index + 1 < pathSubstrings.Length &&
                    pathSubstrings[index + 1].Contains("data"))
                {
                    var dataIndexSubstring = pathSubstrings[index + 1];
                    var indexString = dataIndexSubstring.Replace("data[", "").Replace("]", "");
                    Int32.TryParse(indexString, out var indexInList);
                    switch (result)
                    {
                        case IList list:
                            result = list[indexInList];
                            targetType = result.GetType();
                            index++;
                            continue;
                    }
                }
                var fieldInfo = targetType.GetField(targetPropertyPath, BindingFlags.Instance | BindingFlags.NonPublic);
                result = fieldInfo.GetValue(result);
                targetType = result.GetType();
            }

            return result;
        }
        
        public static T GetAttributeFromProperty<T>(this SerializedProperty property) where T : Attribute
        {
            var info = property.GetFieldInfoAndStaticType();
            return info?.GetCustomAttribute<T>();
        }

        public static void CopyFrom(this SerializedProperty target, SerializedProperty source)
        {
            if (target.propertyType != source.propertyType)
            {
                Debug.LogError($"Attempt to copy SerializedProperty value from different property type!\n" 
                    + $"Target: {target.propertyPath}, source: {source.propertyPath}", 
                    target.serializedObject.targetObject);
                return;
            }

            if (target.propertyType is SerializedPropertyType.Generic &&
                target.type != source.type)
            {
                Debug.LogError($"Attempt to copy SerializedProperty value from generic property of a different type!\n"
                    + $"Target: {target.propertyPath}, source: {source.propertyPath}",
                    target.serializedObject.targetObject);
            }
            switch (target.propertyType)
            {
                case SerializedPropertyType.Generic:
                    var tempTarget = target.Copy();
                    var tempSource = source.Copy();
                    var endOfChildrenIteration = tempTarget.GetEndProperty();
                    while (tempTarget.NextVisible(true) && !SerializedProperty.EqualContents(tempTarget, endOfChildrenIteration))
                    {
                        if (tempTarget == target) return;
                        tempSource.Next(true);
                        tempTarget.CopyFrom(tempSource);
                    }
                    target.serializedObject.Update();
                    break;
                case SerializedPropertyType.Integer:
                    target.intValue = source.intValue;
                    break;
                case SerializedPropertyType.Boolean:
                    target.boolValue = source.boolValue;
                    break;
                case SerializedPropertyType.Float:
                    target.floatValue = source.floatValue;
                    break;
                case SerializedPropertyType.String:
                    target.stringValue = source.stringValue;
                    break;
                case SerializedPropertyType.Color:
                    target.colorValue = source.colorValue;
                    break;
                case SerializedPropertyType.ObjectReference:
                    target.objectReferenceValue = source.objectReferenceValue;
                    break;
                case SerializedPropertyType.LayerMask:
                    target.intValue = source.intValue;
                    break;
                case SerializedPropertyType.Enum:
                    target.enumValueIndex = source.enumValueIndex;
                    break;
                case SerializedPropertyType.Vector2:
                    target.vector2Value = source.vector2Value;
                    break;
                case SerializedPropertyType.Vector3:
                    target.vector3Value = source.vector3Value;
                    break;
                case SerializedPropertyType.Vector4:
                    target.vector4Value = source.vector4Value;
                    break;
                case SerializedPropertyType.Rect:
                    target.rectValue = source.rectValue;
                    break;
                case SerializedPropertyType.ArraySize:
                    target.intValue = source.intValue;
                    break;
                case SerializedPropertyType.Character:
                    target.intValue = source.intValue;
                    break;
                case SerializedPropertyType.AnimationCurve:
                    target.animationCurveValue = source.animationCurveValue;
                    break;
                case SerializedPropertyType.Bounds:
                    target.boundsValue = source.boundsValue;
                    break;
                case SerializedPropertyType.Gradient:
                    throw new Exception($"Copying {SerializedPropertyType.Gradient} is not supported");
                case SerializedPropertyType.Quaternion:
                    target.quaternionValue = source.quaternionValue;
                    break;
                case SerializedPropertyType.ExposedReference:
                    target.exposedReferenceValue = source.exposedReferenceValue;
                    break;
                case SerializedPropertyType.FixedBufferSize:
                    throw new Exception($"Copying {SerializedPropertyType.FixedBufferSize} is not supported");
                case SerializedPropertyType.Vector2Int:
                    target.vector2IntValue = source.vector2IntValue;
                    break;
                case SerializedPropertyType.Vector3Int:
                    target.vector3IntValue = source.vector3IntValue;
                    break;
                case SerializedPropertyType.RectInt:
                    target.rectIntValue = source.rectIntValue;
                    break;
                case SerializedPropertyType.BoundsInt:
                    target.boundsIntValue = source.boundsIntValue;
                    break;
                case SerializedPropertyType.ManagedReference:
                    target.managedReferenceValue = source.managedReferenceValue;
                    break;
                case SerializedPropertyType.Hash128:
                    target.hash128Value = source.hash128Value;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            target.serializedObject.ApplyModifiedProperties();
        }
    }
}