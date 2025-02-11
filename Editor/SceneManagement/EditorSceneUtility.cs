using System.Collections.Generic;
using UnityEditor;

namespace Codeabuse.EditorTools
{
    public static class EditorSceneUtility
    {
        private static readonly Dictionary<string, SceneAsset> sceneAssetsLoaded = new();

        [MenuItem("Scene/Clean cached scene assets")]
        public static void CleanCachedSceneAssets()
        {
            sceneAssetsLoaded.Clear();
        }
        
        public static SceneAsset GetSceneAssetByName(string sceneName)
        {
            foreach (EditorBuildSettingsScene buildScene in EditorBuildSettings.scenes)
            {
                SceneAsset sceneAsset = LoadSceneAsset(buildScene.path);
                if (sceneAsset!= null && sceneAsset.name == sceneName)
                {
                    return sceneAsset;
                }
            }
            return null;
        }

        private static SceneAsset LoadSceneAsset(string path)
        {
            if (sceneAssetsLoaded.TryGetValue(path, out var sceneAsset)) 
                return sceneAsset;
            
            sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(path);
            sceneAssetsLoaded.Add(path, sceneAsset);
            return sceneAsset;
        }
    }
}