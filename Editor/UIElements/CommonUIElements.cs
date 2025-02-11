using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace Codeabuse
{
    public static class CommonUIElements
    {
        public static VisualElement RowContainer => new (){ style = { flexDirection = FlexDirection.Row, flexGrow = 1} };
        public static VisualElement GrowContainer => new() { style = { flexGrow = 1 } };

        public static VisualElement CreateContainer(FlexDirection direction, uint flexGrow = 0) =>
            new() { style = { flexDirection = direction, flexGrow = flexGrow } };

        public static VisualElement CreateDisabledClickablePropertyField(
            SerializedProperty serializedProperty,
            Action<Object> doubleClickAction,
            out PropertyField underlyingField)
        {
            var propertyField = new PropertyField(serializedProperty);
            propertyField.SetEnabled(false);
            var clickableContainer = new VisualElement();
            clickableContainer.Add(propertyField);
            clickableContainer.RegisterCallback<MouseDownEvent>(evt =>
            {
                if (!serializedProperty.objectReferenceValue) 
                    return;
                if (evt.ctrlKey || evt.altKey || evt.shiftKey)
                    return;
                EditorGUIUtility.PingObject(serializedProperty.objectReferenceValue);
                if (evt.clickCount == 2)
                    doubleClickAction?.Invoke(serializedProperty.objectReferenceValue);
            } );
            underlyingField = propertyField;
            return clickableContainer;
        }

        /// <summary>
        /// There is a bug with PropertyField for any Array/List property with custom PropertyDrawer
        /// that can freeze the inspector if PropertyDrawer contains DropdownField. 
        /// To avoid that behavior, use this method instead of a PropertyField.
        /// </summary>
        /// <param name="property">List or Array property</param>
        /// <param name="reorderable"></param>
        /// <param name="showAddRemoveFooter"></param>
        /// <param name="showFoldoutHeader"></param>
        /// <returns></returns>
        public static ListView PropertyListView(
                SerializedProperty property,
                bool reorderable,
                bool showAddRemoveFooter,
                bool showFoldoutHeader = true)
        {
            if (!property.isArray)
            {
                var message = $"{property.name} property of the {property.serializedObject.targetObject.name}" +
                              $" is not an Array or List!";
                Debug.LogError(message);
                var emptyListView = new ListView();
                emptyListView.Add(ErrorBox(message));
                return emptyListView;
            }
            var listView = new ListView
            {
                name = property.name,
                headerTitle = property.displayName,
                bindingPath = property.name,
                showFoldoutHeader = showFoldoutHeader,
                reorderable = reorderable,
                showAddRemoveFooter = showAddRemoveFooter,
                showBorder = true,
                virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight,
                makeItem = () =>
                {
                    var row = RowContainer;
                    row.Add(CreateDragHandle());
                    row.Add(new PropertyField(){style = { flexGrow = 1}});
                    return row;
                }
            };
            return listView;
        }

        private const string drag_handle_style = "unity-list-view__reorderable-handle";
        private const string drag_handle_bar_style = "unity-list-view__reorderable-handle-bar";
        public static VisualElement CreateDragHandle()
        {
            var handle = new VisualElement();
            handle.AddToClassList(drag_handle_style);
            var bar1 = new VisualElement();
            var bar2 = new VisualElement();
            bar1.AddToClassList(drag_handle_bar_style);
            bar2.AddToClassList(drag_handle_bar_style);
            handle.Add(bar1);
            handle.Add(bar2);
            return handle;
        }

        public static HelpBox InfoBox(string message)
        {
            return new HelpBox(message, HelpBoxMessageType.Info);
        }
        
        public static HelpBox ErrorBox(string message)
        {
            return new HelpBox(message, HelpBoxMessageType.Error);
        }

        public static HelpBox WarningBox(string message)
        {
            return new HelpBox(message, HelpBoxMessageType.Warning);
        }
    }
}