using System;
using UnityEngine;

namespace Codeabuse.SceneManagement
{
    [Serializable]
    public struct BuildScene
    {
        public string Name => _name;
        public int Index => _index;
        
        [SerializeField]
        private string _guid;

        [SerializeField]
        private string _name;

        [SerializeField]
        private int _index;

        public static BuildScene Create(string name, int _index, string guid)
        {
            return new BuildScene()
            {
                    _name = name,
                    _index = _index,
                    _guid = guid
            };
        }
    }
}