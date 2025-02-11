using UnityEngine;

namespace Codeabuse.AnimationTools
{
    public class SelectKeyAttribute : PropertyAttribute
    { 
        public string KeyListPropertyPath { get; }
        public string KeyStoragePropertyPath { get; }

        public SelectKeyAttribute(string keyListPropertyPath, string keyStoragePropertyPath = null)
        {
            KeyListPropertyPath = keyListPropertyPath;
            KeyStoragePropertyPath = keyStoragePropertyPath;
        }
    }
}