using UnityEngine;

namespace Codeabuse
{
    [ExecuteAlways]
    public class DisplayedName : MonoBehaviour
    {
        [SerializeField]
        private string _name;
        public string Name => _name;

        private void Awake()
        {
            if (string.IsNullOrEmpty(_name))
                _name = gameObject.name;
        }
    }
}