using UnityEngine;

namespace Codeabuse
{
    public static class MathTools
    {
        public static (float, float) MinMax(float a, float b)
        {
            return a <= b ? (a, b) : (b, a);
        }
        
        public static (double, double) MinMax(double a, double b)
        {
            return a <= b ? (a, b) : (b, a);
        }
        
        public static (int, int) MinMax(int a, int b)
        {
            return a <= b ? (a, b) : (b, a);
        }
        
        public static float UnwrapAngle(float angle)
        {
            angle %= 360;
            return angle switch
            {
                    > 180 => angle - 360,
                    < -180 => angle + 360,
                    _ => angle
            };
        }

        public static float ClampAngle(float value, float angle)
        {
            var half = angle * .5f;
            var unwrapped = UnwrapAngle(value);
            return Mathf.Clamp(unwrapped, -half, +half);
        }

        public static float ClampAngle(float value, float min, float max)
        {
            var unwrapped = UnwrapAngle(value);
            return Mathf.Clamp(unwrapped, min, max);
        }
    }
}