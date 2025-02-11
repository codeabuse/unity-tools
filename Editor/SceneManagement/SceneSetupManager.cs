using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Codeabuse.EditorTools
{
    [InitializeOnLoad]
    public static class SceneSetupManager
    {
        private const string compositions_relative_folder = "Resources/SceneCompositions";
        private static string compositions_path;
        
        private static Dictionary<string, EditorSceneComposition> _savedCompositions = new();

        static SceneSetupManager()
        {
            EditorApplication.update -= OnFirstUpdate;
            EditorApplication.update += OnFirstUpdate;
        }

        private static void OnFirstUpdate()
        {
            EditorApplication.update -= OnFirstUpdate;
            EnsureCompositionsFolderCreated();
        }

        private static void EnsureCompositionsFolderCreated()
        {
            if (!EditorHelpers.TryGetFirstAssetPath(
                        $"Script {nameof(SceneSetupManager)}",
                        out var scriptLocation))
            {
                scriptLocation = "Assets/Editor/";
            }
            else scriptLocation = scriptLocation.Remove(scriptLocation.LastIndexOf('/'));

            EditorHelpers.EnsureAssetsFolderCreated(compositions_path = $"{scriptLocation}/{compositions_relative_folder}");
        }

        public static IEnumerable<ISceneSetup> GetCompositions()
        {
            if (_savedCompositions.Count == 0)
            {
                LoadSceneCompositions();
            }
            return _savedCompositions.Values;
        }

        private static void LoadSceneCompositions()
        {
            _savedCompositions = Resources.LoadAll<EditorSceneComposition>("SceneCompositions").ToDictionary(comp => comp.name);
        }

        public static ISceneSetup GetOrCreate(string name)
        {
            if (_savedCompositions.TryGetValue(name, out var composition))
                return composition;
            composition = ScriptableObject.CreateInstance<EditorSceneComposition>();
            composition.name = name;
            try
            {
                EnsureCompositionsFolderCreated();
                AssetDatabase.CreateAsset(composition, $"{compositions_path}/{composition.name}.asset");
            }
            catch (Exception e)
            {
                Debug.LogError($"Unable to create asset for scene composition '{composition.name}' due to following exception:");
                Debug.LogException(e);
            }
            _savedCompositions[name] = composition;
            return composition;
        }
    }
}