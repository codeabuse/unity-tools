using System;
using UnityEditor;

namespace Codeabuse
{
    /// <summary>
    /// Stores and retrieves Project-specific values from <see cref="EditorPrefs"/>.
    /// Automatically adds product name from the current project to the key.
    /// </summary>
    public static class ProjectPrefs
    {
        public static int GetInt(string key, int defaultValue = default)
        {
            key = ProjectPrefsKeys.ToProjectSpecificKey(key);
            return EditorPrefs.GetInt(key, defaultValue);
        }

        public static void SetInt(string key, int value)
        {
            key = ProjectPrefsKeys.ToProjectSpecificKey(key);
            EditorPrefs.SetInt(key, value);
        }
        
        public static float GetFloat(string key, float defaultValue = default)
        {
            key = ProjectPrefsKeys.ToProjectSpecificKey(key);
            return EditorPrefs.GetFloat(key, defaultValue);
        }

        public static void SetFloat(string key, float value)
        {
            key = ProjectPrefsKeys.ToProjectSpecificKey(key);
            EditorPrefs.SetFloat(key, value);
        }

        public static string GetString(string key, string defaultValue = "")
        {
            key = ProjectPrefsKeys.ToProjectSpecificKey(key);
            return EditorPrefs.GetString(key, defaultValue);
        }

        public static void SetString(string key, string value)
        {
            key = ProjectPrefsKeys.ToProjectSpecificKey(key);
            EditorPrefs.SetString(key, value);
        }
        
        public static bool GetBool(string key, bool defaultValue = default)
        {
            key = ProjectPrefsKeys.ToProjectSpecificKey(key);
            return EditorPrefs.GetBool(key, defaultValue);
        }

        public static void SetBool(string key, bool value)
        {
            key = ProjectPrefsKeys.ToProjectSpecificKey(key);
            EditorPrefs.SetBool(key, value);
        }
        
        
        
        /// <summary>
        /// Only works for enums with <see cref="Int32"/> underlying type!
        /// </summary>
        /// <exception cref="ArgumentException"> thrown if the enum type is not supported</exception>
        public static TValue GetEnum<TValue>(string key, TValue defaultValue = default)
            where TValue : Enum
        {
            key = ProjectPrefsKeys.ToProjectSpecificKey(key);
            if (!typeof(TValue).GetEnumUnderlyingType().IsAssignableFrom(typeof(int)))
                throw new ArgumentException($"{typeof(TValue).Name} does not have an Int32 under it!");
            var intValue = EditorPrefs.GetInt(key, (int)(object)defaultValue);
            return (TValue)Enum.ToObject(typeof(TValue), intValue);
        }

        /// <summary>
        /// Only works for enums with <see cref="Int32"/> underlying type!
        /// </summary>
        /// <exception cref="ArgumentException"> thrown if the enum type is not supported</exception>
        public static void SetEnum<TValue>(string key, TValue value)
            where TValue : Enum
        {
            key = ProjectPrefsKeys.ToProjectSpecificKey(key);
            if (!typeof(TValue).GetEnumUnderlyingType().IsAssignableFrom(typeof(int)))
                throw new ArgumentException($"{typeof(TValue).Name} does not have an Int32 under it!");
            var intValue = (int)(object)value;
            EditorPrefs.SetInt(key, intValue);
        }
    }
}