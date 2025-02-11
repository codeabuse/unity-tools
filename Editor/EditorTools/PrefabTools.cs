using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace Codeabuse.EditorTools
{
    public static class PrefabTools
    {
        public static readonly ConvertToPrefabInstanceSettings PreserveAllOverrides = new ()
        {
                changeRootNameToAssetName = true,
                componentsNotMatchedBecomesOverride = true,
                gameObjectsNotMatchedBecomesOverride = true,
                logInfo = false,
                objectMatchMode = ObjectMatchMode.ByHierarchy,
                recordPropertyOverridesOfMatches = true
        };
        
        public static T InstantiatePreservingPrefabConnection<T>(T source, Scene scene) where T : UnityEngine.Object
        {
            var soruceGameObject = ToGameObject(source);
            
            if (!soruceGameObject)
                throw new Exception($"{source.name} is not a GameObject or Component!");
            
            if (PrefabUtility.IsPartOfAnyPrefab(source) && !soruceGameObject.IsSceneObject())
            {
                return PrefabUtility.InstantiatePrefab(source, scene) as T;
            }

            if (PrefabUtility.GetPrefabInstanceStatus(source) is PrefabInstanceStatus.Connected)
            {
                var instance = Object.Instantiate(source);

                var prefab = PrefabUtility.GetCorrespondingObjectFromOriginalSource(PrefabUtility.GetNearestPrefabInstanceRoot(source));
                PrefabUtility.ConvertToPrefabInstance(
                        ToGameObject(instance),
                        ToGameObject(prefab),
                        PreserveAllOverrides, 
                        InteractionMode.AutomatedAction);
                
                var mods = PrefabUtility.GetPropertyModifications(source);
                PrefabUtility.SetPropertyModifications(instance, mods);
                return instance;
            }

            return Object.Instantiate(source);
        }

        public static bool IsGameObjectOrComponent<T>(T obj) where T : Object
        {
            return obj is GameObject || typeof(Component).IsAssignableFrom(typeof(T));
        }

        public static GameObject ToGameObject(Object obj)
        {
            if (obj is GameObject go) 
                return go;
            if (obj is Component component)
                return component.gameObject;
            return null;
        }
    }
}