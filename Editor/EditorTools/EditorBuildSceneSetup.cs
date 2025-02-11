using System;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace Codeabuse.EditorTools
{
    public class EditorBuildSceneSetup : ISceneSetup, IEquatable<EditorBuildSceneSetup>
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
                    throw new Exception($"Attempted to rename {nameof(EditorBuildSceneSetup)} with invalid internal {nameof(EditorBuildSettingsScene)}");
                _name = null;
                AssetDatabase.RenameAsset(_ebss.path, value);
            }
        }

        public int Id => _ebss.guid.GetHashCode();

        public EditorBuildSceneSetup(EditorBuildSettingsScene editorBuildSettingsScene)
        {
            this._ebss = editorBuildSettingsScene;
        }

        public void Save()
        {
            //NOP
        }

        public void Load()
        {
            EditorSceneManager.OpenScene(_ebss.path);
        }

        public bool Equals(EditorBuildSceneSetup other)
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
            return Equals((EditorBuildSceneSetup)obj);
        }

        public override int GetHashCode()
        {
            return (_ebss != null ? _ebss.GetHashCode() : 0);
        }
    }
}