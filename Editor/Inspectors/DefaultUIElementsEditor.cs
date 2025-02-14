using System.Linq;
using Codeabuse.EditorTools;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace Codeabuse
{
    [CustomEditor(typeof(Object), true)]
    [CanEditMultipleObjects]
    public class DefaultUIElementsEditor : Editor
    {
        [SerializeField]
        private StyleSheet _editorStyles;
        
        private ContextFunction[] _contextFunctions;

        private const string script_property_name = "m_Script";
        protected virtual bool showScriptField => true;
        
        public override VisualElement CreateInspectorGUI()
        {
            var scriptPropertyPassed = false;
            var root = new VisualElement();
            var layoutRoot = root;
            var iterator = serializedObject.GetIterator();
            
            if (iterator.NextVisible(true))
            {
                do
                {
                    VisualElement propertyElement;

                    if (!scriptPropertyPassed && 
                        iterator.propertyPath.Equals(script_property_name) && 
                        serializedObject.targetObject)
                    {
                        if (!showScriptField) continue;
                        propertyElement = CommonUIElements.CreateDisabledClickablePropertyField(
                            iterator.Copy(),
                            obj => AssetDatabase.OpenAsset(obj),
                            out _);
                        
                        scriptPropertyPassed = true;
                        layoutRoot.Add(propertyElement);
                    }
                    else
                    {
                        var property = iterator.Copy();
                        propertyElement = CreatePropertyElement(property);
                        if (propertyElement is not null)
                            layoutRoot.Add(propertyElement);
                    }
                }
                while (iterator.NextVisible(false));
            }
            
            var buttonsRoot = MakeContextActionsButtons();
            root.Add(buttonsRoot);
            
            if (_editorStyles)
                root.styleSheets.Add(_editorStyles);
            
            return root;
        }

        private VisualElement MakeContextActionsButtons()
        {
            if (targets.Length > 1)
                return new VisualElement();
            
            _contextFunctions = target.GetContextFunctions().ToArray();
            
            if (_contextFunctions.Length == 0)
                return new VisualElement();
            
            var foldout = new Foldout()
            {
                    text = "Conext actions",
                    style = { unityFontStyleAndWeight = FontStyle.Bold}
            };

            foreach (var contextFunction in _contextFunctions)
            {
                var container = new VisualElement()
                {
                        style =
                        {
                                flexDirection = FlexDirection.Row,
                                alignItems = Align.Center,
                        }
                };
                var button = new Button(InvokeContextFunction)
                {
                        text = ObjectNames.NicifyVariableName(contextFunction.Label),
                        style =
                        {
                                width = new StyleLength(Length.Percent(75))
                        }
                };

                void InvokeContextFunction()
                {
                    BeforeContextActionCall(contextFunction.Label);
                    contextFunction.Invoke();
                    AfterContextActionCall();
                }
                
                container.Add(button);
                foldout.Add(container);
            }

            return foldout;
        }

        protected virtual void BeforeContextActionCall(string action)
        {
            Undo.RecordObject(target, action);
        }

        protected virtual void AfterContextActionCall()
        {
            serializedObject.Update();
        }

        /// <summary>
        /// Override this method to map particular properties to custom UIElements controls
        /// without using PropertyDrawers. Common approach is to use switch statement/expression on property name.
        /// </summary>
        /// <param name="property"></param>
        /// <returns>return null to skip drawing property</returns>
        protected virtual VisualElement CreatePropertyElement(SerializedProperty property)
        {
            return new PropertyField(property){ name = property.name };
        }
    }
}