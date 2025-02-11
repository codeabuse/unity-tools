using Codeabuse.EditorTools;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Codeabuse
{
    [CustomPropertyDrawer(typeof(SceneAttribute))]
    public class SceneAttributeDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var root = new VisualElement();
            var helpBox = new HelpBox();
            root.Add(helpBox);
            helpBox.Hide();

            var stringValue = property.stringValue;
            if (property.propertyType == SerializedPropertyType.String)
            {
                SceneAsset sceneObject = AssetDatabase.LoadAssetAtPath<SceneAsset>(stringValue);
                var sceneField = new ObjectField(property.displayName)
                {
                    objectType = typeof(SceneAsset)
                };
                root.Add(sceneField);
                
                sceneField.value = sceneObject;
                sceneField.RegisterValueChangedCallback(OnSceneChanged);
                ValidateSceneObject(sceneObject);
            }
            else
            {
                helpBox.ShowMessage("Use SceneAttribute with strings.", HelpBoxMessageType.Error);
                root.Add(new PropertyField(property));
                return root;
            }
            
            void OnSceneChanged(ChangeEvent<Object> evt)
            {
                var sceneObject = evt.newValue;
                property.stringValue = ValidateSceneObject(sceneObject) ? 
                    AssetDatabase.GetAssetPath(sceneObject) : 
                    string.Empty;
                property.serializedObject.ApplyModifiedProperties();
            }

            bool ValidateSceneObject(Object sceneObject)
            {
                if (sceneObject == null && !string.IsNullOrWhiteSpace(stringValue))
                {
                    // try to load it from the build settings for legacy compatibility
                    sceneObject = EditorSceneUtility.GetSceneAssetByName(stringValue);
                }
                if (sceneObject == null && !string.IsNullOrWhiteSpace(stringValue))
                {
                    helpBox.ShowMessage($"Could not find scene {stringValue}",
                        HelpBoxMessageType.Error);
                    return false;
                }
                helpBox.Hide();
                return true;
            }
            return root;
            
        }
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType == SerializedPropertyType.String)
            {
                SceneAsset sceneObject = AssetDatabase.LoadAssetAtPath<SceneAsset>(property.stringValue);

                if (sceneObject == null && !string.IsNullOrWhiteSpace(property.stringValue))
                {
                    // try to load it from the build settings for legacy compatibility
                    sceneObject = EditorSceneUtility.GetSceneAssetByName(property.stringValue);
                }
                if (sceneObject == null && !string.IsNullOrWhiteSpace(property.stringValue))
                {
                    Debug.LogError($"Could not find scene {property.stringValue} in {property.propertyPath}, assign the proper scenes in your NetworkManager");
                }
                SceneAsset scene = (SceneAsset)EditorGUI.ObjectField(position, label, sceneObject, typeof(SceneAsset), true);

                property.stringValue = AssetDatabase.GetAssetPath(scene);
            }
            else
            {
                EditorGUI.LabelField(position, label.text, "Use [Scene] with strings.");
            }
        }
    }
}