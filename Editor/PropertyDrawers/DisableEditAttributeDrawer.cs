using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Codeabuse
{
    [CustomPropertyDrawer(typeof(DisableEditAttribute))]
    public class DisableEditAttributeDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var propField = new PropertyField(property);
            propField.SetEnabled(false);
            return propField;
        }
    }
}