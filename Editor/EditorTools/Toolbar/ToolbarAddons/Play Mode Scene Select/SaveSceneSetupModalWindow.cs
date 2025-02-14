using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Codeabuse.EditorTools
{
    public class SaveSceneSetupModalWindow : EditorWindow
    {
        private VisualElement _scenesRoot;
        private TextField _setupNameField;

        /// <summary>
        /// <returns>A a user submitted name for the saved setup, or None if cancelled. </returns>
        /// </summary>
        public event Action<Option<string>> OnSaveDialogClosed;

        public void ShowLoadedScenes(IEnumerable<string> sceneNames)
        {
            _scenesRoot.Clear();
            foreach (var sceneName in sceneNames)
            {
                _scenesRoot.Add(new Label(sceneName));
            }
        }

        private void OnEnable()
        {
            this.minSize = maxSize = new( 550, 400 );
            this.name = "Save current scenes setup";
        }

        private void OnDisable()
        {
            OnSaveDialogClosed = null;
        }

        private void CreateGUI()
        {
            var container = new VisualElement()
            {
                    style =
                    {
                            paddingBottom = 4,
                            paddingLeft = 4,
                            paddingRight = 4,
                            paddingTop = 4,
                            flexGrow = 1
                    }
            };
            rootVisualElement.Add(container);
            container.Add(new VisualElement(){style = { height = 20}});
            _scenesRoot = new ScrollView(ScrollViewMode.Vertical)
            {
                    verticalScrollerVisibility = ScrollerVisibility.AlwaysVisible,
                    style = { flexGrow = 1}
            };
            container.Add(_scenesRoot);
            var textArea = new VisualElement() { style =
            {
                    flexDirection = FlexDirection.Row,
            } };
            _setupNameField = new TextField("New scene setup name"){style = { flexGrow = 1}};
            var saveButton = new Button(Save) { text = "Save" };
            var cancelButton = new Button(CancelSave) { text = "Cancel" };
            textArea.Add(_setupNameField);
            textArea.Add(saveButton);
            textArea.Add(cancelButton);
            container.Add(textArea);
            //_setupNameField.RegisterCallback<InputEvent>(OnTextSubmit);
            // container.RegisterCallback<KeyUpEvent>(evt =>
            // {
            //     Debug.Log($"ROOT key: {evt.keyCode}\n" + 
            //               $"bubblesUp: {evt.bubbles} tricklesDown: {evt.tricklesDown}\n"+
            //               $"target: {evt.target} current target: {evt.currentTarget}");
            // });
            // _setupNameField.RegisterCallback<KeyUpEvent>(evt =>
            // {
            //     Debug.Log($"text field key: {evt.keyCode}\n" + 
            //               $"bubblesUp: {evt.bubbles} tricklesDown: {evt.tricklesDown}\n"+
            //               $"target: {evt.target} current target: {evt.currentTarget}");
            // });
            // _setupNameField.RegisterCallback<FocusOutEvent>(OnFoucsOut);
        }

        private void OnFoucsOut(FocusOutEvent evt)
        {
            Debug.Log($"text field focus out");
        }

        private void Save()
        {
            var setupName = _setupNameField.text.Trim();
            if (string.IsNullOrEmpty(setupName) || PathTools.EscapeForbidden.IsMatch(setupName))
            {
                Debug.LogWarning("File name must not be empty or contain forbidden characters");
                return;
            }
            
            OnSaveDialogClosed?.Invoke(setupName);
            this.Close();
        }

        private void CancelSave()
        {
            OnSaveDialogClosed?.Invoke(Option<string>.None);
            this.Close();
        }
    }
}