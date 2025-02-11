using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Codeabuse.EditorTools
{
    [Serializable]
    public class SceneSetupWithGUID : SceneSetup
    {
        [SerializeField, DisableEdit]
        private string _guid;

        public SceneSetupWithGUID() { }
        
        public SceneSetupWithGUID(SceneSetup setup)
        {
            this.path = setup.path;
            this.isActive = setup.isActive;
            this.isLoaded = setup.isLoaded;
            this.isSubScene = setup.isSubScene;

            if (AssetDatabase.GUIDFromAssetPath(setup.path) is var guid && guid != new GUID())
            {
                this._guid = guid.ToString();
            }
        }

        public void UpdateGUID()
        {
            if (AssetDatabase.GUIDFromAssetPath(path) is var guid && guid != new GUID())
            {
                this._guid = guid.ToString();
            }
        }

        public bool FixPathIfSceneRenamed()
        {
            UpdateGUID();
            var pathFromGuid = AssetDatabase.GUIDToAssetPath(_guid);
            if (!string.IsNullOrEmpty(pathFromGuid))
            {
                this.path = pathFromGuid;
                return true;
            }
            
            Debug.LogError($"Missing asset with GUID {_guid} / old path {this.path}!");
            return false;
        }
        
        public static SceneSetupWithGUID Convert(SceneSetup setup)
        {
            return new SceneSetupWithGUID(setup);
        }
    }
}