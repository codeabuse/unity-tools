using System;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Codeabuse.SceneManagement
{
    public class BuildSceneSetup : ISceneSetup, IEquatable<BuildSceneSetup>
    {
        public event Action<ISceneSetup> OnEditorUpdated;
        
        private readonly EditorBuildSettingsScene _ebss;

        private string _name;

        public string name
        {
            get => _name ??= _ebss is { } ? Path.GetFileNameWithoutExtension(_ebss.path) : string.Empty;
            set
            {
                if (_ebss is null)
                    throw new Exception($"Attempted to rename {nameof(BuildSceneSetup)} with invalid internal {nameof(EditorBuildSettingsScene)}");
                _name = null;
                AssetDatabase.RenameAsset(_ebss.path, value);
            }
        }

        public int Id => _ebss.guid.GetHashCode();

        public BuildSceneSetup(EditorBuildSettingsScene editorBuildSettingsScene)
        {
            this._ebss = editorBuildSettingsScene;
        }

        public void Save()
        {
            //NOP
        }

        Object ISceneSetup.GetUnderlyingObject()
        {
            return _ebss is null ? null : AssetDatabase.LoadAssetAtPath<Object>(_ebss.path);
        }

        public void Load()
        {
            if (!SceneExists()) 
                return;
            EditorSceneManager.OpenScene(AssetDatabase.GUIDToAssetPath(_ebss.guid));
        }

        private bool SceneExists()
        {
            var fullPath = Path.Combine(
                    Application.dataPath.Remove(Application.dataPath.LastIndexOf('/')), 
                    AssetDatabase.GUIDToAssetPath(_ebss.guid));
            return File.Exists(fullPath);
        }

        public bool Equals(BuildSceneSetup other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(_ebss.guid, other._ebss.guid);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((BuildSceneSetup)obj);
        }

        public override int GetHashCode()
        {
            return (_ebss != null ? _ebss.GetHashCode() : 0);
        }
    }
}