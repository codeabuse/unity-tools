using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Codeabuse.SceneManagement
{
    [InitializeOnLoad]
    public static class EditorSceneCompositionManager
    {
        private const string compositions_path = "Editor/Resources/EditorSceneCompositions";
        
        private static Dictionary<string, EditorSceneComposition> _savedCompositions = new();

        static EditorSceneCompositionManager()
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
            EditorHelpers.EnsureAssetsFolderCreated(compositions_path);
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

        public static Option<ISceneSetup> GetOrCreate(string name)
        {
            if (_savedCompositions.TryGetValue(name, out var composition))
                return composition;
            composition = ScriptableObject.CreateInstance<EditorSceneComposition>();
            composition.name = name;
            try
            {
                EnsureCompositionsFolderCreated();
                AssetDatabase.CreateAsset(composition, $"Assets/{compositions_path}/{composition.name}.asset");
            }
            catch (Exception e)
            {
                Debug.LogError($"Unable to create asset for scene composition '{composition.name}' due to the following exception:");
                Debug.LogException(e);
                return null;
            }
            _savedCompositions[name] = composition;
            return composition;
        }
    }
}