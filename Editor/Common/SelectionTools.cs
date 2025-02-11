using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Codeabuse
{
    public static class SelectionTools
    {
        public enum ObjectFilter
        {
            SceneObjects,
            Prefabs,
            OtherAssets
        }

        [MenuItem("Tools/Group Selected GameObjects _#g")]
        private static void GroupSelectedGameObjectsCommand() => GroupGameObjects(Selection.gameObjects);
        
        public static GameObject GroupGameObjects(GameObject[] gameObjects)
        {
            var selectedGameObjects = FilterObjects(gameObjects, ObjectFilter.SceneObjects).ToArray();
            if (selectedGameObjects.Length == 0) 
                return null;
            return CreateCommonRoot(selectedGameObjects);
        }

        public static IEnumerable<TObject> FilterObjects<TObject>(IEnumerable<TObject> sourceCollection, ObjectFilter filter) 
            where TObject : Object
        {
            Func<TObject, bool> selector = filter switch
            {
                ObjectFilter.SceneObjects => obj => obj is GameObject go && !string.IsNullOrEmpty(go.scene.name),
                ObjectFilter.Prefabs => PrefabUtility.IsPartOfAnyPrefab,
                ObjectFilter.OtherAssets => obj => obj is not GameObject && EditorUtility.IsPersistent(obj),
                _ => throw new ArgumentOutOfRangeException(nameof(filter), filter, $"Incoming value: {(int)filter}")
            };
            
            foreach (var obj in sourceCollection)
            {
                if (selector(obj))
                    yield return obj;
            }
        }

        public static GameObject CreateCommonRoot(GameObject[] sceneObjects)
        {
            var commonParent = 
                sceneObjects.FirstOrDefault(x => x.transform.parent)?.transform.parent;
            var groupRoot = new GameObject("Group");
            GameObjectUtility.EnsureUniqueNameForSibling(groupRoot);
            if (commonParent)
            {
                groupRoot.transform.SetParent(commonParent);
                groupRoot.transform.localPosition = Vector3.zero;
            }
            else groupRoot.transform.position = VectorTools.GetAverage(sceneObjects.Select(go => go.transform.position));
            Undo.RegisterCreatedObjectUndo(groupRoot, "Group Root created");
            foreach (var sceneObject in sceneObjects)
            {
                Undo.SetTransformParent(
                    sceneObject.transform, 
                    groupRoot.transform, 
                    "Change parent of grouped object");
            }

            Selection.activeObject = groupRoot;
            return groupRoot;
        }
    }
}