using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Codeabuse
{
    [CustomPropertyDrawer(typeof(MaskAttribute))]
    public class MaskPropertyDrawer : PropertyDrawer
    {
        private static readonly Dictionary<int, List<string>> generated_names_lists = new();
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var layersCount = ((MaskAttribute)attribute).LayersCount;
            
            if (!generated_names_lists.TryGetValue(layersCount, out var choices))
            {
                generated_names_lists.Add(layersCount, choices = CreateChoicesNames(layersCount));
            }
            
            var maskField = new MaskField(property.displayName, choices, 0)
            {
                bindingPath = property.propertyPath
            };
            
            return maskField;
        }

        private static List<string> CreateChoicesNames(int layersCount)
        {
            var list = new List<string>();
            for (var i = 0; i < layersCount; i++)
            {
                list.Add(i.ToString());
            }
            return list;
        }
    }
}