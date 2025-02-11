using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine.UIElements;

namespace Codeabuse.SceneManagement
{
    [CustomPropertyDrawer(typeof(BuildScene))]
    public class BuildSceneDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var dropdown = new DropdownField(){ label = "Build Scene" };
            var choices = new List<string>();
            
            choices.AddRange(EditorBuildSettings.scenes.Select(
                    ebss => Path.GetFileNameWithoutExtension(ebss.path)));
            
            dropdown.choices = choices;
            var nameProperty = property.FindPropertyRelative("_name");
            var indexProperty = property.FindPropertyRelative("_buildId");
            var guidProperty = property.FindPropertyRelative("_guid");
            
            if (string.IsNullOrEmpty(guidProperty.stringValue))
            {
                guidProperty.stringValue = GuidFromSceneIndex(indexProperty.intValue);
                property.serializedObject.ApplyModifiedProperties();
            }
            
            var sceneName = nameProperty.stringValue;
            if (choices.Contains(sceneName))
            {
                dropdown.index = choices.IndexOf(sceneName);
            }
            else
            {
                var guid = guidProperty.stringValue;
                var match = EditorBuildSettings.scenes.FirstOrDefault(
                        ebss => ebss.guid.ToString() == guid);
                
                if (match is { })
                {
                    nameProperty.stringValue = Path.GetFileNameWithoutExtension(match.path);
                }
                else
                {
                    dropdown.value = "Scene is missing!";
                }
            }
            
            dropdown.RegisterValueChangedCallback(evt =>
            {
                nameProperty.stringValue = evt.newValue;
                indexProperty.intValue = dropdown.index;
                guidProperty.stringValue = GuidFromSceneIndex(dropdown.index);
                property.serializedObject.ApplyModifiedProperties();
            });
            
            return dropdown;
        }

        private static string GuidFromSceneIndex(int index)
        {
            return EditorBuildSettings.scenes.Length <= index ? 
                    string.Empty : 
                    EditorBuildSettings.scenes[index].guid.ToString();
        }
    }
}