using UnityEngine;

namespace Codeabuse.UI
{
    [System.Serializable]
    public struct TintColors
    {
        [SerializeField]
        private Color _normal;
        [SerializeField]
        private Color _hover;
        [SerializeField]
        private Color _pressed;
    }
}