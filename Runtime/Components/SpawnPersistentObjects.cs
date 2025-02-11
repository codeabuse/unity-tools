using System.Collections.Generic;
using UnityEngine;

namespace Codeabuse
{
    public class SpawnPersistentObjects : MonoBehaviour
    {
        [SerializeField]
        private List<GameObject> _prefabs;

        private readonly List<GameObject> _spawnedObjects = new();

        private void Start()
        {
            Spawn();
        }

        public void Spawn()
        {
            foreach (var prefab in _prefabs)
            {
                if (!prefab)
                {
                    continue;
                }

                var instance = Instantiate(prefab);
                _spawnedObjects.Add(instance);
                DontDestroyOnLoad(instance); 
            }
        }

        public void Clear()
        {
            foreach (var spawnedObject in _spawnedObjects)
            {
                if (!spawnedObject)
                    continue;
                Destroy(spawnedObject);
            }
            _spawnedObjects.Clear();
        }
    }
}