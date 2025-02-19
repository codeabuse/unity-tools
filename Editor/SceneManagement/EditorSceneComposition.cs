using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Codeabuse.EditorTools;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Pool;
using Object = UnityEngine.Object;

namespace Codeabuse.SceneManagement
{
    /// <summary>
    /// Manage multi-scene setups with this class. Allows to easily load multiple scenes at once.
    /// </summary>
    [CreateAssetMenu(fileName = "Scene Composition", menuName = "Scene Management/Scene Composition")]
    public class EditorSceneComposition : ScriptableObject, ISceneSetup
    {
        public event Action<ISceneSetup> OnEditorUpdated;
        
        [SerializeField]
        private List<SceneSetupWithGUID> _sceneSetups = new();

        string ISceneSetup.name
        {
            get => $"[{_sceneSetups.Count}] {base.name}";
            set => base.name = value;
        }

        public int Id => (_guid ??= AssetDatabase.GUIDFromAssetPath(AssetDatabase.GetAssetPath(this)))
               .GetHashCode();

        private GUID? _guid;

        private void Awake()
        {
            foreach (var sceneSetup in _sceneSetups)
            {
                sceneSetup.UpdateGUID();
            }
            EditorSceneCompositionManager.Register(this);
        }

        [ContextMenu("Load")]
        public void Load()
        {
            if (_sceneSetups is null or { Count: 0 })
            {
                Debug.LogError($"Scene setup contains no scenes", this);
                return;
            }

            for (var i = _sceneSetups.Count - 1; i >= 0; i--)
            {
                var setup = _sceneSetups[i];
                if (!setup.FixPathIfSceneRenamed())
                {
                    Debug.LogWarning($"Removing lost scene from setup: {setup.path}", this);
                    _sceneSetups.RemoveAt(i);
                }
            }

            var setups = Array.ConvertAll(_sceneSetups.ToArray(),
                    x => x as SceneSetup);
            EditorSceneManager.RestoreSceneManagerSetup(setups);
        }

        [ContextMenu("Save Current Composition")]
        public void Save()
        {
            Undo.RecordObject(this, $"Save Scene Composition '{name}'");
            
            var setups = EditorSceneManager.GetSceneManagerSetup();
            _sceneSetups.Clear();
            foreach (var setup in setups)
            {
                _sceneSetups.Add(new SceneSetupWithGUID(setup));
            }
            
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssetIfDirty(this);
            OnEditorUpdated?.Invoke(this);
        }

        Object ISceneSetup.GetUnderlyingObject()
        {
            return this;
        }

        [ContextMenu("Create Runtime Composition")]
        public void CreateRuntimeComposition()
        {
            var canCreate = true;
            var buildSettingsScenes = ListPool<BuildScene>.Get();
            foreach (var sceneSetup in _sceneSetups)
            {
                if (!sceneSetup.IsBuildScene(out var ebss, out var index))
                {
                    Debug.Log($"Scene '{sceneSetup.path}' is not added to the Build Settings");
                    canCreate = false;
                    continue;
                }
                buildSettingsScenes.Add(BuildScene.Create(Path.GetFileNameWithoutExtension(ebss.path),
                        index, ebss.guid.ToString()));
            }

            if (!canCreate)
            {
                goto release;
            }

            var path = EditorUtility.SaveFilePanelInProject(
                    "Save Runtime Composition",
                    "New Runtime Scene Composition",
                    "asset",
                    "");
            
            if (string.IsNullOrEmpty(path))
                goto release;
            
            try
            {
                var runtimeComposition = SceneComposition.Create(buildSettingsScenes);
                AssetDatabase.CreateAsset(runtimeComposition, path);
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
            
            release:
            ListPool<BuildScene>.Release(buildSettingsScenes);
        }

        private void OnDestroy()
        {
            OnEditorUpdated?.Invoke(this);
        }
    }
}