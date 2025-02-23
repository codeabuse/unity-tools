using UnityEngine;

namespace Codeabuse
{
    public static class CurveFunctions
    {
        public static float Linear(float x) => x;
        public static float Quadratic(float x) => Mathf.Pow(x, 2);
        public static float Cubic(float x) => Mathf.Pow(x, 3);
        public static float Quintic(float x) => Mathf.Pow(x, 4);
        public static float InverseQuadratic(float x) => 1-Mathf.Pow(x, 2);
        public static float InverseCubic(float x) => 1-Mathf.Pow(x, 3);
        public static float InverseQuintic(float x) => 1-Mathf.Pow(x, 4);
    }
}