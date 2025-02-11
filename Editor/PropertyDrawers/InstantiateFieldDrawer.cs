using System;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Codeabuse
{
    [CustomPropertyDrawer(typeof(InstantiateAttribute))]
    public class InstantiateFieldDrawer : PropertyDrawer
    {
        private const string no_value_option = "<null>";
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            PropertyField inspectedPropertyField = null;
            var root = new VisualElement();
            if (attribute is not InstantiateAttribute instantiateAttribute)
                return root;
            if (property.propertyType is not SerializedPropertyType.ManagedReference)
            {
                Debug.LogError(
                    $"{nameof(InstantiateAttribute)} is only usable with {SerializedPropertyType.ManagedReference}"+
                    " serialized field type. Use [SerializeReference] attribute");
                return root;
            }
            var concreteTypes = TypeCache.GetTypesDerivedFrom(instantiateAttribute.BaseType)
                .Where(x => !x.IsAbstract && x.GetConstructors().Any(ctor => ctor.GetParameters().Length == 0))
                .ToList();
            var concreteTypeNames = concreteTypes
                .Select(x => ObjectNames.NicifyVariableName(x.Name))
                .ToList();
            
            concreteTypeNames.Insert(0, no_value_option);
            var currentValueType = property.managedReferenceValue?.GetType();
            var currentValueIndex = currentValueType is not null ? concreteTypes.IndexOf(currentValueType) + 1 : 0;
            var dropdown = new DropdownField(property.displayName, concreteTypeNames, currentValueIndex);
            root.Add(dropdown);
            if (property.managedReferenceValue is not null)
                root.Add(inspectedPropertyField = new PropertyField(property));
            dropdown.RegisterValueChangedCallback(OnTypeSelected);
            
            void OnTypeSelected(ChangeEvent<string> evt)
            {
                var index = concreteTypeNames.IndexOf(evt.newValue) - 1;
                property.managedReferenceValue = index switch
                {
                    >= 0 when index < concreteTypes.Count => Activator.CreateInstance(concreteTypes[index]),
                    -1 => null,
                    _ => property.managedReferenceValue
                };
                property.serializedObject.ApplyModifiedProperties();
                if (property.managedReferenceValue is not null)
                {
                    root.Clear();
                    if (property.hasVisibleChildren)
                    {
                        root.Add(inspectedPropertyField = new PropertyField(property));
                        inspectedPropertyField.BindProperty(property);
                    }
                }
                else if (inspectedPropertyField is not null)
                {
                    root.Remove(inspectedPropertyField);
                    inspectedPropertyField = null;
                }
            }
            
            return root;
        }
    }
}