using UnityEngine;

namespace Codeabuse
{
    public class MaskAttribute : PropertyAttribute
    {
        public int LayersCount { get; }

        public MaskAttribute(int layersCount = 16)
        {
            LayersCount = Mathf.Clamp(layersCount, 2, 64);
        }
    }
}