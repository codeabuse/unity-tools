using System;
using UnityEngine;

namespace Codeabuse
{
    /// <summary>
    /// Allows to attach the button to the PropertyField element of a field with <see cref="SerializeField"/> attribute
    /// and invoke a method with specified name.
    /// Does support serialized generic classes and arrays/lists. Set IsListProperty to true to pass element's index to the target method.
    /// </summary>
    public class FunctionButtonAttribute : PropertyAttribute
    {
        public string MethodName { get; }
        public string ButtonText { get; }
        public bool IsListProperty { get; }
        public bool UpdateSerializedObject { get; }
        
        public string Tooltip { get; set; }

        public FunctionButtonAttribute(
            string methodName, 
            bool isListProperty = false,
            bool updateSerializedObject = true) :
            this(methodName, null, isListProperty, updateSerializedObject)
        { }

        public FunctionButtonAttribute(
            string methodName, 
            string buttonText, 
            bool isListProperty = false, 
            bool updateSerializedObject = true)
        {
            if (string.IsNullOrEmpty(methodName))
                throw new ArgumentException($"{nameof(methodName)} can't be empty!");
            MethodName = methodName;
            ButtonText = buttonText;
            IsListProperty = isListProperty;
            UpdateSerializedObject = updateSerializedObject;
        }
    }
}