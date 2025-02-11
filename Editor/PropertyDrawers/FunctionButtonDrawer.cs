using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Codeabuse
{
    [CustomPropertyDrawer(typeof(FunctionButtonAttribute))]
    public class FunctionButtonDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var root = CommonUIElements.RowContainer;
            root.Add(new PropertyField(property){style = { flexGrow = 1}});
            if (attribute is not FunctionButtonAttribute functionButtonAttribute)
            {
                return root;
            }
            
            if (!ReflectionTools.FindTargetMethodAndParameters(
                        functionButtonAttribute.MethodName, 
                        property, 
                        fieldInfo.DeclaringType,
                        out var buttonTarget))
            {
                root.Add(CommonUIElements.WarningBox(
                        $"Can't find method '{functionButtonAttribute.MethodName}' in type '{fieldInfo.DeclaringType!.Name}'." + 
                        $"\n The method must accept zero arguments or one argument of type '{property.type}'"));
                return root;
            }

            void OnFunctionButtonClicked()
            {
                if (functionButtonAttribute.UpdateSerializedObject)
                {
                    Undo.RecordObject(property.serializedObject.targetObject, functionButtonAttribute.ButtonText);
                }

                //targetMethod.Invoke(methodTarget, parameters);
                buttonTarget.Invoke();
                
                if (functionButtonAttribute.UpdateSerializedObject)
                {
                    property.serializedObject.Update();
                    EditorUtility.SetDirty(property.serializedObject.targetObject);
                }
            }

            var functionButton = new Button(OnFunctionButtonClicked)
            {
                text = functionButtonAttribute.ButtonText ?? ObjectNames.NicifyVariableName(functionButtonAttribute.MethodName),
                tooltip = functionButtonAttribute.Tooltip
            };
            root.Add(functionButton);
            return root;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (attribute is not FunctionButtonAttribute functionButtonAttribute)
            {
                return;
            }
            
            if (!ReflectionTools.FindTargetMethodAndParameters(
                        functionButtonAttribute.MethodName, 
                        property, 
                        fieldInfo.DeclaringType,
                        out var buttonTarget))
            {
                EditorGUI.PropertyField(position, property, label);
                return;
            }

            var buttonLabel = new GUIContent(ObjectNames.NicifyVariableName(functionButtonAttribute.MethodName));
            var buttonSize = EditorStyles.miniButton.CalcSize(buttonLabel);
            
            var fieldPosition = new Rect(position.x, position.y, position.width - buttonSize.x, position.height);
            var buttonPosition = new Rect(position.x + fieldPosition.width, position.y, buttonSize.x, position.height);
            
            EditorGUI.PropertyField(fieldPosition, property, label);
            
            if (GUI.Button(buttonPosition, buttonLabel))
            {
                buttonTarget.Invoke();
            }
        }
    }
}