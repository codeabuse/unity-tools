﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITASK_ENABLED
using Cysharp.Threading.Tasks;
#endif

namespace Codeabuse.SceneManagement
{
    [CreateAssetMenu(fileName = "New Runtime Scene Composition", menuName = "Scene Management/Runtime Scene Composition")]
    public class SceneComposition : ScriptableObject
    {
        [SerializeField]
        private BuildScene[] _scenes;

        public IReadOnlyList<BuildScene> Scenes => _scenes;

        public void Load()
        {
            if (_scenes.Length == 0)
            {
                Debug.LogError($"Scene composition '{name}' contains no scenes", this);
                return;
            }

            SceneManager.LoadScene(_scenes[0].Name);
            if (_scenes.Length == 1)
                return;
            
            for (var i = 1; i < _scenes.Length; i++)
            {
                SceneManager.LoadScene(_scenes[i].Name, LoadSceneMode.Additive);
            }
        }

#if UNITASK_ENABLED
        public async UniTask LoadAsync()
        {
            if (_scenes.Length == 0)
            {
                Debug.LogError("No scenes!");
                return;
            }

            await SceneManager.LoadSceneAsync(_scenes[0].Name);
            if (_scenes.Length == 1)
                return;
            
            for (var i = 1; i < _scenes.Length; i++)
            {
                await SceneManager.LoadSceneAsync(_scenes[i].Name, LoadSceneMode.Additive);
            }
        }
#endif

        public static SceneComposition Create(IEnumerable<BuildScene> buildScenes)
        {
            var composition = CreateInstance<SceneComposition>();
            composition._scenes = buildScenes.ToArray();
            return composition;
        }
    }
}