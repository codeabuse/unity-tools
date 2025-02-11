using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace Codeabuse
{
    [CustomPropertyDrawer(typeof(WithInterfaceAttribute))]
    public class ObjectFieldWithInterfaceDrawer : PropertyDrawer
    {
        private const string obj_field_label_class = "unity-object-field-display__label";
        
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            if (property.propertyType is not SerializedPropertyType.ObjectReference)
            {
                return CommonUIElements.ErrorBox(
                    $"{nameof(WithInterfaceAttribute)} error: Only Object reference fields are allowed");
            }

            var fieldType = property.GetContainingArray(out var array)? 
                    array.GetFieldInfoAndStaticType().FieldType.GetCollectionElementType() : 
                    fieldInfo.FieldType;

            if (attribute is not WithInterfaceAttribute attr)
                throw new NullReferenceException("Invalid attribute");
            
            var objectField = new ObjectField(property.displayName)
            {
                objectType = fieldType,
                value = property.objectReferenceValue
            };

            var label = objectField.Q<Label>(className: obj_field_label_class);

            void FixObjectFieldLabel()
            {
                if (!property.objectReferenceValue)
                {
                    label.text = label.text.Replace(objectField.objectType.Name, attr.InterfaceType.Name);
                }
            }

            FixObjectFieldLabel();
            
            objectField.RegisterValueChangedCallback(evt =>
            {
                var newValue = evt.newValue;
                if (!newValue)
                {
                    FixObjectFieldLabel();
                }
                if (!attr.InterfaceType.IsInstanceOfType(newValue))
                {
                    objectField.value = property.objectReferenceValue;
                    return;
                }

                property.objectReferenceValue = newValue;
                property.serializedObject.ApplyModifiedProperties();
            });
            
            objectField.AddManipulator(new DragAndDropObjectsManipulator(OnDragPerformed));
            
            void OnDragPerformed(Object[] draggedObjects)
            {
                foreach (var draggedObject in draggedObjects)
                {
                    switch (draggedObject)
                    {
                        case GameObject go:
                            foreach (var component in go.GetComponents<Component>())
                            {
                                if (!TrySetComponentWithInterface(component))
                                    continue;
                                return;
                            }
                            break;
                            
                        case Component co:
                            TrySetComponentWithInterface(co);
                            break;
                    }
                }
            }

            bool TrySetComponentWithInterface(Component component)
            {
                if (attr.InterfaceType.IsInstanceOfType(component) || 
                    attr.InterfaceType.MatchGenericInterfaceImplementation(component.GetType()))
                {
                    property.objectReferenceValue = component;
                    objectField.SetValueWithoutNotify(component);
                    property.serializedObject.ApplyModifiedProperties();
                    return true;
                }

                return false;
            }
            
            
            return objectField;
        }
    }
}