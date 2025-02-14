using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Codeabuse.EditorTools
{
    public class BakeStaticMeshWindow : EditorWindow
    {
        private SkinnedMeshRenderer _targetSkinnedMeshRenderer;
        private Button _bakeButton;

        [MenuItem("Window/Skinned Mesh Baking")]
        private static void ShowWindow()
        {
            var window = GetWindow<BakeStaticMeshWindow>();
            window.titleContent = new GUIContent("Bake Static Mesh");
            window.Show();
        }

        private void CreateGUI()
        {
            var targetMeshField = new ObjectField("Target Mesh")
            {
                objectType = typeof(SkinnedMeshRenderer)
            };
            targetMeshField.RegisterValueChangedCallback(OnTargetMeshChanged);
            _bakeButton = new Button(OnBakeClicked) { text = "Bake" };
            _bakeButton.SetEnabled(false);
            var row = new VisualElement()
            {
                style =
                {
                    flexDirection = FlexDirection.Row
                }
            };
            row.Add(targetMeshField);
            row.Add(_bakeButton);
            rootVisualElement.Add(row);
        }

        private void OnTargetMeshChanged(ChangeEvent<Object> evt)
        {
            if (evt.newValue is not SkinnedMeshRenderer smr)
                return;
            _targetSkinnedMeshRenderer = smr;
            _bakeButton.SetEnabled((bool)smr.sharedMesh);
        }

        private void OnBakeClicked()
        {
            if (!_targetSkinnedMeshRenderer)
                return;
            var mesh = MeshTools.BakeStatic(_targetSkinnedMeshRenderer);
            var newName = $"Static {_targetSkinnedMeshRenderer.gameObject.name}";
            MeshTools.SaveMesh(mesh, newName);
        }
    }
}